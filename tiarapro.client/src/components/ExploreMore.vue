<script setup lang="ts">
const services = [
  {
    title: 'Tiara Events',
    logo: 'Events',
    description: 'World-class dental conferences, community networking, and exclusive experiences.',
    color: 'blue',
    route: '/events'  // Added route path
  },
  {
    title: 'Tiara AI',
    logo: 'AI',
    description: 'Instantly analyze dental scans using AI. Quick. Accurate. Revolutionary.',
    color: 'green',
    route: '/ai'  // Added route path
  },
  {
    title: 'Dental Training',
    logo: 'DT',
    description: 'Hands-on dental education led by experts. Learn. Practice. Excel.',
    color: 'red',
    route: '/training'  // Added route path
  }
];
</script>

<template>
  <section class="py-16 bg-white">
    <div class="container mx-auto px-4">
      <h2 class="text-4xl font-bold text-center mb-12">
        Explore More With <span class="text-pink-500">Tiara</span>
      </h2>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div v-for="service in services" :key="service.title"
          class="bg-white rounded-lg p-8 text-center shadow-lg hover:shadow-xl transition-shadow">
          <div class="mb-6 h-24 flex items-center justify-center">
            <img
              :src="`https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-${service.logo}-logo-original.svg`"
              :alt="service.title" class="max-h-20 max-w-[160px] object-contain" />
          </div>
          <p class="text-gray-600">{{ service.description }}</p>

          <!-- Using router-link for navigation -->
          <router-link :to="service.route">
            <button class="mt-6 px-6 py-2 bg-blue-600 text-white rounded-full hover:bg-blue-700 transition-colors">
              Learn More
            </button>
          </router-link>
        </div>
      </div>
    </div>
  </section>
</template>
