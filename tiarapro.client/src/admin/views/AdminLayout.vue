<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const isSidebarOpen = ref(true)

const webstoreExpanded = ref(true)
const dentalTrainingExpanded = ref(true)
const tiaraEventsExpanded = ref(true)
const tiaraAiExpanded = ref(true)

const webstoreNavigation = [
  {
    name: 'Dashboard',
    path: '/admin/dashboard',
    icon: 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6'
  },
  {
    name: 'Categories',
    path: '/admin/categories',
    icon: 'M4 6h16M4 10h16M4 14h16M4 18h16'
  },
  {
    name: 'Products',
    path: '/admin/products',
    icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4'
  },
  {
    name: 'Orders',
    path: '/admin/orders',
    icon: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2'
  },
  {
    name: 'Users',
    path: '/admin/users',
    icon: 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z'
  },
  {
    name: 'Payments',
    path: '/admin/payments',
    icon: 'M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z'
  },
  {
    name: 'Notifications',
    path: '/admin/notifications',
    icon: 'M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V4a2 2 0 10-4 0v1.341C8.67 6.165 8 7.388 8 8.75v5.408c0 .538-.214 1.055-.595 1.437L6 17h5m4 0v1a3 3 0 11-6 0v-1h6z'
  },
  {
    name: 'Promo Codes',
    path: '/admin/promocodes',
    icon: 'M12 8c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2zm0 10c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm0-18C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z'
  },
]

const dentalTrainingNav = [
  {
    name: 'Tiara Dental Training',
    path: '/admin/dental-training',
    icon: 'M12 14l9-5-9-5-9 5 9 5zM12 14l6.16-3.422a12.083 12.083 0 01.665 6.479A11.952 11.952 0 0012 20.055a11.952 11.952 0 00-6.824-2.998 12.078 12.078 0 01.665-6.479L12 14z'
  }
]

const tiaraEventsNav = [
  {
    name: 'Events',
    path: '/admin/events',
    icon: 'M8 2v4m8-4v4m-9 4h10m-12 2a2 2 0 012-2h10a2 2 0 012 2v8a2 2 0 01-2 2H5a2 2 0 01-2-2v-8z'
  }
]

const tiaraAiNav = [
  {
    name: 'TiaraAI Subscriptions',
    path: '/admin/tiaraai-subscriptions',
    icon: 'M13 16h-1v-4h-1m1-4h.01M12 20a8 8 0 100-16 8 8 0 000 16z'
  }
]

async function handleLogout() {
  localStorage.removeItem('isAuthenticated')
  router.push('/admin/login')
}
</script>

