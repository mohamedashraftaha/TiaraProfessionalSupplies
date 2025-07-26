<template>
  <div class="p-6 bg-gray-50 min-h-screen">
    <div class="max-w-5xl mx-auto">
      <!-- Header Section -->
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-2xl font-bold text-gray-800">Notifications</h1>
        <button @click="openAddModal = true"
                class="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-lg transition duration-200 flex items-center gap-2 shadow-sm">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          Add Notification
        </button>
      </div>

      <!-- Search & Filter Bar -->
      <div class="bg-white p-4 rounded-lg shadow-sm mb-6">
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <svg class="h-5 w-5 text-gray-400"
                 xmlns="http://www.w3.org/2000/svg"
                 viewBox="0 0 20 20"
                 fill="currentColor">
              <path fill-rule="evenodd"
                    d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z"
                    clip-rule="evenodd" />
            </svg>
          </div>
          <input v-model="search"
                 @input="onSearchChange"
                 placeholder="Search notifications..."
                 class="border border-gray-200 rounded-lg pl-10 pr-4 py-3 w-full focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition duration-200" />
        </div>
      </div>

      <!-- Notifications Grid -->
      <div v-if="notifications.length > 0" class="space-y-4">
        <div v-for="notification in paginatedNotifications"
             :key="notification.id"
             class="bg-white p-5 rounded-lg shadow-sm border-l-4 hover:shadow-md transition duration-200"
             :class="{
            'bg-green-100 text-green-800': notification.category === 'NewProduct' || notification.category === 'TiaraAiUpdates',
            'bg-blue-100 text-blue-800': notification.category === 'NewFeature' || notification.category === 'NewEvent',
            'bg-orange-100 text-orange-800': notification.category === 'NewTraining',
            'border-gray-300': !notification.active
          }">
          <div class="flex justify-between items-start">
            <div class="space-y-2 flex-1">
              <div class="flex items-center gap-2">
                <h3 class="text-lg font-semibold text-gray-800">{{ notification.title }}</h3>
                <span class="text-xs px-2 py-1 rounded-full font-medium"
                      :class="{
                    'bg-green-100 text-green-800': notification.category === 'NewProduct' || notification.category === 'TiaraAiUpdates',
                    'bg-blue-100 text-blue-800': notification.category === 'NewFeature' || notification.category === 'NewEvent',
                    'bg-orange-100 text-orange-800': notification.category === 'NewTraining',
                    'bg-gray-100 text-gray-800': !['Feature', 'System', 'Update'].includes(notification.category)
                  }">
                  {{ notificationCategories.find(i => i.id == notification.category)?.name || 'Unknown Category' }}
                </span>
              </div>
              <p class="text-sm text-gray-600 leading-relaxed">{{ notification.message }}</p>
            </div>
            <div class="flex flex-col items-end gap-2">
              <button @click="toggleStatus(notification)"
                      class="text-xs px-3 py-1 rounded-full font-medium transition duration-200"
                      :class="{
                  'bg-green-100 text-green-800 hover:bg-green-200': notification.active,
                  'bg-gray-100 text-gray-800 hover:bg-gray-200': !notification.active
                }">
                {{ notification.active ? 'Active' : 'Inactive' }}
              </button>
              <button @click="deleteNotification(notification.id)"
                      class="text-xs text-red-500 hover:text-red-700 flex items-center gap-1 transition duration-200">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
                Delete
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="bg-white p-8 rounded-lg shadow-sm text-center">
        <svg xmlns="http://www.w3.org/2000/svg"
             class="h-12 w-12 mx-auto text-gray-400 mb-4"
             fill="none"
             viewBox="0 0 24 24"
             stroke="currentColor">
          <path stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
        </svg>
        <h3 class="text-lg font-medium text-gray-900 mb-1">No notifications found</h3>
        <p class="text-gray-500">Try adjusting your search criteria or add new notifications.</p>
      </div>

      <!-- Pagination Controls -->
      <div class="flex justify-between items-center mt-6 bg-white p-4 rounded-lg shadow-sm">
        <button @click="prevPage"
                :disabled="currentPage === 1"
                class="px-4 py-2 rounded text-sm font-medium transition duration-200"
                :class="currentPage === 1 ? 'text-gray-400 cursor-not-allowed' : 'text-indigo-600 hover:bg-indigo-50'">
          <div class="flex items-center gap-1">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
            </svg>
            Previous
          </div>
        </button>
        <span class="text-sm text-gray-700">
          Page {{ currentPage }} of {{ totalPages || 1 }}
        </span>
        <button @click="nextPage"
                :disabled="currentPage === totalPages || totalPages === 0"
                class="px-4 py-2 rounded text-sm font-medium transition duration-200"
                :class="currentPage === totalPages || totalPages === 0 ? 'text-gray-400 cursor-not-allowed' : 'text-indigo-600 hover:bg-indigo-50'">
          <div class="flex items-center gap-1">
            Next
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
            </svg>
          </div>
        </button>
      </div>
    </div>

    <!-- Add Notification Modal -->
    <div v-if="openAddModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg max-w-md w-full p-6 relative shadow-lg">
        <h2 class="text-xl font-semibold mb-4">Add Notification</h2>
        <form @submit.prevent="addNotification" class="space-y-4">
          <div>
            <label for="title" class="block font-medium mb-1">Title</label>
            <input id="title"
                   v-model="newNotification.title"
                   type="text"
                   required
                   class="w-full border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-indigo-500" />
          </div>
          <div>
            <label for="message" class="block font-medium mb-1">Message</label>
            <textarea id="message"
                      v-model="newNotification.message"
                      required
                      rows="3"
                      class="w-full border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-indigo-500"></textarea>
          </div>
          <div>
            <label for="category" class="block font-medium mb-1">Category</label>
            <select id="category"
                    v-model="newNotification.category"
                    required
                    class="w-full border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-indigo-500">
              <option disabled value="">Select Category</option>
              <option value="NewProduct">New Product</option>
              <option value="NewFeature">New Feature</option>
              <option value="NewEvent">New Event</option>
              <option value="NewTraining">New Training</option>
              <option value="TiaraAiUpdates">New Tiara AI Updates</option>
            </select>
          </div>
          <div class="flex justify-end gap-3 mt-6">
            <button type="button"
                    @click="openAddModal = false"
                    class="px-4 py-2 rounded border border-gray-300 hover:bg-gray-100">
              Cancel
            </button>
            <button type="submit"
                    class="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded transition duration-200">
              Add
            </button>
          </div>
        </form>
        <button @click="openAddModal = false"
                class="absolute top-3 right-3 text-gray-400 hover:text-gray-700">
          <svg xmlns="http://www.w3.org/2000/svg"
               class="h-6 w-6"
               fill="none"
               viewBox="0 0 24 24"
               stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
    </div>
  </div>
