<script setup lang="ts">
import { Line } from 'vue-chartjs'
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend } from 'chart.js'
import { ref, onMounted } from 'vue'
import { showErrorToast } from '../../utils/helpers.ts'
import api from '../../utils/Api.ts'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend)

// Dashboard stats
const stats = ref({
  orders: 0,
  products: 0,
  payments: 0,
  users: 0,
  revenue: 0,
  orderGrowth: '',
  productGrowth: '',
  userGrowth: '',
  revenueGrowth: ''
})

// Chart data
const chartData = ref({
  labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
  datasets: [
    {
      label: 'Revenue',
      data: [0, 0, 0, 0, 0, 0],
      borderColor: '#2c479e',
      backgroundColor: 'rgba(44, 71, 158, 0.1)',
      tension: 0.4,
      fill: true
    }
  ]
})

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false
    },
    tooltip: {
      callbacks: {
        label: function (context: any) {
          return 'EGP ' + context.raw.toLocaleString()
        }
      }
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      grid: {
        color: 'rgba(0, 0, 0, 0.05)'
      },
      ticks: {
        callback: function (value: any) {
          return 'EGP ' + value.toLocaleString()
        }
      }
    },
    x: {
      grid: {
        display: false
      }
    }
  }
}

const recentOrders = ref([])
const recentPayments = ref([])
const isLoading = ref(true)
const lastUpdated = ref(new Date())

// Format currency
const formatCurrency = (amount: any) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'EGP',
    minimumFractionDigits: 2
  }).format(amount)
}

// Format date
const formatDate = (dateString: any) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return new Intl.DateTimeFormat('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  }).format(date)
}

// Get time since
const getTimeSince = (dateString: any) => {
  if (!dateString) return ''

  const now = new Date().getTime()
  const pastDate = new Date(dateString).getTime()
  const diffMs = now - pastDate
  const diffMins = Math.round(diffMs / 60000)

  if (diffMins < 60) return `${diffMins} minutes ago`

  const diffHours = Math.round(diffMins / 60)
  if (diffHours < 24) return `${diffHours} hours ago`

  const diffDays = Math.round(diffHours / 24)
  if (diffDays === 1) return 'Yesterday'
  if (diffDays < 30) return `${diffDays} days ago`

  return formatDate(dateString)
}

// Get CSS class for status badge
const getStatusClass = (status) => {
  const normalizedStatus = status
  switch (normalizedStatus) {
    case 'delivered':
    case 'completed':
    case 'succeeded':
      return 'bg-green-100 text-green-800'
    case 'shipped':
    case 'processing':
      return 'bg-blue-100 text-blue-800'
    case 'pending':
      return 'bg-yellow-100 text-yellow-800'
    case 'cancelled':
    case 'failed':
      return 'bg-red-100 text-red-800'
    default:
      return 'bg-slate-100 text-slate-800'
  }
}

// Helper to get monthly counts
const getMonthlyCounts = (items, dateField) => {
  const now = new Date();
  const thisMonth = now.getMonth();
  const lastMonth = (thisMonth + 11) % 12;
  const thisYear = now.getFullYear();
  const lastMonthYear = thisMonth === 0 ? thisYear - 1 : thisYear;

  let thisMonthCount = 0;
  let lastMonthCount = 0;

  items.forEach(item => {
    const d = new Date(item[dateField]);
    if (d.getFullYear() === thisYear && d.getMonth() === thisMonth) thisMonthCount++;
    if (d.getFullYear() === lastMonthYear && d.getMonth() === lastMonth) lastMonthCount++;
  });

  return { thisMonthCount, lastMonthCount };
};

// Helper to get monthly revenue
const getMonthlyRevenue = (payments, month, year) =>
  payments
    .filter(p => p.payment_status === 'Success')
    .filter(p => {
      const d = new Date(p.created_at);
      return d.getFullYear() === year && d.getMonth() === month;
    })
    .reduce((sum, p) => sum + Number(p.amount), 0);

// Helper to format growth for display
const formatGrowth = (thisMonth, lastMonth, growth) => {
  if (lastMonth === 0 && thisMonth > 0) return 'New';
  if (growth === 0) return '0%';
  return (growth > 0 ? '+' : '') + growth.toFixed(1) + '%';
};

