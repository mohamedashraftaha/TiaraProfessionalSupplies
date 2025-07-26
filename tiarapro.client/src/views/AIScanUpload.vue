<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '../utils/Api.ts'
import JSZip from 'jszip'
import router from '../router/index.ts'
import { showErrorToast } from '../utils/helpers.ts'

// Types
interface UserSubscription {
    segmentations_used: number
    segmentations_allowed: number
    is_active: boolean
}

const file = ref<File | null>(null)
const multipleFiles = ref<File[] | null>(null)
const showConfirmation = ref(false)
const isDragging = ref(false)
const isUploading = ref(false)
const uploadError = ref<string | null>(null)
const uploadProgress = ref(0)
const toastMessage = ref('')
const toastType = ref<'success' | 'error'>('success')
const userSubscription = ref<UserSubscription | null>(null)
const isLoadingSubscription = ref(true)

// Real-time stats animation
const animatedUsedCount = ref(0)
const animatedAllowedCount = ref(0)

const segmentationsRemaining = computed(() => {
    if (!userSubscription.value) return 0
    return Math.max(0, userSubscription.value.segmentations_allowed - userSubscription.value.segmentations_used)
})

const usagePercentage = computed(() => {
    if (!userSubscription.value || userSubscription.value.segmentations_allowed === 0) return 0
    return Math.min(100, (userSubscription.value.segmentations_used / userSubscription.value.segmentations_allowed) * 100)
})

const getUsageColor = computed(() => {
    const percentage = usagePercentage.value
    if (percentage >= 90) return 'from-red-500 to-red-600'
    if (percentage >= 70) return 'from-yellow-500 to-orange-500'
    return 'from-[#1e3a8a] to-[#4052B5]'
})

const canUpload = computed(() => {
    return userSubscription.value?.is_active && segmentationsRemaining.value > 0
})

// Fetch user's subscription data
const fetchSubscriptionData = async () => {
    try {
        const userId = localStorage.getItem('userId')
        if (!userId) {
            uploadError.value = 'Please log in to continue'
            return
        }

        const response = await api.get(`/api/tiaraaisubscription/user/${userId}/active`)
        if (response.status === 200) {
            userSubscription.value = response.data

            // Animate the numbers for wow factor
            animateNumbers()
        }
    } catch (error) {
        console.error('Error fetching subscription data:', error)
        uploadError.value = 'Unable to load subscription data. Please refresh the page.'
        localStorage.setItem('tiaraAiSubscription', 'false')
        localStorage.setItem('segmentationsAllowed', '0')
        router.push({ name: 'ai' })
    } finally {
        isLoadingSubscription.value = false
    }
}

const animateNumbers = () => {
    if (!userSubscription.value) return

    const animationDuration = 1500
    const steps = 60
    const stepTime = animationDuration / steps

    let currentStep = 0
    const timer = setInterval(() => {
        currentStep++
        const progress = currentStep / steps

        animatedUsedCount.value = Math.floor(userSubscription.value!.segmentations_used * progress)
        animatedAllowedCount.value = Math.floor(userSubscription.value!.segmentations_allowed * progress)

        if (currentStep >= steps) {
            clearInterval(timer)
            animatedUsedCount.value = userSubscription.value!.segmentations_used
            animatedAllowedCount.value = userSubscription.value!.segmentations_allowed
        }
    }, stepTime)
}

onMounted(async () => {
    debugger;
    await fetchSubscriptionData()
})