<template>
  <div class="min-h-screen bg-gray-100">
    <!-- Sidebar -->
    <div
      :class="`fixed inset-y-0 left-0 w-64 bg-white shadow-lg transform transition-transform duration-200 ease-in-out ${isSidebarOpen ? 'translate-x-0' : '-translate-x-full'}`"
      style="height: 100vh;" class="overflow-y-auto">
      <div class="flex h-16 items-center justify-between px-4 border-b">
        <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-logo-original.svg"
          alt="Tiara Logo" class="h-10 object-contain" /> <button @click="isSidebarOpen = !isSidebarOpen"
          class="lg:hidden">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
      <nav class="mt-6 px-4 space-y-6">
        <!-- Webstore Section -->
        <div>
          <div class="flex items-center mb-2 cursor-pointer select-none" @click="webstoreExpanded = !webstoreExpanded">
            <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-logo-original.svg"
              alt="Webstore Logo" class="h-7 w-7 mr-2 object-contain" />
            <span class="font-semibold text-gray-700 text-sm flex-1">Webstore</span>
            <svg :class="`h-4 w-4 transition-transform ${webstoreExpanded ? '' : '-rotate-90'}`" fill="none"
              stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
            </svg>
          </div>
          <div v-show="webstoreExpanded">
            <router-link v-for="item in webstoreNavigation" :key="item.name" :to="item.path"
              class="flex items-center px-4 py-3 text-gray-600 hover:bg-gray-50 rounded-lg mb-2 transition-colors"
              active-class="bg-primary/10 text-primary">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="item.icon" />
              </svg>
              {{ item.name }}
            </router-link>
          </div>
        </div>
        <!-- Dental Training Section -->
        <div>
          <div class="flex items-center mb-2 mt-4 cursor-pointer select-none"
            @click="dentalTrainingExpanded = !dentalTrainingExpanded">
            <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-DT-logo-light.svg"
              alt="Dental Training Logo" class="h-7 w-7 mr-2 object-contain" />
            <span class="font-semibold text-gray-700 text-sm flex-1">Tiara Dental Training</span>
            <svg :class="`h-4 w-4 transition-transform ${dentalTrainingExpanded ? '' : '-rotate-90'}`" fill="none"
              stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
            </svg>
          </div>
          <div v-show="dentalTrainingExpanded">
            <router-link v-for="item in dentalTrainingNav" :key="item.name" :to="item.path"
              class="flex items-center px-4 py-3 text-gray-600 hover:bg-gray-50 rounded-lg mb-2 transition-colors"
              active-class="bg-primary/10 text-primary">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="item.icon" />
              </svg>
              {{ item.name }}
            </router-link>
          </div>
        </div>

        <!-- Tiara Events Section -->
        <div>
          <div class="flex items-center mb-2 mt-4 cursor-pointer select-none"
            @click="tiaraEventsExpanded = !tiaraEventsExpanded">
            <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-Events-logo-original.svg"
              alt="Dental Training Logo" class="h-7 w-7 mr-2 object-contain" />
            <span class="font-semibold text-gray-700 text-sm flex-1">Tiara Events</span>
            <svg :class="`h-4 w-4 transition-transform ${tiaraEventsExpanded ? '' : '-rotate-90'}`" fill="none"
              stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
            </svg>
          </div>
          <div v-show="tiaraEventsExpanded">
            <router-link v-for="item in tiaraEventsNav" :key="item.name" :to="item.path"
              class="flex items-center px-4 py-3 text-gray-600 hover:bg-gray-50 rounded-lg mb-2 transition-colors"
              active-class="bg-primary/10 text-primary">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="item.icon" />
              </svg>
              {{ item.name }}
            </router-link>
          </div>
        </div>
        <!-- Tiara AI Section -->
        <div>
          <div class="flex items-center mb-2 mt-4 cursor-pointer select-none"
            @click="tiaraAiExpanded = !tiaraAiExpanded">
            <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-AI-logo-original.svg"
              alt="Tiara AI Logo" class="h-7 w-7 mr-2 object-contain" />
            <span class="font-semibold text-gray-700 text-sm flex-1">Tiara AI</span>
            <svg :class="`h-4 w-4 transition-transform ${tiaraAiExpanded ? '' : '-rotate-90'}`" fill="none"
              stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
            </svg>
          </div>
          <div v-show="tiaraAiExpanded">
            <router-link v-for="item in tiaraAiNav" :key="item.name" :to="item.path"
              class="flex items-center px-4 py-3 text-gray-600 hover:bg-gray-50 rounded-lg mb-2 transition-colors"
              active-class="bg-primary/10 text-primary">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="item.icon" />
              </svg>
              {{ item.name }}
            </router-link>
          </div>
        </div>
      </nav>
    </div>

    <!-- Main content -->
    <div :class="`${isSidebarOpen ? 'pl-64' : 'pl-0'} transition-all duration-200`">
      <header class="bg-white shadow-sm sticky top-0 z-10">
        <div class="flex h-16 items-center justify-between px-8">
          <div class="w-6 lg:w-auto">
            <button @click="isSidebarOpen = !isSidebarOpen" class="lg:hidden">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
          </div>
          <div class="flex items-center space-x-4">
            <button @click="handleLogout" class="btn">
              Logout
            </button>
          </div>
        </div>

      </header>

      <main class="p-8">
        <router-view v-slot="{ Component }">
          <transition enter-active-class="transition-opacity duration-200" enter-from-class="opacity-0"
            enter-to-class="opacity-100" leave-active-class="transition-opacity duration-200"
            leave-from-class="opacity-100" leave-to-class="opacity-0">
            <component :is="Component" />
          </transition>
        </router-view>
      </main>
    </div>
  </div>
</template>
