<template>
  <div class="py-16 px-4 flex justify-center">
    <div class="bg-white p-8 rounded-2xl shadow-lg w-full max-w-md border border-slate-200">
      <h2 class="text-2xl font-bold mb-6 text-center text-slate-800">Forgot Password</h2>

      <form v-if="step === 1" @submit.prevent="requestResetCode" class="space-y-5">
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Email</label>
          <input v-model="email" type="email" required placeholder="you@example.com"
            class="w-full px-3 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
        </div>
        <button type="submit" :disabled="loading"
          class="w-full bg-primary text-white py-3 rounded-xl hover:bg-primary-dark transition flex items-center justify-center shadow-md font-medium">
          <span v-if="!loading">Send Reset Code</span>
          <svg v-else class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none"
            viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"></path>
          </svg>
        </button>
      </form>

      <form v-else @submit.prevent="resetPassword" class="space-y-5">
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Verification Code</label>
          <input v-model="code" type="text" maxlength="6" minlength="6" required placeholder="6-digit code"
            class="w-full px-3 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">New Password</label>
          <input v-model="newPassword" type="password" required minlength="6" placeholder="New password"
            class="w-full px-3 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
        </div>
        <button type="submit" :disabled="loading"
          class="w-full bg-primary text-white py-3 rounded-xl hover:bg-primary-dark transition flex items-center justify-center shadow-md font-medium">
          <span v-if="!loading">Reset Password</span>
          <svg v-else class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none"
            viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"></path>
          </svg>
        </button>
      </form>

      <p class="text-center text-sm text-slate-500 mt-6">
        <RouterLink to="/login" class="text-primary hover:text-primary-dark font-medium">Back to Login</RouterLink>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../utils/Api.ts'
import { showErrorToast, showSuccessToast } from '../utils/helpers.ts'

const email = ref('')
const code = ref('')
const newPassword = ref('')
const loading = ref(false)
const step = ref(1)
const router = useRouter()

async function requestResetCode() {
  loading.value = true
  try {
    await api.post('/api/user/forgot-password', { email: email.value })
    showSuccessToast('If the email exists, a reset code has been sent.')
    step.value = 2
  } catch (e: any) {
    showErrorToast(e.response?.data?.message || 'Failed to send reset code.')
  } finally {
    loading.value = false
  }
}

async function resetPassword() {
  loading.value = true
  try {
    await api.post('/api/user/reset-password', {
      email: email.value,
      verification_code: code.value,
      new_password: newPassword.value
    })
    showSuccessToast('Password reset successful! Redirecting to login...')
    setTimeout(() => router.push('/login'), 2000)
  } catch (e: any) {
    showErrorToast(e.response?.data?.message || 'Failed to reset password.')
  } finally {
    loading.value = false
  }
}
</script>