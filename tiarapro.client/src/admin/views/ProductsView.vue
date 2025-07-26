<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { showErrorToast, showSuccessToast } from '../../utils/helpers.ts'
import api from '../../utils/Api.ts'

const products = ref([])
const searchQuery = ref('')
const selectedCategory = ref('all')
const showDeleteModal = ref(false)
const showEditModal = ref(false)
const showAddModal = ref(false)
const showEditVariantModal = ref(false)
const productToDelete = ref(null)
const productToEdit = ref(null)
const variantToEdit = ref(null)
const expandedProducts = ref(new Set())
const editForm = ref({
  name: '',
  price: 0,
  categoryId: '',
  sku: '',
  quantity: 0,
  logoUrl: '',
  isVariant: false
})
const editVariantForm = ref({
  id: null,
  sku: '',
  quantity: 0,
  side: 0,
  size: 0,
  onlyOneOption: false,
  variableOption: '',
  variantType: 'side_size'
})
const isLoading = ref(true)
const isSaving = ref(false)
const isExporting = ref(false)
const allCategories = ref([])
const activeCategories = computed(() => allCategories.value.filter(c => c.isActive))

// Product side enum mapping
const productSideNames = {
  0: 'Upper Right',
  1: 'Lower Right',
  2: 'Lower Left',
  3: 'Upper Left',
  4: 'Primary Central',
  5: 'Primary Lateral',
  6: 'Primary Cuspid',
  7: 'Primary Upper Cuspid',
  8: 'Primary Lower Cuspid'
}

const variableOptionNames = {
  0: '9061100 Extrathin',
  1: '9061101 13T',
  2: '9061102 13',
  3: '9061106 Ultra thin',
  4: '9061109 Mini',
  5: 'Blue'
}

// Helper: map string key to enum value for variable options
const variableOptionKeyToEnum = Object.fromEntries(Object.keys(variableOptionNames).map(k => [k, Number(k)]))

// Filter products based on search and category
const filteredProducts = computed(() => {
  return products.value.filter(product => {
    const matchesSearch = product.name.toLowerCase().includes(searchQuery.value.toLowerCase())
    const matchesCategory = selectedCategory.value === 'all' || product.categoryId === selectedCategory.value
    return matchesSearch && matchesCategory
  })
})

const getProducts = async () => {
  isLoading.value = true
  try {
    const response = await api.get("/api/product/withVariants")
    if (response.status !== 200) {
      console.error("Error in getting products", response)
      return
    }
    const allProducts = []
    allProducts.push(...response.data)
    products.value = allProducts.map((product) => {
      const base = {
        id: product.id,
        price: product.price,
        name: product.name,
        image: product.logoUrl ?? "https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg",
        logoUrl: product.logoUrl || "",
        sku: product.sku,
        categoryId: product.categoryId,
        quantity: product.quantity,
        isVariant: product.isVariant || false,
        variant_products: [],
      }
      // If variants, attach them as an array, mapped like base products
      if (product.isVariant && Array.isArray(product.variant_products)) {
        base.variant_products = product.variant_products.map(v => ({
          id: v.id,
          product_id: v.product_id,
          sku: v.sku,
          quantity: v.quantity,
          side: v.side,
          size: v.size,
          range: v.range,
          parent_product_id: v.parent_product_id,
          variable_option: v.variable_option,
          only_one_option: v.only_one_option,
          sideName: productSideNames[v.side] || `Side ${v.side}`,
          displayName: getVariantDisplayName(v)
        }))

        console.log(base.variant_products)
      }
      return base
    })

  } catch (error) {
    console.error(error)
  } finally {
    isLoading.value = false
  }
}

const toggleProductExpansion = (productId) => {
  if (expandedProducts.value.has(productId)) {
    expandedProducts.value.delete(productId)
  } else {
    expandedProducts.value.add(productId)
  }
}

const confirmDelete = (product) => {
  productToDelete.value = product
  showDeleteModal.value = true
}

const handleDelete = async () => {
  try {
    console.log(productToDelete.value)
    const response = await api.delete(`/api/product/${productToDelete.value.id}`)
    if (response.status == 200) {
      showDeleteModal.value = false
      products.value = products.value.filter(p => p.id !== productToDelete.value.id)
      showSuccessToast("Product deleted successfully")
      productToDelete.value = null
    }
  } catch (error) {
    console.error(error)
  }
}

const openEditModal = (product) => {
  productToEdit.value = product
  editForm.value = {
    name: product.name,
    price: product.price,
    categoryId: product.categoryId,
    quantity: product.quantity,
    logoUrl: product.logoUrl || "",
    sku: product.sku || "",
    isVariant: product.isVariant || false
  }
  showEditModal.value = true
}

const openAddModal = () => {
  productToEdit.value = null
  editForm.value = {
    name: '',
    price: 0,
    categoryId: allCategories.value[0]?.id.toString() || '',
    quantity: 0,
    logoUrl: '',
    sku: '',
    isVariant: false
  }
  showAddModal.value = true
}

