<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useCartStore } from '../stores/CartStore'
import api from '../utils/Api.ts'
import { showErrorToast, showSuccessToast, addToCartToast } from '../utils/helpers.ts'
import type { ProductVariant } from '../interfaces/ProductVariant.ts'
const route = useRoute()
const productId = computed(() => Number(route.params.id))

const cartStore = useCartStore();
const totalItems = computed(() => cartStore.totalItems)
// Mock product data
interface Review {
  id: number
  comment: string
  rating: number
  user: string
  date: string
}


interface Product {
  id: number
  name: string
  price: number
  description?: string
  features?: string[]
  specifications?: { name: string; value: string }[]
  image: string
  category: string
  stock: number
  reviews?: Review[]
  isVariant: boolean
}
const variableOptionNames = {
  0: '9061100 Extrathin',
  1: '9061101 13T',
  2: '9061102 13',
  3: '9061106 Ultra thin',
  4: '9061109 Mini',
  5: 'Blue'
}


const checkVariantStockLevel = async (variantId: number): Promise<number> => {
  try {
    const variant = await api.get<ProductVariant>(`api/product/variant/${product.value.id}/${selectedSide.value}/${selectedSize.value}/${selectedVariableOption.value}`)

    return variant.data.quantity || 0
  } catch (error) {
    console.error('Failed to fetch variant stock:', error)
    return 0
  }
}
const product = ref<Partial<Product>>({})
const fetchProductDetails = async () => {
  try {
    console.log("route", route.params.id)
    const productId = route.params.id

    const response = await api.get(`/api/product/${productId}`)
    const productFetched = response.data
    console.log("productFetched", productFetched)

    if (productFetched) {
      product.value.id = productFetched.id
      product.value.name = productFetched.name
      product.value.price = productFetched.price
      product.value.description = productFetched.description
      product.value.features = productFetched.features
      product.value.specifications = productFetched.specifications
      product.value.image = productFetched.logoUrl != undefined || productFetched.logoUrl != null ? productFetched.logoUrl : "https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg"
      product.value.category = productFetched.categoryName
      product.value.stock = productFetched.quantity
      product.value.reviews = productFetched.reviews
      product.value.isVariant = productFetched.isVariant
      if (productFetched.isVariant && productFetched.variant_products) {
        variantProducts.value = productFetched.variant_products
        classifyVariants()
        updateAvailableSelectors()
        prefetchVariantStock()
      }
    } else {
      console.error('Product not found in API response')
    }
  } catch (error) {
    console.error('Error fetching product details:', error)
  }
}

const selectedImage = ref(0)
const quantity = ref(1)

// Check if product is in cart
const isInCart = computed(() => {
  if (product.value.isVariant && selectedVariant.value) {
    return cartStore.cart.some(item => item.id === product.value.id && item.sku === selectedVariant.value.sku)
  } else {
    return cartStore.cart.some(item => item.id === product.value.id)
  }
})

// Get quantity of product in cart
const quantityInCart = computed(() => {
  if (product.value.isVariant && selectedVariant.value) {
    const cartItem = cartStore.cart.find(item => item.id === product.value.id && item.sku === selectedVariant.value.sku)
    return cartItem ? cartItem.quantity : 0
  } else {
    const cartItem = cartStore.cart.find(item => item.id === product.value.id)
    return cartItem ? cartItem.quantity : 0
  }
})

const increaseQuantity = () => {
  if (quantity.value < product.value.stock) {
    quantity.value++
  }
}

const decreaseQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--
  }
}

