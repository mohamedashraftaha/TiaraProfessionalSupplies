<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import api from '../utils/Api.ts'
import ComingSoon from '../components/ComingSoon.vue'

interface EventDTO {
  id: number
  title: string
  description: string
  speakers?: string
  date: string
  location?: string
  image_url?: string
  capacity?: number
  createdAt?: string
  updatedAt?: string
}

const events = ref<EventDTO[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

const fetchEvents = async () => {
  loading.value = true
  error.value = null
  try {
    debugger;
    const res = await api.get('/api/events')
    events.value = res.data
  } catch (e) {
    error.value = 'Failed to load events.'
  } finally {
    loading.value = false
  }
}

onMounted(fetchEvents)

const categories = computed(() => {
  const cats = new Set<string>()
  events.value.forEach(e => {
    if (e.speakers) cats.add(e.speakers)
  })
  return ['All', ...Array.from(cats)]
})
const selectedCategory = ref('All')

const filteredEvents = computed(() => {
  if (selectedCategory.value === 'All') return events.value
  return events.value.filter(event => event.speakers === selectedCategory.value)
})
</script>

<template>
  <ComingSoon />
  <!-- <div class="min-h-screen bg-gradient-to-br from-blue-900 to-indigo-800 text-white py-20">
    <!-- Hero Section -->
  <!-- <section class="text-center mb-20"> -->
  <!-- <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-Events-logo-light.svg" alt="Tiara Events" class="h-32 mx-auto mb-8" />
      <h1 class="text-5xl md:text-6xl font-bold">Dental Education Events</h1>
      <p class="text-xl md:text-2xl text-gray-300 mt-4">
        Join our professional workshops and conferences
      </p>
    </section> -->

  <!-- Category Filter -->
  <!-- <div class="flex justify-center flex-wrap gap-4 mb-12">
      <button v-for="category in categories"
              :key="category"
              @click="selectedCategory = category"
              :class="[
          'px-5 py-2 rounded-full font-medium transition',
          selectedCategory === category
            ? 'bg-cyan-500 text-white'
            : 'bg-white/20 text-white hover:bg-white/30'
        ]">
        {{ category }}
      </button>
    </div>

    <div v-if="loading" class="flex justify-center items-center py-16">
      <svg class="animate-spin h-12 w-12 text-cyan-400" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
        <path class="opacity-75" fill="currentColor"
              d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z" />
      </svg>
    </div>
    <div v-else-if="error" class="text-center text-red-400 py-10">{{ error }}</div>
    <div v-else-if="filteredEvents.length === 0" class="text-center text-gray-200 py-10">No events available.</div> -->

  <!-- Events Grid -->
  <!-- <div v-else class="grid md:grid-cols-2 lg:grid-cols-3 gap-8 px-4">
      <div v-for="event in filteredEvents"
           :key="event.id"
           class="bg-white text-gray-800 rounded-2xl overflow-hidden shadow-lg hover:-translate-y-1 hover:shadow-2xl transition">
        <img :src="event.image_url || 'https://placehold.co/600x400?text=Event'" :alt="event.title" class="w-full h-48 object-cover" />
        <div class="p-6">
          <div class="flex justify-between items-center mb-4">
            <h3 class="text-xl font-bold text-gray-900">{{ event.title }}</h3>
            <span v-if="event.speakers" class="px-3 py-1 bg-cyan-100 text-cyan-700 rounded-full text-sm">
              {{ event.speakers }}
            </span>
          </div>
          <p class="text-gray-600 mb-4 text-sm">{{ event.description }}</p>

          <div class="space-y-2 text-sm mb-6 text-gray-700">
            <div class="flex items-center">
              <svg class="h-5 w-5 mr-2 text-cyan-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
              {{ new Date(event.date).toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric', timeZone: 'Africa/Cairo' }) }}
            <span v-if="event.date">
              &nbsp;at&nbsp;
              {{ new Date(event.date).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', timeZone: 'UTC' }) }}
            </span>
            </div>

            <div class="flex items-center">
              <svg class="h-5 w-5 mr-2 text-cyan-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              {{ event.location }}
            </div>
            <div class="flex items-center">
              <svg class="h-5 w-5 mr-2 text-cyan-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
              <span v-if="event.capacity">{{ event.capacity }} spots available</span>
              <span v-else>Unlimited spots</span>
            </div>
          </div>

          <div class="flex justify-between items-center">
            <span class="text-xl font-bold text-cyan-600">&nbsp;</span>
            <button class="px-5 py-2 bg-cyan-500 text-white rounded-lg hover:bg-cyan-600 transition">
              Register Now
            </button>
          </div>
        </div>
      </div>
    </div>
  </div> -->

</template>
