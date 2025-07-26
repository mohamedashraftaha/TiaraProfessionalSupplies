<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../../utils/Api.ts'
import { showErrorToast, showSuccessToast } from '../../utils/helpers.ts'

const router = useRouter()
const email = ref('')
const password = ref('')
const error = ref('')
const isSubmitting = ref(false)

async function handleLogin() {
  error.value = ''
  isSubmitting.value = true

  try {
    const userPayload = {
      email: email.value,
      password_hash: password.value,
    }

    const response = await api.post('/api/user/signin-admin', userPayload)

    debugger;
    if (response.status === 200) {
      const data = response.data
      if (data.role?.toLowerCase() === 'admin') {
        localStorage.setItem('userRole', data.role)
        localStorage.setItem('token', data.token)
        localStorage.setItem('email', data.email)
        localStorage.setItem('userId', data.user_id)
        localStorage.setItem('isAuthenticated', 'true')
        localStorage.setItem('firstName', data.first_name)
        localStorage.setItem('lastName', data.last_name)

        showSuccessToast('Login successful! Redirecting...')
        setTimeout(() => {
          router.push('/admin/dashboard')
        }, 1500)
      } else {
        error.value = 'You are not authorized to access the admin panel.'
        showErrorToast(error.value)
      }
    }
  } catch (e: any) {
    error.value = e.response?.data?.message || 'Login failed. Please try again.'
    showErrorToast(error.value)
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-100 to-purple-100 px-4">
    <div class="w-full max-w-md bg-white rounded-xl shadow-xl p-8">
      <h2 class="text-center text-3xl font-bold text-gray-800 mb-1">Tiara Pro Admin</h2>
      <p class="text-center text-sm text-gray-500 mb-6">Sign in to manage your store</p>

      <form @submit.prevent="handleLogin" class="space-y-5">
        <div>
          <label for="email" class="block text-sm font-medium text-gray-700">Email address</label>
          <input id="email" v-model="email" type="email" required
            class="w-full mt-1 px-4 py-2 border rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="admin@example.com" />
        </div>

        <div>
          <label for="password" class="block text-sm font-medium text-gray-700">Password</label>
          <input id="password" v-model="password" type="password" required
            class="w-full mt-1 px-4 py-2 border rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="••••••••" />
        </div>

        <div v-if="error" class="text-center text-sm text-red-600">
          {{ error }}
        </div>

        <button type="submit"
          class="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition disabled:opacity-50"
          :disabled="isSubmitting">
          {{ isSubmitting ? 'Signing in...' : 'Sign In' }}
        </button>
      </form>
    </div>
  </div>
</template>

<style scoped>
/* Optional: Add custom style improvements here */
</style>