// Get dashboard data
const getDashboardData = async () => {
  isLoading.value = true
  try {
    // Get stats data using Promise.allSettled to handle 404s gracefully
    const endpoints = [
      '/api/order',
      '/api/product',
      '/api/payment',
      '/api/user'
    ];
    const results = await Promise.allSettled(endpoints.map(endpoint => api.get(endpoint)));

    function extractData(res) {
      if (res.status === 'fulfilled' && res.value.status === 200 && Array.isArray(res.value.data)) {
        return res.value.data;
      }
      // If 404, treat as empty array
      if (res.status === 'rejected' && res.reason?.response?.status === 404) {
        return [];
      }
      // Any other error, also treat as empty array
      return [];
    }

    const [orders, products, payments, users] = results.map(extractData);

    // Calculate stats
    const now = new Date();
    const thisMonth = now.getMonth();
    const lastMonth = (thisMonth + 11) % 12;
    const thisYear = now.getFullYear();
    const lastMonthYear = thisMonth === 0 ? thisYear - 1 : thisYear;

    // Orders growth
    const { thisMonthCount: thisMonthOrders, lastMonthCount: lastMonthOrders } = getMonthlyCounts(orders, 'created_at');
    const orderGrowthRaw = lastMonthOrders === 0
      ? (thisMonthOrders > 0 ? 100 : 0)
      : parseFloat((((thisMonthOrders - lastMonthOrders) / lastMonthOrders) * 100).toFixed(1)) || 0;
    const orderGrowthDisplay = formatGrowth(thisMonthOrders, lastMonthOrders, orderGrowthRaw);

    // Products growth
    const { thisMonthCount: thisMonthProducts, lastMonthCount: lastMonthProducts } = getMonthlyCounts(products, 'created_at');
    const productGrowthRaw = lastMonthProducts === 0
      ? (thisMonthProducts > 0 ? 100 : 0)
      : parseFloat((((thisMonthProducts - lastMonthProducts) / lastMonthProducts) * 100).toFixed(1)) || 0;
    const productGrowthDisplay = formatGrowth(thisMonthProducts, lastMonthProducts, productGrowthRaw);

    // Users growth
    const { thisMonthCount: thisMonthUsers, lastMonthCount: lastMonthUsers } = getMonthlyCounts(users, 'created_at');
    const userGrowthRaw = lastMonthUsers === 0
      ? (thisMonthUsers > 0 ? 100 : 0)
      : parseFloat((((thisMonthUsers - lastMonthUsers) / lastMonthUsers) * 100).toFixed(1)) || 0;
    const userGrowthDisplay = formatGrowth(thisMonthUsers, lastMonthUsers, userGrowthRaw);

    // Revenue growth
    const thisMonthRevenue = getMonthlyRevenue(payments, thisMonth, thisYear);
    const lastMonthRevenue = getMonthlyRevenue(payments, lastMonth, lastMonthYear);
    const revenueGrowthRaw = lastMonthRevenue === 0
      ? (thisMonthRevenue > 0 ? 100 : 0)
      : parseFloat((((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100).toFixed(1)) || 0;
    const revenueGrowthDisplay = formatGrowth(thisMonthRevenue, lastMonthRevenue, revenueGrowthRaw);

    // Total revenue
    const totalRevenue = payments.reduce((sum, payment) => {
      if (payment.payment_status === 'Success') {
        return sum + Number(payment.amount)
      }
      return sum
    }, 0)

    // Update stats
    stats.value = {
      orders: orders.length,
      products: products.length,
      payments: payments.filter(p => p.payment_status === 'Success').length,
      users: users.length,
      revenue: totalRevenue,
      orderGrowth: orderGrowthDisplay,
      productGrowth: productGrowthDisplay,
      userGrowth: userGrowthDisplay,
      revenueGrowth: revenueGrowthDisplay
    }
    console.log(orders)

    // Get recent orders (last 5)
    recentOrders.value = orders
      .sort((a, b) => new Date(b.created_at).getTime() - new Date(a.created_at).getTime())
      .slice(0, 5)
      .map(order => ({
        id: order.id,
        customer: `${order.shipping_first_name || ''} ${order.shipping_last_name || ''}`.trim() || 'Customer',
        amount: order.total_amount,
        status: order.status,
        date: order.created_at
      }))

    // Get recent payments (last 5)
    recentPayments.value = payments
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 5)
      .map(payment => ({
        id: payment.id,
        order_id: payment.order_id,
        amount: payment.amount,
        status: payment.payment_status,
        method: payment.payment_method,
        date: payment.created_at
      }))

    // Generate revenue data for chart (monthly)
    const monthlyRevenue = [0, 0, 0, 0, 0, 0]
    const currentMonth = new Date().getMonth()

    payments.forEach(payment => {
      if (payment.payment_status === 'Success') {
        const paymentDate = new Date(payment.created_at)
        const monthDiff = currentMonth - paymentDate.getMonth()

        if (monthDiff >= 0 && monthDiff < 6) {
          monthlyRevenue[5 - monthDiff] += Number(payment.amount)
        }
      }
    })

    // Update chart data
    const months = []
    for (let i = 5; i >= 0; i--) {
      const d = new Date()
      d.setMonth(d.getMonth() - i)
      months.push(d.toLocaleString('default', { month: 'short' }))
    }

    chartData.value = {
      labels: months,
      datasets: [
        {
          label: 'Revenue',
          data: monthlyRevenue,
          borderColor: '#2c479e',
          backgroundColor: 'rgba(44, 71, 158, 0.1)',
          tension: 0.4,
          fill: true
        }
      ]
    }

    lastUpdated.value = new Date()

  } catch (error) {
    console.error('Error fetching dashboard data:', error)
    showErrorToast('Error loading dashboard data')
  } finally {
    isLoading.value = false
  }
}

onMounted(async () => {
  await getDashboardData()
})
</script>

<template>
  <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-9xl mx-auto">
    <!-- Header -->
    <div class="sm:flex sm:justify-between sm:items-center mb-8">
      <div>
        <h1 class="text-2xl md:text-3xl font-extrabold text-slate-800">Dashboard</h1>
        <p class="text-slate-500 mt-1">Overview of your business performance</p>
      </div>
      <div class="text-sm text-slate-500 flex items-center mt-2 sm:mt-0">
        <span>Last updated: {{ lastUpdated.toLocaleTimeString() }}</span>
        <button @click="getDashboardData" class="ml-2 p-1 rounded hover:bg-slate-100 transition-colors"
          title="Refresh data">
          <svg class="w-4 h-4 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"
            xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15">
            </path>
          </svg>
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

    <div v-else>
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <!-- Orders Card -->
        <div class="bg-white p-6 rounded-xl shadow-sm hover:shadow-md transition-shadow">
          <div class="flex items-center justify-between mb-4">
            <div class="text-slate-500">Total Orders</div>
            <div class="p-2 bg-blue-50 text-primary rounded-lg">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
              </svg>
            </div>
          </div>
          <div class="text-3xl font-bold">{{ stats.orders }}</div>
          <div class="text-green-500 text-sm mt-2">+{{ stats.orderGrowth }}</div>
        </div>

        <!-- Products Card -->
        <div class="bg-white p-6 rounded-xl shadow-sm hover:shadow-md transition-shadow">
          <div class="flex items-center justify-between mb-4">
            <div class="text-slate-500">Products</div>
            <div class="p-2 bg-blue-50 text-primary rounded-lg">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M5 8h14M5 8a2 2 0 110-4h14a2 2 0 110 4M5 8v10a2 2 0 002 2h10a2 2 0 002-2V8m-9 4h4" />
              </svg>
            </div>
          </div>
          <div class="text-3xl font-bold">{{ stats.products }}</div>
          <div class="text-green-500 text-sm mt-2">In stock</div>
        </div>

        <!-- Revenue Card -->
        <div class="bg-white p-6 rounded-xl shadow-sm hover:shadow-md transition-shadow">
          <div class="flex items-center justify-between mb-4">
            <div class="text-slate-500">Revenue</div>
            <div class="p-2 bg-blue-50 text-primary rounded-lg">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
          </div>
          <div class="text-3xl font-bold">{{ formatCurrency(stats.revenue) }}</div>
          <div class="text-green-500 text-sm mt-2">+{{ stats.revenueGrowth }}</div>
        </div>

        <!-- Users Card -->
        <div class="bg-white p-6 rounded-xl shadow-sm hover:shadow-md transition-shadow">
          <div class="flex items-center justify-between mb-4">
            <div class="text-slate-500">Users</div>
            <div class="p-2 bg-blue-50 text-primary rounded-lg">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
              </svg>
            </div>
          </div>
          <div class="text-3xl font-bold">{{ stats.users }}</div>
          <div class="text-green-500 text-sm mt-2">Active customers</div>
        </div>
      </div>

      <!-- Charts and Lists -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Revenue Chart -->
        <div class="lg:col-span-2 bg-white p-6 rounded-xl shadow-sm">
          <div class="flex justify-between items-center mb-4">
            <h2 class="text-lg font-semibold text-slate-800">Revenue Overview</h2>
            <div class="text-sm text-slate-500">Last 6 months</div>
          </div>
          <div class="h-[300px]">
            <Line :data="chartData" :options="chartOptions" />
          </div>
        </div>

        <!-- Recent Orders -->
        <div class="bg-white p-6 rounded-xl shadow-sm">
          <div class="flex justify-between items-center mb-4">
            <h2 class="text-lg font-semibold text-slate-800">Recent Orders</h2>
            <a href="/admin/orders" class="text-primary text-sm hover:underline">View all</a>
          </div>

          <div v-if="recentOrders.length === 0" class="flex flex-col items-center justify-center py-8 text-center">
            <svg class="w-12 h-12 text-slate-300 mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M5 8h14M5 8a2 2 0 110-4h14a2 2 0 110 4M5 8v10a2 2 0 002 2h10a2 2 0 002-2V8m-9 4h4" />
            </svg>
            <p class="text-slate-500">No recent orders</p>
          </div>

          <div v-else class="space-y-3">
            <div v-for="order in recentOrders" :key="order.id"
              class="flex items-center justify-between p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition">
              <div>
                <div class="font-medium text-slate-800">#{{ order.id }} - {{ order.customer }}</div>
                <div class="text-sm text-slate-500 flex items-center space-x-1">
                  <span>{{ formatCurrency(order.amount) }}</span>
                  <span class="text-slate-300">•</span>
                  <span>{{ getTimeSince(order.date) }}</span>
                </div>
              </div>
              <span :class="[
                'px-2 py-1 text-xs rounded-full',
                getStatusClass(order.status)
              ]">
                {{ order.status }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Activity -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-6">
        <!-- Recent Payments -->
        <div class="lg:col-span-2 bg-white p-6 rounded-xl shadow-sm">
          <div class="flex justify-between items-center mb-4">
            <h2 class="text-lg font-semibold text-slate-800">Recent Payments</h2>
            <a href="/admin/payments" class="text-primary text-sm hover:underline">View all</a>
          </div>

          <div v-if="recentPayments.length === 0" class="flex flex-col items-center justify-center py-8 text-center">
            <svg class="w-12 h-12 text-slate-300 mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <p class="text-slate-500">No recent payments</p>
          </div>

          <div v-else>
            <div class="overflow-x-auto">
              <table class="min-w-full">
                <thead>
                  <tr class="text-left text-xs font-medium text-slate-500 uppercase tracking-wider">
                    <th class="px-4 py-3">Order ID</th>
                    <th class="px-4 py-3">Amount</th>
                    <th class="px-4 py-3">Method</th>
                    <th class="px-4 py-3">Date</th>
                    <th class="px-4 py-3">Status</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-100">
                  <tr v-for="payment in recentPayments" :key="payment.id" class="hover:bg-slate-50 transition">
                    <td class="px-4 py-3 whitespace-nowrap">
                      <span class="text-primary font-medium">#{{ payment.order_id }}</span>
                    </td>
                    <td class="px-4 py-3 whitespace-nowrap font-medium">{{ formatCurrency(payment.amount) }}</td>
                    <td class="px-4 py-3 whitespace-nowrap">{{ payment.method }}</td>
                    <td class="px-4 py-3 whitespace-nowrap text-slate-500 text-sm">{{ getTimeSince(payment.date) }}</td>
                    <td class="px-4 py-3 whitespace-nowrap">
                      <span :class="[
                        'px-2 py-1 text-xs rounded-full',
                        getStatusClass(payment.status)
                      ]">
                        {{ payment.status }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div class="bg-white p-6 rounded-xl shadow-sm">
          <h2 class="text-lg font-semibold text-slate-800 mb-4">Quick Actions</h2>
          <div class="space-y-3">
            <a href="/admin/products"
              class="flex items-center p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition group">
              <div
                class="p-2 bg-primary/10 text-primary rounded mr-3 group-hover:bg-primary group-hover:text-white transition">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
              </div>
              <div>
                <div class="font-medium text-slate-800">Add New Product</div>
                <div class="text-xs text-slate-500">Create and publish a new product</div>
              </div>
            </a>

            <a href="/admin/users"
              class="flex items-center p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition group">
              <div
                class="p-2 bg-primary/10 text-primary rounded mr-3 group-hover:bg-primary group-hover:text-white transition">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
                </svg>
              </div>
              <div>
                <div class="font-medium text-slate-800">Add New User</div>
                <div class="text-xs text-slate-500">Create a new user account</div>
              </div>
            </a>

            <a href="/admin/orders"
              class="flex items-center p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition group">
              <div
                class="p-2 bg-primary/10 text-primary rounded mr-3 group-hover:bg-primary group-hover:text-white transition">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                </svg>
              </div>
              <div>
                <div class="font-medium text-slate-800">View Orders</div>
                <div class="text-xs text-slate-500">Manage and process orders</div>
              </div>
            </a>

            <a href="/admin/payments"
              class="flex items-center p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition group">
              <div
                class="p-2 bg-primary/10 text-primary rounded mr-3 group-hover:bg-primary group-hover:text-white transition">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
              </div>
              <div>
                <div class="font-medium text-slate-800">View Payments</div>
                <div class="text-xs text-slate-500">Review transaction history</div>
              </div>
            </a>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
