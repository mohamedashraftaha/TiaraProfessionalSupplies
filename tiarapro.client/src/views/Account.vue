<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { showErrorToast, showSuccessToast } from '../utils/helpers.ts'
import api from '../utils/Api.ts'

const router = useRouter()
const showLogoutModal = ref(false)
const user = ref({
  id: '',
  name: '',
  email: '',
  avatar: '',
  phone: 'N/A',
  memberSince: '',
  tiaraAiActive: false // Set as boolean
})

const isLoading = ref(false)

// Cache configuration
const CACHE_DURATION = 5 * 60 * 1000; // 5 minutes in milliseconds

const subscription = ref({
  plan: 'Tiara AI Complete',
  price: 'EGP 999/month',
  features: [
    'Unlimited AI Analyses',
    '5 Team Members'
  ]
})

const recentOrders = ref([])
const orders = ref([])

const stats = ref({
  totalOrders: {
    value: 0,
    change: '+12%'
  },
  aiAnalyses: {
    value: 128,
    change: '+24%'
  },
  upcomingEvents: {
    value: 3,
    nextDate: 'June 15'
  }
})

const activeSubscription = ref(null)

const formatDate = (dateString: string) => {
  if (!dateString) return 'N/A';
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'EGP',
    minimumFractionDigits: 2
  }).format(amount)
}

const GetUserOrders = async () => {
  // Check if we have cached orders
  const cachedOrders = localStorage.getItem('myOrders');
  const cacheTimestamp = localStorage.getItem('ordersCache_timestamp');

  if (cachedOrders && cacheTimestamp) {
    const now = Date.now();
    const cacheAge = now - parseInt(cacheTimestamp);

    if (cacheAge < CACHE_DURATION) {
      console.log('Using cached orders');
      return JSON.parse(cachedOrders);
    }
  }

  isLoading.value = true;
  try {
    const userId = localStorage.getItem('userId')
    const response = await api.get(`/api/order/user/${userId}`)
    const processedOrders = response.data.map(order => {
      const shipping = 150
      const items = order.order_items.map(item => ({
        name: item.product_name,
        quantity: item.quantity,
        price: item.price,
        image: item.product_image != undefined || item.product_image != null ? item.product_image : "https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg",
      }))
      items.push({
        name: 'Shipping Fee',
        quantity: 1,
        price: shipping,
        image: 'https://img.icons8.com/?size=100&id=6M1qqDqQTtRd&format=png&color=000000'
      })
      return {
        id: `ORD-${order.id}`,
        date: formatDate(order.created_at),
        total: formatCurrency(order.total_amount),
        status: order.status === 'Success' ? 'Delivered' : order.status,
        items: items
      }
    })

    // Cache the processed orders
    localStorage.setItem('cachedOrders', JSON.stringify(processedOrders));
    localStorage.setItem('ordersCache_timestamp', Date.now().toString());
    console.log('Orders fetched and cached');

    return processedOrders;
  } catch (error) {
    console.error("Error while getting user's orders", error)
    showErrorToast("Something went wrong. Please try again")
    return []
  }
  finally {
    isLoading.value = false;
  }
}

const startSubscription = () => {
  router.push('/ai')
}

const confirmLogout = () => {
  showLogoutModal.value = true
}

const handleLogout = () => {
  // Clear all localStorage items
  localStorage.clear();
  localStorage.setItem("isAuthenticated", "false")
  localStorage.setItem("token", "null")
  localStorage.setItem("userId", "0")



  // Clear cache data
  clearCache()

  showSuccessToast("Logged out successfully")

  // Redirect to login page after a short delay
  setTimeout(() => {
    router.push('/login')
  }, 1000)
}

const GetActiveSubscription = async () => {
  // Check if we have cached subscription
  const cachedSubscription = localStorage.getItem('cachedActiveSubscription');
  const cacheTimestamp = localStorage.getItem('subscriptionCache_timestamp');

  if (cachedSubscription && cacheTimestamp) {
    const now = Date.now();
    const cacheAge = now - parseInt(cacheTimestamp);

    if (cacheAge < CACHE_DURATION) {
      console.log('Using cached subscription');
      const subscription = JSON.parse(cachedSubscription);
      if (subscription) {
        activeSubscription.value = subscription;
        user.value.tiaraAiActive = true;
      } else {
        activeSubscription.value = null;
        user.value.tiaraAiActive = false;
      }
      return;
    }
  }

  try {
    debugger;
    const userId = localStorage.getItem('userId')
    const response = await api.get(`/api/tiaraaisubscription/user/${userId}/active`)
    if (response.status === 200) {
      activeSubscription.value = response.data
      user.value.tiaraAiActive = true

      // Cache the subscription data
      localStorage.setItem('cachedActiveSubscription', JSON.stringify(response.data));
      localStorage.setItem('subscriptionCache_timestamp', Date.now().toString());
      console.log('Subscription fetched and cached');
    }
  } catch (error) {
    console.log('No active subscription found')
    activeSubscription.value = null
    user.value.tiaraAiActive = false

    // Cache the null result
    localStorage.setItem('cachedActiveSubscription', JSON.stringify(null));
    localStorage.setItem('subscriptionCache_timestamp', Date.now().toString());
  }
}

