<template>
  <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-7xl mx-auto">

    <!-- Header -->
    <div class="sm:flex sm:justify-between sm:items-center mb-8">
      <!-- Title -->
      <div class="mb-4 sm:mb-0">
        <h1 class="text-2xl md:text-3xl font-extrabold text-slate-800">Events</h1>
      </div>

      <!-- Actions -->
      <div class="grid grid-flow-col sm:auto-cols-max gap-2">
        <!-- Search -->
        <!--<div class="relative">
          <input v-model="searchQuery" type="search" placeholder="Search events..."
                 class="form-input pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:border-cyan-500 focus:ring-cyan-500" />
          <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
            <svg class="w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          </div>
        </div>-->
        <!-- Category Filter -->
        <!--<select v-model="categoryFilter"
                class="form-select border border-slate-300 rounded-lg focus:border-cyan-500 focus:ring-cyan-500 py-2">
          <option value="all">All Categories</option>
          <option v-for="cat in categoryOptions" :key="cat" :value="cat">{{ cat }}</option>
        </select>-->
        <!-- Add Event Button -->
        <button
          class="px-4 py-2 bg-cyan-600 text-white rounded-lg shadow hover:bg-cyan-700 transition flex items-center gap-2"
          @click="openAdd">
          <span class="font-bold text-lg">+</span>
          Add Event
        </button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <svg class="animate-spin h-8 w-8 text-cyan-500" xmlns="http://www.w3.org/2000/svg" fill="none"
        viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
      </svg>
    </div>

    <!-- Mobile: Cards -->
    <div v-if="!loading" class="grid grid-cols-1 gap-4 md:hidden">
      <div v-for="event in events" :key="event.id"
        class="bg-white rounded-xl shadow border border-slate-200 overflow-hidden">
        <div class="p-5">
          <div class="flex justify-between items-start mb-2">
            <div>
              <p class="text-sm text-slate-500">Event</p>
              <p class="font-bold text-slate-900">{{ event.title }}</p>
            </div>
            <span v-if="event.speakers" class="px-2 py-1 rounded-full text-xs font-medium bg-cyan-100 text-cyan-700">
              {{ event.speakers }}
            </span>
          </div>
          <div class="space-y-3 py-3 border-t border-b border-slate-100 my-3">
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Date:</span>
              <span class="font-medium text-slate-700">{{ formatDate(event.date) }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Location:</span>
              <span class="font-medium text-slate-700">{{ event.location }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-500">Capacity:</span>
              <span class="font-medium text-slate-700">{{ event.capacity ?? '—' }}</span>
            </div>
          </div>
          <div class="flex gap-2 mt-4">
            <button @click="openEdit(event)"
              class="flex-1 px-3 py-2 bg-cyan-100 text-cyan-800 hover:bg-cyan-200 rounded-lg transition text-sm font-medium">
              Edit
            </button>
            <button @click="confirmDeleteEvent(event)"
              class="px-3 py-2 text-red-600 hover:text-red-700 hover:bg-red-50 rounded-lg transition text-sm font-medium">
              Delete
            </button>
          </div>
        </div>
      </div>
      <div v-if="events.length === 0" class="py-12 text-center text-slate-500">
        <div class="flex flex-col items-center">
          <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3" />
            <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2" />
          </svg>
          No events found.
        </div>
      </div>
    </div>

    <!-- Desktop: Table -->
    <div v-if="!loading" class="hidden md:block">
      <div class="bg-white rounded-xl shadow border border-slate-200 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-slate-200">
            <thead class="bg-slate-50">
              <tr>
                <th @click="setSort('title')" class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider cursor-pointer">
                  Title <span v-if="sortColumn==='title'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
                </th>
                <th @click="setSort('date')" class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider cursor-pointer">
                  Date <span v-if="sortColumn==='date'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
                </th>
                <th @click="setSort('location')" class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider cursor-pointer">
                  Location <span v-if="sortColumn==='location'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
                </th>
                <th @click="setSort('speakers')" class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider cursor-pointer">
                  Speakers <span v-if="sortColumn==='speakers'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
                </th>
                <th @click="setSort('capacity')" class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider cursor-pointer">
                  Capacity <span v-if="sortColumn==='capacity'">{{ sortDirection==='asc' ? '▲' : '▼' }}</span>
                </th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-slate-200">
              <tr v-for="event in sortedEvents" :key="event.id" class="hover:bg-slate-50">
                <td class="px-6 py-4 font-bold text-slate-800">{{ event.title }}</td>
                <td class="px-6 py-4">{{ formatDate(event.date) }}</td>
                <td class="px-6 py-4">{{ event.location }}</td>
                <td class="px-6 py-4">{{ event.speakers }}</td>
                <td class="px-6 py-4">{{ event.capacity ?? '—' }}</td>
                <td class="px-6 py-4">
                  <div class="flex gap-2">
                    <button @click="openEdit(event)"
                      class="px-3 py-1 bg-cyan-100 text-cyan-800 rounded hover:bg-cyan-200 transition">Edit</button>
                    <button @click="confirmDeleteEvent(event)"
                      class="px-3 py-1 text-red-600 hover:bg-red-50 rounded transition">Delete</button>
                  </div>
                </td>
              </tr>
              <tr v-if="events.length === 0">
                <td colspan="6" class="px-6 py-12 text-center text-slate-500">
                  <div class="flex flex-col items-center">
                    <svg class="w-12 h-12 text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3" />
                      <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2" />
                    </svg>
                    No events found.
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Add/Edit Modal -->
    <div v-if="showForm" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div class="bg-white rounded-xl shadow-xl max-w-lg w-full p-6" @click.stop
        style="max-height:90vh; overflow-y:auto;">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-bold text-cyan-700">{{ editMode ? 'Edit Event' : 'Add Event' }}</h3>
          <button @click="closeForm" class="text-slate-400 hover:text-slate-500">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        <form @submit.prevent="submitForm" class="space-y-4">
          <div v-if="formError" class="text-red-500 mb-2">{{ formError }}</div>
          <div>
            <label class="block font-semibold mb-1">Title</label>
            <input v-model="form.title"
              class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none"
              required />
          </div>
          <div>
            <label class="block font-semibold mb-1">Description</label>
            <textarea v-model="form.description"
              class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none"
              required></textarea>
          </div>
          <div>
            <label class="block font-semibold mb-1">Speakers</label>
            <input v-model="form.speakers"
              class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none" />
          </div>
          <div>
            <label class="block font-semibold mb-1">Date & Time</label>
            <input type="datetime-local" v-model="form.date"
              class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none"
              required />
          </div>
          <div>
            <label class="block font-semibold mb-1">Location</label>
            <input v-model="form.location"
              class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none" />
          </div>
          <div>
            <label class="block font-semibold mb-1">Image URL</label>
            <div class="flex gap-2">
              <input v-model="form.image_url"
                class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none" />
              <button type="button" @click="handleImageUploadClick" :disabled="imageUploading"
                class="px-3 py-2 bg-cyan-600 text-white rounded-lg hover:bg-cyan-700 transition disabled:opacity-50">
                <span v-if="imageUploading">Uploading...</span>
                <span v-else>Upload Image</span>
              </button>
              <input ref="imageUploadInput" type="file" accept="image/*" class="hidden"
                @change="handleImageFileChange" />
            </div>
            <div v-if="form.image_url" class="mt-2">
              <p class="text-xs text-slate-500 mb-1">Image Preview:</p>
              <img :src="form.image_url" alt="Event Image Preview"
                class="h-16 w-16 rounded-md object-cover border border-slate-200"
                @error="(e) => (e.target as HTMLImageElement).src = 'https://placehold.co/600x400?text=Event'" />
            </div>
          </div>
          <div>
            <label class="block font-semibold mb-1">Capacity</label>
            <input type="number" v-model.number="form.capacity"
              class="w-full border border-cyan-300 p-2 rounded-lg focus:ring-2 focus:ring-cyan-400 outline-none"
              min="1" />
          </div>
          <div class="flex justify-end gap-2 mt-6">
            <button type="button"
              class="px-5 py-2 bg-gray-200 text-gray-800 rounded-lg hover:bg-gray-300 transition font-semibold"
              @click="closeForm">
              Cancel
            </button>
            <button type="submit"
              class="px-6 py-2 bg-cyan-600 text-white rounded-lg hover:bg-cyan-700 transition font-semibold shadow">
              {{ editMode ? 'Update' : 'Add' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Delete confirmation modal -->
    <div v-if="showDeleteModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full p-6" @click.stop>
        <h3 class="text-lg font-bold text-slate-800 mb-4">Confirm Deletion</h3>
        <p class="text-slate-600 mb-6">
          Are you sure you want to delete event
          <span class="font-medium text-slate-800">"{{ currentEvent?.title }}"</span>?
          This action cannot be undone.
        </p>
        <div class="flex justify-end space-x-3">
          <button @click="showDeleteModal = false"
            class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 transition-colors">
            Cancel
          </button>
          <button @click="deleteEvent"
            class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors">
            Delete
          </button>
        </div>
      </div>
    </div>
  </div>
</template>



<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import api from '../../utils/Api.ts'

interface EventDTO {
  id?: number;
  title: string;
  description: string;
  speakers?: string;
  date: string;
  location?: string;
  image_url?: string;
  capacity?: number;
  createdAt?: string;
  updatedAt?: string;
}

const events = ref<EventDTO[]>([]);
const showForm = ref(false);
const editMode = ref(false);
const loading = ref(true);
const error = ref<string | null>(null);
const formError = ref<string | null>(null);
const form = ref<EventDTO>({
  title: '',
  description: '',
  speakers: '',
  date: '',
  location: '',
  image_url: '',
  capacity: undefined,
});
const editingId = ref<number | null>(null);
const showDeleteModal = ref(false);
const currentEvent = ref<EventDTO | null>(null);

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
      form.value.image_url = response.data.url
    } else {
      alert('Failed to upload image')
    }
  } catch (error) {
    alert('Failed to upload image')
  } finally {
    imageUploading.value = false
    if (imageUploadInput.value) imageUploadInput.value.value = ''
  }
}

const fetchEvents = async () => {
  debugger;
  loading.value = true;
  error.value = null;
  try {
    const res = await api.get('/api/events');
    events.value = res.data;
  } catch (e) {
    error.value = 'Failed to load events.';
  } finally {
    loading.value = false;
  }
};

const openAdd = () => {
  editMode.value = false;
  formError.value = null;
  form.value = { title: '', description: '', speakers: '', date: '', location: '', image_url: '', capacity: undefined };
  showForm.value = true;
  editingId.value = null;
};

const openEdit = (event: EventDTO) => {
  editMode.value = true;
  formError.value = null;
  // Format date for datetime-local input
  let date = event.date;
  if (date && date.length > 0) {
    const d = new Date(date);
    form.value = { ...event, date: d.toISOString().slice(0, 16) };
  } else {
    form.value = { ...event };
  }
  showForm.value = true;
  editingId.value = event.id!;
};

const closeForm = () => {
  showForm.value = false;
  formError.value = null;
};

const submitForm = async () => {
  formError.value = null;
  // Client-side validation
  if (!form.value.title || !form.value.description) {
    formError.value = 'Title and Description are required.';
    return;
  }
  if (!form.value.date || isNaN(new Date(form.value.date).getTime())) {
    formError.value = 'A valid date and time are required.';
    return;
  }
  if (new Date(form.value.date) < new Date(Date.now() - 5 * 60 * 1000)) {
    formError.value = 'Date must be in the future.';
    return;
  }
  try {
    if (editMode.value && editingId.value) {
      await api.put(`/api/events/${editingId.value}`, form.value);
    } else {
      await api.post('/api/events', form.value);
    }
    showForm.value = false;
    await fetchEvents();
  } catch (e: any) {
    if (e.response && e.response.data) {
      formError.value = e.response.data;
    } else {
      formError.value = 'Failed to save event.';
    }
  }
};

const confirmDeleteEvent = (event: EventDTO) => {
  currentEvent.value = event;
  showDeleteModal.value = true;
};

const deleteEvent = async () => {
  if (!currentEvent.value) return;
  try {
    await api.delete(`/api/events/${currentEvent.value.id}`);
    showDeleteModal.value = false;
    await fetchEvents();
  } catch (e) {
    error.value = 'Failed to delete event.';
  }
};

const formatDate = (dateStr: string) => {
  const d = new Date(dateStr);
  return d.toLocaleString();
};

const sortColumn = ref('date')
const sortDirection = ref('desc')

const setSort = (column) => {
  if (sortColumn.value === column) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortColumn.value = column
    sortDirection.value = 'asc'
  }
}

const sortedEvents = computed(() => {
  const sorted = [...events.value]
  sorted.sort((a, b) => {
    let valA = a[sortColumn.value]
    let valB = b[sortColumn.value]
    if (valA === undefined || valA === null) valA = ''
    if (valB === undefined || valB === null) valB = ''
    if (sortColumn.value.toLowerCase().includes('date') || sortColumn.value.toLowerCase().includes('at')) {
      valA = new Date(valA)
      valB = new Date(valB)
    }
    if (valA < valB) return sortDirection.value === 'asc' ? -1 : 1
    if (valA > valB) return sortDirection.value === 'asc' ? 1 : -1
    return 0
  })
  return sorted
})

onMounted(async () => { await fetchEvents(); });
</script>
