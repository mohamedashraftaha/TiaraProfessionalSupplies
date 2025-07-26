<script setup lang="ts">
  import {useRouter } from 'vue-router'

  const router = useRouter()
</script>

<template>
  <section class="relative bg-gradient-to-r from-blue-900 to-blue-700 text-white py-20">
    <div class="absolute inset-0 z-0">
      <img src="https://images.pexels.com/photos/3845810/pexels-photo-3845810.jpeg"
           alt="Dental Practice"
           class="w-full h-full object-cover opacity-20" />
    </div>
    <div class="container mx-auto px-4 relative z-10">
      <div class="max-w-2xl">
        <h1 class="text-5xl font-bold mb-4">Professional Dental Supplies</h1>
        <p class="text-xl mb-8">Quality equipment and supplies for dental professionals. Trusted by clinics nationwide.</p>
        <div class="space-x-4">
          <router-link
                       to="/products"
                       class="bg-white text-blue-900 px-6 py-3 rounded-lg font-semibold hover:bg-gray-100"
                       >
            Shop Products

          </router-link>
          <!--<a href="#" class="border border-white px-6 py-3 rounded-lg font-semibold hover:bg-white hover:text-blue-900">
            View Specials
          </a>-->
        </div>
      </div>
    </div>
  </section>
</template>
