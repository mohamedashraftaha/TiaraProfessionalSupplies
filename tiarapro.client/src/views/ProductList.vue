<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import ProductCard from '../components/ProductCard.vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../utils/Api.ts'
import { showErrorToast, showSuccessToast } from '../utils/helpers.ts'
import { categoriesConst } from '../utils/helpers.ts'
import SubcategoryCard from '../components/SubcategoryCard.vue'

const route = useRoute()
const router = useRouter()
const loadingProducts = ref(false)
const loadingSubcategories = ref(false)

const isMobileSidebarOpen = ref(false)

const searchQuery = ref('')
const selectedCategory = ref(0)
const sortOption = ref('default')
const products = ref([])

const currentPage = ref(1)
const itemsPerPage = ref(10)
const totalPages = computed(() => Math.ceil(filteredProducts.value.length / itemsPerPage.value))

const categories = ref(categoriesConst)

const subcategories = ref([])
const subcategoriesByCategoryId = ref({})

const isLoading = computed(() => loadingProducts.value || loadingSubcategories.value)

const filteredProducts = computed(() => {
  let result = products.value

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(product =>
      product.name.toLowerCase().includes(query) ||
      (product.categoryName && product.categoryName.toLowerCase().includes(query))
    )
  }

  // Filter by category
  if (selectedCategory.value !== 0) {
    result = result.filter(product => product.categoryId == selectedCategory.value)
  }

  // Sort products
  if (sortOption.value === 'price-asc') {
    result = [...result].sort((a, b) => a.price - b.price)
  } else if (sortOption.value === 'price-desc') {
    result = [...result].sort((a, b) => b.price - a.price)
  } else if (sortOption.value === 'name-asc') {
    result = [...result].sort((a, b) => a.name.localeCompare(b.name))
  } else if (sortOption.value === 'name-desc') {
    result = [...result].sort((a, b) => b.name.localeCompare(a.name))
  }

  return result
})

// Paginated products - only show items for the current page
const paginatedProducts = computed(() => {
  const startIndex = (currentPage.value - 1) * itemsPerPage.value
  const endIndex = startIndex + itemsPerPage.value
  return filteredProducts.value.slice(startIndex, endIndex)
})


// Close mobile sidebar when category is selected
const selectCategory = (categoryId: number, isSubcategory = false) => {
  selectedCategory.value = categoryId
  isMobileSidebarOpen.value = false
  if (isSubcategory) {
    // If a subcategory is selected, clear subcategories in the main view
    subcategories.value = []
  } else {
    fetchSubcategoriesWithProducts(categoryId)
  }
}

const fetchSubcategoriesWithProducts = async (categoryId: number) => {
  loadingSubcategories.value = true;
  try {
    const response = await api.get(`/api/category/${categoryId}/subcategories`)
    if (response.status === 200) {
      subcategories.value = response.data.filter((subcategory: any) => subcategory.is_active === true)
    } else {
      subcategories.value = []
    }
  } catch (error) {
    subcategories.value = []
  } finally {
    loadingSubcategories.value = false;
  }
}

const getProductsOfSelectedCategory = async () => {
  loadingProducts.value = true;
  try {
    const response = await api.get(`/api/category/${selectedCategory.value}/products`)
    products.value = response.data
      .filter((product: any) => product.isActive === true)
      .map((product: any) => ({
        id: product.id,
        price: product.price,
        name: product.name,
        image: product.logoUrl != undefined || product.logoUrl != null || product.logoUrl != '' ? product.logoUrl : "https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg",
        categoryId: product.categoryId,
        categoryName: categories.value.find(category => category.id === product.categoryId)?.name, // Add category name
        quantity: product.quantity,
        isVariant: product.isVariant,
        isActive: product.isActive
      }))
  }
  catch (error) {
    console.error('Error in getting products for selected category', error)
  } finally {
    loadingProducts.value = false;
  }
}

