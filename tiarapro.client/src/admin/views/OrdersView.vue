<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { showErrorToast, showSuccessToast } from '../../utils/helpers.ts'
import api from '../../utils/Api.ts'

const orders = ref([])
const isLoading = ref(true)
const searchQuery = ref('')
const statusFilter = ref('all')
const showViewModal = ref(false)
const showDeleteModal = ref(false)
const currentOrder = ref(null)
const orderDetailsLoading = ref(false)
const orderItems = ref([])
const payments = ref([])

// Status options for filtering
const statusOptions = [
  { value: 'Pending', label: 'Pending' },
  { value: 'Processing', label: 'Processing' },
  { value: 'Shipped', label: 'Shipped' },
  { value: 'Delivered', label: 'Delivered' },
  { value: 'Cancelled', label: 'Cancelled' }
]

const selectedStatus = ref('')
const showStatusConfirmModal = ref(false)
const showPaymobNoteModal = ref(false)
const statusToUpdate = ref('')

// Filter orders based on search and status
const filteredOrders = computed(() => {
  return orders.value.filter(order => {
    const matchesSearch =
      order.id.toString().includes(searchQuery.value) ||
      (order.user?.email && order.user.email.toLowerCase().includes(searchQuery.value.toLowerCase())) ||
      (order.shippingAddress && order.shippingAddress.toLowerCase().includes(searchQuery.value.toLowerCase()))

    const matchesStatus = statusFilter.value === 'all' || order.status === statusFilter.value

    return matchesSearch && matchesStatus
  })
})

// Format date to a readable format
const formatDate = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return new Intl.DateTimeFormat('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

// Format currency
const formatCurrency = (amount) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'EGP'
  }).format(amount)
}

