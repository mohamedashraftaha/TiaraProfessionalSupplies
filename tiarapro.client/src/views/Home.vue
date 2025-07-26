<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Hero from '../components/Hero.vue'
import Categories from '../components/Categories.vue'
import FeaturedBrands from '../components/FeaturedBrands.vue'
//import SpecialOffer from '../components/SpecialOffer.vue'
import ExploreMore from '../components/ExploreMore.vue'
import Partners from '../components/Partners.vue'

// Mock featured products data
const featuredProducts = ref([
  {
    id: 1,
    name: 'Dental Composite Kit',
    price: 1200,
    image: 'https://images.unsplash.com/photo-1588776814546-1ffcf47267a5?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60',
    category: 'Dental Materials'
  },
  {
    id: 2,
    name: 'Orthodontic Pliers Set',
    price: 2500,
    image: 'https://images.unsplash.com/photo-1589279003513-467d320f47eb?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60',
    category: 'Instruments'
  },
  {
    id: 3,
    name: 'Digital X-Ray Sensor',
    price: 8000,
    image: 'https://images.unsplash.com/photo-1576671414121-aa2d0967199c?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60',
    category: 'Equipment'
  },
  {
    id: 4,
    name: 'Dental Chair Unit',
    price: 35000,
    image: 'https://images.unsplash.com/photo-1606811971618-4486d14f3f99?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60',
    category: 'Equipment'
  }
])

</script>

<template>
  <div>
    <!-- Hero Section -->
    <Hero />

    <!-- Categories Section -->
    <Categories />
    <FeaturedBrands />


    <ExploreMore />
    <!-- Featured Products -->
    <!--<section class="py-16 bg-gray-50">
    <div class="container-custom">
      <div class="flex justify-between items-center mb-8">
        <h2 class="text-3xl font-bold">Featured Products</h2>
        <router-link to="/products" class="text-primary hover:text-primary-dark font-medium">
          View All Products →
        </router-link>
      </div>
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
        <ProductCard v-for="product in featuredProducts" :key="product.id" :product="product" />
      </div>
    </div>
  </section>-->
    <!-- Partners & Technology Section -->
    <Partners />

    <!-- Training Programs -->
    <!--<section class="py-16 bg-white">
    <div class="container-custom">
      <h2 class="text-3xl font-bold text-center mb-12">Dental Training Programs</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div class="card hover:shadow-lg transition-shadow overflow-hidden">
          <img
            src="https://images.unsplash.com/photo-1576671414121-aa2d0967199c?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80"
            alt="Basic Dental Training" class="w-full h-48 object-cover">
          <div class="p-6">
            <h3 class="text-xl font-bold mb-2">Basic Dental Training</h3>
            <p class="text-gray-600 mb-4">Comprehensive training for dental students and new practitioners.</p>
            <router-link to="/training/basic" class="text-primary hover:text-primary-dark font-medium">
              Learn More →
            </router-link>
          </div>
        </div>
        <div class="card hover:shadow-lg transition-shadow overflow-hidden">
          <img
            src="https://images.unsplash.com/photo-1606811841689-23dfddce3e95?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80"
            alt="Advanced Techniques" class="w-full h-48 object-cover">
          <div class="p-6">
            <h3 class="text-xl font-bold mb-2">Advanced Techniques</h3>
            <p class="text-gray-600 mb-4">Specialized training for experienced dental professionals.</p>
            <router-link to="/training/advanced" class="text-primary hover:text-primary-dark font-medium">
              Learn More →
            </router-link>
          </div>
        </div>
        <div class="card hover:shadow-lg transition-shadow overflow-hidden">
          <img
            src="https://images.unsplash.com/photo-1579684385127-1ef15d508118?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80"
            alt="Radiology Certification" class="w-full h-48 object-cover">
          <div class="p-6">
            <h3 class="text-xl font-bold mb-2">Radiology Certification</h3>
            <p class="text-gray-600 mb-4">Specialized training in maxillofacial radiology techniques.</p>
            <router-link to="/training/radiology" class="text-primary hover:text-primary-dark font-medium">
              Learn More →
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </section>-->
    <!-- Testimonials -->
    <!--<section class="py-16 bg-gray-50">
    <div class="container-custom">
      <h2 class="text-3xl font-bold text-center mb-12">What Our Customers Say</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div class="card p-6">
          <div class="flex items-center mb-4">
            <div class="text-yellow-400 flex">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
            </div>
          </div>
          <p class="text-gray-600 mb-4">"The quality of dental supplies from Tiara Professional is exceptional. Their
            products have significantly improved my practice efficiency."</p>
          <div class="font-medium">Dr. Ahmed Hassan</div>
          <div class="text-sm text-gray-500">Dental Surgeon, Cairo</div>
        </div>
        <div class="card p-6">
          <div class="flex items-center mb-4">
            <div class="text-yellow-400 flex">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
            </div>
          </div>
          <p class="text-gray-600 mb-4">"The training programs offered by Tiara Dental Training have been invaluable
            for my professional development. Highly recommended!"</p>
          <div class="font-medium">Dr. Fatima Mahmoud</div>
          <div class="text-sm text-gray-500">Orthodontist, Alexandria</div>
        </div>
        <div class="card p-6">
          <div class="flex items-center mb-4">
            <div class="text-yellow-400 flex">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
            </div>
          </div>
          <p class="text-gray-600 mb-4">"The AI diagnostic tools provided by the Egyptian Maxillofacial Radiology
            Alliance have revolutionized how I analyze radiographs."</p>
          <div class="font-medium">Dr. Mohammed Ali</div>
          <div class="text-sm text-gray-500">Radiologist, Giza</div>
        </div>
      </div>
    </div>
  </section>-->
    <!-- Call to Action -->
    <section class="py-16 bg-primary text-white">
      <div class="container-custom text-center">
        <h2 class="text-3xl font-bold mb-4">Ready to Elevate Your Dental Practice?</h2>
        <p class="text-xl mb-8 max-w-3xl mx-auto">
          Join thousands of dental professionals who trust Tiara Professional
          Supplies for their equipment, materials, and training needs.
        </p>
        <div class="flex flex-col sm:flex-row justify-center gap-4">
          <router-link to="/products" class="btn bg-white text-primary hover:bg-gray-100">
            Shop Now
          </router-link>
          <router-link to="/contact" class="btn border-2 border-white text-white hover:bg-white hover:text-primary">
            Contact Us
          </router-link>
        </div>
      </div>
    </section>
  </div>
</template>
