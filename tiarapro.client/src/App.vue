<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'
import { computed, onMounted, onUnmounted } from 'vue'
import Navbar from './components/Navbar.vue'
import Footer from './components/Footer.vue'
import { showErrorToast } from './utils/helpers'
import api from './utils/Api'

const route = useRoute()
const router = useRouter()

const showPublicLayout = computed(() => !route.path.startsWith('/admin'))

let intervalId: number

const validateToken = async () => {

  console.log("VALIDATING SESSION")
  debugger;
  const token = localStorage.getItem('token')
  if (!token || token == 'null') return

  try {
    await api.post('/api/user/ValidateTokenExpiry', token)
  } catch (error) {
    console.warn('Token is invalid or expired. Logging out...')
    localStorage.clear()
    localStorage.setItem('token', 'null');
    localStorage.setItem('userId', '0');
    showErrorToast('Session expired. Please log in again.')
    setTimeout(() => {
      router.push({ path: '/login', query: { expired: 'true' } })
    }, 1000)
  }
}

onMounted(() => {
  validateToken(); // Run once immediately
  intervalId = setInterval(validateToken, 15 * 60 * 1000) // every 15 minutes
})

onUnmounted(() => {
  clearInterval(intervalId)
})
</script>

<template>
  <div class="min-h-screen flex flex-col">
    <Navbar v-if="showPublicLayout" />

    <main class="flex-grow">
      <router-view />
    </main>

    <Footer v-if="showPublicLayout" />
  </div>
</template>