// Get CSS class for status badge
const getStatusClass = (status) => {
  switch (status) {
    case 'Delivered':
      return 'bg-green-100 text-green-800'
    case 'Shipped':
      return 'bg-blue-100 text-blue-800'
    case 'Processing':
      return 'bg-purple-100 text-purple-800'
    case 'Pending':
      return 'bg-yellow-100 text-yellow-800'
    case 'Cancelled':
      return 'bg-red-100 text-red-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

const getOrders = async () => {
  isLoading.value = true
  try {
    // Replace with your actual API endpoint
    // If you need to get orders for a specific user, you'll need to adjust this
    const response = await api.get('/api/order')
    if (response.status === 200) {
      orders.value = response.data.map(order => ({
        id: order.id,
        userId: order.user_id,
        status: order.status,
        totalAmount: order.total_amount,
        shippingAddress: `${order.shipping_address}, ${order.shipping_city}, ${order.shipping_state} ${order.shipping_postal_code}, ${order.shipping_country}`,
        shippingPhone: order.shipping_phone,
        createdAt: order.created_at,
        updatedAt: order.updated_at
      }))
    } else {
      showErrorToast('No Orders Yet')
    }
  } catch (error) {
    console.error('Error fetching orders:', error)
  } finally {
    isLoading.value = false
  }
}

const viewOrderDetails = async (order) => {
  currentOrder.value = order
  orderDetailsLoading.value = true
  showViewModal.value = true

  try {
    // Fetch detailed order information including items and payments
    debugger;
    const response = await api.get(`/api/order/${order.id}`)
    if (response.status === 200) {
      const detailedOrder = response.data
      var userResponse = await api.get(`/api/user/${detailedOrder.user_id}`)
      if (userResponse.status === 200) {
        detailedOrder.user = userResponse.data
      } else {
        console.error('Failed to fetch user details')
      }

      currentOrder.value = {
        ...order,
        ...detailedOrder
      }
      orderItems.value = detailedOrder.order_items || []
      payments.value = detailedOrder.payments || []
    } else {
      showErrorToast('Failed to fetch order details')
    }
  } catch (error) {
    console.error('Error fetching order details:', error)
    showErrorToast('Error fetching order details. Please try again.')
  } finally {
    orderDetailsLoading.value = false
  }
}

const confirmDeleteOrder = (order) => {
  currentOrder.value = order
  showDeleteModal.value = true
}

const deleteOrder = async () => {
  if (!currentOrder.value) return

  try {
    const response = await api.delete(`/api/order/${currentOrder.value.id}`)
    if (response.status === 200) {
      showSuccessToast('Order deleted successfully')
      orders.value = orders.value.filter(order => order.id !== currentOrder.value.id)
      showDeleteModal.value = false
      currentOrder.value = null
    } else {
      showErrorToast('Failed to delete order')
    }
  } catch (error) {
    console.error('Error deleting order:', error)
    showErrorToast('Error deleting order. Please try again.')
  }
}

const openStatusChange = () => {
  selectedStatus.value = currentOrder.value?.status || ''
}

const handleSaveStatus = () => {
  if (selectedStatus.value === 'Cancelled') {
    showPaymobNoteModal.value = true
  } else {
    showStatusConfirmModal.value = true
  }
  statusToUpdate.value = selectedStatus.value
}

const handlePaymobNext = () => {
  showPaymobNoteModal.value = false
  showStatusConfirmModal.value = true
}

const confirmStatusChange = async () => {
  showStatusConfirmModal.value = false
  try {
    const response = await api.put(`/api/order/${currentOrder.value.id}/status`, JSON.stringify(statusToUpdate.value), {
      headers: {
        'Content-Type': 'application/json'
      }
    })
    if (response.status === 200) {
      showSuccessToast('Order status updated successfully')
      currentOrder.value.status = statusToUpdate.value
      const orderIndex = orders.value.findIndex(o => o.id === currentOrder.value.id)
      if (orderIndex !== -1) {
        orders.value[orderIndex].status = statusToUpdate.value
      }
    } else {
      showErrorToast('Failed to update order status')
    }
  } catch (error) {
    showErrorToast('Error updating order status. Please try again.')
  }
}

onMounted(async () => {
  await getOrders()
})
</script>

<template>
  <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-9xl mx-auto">
    <!-- Header -->
    <div class="sm:flex sm:justify-between sm:items-center mb-8">
      <!-- Left: Title -->
      <div class="mb-4 sm:mb-0">
        <h1 class="text-2xl md:text-3xl font-extrabold text-slate-800">Orders</h1>
      </div>

      <!-- Right: Actions -->
      <div class="grid grid-flow-col sm:auto-cols-max justify-start sm:justify-end gap-2">
        <!-- Search -->
        <div class="relative">
          <input v-model="searchQuery" type="search" placeholder="Search orders..."
            class="form-input pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:border-primary focus:ring-primary" />
          <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
            <svg class="w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
          </div>
        </div>

        <!-- Status Filter -->
        <select v-model="statusFilter"
          class="form-select border border-slate-300 rounded-lg focus:border-primary focus:ring-primary py-2">
          <option v-for="option in statusOptions" :key="option.value" :value="option.value">
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

    <!-- Orders Cards (mobile view) -->
    <div v-if="!isLoading" class="grid grid-cols-1 gap-4 md:hidden">
      <div v-for="order in filteredOrders" :key="order.id"
        class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <div class="p-5">
          <div class="flex justify-between items-start mb-2">
            <div>
              <p class="text-sm text-slate-500">Order ID</p>
              <p class="font-medium text-slate-900">#{{ order.id }}</p>
            </div>
            <span :class="[
              'px-2 py-1 rounded-full text-xs font-medium',
              getStatusClass(order.status)
            ]">
              {{ order.status }}
            </span>
          </div>

          <div class="space-y-3 py-3 border-t border-b border-slate-100 my-3">
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Customer:</span>
              <span class="font-medium text-slate-700">{{ order.userId }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Date:</span>
              <span class="font-medium text-slate-700">{{ formatDate(order.createdAt) }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Total:</span>
              <span class="font-medium text-slate-900">{{ formatCurrency(order.totalAmount) }}</span>
            </div>
          </div>

          <div class="flex gap-2 mt-4">
            <button @click="viewOrderDetails(order)"
              class="flex-1 px-3 py-2 bg-primary/10 text-primary hover:bg-primary/20 rounded-lg transition-colors text-sm font-medium">
              View Details
            </button>
            <button @click="confirmDeleteOrder(order)"
              class="px-3 py-2 text-red-600 hover:text-red-700 hover:bg-red-50 rounded-lg transition-colors text-sm font-medium">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16">
                </path>
              </svg>
            </button>
          </div>
        </div>
      </div>

      <!-- Empty state for cards -->
      <div v-if="filteredOrders.length === 0" class="py-12 text-center">
        <div class="flex flex-col items-center">
          <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
            xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z">
            </path>
          </svg>
          <p class="text-slate-500 font-medium">No orders found</p>
          <p class="text-slate-400 text-sm mt-1">Try adjusting your search or filters</p>
        </div>
      </div>
    </div>

    <!-- Orders Table (desktop view) -->
    <div v-if="!isLoading" class="hidden md:block">
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-slate-200">
            <thead class="bg-slate-50">
              <tr>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Order ID
                </th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Customer
                </th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Date</th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Total</th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Status</th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-slate-200">
              <tr v-for="order in filteredOrders" :key="order.id" class="hover:bg-slate-50 transition-colors">
                <td class="px-6 py-4 whitespace-nowrap font-medium text-slate-800">#{{ order.id }}</td>
                <td class="px-6 py-4 whitespace-nowrap">{{ order.userId }}</td>
                <td class="px-6 py-4 whitespace-nowrap text-slate-600">{{ formatDate(order.createdAt) }}</td>
                <td class="px-6 py-4 whitespace-nowrap font-medium">{{ formatCurrency(order.totalAmount) }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="[
                    'px-2 py-1 rounded-full text-xs font-medium',
                    getStatusClass(order.status)
                  ]">
                    {{ order.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex space-x-3">
                    <button @click="viewOrderDetails(order)"
                      class="text-primary hover:text-primary-dark transition-colors">
                      <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                        xmlns="http://www.w3.org/2000/svg">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path>
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z">
                        </path>
                      </svg>
                    </button>
                    <button @click="confirmDeleteOrder(order)"
                      class="text-red-600 hover:text-red-800 transition-colors">
                      <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                        xmlns="http://www.w3.org/2000/svg">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16">
                        </path>
                      </svg>
                    </button>
                  </div>
                </td>
              </tr>

              <!-- Empty state for table -->
              <tr v-if="filteredOrders.length === 0">
                <td colspan="6" class="px-6 py-12 text-center">
                  <div class="flex flex-col items-center">
                    <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                      xmlns="http://www.w3.org/2000/svg">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z">
                      </path>
                    </svg>
                    <p class="text-slate-500 font-medium">No orders found</p>
                    <p class="text-slate-400 text-sm mt-1">Try adjusting your search or filters</p>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>

  <!-- Order Details Modal -->
  <div v-if="showViewModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
    <div class="bg-white rounded-xl shadow-xl max-w-3xl w-full p-6 max-h-[90vh] overflow-y-auto" @click.stop>
      <div class="flex justify-between items-start mb-6">
        <h3 class="text-lg font-bold text-slate-800">Order #{{ currentOrder?.id }}</h3>
        <button @click="showViewModal = false" class="text-slate-400 hover:text-slate-500">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>

      <!-- Loading indicator -->
      <div v-if="orderDetailsLoading" class="flex justify-center py-12">
        <svg class="animate-spin h-8 w-8 text-primary" xmlns="http://www.w3.org/2000/svg" fill="none"
          viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor"
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
          </path>
        </svg>
      </div>

      <div v-else>
        <!-- Order Summary -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
          <div class="bg-slate-50 p-4 rounded-lg">
            <h4 class="font-medium text-slate-800 mb-2">Order Information</h4>
            <div class="space-y-2 text-sm">
              <div class="flex justify-between">
                <span class="text-slate-500">Order ID:</span>
                <span class="font-medium">{{ currentOrder?.id }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-slate-500">Date:</span>
                <span>{{ formatDate(currentOrder?.createdAt) }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-slate-500">Last Updated:</span>
                <span>{{ formatDate(currentOrder?.updatedAt) }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-slate-500">Status:</span>
                <span :class="[
                  'px-2 py-1 rounded-full text-xs font-medium',
                  getStatusClass(currentOrder?.status)
                ]">{{ currentOrder?.status }}</span>
              </div>
            </div>
          </div>

          <div class="bg-slate-50 p-4 rounded-lg">
            <h4 class="font-medium text-slate-800 mb-2">Customer Information</h4>
            <div class="space-y-3 text-sm">
              <div class="flex justify-between">
                <span class="text-slate-500">Email:</span>
                <span class="font-medium">{{ currentOrder?.user?.email }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-slate-500">Name:</span>
                {{
                  [currentOrder?.user?.first_name, currentOrder?.user?.middle_name, currentOrder?.user?.last_name]
                    .filter(Boolean)
                    .join(' ')
                }}
              </div>


              <div class="flex justify-between">
                <span class="text-slate-500">Shipping Address:</span>
                <span class="text-right">{{ currentOrder?.shippingAddress }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-slate-500">Phone:</span>
                <span>{{ currentOrder?.shippingPhone }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Status Management -->
        <div class="mb-6 p-4 bg-slate-50 rounded-lg">
          <h4 class="font-medium text-slate-800 mb-2">Update Status</h4>
          <div class="flex gap-2 flex-wrap items-center">
            <select v-model="selectedStatus"
              class="form-select border border-slate-300 rounded-lg focus:border-primary focus:ring-primary py-2">
              <option v-for="option in statusOptions" :key="option.value" :value="option.value">
                {{ option.label }}
              </option>
            </select>
            <button :disabled="selectedStatus === currentOrder?.status" @click="handleSaveStatus"
              class="px-4 py-2 bg-primary text-white rounded-lg disabled:opacity-50 disabled:cursor-not-allowed">
              Save
            </button>
          </div>
        </div>

        <!-- Paymob Note Modal -->
        <div v-if="showPaymobNoteModal"
          class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
          <div class="bg-white rounded-xl shadow-xl max-w-md w-full p-6" @click.stop>
            <h3 class="text-lg font-bold text-slate-800 mb-4">Important Note</h3>
            <p class="text-slate-600 mb-6">
              This order was paid by credit card. Please ensure you refund the amount on Paymob before cancelling the
              order.
            </p>
            <div class="flex justify-end">
              <button @click="handlePaymobNext" class="px-4 py-2 bg-primary text-white rounded-lg">Next</button>
            </div>
          </div>
        </div>

        <!-- Status Confirm Modal -->
        <div v-if="showStatusConfirmModal"
          class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
          <div class="bg-white rounded-xl shadow-xl max-w-md w-full p-6" @click.stop>
            <h3 class="text-lg font-bold text-slate-800 mb-4">Confirm Status Change</h3>
            <p class="text-slate-600 mb-6">
              Are you sure you want to change the status to <span class="font-medium text-slate-800">{{ statusToUpdate
              }}</span>?
            </p>
            <div class="flex justify-end space-x-3">
              <button @click="showStatusConfirmModal = false"
                class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">Cancel</button>
              <button @click="confirmStatusChange" class="px-4 py-2 bg-primary text-white rounded-lg">Confirm</button>
            </div>
          </div>
        </div>

        <!-- Order Items -->
        <div class="mb-6">
          <h4 class="font-medium text-slate-800 mb-2">Order Items</h4>
          <div class="bg-white border border-slate-200 rounded-lg overflow-hidden">
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-slate-200">
                <thead class="bg-slate-50">
                  <tr>
                    <th scope="col"
                      class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Product
                    </th>
                    <th scope="col"
                      class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Price
                    </th>
                    <th scope="col"
                      class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Quantity
                    </th>
                    <th scope="col"
                      class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Subtotal
                    </th>
                  </tr>
                </thead>
                <tbody class="bg-white divide-y divide-slate-200">
                  <tr v-for="item in orderItems" :key="item.id" class="hover:bg-slate-50">
                    <td class="px-4 py-3">
                      <div class="flex items-center">
                        <div class="h-8 w-8 flex-shrink-0 mr-3">
                          <img
                            :src="item.product?.logoUrl || 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg'"
                            alt="Product" class="h-8 w-8 rounded object-cover border border-slate-200"
                            @error="(e) => (e.target as HTMLImageElement).src = 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg'" />
                        </div>
                        <div class="font-medium text-sm text-slate-800">{{ item.product_name || 'Product' }}</div>
                      </div>
                    </td>
                    <td class="px-4 py-3 text-sm">{{ formatCurrency(item.price) }}</td>
                    <td class="px-4 py-3 text-sm">{{ item.quantity }}</td>
                    <td class="px-4 py-3 text-sm font-medium">{{ formatCurrency(item.price * item.quantity) }}</td>
                  </tr>

                  <!-- Empty state -->
                  <tr v-if="orderItems.length === 0">
                    <td colspan="4" class="px-4 py-4 text-center text-sm text-slate-500">
                      No items found for this order.
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Payments -->
        <div class="mb-6">
          <h4 class="font-medium text-slate-800 mb-2">Payments</h4>
          <div class="bg-white border border-slate-200 rounded-lg overflow-hidden">
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-slate-200">
                <thead class="bg-slate-50">
                  <tr>
                    <th scope="col"
                      class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Payment
                      ID</th>
                    <th scope="col"
                      class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Method
                    </th>
                    <th scope="col"
                      class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Amount
                    </th>
                    <th scope="col"
                      class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Status
                    </th>
                    <th scope="col"
                      class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Date
                    </th>
                  </tr>
                </thead>
                <tbody class="bg-white divide-y divide-slate-200">
                  <tr v-for="payment in payments" :key="payment.id" class="hover:bg-slate-50">
                    <td class="px-4 py-3 text-sm font-medium">{{ payment.transaction_id }}</td>
                    <td class="px-4 py-3 text-sm">{{ payment.payment_method }}</td>
                    <td class="px-4 py-3 text-sm">{{ formatCurrency(payment.amount) }}</td>
                    <td class="px-4 py-3 text-sm"> {{ payment.payment_status }}</td>
                    <td class="px-4 py-3 text-sm">{{ formatDate(payment.created_at) }}</td>
                  </tr>

                  <!-- Empty state -->
                  <tr v-if="payments.length === 0">
                    <td colspan="5" class="px-4 py-4 text-center text-sm text-slate-500">
                      No payments found for this order.
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Order Summary -->
        <div class="bg-slate-50 p-4 rounded-lg mb-6">
          <h4 class="font-medium text-slate-800 mb-2">Order Summary</h4>
          <div class="space-y-2">
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Subtotal:</span>
              <span>{{ formatCurrency(currentOrder?.totalAmount || 0) }}</span>
            </div>
            <div class="flex justify-between font-medium pt-2 border-t border-slate-200 mt-2">
              <span>Total:</span>
              <span>{{ formatCurrency(currentOrder?.totalAmount || 0) }}</span>
            </div>
          </div>
        </div>

        <!-- Action buttons -->
        <div class="flex justify-end space-x-3 mt-6">
          <button @click="showViewModal = false"
            class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
            Close
          </button>
          <button @click="confirmDeleteOrder(currentOrder)"
            class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors">
            Delete Order
          </button>
        </div>
      </div>
    </div>
  </div>

  <!-- Delete confirmation modal -->
  <div v-if="showDeleteModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
    <div class="bg-white rounded-xl shadow-xl max-w-md w-full p-6" @click.stop>
      <h3 class="text-lg font-bold text-slate-800 mb-4">Confirm Deletion</h3>
      <p class="text-slate-600 mb-6">
        Are you sure you want to delete Order <span class="font-medium text-slate-800">#{{ currentOrder?.id }}</span>?
        This action cannot be undone.
      </p>
      <div class="flex justify-end space-x-3">
        <button @click="showDeleteModal = false"
          class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
          Cancel
        </button>
        <button @click="deleteOrder"
          class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors">
          Delete
        </button>
      </div>
    </div>
  </div>
</template>