const openEditVariantModal = (variant, parentProduct) => {
  variantToEdit.value = { ...variant, parentProduct }
  let variantType = 'side_size';
  let variableOptionKey = '';
  if (variant.variable_option != null && variant.variable_option !== '' && variant.variable_option !== undefined) {
    variantType = 'variable_option';
    // Convert enum value to string key for dropdown
    variableOptionKey = typeof variant.variable_option === 'number'
      ? String(variant.variable_option)
      : String(variant.variable_option);
  } else if (variant.side != null && variant.size != null) {
    variantType = 'side_size';
  } else if (variant.size != null) {
    variantType = 'size_only';
  } else if (variant.side != null) {
    variantType = 'side_only';
  }
  editVariantForm.value = {
    id: variant.id,
    sku: variant.sku || '',
    quantity: variant.quantity,
    side: variant.side,
    size: variant.size,
    onlyOneOption: variant.only_one_option || false,
    variableOption: variableOptionKey,
    variantType
  }
  showEditVariantModal.value = true
}

const handleSaveProduct = async () => {
  isSaving.value = true
  try {
    if (productToEdit.value) {
      // EDIT EXISTING PRODUCT
      const updatedProduct = { ...productToEdit.value, ...editForm.value }
      const response = await api.put(`/api/product/${productToEdit.value.id}`, updatedProduct)
      if (response.status === 200) {
        showSuccessToast("Product updated successfully")
        await getProducts()
        showEditModal.value = false
      } else {
        showErrorToast("Failed to update product")
      }
    } else {
      // ADD NEW PRODUCT (existing logic)
      try {
        const response = await api.post('/api/product', editForm.value)
        if (response.status === 200 || response.status === 201) {
          showSuccessToast("Product Added Successfully");
          const newId = response.data.id || Math.max(0, ...products.value.map(p => p.id)) + 1
          const categoryName = allCategories.value.find(c => c.id.toString() === editForm.value.categoryId)?.name || 'Unknown'

          const newProduct = {
            id: newId,
            ...editForm.value,
            image: editForm.value.logoUrl || "https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg",
            categoryName,
          }

          products.value.push(newProduct)
          showAddModal.value = false
        } else {
          showErrorToast("Something went wrong, please try again")
        }
      } catch (apiError) {
        console.error(apiError)
        showErrorToast("Failed to add product")
      }
    }
  } catch (error) {
    showErrorToast("Failed to save product")
    console.error(error)
  } finally {
    isSaving.value = false
  }
}

const handleSaveVariant = async () => {
  isSaving.value = true
  try {
    // Prepare payload
    let variableOption = editVariantForm.value.variableOption
    // Convert variableOption to number if it's a string key
    let variableOptionPayload = null;
    if (editVariantForm.value.variantType === 'variable_option' && variableOption !== '' && variableOption !== null && variableOption !== undefined) {
      if (typeof variableOption === 'string' && variableOptionKeyToEnum.hasOwnProperty(variableOption)) {
        variableOptionPayload = variableOptionKeyToEnum[variableOption];
      }
    }
    const payload = {
      id: editVariantForm.value.id,
      sku: editVariantForm.value.sku,
      quantity: editVariantForm.value.quantity,
      side: editVariantForm.value.side,
      size: editVariantForm.value.size,
      variable_option: variableOptionPayload,
      only_one_option: editVariantForm.value.onlyOneOption,
      product_id: variantToEdit.value.parentProduct.id,
      parent_product_id: variantToEdit.value.parentProduct.id,
      range: 7 // default
    }
    const response = await api.put(`/api/product/variant/${payload.id}`, payload)
    if (response.status === 200) {
      // Update local product variant
      const productIndex = products.value.findIndex(p => p.id === variantToEdit.value.parentProduct.id)
      if (productIndex !== -1) {
        const variantIndex = products.value[productIndex].variant_products.findIndex(v => v.id === payload.id)
        if (variantIndex !== -1) {
          products.value[productIndex].variant_products[variantIndex] = {
            ...products.value[productIndex].variant_products[variantIndex],
            ...payload,
            displayName: getVariantDisplayName(payload)
          }
        }
      }
      showEditVariantModal.value = false
      showSuccessToast('Variant updated successfully')
    } else {
      showErrorToast('Failed to update variant')
    }
  } catch (error) {
    showErrorToast('Failed to update variant')
    console.error(error)
  } finally {
    isSaving.value = false
  }
}

const getStockStatusClass = (quantity) => {
  if (quantity <= 0) return 'bg-red-100 text-red-800'
  if (quantity < 10) return 'bg-yellow-100 text-yellow-800'
  return 'bg-green-100 text-green-800'
}

const getTotalVariantStock = (product) => {
  if (!product.isVariant || !product.variant_products) return product.quantity
  return product.variant_products.reduce((total, variant) => total + variant.quantity, 0)
}

// New Export function
const exportProducts = () => {
  isExporting.value = true
  try {
    const productsToExport = filteredProducts.value
    const header = ['Name', 'SKU', 'Price', 'Category', 'Stock', 'Has Variants']
    let csvContent = header.join(',') + '\n'

    productsToExport.forEach(product => {
      const totalStock = getTotalVariantStock(product)
      const row = [
        `"${product.name.replace(/"/g, '""')}"`,
        `"${product.sku || ''}"`,
        product.price,
        `"${product.categoryName}"`,
        totalStock,
        product.isVariant ? 'Yes' : 'No'
      ]
      csvContent += row.join(',') + '\n'

      // Add variant details if present
      if (product.isVariant && product.variant_products) {
        product.variant_products.forEach(variant => {
          const variantRow = [
            `"  → ${variant.sideName} Size ${variant.size}"`,
            `"${variant.sku || ''}"`,
            '',
            '',
            variant.quantity,
            'Variant'
          ]
          csvContent += variantRow.join(',') + '\n'
        })
      }
    })

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.setAttribute('href', url)
    link.setAttribute('download', 'products_export.csv')
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)

    showSuccessToast("Products exported successfully")
  } catch (error) {
    console.error('Export error:', error)
    showErrorToast("Failed to export products")
  } finally {
    isExporting.value = false
  }
}