const addToCart = async () => {
  debugger;
  if (product.value.isVariant) {
    if (!selectedVariant.value) {
      showErrorToast('Please select a valid combination of side and size.');
      return;
    }

    const stockLevel = await checkVariantStockLevel(selectedVariant.value.id)
    if (stockLevel <= 0) {
      showErrorToast('This option is out of stock.');
      return;
    } else {
      if (stockLevel < quantity.value) {
        showErrorToast('Oops! That is more than we can offer for this option.Please choose a lower quantity.');
        return;
      }
    }
    var productName = product.value.name
    if (selectedSide.value != null) {
      productName += `,${sideMap[selectedSide.value]}`
    }
    if (selectedSize.value != null) {
      productName += `,S${selectedSize.value}`
    }
    if (selectedVariableOption.value != null) {
      productName += `,${variableOptionNames[selectedVariableOption.value]}`
    }

    cartStore.addToCart({
      id: product.value.id,
      name: productName,
      price: product.value.price,
      quantity: quantity.value,
      image: product.value.image,
      sku: selectedVariant.value.sku,
      side: selectedSide.value,
      size: selectedSize.value
    })
    quantity.value = 1
    addToCartToast('Added to cart!')
  } else {
    cartStore.addToCart({
      id: product.value.id,
      name: product.value.name,
      price: product.value.price,
      quantity: quantity.value,
      image: product.value.image
    })
    quantity.value = 1
    addToCartToast('Added to cart!')
  }
}

// Update incrementCartItem to use direct cart management
const incrementCartItem = () => {
  debugger;

  if (product.value.isVariant) {
    if (!isInCart.value) {
      // If not in cart yet, add it with quantity 1
      cartStore.addToCart({
        id: product.value.id,
        name: product.value.name,
        price: product.value.price,
        quantity: quantity.value,
        image: product.value.image

      })
      addToCartToast('Added to cart!')
    } else if (quantityInCart.value < product.value.stock) {
      // Otherwise increment existing item
      cartStore.addToCart({
        id: product.value.id,
        name: product.value.name,
        price: product.value.price,
        quantity: quantity.value,
        image: product.value.image
      })
      addToCartToast('Added to cart!')
    }

  }
  else {
    if (!isInCart.value) {
      // If not in cart yet, add it with quantity 1
      cartStore.addToCart({
        id: product.value.id,
        name: product.value.name,
        price: product.value.price,
        quantity: 1,
        image: product.value.image

      })
      addToCartToast('Added to cart!')
    } else if (quantityInCart.value < product.value.stock) {
      // Otherwise increment existing item
      cartStore.addToCart({
        id: product.value.id,
        name: product.value.name,
        price: product.value.price,
        quantity: 1,
        image: product.value.image
      })
      addToCartToast('Added to cart!')
    }


  }
}

// Update decrementCartItem to use direct cart management
const decrementCartItem = () => {
  if (isInCart.value) {
    cartStore.removeFromCart(product.value.id)
  }
}

// Remove item completely from cart
const removeFromCart = () => {
  // Remove the item completely by calling removeFromCart for each quantity
  const currentQuantity = quantityInCart.value
  for (let i = 0; i < currentQuantity; i++) {
    cartStore.removeFromCart(product.value.id)
  }
}

const activeTab = ref('description')
const selectedSize = ref(null)
const selectedSide = ref(null)

const sideMap = {
  0: 'Upper Right',
  1: 'Lower Right',
  2: 'Lower Left',
  3: 'Upper Left',
  4: 'Primary Central',
  5: 'Primary Lateral',
  6: 'Primary Cuspid',
  7: 'Primary Upper Cuspid',
  8: 'Primary Lower Cuspid',
}

const variantProducts = ref([])
const sideSizeVariants = ref([])
const sizeOnlyVariants = ref([])
const variableOptionVariants = ref([])
const availableSides = ref([])
const availableSizes = ref([])
const availableVariableOptions = ref([])
const selectedVariant = ref(null)
const selectedVariableOption = ref(null)

// After variantProducts.value is set in fetchProductDetails, store stock info for each variant
const variantStockMap = ref({})

function prefetchVariantStock() {
  // Build a map: key = sku, value = quantity
  const map = {}
  if (variantProducts.value && Array.isArray(variantProducts.value)) {
    for (const v of variantProducts.value) {
      map[v.sku] = v.quantity
    }
  }
  variantStockMap.value = map
}

function classifyVariants() {
  sideSizeVariants.value = variantProducts.value.filter(v => !v.only_one_option)
  sizeOnlyVariants.value = variantProducts.value.filter(v => v.only_one_option && v.side == null && v.size != null)
  variableOptionVariants.value = variantProducts.value.filter(v => v.only_one_option && v.side == null && v.size == null)
}

