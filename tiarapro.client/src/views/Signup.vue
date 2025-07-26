<template>
  <div class="py-16 px-4 flex justify-center">
    <div class="bg-white p-8 rounded-2xl shadow-lg w-full max-w-md border border-slate-200">
      <h2 class="text-2xl font-bold mb-6 text-center text-slate-800">Create an Account</h2>

      <form @submit.prevent="handleSignup" class="space-y-4">
        <!-- Name Fields -->
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">First Name</label>
            <div class="relative">
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <svg class="h-5 w-5 text-slate-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                  fill="currentColor">
                  <path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z"
                    clip-rule="evenodd" />
                </svg>
              </div>
              <input v-model="firstName" type="text" required placeholder="John"
                class="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Middle Name</label>
            <input v-model="middleName" type="text" required placeholder="Doe"
              class="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
          </div>

          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Last Name</label>
            <input v-model="lastName" type="text" required placeholder="Doe"
              class="w-full px-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
          </div>
        </div>

        <!-- Email -->
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Email</label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg class="h-5 w-5 text-slate-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                fill="currentColor">
                <path d="M2.003 5.884L10 9.882l7.997-3.998A2 2 0 0016 4H4a2 2 0 00-1.997 1.884z" />
                <path d="M18 8.118l-8 4-8-4V14a2 2 0 002 2h12a2 2 0 002-2V8.118z" />
              </svg>
            </div>
            <input v-model="email" type="email" required placeholder="you@example.com"
              class="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
          </div>
        </div>

        <!-- Address -->
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Address</label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg class="h-5 w-5 text-slate-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                fill="currentColor">
                <path fill-rule="evenodd"
                  d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z"
                  clip-rule="evenodd" />
              </svg>
            </div>
            <input v-model="address" type="text" required placeholder="123 Main St"
              class="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
          </div>
        </div>

        <!-- Location Fields -->
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Governorate</label>
            <select v-model="state" required
              class="w-full px-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary">
              <option value="">Select Governorate</option>
              <option v-for="gov in governorates" :key="gov" :value="gov">{{ gov }}</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">City</label>
            <select v-model="city" required
              class="w-full px-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary">
              <option value="">Select City</option>
              <option v-for="city in cities" :key="city" :value="city">{{ city }}</option>
            </select>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Country</label>
            <input v-model="country" type="text" required value="Egypt" disabled
              class="w-full px-3 py-2 border border-slate-300 rounded-xl bg-slate-50" />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Phone Number</label>
            <div class="relative">
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <svg class="h-5 w-5 text-slate-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                  fill="currentColor">
                  <path
                    d="M2 3a1 1 0 011-1h2.153a1 1 0 01.986.836l.74 4.435a1 1 0 01-.54 1.06l-1.548.773a11.037 11.037 0 006.105 6.105l.774-1.548a1 1 0 011.059-.54l4.435.74a1 1 0 01.836.986V17a1 1 0 01-1 1h-2C7.82 18 2 12.18 2 5V3z" />
                </svg>
              </div>
              <input v-model="phone" type="text" required placeholder="01XXXXXXXXX" @input="validatePhone"
                class="w-full pl-10 pr-3 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
            </div>
            <p v-if="phoneError" class="mt-1 text-xs text-red-500">{{ phoneError }}</p>
          </div>
        </div>

        <!-- Password -->
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Password</label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg class="h-5 w-5 text-slate-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                fill="currentColor">
                <path fill-rule="evenodd"
                  d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z"
                  clip-rule="evenodd" />
              </svg>
            </div>
            <input v-model="password" :type="showPassword ? 'text' : 'password'" required placeholder="••••••••"
              class="w-full pl-10 pr-10 py-2 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary" />
            <div class="absolute inset-y-0 right-0 pr-3 flex items-center">
              <button type="button" @click="showPassword = !showPassword"
                class="text-slate-400 hover:text-slate-600 focus:outline-none">
                <svg v-if="showPassword" class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                  fill="currentColor">
                  <path d="M10 12a2 2 0 100-4 2 2 0 000 4z" />
                  <path fill-rule="evenodd"
                    d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z"
                    clip-rule="evenodd" />
                </svg>
                <svg v-else class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                  <path fill-rule="evenodd"
                    d="M3.707 2.293a1 1 0 00-1.414 1.414l14 14a1 1 0 001.414-1.414l-1.473-1.473A10.014 10.014 0 0019.542 10C18.268 5.943 14.478 3 10 3a9.958 9.958 0 00-4.512 1.074l-1.78-1.781zm4.261 4.26l1.514 1.515a2.003 2.003 0 012.45 2.45l1.514 1.514a4 4 0 00-5.478-5.478z"
                    clip-rule="evenodd" />
                  <path
                    d="M12.454 16.697L9.75 13.992a4 4 0 01-3.742-3.741L2.335 6.578A9.98 9.98 0 00.458 10c1.274 4.057 5.065 7 9.542 7 .847 0 1.669-.105 2.454-.303z" />
                </svg>
              </button>
            </div>
          </div>
          <p class="mt-1 text-xs text-slate-500">Password must be at least 8 characters</p>
        </div>

        <!-- Submit Button -->
        <button type="submit" :disabled="loading"
          class="w-full bg-primary text-white py-3 rounded-xl hover:bg-primary-dark transition flex justify-center items-center shadow-md">
          <span v-if="!loading">Create Account</span>
          <svg v-else class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none"
            viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z" />
          </svg>
        </button>
      </form>

      <!-- Sign-in Link -->
      <p class="text-center text-sm text-slate-500 mt-6">
        Already have an account?
        <RouterLink to="/login" class="text-primary hover:text-primary-dark font-medium">Sign In</RouterLink>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { showErrorToast, showSuccessToast } from '../utils/helpers.ts'
