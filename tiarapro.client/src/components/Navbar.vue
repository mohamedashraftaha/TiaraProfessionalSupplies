<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useCartStore } from '../stores/CartStore'
import { storeToRefs } from 'pinia'
import { useRouter, useRoute } from 'vue-router'
import api from '../utils/Api.ts'
const router = useRouter()
const route = useRoute()
const isMenuOpen = ref(false)
const isNotificationsOpen = ref(false)
const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value
}
const toggleNotifications = () => {
  isNotificationsOpen.value = !isNotificationsOpen.value
}
const cartStore = useCartStore();
const { totalItems } = storeToRefs(cartStore);
console.log("TOTAL ITEMS", totalItems.value)
// Tab navigation for main features
const navTabs = [
  { name: 'Tiara Dental Training', route: '/training' },
  // { name: 'Events', route: '/events' },
  { name: 'Tiara AI', route: '/ai' }
]
// Notification logic
const userId = localStorage.getItem('userId')
const unreadCount = ref(0)
const fetchUnreadCount = async () => {
  if (!userId) return
  try {
    const response = await api.get(`/api/notifications/user/${userId}`)
    unreadCount.value = response.data.filter((n: any) => !n.isRead).length
  } catch {
    unreadCount.value = 0
  }
}
onMounted(() => {
  fetchUnreadCount()
  window.addEventListener('refresh-unread-count', fetchUnreadCount)
})
// Function to navigate to the respective category route
</script>

<template>
  <header class="bg-white shadow-sm">
    <div class="bg-[#1e3a8a] text-white py-2 text-center text-sm">
      Professional Dental Supplies
      <div class="absolute right-4 top-2 space-x-4 hidden sm:block">
        <router-link to="/contact" class="hover:text-gray-200">
          Customer Support: +20 24500090

        </router-link>
      </div>
    </div>

    <nav class="container mx-auto px-4 py-4">
      <!-- Desktop Navigation (unchanged) -->
      <div class="hidden lg:flex items-center justify-between">
        <div>
          <router-link to="/">
            <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-logo-original.svg"
              alt="Tiara Logo" class="h-20 w-full object-fit" />
          </router-link>
        </div>

        <div class="flex-1 max-w-xl px-4">
          <div class="flex justify-center space-x-8">
            <div class="flex border-b border-gray-200">
              <router-link v-for="tab in navTabs" :key="tab.route" :to="tab.route"
                class="px-6 py-2 text-lg font-medium focus:outline-none transition-colors border-b-2"
                :class="route.path === tab.route ? 'border-blue-600 text-blue-600' : 'border-transparent text-gray-600 hover:text-blue-600'">
                {{ tab.name }}
              </router-link>
            </div>
          </div>
        </div>

        <div class="flex items-center space-x-6">
          <!-- Notifications Button -->
          <router-link to="/notifications" class="relative text-gray-700 hover:text-primary">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M15 17h5l-1.405-1.405A2 2 0 0018 14V9a6 6 0 10-12 0v5a2 2 0 00-1.595 1.595L4 17h5m6 0v1a2 2 0 11-4 0v-1m-2 0v1a4 4 0 008 0v-1M9 6h6" />
            </svg>
            <span v-if="unreadCount > 0"
              class="absolute -top-2 -right-2 bg-red-600 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
              {{ unreadCount }}
            </span>
          </router-link>

          <!-- Cart Button -->
          <router-link to="/cart" class="relative text-gray-700 hover:text-primary">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
            <span v-if="totalItems > 0"
              class="absolute -top-2 -right-2 bg-primary text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
              {{ totalItems }}
            </span>
          </router-link>

          <!-- Account Button -->
          <router-link to="/account" class="text-gray-700 hover:text-primary">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
          </router-link>
        </div>
      </div>

      <!-- Mobile Navigation -->
      <div class="lg:hidden">
        <!-- Mobile Header Row -->
        <div class="flex items-center justify-between">
          <!-- Logo -->
          <div class="flex-shrink-0">
            <router-link to="/">
              <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-logo-original.svg"
                alt="Tiara Logo" class="h-12 w-auto" />
            </router-link>
          </div>

          <!-- Mobile Actions -->
          <div class="flex items-center space-x-4">
            <!-- Cart Button -->
            <router-link to="/cart" class="relative text-gray-700 hover:text-primary">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
              <span v-if="totalItems > 0"
                class="absolute -top-2 -right-2 bg-primary text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
                {{ totalItems }}
              </span>
            </router-link>

            <!-- Account Button -->
            <router-link to="/account" class="text-gray-700 hover:text-primary">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
            </router-link>

            <!-- Mobile Menu Button -->
            <button @click="toggleMenu" class="text-gray-700 hover:text-primary">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
          </div>
        </div>

        <!-- Mobile Categories Row -->
        <div class="mt-4 border-t pt-4">
          <div class="flex justify-center space-x-4 overflow-x-auto">
            <div class="flex w-full border-b border-gray-200">
              <router-link v-for="tab in navTabs" :key="tab.route" :to="tab.route"
                class="flex-1 px-4 py-2 text-base font-medium text-center focus:outline-none transition-colors border-b-2"
                :class="route.path === tab.route ? 'border-blue-600 text-blue-600' : 'border-transparent text-gray-600 hover:text-blue-600'">
                {{ tab.name }}
              </router-link>
            </div>
          </div>
        </div>

        <!-- Mobile Menu Dropdown -->
        <div v-if="isMenuOpen" class="mt-4 border-t pt-4 space-y-4">
          <router-link to="/notifications" class="flex items-center space-x-3 text-gray-700 hover:text-primary"
            @click="isMenuOpen = false">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M15 17h5l-1.405-1.405A2 2 0 0018 14V9a6 6 0 10-12 0v5a2 2 0 00-1.595 1.595L4 17h5m6 0v1a2 2 0 11-4 0v-1m-2 0v1a4 4 0 008 0v-1M9 6h6" />
            </svg>
            <span>Notifications</span>
            <span v-if="unreadCount > 0"
              class="ml-2 bg-red-600 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
              {{ unreadCount }}
            </span>
          </router-link>

          <router-link to="/contact" class="flex items-center space-x-3 text-gray-700 hover:text-primary"
            @click="isMenuOpen = false">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
            </svg>
            <span>
              Customer Support: +20 24500090
            </span>
          </router-link>
        </div>
      </div>
    </nav>
  </header>
</template>