const getAllProducts = async () => {
  loadingProducts.value = true;
  try {
    const allProducts = []
    const response = await api.get('/api/product')
    if (response.status != 200) {
      console.error("Error in getting products", response)
      showErrorToast("Error in getting products")
      return
    }
    allProducts.push(...response.data)
    products.value = allProducts
      .filter((product: any) => product.isActive === true)
      .map((product: any) => ({
        id: product.id,
        price: product.price,
        name: product.name,
        image: product.logoUrl != undefined || product.logoUrl != null ? product.logoUrl : "https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/tooth-circle-svgrepo-com.svg",
        categoryId: product.categoryId,
        categoryName: categories.value.find(category => category.id === product.categoryId)?.name, // Add category name
        quantity: product.quantity,
        isVariant: product.isVariant,
        isActive: product.isActive
      }))
    // Set the products.value with all the gathered products
    console.log("Products", products.value)
  } catch (error) {
    console.error("Error in getting products", error)
  } finally {
    loadingProducts.value = false;
  }
}

// Pagination control methods
const goToPage = (page: number) => {
  if (page > 0 && page <= totalPages.value) {
    currentPage.value = page
    router.push({ query: { ...route.query, page: page.toString() } })
  }
}

const goToNextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

const goToPrevPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

// Get visible page numbers for pagination
const getPageNumbers = computed(() => {
  const range = 2 // Show 2 pages before and after current page
  let start = Math.max(1, currentPage.value - range)
  let end = Math.min(totalPages.value, currentPage.value + range)

  // Always show at least 5 page numbers if possible
  if (end - start + 1 < 5) {
    if (start === 1) {
      end = Math.min(5, totalPages.value)
    } else if (end === totalPages.value) {
      start = Math.max(1, totalPages.value - 4)
    }
  }

  const pages = []
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  return pages
})

// Watch for changes to selectedCategory and update the query
watch(selectedCategory, (newCategory) => {
  debugger;
  const newQuery: Record<string, string> = {
    ...route.query,
    category: newCategory.toString()
  };
  // When changing category, reset page to 1
  if (route.query.page && currentPage.value !== 1) {
    newQuery.page = '1';
    //currentPage.value = 1;
  }
  router.replace({ query: newQuery });
  if (newCategory !== 0) {
    getProductsOfSelectedCategory();

    if (selectedCategory.value == 1 || selectedCategory.value == 2 || selectedCategory.value == 3) {
      fetchSubcategoriesWithProducts(selectedCategory.value)
    }
    else {
      subcategories.value = []
    }
  } else {
    subcategories.value = []
    getAllProducts();
  }
});

// Watch for changes to currentPage and update the query
watch(currentPage, (page) => {
  const newQuery = { ...route.query, page: page.toString() };
  router.replace({ query: newQuery });
});

// Watch for changes to searchQuery, sortOption, itemsPerPage and update the query
watch([searchQuery, sortOption, itemsPerPage], ([newSearch, newSort, newPerPage]) => {
  const newQuery = {
    ...route.query,
    search: newSearch,
    sort: newSort,
    perPage: newPerPage.toString(),
    page: '1', // Reset to first page on filter/sort change
  };
  currentPage.value = 1;
  router.replace({ query: newQuery });
});

// Restore state from route query on mount and when route changes
function restoreStateFromQuery(query) {
  if (query.category !== undefined) selectedCategory.value = parseInt(query.category as string) || 0;
  if (query.page !== undefined) currentPage.value = parseInt(query.page as string) || 1;
  if (query.search !== undefined) searchQuery.value = query.search as string;
  if (query.sort !== undefined) sortOption.value = query.sort as string;
  if (query.perPage !== undefined) itemsPerPage.value = parseInt(query.perPage as string) || 10;
}

watch(
  () => route.query,
  (query) => {
    restoreStateFromQuery(query);
  },
  { immediate: true }
);

// Fetch all subcategories for all categories on mount
const fetchAllSubcategories = async () => {
  const result = {}
  for (const category of categories.value) {
    if (category.id === 0) continue // skip 'All Categories'
    try {
      const response = await api.get(`/api/category/${category.id}/subcategories`)
      if (response.status === 200 && Array.isArray(response.data)) {
        result[category.id] = response.data.filter((subcategory: any) => subcategory.is_active === true)
      } else {
        result[category.id] = []
      }
    } catch (e) {
      result[category.id] = []
    }
  }
  subcategoriesByCategoryId.value = result
}

onMounted(async () => {
  restoreStateFromQuery(route.query);
  fetchAllSubcategories();
  loadingProducts.value = true;
  try {
    if (selectedCategory.value !== 0) {
      await getProductsOfSelectedCategory();
      if (selectedCategory.value == 1 || selectedCategory.value == 2 || selectedCategory.value == 3) {
        fetchSubcategoriesWithProducts(selectedCategory.value)
      } else {
        subcategories.value = []
      }
    } else {
      await getAllProducts();
    }
  }
  catch (error) {
    console.error("Error in getting products", error)
  }
  finally {
    loadingProducts.value = false;
  }
});
</script>