const addVariant = async () => {
  if (!productToEdit.value.variant_products) productToEdit.value.variant_products = []
  let newVariant: any = {
    sku: editVariantForm.value.sku,
    quantity: editVariantForm.value.quantity,
    product_id: productToEdit.value.id,
    parent_product_id: productToEdit.value.id,
    range: 7 // default value

  }
  if (editVariantForm.value.variantType === 'side_size') {
    newVariant.side = editVariantForm.value.side
    newVariant.size = editVariantForm.value.size
  } else if (editVariantForm.value.variantType === 'size_only') {
    newVariant.size = editVariantForm.value.size
    newVariant.only_one_option = true
  } else if (editVariantForm.value.variantType === 'variable_option') {
    newVariant.variable_option = editVariantForm.value.variableOption
    newVariant.only_one_option = true
  }
  try {
    const response = await api.post('/api/product/variant', newVariant)
    if (response.status === 200 || response.status === 201) {
      const createdVariant = response.data
      createdVariant.displayName = getVariantDisplayName(createdVariant)
      productToEdit.value.variant_products.push(createdVariant)
      showSuccessToast('Variant added successfully')
    } else {
      showErrorToast('Failed to add variant')
    }
  } catch (error) {
    showErrorToast('Failed to add variant')
    console.error(error)
  }
  editVariantForm.value = { id: null, sku: '', quantity: 0, side: 0, size: 0, onlyOneOption: false, variableOption: '', variantType: 'side_size' }
}

const getCategoryName = (categoryId) => {
  const cat = allCategories.value.find(c => c.id === categoryId)
  return cat ? cat.name : 'Unknown'
}

// Utility function to get variant display name
function getVariantDisplayName(variant) {
  // If variable_option is present, use its name
  if (variant.variable_option) {
    // Try to map to variableOptionNames, fallback to the value
    return variableOptionNames[variant.variable_option] || variant.variable_option;
  }
  // If both side and size are present (not null/undefined)
  if (variant.side != null && variant.size != null) {
    return `${productSideNames[variant.side] || 'Side ' + variant.side}-${variant.size}`;
  }
  // If only size is present
  if (variant.size != null) {
    return `${variant.size}`;
  }
  // If only side is present
  if (variant.side != null) {
    return productSideNames[variant.side] || `Side ${variant.side}`;
  }
  return 'Variant';
}

// Image upload state
const imageUploading = ref(false)
const imageUploadInput = ref<HTMLInputElement | null>(null)

const handleImageUploadClick = () => {
  imageUploadInput.value?.click()
}

