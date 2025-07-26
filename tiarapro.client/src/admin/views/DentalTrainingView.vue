<template>
    <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-7xl mx-auto">

        <!-- Header -->
        <div class="sm:flex sm:justify-between sm:items-center mb-8">
            <!-- Title -->
            <div class="mb-4 sm:mb-0">
                <h1 class="text-2xl md:text-3xl font-extrabold text-slate-800 tracking-tight">Dental Training</h1>
            </div>

            <!-- Actions -->
            <div class="flex flex-wrap gap-2 items-center">
                <button
                    class="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg shadow hover:bg-cyan-700 focus:ring-2 focus:ring-cyan-400 transition font-semibold"
                    @click="openAdd" aria-label="Add Training">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4" />
                    </svg>
                    Add Training
                </button>
                <button
                    class="flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg shadow hover:bg-green-700 focus:ring-2 focus:ring-green-400 transition font-semibold"
                    @click="exportAllRegistrationsToExcel" aria-label="Export All Registrations">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round"
                            d="M16 16v2a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2" />
                        <rect width="8" height="8" x="12" y="4" rx="2" />
                        <path stroke-linecap="round" stroke-linejoin="round" d="M15 9v6m3-3h-6" />
                    </svg>
                    Export All
                </button>
            </div>
        </div>

        <!-- Loading -->
        <div v-if="loading" class="flex justify-center py-12">
            <svg class="animate-spin h-8 w-8 text-cyan-500" xmlns="http://www.w3.org/2000/svg" fill="none"
                viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
            </svg>
        </div>

        <!-- Mobile: Cards -->
        <div v-if="!loading" class="grid grid-cols-1 gap-4 md:hidden">
            <div v-for="training in trainings" :key="training.id"
                class="bg-white rounded-xl shadow border border-slate-200 overflow-hidden hover:shadow-lg transition">
                <div class="p-5">
                    <div class="flex justify-between items-start mb-2">
                        <div>
                            <p class="text-xs text-slate-400 uppercase tracking-wide">Training</p>
                            <p class="font-bold text-slate-900 text-lg">{{ training.title }}</p>
                        </div>
                        <span v-if="training.instructors"
                            class="px-2 py-1 rounded-full text-xs font-medium bg-cyan-100 text-cyan-700">
                            {{ training.instructors }}
                        </span>
                    </div>
                    <div class="space-y-3 py-3 border-t border-b border-slate-100 my-3">
                        <div class="flex justify-between text-sm">
                            <span class="text-slate-500">Date:</span>
                            <span class="font-medium text-slate-700">{{ formatDate(training.date) }}</span>
                        </div>
                        <div class="flex justify-between text-sm">
                            <span class="text-slate-500">Location:</span>
                            <span class="font-medium text-slate-700">{{ training.location || 'Not specified' }}</span>
                        </div>
                        <div class="flex justify-between text-sm">
                            <span class="text-slate-500">Capacity:</span>
                            <span class="font-medium text-slate-700">{{ training.capacity ?? '—' }}</span>
                        </div>
                    </div>
                    <div class="flex flex-col gap-2 mt-4">
                        <button @click="openEdit(training)"
                            class="flex items-center justify-center gap-2 px-3 py-2 bg-cyan-100 text-cyan-800 hover:bg-cyan-200 rounded-lg transition text-sm font-medium focus:ring-2 focus:ring-cyan-400"
                            aria-label="Edit Training" title="Edit Training">
                            <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round"
                                    d="M15.232 5.232l3.536 3.536M9 13l6-6m2 2l-6 6m-2 2h2v2H7v-2h2z" />
                            </svg>
                            Edit
                        </button>
                        <button @click="confirmDeleteTraining(training)"
                            class="flex items-center justify-center gap-2 px-3 py-2 text-red-600 hover:text-red-700 hover:bg-red-50 rounded-lg transition text-sm font-medium focus:ring-2 focus:ring-red-400"
                            aria-label="Delete Training" title="Delete Training">
                            <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
                            </svg>
                            Delete
                        </button>
                        <button @click="exportRegistrationsToExcel(training)"
                            class="flex items-center justify-center gap-2 px-3 py-2 bg-green-100 text-green-800 hover:bg-green-200 rounded-lg transition text-sm font-medium focus:ring-2 focus:ring-green-400"
                            aria-label="Export Registrations" title="Export registrations for this training">
                            <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round"
                                    d="M16 16v2a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2" />
                                <rect width="8" height="8" x="12" y="4" rx="2" />
                                <path stroke-linecap="round" stroke-linejoin="round" d="M15 9v6m3-3h-6" />
                            </svg>
                            Export
                        </button>
                    </div>
                </div>
            </div>
            <div v-if="trainings.length === 0" class="py-12 text-center text-slate-500">
                <div class="flex flex-col items-center">
                    <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3" />
                        <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2" />
                    </svg>
                    No dental training programs found.
                </div>
            </div>
        </div>

        <!-- Desktop: Table -->
        <div v-if="!loading" class="hidden md:block">
            <div class="bg-white rounded-xl shadow border border-slate-200 overflow-hidden">
                <div class="overflow-x-auto">
                    <table class="min-w-full w-[1200px] divide-y divide-slate-200">
                        <thead class="bg-slate-50">
                            <tr>
                                <th
                                    class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                    Title</th>
                                <th
                                    class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                    Date</th>
                                <th
                                    class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                    Location</th>
                                <th
                                    class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                    Instructors</th>
                                <th
                                    class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                    Capacity</th>
                                <th
                                    class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                    Packages</th>
                                <th
                                    class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                    Registrations</th>
                                <th
                                    class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                    Actions</th>
                            </tr>
                        </thead>
                        <tbody class="bg-white divide-y divide-slate-200">
                            <tr v-for="training in trainings" :key="training.id" class="hover:bg-slate-50 transition">
                                <td class="px-6 py-4 font-bold text-slate-800 text-base">{{ training.title }}</td>
                                <td class="px-6 py-4 text-slate-700">{{ formatDate(training.date) }}</td>
                                <td class="px-6 py-4 text-slate-700">{{ training.location || 'Not specified' }}</td>
                                <td class="px-6 py-4 text-slate-700">{{ training.instructors || 'Not specified' }}</td>
                                <td class="px-6 py-4 text-slate-700">{{ training.capacity ?? '—' }}</td>
                                <td class="px-6 py-4 text-slate-700">
                                    <div v-if="packages[training.id] && packages[training.id].length">
                                        <div v-for="pkg in packages[training.id]" :key="pkg.id">
                                            <span class="font-semibold">{{ pkg.name }}</span>
                                            <span class="text-xs text-slate-500">({{ pkg.price.toLocaleString(undefined,
                                                { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }} EGP)</span>
                                        </div>
                                    </div>
                                    <span v-else class="text-slate-400">—</span>
                                </td>
                                <td class="px-6 py-4">
                                    <span v-if="registrationsCount[training.id] !== undefined"
                                        class="inline-block px-2 py-1 rounded-full bg-cyan-50 text-cyan-700 text-xs font-semibold">
                                        {{ registrationsCount[training.id] }} registered
                                    </span>
                                    <span v-else class="text-slate-400">—</span>
                                    <button @click="openRegistrationsModal(training)"
                                        class="ml-2 px-2 py-1 text-cyan-700 hover:underline text-xs focus:outline-none focus:underline"
                                        aria-label="View Registrations" title="View Registrations">
                                        View
                                    </button>
                                </td>
                                <td class="px-6 py-4">
                                    <div class="flex gap-2">
                                        <button @click="openEdit(training)"
                                            class="flex items-center justify-center w-8 h-8 bg-cyan-100 text-cyan-800 rounded hover:bg-cyan-200 transition focus:ring-2 focus:ring-cyan-400"
                                            aria-label="Edit Training" title="Edit Training">
                                            <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2"
                                                viewBox="0 0 24 24">
                                                <path stroke-linecap="round" stroke-linejoin="round"
                                                    d="M15.232 5.232l3.536 3.536M9 13l6-6m2 2l-6 6m-2 2h2v2H7v-2h2z" />
                                            </svg>
                                        </button>
                                        <button @click="confirmDeleteTraining(training)"
                                            class="flex items-center justify-center w-8 h-8 text-red-600 hover:bg-red-50 rounded transition focus:ring-2 focus:ring-red-400"
                                            aria-label="Delete Training" title="Delete Training">
                                            <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2"
                                                viewBox="0 0 24 24">
                                                <path stroke-linecap="round" stroke-linejoin="round"
                                                    d="M6 18L18 6M6 6l12 12" />
                                            </svg>
                                        </button>
                                        <button @click="exportRegistrationsToExcel(training)"
                                            class="flex items-center justify-center w-8 h-8 bg-green-100 text-green-800 rounded hover:bg-green-200 transition focus:ring-2 focus:ring-green-400"
                                            aria-label="Export Registrations"
                                            title="Export registrations for this training">
                                            <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2"
                                                viewBox="0 0 24 24">
                                                <path stroke-linecap="round" stroke-linejoin="round"
                                                    d="M16 16v2a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2" />
                                                <rect width="8" height="8" x="12" y="4" rx="2" />
                                                <path stroke-linecap="round" stroke-linejoin="round"
                                                    d="M15 9v6m3-3h-6" />
                                            </svg>
                                        </button>
                                    </div>
                                </td>
                            </tr>
                            <tr v-if="trainings.length === 0">
                                <td colspan="8" class="px-6 py-12 text-center text-slate-500">
                                    <div class="flex flex-col items-center">
                                        <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor"
                                            viewBox="0 0 24 24">
                                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                                d="M12 8v4l3 3" />
                                            <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2" />
                                        </svg>
                                        No dental training programs found.
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- Packages Table -->
        <!-- <div class="bg-white rounded-xl shadow border border-slate-200 overflow-hidden mt-10">
            <div class="flex justify-between items-center px-6 py-4">
                <span class="font-semibold text-cyan-700 text-lg">All Packages</span>
                <button @click="openAddPackage()" :disabled="addableTrainings.length === 0"
                    class="px-4 py-2 bg-cyan-600 text-white rounded hover:bg-cyan-700 disabled:opacity-50">Add
                    Package</button>
            </div>
            <div class="overflow-x-auto">
                <table class="min-w-full divide-y divide-slate-200">
                    <thead class="bg-slate-50">
                        <tr>
                            <th
                                class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                Package Name</th>
                            <th
                                class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                Price (EGP)</th>

                            <th
                                class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="pkg in allPackages" :key="pkg.id">
                            <td class="px-4 py-2">{{ pkg.name }}</td>
                            <td class="px-4 py-2">{{ pkg.price.toLocaleString(undefined, {
                                minimumFractionDigits: 2,
                                maximumFractionDigits: 2
                            }) }}</td>
                            <td class="px-4 py-2">
                                <button @click="openEditPackage(pkg)"
                                    class="text-cyan-700 hover:underline mr-2">Edit</button>
                                <button @click="deletePackage(pkg)" class="text-red-600 hover:underline">Delete</button>
                            </td>
                        </tr>
                        <tr v-if="allPackages.length === 0">
                            <td colspan="4" class="text-slate-400 px-4 py-4">No packages added yet.</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div> -->

        <!-- Add/Edit Modal -->
        <div v-if="showForm" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
            <div class="bg-white rounded-xl shadow-xl max-w-lg w-full p-6" @click.stop
                style="max-height:90vh; overflow-y:auto;">
                <div class="flex justify-between items-center mb-4">
                    <h3 class="text-lg font-bold text-cyan-700">{{ editMode ? 'Edit Training' : 'Add Training' }}</h3>
                    <button @click="closeForm" class="text-slate-400 hover:text-slate-500">
                        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>
                <form @submit.prevent="submitForm" class="space-y-4">
                    <div v-if="formError" class="text-red-500 mb-2">{{ formError }}</div>
                    <div>
                        <label class="block font-semibold mb-1">Title</label>
                        <input v-model="form.title"
                            class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none"
                            required />
                    </div>
                    <div>
                        <label class="block font-semibold mb-1">Description</label>
                        <textarea v-model="form.description"
                            class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none"
                            required></textarea>
                    </div>
                    <div>
                        <label class="block font-semibold mb-1">Instructors</label>
                        <input v-model="form.instructors"
                            class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none" />
                    </div>
                    <div>
                        <label class="block font-semibold mb-1">Date & Time</label>
                        <input type="datetime-local" v-model="form.date"
                            class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none"
                            required />
                    </div>
                    <div>
                        <label class="block font-semibold mb-1">Location</label>
                        <input v-model="form.location"
                            class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none" />
                    </div>
                    <div>
                        <label class="block font-semibold mb-1">Image URL</label>
                        <div class="flex gap-2">
                            <input v-model="form.image_url"
                                class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none" />
                            <button type="button" @click="handleImageUploadClick" :disabled="imageUploading"
                                class="px-3 py-2 bg-cyan-600 text-white rounded-lg hover:bg-cyan-700 transition disabled:opacity-50">
                                <span v-if="imageUploading">Uploading...</span>
                                <span v-else>Upload Image</span>
                            </button>
                            <input ref="imageUploadInput" type="file" accept="image/*" class="hidden"
                                @change="handleImageFileChange" />
                        </div>
                        <div v-if="form.image_url" class="mt-2">
                            <p class="text-xs text-slate-500 mb-1">Image Preview:</p>
                            <img :src="form.image_url" alt="Training Image Preview"
                                class="h-16 w-16 rounded-md object-cover border border-slate-200"
                                @error="(e) => (e.target as HTMLImageElement).src = 'https://placehold.co/600x400?text=Dental+Training'" />
                        </div>
                    </div>
                    <div>
                        <label class="block font-semibold mb-1">Capacity</label>
                        <input type="number" v-model.number="form.capacity"
                            class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none"
                            min="1" />
                    </div>
                    <div>
                        <label class="block font-semibold mb-1">Packages (at least one required)</label>
                        <div class="flex flex-col gap-2">
                            <div v-for="(pkg, idx) in packageInputs" :key="idx" class="flex gap-2 items-center">
                                <input v-model="pkg.name" placeholder="Package Name"
                                    class="border border-cyan-300 p-2 rounded-lg flex-1" />
                                <input v-model.number="pkg.price" type="number" min="0" step="0.01"
                                    placeholder="Price (EGP)" class="border border-cyan-300 p-2 rounded-lg w-32" />
                            </div>
                        </div>
                    </div>
                    <div class="flex justify-end gap-2 mt-6">
                        <button type="button"
                            class="px-5 py-2 bg-gray-200 text-gray-800 rounded-lg hover:bg-gray-300 transition font-semibold"
                            @click="closeForm">
                            Cancel
                        </button>
                        <button type="submit"
                            class="px-6 py-2 bg-cyan-600 text-white rounded-lg hover:bg-cyan-700 transition font-semibold shadow">
                            {{ editMode ? 'Update' : 'Add' }}
                        </button>
                    </div>
                </form>
            </div>
        </div>

        <!-- Delete confirmation modal -->
        <div v-if="showDeleteModal"
            class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
            <div class="bg-white rounded-xl shadow-xl max-w-md w-full p-6" @click.stop>
                <h3 class="text-lg font-bold text-slate-800 mb-4">Confirm Deletion</h3>
                <p class="text-slate-600 mb-6">
                    Are you sure you want to delete training
                    <span class="font-medium text-slate-800">"{{ currentTraining?.title }}"</span>?
                    This action cannot be undone.
                </p>
                <div class="flex justify-end space-x-3">
                    <button @click="showDeleteModal = false"
                        class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
                        Cancel
                    </button>
                    <button @click="deleteTraining"
                        class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors">
                        Delete
                    </button>
                </div>
            </div>
        </div>

        <!-- Registrations Modal -->
        <div v-if="showRegistrationsModal"
            class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
            <div class="bg-white rounded-xl shadow-xl max-w-2xl w-full p-6" @click.stop>
                <div class="flex justify-between items-center mb-4">
                    <h3 class="text-lg font-bold text-cyan-700">Registrations for {{ selectedTraining?.title }}</h3>
                    <button @click="closeRegistrationsModal" class="text-slate-400 hover:text-slate-500">
                        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>
                <div v-if="registrationsLoading" class="flex justify-center py-8">
                    <svg class="animate-spin h-8 w-8 text-cyan-500" xmlns="http://www.w3.org/2000/svg" fill="none"
                        viewBox="0 0 24 24">
                        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
                        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                    </svg>
                </div>
                <div v-else>
                    <div v-if="registrationsError" class="text-red-500 mb-4">{{ registrationsError }}</div>
                    <div v-if="registrationsList.length === 0" class="text-slate-500 text-center py-8">No registrations
                        found.</div>
                    <table v-else class="min-w-full divide-y divide-slate-200">
                        <thead class="bg-slate-50">
                            <tr>
                                <th
                                    class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                    Name</th>
                                <th
                                    class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                    Email</th>
                                <th
                                    class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                    Registered At</th>
                            </tr>
                        </thead>
                        <tbody class="bg-white divide-y divide-slate-200">
                            <tr v-for="reg in registrationsList" :key="reg.user_id">
                                <td class="px-4 py-2">{{ reg.first_name }} {{ reg.last_name }}</td>
                                <td class="px-4 py-2">{{ reg.email }}</td>
                                <td class="px-4 py-2">{{ formatDate(reg.registered_at) }}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- Package Add/Edit Modal -->
        <div v-if="showPackageForm"
            class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
            <div class="bg-white rounded-xl shadow-xl max-w-md w-full p-6" @click.stop>
                <h3 class="text-lg font-bold text-cyan-700 mb-4">{{ packageEditMode ? 'Edit' : 'Add' }} Package</h3>
                <form @submit.prevent="submitPackageForm" class="space-y-4">
                    <div v-if="packageFormError" class="text-red-500 mb-2">{{ packageFormError }}</div>
                    <div>
                        <label class="block font-semibold mb-1">Name</label>
                        <input v-model="packageForm.name"
                            class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none"
                            required />
                    </div>
                    <div>
                        <label class="block font-semibold mb-1">Price (EGP)</label>
                        <input type="number" v-model.number="packageForm.price"
                            class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none"
                            min="0" step="0.01" required />
                    </div>
                    <div>
                        <label class="block font-semibold mb-1">Parent Training</label>
                        <select v-model="packageForm.dentalTrainingId"
                            class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none"
                            :disabled="packageEditMode">
                            <option v-for="training in addableTrainings" :key="training.id" :value="training.id">
                                {{ training.title }}
                            </option>
                        </select>
                    </div>
                    <div class="flex justify-end gap-2 mt-6">
                        <button type="button"
                            class="px-5 py-2 bg-gray-200 text-gray-800 rounded-lg hover:bg-gray-300 transition font-semibold"
                            @click="closePackageForm">Cancel</button>
                        <button type="submit"
                            class="px-6 py-2 bg-cyan-600 text-white rounded-lg hover:bg-cyan-700 transition font-semibold shadow">{{
                                packageEditMode ? 'Update' : 'Add' }}</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import api from '../../utils/Api.ts'
import * as XLSX from 'xlsx';

interface DentalTrainingDTO {
    id?: number;
    title: string;
    description: string;
    instructors?: string;
    date: string;
    location?: string;
    image_url?: string;
    price?: number;
    capacity?: number;
    createdAt?: string;
    updatedAt?: string;
}

interface Registration {
    user_id: number;
    first_name: string;
    last_name: string;
    email: string;
    registered_at: string;
}

interface DentalTrainingPackageDTO {
    id?: number;
    dentalTrainingId: number;
    name: string;
    price: number;
}

const trainings = ref<DentalTrainingDTO[]>([]);
const showForm = ref(false);
const editMode = ref(false);
const loading = ref(true);
const error = ref<string | null>(null);
const formError = ref<string | null>(null);
const form = ref<DentalTrainingDTO>({
    title: '',
    description: '',
    instructors: '',
    date: '',
    location: '',
    image_url: '',
    price: undefined,
    capacity: undefined,
});
const editingId = ref<number | null>(null);
const showDeleteModal = ref(false);
const currentTraining = ref<DentalTrainingDTO | null>(null);

// Registrations state
const registrationsCount = ref<Record<number, number>>({});
const showRegistrationsModal = ref(false);
const selectedTraining = ref<DentalTrainingDTO | null>(null);
const registrationsList = ref<Registration[]>([]);
const registrationsLoading = ref(false);
const registrationsError = ref<string | null>(null);

// Image upload state
const imageUploading = ref(false)
const imageUploadInput = ref<HTMLInputElement | null>(null)

// Dental Training Packages state
const packages = ref<Record<number, DentalTrainingPackageDTO[]>>({});
const packageForm = ref<DentalTrainingPackageDTO>({ id: undefined, dentalTrainingId: 0, name: '', price: 0 });
const showPackageForm = ref(false);
const packageEditMode = ref(false);
const packageFormError = ref('');
const currentPackageTrainingId = ref<number | null>(null);

const allPackages = computed(() => Object.values(packages.value).flat());
const getTrainingTitle = (id: number) => {
    const t = trainings.value.find(t => t.id === id);
    return t ? t.title : 'Unknown';
};
const addableTrainings = computed(() => {
    // Only trainings with <2 packages
    const counts: Record<number, number> = {};
    for (const pkg of allPackages.value) {
        counts[pkg.dentalTrainingId] = (counts[pkg.dentalTrainingId] || 0) + 1;
    }
    return trainings.value.filter(t => (counts[t.id] || 0) < 2);
});

const packageInputs = ref([
    { name: '', price: null },
    { name: '', price: null }
]);

const handleImageUploadClick = () => {
    imageUploadInput.value?.click()
}

const handleImageFileChange = async (event: Event) => {
    const files = (event.target as HTMLInputElement).files
    if (!files || files.length === 0) return
    const file = files[0]
    const formData = new FormData()
    formData.append('file', file)
    imageUploading.value = true
    try {
        const response = await api.post('/api/product/upload-image', formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        })
        if (response.data && response.data.url) {
            form.value.image_url = response.data.url
        } else {
            alert('Failed to upload image')
        }
    } catch (error) {
        alert('Failed to upload image')
    } finally {
        imageUploading.value = false
        if (imageUploadInput.value) imageUploadInput.value.value = ''
    }
}

const fetchTrainings = async () => {
    loading.value = true;
    error.value = null;
    try {
        const res = await api.get('/api/dentaltraining');
        // Filter out invalid records and only show records with title and description
        trainings.value = (res.data || []).filter(training =>
            training.title &&
            training.description &&
            training.title.trim() !== '' &&
            training.description.trim() !== ''
        );
    } catch (e) {
        error.value = 'Failed to load dental trainings.';
        trainings.value = [];
    } finally {
        loading.value = false;
    }
};

const openAdd = () => {
    editMode.value = false;
    formError.value = null;
    form.value = { title: '', description: '', instructors: '', date: '', location: '', image_url: '', capacity: undefined };
    packageInputs.value = [{ name: '', price: null }, { name: '', price: null }];
    showForm.value = true;
    editingId.value = null;
};

const openEdit = (training: DentalTrainingDTO) => {
    editMode.value = true;
    formError.value = null;
    // Format date for datetime-local input
    let date = training.date;
    if (date && date.length > 0) {
        const d = new Date(date);
        form.value = { ...training, date: d.toISOString().slice(0, 16) };
    } else {
        form.value = { ...training };
    }
    // Pre-fill packageInputs with the packages for this training (up to 2)
    const pkgs = packages.value[training.id!] || [];
    packageInputs.value = [
        pkgs[0] ? { name: pkgs[0].name, price: pkgs[0].price } : { name: '', price: null },
        pkgs[1] ? { name: pkgs[1].name, price: pkgs[1].price } : { name: '', price: null }
    ];
    showForm.value = true;
    editingId.value = training.id!;
};

const closeForm = () => {
    showForm.value = false;
    formError.value = null;
};

const submitForm = async () => {
    formError.value = null;
    // Client-side validation
    if (!form.value.title || !form.value.description) {
        formError.value = 'Title and Description are required.';
        return;
    }
    if (!form.value.date || isNaN(new Date(form.value.date).getTime())) {
        formError.value = 'A valid date and time are required.';
        return;
    }
    if (new Date(form.value.date) < new Date(Date.now() - 5 * 60 * 1000)) {
        formError.value = 'Date must be in the future.';
        return;
    }
    // Validate at least one package
    const packages = packageInputs.value.filter(pkg => pkg.name && pkg.price != null && pkg.price !== '');
    if (packages.length === 0) {
        formError.value = 'At least one package (name and price) is required.';
        return;
    }
    try {
        const dto = { ...form.value, packages };
        if (editMode.value && editingId.value) {
            await api.put(`/api/dentaltraining/${editingId.value}`, dto);
        } else {
            await api.post('/api/dentaltraining', dto);
        }
        showForm.value = false;
        await fetchTrainings();
        await fetchAllPackages();
    } catch (e) {
        if (e.response && e.response.data) {
            formError.value = e.response.data;
        } else {
            formError.value = 'Failed to save training.';
        }
    }
};

const confirmDeleteTraining = (training: DentalTrainingDTO) => {
    currentTraining.value = training;
    showDeleteModal.value = true;
};

const deleteTraining = async () => {
    if (!currentTraining.value) return;
    try {
        await api.delete(`/api/dentaltraining/${currentTraining.value.id}`);
        showDeleteModal.value = false;
        await fetchTrainings();
    } catch (e) {
        error.value = 'Failed to delete training.';
    }
};

const formatDate = (dateStr: string) => {
    if (!dateStr) return 'No date set';
    const d = new Date(dateStr);
    if (isNaN(d.getTime())) return 'Invalid date';
    return d.toLocaleString();
};

const fetchRegistrations = async (trainingId: number) => {
    registrationsLoading.value = true;
    registrationsError.value = null;
    try {
        const res = await api.get(`/api/dentaltraining/${trainingId}/registrations`);
        registrationsList.value = res.data || [];
        registrationsCount.value[trainingId] = registrationsList.value.length;
    } catch (e: any) {
        registrationsError.value = 'Failed to load registrations.';
        registrationsList.value = [];
    } finally {
        registrationsLoading.value = false;
    }
};

const openRegistrationsModal = async (training: DentalTrainingDTO) => {
    selectedTraining.value = training;
    showRegistrationsModal.value = true;
    await fetchRegistrations(training.id!);
};

const closeRegistrationsModal = () => {
    showRegistrationsModal.value = false;
    selectedTraining.value = null;
    registrationsList.value = [];
    registrationsError.value = null;
};

// Fetch registrations count for all trainings after loading trainings
const fetchAllRegistrationsCount = async () => {
    for (const training of trainings.value) {
        await fetchRegistrations(training.id!);
    }
};

const exportAllRegistrationsToExcel = async () => {
    let allRegistrations = [];
    for (const training of trainings.value) {
        try {
            const res = await api.get(`/api/dentaltraining/${training.id}/registrations`);
            const regs = (res.data || []).map((reg: any) => ({
                TrainingTitle: training.title,
                TrainingDate: formatDate(training.date),
                Name: reg.first_name + ' ' + reg.last_name,
                Email: reg.email,
                RegisteredAt: formatDate(reg.registered_at),
            }));
            allRegistrations = allRegistrations.concat(regs);
        } catch (e) {
            // Optionally handle error
        }
    }
    if (allRegistrations.length === 0) {
        alert('No registrations to export.');
        return;
    }
    const ws = XLSX.utils.json_to_sheet(allRegistrations);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Registrations');
    XLSX.writeFile(wb, 'dental_training_registrations.xlsx');
};

const exportRegistrationsToExcel = async (training: DentalTrainingDTO) => {
    try {
        const res = await api.get(`/api/dentaltraining/${training.id}/registrations`);
        const regs = (res.data || []).map((reg: any) => ({
            TrainingTitle: training.title,
            TrainingDate: formatDate(training.date),
            Name: reg.first_name + ' ' + reg.last_name,
            Email: reg.email,
            RegisteredAt: formatDate(reg.registered_at),
        }));
        if (regs.length === 0) {
            alert('No registrations to export for this training.');
            return;
        }
        const ws = XLSX.utils.json_to_sheet(regs);
        const wb = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'Registrations');
        XLSX.writeFile(wb, `registrations_${training.title.replace(/[^a-z0-9]/gi, '_').toLowerCase()}.xlsx`);
    } catch (e) {
        alert('Failed to export registrations.');
    }
};

