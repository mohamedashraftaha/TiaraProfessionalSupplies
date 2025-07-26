<template>
  <div class="py-16 px-4 flex justify-center">
    <div class="bg-white p-8 rounded-2xl shadow-lg w-full max-w-md border border-slate-200">
      <h2 class="text-2xl font-bold mb-6 text-center text-slate-800">Login to Your Account</h2>

      <form @submit.prevent="handleLogin" class="space-y-5">
        <!-- Email -->
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
            <input v-model="email" type="email" required placeholder="you@example.com"
              class="w-full pl-10 pr-3 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
          </div>
        </div>

        <!-- Password -->
        <div>
          <div class="flex justify-between items-center mb-1">
            <label class="text-sm font-medium text-slate-700">Password</label>
            <RouterLink to="/forgot-password" class="text-xs text-primary hover:text-primary-dark">Forgot password?
            </RouterLink>
          </div>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg class="h-5 w-5 text-slate-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                fill="currentColor">
                <path fill-rule="evenodd"
                  d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z"
                  clip-rule="evenodd" />
              </svg>
            </div>
            <input v-model="password" :type="showPassword ? 'text' : 'password'" required placeholder="••••••••"
              class="w-full pl-10 pr-10 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
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
                <svg v-else class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                  <path fill-rule="evenodd"
                    d="M3.707 2.293a1 1 0 00-1.414 1.414l14 14a1 1 0 001.414-1.414l-1.473-1.473A10.014 10.014 0 0019.542 10C18.268 5.943 14.478 3 10 3a9.958 9.958 0 00-4.512 1.074l-1.78-1.781zm4.261 4.26l1.514 1.515a2.003 2.003 0 012.45 2.45l1.514 1.514a4 4 0 00-5.478-5.478z"
                    clip-rule="evenodd" />
                  <path
                    d="M12.454 16.697L9.75 13.992a4 4 0 01-3.742-3.741L2.335 6.578A9.98 9.98 0 00.458 10c1.274 4.057 5.065 7 9.542 7 .847 0 1.669-.105 2.454-.303z" />
                </svg>
              </button>
            </div>
          </div>
        </div>

        <!-- Remember me -->
        <div class="flex items-center">
          <input id="remember-me" type="checkbox"
            class="h-4 w-4 text-primary focus:ring-primary border-slate-300 rounded">
          <label for="remember-me" class="ml-2 block text-sm text-slate-700">Remember me</label>
        </div>

        <!-- Login Button -->
        <button type="submit" :disabled="loading"
          class="w-full bg-primary text-white py-3 rounded-xl hover:bg-primary-dark transition flex items-center justify-center shadow-md font-medium">
          <span v-if="!loading">Login</span>
          <svg v-else class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none"
            viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"></path>
          </svg>
        </button>
      </form>

      <!-- Sign-up Link -->
      <p class="text-center text-sm text-slate-500 mt-6">
        Don't have an account?
        <RouterLink to="/signup" class="text-primary hover:text-primary-dark font-medium">Sign up</RouterLink>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../utils/Api.ts'
import { showErrorToast, showSuccessToast } from '../utils/helpers.ts'

const email = ref('')
const password = ref('')
const showPassword = ref(false)
const loading = ref(false)
const router = useRouter()

async function handleLogin() {
  loading.value = true
  const userPayload = {
    email: email.value,
    password_hash: password.value,
  }
  try {
    const response = await api.post('/api/user/signin', userPayload)
    if (response.status === 200) {
      const data = response.data
      if (data.role == 'User' || data.role == 'user') {
        debugger;
        localStorage.setItem('userRole', data.role)
        localStorage.setItem('token', data.token)
        localStorage.setItem('email', data.email)
        localStorage.setItem('userId', data.user_id)
        localStorage.setItem('isAuthenticated', 'true')
        localStorage.setItem('firstName', data.first_name)
        localStorage.setItem('middleName', data.middle_name)
        localStorage.setItem('lastName', data.last_name)
        localStorage.setItem('City', data.city)
        localStorage.setItem('Country', data.country)
        localStorage.setItem('Address', data.address)
        localStorage.setItem('PostalCode', data.postal_code)
        localStorage.setItem('State', data.state)
        localStorage.setItem('PhoneNumber', data.phone)

        // Format the date before storing
        const memberSince = new Date(data.created_at).toISOString()
        localStorage.setItem('memberSince', memberSince)
      }
      showSuccessToast("Login successful! Redirecting...")
      setTimeout(() => {
        router.push('/')
      }, 2000)
    } else {
      showErrorToast('Login failed. Please try again.')
    }
  } catch (error) {
    if (error.response?.status === 401) {
      showErrorToast('Invalid email or password.')
    } else {
      showErrorToast('An error occurred. Please try again later.')
    }
    console.error('Login error:', error)
  } finally {
    loading.value = false
  }
}
</script>