</template>
<script setup>
  import { ref, computed, watch } from 'vue'
  import axios from 'axios'
  import api from '../../utils/Api.ts'
  import { showErrorToast, showSuccessToast, categoriesConst } from '../../utils/helpers.ts'

  const search = ref('')
  const openAddModal = ref(false)
  const currentPage = ref(1)
  const itemsPerPage = 5
  const notifications = ref([])

  const fetchNotifications = async () => {
    try {
      // Use the user notifications endpoint for per-user read status
      const userId = localStorage.getItem('userId');
      const response = await api.get(`/api/notifications/user/${userId}`);
      notifications.value = response.data;
    } catch (error) {
      console.error(`Error fetching notifications: ${error}`)
    }
  }

  // Fetch on component mount
  fetchNotifications()

  // Filtered notifications based on search term
  const filteredNotifications = computed(() => {
    if (!search.value) return notifications.value
    const term = search.value.toLowerCase()
    return notifications.value.filter(n =>
      n.title.toLowerCase().includes(term) ||
      n.message.toLowerCase().includes(term) ||
      n.category.toLowerCase().includes(term)
    )
  })

  // Pagination computations
  const totalPages = computed(() => Math.ceil(filteredNotifications.value.length / itemsPerPage))

  const paginatedNotifications = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage
    return filteredNotifications.value.slice(start, start + itemsPerPage)
  })

  // Reset page if currentPage is out of bounds after filtering
  watch(filteredNotifications, () => {
    if (currentPage.value > totalPages.value) {
      currentPage.value = totalPages.value || 1
    }
  })

  const notificationCategories = [
    {
      id: "0",
      name: "New Product"
    },
    {
      id: "1",
      name: "New Feature"
    },
    {
      id: "2",
      name: "New Event"
    },
    {
      id: "3",
      name: "New Training"
    },
    {
      id: "4",
      name: "Tiara AI Updates"
    },

  ]
  // Pagination controls
  const nextPage = () => {
    if (currentPage.value < totalPages.value) currentPage.value++
  }
  const prevPage = () => {
    if (currentPage.value > 1) currentPage.value--
  }

  // Reset to first page when search changes
  const onSearchChange = () => {
    currentPage.value = 1
  }

  // New notification form data
  const newNotification = ref({
    title: '',
    message: '',
    category: '',
    active: true,
  })

  // Add new notification
  const addNotification = async () => {
    if (!newNotification.value.title || !newNotification.value.message || !newNotification.value.category) {
      showErrorToast('Please fill all fields.')
      return
    }
    try {
      const response = await api.post('/api/notifications', newNotification.value)
      notifications.value.unshift(response.data)
      newNotification.value = { title: '', message: '', category: '', active: true }
      openAddModal.value = false
    } catch (error) {
      console.error('Error adding notification:', error)
      showErrorToast('Failed to add notification.')
    }
  }

  // Toggle notification active status
  const toggleStatus = async (notification) => {
    try {
      const updated = { ...notification, active: !notification.active }
      await api.put(`/api/notifications/${notification.id}`, updated)
      notification.active = !notification.active
    } catch (error) {
      console.error('Error updating notification status:', error)
      alert('Failed to update status.')
    }
  }

  // Delete notification
  const deleteNotification = async (id) => {
    if (!confirm('Are you sure you want to delete this notification?')) return
    try {
      await api.delete(`/api/notifications/${id}`)
      notifications.value = notifications.value.filter(n => n.id !== id)
    } catch (error) {
      console.error('Error deleting notification:', error)
      alert('Failed to delete notification.')
    }
  }
</script>
