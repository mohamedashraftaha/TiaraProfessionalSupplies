<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { showErrorToast, showSuccessToast } from '../../utils/helpers.ts'
import api from '../../utils/Api.ts'

const users = ref([])
const isLoading = ref(true)
const searchQuery = ref('')
const roleFilter = ref('all')
const showViewModal = ref(false)
const showAddModal = ref(false) // New modal state
const currentUser = ref(null)
const isEditing = ref(false)
const userSubscription = ref(null)
const isLoadingSubscription = ref(false)
const editForm = ref({
  firstName: '',
  middleName: '',
  lastName: '',
  email: '',
  phone: '',
  address: '',
  city: '',
  password_hash: '',
  state: '',
  postalCode: '',
  country: '',
  role: '',
  tiaraAiActive: false
})

// New user form & related states
const newUserForm = ref({
  firstName: '',
  middleName: '',
  lastName: '',
  email: '',
  password: '',
  phone: '',
  address: '',
  city: '',
  state: '',
  postalCode: '',
  country: '',
  role: 'User'
})
const showPassword = ref(false)
const isAddingUser = ref(false)

// Role options for filtering
const roleOptions = [
  { value: 'all', label: 'All Roles' },
  { value: 'User', label: 'User' },
  { value: 'Admin', label: 'Admin' }
]

// Filter users based on search and role
const filteredUsers = computed(() => {
  return users.value.filter(user => {
    const matchesSearch =
      user.email.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      (user.firstName + ' ' + user.middleName + ' ' + user.lastName).toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      (user.phone && user.phone.includes(searchQuery.value))

    const matchesRole = roleFilter.value === 'all' || user.role === roleFilter.value

    return matchesSearch && matchesRole
  })
})

// Format date to a readable format
const formatDate = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return new Intl.DateTimeFormat('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  }).format(date)
}

// Calculate time since account creation (e.g., "2 months ago")
const getTimeSince = (dateString) => {
  if (!dateString) return ''

  const now = new Date().getTime()
  const createdDate = new Date(dateString).getTime()
  const diffTime = Math.abs(now - createdDate)
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))

  if (diffDays < 1) return 'Today'
  if (diffDays === 1) return 'Yesterday'
  if (diffDays < 30) return `${diffDays} days ago`
  if (diffDays < 365) {
    const months = Math.floor(diffDays / 30)
    return `${months} ${months === 1 ? 'month' : 'months'} ago`
  }

  const years = Math.floor(diffDays / 365)
  return `${years} ${years === 1 ? 'year' : 'years'} ago`
}

// Get background color for AI status badge
const getAiStatusClass = (active) => {
  return active ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
}

// Get users from API
const getUsers = async () => {
  isLoading.value = true
  try {
    const response = await api.get('/api/user')
    if (response.status === 200) {
      users.value = response.data.map(user => ({
        id: user.id,
        email: user.email,
        firstName: user.first_name || '',
        middleName: user.middle_name || '',
        lastName: user.last_name || '',
        fullName: `${user.first_name || ''} ${user.middle_name || ''} ${user.last_name || ''}`.trim(),
        phone: user.phone,
        role: user.role,
        address: user.address,
        city: user.city,
        state: user.state,
        postalCode: user.postal_code,
        country: user.country,
        tiaraAiActive: user.tiara_ai_active || false,
        createdAt: user.created_at,
        isVerified: user.is_verified
      }))
    } else {
      showErrorToast('Failed to fetch users')
    }
  } catch (error) {
    console.error('Error fetching users:', error)
  } finally {
    isLoading.value = false
  }
}

// Open add user modal
const openAddUserModal = () => {
  // Reset form
  newUserForm.value = {
    firstName: '',
    middleName: '',
    lastName: '',
    email: '',
    password: '',
    phone: '',
    address: '',
    city: '',
    state: '',
    postalCode: '',
    country: '',
    role: 'User'
  }
  showPassword.value = false
  showAddModal.value = true
}