const handleImageFileChange = async (event: Event) => {
  const files = (event.target as HTMLInputElement).files
  if (!files || files.length === 0) return
  const file = files[0]
  const formData = new FormData()
  formData.append('file', file)
  imageUploading.value = true
  try {
    const response = await api.post('/api/product/upload-image', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
    if (response.data && response.data.url) {
      editForm.value.logoUrl = response.data.url
      showSuccessToast('Image uploaded successfully!')
    } else {
      showErrorToast('Failed to upload image')
    }
  } catch (error) {
    showErrorToast('Failed to upload image')
  } finally {
    imageUploading.value = false
    if (imageUploadInput.value) imageUploadInput.value.value = ''
  }
}

onMounted(async () => {
  await getProducts()
  // Fetch categories for product form
  const res = await api.get('/api/category')
  allCategories.value = res.data
})
</script>

<template>
  <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-9xl mx-auto">
    <!-- Header -->
    <div class="sm:flex sm:justify-between sm:items-center mb-8">
      <!-- Left: Title -->
      <div class="mb-4 sm:mb-0">
        <h1 class="text-2xl md:text-3xl font-extrabold text-slate-800">Products</h1>
      </div>

      <!-- Right: Actions -->
      <div class="grid grid-flow-col sm:auto-cols-max justify-start sm:justify-end gap-2">
        <!-- Search -->
        <div class="relative">
          <input v-model="searchQuery" type="search" placeholder="Search products..."
            class="form-input pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:border-primary focus:ring-primary" />
          <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
            <svg class="w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
          </div>
        </div>

        <!-- Category Filter -->
        <select v-model="selectedCategory"
          class="form-select border border-slate-300 rounded-lg focus:border-primary focus:ring-primary py-2">
          <option value="all">All Categories</option>
          <option v-for="category in allCategories" :key="category.id" :value="category.id">
            {{ category.name }}
          </option>
        </select>

        <!-- Export button -->
        <button @click="exportProducts"
          class="btn bg-slate-700 hover:bg-slate-800 text-white rounded-lg px-4 py-2 inline-flex items-center space-x-2 shadow-sm transition duration-150 ease-in-out"
          :disabled="isExporting">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z">
            </path>
          </svg>
          <span v-if="isExporting">Exporting...</span>
          <span v-else>Export</span>
        </button>

        <!-- Add button -->
        <button @click="openAddModal"
          class="btn bg-primary hover:bg-primary-dark text-white rounded-lg px-4 py-2 inline-flex items-center space-x-2 shadow-sm transition duration-150 ease-in-out">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
          </svg>
          <span>Add Product</span>
        </button>
      </div>
    </div>

    <!-- Loading indicator -->
    <div v-if="isLoading" class="flex justify-center py-12">
      <svg class="animate-spin h-8 w-8 text-primary" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor"
          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
        </path>
      </svg>
    </div>

    <!-- Cards view (for small screens) -->
    <div v-if="!isLoading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 md:hidden">
      <div v-for="product in filteredProducts" :key="product.id"
        class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden hover:shadow-md transition-shadow duration-300">
        <div class="relative pb-[56.25%] overflow-hidden bg-slate-50">
          <img :src="product.image" :alt="product.name" class="absolute inset-0 w-full h-full object-cover"
            @error="(e) => (e.target as HTMLImageElement).src = 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg'" />
        </div>
        <div class="p-4">
          <h3 class="font-semibold text-slate-800 text-lg mb-1 truncate">{{ product.name }}</h3>
          <div class="flex justify-between items-center mb-2">
            <p class="text-slate-600">{{ getCategoryName(product.categoryId) }}</p>
            <p class="font-medium text-slate-900">EGP {{ product.price }}</p>
          </div>

          <!-- Variants indicator for cards -->
          <div v-if="product.isVariant && product.variant_products" class="mb-2">
            <div class="flex items-center justify-between">
              <span class="text-xs text-blue-600 font-medium">
                {{ product.variant_products.length }} variants
              </span>
              <button @click="toggleProductExpansion(product.id)"
                class="text-xs text-blue-600 hover:text-blue-800 underline">
                {{ expandedProducts.has(product.id) ? 'Hide' : 'Show' }} variants
              </button>
            </div>
          </div>

          <!-- Mobile variants list -->
          <div v-if="product.isVariant && product.variant_products && expandedProducts.has(product.id)"
            class="mb-3 space-y-2">
            <div v-for="variant in product.variant_products" :key="variant.id" class="bg-slate-50 p-2 rounded text-xs">
              <div class="flex justify-between items-center">
                <div>
                  <span class="font-medium">{{ variant.displayName }}</span>
                </div>
                <div class="flex items-center space-x-1">
                  <span :class="[
                    'px-1 py-0.5 rounded text-xs',
                    getStockStatusClass(variant.quantity)
                  ]">
                    {{ variant.quantity }}
                  </span>
                  <button @click="openEditVariantModal(variant, product)" class="text-primary hover:text-primary-dark">
                    <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z">
                      </path>
                    </svg>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <div class="flex justify-between items-center">
            <span :class="[
              'px-2 py-1 rounded-full text-xs font-medium',
              getStockStatusClass(getTotalVariantStock(product))
            ]">
              {{ getTotalVariantStock(product) > 0 ? `${getTotalVariantStock(product)} in stock` : 'Out of stock' }}
            </span>
            <div class="flex space-x-2">
              <button @click="openEditModal(product)"
                class="p-2 text-primary hover:text-primary-dark rounded-full hover:bg-primary/10 transition-colors">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                  xmlns="http://www.w3.org/2000/svg">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z">
                  </path>
                </svg>
              </button>
              <button @click="confirmDelete(product)"
                class="p-2 text-red-600 hover:text-red-800 rounded-full hover:bg-red-50 transition-colors">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                  xmlns="http://www.w3.org/2000/svg">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16">
                  </path>
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty state for cards -->
      <div v-if="filteredProducts.length === 0" class="col-span-full py-12 text-center">
        <div class="flex flex-col items-center">
          <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
            xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M4 7v10c0 2.21 3.582 4 8 4s8-1.79 8-4V7M4 7c0 2.21 3.582 4 8 4s8-1.79 8-4M4 7c0-2.21 3.582-4 8-4s8 1.79 8 4">
            </path>
          </svg>
          <p class="text-slate-500 font-medium">No products found</p>
          <p class="text-slate-400 text-sm mt-1">Try adjusting your search or filters</p>
        </div>
      </div>
    </div>

    <!-- Table (for medium-large screens) -->
    <div v-if="!isLoading" class="hidden md:block">
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-slate-200">
            <thead class="bg-slate-50">
              <tr>
                <th scope="col"
                  class="px-3 py-3 w-48 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Product
                </th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">SKU</th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Price</th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Category
                </th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Stock</th>
                <th scope="col"
                  class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-slate-200">
              <template v-for="product in filteredProducts" :key="product.id">
                <!-- Main Product Row -->
                <tr class="hover:bg-slate-50 transition-colors">
                  <td class="px-5 py-5 whitespace-nowrap">
                    <div class="flex items-center space-x-3">
                      <!-- Expand/Collapse button for products with variants -->
                      <button v-if="product.isVariant && product.variant_products?.length"
                        @click="toggleProductExpansion(product.id)"
                        class="text-slate-400 hover:text-slate-600 transition-colors">
                        <svg class="w-4 h-4 transform transition-transform duration-200"
                          :class="{ 'rotate-90': expandedProducts.has(product.id) }" fill="none" stroke="currentColor"
                          viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
                        </svg>
                      </button>
                      <div v-else class="w-4"></div> <!-- Spacer for alignment -->

                      <div class="h-14 w-14 flex-shrink-0">
                        <img :src="product.image" alt="Product Image"
                          class="h-14 w-full rounded-lg object-cover border border-slate-200"
                          @error="(e) => (e.target as HTMLImageElement).src = 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg'" />
                      </div>
                      <div class="min-w-0 max-w-[8rem]">
                        <div class="font-medium text-slate-800 break-words whitespace-normal">{{ product.name }}</div>
                        <div v-if="product.isVariant && product.variant_products?.length"
                          class="text-xs text-blue-600 mt-1">
                          {{ product.variant_products.length }} variants
                        </div>
                      </div>
                    </div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap font-medium">{{ product.sku }}</td>
                  <td class="px-6 py-4 whitespace-nowrap font-medium">EGP {{ product.price }}</td>
                  <td class="px-6 py-4 whitespace-nowrap text-slate-600">{{ getCategoryName(product.categoryId) }}</td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <span :class="[
                      'px-2 py-1 rounded-full text-xs font-medium',
                      getStockStatusClass(getTotalVariantStock(product))
                    ]">
                      {{ getTotalVariantStock(product) > 0 ? `${getTotalVariantStock(product)} total` : 'Out of stock'
                      }}
                    </span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="flex space-x-3">
                      <button @click="openEditModal(product)"
                        class="text-primary hover:text-primary-dark transition-colors">
                        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                          xmlns="http://www.w3.org/2000/svg">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z">
                          </path>
                        </svg>
                      </button>
                      <button @click="confirmDelete(product)" class="text-red-600 hover:text-red-800 transition-colors">
                        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                          xmlns="http://www.w3.org/2000/svg">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16">
                          </path>
                        </svg>
                      </button>
                    </div>
                  </td>
                </tr>

                <!-- Variants Subtable -->
                <tr v-if="product.isVariant && product.variant_products?.length && expandedProducts.has(product.id)"
                  class="bg-slate-25">
                  <td colspan="6" class="px-0 py-0">
                    <div class="bg-slate-25 border-t border-slate-100">
                      <table class="min-w-full">
                        <thead class="bg-slate-100">
                          <tr>
                            <th
                              class="px-12 py-2 text-left text-xs font-medium text-slate-600 uppercase tracking-wider">
                              Variant</th>
                            <th class="px-6 py-2 text-left text-xs font-medium text-slate-600 uppercase tracking-wider">
                              SKU</th>
                            <th class="px-6 py-2 text-left text-xs font-medium text-slate-600 uppercase tracking-wider">
                              Side</th>
                            <th class="px-6 py-2 text-left text-xs font-medium text-slate-600 uppercase tracking-wider">
                              Size</th>
                            <th class="px-6 py-2 text-left text-xs font-medium text-slate-600 uppercase tracking-wider">
                              Stock</th>
                            <th class="px-6 py-2 text-left text-xs font-medium text-slate-600 uppercase tracking-wider">
                              Actions</th>
                          </tr>
                        </thead>
                        <tbody class="divide-y divide-slate-100">
                          <tr v-for="variant in product.variant_products" :key="variant.id"
                            class="hover:bg-slate-50 transition-colors">
                            <td class="px-12 py-3 whitespace-nowrap">
                              <div class="text-sm text-slate-700">
                                {{ variant.displayName }}
                              </div>
                            </td>
                            <td class="px-6 py-3 whitespace-nowrap text-sm text-slate-600">{{ variant.sku }}</td>
                            <td class="px-6 py-3 whitespace-nowrap text-sm text-slate-600">{{ variant.side }}</td>
                            <td class="px-6 py-3 whitespace-nowrap text-sm text-slate-600">{{ variant.size }}</td>
                            <td class="px-6 py-3 whitespace-nowrap">
                              <span :class="[
                                'px-2 py-1 rounded-full text-xs font-medium',
                                getStockStatusClass(variant.quantity)
                              ]">
                                {{ variant.quantity }}
                              </span>
                            </td>
                            <td class="px-6 py-3 whitespace-nowrap">
                              <button @click="openEditVariantModal(variant, product)"
                                class="text-primary hover:text-primary-dark transition-colors">
                                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                                  xmlns="http://www.w3.org/2000/svg">
                                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                    d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z">
                                  </path>
                                </svg>
                              </button>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                  </td>
                </tr>
              </template>

              <!-- Empty state for table -->
              <tr v-if="filteredProducts.length === 0">
                <td colspan="6" class="px-6 py-12 text-center">
                  <div class="flex flex-col items-center">
                    <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                      xmlns="http://www.w3.org/2000/svg">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M4 7v10c0 2.21 3.582 4 8 4s8-1.79 8-4V7M4 7c0 2.21 3.582 4 8 4s8-1.79 8-4M4 7c0-2.21 3.582-4 8-4s8 1.79 8 4">
                      </path>
                    </svg>
                    <p class="text-slate-500 font-medium">No products found</p>
                    <p class="text-slate-400 text-sm mt-1">Try adjusting your search or filters</p>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>

  <!-- Edit Variant Modal -->
  <div v-if="showEditVariantModal"
    class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
    <div class="bg-white rounded-xl shadow-xl max-w-md w-full p-6" @click.stop
      style="max-height:90vh; overflow-y:auto;">
      <h3 class="text-lg font-bold text-slate-800 mb-6">Edit Variant</h3>

      <div class="space-y-4">
        <!-- Parent Product Info -->
        <div class="bg-slate-50 p-3 rounded-lg">
          <p class="text-sm text-slate-600">Parent Product:</p>
          <p class="font-medium text-slate-800">{{ variantToEdit?.parentProduct?.name }}</p>
        </div>

        <!-- SKU -->
        <div>
          <label for="variant-sku" class="block text-sm font-medium text-slate-700 mb-1">SKU</label>
          <input id="variant-sku" v-model="editVariantForm.sku" type="text"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
            placeholder="Enter variant SKU" />
        </div>

        <!-- Variable Option (conditionally shown) -->
        <div v-if="editVariantForm.variableOption !== null && editVariantForm.variableOption !== ''">
          <label class="block text-sm font-medium text-slate-700 mb-1">Variable Option</label>
          <select v-model="editVariantForm.variableOption"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary">
            <option disabled value="">Select option</option>
            <option v-for="(name, key) in variableOptionNames" :key="key" :value="key">{{ name }}</option>
          </select>
        </div>

        <!-- Size only if side is null and variableOption is null -->
        <div
          v-if="(editVariantForm.side === null || editVariantForm.side === undefined) && (editVariantForm.variableOption === null || editVariantForm.variableOption === '')">
          <label for="variant-size" class="block text-sm font-medium text-slate-700 mb-1">Size</label>
          <input id="variant-size" v-model.number="editVariantForm.size" type="number" min="0" step="1"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
            placeholder="0" />
        </div>

        <!-- Side and Size if both are not null -->
        <template
          v-if="editVariantForm.side !== null && editVariantForm.side !== undefined && editVariantForm.size !== null && editVariantForm.size !== undefined">
          <div>
            <label for="variant-side" class="block text-sm font-medium text-slate-700 mb-1">Side</label>
            <select id="variant-side" v-model.number="editVariantForm.side"
              class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary">
              <option v-for="(name, value) in productSideNames" :key="value" :value="value">
                {{ name }}
              </option>
            </select>
          </div>
          <div>
            <label for="variant-size" class="block text-sm font-medium text-slate-700 mb-1">Size</label>
            <input id="variant-size" v-model.number="editVariantForm.size" type="number" min="0" step="1"
              class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
              placeholder="0" />
          </div>
        </template>

        <!-- Quantity -->
        <div>
          <label for="variant-quantity" class="block text-sm font-medium text-slate-700 mb-1">Stock Quantity</label>
          <input id="variant-quantity" v-model.number="editVariantForm.quantity" type="number" min="0" step="1"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
            placeholder="0" />
        </div>
      </div>

      <div class="flex justify-end space-x-3 mt-6">
        <button @click="showEditVariantModal = false"
          class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
          Cancel
        </button>
        <button @click="handleSaveVariant"
          class="px-4 py-2 bg-primary text-white rounded-lg hover:bg-primary-dark transition-colors"
          :disabled="isSaving">
          <span v-if="isSaving">Saving...</span>
          <span v-else>Save Variant</span>
        </button>
      </div>
    </div>
  </div>

  <!-- Delete confirmation modal -->
  <div v-if="showDeleteModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
    <div class="bg-white rounded-xl shadow-xl max-w-md w-full p-6" @click.stop>
      <h3 class="text-lg font-bold text-slate-800 mb-4">Confirm Deletion</h3>
      <p class="text-slate-600 mb-6">
        Are you sure you want to delete <span class="font-medium text-slate-800">{{ productToDelete?.name }}</span>?
        This action cannot be undone.
      </p>
      <div class="flex justify-end space-x-3">
        <button @click="showDeleteModal = false"
          class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
          Cancel
        </button>
        <button @click="handleDelete"
          class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors">
          Delete
        </button>
      </div>
    </div>
  </div>

  <!-- Edit Product modal -->
  <div v-if="showEditModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
    <div class="bg-white rounded-xl shadow-xl max-w-lg w-full p-6" @click.stop
      style="max-height:90vh; overflow-y:auto;">
      <h3 class="text-lg font-bold text-slate-800 mb-6">Edit Product</h3>

      <div class="space-y-4">
        <!-- Product Name -->
        <div class="mb-3">
          <label for="product-name" class="block text-sm font-medium text-slate-700 mb-1">Product Name</label>
          <input id="product-name" v-model="editForm.name" type="text"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
            placeholder="Enter product name" />
        </div>
        <!-- Is Variant Checkbox -->
        <div class="mb-3">
          <label class="block text-sm font-medium text-slate-700 mb-1">Product Type</label>
          <label class="flex items-center gap-2">
            <input type="checkbox" v-model="editForm.isVariant" /> This product has variants
          </label>
        </div>
        <!-- Price -->
        <div class="mb-3">
          <label for="product-price" class="block text-sm font-medium text-slate-700 mb-1">Price (EGP)</label>
          <input id="product-price" v-model.number="editForm.price" type="number" min="0" step="0.01"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
            placeholder="0.00" />
        </div>
        <!-- Category -->
        <div class="mb-3">
          <label for="product-category" class="block text-sm font-medium text-slate-700 mb-1">Category</label>
          <select id="product-category" v-model="editForm.categoryId"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary">
            <option disabled value="">Select category</option>
            <option v-for="category in allCategories" :key="category.id" :value="category.id">
              {{ category.name }}
            </option>
          </select>
        </div>
        <!-- SKU -->
        <div class="mb-3">
          <label for="sku" class="block text-sm font-medium text-slate-700 mb-1">SKU</label>
          <input id="sku" v-model="editForm.sku" type="text"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
            placeholder="Enter product SKU" />
        </div>
        <!-- Quantity -->
        <div class="mb-3">
          <label for="product-quantity" class="block text-sm font-medium text-slate-700 mb-1">Stock Quantity</label>
          <input id="product-quantity" v-model.number="editForm.quantity" type="number" min="0" step="1"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
            placeholder="0" />
        </div>
        <!-- logo Url -->
        <div class="mb-3">
          <label for="product-logoUrl" class="block text-sm font-medium text-slate-700 mb-1">Image URL</label>
          <div class="flex gap-2">
            <input id="product-logoUrl" v-model="editForm.logoUrl" type="text"
              class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
              placeholder="Enter image URL" />
            <button type="button" @click="handleImageUploadClick" :disabled="imageUploading"
              class="px-3 py-2 bg-cyan-600 text-white rounded-lg hover:bg-cyan-700 transition disabled:opacity-50">
              <span v-if="imageUploading">Uploading...</span>
              <span v-else>Upload Image</span>
            </button>
            <input ref="imageUploadInput" type="file" accept="image/*" class="hidden" @change="handleImageFileChange" />
          </div>
          <div v-if="editForm.logoUrl" class="mt-2">
            <p class="text-xs text-slate-500 mb-1">Image Preview:</p>
            <img :src="editForm.logoUrl" alt="Logo Preview"
              class="h-16 w-16 rounded-md object-cover border border-slate-200"
              @error="(e) => (e.target as HTMLImageElement).src = 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg'" />
          </div>
        </div>
        <!-- Variant Management Section -->
        <div v-if="editForm.isVariant" class="mb-4">
          <h4 class="font-semibold mb-2 mt-4">Variants</h4>
          <div v-for="variant in productToEdit?.variant_products || []" :key="variant.id"
            class="flex items-center gap-2 mb-2">
            <span>{{ variant.displayName }}</span>
            <input v-model.number="variant.quantity" type="number" min="0" class="w-20 border rounded px-2 py-1" />
            <button @click="openEditVariantModal(variant, productToEdit)" class="btn btn-xs">Edit</button>
          </div>
          <!-- Add Variant Form -->
          <div class="flex flex-col gap-2 mt-2">
            <label class="block text-xs font-medium text-slate-600 mb-2">Add New Variant</label>
            <div class="mb-2">
              <label class="block text-xs font-medium text-slate-600 mb-1">Variant Type <span
                  class="text-red-500">*</span></label>
              <select v-model="editVariantForm.variantType" class="border rounded px-2 py-1 w-full">
                <option disabled value="">Select variant type</option>
                <option value="side_size">Side + Size</option>
                <option value="size_only">Size Only</option>
                <option value="variable_option">Variable Option</option>
              </select>
            </div>
            <div v-if="editVariantForm.variantType" class="grid grid-cols-4 gap-2 mb-2">
              <template v-if="editVariantForm.variantType === 'side_size'">
                <div class="flex flex-col">
                  <label class="block text-xs font-medium text-slate-600 mb-1">Side</label>
                  <select v-model="editVariantForm.side" class="border rounded px-2 py-1">
                    <option v-for="(name, value) in productSideNames" :key="value" :value="value">{{ name }}</option>
                  </select>
                </div>
                <div class="flex flex-col">
                  <label class="block text-xs font-medium text-slate-600 mb-1">Size</label>
                  <input v-model.number="editVariantForm.size" type="number" min="0" placeholder="Size"
                    class="w-full border rounded px-2 py-1" />
                </div>
              </template>
              <template v-else-if="editVariantForm.variantType === 'size_only'">
                <div class="flex flex-col col-span-2">
                  <label class="block text-xs font-medium text-slate-600 mb-1">Size</label>
                  <input v-model.number="editVariantForm.size" type="number" min="0" placeholder="Size"
                    class="w-full border rounded px-2 py-1" />
                </div>
                <div></div>
                <div></div>
              </template>
              <template v-else-if="editVariantForm.variantType === 'side_only'">
                <div class="flex flex-col col-span-2">
                  <label class="block text-xs font-medium text-slate-600 mb-1">Side</label>
                  <select v-model="editVariantForm.side" class="border rounded px-2 py-1">
                    <option v-for="(name, value) in productSideNames" :key="value" :value="value">{{ name }}</option>
                  </select>
                </div>
                <div></div>
                <div></div>
              </template>
              <template v-else-if="editVariantForm.variantType === 'variable_option'">
                <div class="flex flex-col col-span-2">
                  <label class="block text-xs font-medium text-slate-600 mb-1">Variable Option</label>
                  <select v-model="editVariantForm.variableOption" class="border rounded px-2 py-1">
                    <option disabled value="">Select option</option>
                    <option v-for="(name, key) in variableOptionNames" :key="key" :value="key">{{ name }}</option>
                  </select>
                </div>
                <div></div>
                <div></div>
              </template>
              <div class="flex flex-col">
                <label class="block text-xs font-medium text-slate-600 mb-1">SKU</label>
                <input v-model="editVariantForm.sku" type="text" placeholder="SKU"
                  class="w-full border rounded px-2 py-1" />
              </div>
              <div class="flex flex-col">
                <label class="block text-xs font-medium text-slate-600 mb-1">Qty</label>
                <input v-model.number="editVariantForm.quantity" type="number" min="0" placeholder="Qty"
                  class="w-full border rounded px-2 py-1" />
              </div>
            </div>
            <button @click="addVariant" class="btn btn-primary btn-xs self-end"
              :disabled="!editVariantForm.variantType">Add Variant</button>
          </div>
        </div>
      </div>

      <div class="flex justify-end space-x-3 mt-6">
        <button @click="showEditModal = false"
          class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
          Cancel
        </button>
        <button @click="handleSaveProduct"
          class="px-4 py-2 bg-primary text-white rounded-lg hover:bg-primary-dark transition-colors"
          :disabled="isSaving">
          <span v-if="isSaving">Saving...</span>
          <span v-else>Save Changes</span>
        </button>
      </div>
    </div>
  </div>

  <!-- Add Product modal -->
  <div v-if="showAddModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
    <div class="bg-white rounded-xl shadow-xl max-w-lg w-full p-6" @click.stop
      style="max-height:90vh; overflow-y:auto;">
      <h3 class="text-lg font-bold text-slate-800 mb-6">Add New Product</h3>

      <div class="space-y-4">
        <!-- Product Name -->
        <div>
          <label for="new-product-name" class="block text-sm font-medium text-slate-700 mb-1">Product Name</label>
          <input id="new-product-name" v-model="editForm.name" type="text"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
            placeholder="Enter product name" />
        </div>

        <!-- Is Variant Checkbox -->
        <div>
          <label class="flex items-center gap-2">
            <input type="checkbox" v-model="editForm.isVariant" /> This product has variants
          </label>
        </div>

        <!-- Price -->
        <div>
          <label for="new-product-price" class="block text-sm font-medium text-slate-700 mb-1">Price (EGP)</label>
          <input id="new-product-price" v-model.number="editForm.price" type="number" min="0" step="0.01"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
            placeholder="0.00" />
        </div>

        <!-- SKU -->
        <div>
          <label for="new-product-sku" class="block text-sm font-medium text-slate-700 mb-1">SKU</label>
          <input id="new-product-sku" v-model="editForm.sku" type="text"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
            placeholder="Enter product SKU" />
        </div>

        <!-- Category -->
        <div>
          <label for="new-product-category" class="block text-sm font-medium text-slate-700 mb-1">Category</label>
          <select id="new-product-category" v-model="editForm.categoryId"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary">
            <option disabled value="">Select category</option>
            <option v-for="category in allCategories" :key="category.id" :value="category.id">
              {{ category.name }}
            </option>
          </select>
        </div>

        <!-- Quantity -->
        <div>
          <label for="new-product-quantity" class="block text-sm font-medium text-slate-700 mb-1">Stock Quantity</label>
          <input id="new-product-quantity" v-model.number="editForm.quantity" type="number" min="0" step="1"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
            placeholder="0" />
        </div>

        <!-- Logo URL -->
        <div>
          <label for="new-product-logoUrl" class="block text-sm font-medium text-slate-700 mb-1">Image URL</label>
          <div class="flex gap-2">
            <input id="new-product-logoUrl" v-model="editForm.logoUrl" type="text"
              class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary"
              placeholder="Enter image URL" />
            <button type="button" @click="handleImageUploadClick" :disabled="imageUploading"
              class="px-3 py-2 bg-cyan-600 text-white rounded-lg hover:bg-cyan-700 transition disabled:opacity-50">
              <span v-if="imageUploading">Uploading...</span>
              <span v-else>Upload Image</span>
            </button>
            <input ref="imageUploadInput" type="file" accept="image/*" class="hidden" @change="handleImageFileChange" />
          </div>
          <!-- Preview of the image -->
          <div v-if="editForm.logoUrl" class="mt-2">
            <p class="text-xs text-slate-500 mb-1">Image Preview:</p>
            <img :src="editForm.logoUrl" alt="Logo Preview"
              class="h-16 w-16 rounded-md object-cover border border-slate-200"
              @error="(e) => (e.target as HTMLImageElement).src = 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg'" />
          </div>
        </div>
      </div>

      <div class="flex justify-end space-x-3 mt-6">
        <button @click="showAddModal = false"
          class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
          Cancel
        </button>
        <button @click="handleSaveProduct"
          class="px-4 py-2 bg-primary text-white rounded-lg hover:bg-primary-dark transition-colors"
          :disabled="isSaving">
          <span v-if="isSaving">Adding...</span>
          <span v-else>Add Product</span>
        </button>
      </div>
    </div>
  </div>
</template>
