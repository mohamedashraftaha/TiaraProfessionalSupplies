<script setup lang="ts">
import { useCartStore } from '../stores/CartStore'
import { computed, onMounted } from 'vue'
const cartStore = useCartStore();
const props = defineProps<{
  product: {
    id: number;
    name: string;
    price: number;
    image: string;
    category: string;
    quantity: number;
    isVariant: boolean;
  }
}>()
const isOutOfStock = computed(() => props.product.quantity <= 0)
// Check if product is in cart
const isInCart = computed(() => {
  return cartStore.cart.some(item => item.id === props.product.id)
})
// Get quantity of product in cart
const quantityInCart = computed(() => {
  const cartItem = cartStore.cart.find(item => item.id === props.product.id)
  return cartItem ? cartItem.quantity : 0
})
// Toggle add/remove from cart
const toggleCart = () => {
  if (isInCart.value) {
    cartStore.removeFromCart(props.product.id)
  } else {
    cartStore.addToCart({
      id: props.product.id,
      name: props.product.name,
      price: props.product.price,
      quantity: 1,
      image: props.product.image
    })
  }
}
// Increment quantity in cart
const incrementQuantity = () => {
  console.log("pro", props.product);
  cartStore.addToCart({
    id: props.product.id,
    name: props.product.name,
    price: props.product.price,
    quantity: 1,
    image: props.product.image
  })
}
// Decrement quantity in cart
const decrementQuantity = () => {
  cartStore.removeFromCart(props.product.id)
}
</script>
<template>
  <div class="card hover:shadow-lg transition-shadow overflow-hidden" :class="{ 'opacity-90 grayscale': isOutOfStock }">
    <router-link :to="`/products/${product.id}`" class="block h-48 overflow-hidden relative">
      <img :src="product.image" :alt="product.name" class="w-full h-full object-contain">
      <!-- Out of stock overlay -->
      <div v-if="isOutOfStock" class="absolute inset-0 bg-black bg-opacity-50 flex items-center justify-center">
        <div
          class="bg-red-500 text-white py-1 px-3 rounded-full transform rotate-12 font-bold text-sm tracking-wider shadow-lg">
          OUT OF STOCK
        </div>
      </div>
    </router-link>
    <div class="p-4">
      <div class="text-sm text-gray-500 mb-1">{{ product.category }}</div>
      <router-link :to="`/products/${product.id}`" class="block">
        <h3 class="font-semibold text-lg mb-2 hover:text-primary">{{ product.name }}</h3>
      </router-link>
      <div class="flex justify-between items-center">
        <div class="flex flex-col">
          <span class="font-bold text-lg">{{ product.price }} EGP</span>
          <span v-if="isOutOfStock" class="text-red-500 text-xs font-medium">Currently unavailable</span>
        </div>
        <!-- Cart controls -->
        <div v-if="isOutOfStock" class="tooltip" data-tip="Notify me when available">
          <button class="btn-outline border-gray-300 text-gray-500 hover:bg-gray-100 btn-sm rounded-full p-2">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1" />
            </svg>
          </button>
        </div>
        <div v-else>
          <div v-if="product.isVariant">
            <router-link :to="`/products/${product.id}`">
              <button class="btn-primary btn-sm rounded-full p-2 w-full">View Options</button>
            </router-link>
          </div>
          <div v-else>
            <div v-if="isInCart" class="flex items-center space-x-2">
              <button @click="decrementQuantity" class="btn-primary btn-sm rounded-full p-2">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18 12H6" />
                </svg>
              </button>
              <span class="font-medium">{{ quantityInCart }}</span>
              <button @click="incrementQuantity" class="btn-primary btn-sm rounded-full p-2">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
              </button>
            </div>
            <button v-else @click="toggleCart" class="btn-primary btn-sm rounded-full p-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
