<script setup lang="ts">
import { ref } from 'vue';

const partners = ref([
  {
    id: 1,
    name: 'Dental Mesh',
    logo: 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/logo_256.png',
    description: 'Advanced AI technologies powering Tiara AI for dental diagnostics and treatment planning.'
  }
  // You can add more partners here as needed
]);
</script>

<template>
  <section class="py-16 bg-white">
    <div class="container mx-auto px-4">
      <h2 class="text-3xl font-bold text-center mb-6">Partners & Technology</h2>
      <p class="text-gray-600 text-center max-w-3xl mx-auto mb-12">
        We partner with industry-leading technology providers to bring you the most advanced dental solutions.
      </p>

      <div class="grid grid-cols-1 md:grid-cols-1 lg:grid-cols-1 gap-8">
        <div v-for="partner in partners" :key="partner.id"
          class="bg-white rounded-lg shadow-sm p-8 flex flex-col items-center text-center hover:shadow-md transition-shadow">
          <img :src="partner.logo" :alt="partner.name" class="h-16 mb-6">
          <h3 class="text-xl font-semibold mb-3">{{ partner.name }}</h3>
          <p class="text-gray-600">{{ partner.description }}</p>
        </div>
      </div>
    </div>
  </section>
</template>
