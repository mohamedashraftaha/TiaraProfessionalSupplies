<script setup lang="ts">
  import { useRoute } from 'vue-router'
  import {onMounted } from 'vue'

  const route = useRoute()
  const orders = JSON.parse(localStorage.getItem('myOrders')) || []
  onMounted(() => {
    console.log("ORDERS", orders)
  })

  
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <main class="container mx-auto px-4 py-8">
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex justify-between items-center mb-6">
          <h3 class="font-semibold">Recent Orders</h3>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="text-left text-gray-600 border-b">
                <th class="pb-4 font-medium">Order ID</th>
                <th class="pb-4 font-medium">Date</th>
                <th class="pb-4 font-medium">Items</th>
                <th class="pb-4 font-medium">Total</th>
                <th class="pb-4 font-medium">Status</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="order in orders" :key="order.id" class="border-b">
                <td class="py-4">
                  <span class="font-medium text-[#4052B5]">{{ order.id }}</span>
                </td>
                <td class="py-4 text-gray-600">{{ order.date }}</td>
                <td class="py-4 text-gray-600">
                  <div class="grid grid-cols-1 sm:grid-cols-2 gap-2">
                    <div v-for="(item, index) in order.items"
                         :key="index"
                         class="flex items-center gap-3 bg-gray-50 p-2 rounded-lg shadow-sm">
                      <img :src="item.image"
                           alt="Item image"
                           class="w-10 h-10 object-cover rounded" />
                      <div class="text-sm">
                        <div class="font-medium">{{ item.name }}</div>
                        <div class="text-gray-500">x{{ item.quantity }}</div>
                      </div>
                    </div>
                  </div>
                </td>
                <td class="py-4 font-medium">{{ order.total }}</td>
                <td class="py-4">
                  <span class="px-3 py-1 bg-green-100 text-green-700 rounded-full text-sm">
                    {{ order.status }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </main>
  </div>
</template>