import api from '../utils/Api.ts'
import { useRouter } from 'vue-router'

const router = useRouter()

const firstName = ref('')
const middleName = ref('')
const lastName = ref('')
const email = ref('')
const password = ref('')
const address = ref('')
const city = ref('')
const state = ref('')
const country = ref('Egypt')
const phone = ref('')
const loading = ref(false)
const showPassword = ref(false)
const phoneError = ref('')

const governorates = [
  'Cairo',
  'Giza',
  'Alexandria',
  'Dakahlia',
  'Red Sea',
  'Beheira',
  'Fayoum',
  'Gharbiya',
  'Ismailia',
  'Menofia',
  'Minya',
  'Qaliubiya',
  'New Valley',
  'Suez',
  'Aswan',
  'Assiut',
  'Beni Suef',
  'Port Said',
  'Damietta',
  'Sharkia',
  'South Sinai',
  'Kafr Al sheikh',
  'Matrouh',
  'Luxor',
  'Qena',
  'North Sinai',
  'Sohag'
]

const cities = computed(() => {
  const cityMap = {
    'Cairo': [
      'Nasr City',
      'Maadi',
      'Heliopolis',
      'New Cairo',
      '6th of October City',
      'Sheikh Zayed City',
      'Dokki',
      'Garden City',
      'Zamalek',
      'Ain Shams',
      'Shubra',
      'El Marg',
      'El Rehab',
      'El Tagamoa',
      'El Sherouk',
      'Badr City',
      'New Administrative Capital'
    ],
    'Giza': [
      'Giza',
      '6th of October',
      'Sheikh Zayed',
      'Agouza',
      'Mohandessin',
      'Dokki',
      'Imbaba',
      'El Haram',
      'El Omraniya',
      'El Ayyat',
      'El Badrasheen',
      'El Saff',
      'El Hawamdeya',
      'El Warraq',
      'El Kerdasa'
    ]
  }
  return cityMap[state.value] || []
})

function validatePhone() {
  const phoneRegex = /^01[0125][0-9]{8}$/
  if (!phone.value) {
    phoneError.value = 'Phone number is required'
  } else if (!phoneRegex.test(phone.value)) {
    phoneError.value = 'Please enter a valid Egyptian phone number (e.g., 01XXXXXXXXX)'
  } else {
    phoneError.value = ''
  }
}

function validateEmail(email) {
  const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
  return emailRegex.test(email)
}

async function handleSignup() {
  if (!validateEmail(email.value)) {
    showErrorToast('Please enter a valid email address')
    return
  }

  if (phoneError.value) {
    showErrorToast('Please fix the phone number errors')
    return
  }

  const userPayload = {
    first_name: firstName.value,
    middle_name: middleName.value,
    last_name: lastName.value,
    email: email.value,
    password_hash: password.value,
    role: 'User',
    address: address.value,
    city: city.value,
    state: state.value,
    country: country.value,
    phone: phone.value,
  }

  loading.value = true
  try {
    const response = await api.post('/api/user/signup', userPayload)
    if (response.status === 201) {
      showSuccessToast("Welcome to Tiara! Redirecting to login page")
      setTimeout(() => {
        router.push('/login')
      }, 2000)
    }
    else if (response.status === 409) {
      showErrorToast('Email already exists. Please use a different email.')
    } else {
      showErrorToast(`Signup failed: ${response.data?.message || 'Unknown error'}`)
    }
  } catch (error) {
    console.error('Signup error:', error)
    showErrorToast('An error occurred during signup. Please try again later.')
  } finally {
    loading.value = false
  }
}
</script>
