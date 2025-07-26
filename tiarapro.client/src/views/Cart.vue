<script setup lang="ts">
import { ref, computed } from 'vue'
import { useCartStore } from '../stores/CartStore'
import api from '../utils/Api'

const cartStore = useCartStore()

const cartItems = computed(() => cartStore.cart)


console.log("CartItems", cartItems.value)
const updateQuantity = (id: number, quantity: number) => {
  debugger;
  const item = cartStore.cart.find(i => i.id === id)
  console.log("item", item)
  if (!item) return

  if (quantity > item.quantity) {
    cartStore.addToCart({ id: item.id, name: item.name, price: item.price, image: item.image, quantity: item.quantity })
  } else if (quantity < item.quantity && quantity >= 1) {
    const diff = item.quantity - quantity
    for (let i = 0; i < diff; i++) {
      cartStore.removeFromCart(id)
    }
  } else {
    if (quantity == 0) {
      cartStore.removeFromCart(id)
    }
  }
}

const removeItem = (id: number) => {
  const item = cartStore.cart.find(i => i.id === id)
  if (!item) return
  for (let i = 0; i < item.quantity; i++) {
    cartStore.removeFromCart(id)
  }
}

const subtotal = computed(() =>
  cartStore.cart.reduce((total, item) => total + item.price * item.quantity, 0)
)
const shipping = ref(150)
//const tax = computed(() => subtotal.value * 0.14)
const total = computed(() => subtotal.value + shipping.value)

const promoCode = ref('')
const promoCodeError = ref('')
const promoCodeSuccess = ref('')
const discount_amount = ref(0)
const finalTotal = computed(() => total.value - discount_amount.value)

const applyPromo = async () => {
  try {
    promoCodeError.value = ''
    promoCodeSuccess.value = ''

    const userId = localStorage.getItem('userId');
    if (!userId || userId === 'null' || userId.toString() == '0') {
      promoCodeError.value = 'You must be signed in to use a promo code.';
      return;
    }

    const response = await api.post('/api/PromoCode/validate', {
      code: promoCode.value,
      order_amount: total.value,
      user_id: parseInt(userId)
    })

    const result = await response.data

    if (result.is_valid) {
      promoCodeSuccess.value = result.message
      discount_amount.value = result.discount_amount
      cartStore.setPromoCode({
        code: promoCode.value,
        discount_amount: result.discount_amount,
        promo_code_id: result.promo_code_id
      })
    } else {
      promoCodeError.value = result.message
      discount_amount.value = 0
      cartStore.setPromoCode(null)
    }
  } catch (error) {
    promoCodeError.value = 'Invalid promo code'
    discount_amount.value = 0
    cartStore.setPromoCode(null)
  }
}
</script>