// Handle add user
const handleAddUser = async () => {
  isAddingUser.value = true
  try {
    const userPayload = {
      first_name: newUserForm.value.firstName,
      middle_name: newUserForm.value.middleName,
      last_name: newUserForm.value.lastName,
      email: newUserForm.value.email,
      password_hash: newUserForm.value.password,
      role: newUserForm.value.role,
      address: newUserForm.value.address,
      city: newUserForm.value.city,
      state: newUserForm.value.state,
      postal_code: newUserForm.value.postalCode,
      country: newUserForm.value.country,
      phone: newUserForm.value.phone
    }

    const response = await api.post('/api/user/signup', userPayload)

    if (response.status === 201) {
      showSuccessToast('User added successfully')

      // Refresh user list
      await getUsers()

      // Close modal
      showAddModal.value = false
    } else if (response.status === 409) {
      showErrorToast('Email already exists. Please use a different email.')
    } else {
      showErrorToast(`Failed to add user: ${response.data?.message || 'Unknown error'}`)
    }
  } catch (error) {
    console.error('Error adding user:', error)
    showErrorToast('Error adding user. Please try again.')
  } finally {
    isAddingUser.value = false
  }
}

// Fetch user subscription data
const fetchUserSubscription = async (userId) => {
  isLoadingSubscription.value = true
  userSubscription.value = null

  try {
    const response = await api.get(`/api/tiaraaisubscription/user/${userId}/active`)
    if (response.status === 200) {
      userSubscription.value = response.data
    }
  } catch (error) {
    console.log('No active subscription found for user:', userId)
    userSubscription.value = null
  } finally {
    isLoadingSubscription.value = false
  }
}

// View/edit user details
const viewUserDetails = (user) => {
  console.log("HERE")
  currentUser.value = user

  console.log(user)
  // Initialize edit form with current values
  editForm.value = {
    firstName: user.firstName || '',
    middleName: user.middleName || '',
    lastName: user.lastName || '',
    email: user.email || '',
    phone: user.phone || '',
    address: user.address || '',
    city: user.city || '',
    state: user.state || '',
    postalCode: user.postalCode || '',
    country: user.country || '',
    role: user.role || 'User',
    tiaraAiActive: user.tiaraAiActive || false,
    password_hash: user.passwordHash || ''
  }

  // Fetch user subscription data
  fetchUserSubscription(user.id)

  // Show the modal in edit mode
  isEditing.value = true
  showViewModal.value = true
}

// Cancel editing
const cancelEditing = () => {
  showViewModal.value = false
}

// Save user changes
const saveUserChanges = async () => {
  if (!currentUser.value) return

  try {
    const payload = {
      first_name: editForm.value.firstName,
      middle_name: editForm.value.middleName,
      last_name: editForm.value.lastName,
      email: editForm.value.email,
      phone: editForm.value.phone,
      address: editForm.value.address,
      city: editForm.value.city,
      state: editForm.value.state,
      postal_code: editForm.value.postalCode,
      country: editForm.value.country,
      role: editForm.value.role,
      tiara_ai_active: editForm.value.tiaraAiActive,
      password_hash: editForm.value.password_hash
    }

    const response = await api.post(`/api/user/updateUser`, payload)
    if (response.status === 200 || response.status === 201) {
      showSuccessToast('User updated successfully')

      // Update the user in the local array
      const userIndex = users.value.findIndex(u => u.id === currentUser.value.id)
      if (userIndex !== -1) {
        users.value[userIndex] = {
          ...users.value[userIndex],
          firstName: editForm.value.firstName,
          middleName: editForm.value.middleName,
          lastName: editForm.value.lastName,
          fullName: `${editForm.value.firstName || ''} ${editForm.value.middleName || ''} ${editForm.value.lastName || ''}`.trim(),
          email: editForm.value.email,
          phone: editForm.value.phone,
          address: editForm.value.address,
          city: editForm.value.city,
          state: editForm.value.state,
          postalCode: editForm.value.postalCode,
          country: editForm.value.country,
          role: editForm.value.role,
          tiaraAiActive: editForm.value.tiaraAiActive
        }
      }

      showViewModal.value = false
    } else {
      showErrorToast('Failed to update user')
    }
  } catch (error) {
    console.error('Error updating user:', error)
    showErrorToast('Error updating user. Please try again.')
  }
}

