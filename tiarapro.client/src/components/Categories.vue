<script setup lang="ts">

  import { categoriesConst }  from '../utils/helpers.ts'
  const categories = categoriesConst
</script>
<template>
  <section class="py-16 bg-gray-50">
    <div class="container mx-auto px-4">
      <h2 class="text-3xl font-bold text-center mb-12">Browse Categories</h2>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        <router-link :to="`/products?category=${category.id}`"
                     v-for="category in categories"
                     :key="category.name"
                     class="bg-white p-6 rounded-lg shadow-sm hover:shadow-md transition-shadow text-center cursor-pointer flex flex-col justify-between h-64">
          <div class="flex-grow flex flex-col items-center">
            <div class="w-full h-full rounded-full flex items-center justify-center mx-auto mb-4">
              <img :src="category.icon" alt="icon" class="w-25 h-20 object-contain" />
            </div>
          </div>
          <div>
            <h3 class="font-semibold text-lg mb-1">{{ category.name }}</h3>
            <!--<p class="text-gray-600 text-sm">{{ category.count }}</p>-->
          </div>
        </router-link>

      </div>
    </div>
  </section>
</template>