<template>
  <div class="bg-gray-50 py-8">
    <div class="container-custom">
      <h1 class="text-3xl font-bold mb-8">Shopping Cart</h1>

      <div v-if="cartItems.length > 0" class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Cart Items -->
        <div class="lg:col-span-2">
          <div class="bg-white rounded-lg shadow-sm overflow-hidden">
            <div class="p-6 border-b border-gray-200">
              <h2 class="text-xl font-bold">Cart Items ({{ cartItems.length }})</h2>
            </div>
            <div class="divide-y divide-gray-200">
              <div v-for="item in cartItems" :key="item.id" class="p-6 flex flex-col sm:flex-row">
                <div class="sm:w-24 sm:h-24 mb-4 sm:mb-0 flex-shrink-0">
                  <img :src="item.image || 'https://via.placeholder.com/100'" :alt="item.name"
                    class="w-full h-full object-cover rounded-md" />
                </div>
                <div class="sm:ml-6 flex-1">
                  <div class="flex flex-col sm:flex-row sm:justify-between">
                    <div>
                      <h3 class="text-lg font-medium text-gray-900">{{ item.name }}</h3>
                      <p class="mt-1 text-sm text-gray-500">Item #{{ item.id }}</p>
                    </div>
                    <p class="mt-1 text-lg font-medium text-gray-900">{{ item.price }} EGP</p>
                  </div>
                  <div class="mt-4 flex justify-between items-center">
                    <div class="flex items-center">
                      <button @click="updateQuantity(item.id, item.quantity - 1)"
                        class="p-1 border border-gray-300 rounded-l-md bg-gray-100 hover:bg-gray-200">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24"
                          stroke="currentColor">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4" />
                        </svg>
                      </button>
                      <input type="number" :value="item.quantity" min="1"
                        class="p-1 w-12 text-center border-t border-b border-gray-300"
                        @change="updateQuantity(item.id, parseInt(($event.target as HTMLInputElement).value))" />
                      <button @click="updateQuantity(item.id, item.quantity + 1)"
                        class="p-1 border border-gray-300 rounded-r-md bg-gray-100 hover:bg-gray-200">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24"
                          stroke="currentColor">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                        </svg>
                      </button>
                    </div>
                    <button @click="removeItem(item.id)" class="text-sm font-medium text-red-600 hover:text-red-500">
                      Remove
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Order Summary -->
        <div>
          <div class="bg-white rounded-lg shadow-sm overflow-hidden">
            <div class="p-6 border-b border-gray-200">
              <h2 class="text-xl font-bold">Order Summary</h2>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div class="flex justify-between">
                  <p class="text-gray-600">Subtotal</p>
                  <p class="font-medium">{{ subtotal }} EGP</p>
                </div>
                <div class="flex justify-between">
                  <p class="text-gray-600">Shipping</p>
                  <p class="font-medium">{{ shipping }} EGP</p>
                </div>
                <div v-if="discount_amount > 0" class="flex justify-between">
                  <p class="text-gray-600">Discount</p>
                  <p class="font-medium text-green-600">-{{ discount_amount }} EGP</p>
                </div>
                <div class="border-t border-gray-200 pt-4 flex justify-between">
                  <p class="text-lg font-bold">Total</p>
                  <p class="text-lg font-bold text-primary">{{ finalTotal.toFixed(2) }} EGP</p>
                </div>
              </div>

              <!-- Promo Code -->
              <div class="mt-6">
                <label for="promo-code" class="block text-sm font-medium text-gray-700 mb-1">Promo Code</label>
                <div class="flex">
                  <input type="text" id="promo-code" v-model="promoCode"
                    class="block w-full rounded-l-md border-gray-300 shadow-sm focus:border-primary focus:ring-primary"
                    placeholder="Enter code" />
                  <button @click="applyPromo"
                    class="bg-gray-200 text-gray-700 px-4 py-2 rounded-r-md hover:bg-gray-300">
                    Apply
                  </button>
                </div>
                <p v-if="promoCodeError" class="mt-2 text-sm text-red-600">{{ promoCodeError }}</p>
                <p v-if="promoCodeSuccess" class="mt-2 text-sm text-green-600">{{ promoCodeSuccess }}</p>
              </div>

              <!-- Checkout Button -->
              <div class="mt-6">
                <router-link to="/checkout" class="btn btn-primary w-full flex justify-center">
                  Proceed to Checkout
                </router-link>
              </div>
            </div>
          </div>

          <!-- Continue Shopping -->
          <div class="mt-6 text-center">
            <router-link to="/products" class="text-primary hover:text-primary-dark font-medium">
              ← Continue Shopping
            </router-link>
          </div>
        </div>
      </div>

      <!-- Empty Cart -->
      <div v-else class="bg-white rounded-lg shadow-sm p-8 text-center">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 mx-auto text-gray-400 mb-4" fill="none"
          viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
            d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
        </svg>
        <h2 class="text-2xl font-bold mb-2">Your cart is empty</h2>
        <p class="text-gray-600 mb-6">Looks like you haven't added any products to your cart yet.</p>
        <router-link to="/products" class="btn btn-primary">
          Start Shopping
        </router-link>
      </div>
    </div>
  </div>
</template>