// Function to clear cache when needed
const clearCache = () => {
  localStorage.removeItem('cachedOrders');
  localStorage.removeItem('ordersCache_timestamp');
  localStorage.removeItem('cachedActiveSubscription');
  localStorage.removeItem('subscriptionCache_timestamp');
}

onMounted(async () => {

  debugger;

  user.value = {
    id: localStorage.getItem('userId') || '',
    name: `${localStorage.getItem('firstName')} ${localStorage.getItem('middleName')} ${localStorage.getItem('lastName')}`.trim() || 'Unknown',
    email: localStorage.getItem('email') || localStorage.getItem('userEmail') || '',
    avatar: '',
    phone: localStorage.getItem('PhoneNumber') || 'N/A',
    memberSince: formatDate(localStorage.getItem('memberSince') || ''),
    tiaraAiActive: localStorage.getItem('tiaraAiActive') === 'true' // Fix boolean conversion

  }
  const userOrders = await GetUserOrders()
  orders.value = userOrders
  recentOrders.value = userOrders.slice(0, 3)
  stats.value.totalOrders.value = userOrders.length

  // Check for active subscription
  await GetActiveSubscription()

  localStorage.setItem("myOrders", JSON.stringify(orders.value))
})
</script>

<template>
  <div class="min-h-screen bg-slate-50">
    <main class="container mx-auto px-4 py-8">
      <!-- Page Header -->
      <div class="flex flex-col md:flex-row justify-between items-start md:items-center mb-8 gap-4">
        <div class="flex items-center">
          <div>
            <h2 class="text-2xl font-bold text-slate-800">My Account</h2>
            <p class="text-slate-600">Welcome back, {{ user.name }}</p>
          </div>
        </div>
        <div class="flex gap-3">
          <button class="bg-primary text-white px-5 py-2 rounded-lg hover:bg-primary-dark transition shadow-sm"
            @click="router.push('/my-orders')">
            My Orders
          </button>
          <!-- <button class="border border-primary text-primary px-5 py-2 rounded-lg hover:bg-primary/5 transition">
            Edit Profile
          </button> -->
          <button @click="confirmLogout"
            class="bg-slate-100 hover:bg-slate-200 text-slate-700 px-4 py-2 rounded-lg transition-colors flex items-center space-x-2">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"></path>
            </svg>
            <span>Logout</span>
          </button>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Left Column -->
        <div>
          <!-- Profile Card -->
          <div class="bg-white rounded-xl shadow-sm p-6 mb-6">
            <div class="flex items-center mb-6">
              <div
                class="w-16 h-16 bg-primary rounded-full flex items-center justify-center text-xl font-bold text-white">
                {{user.name.split(' ').map(n => n[0]).join('')}}
              </div>
              <div class="ml-4">
                <h3 class="text-lg font-semibold text-slate-800">{{ user.name }}</h3>
                <p class="text-slate-600 text-sm">{{ user.email }}</p>
                <p class="text-slate-500 text-sm mt-1">Member since {{ user.memberSince }}</p>
              </div>
            </div>

            <div class="space-y-4 text-sm">
              <div class="pt-4 border-t border-slate-100">
                <span class="text-slate-500 block mb-1">Tiara AI Status</span>
                <p class="font-medium" :class="user.tiaraAiActive ? 'text-green-600' : 'text-red-500'">
                  {{ user.tiaraAiActive ? 'Active' : 'Inactive' }}
                </p>
              </div>
            </div>
          </div>

          <!-- Stats -->
          <div class="grid grid-cols-1 gap-4">
            <div class="bg-white rounded-xl shadow-sm p-6">
              <div class="flex items-center justify-between mb-2">
                <h4 class="text-slate-600 font-medium">Total Orders</h4>
                <span class="text-green-600 text-sm font-medium">{{ stats.totalOrders.change }}</span>
              </div>
              <p class="text-2xl font-bold text-slate-800">{{ stats.totalOrders.value }}</p>
            </div>
          </div>
        </div>

        <!-- Right Column -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Subscription Card -->
          <div class="bg-white rounded-xl shadow-sm overflow-hidden">
            <div class="p-6">
              <div class="flex items-center justify-between mb-2">
                <div>
                  <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-AI-logo-original.svg"
                    alt="Tiara AI" class="h-8 mb-2">
                  <p v-if="activeSubscription" class="text-green-500 font-medium">Active</p>
                  <p v-else class="text-red-500 font-medium">Inactive</p>
                </div>
                <div v-if="activeSubscription" class="text-right">
                  <p class="text-sm text-slate-500">{{ activeSubscription.subscription?.name }}</p>
                  <p class="text-lg font-bold text-slate-800">{{ activeSubscription.segmentations_used }}/{{
                    activeSubscription.segmentations_allowed }}</p>
                  <p class="text-xs text-slate-500">Segmentations used</p>
                </div>
              </div>
            </div>

            <div v-if="!activeSubscription" class="bg-gradient-to-br from-slate-50 to-slate-100 p-6 text-center">
              <h3 class="text-xl font-semibold mb-4 text-slate-800">Unlock the Power of AI in Your Practice</h3>
              <p class="text-slate-600 mb-6 max-w-lg mx-auto">Transform your dental practice with advanced AI-powered
                diagnostics and treatment planning.</p>
              <button @click="startSubscription"
                class="bg-primary text-white px-8 py-3 rounded-lg font-semibold hover:bg-primary-dark transition shadow-sm">
                Subscribe Now
              </button>
            </div>

            <div v-else class="bg-gradient-to-br from-green-50 to-green-100 p-6">
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4 text-center">
                <div>
                  <p class="text-sm text-green-600 font-medium">Plan</p>
                  <p class="text-lg font-bold text-green-800">{{ activeSubscription.subscription?.name }}</p>
                </div>
                <div>
                  <p class="text-sm text-green-600 font-medium">Remaining</p>
                  <p class="text-lg font-bold text-green-800">{{ activeSubscription.segmentations_allowed -
                    activeSubscription.segmentations_used }}</p>
                </div>
                <div>
                  <p class="text-sm text-green-600 font-medium">Status</p>
                  <p class="text-lg font-bold text-green-800">Active</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Recent Orders -->
          <div class="bg-white rounded-xl shadow-sm overflow-hidden">
            <div class="p-6 flex justify-between items-center border-b border-slate-100">
              <h3 class="font-semibold text-slate-800">Recent Orders</h3>
              <button class="text-primary hover:text-primary-dark transition text-sm font-medium"
                @click="router.push('/my-orders')">
                View All Orders
              </button>
            </div>

            <div class="overflow-x-auto">
              <table class="w-full">
                <thead>
                  <tr class="text-left text-slate-500 border-b border-slate-100">
                    <th class="px-6 py-3 font-medium">Order ID</th>
                    <th class="px-6 py-3 font-medium">Date</th>
                    <th class="px-6 py-3 font-medium">Items</th>
                    <th class="px-6 py-3 font-medium">Total</th>
                    <th class="px-6 py-3 font-medium">Status</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-if="isLoading" class="text-center">
                    <td colspan="5" class="py-8">
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 mx-auto text-slate-400 animate-spin"
                        fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M12 3v3M12 18v3M4.22 4.22l2.12 2.12M16.66 16.66l2.12 2.12M3 12h3M18 12h3M4.22 19.78l2.12-2.12M16.66 7.34l2.12-2.12" />
                      </svg>
                      <p class="text-slate-500 mt-2">Loading orders...</p>
                    </td>
                  </tr>
                  <tr v-else-if="recentOrders.length === 0" class="text-center">
                    <td colspan="5" class="py-8">
                      <svg class="w-12 h-12 text-slate-300 mx-auto mb-4" fill="none" stroke="currentColor"
                        viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path>
                      </svg>
                      <p class="text-slate-500 font-medium">No orders found</p>
                      <p class="text-slate-400 text-sm mt-1">Your order history will appear here</p>
                    </td>
                  </tr>
                  <tr v-else v-for="order in recentOrders" :key="order.id"
                    class="border-b border-slate-100 hover:bg-slate-50 transition">
                    <td class="px-6 py-4">
                      <span class="font-medium text-primary">{{ order.id }}</span>
                    </td>
                    <td class="px-6 py-4 text-slate-600">{{ order.date }}</td>
                    <td class="px-6 py-4 text-slate-600">
                      <div class="grid grid-cols-1 sm:grid-cols-2 gap-2">
                        <div v-for="(item, index) in order.items" :key="index"
                          class="flex items-center gap-2 bg-slate-50 p-2 rounded-lg shadow-sm">
                          <img :src="item.image" alt="Item image" class="w-10 h-10 object-cover rounded" />
                          <div class="text-sm">
                            <div class="font-medium text-slate-700">{{ item.name }}</div>
                            <div class="text-slate-500">x{{ item.quantity }}</div>
                          </div>
                        </div>
                      </div>
                    </td>
                    <td class="px-6 py-4 font-medium text-slate-800">{{ order.total }}</td>
                    <td class="px-6 py-4">
                      <span class="px-3 py-1 bg-green-100 text-green-700 rounded-full text-sm font-medium">
                        {{ order.status }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Footer -->
    <footer class="bg-white border-t border-slate-200 py-4 mt-8">
      <div class="container mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center text-sm text-slate-500">
          © 2025 Tiara. All rights reserved.
        </div>
      </div>
    </footer>
  </div>

  <!-- Logout confirmation modal -->
  <div v-if="showLogoutModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
    <div class="bg-white rounded-xl shadow-xl max-w-md w-full p-6" @click.stop>
      <h3 class="text-lg font-bold text-slate-800 mb-4">Confirm Logout</h3>
      <p class="text-slate-600 mb-6">
        Are you sure you want to logout? You will need to login again to access your account.
      </p>
      <div class="flex justify-end space-x-3">
        <button @click="showLogoutModal = false"
          class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
          Cancel
        </button>
        <button @click="handleLogout"
          class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors">
          Logout
        </button>
      </div>
    </div>
  </div>
</template>
