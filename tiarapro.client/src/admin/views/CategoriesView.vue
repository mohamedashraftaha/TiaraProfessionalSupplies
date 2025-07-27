<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '../../utils/Api.ts'

const categories = ref([])
const loading = ref(false)
const showAddModal = ref(false)
const showEditModal = ref(false)
const showDeleteModal = ref(false)
const currentCategory = ref(null)
const form = ref({ name: '', parent_category_id: null, logo_url: '', is_active: true })
const deleteTarget = ref(null)

const sortColumn = ref('name')
const sortDirection = ref('asc')

const setSort = (column) => {
  if (sortColumn.value === column) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortColumn.value = column
    sortDirection.value = 'asc'
  }
}

const sortedCategories = computed(() => {
  const sorted = [...categories.value]
  sorted.sort((a, b) => {
    let valA, valB
    if (sortColumn.value === 'parent') {
      valA = parentName(a)
      valB = parentName(b)
    } else if (sortColumn.value === 'is_active') {
      valA = a.is_active ? 1 : 0
      valB = b.is_active ? 1 : 0
    } else {
      valA = a[sortColumn.value]
      valB = b[sortColumn.value]
    }
    if (valA === undefined || valA === null) valA = ''
    if (valB === undefined || valB === null) valB = ''
    if (valA < valB) return sortDirection.value === 'asc' ? -1 : 1
    if (valA > valB) return sortDirection.value === 'asc' ? 1 : -1
    return 0
  })
  return sorted
})

const fetchCategories = async () => {
  debugger;
  loading.value = true
  try {
    const res = await api.get('/api/category')
    categories.value = res.data
  } finally {
    loading.value = false
  }
}

const openAddModal = () => {
  form.value = { name: '', parent_category_id: null, logo_url: '', is_active: true }
  showAddModal.value = true
}
const openEditModal = (cat) => {
  currentCategory.value = cat
  form.value = { name: cat.name, parent_category_id: cat.parent_category_id, logo_url: cat.logo_url, is_active: cat.is_active }
  showEditModal.value = true
}
const openDeleteModal = (cat) => {
  deleteTarget.value = cat
  showDeleteModal.value = true
}
const addCategory = async () => {
  await api.post('/api/category', form.value)
  showAddModal.value = false
  fetchCategories()
}
const editCategory = async () => {
  await api.put(`/api/category/${currentCategory.value.id}`, { ...currentCategory.value, ...form.value })
  showEditModal.value = false
  fetchCategories()
}
const deleteCategory = async () => {
  await api.delete(`/api/category/${deleteTarget.value.id}`)
  showDeleteModal.value = false
  fetchCategories()
}
const toggleActive = async (cat) => {
  await api.put(`/api/category/${cat.id}`, { ...cat, is_active: !cat.is_active })
  fetchCategories()
}

onMounted(async () => {
  await fetchCategories()
})

const parentName = (cat) => {
  if (!cat.parent_category_id) return ''
  const parent = categories.value.find(c => c.id === cat.parent_category_id)
  return parent ? parent.name : ''
}
</script>

<template>
  <div>
    <div class="flex justify-between items-center mb-8">
      <h1 class="text-2xl font-bold">Categories</h1>
      <button class="btn" @click="openAddModal">Add Category</button>
    </div>
    <div v-if="loading" class="py-8 text-center text-gray-400">Loading...</div>
    <div v-else class="bg-white rounded-lg shadow overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th @click="setSort('name')" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer">
              Name <span v-if="sortColumn==='name'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
            </th>
            <th @click="setSort('parent')" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer">
              Parent <span v-if="sortColumn==='parent'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
            </th>
            <th @click="setSort('is_active')" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer">
              Active <span v-if="sortColumn==='is_active'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="cat in sortedCategories" :key="cat.id">
            <td class="px-6 py-4 whitespace-nowrap">{{ cat.name }}</td>
            <td class="px-6 py-4 whitespace-nowrap">{{ parentName(cat) }}</td>
            <td class="px-6 py-4 whitespace-nowrap">
              <button @click="toggleActive(cat)" :class="cat.is_active ? 'text-green-600' : 'text-gray-400'">
                <span v-if="cat.is_active">Active</span>
                <span v-else>Inactive</span>
              </button>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <button class="text-primary hover:text-primary/80 mr-2" @click="openEditModal(cat)">Edit</button>
              <button class="text-red-600 hover:text-red-800" @click="openDeleteModal(cat)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <!-- Add Modal -->
    <div v-if="showAddModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-30">
      <div class="bg-white rounded-lg p-6 w-full max-w-md">
        <h2 class="text-lg font-bold mb-4">Add Category</h2>
        <form @submit.prevent="addCategory" class="space-y-4">
          <input v-model="form.name" class="w-full border rounded px-3 py-2" placeholder="Name" required />
          <select v-model="form.parent_category_id" class="w-full border rounded px-3 py-2">
            <option :value="null">No Parent</option>
            <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
          </select>
          <input v-model="form.logo_url" class="w-full border rounded px-3 py-2" placeholder="Logo URL" />
          <div v-if="form.logo_url" class="mt-2">
            <p class="text-xs text-gray-500 mb-1">Logo Preview:</p>
            <img :src="form.logo_url" alt="Logo Preview"
              class="h-16 w-16 rounded-md object-cover border border-slate-200" />
          </div>
          <label class="flex items-center gap-2">
            <input type="checkbox" v-model="form.is_active" /> Active
          </label>
          <div class="flex justify-end gap-2">
            <button type="button" class="btn" @click="showAddModal = false">Cancel</button>
            <button type="submit" class="btn btn-primary">Add</button>
          </div>
        </form>
      </div>
    </div>
    <!-- Edit Modal -->
    <div v-if="showEditModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-30">
      <div class="bg-white rounded-lg p-6 w-full max-w-md">
        <h2 class="text-lg font-bold mb-4">Edit Category</h2>
        <form @submit.prevent="editCategory" class="space-y-4">
          <input v-model="form.name" class="w-full border rounded px-3 py-2" placeholder="Name" required />
          <select v-model="form.parent_category_id" class="w-full border rounded px-3 py-2">
            <option :value="null">No Parent</option>
            <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
          </select>
          <input v-model="form.logo_url" class="w-full border rounded px-3 py-2" placeholder="Logo URL" />
          <div v-if="form.logo_url" class="mt-2">
            <p class="text-xs text-gray-500 mb-1">Logo Preview:</p>
            <img :src="form.logo_url" alt="Logo Preview"
              class="h-16 w-16 rounded-md object-cover border border-slate-200" />
          </div>
          <label class="flex items-center gap-2">
            <input type="checkbox" v-model="form.is_active" /> Active
          </label>
          <div class="flex justify-end gap-2">
            <button type="button" class="btn" @click="showEditModal = false">Cancel</button>
            <button type="submit" class="btn btn-primary">Save</button>
          </div>
        </form>
      </div>
    </div>
    <!-- Delete Modal -->
    <div v-if="showDeleteModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-30">
      <div class="bg-white rounded-lg p-6 w-full max-w-md">
        <h2 class="text-lg font-bold mb-4">Delete Category</h2>
        <p>Are you sure you want to delete <b>{{ deleteTarget?.name }}</b>?</p>
        <div class="flex justify-end gap-2 mt-4">
          <button class="btn" @click="showDeleteModal = false">Cancel</button>
          <button class="btn btn-danger" @click="deleteCategory">Delete</button>
        </div>
      </div>
    </div>
  </div>
</template>