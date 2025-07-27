<script setup lang="ts">
  import { ref, onMounted, computed } from 'vue'
  import { showErrorToast } from '../../utils/helpers.ts'
  import api from '../../utils/Api.ts'

  const payments = ref([])
  const isLoading = ref(true)
  const searchQuery = ref('')
  const statusFilter = ref('all')
  const showViewModal = ref(false)
  const currentPayment = ref(null)

  const sortColumn = ref('created_at')
  const sortDirection = ref('desc')

  const setSort = (column) => {
    if (sortColumn.value === column) {
      sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
    } else {
      sortColumn.value = column
      sortDirection.value = 'asc'
    }
  }

  const sortedPayments = computed(() => {
    const sorted = [...filteredPayments.value]
    sorted.sort((a, b) => {
      let valA = a[sortColumn.value]
      let valB = b[sortColumn.value]
      if (valA === undefined || valA === null) valA = ''
      if (valB === undefined || valB === null) valB = ''
      if (sortColumn.value.toLowerCase().includes('date') || sortColumn.value.toLowerCase().includes('at')) {
        valA = new Date(valA)
        valB = new Date(valB)
      }
      if (valA < valB) return sortDirection.value === 'asc' ? -1 : 1
      if (valA > valB) return sortDirection.value === 'asc' ? 1 : -1
      return 0
    })
    return sorted
  })

  // Status options for filtering
  const statusOptions = [
    { value: 'all', label: 'All Statuses' },
    { value: 'succeeded', label: 'Succeeded' },
    { value: 'pending', label: 'Pending' },
    { value: 'failed', label: 'Failed' }
  ]

  // Filter payments based on search and status
  const filteredPayments = computed(() => {
    return payments.value.filter(payment => {
      const matchesSearch =
        (payment.transaction_id && payment.transaction_id.includes(searchQuery.value.toLowerCase())) ||
        payment.order_id.toString().includes(searchQuery.value)

      const matchesStatus = statusFilter.value === 'all' || payment.status === statusFilter.value

      return matchesSearch && matchesStatus
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

  // Format currency
  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'EGP',
      minimumFractionDigits: 2
    }).format(amount)
  }

  // Get CSS class for status badge
  const getStatusClass = (status) => {
    switch (status) {
      case 'succeeded':
        return 'bg-green-100 text-green-800'
      case 'pending':
        return 'bg-yellow-100 text-yellow-800'
      case 'failed':
        return 'bg-red-100 text-red-800'
      default:
        return 'bg-gray-100 text-gray-800'
    }
  }

  // View payment details
  const viewPaymentDetails = (payment) => {
    currentPayment.value = payment
    showViewModal.value = true
  }

  // Close modal
  const closeModal = () => {
    showViewModal.value = false
  }

  // Get payments from API
  const getPayments = async () => {
    isLoading.value = true
    try {
      const response = await api.get('/api/payment')
      if (response.status === 200) {
        payments.value = response.data.map(payment => ({
          id: payment.id,
          transaction_id: payment.transaction_id || `TXN-${payment.id}`,
          order_id: payment.order_id,
          amount: payment.amount,
          payment_method: payment.payment_method,
          status: payment.payment_status,
          created_at: payment.created_at,
          cardholder_name: payment.cardholder_name,
          card_last4: payment.card_last_four,
          card_brand: payment.card_brand,
          notes: payment.notes
        }))
      } else {
        showErrorToast('Failed to fetch payments')
      }
    } catch (error) {
      console.error('Error fetching payments:', error)
    } finally {
      isLoading.value = false
    }
  }

  onMounted(async () => {
    await getPayments()
  })
</script>

<template>
  <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-9xl mx-auto">
    <!-- Header -->
    <div class="sm:flex sm:justify-between sm:items-center mb-8">
      <!-- Left: Title -->
      <div class="mb-4 sm:mb-0">
        <h1 class="text-2xl md:text-3xl font-extrabold text-slate-800">Payments</h1>
        <p class="text-slate-500 mt-1">View and track all payment transactions</p>
      </div>

      <!-- Right: Actions -->
      <div class="grid grid-flow-col sm:auto-cols-max justify-start sm:justify-end gap-2">
        <!-- Search -->
        <div class="relative">
          <input v-model="searchQuery" type="search" placeholder="Search transactions..."
                 class="form-input pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:border-primary focus:ring-primary" />
          <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
            <svg class="w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
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

        <!-- Export button -->
        <button class="btn bg-white border-slate-200 hover:border-slate-300 text-slate-600 rounded-lg px-4 py-2 inline-flex items-center space-x-2 shadow-sm transition duration-150 ease-in-out">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"></path>
          </svg>
          <span>Export</span>
        </button>
      </div>
    </div>

    <!-- Loading indicator -->
    <div v-if="isLoading" class="flex justify-center py-12">
      <svg class="animate-spin h-8 w-8 text-primary" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
      </svg>
    </div>

    <!-- Mobile Payments Cards -->
    <div v-if="!isLoading" class="grid grid-cols-1 gap-4 md:hidden">
      <div v-for="payment in filteredPayments" :key="payment.id"
           class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <div class="p-5">
          <div class="flex justify-between items-start mb-2">
            <div>
              <p class="text-sm text-slate-500">Transaction</p>
              <p class="font-medium text-slate-900">{{ payment.transaction_id || '-' }}</p>
            </div>
            <span :class="[
              'px-2 py-1 rounded-full text-xs font-medium',
              getStatusClass(payment.status)
            ]">
              {{ payment.status }}
            </span>
          </div>

          <div class="space-y-3 py-3 border-t border-b border-slate-100 my-3">
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Order ID:</span>
              <span class="font-medium text-slate-700">#{{ payment.order_id }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Amount:</span>
              <span class="font-medium text-slate-900">{{ formatCurrency(payment.amount) }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Date:</span>
              <span class="font-medium text-slate-700">{{ formatDate(payment.created_at) }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Method:</span>
              <span class="font-medium text-slate-700">{{ payment.payment_method }}</span>
            </div>
          </div>

          <button @click="viewPaymentDetails(payment)"
                  class="w-full mt-2 px-3 py-2 bg-primary/10 text-primary hover:bg-primary/20 rounded-lg transition-colors text-sm font-medium">
            View Details
          </button>
        </div>
      </div>

      <!-- Empty state for cards -->
      <div v-if="filteredPayments.length === 0" class="py-12 text-center">
        <div class="flex flex-col items-center">
          <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
          </svg>
          <p class="text-slate-500 font-medium">No payments found</p>
          <p class="text-slate-400 text-sm mt-1">Try adjusting your search or filters</p>
        </div>
      </div>
    </div>

    <!-- Payments Table (desktop view) -->
    <div v-if="!isLoading" class="hidden md:block">
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-slate-200">
            <thead class="bg-slate-50">
              <tr>
                <th @click="setSort('transaction_id')" class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider cursor-pointer">
                  Transaction ID <span v-if="sortColumn==='transaction_id'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
                </th>
                <th @click="setSort('order_id')" class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider cursor-pointer">
                  Order <span v-if="sortColumn==='order_id'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
                </th>
                <th @click="setSort('amount')" class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider cursor-pointer">
                  Amount <span v-if="sortColumn==='amount'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
                </th>
                <th @click="setSort('payment_method')" class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider cursor-pointer">
                  Payment Method <span v-if="sortColumn==='payment_method'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
                </th>
                <th @click="setSort('created_at')" class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider cursor-pointer">
                  Date <span v-if="sortColumn==='created_at'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
                </th>
                <th @click="setSort('status')" class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider cursor-pointer">
                  Status <span v-if="sortColumn==='status'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
                </th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-slate-200">
              <tr v-for="payment in sortedPayments" :key="payment.id" class="hover:bg-slate-50 transition-colors">
                <td class="px-6 py-4 whitespace-nowrap font-medium text-slate-800">{{ payment.transaction_id || '-' }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <a href="#" class="text-primary hover:underline">#{{ payment.order_id }}</a>
                </td>
                <td class="px-6 py-4 whitespace-nowrap font-medium">{{ formatCurrency(payment.amount) }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div>{{ payment.payment_method }}</div>
                  <div v-if="payment.card_last4" class="text-xs text-slate-500">
                    {{ payment.card_brand }} •••• {{ payment.card_last4 }}
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-slate-600">{{ formatDate(payment.created_at) }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="[
                    'px-2 py-1 rounded-full text-xs font-medium',
                    getStatusClass(payment.status)
                  ]">
                    {{ payment.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <button @click="viewPaymentDetails(payment)" class="text-primary hover:text-primary-dark transition-colors">
                    View Details
                  </button>
                </td>
              </tr>

              <!-- Empty state for table -->
              <tr v-if="filteredPayments.length === 0">
                <td colspan="7" class="px-6 py-12 text-center">
                  <div class="flex flex-col items-center">
                    <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                    <p class="text-slate-500 font-medium">No payments found</p>
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

  <!-- Payment Details Modal -->
  <div v-if="showViewModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
    <div class="bg-white rounded-xl shadow-xl max-w-2xl w-full p-6 max-h-[90vh] overflow-y-auto" @click.stop>
      <div class="flex justify-between items-start mb-6">
        <h3 class="text-lg font-bold text-slate-800">Payment Details</h3>
        <button @click="closeModal" class="text-slate-400 hover:text-slate-500">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>

      <div v-if="currentPayment">
        <!-- Transaction Info -->
        <div class="mb-6 p-4 bg-slate-50 rounded-lg">
          <div class="flex justify-between items-center mb-4">
            <div>
              <h4 class="font-medium text-slate-800">Transaction Information</h4>
              <p class="text-primary font-medium">{{ currentPayment.transaction_id }}</p>
            </div>
            <span :class="[
              'px-3 py-1 rounded-full text-sm font-medium',
              getStatusClass(currentPayment.status)
            ]">
              {{ currentPayment.status }}
            </span>
          </div>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="text-sm">
              <p class="text-slate-500">Date</p>
              <p class="font-medium">{{ formatDate(currentPayment.created_at) }}</p>
            </div>
            <div class="text-sm">
              <p class="text-slate-500">Amount</p>
              <p class="font-medium">{{ formatCurrency(currentPayment.amount) }}</p>
            </div>
            <div class="text-sm">
              <p class="text-slate-500">Payment Method</p>
              <p class="font-medium">{{ currentPayment.payment_method }}</p>
              <p v-if="currentPayment.card_brand" class="text-xs text-slate-500 mt-1">
                {{ currentPayment.card_brand }} ending in {{ currentPayment.card_last4 }}
              </p>
            </div>
            <div class="text-sm">
              <p class="text-slate-500">Order ID</p>
              <p class="font-medium">#{{ currentPayment.order_id }}</p>
            </div>
          </div>
        </div>

        <!-- Customer Info -->
        <!--<div class="mb-6 p-4 bg-slate-50 rounded-lg">
          <h4 class="font-medium text-slate-800 mb-4">Customer Information</h4>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="text-sm">
              <p class="text-slate-500">Name</p>
              <p class="font-medium">{{ currentPayment.customer_name }}</p>
            </div>
            <div class="text-sm">
              <p class="text-slate-500">Email</p>
              <p class="font-medium">{{ currentPayment.customer_email }}</p>
            </div>
          </div>
        </div>-->

        <!-- Status Timeline -->
        <div class="mb-6">
          <h4 class="font-medium text-slate-800 mb-4">Payment Timeline</h4>
          <div class="border-l-2 border-slate-200 pl-4 ml-2 space-y-4">
            <div class="relative">
              <div class="w-3 h-3 rounded-full bg-green-500 absolute -left-[22px] top-1"></div>
              <div class="text-sm">
                <p class="font-medium">Payment Initiated</p>
                <p class="text-slate-500">{{ formatDate(currentPayment.created_at) }}</p>
              </div>
            </div>

            <div v-if="currentPayment.status === 'succeeded'" class="relative">
              <div class="w-3 h-3 rounded-full bg-green-500 absolute -left-[22px] top-1"></div>
              <div class="text-sm">
                <p class="font-medium">Payment Successful</p>
                <p class="text-slate-500">{{ formatDate(currentPayment.created_at) }}</p>
              </div>
            </div>

            <div v-if="currentPayment.status === 'failed'" class="relative">
              <div class="w-3 h-3 rounded-full bg-red-500 absolute -left-[22px] top-1"></div>
              <div class="text-sm">
                <p class="font-medium">Payment Failed</p>
                <p class="text-slate-500">{{ formatDate(currentPayment.created_at) }}</p>
                <p class="text-red-500 text-xs mt-1">Transaction declined by payment processor</p>
              </div>
            </div>

            <div v-if="currentPayment.status === 'pending'" class="relative">
              <div class="w-3 h-3 rounded-full bg-yellow-500 absolute -left-[22px] top-1"></div>
              <div class="text-sm">
                <p class="font-medium">Payment Pending</p>
                <p class="text-slate-500">{{ formatDate(currentPayment.created_at) }}</p>
                <p class="text-yellow-500 text-xs mt-1">Awaiting confirmation from payment processor</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Action buttons -->
        <div class="flex justify-end">
          <button @click="closeModal"
                  class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
            Close
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