const handleFileUpload = (event: Event) => {
    const input = event.target as HTMLInputElement
    if (input.files && input.files.length > 0) {
        const uploadedFiles = Array.from(input.files)

        // Accept .nii, .nii.gz, .dcm, .nrrd, and .zip
        const allowedExtensions = [
            '.nii', '.nii.gz', '.dcm', '.nrrd', '.zip'
        ]
        const isAllowed = (name: string) =>
            allowedExtensions.some(ext => name.toLowerCase().endsWith(ext))

        const filteredFiles = uploadedFiles.filter(f => isAllowed(f.name))
        if (filteredFiles.length === 0) {
            uploadError.value = 'Unsupported file format. Please upload .nii, .nii.gz, .dcm, .nrrd, or ZIPs of DICOMs.'
            file.value = null
            multipleFiles.value = null
            return
        }

        if (filteredFiles.length === 1 && filteredFiles[0].name.toLowerCase().endsWith('.zip')) {
            file.value = filteredFiles[0]
            multipleFiles.value = null
        } else {
            multipleFiles.value = filteredFiles
            file.value = null
        }
    }
}

const handleDrop = (event: DragEvent) => {
    event.preventDefault()
    isDragging.value = false

    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
        const uploadedFiles = Array.from(event.dataTransfer.files)
        const allowedExtensions = [
            '.nii', '.nii.gz', '.dcm', '.nrrd', '.zip'
        ]
        const isAllowed = (name: string) =>
            allowedExtensions.some(ext => name.toLowerCase().endsWith(ext))
        const filteredFiles = uploadedFiles.filter(f => isAllowed(f.name))
        if (filteredFiles.length === 0) {
            uploadError.value = 'Unsupported file format. Please upload .nii, .nii.gz, .dcm, .nrrd, or ZIPs of DICOMs.'
            file.value = null
            multipleFiles.value = null
            return
        }
        if (filteredFiles.length === 1 && filteredFiles[0].name.toLowerCase().endsWith('.zip')) {
            file.value = filteredFiles[0]
            multipleFiles.value = null
        } else {
            multipleFiles.value = filteredFiles
            file.value = null
        }
    }
}

const handleDragOver = (event: DragEvent) => {
    event.preventDefault()
    isDragging.value = true
}

const handleDragLeave = () => {
    isDragging.value = false
}

const clearFile = () => {
    file.value = null
    multipleFiles.value = null
    uploadError.value = null
    uploadProgress.value = 0
}

const prepareUploadFile = async (): Promise<File | null> => {
    if (file.value) {
        return file.value
    } else if (multipleFiles.value?.length) {
        const zip = new JSZip()
        multipleFiles.value.forEach(f => {
            zip.file(f.name, f)
        })
        const zippedContent = await zip.generateAsync({ type: 'blob' })
        return new File([zippedContent], 'dicoms.zip', { type: 'application/zip' })
    } else {
        return null
    }
}

const handleSubmit = async () => {
    if (!canUpload.value) {
        if (!userSubscription.value?.is_active) {
            uploadError.value = 'No active subscription found. Please subscribe to TiaraAI first.'
            showToast(uploadError.value, 'error')
            return
        }
        if (segmentationsRemaining.value <= 0) {
            uploadError.value = 'No segmentations remaining. Please upgrade your plan.'
            showToast(uploadError.value, 'error')
            return
        }
    }

    isUploading.value = true
    uploadError.value = null

    try {
        const uploadFile = await prepareUploadFile()

        if (!uploadFile) {
            uploadError.value = 'Please select a file to upload.'
            isUploading.value = false
            return
        }
        debugger;

        const formData = new FormData()
        formData.append('file', uploadFile)

        const token = localStorage.getItem('token')

        await api.post('/api/scans/upload', formData, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'multipart/form-data'
            },
            onUploadProgress: (progressEvent) => {
                if (progressEvent.total) {
                    uploadProgress.value = Math.round((progressEvent.loaded * 100) / progressEvent.total)
                    if (uploadProgress.value === 100) {
                        showConfirmation.value = true
                        isUploading.value = false
                        showToast('Upload successful! Your dental scan is being processed.', 'success')
                        // Refresh subscription data to update usage
                        fetchSubscriptionData()
                    }
                }
            }
        })
    } catch (error) {
        console.error('Upload failed:', error)
        if (error.response.status === 401) {

            showErrorToast("Session expired. Please log in again.")
            localStorage.clear();
            localStorage.setItem("isAuthenticated", "false")
            localStorage.setItem("token", "null")
            localStorage.setItem("userId", "0")

            setTimeout(() => {
                router.push('/login')
            }, 2000)
        }
        uploadError.value = 'Upload failed. Please try again later.'
        showToast(uploadError.value, 'error')
        isUploading.value = false
    }
}

