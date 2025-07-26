<template>
  <div class="min-h-screen bg-white">
    <main>
      <!-- Hero Section -->
      <section class="relative py-24 bg-[#1e3a8a]">
        <div class="container mx-auto px-4 relative">
          <div class="max-w-xl mx-auto text-center">
            <!-- Logo and Partnership Section -->
            <div class="flex flex-col items-center mb-8">
              <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-AI-logo-light.svg"
                class="w-33 h-35 mx-auto mb-4" />

              <!-- Partnership Display -->
              <!--<div class="flex items-center justify-center mt-3">
                <span class="text-white/80 text-xl">Powered by</span>
                <div class="flex items-center ml-3 bg-white px-3 py-1 rounded-full">
                  <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/logo_128.png"
                       alt="DentalMesh.ai"
                       class="h-full w-full" />
                </div>
              </div>-->
            </div>

            <h1 class="text-5xl font-bold mb-6 text-white">
              Transform Your Practice with AI
            </h1>
            <p class="text-xl text-white/80 mb-12">
              Harness the power of artificial intelligence to enhance diagnostic accuracy, streamline workflows, and
              improve patient outcomes
            </p>
            <div class="flex justify-center gap-6">
              <!-- Subscribe Now Button -->
              <button @click="scrollToSubscription"
                class="px-8 py-4 bg-white text-[#1e3a8a] rounded-lg font-semibold hover:bg-gray-100 transition-colors">
                Subscribe Now
              </button>
            </div>
          </div>
        </div>
      </section>

      <!-- Features Grid -->
      <section class="py-24">
        <div class="container mx-auto px-4">
          <h2 class="text-3xl font-bold text-center mb-16">How Tiara AI Transforms Dental Practices</h2>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-12 max-w-5xl mx-auto">
            <div v-for="feature in features" :key="feature.title"
              class="bg-white p-8 rounded-lg border border-gray-200 hover:border-[#1e3a8a] transition-colors">
              <div class="w-16 h-16 bg-[#1e3a8a]/10 rounded-lg flex items-center justify-center mb-6">
                <i :class="[feature.icon, 'text-[#1e3a8a] text-2xl']"></i>
              </div>
              <h3 class="text-xl font-bold mb-4">{{ feature.title }}</h3>
              <p class="text-gray-600">{{ feature.description }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Technology Partnership Section -->
      <section class="py-16 bg-gray-50">
        <div class="container mx-auto px-4">
          <div class="max-w-4xl mx-auto">
            <h2 class="text-3xl font-bold text-center mb-8">Powered by Advanced Technology</h2>
            <div class="bg-white rounded-lg shadow-sm p-8">
              <div class="flex flex-col md:flex-row items-center justify-between">
                <div class="md:w-1/2 mb-8 md:mb-0 md:pr-8">
                  <h3 class="text-2xl font-bold mb-4">Partnership with DentalMesh.ai</h3>
                  <p class="text-gray-600 mb-4">
                    Tiara AI leverages DentalMesh.ai's cutting-edge dental image analysis technology to provide you with
                    the most accurate and reliable AI-powered diagnostics in the industry.
                  </p>
                  <p class="text-gray-600">
                    This strategic partnership combines our clinical expertise with DentalMesh.ai's advanced machine
                    learning algorithms to deliver superior results for your practice.
                  </p>
                </div>
                <div class="md:w-1/2 flex justify-center">
                  <a href="https://dentalmesh.ai/" target="_blank" rel="noopener noreferrer"
                    class="transition-transform hover:scale-105">
                    <div
                      class="flex items-center p-6 border border-gray-200 rounded-lg hover:shadow-md transition-shadow">
                      <img src="https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/logo_256.png"
                        alt="DentalMesh.ai" class="h-24" />
                    </div>
                  </a>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Subscription Section -->
      <section id="subscribe" class="py-24 bg-gradient-to-br from-gray-50 to-blue-50">
        <div class="container mx-auto px-4">
          <div class="max-w-4xl mx-auto text-center mb-16">
            <h2 class="text-4xl font-bold mb-6 text-gray-900">Choose Your TiaraAI Plan</h2>
            <p class="text-xl text-gray-600 max-w-2xl mx-auto">
              Start leveraging the power of AI today with our flexible subscription plans designed for dental practices
              of all sizes
            </p>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-3 gap-8 max-w-6xl mx-auto">
            <!-- Light Package -->
            <div
              class="bg-white rounded-2xl shadow-lg border border-gray-100 overflow-hidden hover:shadow-2xl hover:scale-105 transition-all duration-300">
              <div class="p-8">
                <div class="text-center mb-8">
                  <div class="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <i class="fas fa-seedling text-gray-600 text-2xl"></i>
                  </div>
                  <h3 class="text-2xl font-bold mb-3 text-gray-900">Light Package</h3>
                  <div class="mb-4">
                    <span class="text-4xl font-bold text-gray-900">EGP 2,900</span>
                    <!-- <span class="text-gray-500 text-lg">/month</span> -->
                  </div>
                  <p class="text-gray-600 font-medium">Perfect for small practices</p>
                </div>

                <ul class="space-y-4 mb-8">
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-green-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-green-600 text-xs"></i>
                    </div>
                    <span class="text-gray-700">10 AI Segmentations</span>
                  </li>
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-green-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-green-600 text-xs"></i>
                    </div>
                    <span class="text-gray-700">Advanced AI Analysis</span>
                  </li>
                  <!-- <li class="flex items-center">
                    <div class="w-5 h-5 bg-green-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-green-600 text-xs"></i>
                    </div>
                    <span class="text-gray-700">Cloud Storage</span>
                  </li>
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-green-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-green-600 text-xs"></i>
                    </div>
                    <span class="text-gray-700">Email Support</span>
                  </li> -->
                </ul>

                <button @click="selectSubscription(1, 'Light Package')"
                  class="w-full py-4 bg-gradient-to-r from-gray-600 to-gray-700 text-white rounded-xl font-semibold hover:from-gray-700 hover:to-gray-800 transition-all duration-300 transform hover:scale-105 shadow-lg">
                  Get Started
                </button>
              </div>
            </div>

            <!-- Pro Package -->
            <div
              class="bg-white rounded-2xl shadow-2xl border-2 border-[#1e3a8a] overflow-hidden relative hover:shadow-3xl hover:scale-105 transition-all duration-300 transform scale-105">
              <div class="absolute top-0 left-1/2 transform -translate-x-1/2 -translate-y-1/2">
              </div>
              <div class="p-8 pt-12">
                <div class="text-center mb-8">
                  <div class="w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <i class="fas fa-crown text-[#1e3a8a] text-2xl"></i>
                  </div>
                  <h3 class="text-2xl font-bold mb-3 text-gray-900">Pro</h3>
                  <div class="mb-4">
                    <span class="text-4xl font-bold text-[#1e3a8a]">EGP 6,100</span>
                    <!-- <span class="text-gray-500 text-lg">/month</span> -->
                  </div>
                  <p class="text-gray-600 font-medium">Ideal for growing practices</p>
                </div>

                <ul class="space-y-4 mb-8">
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-blue-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-[#1e3a8a] text-xs"></i>
                    </div>
                    <span class="text-gray-700 font-medium">25 AI Segmentations</span>
                  </li>
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-blue-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-[#1e3a8a] text-xs"></i>
                    </div>
                    <span class="text-gray-700 font-medium">Advanced AI Analysis</span>
                  </li>
                  <!-- <li class="flex items-center">
                    <div class="w-5 h-5 bg-blue-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-[#1e3a8a] text-xs"></i>
                    </div>
                    <span class="text-gray-700 font-medium">Cloud Storage</span>
                  </li>
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-blue-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-[#1e3a8a] text-xs"></i>
                    </div>
                    <span class="text-gray-700 font-medium">Priority Support</span>
                  </li>
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-blue-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-[#1e3a8a] text-xs"></i>
                    </div>
                    <span class="text-gray-700 font-medium">Treatment Planning</span>
                  </li> -->
                </ul>

                <button @click="selectSubscription(2, 'Pro')"
                  class="w-full py-4 bg-gradient-to-r from-[#1e3a8a] to-blue-600 text-white rounded-xl font-semibold hover:from-blue-700 hover:to-blue-800 transition-all duration-300 transform hover:scale-105 shadow-lg">
                  Choose Pro
                </button>
              </div>
            </div>

            <!-- Premium Package -->
            <div
              class="bg-white rounded-2xl shadow-lg border border-gray-100 overflow-hidden hover:shadow-2xl hover:scale-105 transition-all duration-300">
              <div class="p-8">
                <div class="text-center mb-8">
                  <div class="w-16 h-16 bg-yellow-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <i class="fas fa-gem text-yellow-600 text-2xl"></i>
                  </div>
                  <h3 class="text-2xl font-bold mb-3 text-gray-900">Premium</h3>
                  <div class="mb-4">
                    <span class="text-4xl font-bold text-gray-900">EGP 9,800</span>
                    <span class="text-gray-500 text-lg">/month</span>
                  </div>
                  <p class="text-gray-600 font-medium">For busy practices</p>
                </div>

                <ul class="space-y-4 mb-8">
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-yellow-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-yellow-600 text-xs"></i>
                    </div>
                    <span class="text-gray-700">50 AI Segmentations</span>
                  </li>
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-yellow-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-yellow-600 text-xs"></i>
                    </div>
                    <span class="text-gray-700">Advanced AI Analysis</span>
                  </li>
                  <!-- <li class="flex items-center">
                    <div class="w-5 h-5 bg-yellow-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-yellow-600 text-xs"></i>
                    </div>
                    <span class="text-gray-700">Unlimited Cloud Storage</span>
                  </li>
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-yellow-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-yellow-600 text-xs"></i>
                    </div>
                    <span class="text-gray-700">24/7 Support</span>
                  </li>
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-yellow-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-yellow-600 text-xs"></i>
                    </div>
                    <span class="text-gray-700">Treatment Planning</span>
                  </li>
                  <li class="flex items-center">
                    <div class="w-5 h-5 bg-yellow-100 rounded-full flex items-center justify-center mr-3">
                      <i class="fas fa-check text-yellow-600 text-xs"></i>
                    </div>
                    <span class="text-gray-700">API Access</span>
                  </li> -->
                </ul>

                <button @click="selectSubscription(3, 'Premium')"
                  class="w-full py-4 bg-gradient-to-r from-gray-800 to-gray-900 text-white rounded-xl font-semibold hover:from-gray-900 hover:to-black transition-all duration-300 transform hover:scale-105 shadow-lg">
                  Go Premium
                </button>
              </div>
            </div>
          </div>

          <div class="text-center mt-16">
            <div class="bg-white rounded-2xl shadow-lg p-8 max-w-2xl mx-auto">
              <h3 class="text-2xl font-bold mb-4 text-gray-900">Need Custom Solutions?</h3>
              <p class="text-gray-600 mb-6">Get in touch with our sales team to discuss enterprise pricing and custom
                features tailored to your practice.</p>
              <button
                class="px-8 py-4 border-2 border-[#1e3a8a] text-[#1e3a8a] rounded-xl font-semibold hover:bg-[#1e3a8a] hover:text-white transition-all duration-300 transform hover:scale-105">
                Contact Our Sales Team
              </button>
            </div>
          </div>
        </div>
      </section>


    </main>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useCartStore } from '../stores/CartStore';
import { useRouter } from 'vue-router';
import { showInfoToast } from '../utils/helpers';

const cartStore = useCartStore();
const router = useRouter();

const features = [
  {
    title: 'Diagnostic Assistance',
    description: 'Our AI helps identify abnormalities in dental images with higher accuracy',
    icon: 'fas fa-brain'
  },
  {
    title: 'Treatment Planning',
    description: 'Generate personalized treatment plans based on patient history and best practices',
    icon: 'fas fa-clipboard-list'
  },
  {
    title: 'Risk Assessment',
    description: 'Predict potential oral health issues before they develop, allowing for preventive intervention',
    icon: 'fas fa-chart-line'
  }
];

const subscriptionPlans = [
  { id: 1, name: 'Light Package', price: 2900, segmentations: 10 },
  { id: 2, name: 'Pro', price: 6100, segmentations: 25 },
  { id: 3, name: 'Premium', price: 9800, segmentations: 50 }
];

const scrollToSubscription = () => {
  const section = document.getElementById('subscribe');
  if (section) {
    section.scrollIntoView({ behavior: 'smooth' });
  }
};

const selectSubscription = (subscriptionId: number, subscriptionName: string) => {
  // Check if user is authenticated
  debugger;
  const userToken = localStorage.getItem('token');
  const userEmail = localStorage.getItem('email');

  if (userToken === "null" && !userEmail) {
    // User is not signed in, show alert and redirect to login
    showInfoToast('Please sign in to subscribe to Tiara AI plans. You will be redirected to the login page.');
    setTimeout(() => {
      router.push('/login');
    }, 2000);
    return;
  }

  const plan = subscriptionPlans.find(p => p.id === subscriptionId);
  if (plan) {
    // Clear cart first to ensure only the subscription is added
    cartStore.clearCart();

    debugger;
    // Add subscription to cart with proper naming format
    cartStore.addToCart({
      id: subscriptionId + 1000000, // Use unique ID for subscriptions
      name: `Tiara AI ${subscriptionName}`, // Updated naming format
      price: plan.price,
      image: 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-AI-logo-original.svg',
      quantity: 1
    });

    // Navigate to checkout
    router.push('/checkout');
  }
};
</script>
