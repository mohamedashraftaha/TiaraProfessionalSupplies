<script setup lang="ts">
import { useCartStore } from '../stores/CartStore'
import { computed, onMounted } from 'vue'
const cartStore = useCartStore();
const props = defineProps<{
    subcategory: {
        id: number;
        name: string;
        logo_url: string;
    }
}>()
</script>
<template>
    <div
        class="card hover:shadow-2xl transition-shadow overflow-hidden rounded-xl bg-gradient-to-br from-white to-blue-50 relative group">
        <div class="absolute top-3 left-3 z-10">
            <span
                class="bg-blue-500 text-white text-xs font-semibold px-3 py-1 rounded-full shadow-md">Subcategory</span>
        </div>
        <router-link :to="`/products?category=${subcategory.id}`"
            class="block h-48 overflow-hidden relative rounded-t-xl">
            <img :src="subcategory.logo_url" :alt="subcategory.name"
                class="w-full h-full object-contain group-hover:scale-105 transition-transform duration-300">
        </router-link>
        <div class="p-4 flex flex-col items-center">
            <router-link :to="`/products?category=${subcategory.id}`" class="block w-full text-center">
                <h3 class="font-semibold text-lg mb-2 hover:text-blue-600 transition-colors">{{ subcategory.name }}</h3>
            </router-link>
        </div>
    </div>
</template>