const closeModal = () => {
    showConfirmation.value = false
    file.value = null
    multipleFiles.value = null
}

const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 Bytes'
    const k = 1024
    const sizes = ['Bytes', 'KB', 'MB', 'GB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

const showToast = (message: string, type: 'success' | 'error' = 'success') => {
    toastMessage.value = message
    toastType.value = type
    setTimeout(() => {
        toastMessage.value = ''
    }, 5000)
}
</script>

<template>
    <div class="min-h-screen bg-gradient-to-br from-slate-50 via-indigo-50 to-blue-50 relative overflow-hidden">
        <!-- Enhanced Animated Background -->
        <div class="absolute inset-0 overflow-hidden pointer-events-none z-0">
            <!-- Primary floating orbs -->
            <div class="absolute -top-10 -left-10 w-96 h-96 bg-[#1e3a8a]/10 rounded-full blur-3xl animate-pulse"></div>
            <div class="absolute top-1/4 right-10 w-80 h-80 bg-[#4052B5]/15 rounded-full blur-3xl animate-pulse"
                style="animation-delay: 1s"></div>
            <div class="absolute bottom-20 left-1/4 w-64 h-64 bg-indigo-400/10 rounded-full blur-3xl animate-pulse"
                style="animation-delay: 2s"></div>

            <!-- Floating particles -->
            <div class="absolute top-20 left-20 w-2 h-2 bg-[#1e3a8a]/30 rounded-full animate-bounce"></div>
            <div class="absolute top-40 right-40 w-1 h-1 bg-[#4052B5]/40 rounded-full animate-bounce"
                style="animation-delay: 0.5s"></div>
            <div class="absolute bottom-40 left-60 w-1.5 h-1.5 bg-indigo-400/30 rounded-full animate-bounce"
                style="animation-delay: 1.5s"></div>
        </div>

        <!-- Main Content -->
        <main class="max-w-6xl mx-auto py-8 px-4 sm:px-6 lg:px-8 relative z-10">
            <!-- Header Section with Logo -->
            <div class="text-center mb-12">
                <div class="flex justify-center mb-6">
                    <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-AI-logo-original.svg"
                        alt="TiaraAI Logo" class="h-16 w-auto filter drop-shadow-lg">
                </div>
                <h1
                    class="text-5xl font-bold bg-gradient-to-r from-[#1e3a8a] to-[#4052B5] bg-clip-text text-transparent mb-4">
                    AI Scan Analysis
                </h1>
                <p class="text-xl text-slate-600 max-w-3xl mx-auto leading-relaxed">
                    Transform your dental diagnostics with cutting-edge AI technology.
                    Upload your scans and receive detailed insights within minutes.
                </p>
            </div>

            <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
                <!-- Left Column - Subscription Stats -->
                <div class="lg:col-span-1">
                    <!-- Subscription Status Card -->
                    <div class="bg-white/70 backdrop-blur-xl rounded-2xl shadow-2xl border border-white/20 p-6 mb-6">
                        <div class="flex items-center justify-between mb-4">
                            <h3 class="text-lg font-semibold text-slate-800">Your Plan</h3>
                            <div class="w-3 h-3 rounded-full"
                                :class="userSubscription?.is_active ? 'bg-green-500 animate-pulse' : 'bg-red-500'">
                            </div>
                        </div>

                        <div v-if="isLoadingSubscription" class="animate-pulse">
                            <div class="h-4 bg-slate-200 rounded mb-3"></div>
                            <div class="h-8 bg-slate-200 rounded mb-3"></div>
                            <div class="h-2 bg-slate-200 rounded"></div>
                        </div>

                        <div v-else-if="userSubscription" class="space-y-4">
                            <!-- Usage Counter -->
                            <div class="text-center">
                                <div class="relative">
                                    <div class="text-3xl font-bold text-[#1e3a8a] mb-1">
                                        {{ animatedUsedCount }}<span class="text-xl text-slate-500">/ {{
                                            animatedAllowedCount }}</span>
                                    </div>
                                    <p class="text-sm text-slate-600">Segmentations Used</p>
                                </div>
                            </div>

                            <!-- Progress Bar -->
                            <div class="space-y-2">
                                <div class="flex justify-between text-sm text-slate-600">
                                    <span>Usage</span>
                                    <span>{{ Math.round(usagePercentage) }}%</span>
                                </div>
                                <div class="w-full bg-slate-200 rounded-full h-3 overflow-hidden">
                                    <div class="h-full bg-gradient-to-r transition-all duration-1000 ease-out rounded-full"
                                        :class="getUsageColor" :style="{ width: usagePercentage + '%' }"></div>
                                </div>
                            </div>

                            <!-- Remaining Count -->
                            <div class="bg-gradient-to-r from-[#1e3a8a]/10 to-[#4052B5]/10 rounded-xl p-4 text-center">
                                <div class="text-2xl font-bold"
                                    :class="segmentationsRemaining > 0 ? 'text-[#1e3a8a]' : 'text-red-500'">
                                    {{ segmentationsRemaining }}
                                </div>
                                <p class="text-sm text-slate-600">Remaining</p>
                            </div>

                            <!-- Warning if low -->
                            <div v-if="segmentationsRemaining <= 2 && segmentationsRemaining > 0"
                                class="bg-yellow-50 border border-yellow-200 rounded-lg p-3">
                                <div class="flex items-center">
                                    <svg class="w-5 h-5 text-yellow-600 mr-2" fill="none" stroke="currentColor"
                                        viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16c-.77.833.192 2.5 1.732 2.5z">
                                        </path>
                                    </svg>
                                    <p class="text-sm text-yellow-800">Only {{ segmentationsRemaining }} segmentations
                                        left!</p>
                                </div>
                            </div>

                            <!-- No segmentations left -->
                            <div v-if="segmentationsRemaining <= 0"
                                class="bg-red-50 border border-red-200 rounded-lg p-3">
                                <div class="flex items-center">
                                    <svg class="w-5 h-5 text-red-600 mr-2" fill="none" stroke="currentColor"
                                        viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                                    </svg>
                                    <p class="text-sm text-red-800">No segmentations remaining. Please upgrade your
                                        plan.</p>
                                </div>
                            </div>
                        </div>

                        <div v-else class="text-center py-8">
                            <svg class="w-12 h-12 text-slate-400 mx-auto mb-3" fill="none" stroke="currentColor"
                                viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                    d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                            </svg>
                            <p class="text-slate-600 text-sm">No active subscription found</p>
                            <router-link to="/ai"
                                class="inline-block mt-3 text-[#1e3a8a] hover:text-[#4052B5] font-medium text-sm">
                                Subscribe to TiaraAI →
                            </router-link>
                        </div>
                    </div>

                    <!-- Quick Stats -->
                    <div class="grid grid-cols-2 gap-4">
                        <div class="bg-white/50 backdrop-blur-xl rounded-xl p-4 text-center border border-white/20">
                            <div class="text-lg font-bold text-[#1e3a8a]">Fast</div>
                            <div class="text-xs text-slate-600">Analysis</div>
                        </div>
                        <div class="bg-white/50 backdrop-blur-xl rounded-xl p-4 text-center border border-white/20">
                            <div class="text-lg font-bold text-[#4052B5]">Accurate</div>
                            <div class="text-xs text-slate-600">Results</div>
                        </div>
                    </div>
                </div>

                <!-- Right Column - Upload Interface -->
                <div class="lg:col-span-2">
                    <div
                        class="bg-white/70 backdrop-blur-xl rounded-2xl shadow-2xl border border-white/20 overflow-hidden">
                        <div class="p-8">
                            <form @submit.prevent="handleSubmit" class="space-y-8">
                                <!-- Upload Zone -->
                                <div :class="[
                                    'relative border-2 border-dashed rounded-2xl p-12 text-center transition-all duration-500 cursor-pointer group',
                                    isDragging
                                        ? 'border-[#1e3a8a] bg-[#1e3a8a]/10 scale-105'
                                        : canUpload
                                            ? 'border-slate-300 hover:border-[#1e3a8a]/50 hover:bg-[#1e3a8a]/5'
                                            : 'border-red-300 bg-red-50/50 cursor-not-allowed'
                                ]" @dragover.prevent="handleDragOver" @dragleave.prevent="handleDragLeave"
                                    @drop.prevent="handleDrop"
                                    @click="canUpload && ($refs.fileInput as HTMLInputElement)?.click()">

                                    <input ref="fileInput" type="file" accept=".zip,.dcm,.nii,.nii.gz,.nrrd" multiple
                                        @change="handleFileUpload" class="hidden"
                                        :disabled="isUploading || !canUpload" />

                                    <!-- Upload Icon with enhanced animation -->
                                    <div class="relative mb-6">
                                        <div class="w-24 h-24 mx-auto bg-gradient-to-br from-[#1e3a8a] to-[#4052B5] rounded-2xl flex items-center justify-center group-hover:scale-110 transition-all duration-500 shadow-lg"
                                            :class="!canUpload && 'opacity-50'">
                                            <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor"
                                                viewBox="0 0 24 24">
                                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                                    d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
                                            </svg>
                                        </div>
                                        <!-- Floating ring animation -->
                                        <div class="absolute inset-0 rounded-2xl border-2 border-[#1e3a8a]/20 animate-ping"
                                            v-if="isDragging"></div>
                                    </div>

                                    <!-- Upload Text -->
                                    <div class="relative">
                                        <h3 class="text-2xl font-semibold mb-2"
                                            :class="canUpload ? 'text-slate-800' : 'text-red-600'">
                                            {{ canUpload ? 'Drop your files here' : 'Upload Disabled' }}
                                        </h3>
                                        <p class="mb-4" :class="canUpload ? 'text-slate-600' : 'text-red-500'">
                                            {{ canUpload
                                                ? 'or click to browse'
                                                : (segmentationsRemaining <= 0 ? 'No segmentations remaining'
                                                    : 'No active subscription') }} </p>
                                                <p class="text-sm text-slate-500">
                                                    Supports NIfTI (.nii, .nii.gz), DICOM (.dcm), NRRD (.nrrd), and ZIPs
                                                    of multiple DICOMs
                                                </p>
                                    </div>
                                </div>

                                <!-- Selected Files Display -->
                                <div v-if="file || multipleFiles" class="space-y-4">
                                    <div class="flex items-center justify-between">
                                        <h4 class="font-semibold text-slate-800 flex items-center">
                                            <svg class="w-5 h-5 mr-2 text-[#1e3a8a]" fill="none" stroke="currentColor"
                                                viewBox="0 0 24 24">
                                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                                    d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                                            </svg>
                                            Selected Files
                                        </h4>
                                        <button type="button" @click="clearFile"
                                            class="text-slate-500 hover:text-red-500 text-sm px-3 py-1 rounded-lg hover:bg-red-50 transition-colors">
                                            Clear all
                                        </button>
                                    </div>

                                    <div class="space-y-3 max-h-48 overflow-y-auto">
                                        <!-- Single file display -->
                                        <div v-if="file"
                                            class="bg-gradient-to-r from-slate-50 to-blue-50 rounded-xl p-4 border border-slate-200 flex items-center space-x-4 hover:shadow-md transition-shadow">
                                            <div
                                                class="w-12 h-12 bg-gradient-to-br from-[#1e3a8a] to-[#4052B5] rounded-xl flex items-center justify-center">
                                                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor"
                                                    viewBox="0 0 24 24">
                                                    <path stroke-linecap="round" stroke-linejoin="round"
                                                        stroke-width="2"
                                                        d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                                                </svg>
                                            </div>
                                            <div class="flex-1 min-w-0">
                                                <p class="font-medium text-slate-800 truncate">{{ file.name }}</p>
                                                <p class="text-sm text-slate-500">{{ formatFileSize(file.size) }}</p>
                                            </div>
                                        </div>

                                        <!-- Multiple files display -->
                                        <div v-else-if="multipleFiles" v-for="f in multipleFiles" :key="f.name"
                                            class="bg-gradient-to-r from-slate-50 to-blue-50 rounded-xl p-4 border border-slate-200 flex items-center space-x-4 hover:shadow-md transition-shadow">
                                            <div
                                                class="w-12 h-12 bg-gradient-to-br from-[#1e3a8a] to-[#4052B5] rounded-xl flex items-center justify-center">
                                                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor"
                                                    viewBox="0 0 24 24">
                                                    <path stroke-linecap="round" stroke-linejoin="round"
                                                        stroke-width="2"
                                                        d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                                                </svg>
                                            </div>
                                            <div class="flex-1 min-w-0">
                                                <p class="font-medium text-slate-800 truncate">{{ f.name }}</p>
                                                <p class="text-sm text-slate-500">{{ formatFileSize(f.size) }}</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Upload Progress -->
                                <div v-if="isUploading" class="space-y-4">
                                    <div class="flex justify-between items-center">
                                        <span class="text-sm font-medium text-slate-700">Processing your scan...</span>
                                        <span class="text-sm text-[#1e3a8a] font-semibold">{{ uploadProgress }}%</span>
                                    </div>
                                    <div class="w-full bg-slate-200 rounded-full h-3 overflow-hidden">
                                        <div class="bg-gradient-to-r from-[#1e3a8a] to-[#4052B5] h-3 rounded-full transition-all duration-500 ease-out"
                                            :style="{ width: uploadProgress + '%' }"></div>
                                    </div>
                                    <div class="flex items-center justify-center space-x-3 text-[#1e3a8a]">
                                        <div
                                            class="animate-spin rounded-full h-5 w-5 border-2 border-[#1e3a8a] border-t-transparent">
                                        </div>
                                        <span class="text-sm font-medium">AI is analyzing your dental scan...</span>
                                    </div>
                                </div>

                                <!-- Submit Button -->
                                <div class="flex justify-center">
                                    <button type="submit"
                                        :disabled="(!file && !multipleFiles) || isUploading || !canUpload"
                                        class="relative bg-gradient-to-r from-[#1e3a8a] to-[#4052B5] text-white px-10 py-4 rounded-xl font-semibold text-lg transition-all duration-300 transform hover:scale-105 disabled:opacity-50 disabled:cursor-not-allowed disabled:transform-none shadow-lg hover:shadow-xl">
                                        <span class="flex items-center">
                                            <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor"
                                                viewBox="0 0 24 24">
                                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                                    d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12" />
                                            </svg>
                                            {{ isUploading ? 'Analyzing...' : 'Start AI Analysis' }}
                                        </span>
                                        <!-- Shimmer effect -->
                                        <div v-if="canUpload && !isUploading"
                                            class="absolute inset-0 rounded-xl bg-gradient-to-r from-transparent via-white/20 to-transparent opacity-0 hover:opacity-100 transition-opacity duration-500 hover:animate-pulse">
                                        </div>
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </main>

        <!-- Enhanced Success Modal -->
        <div v-if="showConfirmation"
            class="fixed inset-0 flex items-center justify-center bg-black/50 backdrop-blur-sm z-50">
            <div class="bg-white p-8 rounded-2xl shadow-xl max-w-md w-full mx-4 animate-scale-in">
                <div class="text-center">
                    <div
                        class="w-16 h-16 bg-gradient-to-r from-[#1e3a8a]/10 to-[#4052B5]/10 rounded-full flex items-center justify-center mx-auto mb-4">
                        <div class="text-4xl animate-bounce">🎉</div>
                    </div>
                    <h3 class="text-2xl font-semibold text-slate-900 mb-2">Success!</h3>
                    <div class="space-y-4 text-slate-600">
                        <p>Your dental scan has been successfully received and is now being processed by our AI system.
                        </p>
                        <p
                            class="text-sm bg-gradient-to-r from-[#1e3a8a]/5 to-[#4052B5]/5 p-4 rounded-lg border border-[#1e3a8a]/10">
                            Expected processing time: <span class="font-medium text-[#1e3a8a]">8-10 minutes</span>
                        </p>
                        <p>We'll send you an email with the detailed results as soon as the analysis is complete.</p>
                    </div>
                    <button @click="closeModal"
                        class="mt-6 bg-gradient-to-r from-[#1e3a8a] to-[#4052B5] text-white w-full py-3 rounded-lg hover:shadow-lg transition-all duration-300 transform hover:scale-105 font-semibold">
                        Got it, thanks!
                    </button>
                </div>
            </div>
        </div>

        <!-- Enhanced Toast Messages -->
        <div v-if="toastMessage" :class="[
            'fixed top-6 right-6 p-4 rounded-xl shadow-2xl z-50 transition-all duration-500 transform backdrop-blur-sm',
            'max-w-sm border',
            toastType === 'success'
                ? 'bg-green-50 text-green-800 border-green-200'
                : 'bg-red-50 text-red-800 border-red-200'
        ]" style="animation: slideInRight 0.5s ease-out">
            <div class="flex items-center space-x-3">
                <div :class="[
                    'w-8 h-8 rounded-full flex items-center justify-center',
                    toastType === 'success' ? 'bg-green-500' : 'bg-red-500'
                ]">
                    <svg v-if="toastType === 'success'" class="w-4 h-4 text-white" fill="none" stroke="currentColor"
                        viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                    </svg>
                    <svg v-else class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M6 18L18 6M6 6l12 12" />
                    </svg>
                </div>
                <p class="font-medium">{{ toastMessage }}</p>
            </div>
        </div>
    </div>