// Toggle Tiara AI status directly from the list
const toggleTiaraAi = async (userId, currentStatus) => {
  try {
    const response = await api.put(`/api/user/${userId}/tiara-ai`, { active: !currentStatus })
    if (response.status === 200) {
      showSuccessToast(`Tiara AI ${!currentStatus ? 'activated' : 'deactivated'} successfully`)

      // Update the user in the local array
      const userIndex = users.value.findIndex(u => u.id === userId)
      if (userIndex !== -1) {
        users.value[userIndex].tiaraAiActive = !currentStatus

        // If we're viewing the details of this user, update that too
        if (currentUser.value && currentUser.value.id === userId) {
          currentUser.value.tiaraAiActive = !currentStatus
          editForm.value.tiaraAiActive = !currentStatus
        }
      }
    } else {
      showErrorToast('Failed to update Tiara AI status')
    }
  } catch (error) {
    console.error('Error updating Tiara AI status:', error)
    showErrorToast('Error updating Tiara AI status. Please try again.')
  }
}

onMounted(async () => {
  await getUsers()
})
</script>

<template>
  <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-9xl mx-auto">
    <!-- Header -->
    <div class="sm:flex sm:justify-between sm:items-center mb-8">
      <!-- Left: Title -->
      <div class="mb-4 sm:mb-0">
        <h1 class="text-2xl md:text-3xl font-extrabold text-slate-800">Users Management</h1>
        <p class="text-slate-500 mt-1">Manage user accounts, roles, and Tiara AI access</p>
      </div>

      <!-- Right: Actions -->
      <div class="grid grid-flow-col sm:auto-cols-max justify-start sm:justify-end gap-2">
        <!-- Search -->
        <div class="relative">
          <input v-model="searchQuery" type="search" placeholder="Search users..."
            class="form-input pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:border-primary focus:ring-primary" />
          <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
            <svg class="w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
          </div>
        </div>

        <!-- Role Filter -->
        <select v-model="roleFilter"
          class="form-select border border-slate-300 rounded-lg focus:border-primary focus:ring-primary py-2">
          <option v-for="option in roleOptions" :key="option.value" :value="option.value">
            {{ option.label }}
          </option>
        </select>

        <!-- Export button (optional) -->
        <button
          class="btn bg-white border-slate-200 hover:border-slate-300 text-slate-600 rounded-lg px-4 py-2 inline-flex items-center space-x-2 shadow-sm transition duration-150 ease-in-out">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"></path>
          </svg>
          <span>Export</span>
        </button>

        <!-- Add User button -->
        <button @click="openAddUserModal"
          class="btn bg-primary hover:bg-primary-dark text-white rounded-lg px-4 py-2 inline-flex items-center space-x-2 shadow-sm transition duration-150 ease-in-out">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
          </svg>
          <span>Add User</span>
        </button>
      </div>
    </div>

    <!-- Loading indicator -->
    <div v-if="isLoading" class="flex justify-center py-12">
      <svg class="animate-spin h-8 w-8 text-primary" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor"
          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
        </path>
      </svg>
    </div>

    <!-- Users Cards (mobile view) -->
    <div v-if="!isLoading" class="grid grid-cols-1 gap-4 md:hidden">
      <div v-for="user in filteredUsers" :key="user.id"
        class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <div class="p-5">
          <div class="flex justify-between items-start mb-2">
            <div>
              <p class="font-medium text-slate-900">{{ user.fullName || 'Unnamed User' }}</p>
              <p class="text-slate-500 text-sm">{{ user.email }}</p>
            </div>
            <span :class="[
              'px-2 py-1 rounded-full text-xs font-medium',
              getAiStatusClass(user.tiaraAiActive)
            ]">
              {{ user.tiaraAiActive ? 'AI Active' : 'AI Inactive' }}
            </span>
          </div>

          <div class="space-y-3 py-3 border-t border-b border-slate-100 my-3">
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Role:</span>
              <span class="font-medium text-slate-700">{{ user.role }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Member since:</span>
              <span class="font-medium text-slate-700">{{ formatDate(user.createdAt) }}</span>
            </div>
            <div v-if="user.address || user.city" class="flex justify-between text-sm">
              <span class="text-slate-500">Location:</span>
              <span class="font-medium text-slate-700">{{ user.city || '' }}{{ user.city && user.country ? ', ' : ''
              }}{{ user.country || '' }}</span>
            </div>
          </div>

          <div class="flex gap-2 mt-4">
            <button @click="viewUserDetails(user)"
              class="flex-1 px-3 py-2 bg-primary text-white hover:bg-primary-dark rounded-lg transition-colors text-sm font-medium">
              Edit User
            </button>
            <button @click="toggleTiaraAi(user.id, user.tiaraAiActive)"
              class="flex-1 px-3 py-2 bg-slate-100 text-slate-700 hover:bg-slate-200 rounded-lg transition-colors text-sm font-medium">
              {{ user.tiaraAiActive ? 'Disable AI' : 'Enable AI' }}
            </button>
          </div>
        </div>
      </div>

      <!-- Empty state for cards -->
      <div v-if="filteredUsers.length === 0" class="py-12 text-center">
        <div class="flex flex-col items-center">
          <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
            xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z">
            </path>
          </svg>
          <p class="text-slate-500 font-medium">No users found</p>
          <p class="text-slate-400 text-sm mt-1">Try adjusting your search or filters</p>
        </div>
      </div>
    </div>

    <!-- Users Table (desktop view) -->
    <div v-if="!isLoading" class="hidden md:block">
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-slate-200">
            <thead class="bg-slate-50">
              <tr>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">User</th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Role</th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Joined</th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Location
                </th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Tiara AI
                </th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-slate-200">
              <tr v-for="user in filteredUsers" :key="user.id" class="hover:bg-slate-50 transition-colors">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div
                      class="h-10 w-10 bg-primary/10 rounded-full flex items-center justify-center text-primary font-medium">
                      {{ user.firstName?.[0] || '' }}{{ user.lastName?.[0] || '' }}
                    </div>
                    <div class="ml-4">
                      <div class="font-medium text-slate-800">{{ user.fullName || 'Unnamed User' }}</div>
                      <div class="text-slate-500 text-sm">{{ user.email }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="px-2 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800">
                    {{ user.role }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-slate-600">
                  <div>{{ formatDate(user.createdAt) }}</div>
                  <div class="text-slate-400 text-xs">{{ getTimeSince(user.createdAt) }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-slate-600">
                  {{ user.city || 'N/A' }}{{ user.city && user.country ? ', ' : '' }}{{ user.country || '' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="[
                    'px-2 py-1 rounded-full text-xs font-medium',
                    getAiStatusClass(user.tiaraAiActive)
                  ]">
                    {{ user.tiaraAiActive ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex space-x-3">
                    <button @click="viewUserDetails(user)"
                      class="text-primary hover:text-primary-dark transition-colors" title="Edit User">
                      <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                        xmlns="http://www.w3.org/2000/svg">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z">
                        </path>
                      </svg>
                    </button>
                    <button @click="toggleTiaraAi(user.id, user.tiaraAiActive)"
                      class="text-slate-600 hover:text-slate-800 transition-colors"
                      :title="user.tiaraAiActive ? 'Disable AI' : 'Enable AI'">
                      <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                        xmlns="http://www.w3.org/2000/svg">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z">
                        </path>
                      </svg>
                    </button>
                  </div>
                </td>
              </tr>

              <!-- Empty state for table -->
              <tr v-if="filteredUsers.length === 0">
                <td colspan="6" class="px-6 py-12 text-center">
                  <div class="flex flex-col items-center">
                    <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                      xmlns="http://www.w3.org/2000/svg">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z">
                      </path>
                    </svg>
                    <p class="text-slate-500 font-medium">No users found</p>
                    <p class="text-slate-400 text-sm mt-1">Try adjusting your search or filters</p>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Add User Modal -->
    <div v-if="showAddModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div class="bg-white rounded-xl shadow-xl max-w-3xl w-full p-6 max-h-[90vh] overflow-y-auto" @click.stop>
        <div class="flex justify-between items-start mb-6">
          <h3 class="text-lg font-bold text-slate-800">Add New User</h3>
          <button @click="showAddModal = false" class="text-slate-400 hover:text-slate-500">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>

        <form @submit.prevent="handleAddUser">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
            <!-- Basic Information -->
            <div>
              <h4 class="font-medium text-slate-800 mb-4">Basic Information</h4>
              <div class="space-y-4">
                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-1">First Name</label>
                    <div class="relative">
                      <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                        <svg class="h-5 w-5 text-slate-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                          fill="currentColor">
                          <path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z"
                            clip-rule="evenodd" />
                        </svg>
                      </div>
                      <input v-model="newUserForm.firstName" type="text" required placeholder="John"
                        class="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-1">Last Name</label>
                    <input v-model="newUserForm.lastName" type="text" required placeholder="Doe"
                      class="w-full px-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
                  </div>
                </div>

                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1">Email</label>
                  <div class="relative">
                    <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                      <svg class="h-5 w-5 text-slate-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                        fill="currentColor">
                        <path d="M2.003 5.884L10 9.882l7.997-3.998A2 2 0 0016 4H4a2 2 0 00-1.997 1.884z" />
                        <path d="M18 8.118l-8 4-8-4V14a2 2 0 002 2h12a2 2 0 002-2V8.118z" />
                      </svg>
                    </div>
                    <input v-model="newUserForm.email" type="email" required placeholder="you@example.com"
                      class="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
                  </div>
                </div>

                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1">Password</label>
                  <div class="relative">
                    <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                      <svg class="h-5 w-5 text-slate-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                        fill="currentColor">
                        <path fill-rule="evenodd"
                          d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z"
                          clip-rule="evenodd" />
                      </svg>
                    </div>
                    <input v-model="newUserForm.password" :type="showPassword ? 'text' : 'password'" required
                      placeholder="••••••••"
                      class="w-full pl-10 pr-10 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
                    <div class="absolute inset-y-0 right-0 pr-3 flex items-center">
                      <button type="button" @click="showPassword = !showPassword"
                        class="text-slate-400 hover:text-slate-600 focus:outline-none">
                        <svg v-if="showPassword" class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                          fill="currentColor">
                          <path d="M10 12a2 2 0 100-4 2 2 0 000 4z" />
                          <path fill-rule="evenodd"
                            d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z"
                            clip-rule="evenodd" />
                        </svg>
                        <svg v-else class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                          fill="currentColor">
                          <path fill-rule="evenodd"
                            d="M3.707 2.293a1 1 0 00-1.414 1.414l14 14a1 1 0 001.414-1.414l-1.473-1.473A10.014 10.014 0 0019.542 10C18.268 5.943 14.478 3 10 3a9.958 9.958 0 00-4.512 1.074l-1.78-1.781zm4.261 4.26l1.514 1.515a2.003 2.003 0 012.45 2.45l1.514 1.514a4 4 0 00-5.478-5.478z"
                            clip-rule="evenodd" />
                          <path
                            d="M12.454 16.697L9.75 13.992a4 4 0 01-3.742-3.741L2.335 6.578A9.98 9.98 0 00.458 10c1.274 4.057 5.065 7 9.542 7 .847 0 1.669-.105 2.454-.303z" />
                        </svg>
                      </button>
                    </div>
                  </div>
                  <p class="mt-1 text-xs text-slate-500">Password must be at least 8 characters</p>
                </div>

                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1">Phone Number</label>
                  <div class="relative">
                    <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                      <svg class="h-5 w-5 text-slate-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                        fill="currentColor">
                        <path
                          d="M2 3a1 1 0 011-1h2.153a1 1 0 01.986.836l.74 4.435a1 1 0 01-.54 1.06l-1.548.773a11.037 11.037 0 006.105 6.105l.774-1.548a1 1 0 011.059-.54l4.435.74a1 1 0 01.836.986V17a1 1 0 01-1 1h-2C7.82 18 2 12.18 2 5V3z" />
                      </svg>
                    </div>
                    <input v-model="newUserForm.phone" type="text" required placeholder="+201XXXXXXXXX"
                      class="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
                  </div>
                </div>

                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1">Role</label>
                  <select v-model="newUserForm.role"
                    class="w-full px-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary">
                    <option value="User">User</option>
                    <option value="Admin">Admin</option>
                  </select>
                </div>
              </div>
            </div>

            <!-- Address Information -->
            <div>
              <h4 class="font-medium text-slate-800 mb-4">Address Information</h4>
              <div class="space-y-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1">Address</label>
                  <div class="relative">
                    <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                      <svg class="h-5 w-5 text-slate-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                        fill="currentColor">
                        <path fill-rule="evenodd"
                          d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z"
                          clip-rule="evenodd" />
                      </svg>
                    </div>
                    <input v-model="newUserForm.address" type="text" required placeholder="123 Main St"
                      class="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
                  </div>
                </div>

                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-1">Country</label>
                    <input v-model="newUserForm.country" type="text" required placeholder="Egypt"
                      class="w-full px-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-1">State/Governorate</label>
                    <input v-model="newUserForm.state" type="text" required placeholder="Cairo"
                      class="w-full px-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
                  </div>
                </div>

                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-1">City</label>
                    <input v-model="newUserForm.city" type="text" required placeholder="Nasr City"
                      class="w-full px-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-1">Postal Code</label>
                    <input v-model="newUserForm.postalCode" type="text" placeholder="11111"
                      class="w-full px-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Action buttons -->
          <div class="flex justify-end space-x-3 mt-6">
            <button type="button" @click="showAddModal = false"
              class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
              Cancel
            </button>
            <button type="submit" :disabled="isAddingUser"
              class="w-full bg-primary text-white py-3 rounded-xl hover:bg-primary-dark transition flex justify-center items-center shadow-md">
              <span v-if="!isAddingUser">Create Account</span>
              <svg v-else class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none"
                viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z" />
              </svg>
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- User Edit Modal -->
    <div v-if="showViewModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div class="bg-white rounded-xl shadow-xl max-w-3xl w-full p-6 max-h-[90vh] overflow-y-auto" @click.stop>
        <div class="flex justify-between items-start mb-6">
          <h3 class="text-lg font-bold text-slate-800">Edit User</h3>
          <button @click="showViewModal = false" class="text-slate-400 hover:text-slate-500">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>

        <!-- Loading indicator removed since no longer needed -->

        <div>
          <!-- TiaraAI Subscription Information -->
          <div class="mb-6">
            <div class="bg-gradient-to-r from-blue-50 to-indigo-50 rounded-xl border border-blue-200 overflow-hidden">
              <div class="px-6 py-4 bg-gradient-to-r from-blue-600 to-indigo-600">
                <div class="flex items-center space-x-3">
                  <div class="w-10 h-10 bg-white/20 rounded-lg flex items-center justify-center">
                    <svg class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                  </div>
                  <div>
                    <h4 class="text-lg font-bold text-white">TiaraAI Subscription</h4>
                    <p class="text-blue-100 text-sm">AI-powered dental diagnostics subscription status</p>
                  </div>
                </div>
              </div>

              <!-- Loading State -->
              <div v-if="isLoadingSubscription" class="p-6">
                <div class="flex items-center justify-center space-x-2">
                  <svg class="animate-spin h-5 w-5 text-blue-600" xmlns="http://www.w3.org/2000/svg" fill="none"
                    viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor"
                      d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                    </path>
                  </svg>
                  <span class="text-slate-600">Loading subscription data...</span>
                </div>
              </div>

              <!-- Active Subscription -->
              <div v-else-if="userSubscription" class="p-6">
                <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
                  <!-- Status Card -->
                  <div class="bg-green-50 border border-green-200 rounded-lg p-4">
                    <div class="flex items-center space-x-2 mb-2">
                      <div class="w-3 h-3 bg-green-500 rounded-full animate-pulse"></div>
                      <span class="text-green-700 font-medium text-sm">ACTIVE</span>
                    </div>
                    <p class="text-2xl font-bold text-green-800">{{ (userSubscription.subscription?.name) || 'Premium Plan' }}</p>
                    <p class="text-green-600 text-sm">Subscription is active</p>
                  </div>

                  <!-- Usage Card -->
                  <div class="bg-blue-50 border border-blue-200 rounded-lg p-4">
                    <div class="flex items-center justify-between mb-2">
                      <span class="text-blue-700 font-medium text-sm">USAGE</span>
                      <span class="text-xs text-blue-600 bg-blue-100 px-2 py-1 rounded-full">
                        {{ Math.round((userSubscription.segmentations_used / userSubscription.segmentations_allowed) *
                          100) }}%
                      </span>
                    </div>
                    <p class="text-2xl font-bold text-blue-800">
                      {{ userSubscription.segmentations_used }}/{{ userSubscription.segmentations_allowed }}
                    </p>
                    <p class="text-blue-600 text-sm">AI segmentations used</p>
                  </div>

                  <!-- Remaining Card -->
                  <div class="bg-purple-50 border border-purple-200 rounded-lg p-4">
                    <div class="flex items-center space-x-2 mb-2">
                      <svg class="w-4 h-4 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd"
                          d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-8.293l-3-3a1 1 0 00-1.414 0l-3 3a1 1 0 001.414 1.414L9 9.414V13a1 1 0 102 0V9.414l1.293 1.293a1 1 0 001.414-1.414z"
                          clip-rule="evenodd"></path>
                      </svg>
                      <span class="text-purple-700 font-medium text-sm">REMAINING</span>
                    </div>
                    <p class="text-2xl font-bold text-purple-800">
                      {{ userSubscription.segmentations_allowed - userSubscription.segmentations_used }}
                    </p>
                    <p class="text-purple-600 text-sm">Segmentations left</p>
                  </div>
                </div>

                <!-- Progress Bar -->
                <div class="mb-4">
                  <div class="flex justify-between text-sm text-slate-600 mb-2">
                    <span>Usage Progress</span>
                    <span>{{ userSubscription.segmentations_used }} of {{ userSubscription.segmentations_allowed }}
                      used</span>
                  </div>
                  <div class="w-full bg-gray-200 rounded-full h-3">
                    <div
                      class="bg-gradient-to-r from-blue-500 to-purple-600 h-3 rounded-full transition-all duration-300"
                      :style="{ width: Math.min((userSubscription.segmentations_used / userSubscription.segmentations_allowed) * 100, 100) + '%' }">
                    </div>
                  </div>
                </div>

                <!-- Subscription Details -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4 pt-4 border-t border-slate-200">
                  <div>
                    <p class="text-sm text-slate-500 mb-1">Subscription ID</p>
                    <p class="font-medium text-slate-800">#{{ userSubscription.id }}</p>
                  </div>
                  <div>
                    <p class="text-sm text-slate-500 mb-1">Order ID</p>
                    <p class="font-medium text-slate-800">#{{ userSubscription.order_id }}</p>
                  </div>
                  <div>
                    <p class="text-sm text-slate-500 mb-1">Subscribed Date</p>
                    <p class="font-medium text-slate-800">{{ formatDate(userSubscription.subscribed_at) }}</p>
                  </div>
                  <div v-if="userSubscription.expires_at">
                    <p class="text-sm text-slate-500 mb-1">Expires Date</p>
                    <p class="font-medium text-slate-800">{{ formatDate(userSubscription.expires_at) }}</p>
                  </div>
                </div>
              </div>

              <!-- No Subscription -->
              <div v-else class="p-6">
                <div class="text-center py-8">
                  <div class="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <svg class="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M18.364 5.636l-3.536 3.536m0 5.656l3.536 3.536M9.172 9.172L5.636 5.636m3.536 9.192L5.636 18.364M12 2.25a9.75 9.75 0 110 19.5 9.75 9.75 0 010-19.5z">
                      </path>
                    </svg>
                  </div>
                  <h5 class="text-lg font-semibold text-slate-700 mb-2">No Active Subscription</h5>
                  <p class="text-slate-500 text-sm mb-4">This user doesn't have an active TiaraAI subscription.</p>
                  <div
                    class="inline-flex items-center space-x-2 text-sm text-slate-600 bg-slate-100 px-3 py-2 rounded-lg">
                    <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd"
                        d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
                        clip-rule="evenodd"></path>
                    </svg>
                    <span>User can purchase a subscription to access AI features</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Edit user form -->
          <div>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
              <!-- Basic Information -->
              <div>
                <h4 class="font-medium text-slate-800 mb-4">Basic Information</h4>
                <div class="space-y-4">
                  <div class="grid grid-cols-2 gap-4">
                    <div>
                      <label class="block text-sm font-medium text-slate-700 mb-1">First Name</label>
                      <input v-model="editForm.firstName" type="text"
                        class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary" />
                    </div>
                    <div>
                      <label class="block text-sm font-medium text-slate-700 mb-1">Middle Name</label>
                      <input v-model="editForm.middleName" type="text"
                        class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary" />
                    </div>

                    <div>
                      <label class="block text-sm font-medium text-slate-700 mb-1">Last Name</label>
                      <input v-model="editForm.lastName" type="text"
                        class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary" />
                    </div>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-1">Email</label>
                    <input v-model="editForm.email" type="email"
                      class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary" />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-1">Phone</label>
                    <input v-model="editForm.phone" type="text"
                      class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary" />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-1">Role</label>
                    <select v-model="editForm.role"
                      class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary">
                      <option value="User">User</option>
                      <option value="Admin">Admin</option>
                    </select>
                  </div>

                  <div>
                    <label class="flex items-center">
                      <input type="checkbox" v-model="editForm.tiaraAiActive"
                        class="form-checkbox h-5 w-5 text-primary rounded border-slate-300 focus:ring-primary" />
                      <span class="ml-2 text-sm text-slate-700">Tiara AI Active</span>
                    </label>
                  </div>
                </div>
              </div>

              <!-- Address Information -->
              <div>
                <h4 class="font-medium text-slate-800 mb-4">Address Information</h4>
                <div class="space-y-4">
                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-1">Address</label>
                    <input v-model="editForm.address" type="text"
                      class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary" />
                  </div>

                  <div class="grid grid-cols-2 gap-4">
                    <div>
                      <label class="block text-sm font-medium text-slate-700 mb-1">City</label>
                      <input v-model="editForm.city" type="text"
                        class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary" />
                    </div>
                    <div>
                      <label class="block text-sm font-medium text-slate-700 mb-1">State</label>
                      <input v-model="editForm.state" type="text"
                        class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary" />
                    </div>
                  </div>

                  <div class="grid grid-cols-2 gap-4">
                    <div>
                      <label class="block text-sm font-medium text-slate-700 mb-1">Postal Code</label>
                      <input v-model="editForm.postalCode" type="text"
                        class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary" />
                    </div>
                    <div>
                      <label class="block text-sm font-medium text-slate-700 mb-1">Country</label>
                      <input v-model="editForm.country" type="text"
                        class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary" />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Action buttons -->
          <div class="flex justify-end space-x-3 mt-6">
            <button @click="cancelEditing"
              class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
              Cancel
            </button>
            <button @click="saveUserChanges"
              class="px-4 py-2 bg-primary text-white rounded-lg hover:bg-primary-dark transition-colors">
              Save Changes
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
