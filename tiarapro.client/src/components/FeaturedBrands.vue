<script setup lang="ts">
  import ProductCard from './ProductCard.vue'

  const bestSellers = [
    {
      id: 51,
      name: "NuSmile Bioflx - Primary 2nd Molar",
      price: 850,
      image: "https://d2j6dbq0eux0bg.cloudfront.net/images/15192231/3871876441.jpg",
      category: "NuSmile",
      quantity: 981,
      isVariant: true
    },
    {
      id: 51,
      name: "NuSmile Bioflx - Primary 1st Molar",
      price: 850,
      image: "https://d2j6dbq0eux0bg.cloudfront.net/images/15192231/3871924075.jpg",
      category: "NuSmile",
      quantity: 961,
      isVariant: true
    },
    {
      id: 5,
      name: "Avalon Biomed NeoSEALER Flo (2.2gm)",
      price: 4000.00,
      image: "https://d2j6dbq0eux0bg.cloudfront.net/images/15192231/1806588906.jpg",
      category: "Avalon Biomed",
      quantity: 0,
      isVariant: false
    },
    {
      id: 4,
      name: "Avalon Biomed NeoPUTTY(0.5gm) ",
      price: 2500,
      image: "https://d2j6dbq0eux0bg.cloudfront.net/images/15192231/2997835774.jpg",
      category: "Avalon Biomed",
      quantity: 961,
      isVariant: false
    },

    // Add more mock or fetched products here
  ]
</script>

<template>
  <section class="py-16 bg-white">
    <div class="max-w-screen-xl mx-auto px-4">
      <h2 class="text-3xl font-bold text-center mb-12">Best Sellers</h2>

      <!-- Centered grid layout -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        <ProductCard v-for="product in bestSellers" :key="product.id" :product="product" />
      </div>
    </div>
  </section>
</template>