function updateAvailableSelectors() {
  if (sideSizeVariants.value.length) {
    availableSides.value = [...new Set(sideSizeVariants.value.map(v => v.side))]
    if (selectedSide.value === null) selectedSide.value = availableSides.value[0]
    updateAvailableSizes()
  } else if (sizeOnlyVariants.value.length) {
    availableSizes.value = [...new Set(sizeOnlyVariants.value.map(v => v.size))]
    if (selectedSize.value === null) selectedSize.value = availableSizes.value[0]
  } else if (variableOptionVariants.value.length) {
    availableVariableOptions.value = [...new Set(variableOptionVariants.value.map(v => v.variable_option))]
    if (selectedVariableOption.value === null) selectedVariableOption.value = availableVariableOptions.value[0]
  }
}

function updateAvailableSizes() {
  if (selectedSide.value !== null) {
    availableSizes.value = sideSizeVariants.value
      .filter(v => v.side === selectedSide.value)
      .map(v => v.size)
      .sort((a, b) => a - b)
    selectedSize.value = availableSizes.value[0]
    updateSelectedVariant()
  }
}

function updateSelectedVariant() {
  if (sideSizeVariants.value.length) {
    selectedVariant.value = sideSizeVariants.value.find(
      v => v.side === selectedSide.value && v.size === selectedSize.value
    )
  } else if (sizeOnlyVariants.value.length) {
    selectedVariant.value = sizeOnlyVariants.value.find(
      v => v.size === selectedSize.value
    )
  } else if (variableOptionVariants.value.length) {
    selectedVariant.value = variableOptionVariants.value.find(
      v => v.variable_option === selectedVariableOption.value
    )
  } else {
    selectedVariant.value = null
  }
}

watch(selectedSide, updateAvailableSizes)
watch(selectedSize, updateSelectedVariant)
watch(selectedVariableOption, updateSelectedVariant)

// List of all cart items for this product (all variants)
const cartItemsForProduct = computed(() => {
  return cartStore.cart.filter(item => item.id === product.value.id)
})

onMounted(() => {
  fetchProductDetails()
})


</script>