const fetchPackages = async (trainingId: number) => {
    try {
        const res = await api.get(`/api/dentaltrainingpackage/${trainingId}`);
        packages.value[trainingId] = res.data || [];
    } catch (e) {
        packages.value[trainingId] = [];
    }
};

const fetchAllPackages = async () => {
    for (const training of trainings.value) {
        await fetchPackages(training.id!);
    }
};

const openAddPackage = () => {
    packageEditMode.value = false;
    packageFormError.value = '';
    packageForm.value = { id: undefined, dentalTrainingId: addableTrainings.value.length > 0 ? addableTrainings.value[0].id : 0, name: '', price: 0 };
    showPackageForm.value = true;
};

const openEditPackage = (pkg: DentalTrainingPackageDTO) => {
    packageEditMode.value = true;
    packageFormError.value = '';
    packageForm.value = { ...pkg };
    showPackageForm.value = true;
};

const closePackageForm = () => {
    showPackageForm.value = false;
    packageFormError.value = '';
};

const submitPackageForm = async () => {
    packageFormError.value = '';
    if (!packageForm.value.name || packageForm.value.price == null || isNaN(packageForm.value.price)) {
        packageFormError.value = 'Name and valid price are required.';
        return;
    }
    try {
        if (packageEditMode.value && packageForm.value.id) {
            await api.put(`/api/dentaltrainingpackage/${packageForm.value.id}`, packageForm.value);
        } else {
            await api.post('/api/dentaltrainingpackage', packageForm.value);
        }
        showPackageForm.value = false;
        await fetchPackages(packageForm.value.dentalTrainingId);
    } catch (e) {
        packageFormError.value = 'Failed to save package.';
    }
};

const deletePackage = async (pkg: DentalTrainingPackageDTO) => {
    if (!confirm('Are you sure you want to delete this package?')) return;
    try {
        await api.delete(`/api/dentaltrainingpackage/${pkg.id}`);
        await fetchPackages(pkg.dentalTrainingId);
    } catch (e) {
        alert('Failed to delete package.');
    }
};

onMounted(async () => {
    await fetchTrainings();
    await fetchAllRegistrationsCount();
    await fetchAllPackages();
});
</script>