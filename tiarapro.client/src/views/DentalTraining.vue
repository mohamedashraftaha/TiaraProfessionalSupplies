<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import api from '../utils/Api.ts'
import { useCartStore } from '../stores/CartStore'
import { useRouter } from 'vue-router'
import { showInfoToast } from '../utils/helpers'

interface DentalTrainingDTO {
    id: number
    title: string
    description: string
    instructors?: string
    date: string
    location?: string
    image_url?: string
    capacity?: number
    createdAt?: string
    updatedAt?: string
    price?: number
}

interface DentalTrainingPackageDTO {
    id: number
    dentalTrainingId: number
    name: string
    price: number
}

const trainings = ref<DentalTrainingDTO[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
const cartStore = useCartStore()
const router = useRouter()
const userId = localStorage.getItem('userId')
const registeredTrainings = ref<{ [key: number]: boolean }>({})
const packages = ref<Record<number, DentalTrainingPackageDTO[]>>({})
const selectedPackage = ref<Record<number, number | null>>({})

const fetchTrainings = async () => {
    loading.value = true
    error.value = null
    try {
        const res = await api.get('/api/dentaltraining')
        trainings.value = Array.isArray(res.data) ? res.data : []
    } catch (e) {
        error.value = 'Failed to load dental trainings.'
        trainings.value = []
        console.error('Error fetching dental trainings:', e)
    } finally {
        loading.value = false
    }
}

const checkRegistrationStatus = async () => {
    if (!userId || userId === 'null' || userId === '0') return
    await Promise.all(trainings.value.map(async (training) => {
        try {
            const res = await api.get(`/api/dentaltraining/${training.id}/is-registered/${userId}`)
            registeredTrainings.value[training.id] = res.data.registered
        } catch (e) {
            registeredTrainings.value[training.id] = false
        }
    }))
}

const fetchPackages = async (trainingId: number) => {
    try {
        const res = await api.get(`/api/dentaltrainingpackage/${trainingId}`);
        packages.value[trainingId] = res.data || [];
        if (res.data && res.data.length > 0) {
            selectedPackage.value[trainingId] = res.data[0].id;
        } else {
            selectedPackage.value[trainingId] = null;
        }
    } catch (e) {
        packages.value[trainingId] = [];
        selectedPackage.value[trainingId] = null;
    }
};

onMounted(async () => {
    await fetchTrainings();
    await checkRegistrationStatus();
    for (const training of trainings.value) {
        await fetchPackages(training.id);
    }
});

watch(trainings, async () => {
    await checkRegistrationStatus();
    for (const training of trainings.value) {
        await fetchPackages(training.id);
    }
});

const categories = computed(() => {
    const cats = new Set<string>()
    trainings.value.forEach(t => {
        if (t.instructors) cats.add(t.instructors)
    })
    return ['All', ...Array.from(cats)]
})
const selectedCategory = ref('All')

const filteredTrainings = computed(() => {
    if (selectedCategory.value === 'All') return trainings.value
    return trainings.value.filter(training => training.instructors === selectedCategory.value)
})

const handleRegister = (training) => {
    const userToken = localStorage.getItem('token')
    const userEmail = localStorage.getItem('email')
    if (userToken === "null" && !userEmail) {
        showInfoToast('Please sign in to register for dental training. You will be redirected to the login page.')
        setTimeout(() => {
            router.push('/login')
        }, 2000)
        return
    }
    // Clear cart first to ensure only the registration is added
    cartStore.clearCart()
    let pkg = null;
    if (packages.value[training.id] && packages.value[training.id].length > 0) {
        pkg = packages.value[training.id].find(p => p.id === selectedPackage.value[training.id]);
        if (!pkg) {
            showInfoToast('Please select a package before registering.');
            return;
        }
    }
    cartStore.addToCart({
        id: 2000000 + training.id, // Unique ID for dental training registration
        name: `Dental Training: ${training.title}` + (pkg ? ` - ${pkg.name}` : ''),
        price: pkg ? pkg.price : 0,
        image: training.image_url || 'https://placehold.co/600x400?text=Dental+Training',
        quantity: 1
    })
    router.push('/checkout')
}
</script>

<template>
    <div class="min-h-screen bg-gradient-to-br from-blue-900 to-indigo-800 text-white py-20">
        <!-- Hero Section -->
        <section class="text-center mb-20">
            <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-DT-logo-light.svg"
                alt="Tiara Training" class="h-32 mx-auto mb-8" />
            <h1 class="text-5xl md:text-6xl font-bold">Professional Dental Training</h1>
            <p class="text-xl md:text-2xl text-gray-300 mt-4">
                Advance your dental skills with expert-led training programs
            </p>
        </section>

        <!-- Category Filter -->
        <div class="flex justify-center flex-wrap gap-4 mb-12">
            <button v-for="category in categories" :key="category" @click="selectedCategory = category" :class="[
                'px-5 py-2 rounded-full font-medium transition',
                selectedCategory === category
                    ? 'bg-cyan-500 text-white'
                    : 'bg-white/20 text-white hover:bg-white/30'
            ]">
                {{ category }}
            </button>
        </div>

        <div v-if="loading" class="flex justify-center items-center py-16">
            <svg class="animate-spin h-12 w-12 text-cyan-400" xmlns="http://www.w3.org/2000/svg" fill="none"
                viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z" />
            </svg>
        </div>
        <div v-else-if="error" class="text-center text-red-400 py-10">{{ error }}</div>
        <div v-else-if="filteredTrainings.length === 0" class="text-center text-gray-200 py-10">No dental training
            programs available.</div>

        <!-- Trainings Grid -->
        <div v-else class="grid md:grid-cols-2 lg:grid-cols-3 gap-8 px-4">
            <div v-for="training in filteredTrainings" :key="training.id"
                class="bg-white text-gray-800 rounded-2xl overflow-hidden shadow-lg hover:-translate-y-1 hover:shadow-2xl transition relative">
                <!-- Registered Ribbon -->
                <div v-if="registeredTrainings[training.id]"
                    class="absolute -left-10 top-8 w-40 z-40 transform -rotate-45 bg-gradient-to-r from-green-500 to-emerald-600 text-white font-bold py-1.5 text-center shadow-lg">
                    <svg class="w-4 h-4 mr-1 inline-block" fill="none" stroke="currentColor" stroke-width="2"
                        viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                    </svg>
                    Registered
                </div>
                <img :src="training.image_url || 'https://placehold.co/600x400?text=Dental+Training'"
                    :alt="training.title" class="w-full h-48 object-cover" />
                <div class="p-6">
                    <div class="flex justify-between items-center mb-4">
                        <h3 class="text-xl font-bold text-gray-900">{{ training.title }}</h3>
                        <span v-if="training.instructors"
                            class="px-3 py-1 bg-cyan-100 text-cyan-700 rounded-full text-sm">
                            {{ training.instructors }}
                        </span>
                    </div>
                    <p class="text-gray-600 mb-4 text-sm">{{ training.description }}</p>

                    <div class="space-y-2 text-sm mb-6 text-gray-700">
                        <div class="flex items-center">
                            <svg class="h-5 w-5 mr-2 text-cyan-500" fill="none" viewBox="0 0 24 24"
                                stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                    d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                            </svg>
                            {{ new Date(training.date).toLocaleDateString('en-US', {
                                month: 'long', day: 'numeric',
                                year: 'numeric', timeZone: 'Africa/Cairo'
                            }) }}
                            <span v-if="training.date">
                                &nbsp;at&nbsp;
                                {{ new Date(training.date).toLocaleTimeString([], {
                                    hour: '2-digit', minute: '2-digit',
                                    timeZone: 'UTC'
                                }) }}
                            </span>
                        </div>

                        <div class="flex items-center">
                            <svg class="h-5 w-5 mr-2 text-cyan-500" fill="none" viewBox="0 0 24 24"
                                stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                    d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                    d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                            </svg>
                            {{ training.location }}
                        </div>
                        <div class="flex items-center">
                            <svg class="h-5 w-5 mr-2 text-cyan-500" fill="none" viewBox="0 0 24 24"
                                stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                    d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                            </svg>
                            <span v-if="training.capacity">{{ training.capacity }} spots available</span>
                            <span v-else>Unlimited spots</span>
                        </div>
                    </div>

                    <!-- Packages Selection -->
                    <div v-if="packages[training.id] && packages[training.id].length > 0" class="mb-4">
                        <div class="font-semibold mb-1">Choose a Package:</div>
                        <div class="flex flex-col gap-2">
                            <label v-for="pkg in packages[training.id]" :key="pkg.id"
                                class="flex items-center gap-2 cursor-pointer">
                                <input type="radio" :name="'pkg-' + training.id" :value="pkg.id"
                                    v-model="selectedPackage[training.id]" />
                                <span>{{ pkg.name }} - {{ pkg.price.toLocaleString(undefined, {
                                    minimumFractionDigits:
                                        2, maximumFractionDigits: 2
                                }) }} EGP</span>
                            </label>
                        </div>
                    </div>

                    <div class="flex justify-between items-center">
                        <span class="text-xl font-bold text-cyan-600">&nbsp;</span>
                        <div class="w-full flex justify-end">
                            <button v-if="!registeredTrainings[training.id]"
                                class="px-5 py-2 bg-cyan-500 text-white rounded-lg hover:bg-cyan-600 transition"
                                @click="handleRegister(training)">
                                Register Now
                            </button>
                            <button v-else
                                class="px-5 py-2 bg-green-500 text-white rounded-lg flex items-center gap-2 cursor-not-allowed opacity-90 shadow-none"
                                disabled aria-disabled="true" title="You are already registered for this training">
                                <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2"
                                    viewBox="0 0 24 24">
                                    <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                                </svg>
                                Already Registered
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
/* Registered Ribbon Styles */
.bg-white .absolute[style*="rotate-45"] {
    pointer-events: none;
    font-size: 0.95rem;
    letter-spacing: 0.02em;
}
</style>