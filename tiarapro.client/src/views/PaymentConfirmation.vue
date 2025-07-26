<template>
  <div class="max-w-2xl mx-auto py-16 text-center">
    <div v-if="isLoading" class="flex flex-col items-center justify-center">
      <div class="animate-spin rounded-full h-16 w-16 border-b-2 border-primary"></div>
      <p class="mt-4 text-gray-600">Processing your order, please wait...</p>
    </div>

    <template v-else>
      <svg v-if="paymentSuccess" class="mx-auto mb-6 h-16 w-16 text-green-500" fill="none" stroke="currentColor"
        stroke-width="2" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
      </svg>

      <svg v-else class="mx-auto mb-6 h-16 w-16 text-red-500" fill="none" stroke="currentColor" stroke-width="2"
        viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
      </svg>

      <h1 class="text-3xl font-bold text-gray-800 mb-4">
        {{ paymentSuccess ? (paymentMethod === 'credit' ? 'Payment Successful!' : 'Order Placed Successfully!') :
          'Order Failed' }}
      </h1>
      <p v-if="paymentSuccess" class="text-lg text-gray-600">
        Thank you for your purchase. A confirmation email has been sent to you.<br />
        <span class="font-medium">Order ID: {{ orderId }}</span>
      </p>
      <p v-else class="text-lg text-gray-600">
        Something went wrong with your payment. Please try again or contact support.
      </p>

      <router-link to="/"
        class="mt-6 inline-block px-6 py-2 text-white bg-primary rounded hover:bg-primary/90 transition">
        Return to Home
      </router-link>
    </template>
  </div>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
import { onMounted, ref } from 'vue'
import api from '../utils/Api.ts'
import { showErrorToast } from '../utils/helpers.ts'
import router from '../router/index.ts'

const route = useRoute()
const paymentSuccess = ref(false)
const orderId = ref<string | null>(null)
const isLoading = ref(true)
const query = route.query
const success = query.success === 'true'
const order = query.order as string
const status = success ? 'Success' : 'Failed'
const tiaraOrderId = query.merchant_order_id
const paymentMethod = query.payment_method as string || 'credit'

const isTiaraAiSubscription = tiaraOrderId.includes('tiaraai') || null
const isTiaraDentalTraining = tiaraOrderId.includes('tiaradentaltraining') || null
const confirmOrder = async () => {

  try {
    debugger;

    if (!isTiaraAiSubscription) {
      let tOrderId = tiaraOrderId;
      if (isTiaraDentalTraining) {
        const tiaraId = Array.isArray(tiaraOrderId) ? tiaraOrderId[0].split('-')[1] : tiaraOrderId.split('-')[1];
        tOrderId = tiaraId;
      }

      var response = await api.post(`/api/order/confirmOrder/${tOrderId}/${status}`)
      if (response.status !== 200) {
        showErrorToast("Something went wrong please try again.")
        return;
      }
    }
    else {
      const tiaraId = Array.isArray(tiaraOrderId) ? tiaraOrderId[0].split('-')[1] : tiaraOrderId.split('-')[1];
      try {
        var response = await api.post('/api/tiaraaisubscription/user/' + localStorage.getItem('userId') + '/activate/' + tiaraId
        );

        if (response.status !== 200) {
          showErrorToast("Something went wrong please try again.")
          return;
        }
        if (response.status === 200) {
          localStorage.setItem('tiaraAiActive', 'true');
        }

        var response = await api.post(`/api/order/confirmOrder/${tiaraId}/${status}`)
        if (response.status !== 200) {
          showErrorToast("Something went wrong please try again.")
          return;
        }


        router.push({ name: 'ai-scan-upload' });
      } catch (subscriptionError) {
        console.error('Error creating subscription record:', subscriptionError);
        showErrorToast("Something went wrong please try again.")
        return;
      }
    }

  }
  catch (error) {
    showErrorToast("Something went wrong please try again.")
    return
  }
}

const updatePaymentStatus = async () => {
  try {
    var response = await api.post(
      `/api/payment/payment-status/${order}/${status}`
    )
    if (response.status !== 200) {
      throw new Error('Failed to update payment status')
    }
  } catch (error) {
    console.error('Error updating payment status:', error)
    throw error
  }
}

const createUserPromoCode = async () => {
  try {
    const response = await api.post('/api/promocode/createUserUsage', {
      user_id: localStorage.getItem('userId'),
      promo_code_id: localStorage.getItem('promoCodeId'),
      order_id: tiaraOrderId,
      used_at: new Date()
    })
    if (response.status !== 200) {
      throw new Error('Failed to create user promo code')
    }
  } catch (error) {
    console.error('Error creating user promo code:', error)
  }
  finally {
    localStorage.removeItem('promoCodeId')
    localStorage.removeItem('promoCodeApplied')
  }
}
onMounted(async () => {
  paymentSuccess.value = success
  orderId.value = order

  debugger;
  try {
    debugger;

    if (paymentMethod === 'credit') {
      await updatePaymentStatus();
    }
    await confirmOrder();

    if (localStorage.getItem('promoCodeApplied') == 'true') {
      await createUserPromoCode();
    }
  } catch (err) {
    console.error('Error processing payment:', err)
    paymentSuccess.value = false
  } finally {
    isLoading.value = false
  }
})
</script>