</template>

<style scoped>
@keyframes slideInRight {
    from {
        transform: translateX(100%);
        opacity: 0;
    }

    to {
        transform: translateX(0);
        opacity: 1;
    }
}

@keyframes pulse {

    0%,
    100% {
        opacity: 1;
        transform: scale(1);
    }

    50% {
        opacity: 0.8;
        transform: scale(1.05);
    }
}

@keyframes bounce {

    0%,
    20%,
    53%,
    80%,
    100% {
        transform: translate3d(0, 0, 0);
    }

    40%,
    43% {
        transform: translate3d(0, -30px, 0);
    }

    70% {
        transform: translate3d(0, -15px, 0);
    }

    90% {
        transform: translate3d(0, -4px, 0);
    }
}

@keyframes scaleIn {
    from {
        opacity: 0;
        transform: scale(0.9);
    }

    to {
        opacity: 1;
        transform: scale(1);
    }
}

.animate-pulse {
    animation: pulse 4s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

.animate-scale-in {
    animation: scaleIn 0.3s ease-out;
}

.animate-bounce {
    animation: bounce 1s ease-in-out;
}

/* Custom scrollbar */
::-webkit-scrollbar {
    width: 6px;
}

::-webkit-scrollbar-track {
    background: #f1f5f9;
    border-radius: 3px;
}

::-webkit-scrollbar-thumb {
    background: linear-gradient(to bottom, #1e3a8a, #4052B5);
    border-radius: 3px;
}

::-webkit-scrollbar-thumb:hover {
    background: linear-gradient(to bottom, #1e40af, #4338ca);
}
</style>