<template>
  <div class="bg-gray-50 py-8">
    <div class="container-custom">
      <!-- Breadcrumbs -->
      <nav class="flex mb-8" aria-label="Breadcrumb">
        <ol class="inline-flex items-center space-x-1 md:space-x-3">
          <li class="inline-flex items-center">
            <router-link to="/" class="text-gray-600 hover:text-primary">
              Home
            </router-link>
          </li>
          <li>
            <div class="flex items-center">
              <svg class="w-6 h-6 text-gray-400" fill="currentColor" viewBox="0 0 20 20"
                xmlns="http://www.w3.org/2000/svg">
                <path fill-rule="evenodd"
                  d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z"
                  clip-rule="evenodd"></path>
              </svg>
              <router-link to="/products" class="ml-1 text-gray-600 hover:text-primary md:ml-2">
                Products
              </router-link>
            </div>
          </li>
          <li aria-current="page">
            <div class="flex items-center">
              <svg class="w-6 h-6 text-gray-400" fill="currentColor" viewBox="0 0 20 20"
                xmlns="http://www.w3.org/2000/svg">
                <path fill-rule="evenodd"
                  d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z"
                  clip-rule="evenodd"></path>
              </svg>
              <span class="ml-1 text-gray-500 md:ml-2">{{ product.name }}</span>
            </div>
          </li>
        </ol>
      </nav>

      <!-- Product Details -->
      <div class="bg-white rounded-lg shadow-sm overflow-hidden">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8 p-6">
          <!-- Product Images -->
          <div>
            <div class="mb-4 overflow-hidden rounded-lg">
              <img :src="product.image" :alt="product.name" class="w-full w-full">
            </div>
          </div>

          <!-- Product Info -->
          <div>
            <!-- Cart badge -->
            <div class="flex justify-end mb-2">
              <router-link to="/cart" class="relative inline-flex items-center group">
                <svg xmlns="http://www.w3.org/2000/svg"
                  class="h-7 w-7 text-gray-600 group-hover:text-blue-600 transition-colors" fill="none"
                  viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
                <span v-if="totalItems > 0"
                  class="absolute -top-2 -right-2 bg-blue-600 text-white text-xs font-bold rounded-full px-2 py-0.5 shadow">{{
                    totalItems }}</span>
              </router-link>
            </div>
            <h1 class="text-3xl font-bold mb-2">{{ product.name }}</h1>
            <div class="flex items-center mb-4">
              <div class="flex text-yellow-400">
                <svg v-for="i in 5" :key="i" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20"
                  fill="currentColor">
                  <path
                    d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                </svg>
              </div>
              <span class="ml-2 text-gray-600">{{ product.reviews }} reviews</span>
            </div>
            <p class="text-2xl font-bold text-primary mb-4">{{ product.price }} EGP</p>
            <p class="text-gray-600 mb-6 whitespace-pre-wrap">{{ product.description }}</p>

            <!-- Stock Status -->
            <div class="mb-6">
              <span v-if="product.stock > 0" class="text-green-600 font-medium">
                <span class="inline-block w-3 h-3 bg-green-600 rounded-full mr-1"></span>
                In Stock
              </span>
              <span v-else class="text-red-600 font-medium">
                <span class="inline-block w-3 h-3 bg-red-600 rounded-full mr-1"></span>
                Out of Stock
              </span>
            </div>

            <!-- Streamlined Cart Controls -->
            <div v-if="!product.isVariant" class="mb-6">
              <label for="quantity" class="block text-sm font-medium text-gray-700 mb-2">Quantity</label>
              <div class="flex items-center">
                <!-- Quantity and Cart Controls Combined -->
                <div class="flex items-center">
                  <button @click="decrementCartItem"
                    class="p-3 border border-gray-300 rounded-l-md bg-gray-50 hover:bg-gray-100 transition-colors">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                      stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4" />
                    </svg>
                  </button>
                  <span
                    class="px-4 py-3 font-medium text-lg border-t border-b border-gray-300 min-w-[3rem] text-center">
                    {{ isInCart ? quantityInCart : 0 }}
                  </span>
                  <button @click="incrementCartItem"
                    class="p-3 border border-gray-300 rounded-r-md bg-gray-50 hover:bg-gray-100 transition-colors"
                    :disabled="product.stock <= 0 || quantityInCart >= product.stock">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                      stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                    </svg>
                  </button>
                </div>
                <!-- Remove button (only visible when item is in cart) -->
                <button v-if="isInCart" @click="removeFromCart"
                  class="ml-4 px-4 py-3 border border-red-500 rounded-md bg-white text-red-500 hover:bg-red-50 flex items-center">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                  Remove
                </button>
              </div>
            </div>
            <!-- Variant Add to Cart Controls -->
            <div v-if="product.isVariant" class="mb-6">
              <label for="quantity" class="block text-sm font-medium text-gray-700 mb-2">Quantity</label>
              <div class="flex items-center mb-4">
                <button @click="quantity > 1 ? quantity-- : null"
                  class="p-3 border border-gray-300 rounded-l-md bg-gray-50 hover:bg-gray-100 transition-colors">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4" />
                  </svg>
                </button>
                <span class="px-4 py-3 font-medium text-lg border-t border-b border-gray-300 min-w-[3rem] text-center">
                  {{ quantity }}
                </span>
                <button @click="quantity++"
                  class="p-3 border border-gray-300 rounded-r-md bg-gray-50 hover:bg-gray-100 transition-colors">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                  </svg>
                </button>
              </div>
              <button @click="addToCart"
                class="w-full bg-blue-600 text-white py-3 rounded-lg font-semibold hover:bg-blue-700 transition-colors"
                :disabled="!selectedVariant">
                Add to Cart
              </button>
            </div>

            <!-- Cart Summary (show all variants in cart for this product) -->
            <div v-if="cartItemsForProduct.length > 0"
              class="mb-6 p-3 bg-gray-100 rounded-md border border-gray-200 text-sm flex flex-col gap-2 items-start">
              <div class="flex justify-between items-center w-full">
                <span class="font-semibold text-gray-700">Cart Summary</span>
                <span class="font-bold text-primary">
                  {{cartItemsForProduct.reduce((sum, item) => sum + item.price * item.quantity, 0).toLocaleString()}}
                  EGP
                </span>
              </div>
              <div class="w-full">
                <div v-for="item in cartItemsForProduct" :key="item.sku || item.id"
                  class="flex justify-between items-center py-1">
                  <span class="text-gray-700">{{ item.name }}</span>
                  <span class="text-gray-600">x{{ item.quantity }}</span>
                  <span class="text-gray-800 font-medium">{{ (item.price * item.quantity).toLocaleString() }} EGP</span>
                </div>
              </div>
              <router-link to="/cart"
                class="mt-1 px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 text-xs font-medium">View
                Cart</router-link>
            </div>

            <!-- Wishlist Button -->
            <div class="flex justify-end">
              <button class="btn border border-gray-300 bg-white hover:bg-gray-50 p-3 rounded-full flex items-center">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-gray-600" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                </svg>
              </button>
            </div>

            <!-- Variant Options -->
            <div v-if="product.isVariant" class="mb-6 space-y-6">
              <!-- Side + Size Variant -->
              <template v-if="sideSizeVariants.length">
                <!-- Side Selection -->
                <div class="space-y-3">
                  <label class="flex items-center text-sm font-semibold text-gray-800 mb-3">
                    <div class="w-1 h-5 bg-gradient-to-b from-blue-500 to-purple-600 rounded-full mr-3"></div>
                    Side Selection
                  </label>
                  <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                    <label v-for="side in availableSides" :key="side" class="relative cursor-pointer group">
                      <input type="radio" :value="side" v-model="selectedSide" class="sr-only" />
                      <div
                        class="border-2 rounded-xl p-4 text-center transition-all duration-300 ease-in-out transform hover:scale-105 hover:shadow-lg"
                        :class="selectedSide === side
                          ? 'border-blue-500 bg-gradient-to-r from-blue-50 to-purple-50 shadow-md ring-2 ring-blue-200'
                          : 'border-gray-200 bg-white hover:border-blue-300 hover:bg-gray-50'">
                        <span class="font-medium transition-colors duration-200"
                          :class="selectedSide === side ? 'text-blue-700' : 'text-gray-700'">
                          {{ sideMap[side] }}
                        </span>
                        <!-- Checkmark for selected state -->
                        <div v-if="selectedSide === side"
                          class="absolute -top-2 -right-2 w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center shadow-lg">
                          <svg class="w-3 h-3 text-white" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd"
                              d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                              clip-rule="evenodd"></path>
                          </svg>
                        </div>
                      </div>
                    </label>
                  </div>
                </div>
                <!-- Size Selection -->
                <div class="space-y-3">
                  <label class="flex items-center text-sm font-semibold text-gray-800 mb-3">
                    <div class="w-1 h-5 bg-gradient-to-b from-blue-500 to-purple-600 rounded-full mr-3"></div>
                    Size Selection
                  </label>
                  <div class="grid grid-cols-2 sm:grid-cols-4 gap-3">
                    <label v-for="size in availableSizes" :key="size" class="relative cursor-pointer group">
                      <input type="radio" :value="size" v-model="selectedSize" class="sr-only"
                        :disabled="sideSizeVariants.find(v => v.side === selectedSide && v.size === size && variantStockMap[v.sku] === 0) || sizeOnlyVariants.find(v => v.size === size && variantStockMap[v.sku] === 0)" />
                      <div
                        class="border-2 rounded-xl p-4 text-center transition-all duration-300 ease-in-out transform hover:scale-105 hover:shadow-lg"
                        :class="selectedSize === size
                          ? 'border-blue-500 bg-gradient-to-r from-blue-50 to-purple-50 shadow-md ring-2 ring-blue-200'
                          : 'border-gray-200 bg-white hover:border-blue-300 hover:bg-gray-50'">
                        <span class="font-medium transition-colors duration-200"
                          :class="selectedSize === size ? 'text-blue-700' : 'text-gray-700'">
                          {{ size }}
                        </span>
                        <!-- Out of Stock label for size -->
                        <span
                          v-if="sideSizeVariants.find(v => v.side === selectedSide && v.size === size && variantStockMap[v.sku] === 0) || sizeOnlyVariants.find(v => v.size === size && variantStockMap[v.sku] === 0)"
                          class="absolute top-2 right-2 bg-red-100 text-red-600 text-xs font-semibold px-2 py-0.5 rounded">Out
                          of Stock</span>
                        <!-- Checkmark for selected state -->
                        <div v-if="selectedSize === size"
                          class="absolute -top-2 -right-2 w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center shadow-lg">
                          <svg class="w-3 h-3 text-white" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd"
                              d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                              clip-rule="evenodd"></path>
                          </svg>
                        </div>
                      </div>
                    </label>
                  </div>
                </div>
              </template>
              <!-- Size Only Variant -->
              <template v-else-if="sizeOnlyVariants.length">
                <div class="space-y-3">
                  <label class="flex items-center text-sm font-semibold text-gray-800 mb-3">
                    <div class="w-1 h-5 bg-gradient-to-b from-blue-500 to-purple-600 rounded-full mr-3"></div>
                    Size Selection
                  </label>
                  <div class="grid grid-cols-2 sm:grid-cols-4 gap-3">
                    <label v-for="size in availableSizes" :key="size" class="relative cursor-pointer group">
                      <input type="radio" :value="size" v-model="selectedSize" class="sr-only"
                        :disabled="sizeOnlyVariants.find(v => v.size === size && variantStockMap[v.sku] === 0)" />
                      <div
                        class="border-2 rounded-xl p-4 text-center transition-all duration-300 ease-in-out transform hover:scale-105 hover:shadow-lg"
                        :class="selectedSize === size
                          ? 'border-blue-500 bg-gradient-to-r from-blue-50 to-purple-50 shadow-md ring-2 ring-blue-200'
                          : 'border-gray-200 bg-white hover:border-blue-300 hover:bg-gray-50'">
                        <span class="font-medium transition-colors duration-200"
                          :class="selectedSize === size ? 'text-blue-700' : 'text-gray-700'">
                          {{ size }}
                        </span>
                        <!-- Out of Stock label for size -->
                        <span v-if="sizeOnlyVariants.find(v => v.size === size && variantStockMap[v.sku] === 0)"
                          class="absolute top-2 right-2 bg-red-100 text-red-600 text-xs font-semibold px-2 py-0.5 rounded">Out
                          of Stock</span>
                        <!-- Checkmark for selected state -->
                        <div v-if="selectedSize === size"
                          class="absolute -top-2 -right-2 w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center shadow-lg">
                          <svg class="w-3 h-3 text-white" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd"
                              d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                              clip-rule="evenodd"></path>
                          </svg>
                        </div>
                      </div>
                    </label>
                  </div>
                </div>
              </template>
              <!-- Variable Option Variant -->
              <template v-else-if="variableOptionVariants.length">
                <div class="space-y-3">
                  <label class="flex items-center text-sm font-semibold text-gray-800 mb-3">
                    <div class="w-1 h-5 bg-gradient-to-b from-blue-500 to-purple-600 rounded-full mr-3"></div>
                    Option Selection
                  </label>
                  <div class="grid grid-cols-2 sm:grid-cols-4 gap-3">
                    <label v-for="option in availableVariableOptions" :key="option"
                      class="relative cursor-pointer group">
                      <input type="radio" :value="option" v-model="selectedVariableOption" class="sr-only" />
                      <div
                        class="border-2 rounded-xl p-4 text-center transition-all duration-300 ease-in-out transform hover:scale-105 hover:shadow-lg"
                        :class="selectedVariableOption === option
                          ? 'border-blue-500 bg-gradient-to-r from-blue-50 to-purple-50 shadow-md ring-2 ring-blue-200'
                          : 'border-gray-200 bg-white hover:border-blue-300 hover:bg-gray-50'">
                        <span class="font-medium transition-colors duration-200 block whitespace-normal break-words"
                          :class="selectedVariableOption === option ? 'text-blue-700' : 'text-gray-700'">
                          {{ variableOptionNames[option] }}
                        </span>
                        <div v-if="selectedVariableOption === option"
                          class="absolute -top-2 -right-2 w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center shadow-lg">
                          <svg class="w-3 h-3 text-white" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd"
                              d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                              clip-rule="evenodd"></path>
                          </svg>
                        </div>
                      </div>
                    </label>
                  </div>
                </div>
              </template>
            </div>
          </div>
        </div>

        <!-- Tabs -->
        <div class="border-t border-gray-200">
          <div class="flex border-b border-gray-200">
            <button @click="activeTab = 'description'" class="px-6 py-3 font-medium text-sm"
              :class="activeTab === 'description' ? 'border-b-2 border-primary text-primary' : 'text-gray-500 hover:text-gray-700'">
              Description
            </button>
            <button @click="activeTab = 'specifications'" class="px-6 py-3 font-medium text-sm"
              :class="activeTab === 'specifications' ? 'border-b-2 border-primary text-primary' : 'text-gray-500 hover:text-gray-700'">
              Specifications
            </button>
            <button @click="activeTab = 'reviews'" class="px-6 py-3 font-medium text-sm"
              :class="activeTab === 'reviews' ? 'border-b-2 border-primary text-primary' : 'text-gray-500 hover:text-gray-700'">
              Reviews ({{ product.reviews }})
            </button>
          </div>

          <!-- Tab Content -->
          <div class="p-6">
            <!-- Description Tab -->
            <div v-if="activeTab === 'description'">
              <div v-if="product.description || product.features">
                <p v-if="product.description" class="text-gray-600 mb-4  whitespace-pre-wrap">{{ product.description }}
                </p>

                <div v-if="product.features && product.features.length">
                  <h3 class="font-bold text-lg mb-2">Features</h3>
                  <ul class="list-disc pl-5 mb-4 text-gray-600 space-y-1">
                    <li v-for="(feature, index) in product.features" :key="index">{{ feature }}</li>
                  </ul>
                </div>
              </div>
              <div v-else>
                <p class="text-gray-600 mb-4">No Description Available</p>
              </div>
            </div>



            <!-- Specifications Tab -->

            <div v-if="activeTab === 'specifications'">
              <div v-if="product.specifications && product.specifications.length">
                <div class="overflow-hidden">
                  <table class="min-w-full divide-y divide-gray-200">
                    <tbody class="divide-y divide-gray-200">
                      <tr v-for="(spec, index) in product.specifications" :key="index">
                        <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 bg-gray-50 w-1/3">
                          {{ spec.name }}
                        </td>
                        <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          {{ spec.value }}
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
              <div v-else>
                <p class="text-gray-600 mb-4">No specifications available.</p>
              </div>
            </div>


            <!-- Reviews Tab -->
            <div v-if="activeTab === 'reviews'">
              <div v-if="product?.reviews?.length">
                <div v-for="review in product.reviews" :key="review.id"
                  class="mb-6 pb-6 border-b border-gray-200 last:border-b-0 last:mb-0 last:pb-0">
                  <div class="flex items-center mb-2">
                    <div class="flex text-yellow-400">
                      <svg v-for="i in 5" :key="i" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5"
                        viewBox="0 0 20 20" :fill="i <= review.rating ? 'currentColor' : 'none'">
                        <path
                          d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                      </svg>
                    </div>
                    <span class="ml-2 text-gray-600">{{ review.rating }}/5</span>
                  </div>
                  <h4 class="font-medium mb-1">{{ review.user }}</h4>
                  <p class="text-sm text-gray-500 mb-2">{{ review.date }}</p>
                  <p class="text-gray-600">{{ review.comment }}</p>
                </div>
              </div>
              <div v-else class="text-center py-8">
                <p class="text-gray-500">No reviews yet. Be the first to review this product!</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
