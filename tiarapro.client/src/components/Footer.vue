<template>
  <footer class="bg-gray-800 text-white">
    <div class="container-custom py-5">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-8">
        <div>
          <h3 class="text-xl font-bold mb-4">Tiara Professional Supplies</h3>
          <p class="text-gray-300">
            Providing top-quality dental supplies, training, and innovative solutions for dental professionals.
          </p>
        </div>
        <div>
          <h4 class="text-lg font-semibold mb-4">Quick Links</h4>
          <ul class="space-y-2">
            <li><router-link to="/" class="text-gray-300 hover:text-white">Home</router-link></li>
            <li><router-link to="/products" class="text-gray-300 hover:text-white">Products</router-link></li>
            <li><router-link to="/ai" class="text-gray-300 hover:text-white">Tiara AI</router-link></li>
            <li><router-link to="/training" class="text-gray-300 hover:text-white">Tiara Dental Training</router-link>
            </li>
            <li><router-link to="/events" class="text-gray-300 hover:text-white">Tiara Events</router-link></li>
            <li><router-link to="/about" class="text-gray-300 hover:text-white">About Us</router-link></li>
          </ul>
        </div>
        <div>
          <h4 class="text-lg font-semibold mb-4">Customer Service</h4>
          <ul class="space-y-2">
            <li><router-link to="/contact" class="text-gray-300 hover:text-white">Contact Us</router-link></li>
            <li><router-link to="/shipping-policy" class="text-gray-300 hover:text-white">Shipping Policy</router-link>
            </li>
            <li><router-link to="/refund-exchange-policy" class="text-gray-300 hover:text-white">Returns &
                Refunds</router-link></li>
            <li><router-link to="/privacy-policy" class="text-gray-300 hover:text-white">Privacy Policy</router-link>
            </li>
          </ul>
        </div>
        <div>
          <h4 class="text-lg font-semibold mb-4">Contact Information</h4>
          <ul class="space-y-2 text-gray-300">
            <li class="flex items-center">
              <a href="https://www.google.com/maps/dir/?api=1&destination=6%20Merryland%20Buildings%2C%20Heliopolis%2C%20Cairo%20Governorate%2C%20Egypt"
                target="_blank" class="flex items-center">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10 mr-2" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                6 Merryland Buildings, Heliopolis, Cairo Governorate, Egypt
              </a>
            </li>

            <li class="flex items-center">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
              </svg>
              info@tiarapro.com
            </li>
            <li class="flex items-center">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
              </svg>
              +20 24500090
            </li>
          </ul>
          <div class="mt-4 flex space-x-4">
            <a href="https://www.facebook.com/sharer/sharer.php?u=https%3A%2F%2Ftiarapro.ecwid.com%2F"
              class="text-gray-300 hover:text-white">
              <span class="sr-only">Facebook</span>
              <svg class="h-6 w-6" fill="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                <path fill-rule="evenodd"
                  d="M22 12c0-5.523-4.477-10-10-10S2 6.477 2 12c0 4.991 3.657 9.128 8.438 9.878v-6.987h-2.54V12h2.54V9.797c0-2.506 1.492-3.89 3.777-3.89 1.094 0 2.238.195 2.238.195v2.46h-1.26c-1.243 0-1.63.771-1.63 1.562V12h2.773l-.443 2.89h-2.33v6.988C18.343 21.128 22 16.991 22 12z"
                  clip-rule="evenodd" />
              </svg>
            </a>
            <a href="https://x.com/intent/post?url=https%3A%2F%2Ftiarapro.ecwid.com%2F&text=Tiara%20Professional%20Supplies%3A"
              class="text-gray-300 hover:text-white">
              <span class="sr-only">X</span>
              <svg class="h-6 w-6" fill="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                <path
                  d="M8.29 20.251c7.547 0 11.675-6.253 11.675-11.675 0-.178 0-.355-.012-.53A8.348 8.348 0 0022 5.92a8.19 8.19 0 01-2.357.646 4.118 4.118 0 001.804-2.27 8.224 8.224 0 01-2.605.996 4.107 4.107 0 00-6.993 3.743 11.65 11.65 0 01-8.457-4.287 4.106 4.106 0 001.27 5.477A4.072 4.072 0 012.8 9.713v.052a4.105 4.105 0 003.292 4.022 4.095 4.095 0 01-1.853.07 4.108 4.108 0 003.834 2.85A8.233 8.233 0 012 18.407a11.616 11.616 0 006.29 1.84" />
              </svg>
            </a>
          </div>
        </div>
      </div>
      <div class="mt-8 pt-8 border-t border-gray-700 text-center text-gray-400">
        <p>&copy; {{ new Date().getFullYear() }} Tiara Professional Supplies. All rights reserved.</p>
      </div>
    </div>
  </footer>
</template>