<template>
  <div class="min-h-screen bg-white">
    <main class="container mx-auto px-4 py-4 sm:py-8">
      <!-- Breadcrumb -->
      <div class="flex items-center text-sm mb-4 sm:mb-8">
        <a href="/" class="text-gray-600 hover:text-blue-600">Home</a>
        <span class="mx-2">/</span>
        <span class="text-gray-900">Products</span>
      </div>

      <!-- Mobile Filter Button -->
      <div class="lg:hidden mb-4">
        <button @click="isMobileSidebarOpen = !isMobileSidebarOpen"
          class="w-full flex items-center justify-center px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors touch-manipulation">
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.207A1 1 0 013 6.5V4z" />
          </svg>
          Filter & Categories
        </button>
      </div>

      <div class="flex gap-4 lg:gap-8">
        <!-- Desktop Sidebar -->
        <div class="hidden lg:block w-64 flex-shrink-0">
          <div class="bg-white rounded-lg shadow-sm p-6 sticky top-4">
            <h2 class="font-bold text-lg mb-4">Categories</h2>
            <ul class="space-y-2">
              <li v-for="category in categories" :key="category.id">
                <button @click="selectCategory(category.id)"
                  class="w-full text-left py-2 px-3 rounded hover:text-blue-600 hover:bg-blue-50 transition-colors font-medium flex items-center gap-2"
                  :class="selectedCategory === category.id ? 'text-blue-600 font-semibold bg-blue-50' : 'text-gray-700'">
                  <span>{{ category.name }}</span>
                </button>
                <transition-group name="fade" tag="ul">
              <li v-for="subcat in subcategoriesByCategoryId[category.id]" :key="subcat.id"
                v-if="subcategoriesByCategoryId[category.id] && subcategoriesByCategoryId[category.id].length > 0"
                class="ml-6 border-l-2 border-blue-100 pl-3 py-1">
                <button @click="selectCategory(subcat.id, true)"
                  class="w-full text-left py-1 px-2 rounded hover:text-blue-500 hover:bg-blue-100 transition-colors text-sm flex items-center gap-2"
                  :class="selectedCategory === subcat.id ? 'text-blue-600 font-semibold bg-blue-100' : 'text-gray-500'">
                  <span
                    style="width:7px;height:7px;background:#2563eb;border-radius:50%;display:inline-block;flex-shrink:0;margin-right:8px;"></span>
                  <span style="line-height:1.2;">{{ subcat.name }}</span>
                </button>
              </li>
              </transition-group>
              </li>
            </ul>
          </div>
        </div>

        <!-- Mobile Sidebar Overlay -->
        <div v-if="isMobileSidebarOpen" class="lg:hidden fixed inset-0 z-50 bg-black bg-opacity-50"
          @click="isMobileSidebarOpen = false">
          <div class="fixed inset-y-0 left-0 max-w-xs w-full bg-white shadow-xl" @click.stop>
            <div class="flex items-center justify-between p-4 border-b">
              <h2 class="font-bold text-lg">Categories</h2>
              <button @click="isMobileSidebarOpen = false"
                class="p-2 text-gray-400 hover:text-gray-600 touch-manipulation">
                <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
            <div class="p-4 overflow-y-auto">
              <ul class="space-y-1">
                <li v-for="category in categories" :key="category.id">
                  <button @click="selectCategory(category.id)"
                    class="w-full text-left py-3 px-3 rounded hover:text-blue-600 hover:bg-blue-50 transition-colors touch-manipulation"
                    :class="selectedCategory === category.id ? 'text-blue-600 font-semibold bg-blue-50' : 'text-gray-600'">
                    {{ category.name }}
                  </button>
                  <transition-group name="fade" tag="ul">
                <li v-for="subcat in subcategoriesByCategoryId[category.id]" :key="subcat.id"
                  v-if="subcategoriesByCategoryId[category.id] && subcategoriesByCategoryId[category.id].length > 0"
                  class="ml-6 border-l-2 border-blue-100 pl-3 py-1">
                  <button @click="selectCategory(subcat.id, true)"
                    class="w-full text-left py-1 px-2 rounded hover:text-blue-500 hover:bg-blue-100 transition-colors text-sm flex items-center gap-2"
                    :class="selectedCategory === subcat.id ? 'text-blue-600 font-semibold bg-blue-100' : 'text-gray-500'">
                    <span
                      style="width:7px;height:7px;background:#2563eb;border-radius:50%;display:inline-block;flex-shrink:0;margin-right:8px;"></span>
                    <span style="line-height:1.2;">{{ subcat.name }}</span>
                  </button>
                </li>
                </transition-group>
                </li>
              </ul>
            </div>
          </div>
        </div>

        <!-- Main Content -->
        <div class="flex-1 min-w-0">
          <!-- Search & Sort -->
          <div class="mb-4 sm:mb-6 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 sm:gap-4">
            <div class="sm:col-span-2 lg:col-span-1">
              <input v-model="searchQuery" type="text" placeholder="Search products..."
                class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-base focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent" />
            </div>

            <div>
              <select v-model="sortOption"
                class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-base focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white">
                <option value="default">Default</option>
                <option value="price-asc">Price: Low to High</option>
                <option value="price-desc">Price: High to Low</option>
                <option value="name-asc">Name: A to Z</option>
                <option value="name-desc">Name: Z to A</option>
              </select>
            </div>

            <div class="sm:col-span-2 lg:col-span-1">
              <select v-model="itemsPerPage"
                class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-base focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white">
                <option :value="8">8 per page</option>
                <option :value="10">10 per page</option>
                <option :value="12">12 per page</option>
                <option :value="24">24 per page</option>
                <option :value="48">48 per page</option>
              </select>
            </div>
          </div>

          <!-- Combined Subcategory and Product Grid -->
          <div v-if="!isLoading && (subcategories.length > 0 || paginatedProducts.length > 0)"
            class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 mb-8">
            <SubcategoryCard v-for="subcategory in subcategories" :key="'subcat-' + subcategory.id"
              :subcategory="subcategory" />
            <ProductCard v-for="product in paginatedProducts" :key="'prod-' + product.id" :product="product"
              :selected-category="selectedCategory" :current-page="currentPage" :search-query="searchQuery"
              :sort-option="sortOption" :items-per-page="itemsPerPage" />
          </div>

          <!-- Product Grid -->
          <div v-if="isLoading" class="text-center py-12 sm:py-16">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 sm:h-16 sm:w-16 mx-auto text-gray-400 animate-spin"
              fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M12 3v3M12 18v3M4.22 4.22l2.12 2.12M16.66 16.66l2.12 2.12M3 12h3M18 12h3M4.22 19.78l2.12-2.12M16.66 7.34l2.12-2.12" />
            </svg>
            <p class="text-gray-500 mt-2 text-sm sm:text-base">Loading products & subcategories...</p>
          </div>

          <div v-else-if="!isLoading && subcategories.length === 0 && paginatedProducts.length === 0"
            class="text-center py-12 sm:py-16">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 sm:h-16 sm:w-16 mx-auto text-gray-400 mb-4"
              fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <h3 class="text-lg font-medium text-gray-900 mb-2">No products found</h3>
            <p class="text-gray-500 text-sm sm:text-base">Try adjusting your search or filter criteria.</p>
          </div>

          <!-- Mobile-Optimized Pagination Controls -->
          <div v-if="totalPages > 1 && !isLoading" class="mt-6 sm:mt-8">
            <!-- Mobile Pagination (Simplified) -->
            <div class="flex sm:hidden items-center justify-between">
              <button @click="goToPrevPage" :disabled="currentPage === 1" :class="[
                'flex items-center px-4 py-2 text-sm font-medium rounded-lg transition-colors touch-manipulation',
                currentPage === 1
                  ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                  : 'bg-white text-gray-700 hover:bg-gray-50 border border-gray-300'
              ]">
                <svg class="h-4 w-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd"
                    d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z"
                    clip-rule="evenodd" />
                </svg>
                Previous
              </button>

              <span class="text-sm text-gray-700 px-2">
                {{ currentPage }} of {{ totalPages }}
              </span>

              <button @click="goToNextPage" :disabled="currentPage === totalPages" :class="[
                'flex items-center px-4 py-2 text-sm font-medium rounded-lg transition-colors touch-manipulation',
                currentPage === totalPages
                  ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                  : 'bg-white text-gray-700 hover:bg-gray-50 border border-gray-300'
              ]">
                Next
                <svg class="h-4 w-4 ml-1" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd"
                    d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z"
                    clip-rule="evenodd" />
                </svg>
              </button>
            </div>

            <!-- Desktop Pagination (Full) -->
            <div class="hidden sm:flex justify-center">
              <nav class="inline-flex rounded-md shadow">
                <!-- Previous Page Button -->
                <button @click="goToPrevPage" :disabled="currentPage === 1" :class="[
                  'relative inline-flex items-center px-4 py-2 rounded-l-md border text-sm font-medium',
                  currentPage === 1
                    ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                    : 'bg-white text-gray-700 hover:bg-gray-50'
                ]">
                  <span class="sr-only">Previous</span>
                  <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor"
                    aria-hidden="true">
                    <path fill-rule="evenodd"
                      d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z"
                      clip-rule="evenodd" />
                  </svg>
                </button>

                <!-- First Page Button (if not in view) -->
                <button v-if="!getPageNumbers.includes(1)" @click="goToPage(1)"
                  class="relative inline-flex items-center px-4 py-2 border border-gray-300 bg-white text-sm font-medium text-gray-700 hover:bg-gray-50">
                  1
                </button>

                <!-- Ellipsis (if needed) -->
                <span v-if="getPageNumbers[0] > 2"
                  class="relative inline-flex items-center px-4 py-2 border border-gray-300 bg-white text-sm font-medium text-gray-700">
                  ...
                </span>

                <!-- Page Numbers -->
                <button v-for="page in getPageNumbers" :key="page" @click="goToPage(page)" :class="[
                  'relative inline-flex items-center px-4 py-2 border text-sm font-medium',
                  currentPage === page
                    ? 'z-10 bg-blue-50 border-blue-500 text-blue-600'
                    : 'bg-white border-gray-300 text-gray-700 hover:bg-gray-50'
                ]">
                  {{ page }}
                </button>

                <!-- Ellipsis (if needed) -->
                <span v-if="getPageNumbers[getPageNumbers.length - 1] < totalPages - 1"
                  class="relative inline-flex items-center px-4 py-2 border border-gray-300 bg-white text-sm font-medium text-gray-700">
                  ...
                </span>

                <!-- Last Page Button (if not in view) -->
                <button v-if="!getPageNumbers.includes(totalPages) && totalPages > 1" @click="goToPage(totalPages)"
                  class="relative inline-flex items-center px-4 py-2 border border-gray-300 bg-white text-sm font-medium text-gray-700 hover:bg-gray-50">
                  {{ totalPages }}
                </button>

                <!-- Next Page Button -->
                <button @click="goToNextPage" :disabled="currentPage === totalPages" :class="[
                  'relative inline-flex items-center px-4 py-2 rounded-r-md border text-sm font-medium',
                  currentPage === totalPages
                    ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                    : 'bg-white text-gray-700 hover:bg-gray-50'
                ]">
                  <span class="sr-only">Next</span>
                  <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor"
                    aria-hidden="true">
                    <path fill-rule="evenodd"
                      d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z"
                      clip-rule="evenodd" />
                  </svg>
                </button>
              </nav>
            </div>
          </div>

          <!-- Page info -->
          <div v-if="filteredProducts.length > 0 && !isLoading"
            class="mt-4 text-center text-xs sm:text-sm text-gray-500">
            Showing {{ (currentPage - 1) * itemsPerPage + 1 }} to {{ Math.min(currentPage * itemsPerPage,
              filteredProducts.length) }} of {{ filteredProducts.length }} products
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
/* Touch-friendly interactive elements */
.touch-manipulation {
  touch-action: manipulation;
  -webkit-tap-highlight-color: transparent;
}

/* Ensure proper spacing for mobile */
@media (max-width: 640px) {
  .xs\:grid-cols-2 {
    grid-template-columns: repeat(2, 1fr);
  }
}

/* Mobile-specific form improvements */
@media (max-width: 768px) {

  input,
  select {
    font-size: 16px;
    /* Prevent zoom on iOS */
  }
}

/* Smooth sidebar animation */
.fixed.inset-y-0 {
  transition: transform 0.3s ease-in-out;
}

/* Sticky positioning for desktop sidebar */
@media (min-width: 1024px) {
  .sticky {
    position: sticky;
  }
}
</style>
