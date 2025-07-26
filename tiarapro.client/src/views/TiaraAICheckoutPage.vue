<script setup lang="ts">
  import { computed, ref } from 'vue';
  import api from '../utils/Api.ts'
  import { useCartStore } from '../stores/CartStore'; // adjust path if needed
  import { showErrorToast } from '../utils/helpers.ts'

  const cartStore = useCartStore();
  const cartItems = computed(() => cartStore.cart);

  const userToken = ref<string | null>(localStorage.getItem('userToken'));
  const userEmail = ref<string | null>(localStorage.getItem('userEmail'));

  const loading = ref(false);
  const userFullName = ref('');
  const userAddress = ref('');
  const userPhoneNumber = ref('');
  const userCity = ref('');
  const userState = ref('');
  const userPostalCode = ref('');

  const subtotal = computed(() =>
    cartItems.value.reduce((total, item) => total + item.price * item.quantity, 0)
  );
  const shipping = ref(150);
  const tax = computed(() => subtotal.value * 0.14);
  const total = computed(() => subtotal.value + shipping.value);

  const paymentMethod = ref('credit');
  const cardholderName = ref('');
  const cardNumber = ref('');
  const expiryDate = ref('');
  const cvv = ref('');
  const email = ref(userEmail.value || '');
  const errors = ref<{ [key: string]: string }>({});

  // Validate form fields
  const validatePayment = () => {
    errors.value = {};

    // Validate name, email, and address
    if (!userFullName.value) errors.value.userFullName = 'Full name is required';
    if (!userEmail.value) errors.value.userEmail = 'Email is required';
    if (!userAddress.value) errors.value.userAddress = 'Address is required';
    if (!userPhoneNumber.value) errors.value.userPhoneNumber = 'Phone Number is required';

    // Validate payment details if credit card is chosen
    //if (paymentMethod.value === 'credit') {
    //  if (!cardholderName.value) errors.value.cardholderName = 'Cardholder name is required';
    //  if (!cardNumber.value.match(/^\d{16}$/)) errors.value.cardNumber = 'Enter a valid 16-digit card number';
    //  if (!expiryDate.value.match(/^(0[1-9]|1[0-2])\/\d{2}$/)) errors.value.expiryDate = 'Enter a valid MM/YY expiry date';
    //  if (!cvv.value.match(/^\d{3}$/)) errors.value.cvv = 'CVV must be 3 digits';
    //}
  };

  // Auto-format expiry date by adding '/' after first two digits
  const handleExpiryDateInput = () => {
    if (expiryDate.value.length === 2 && !expiryDate.value.includes('/')) {
      expiryDate.value = expiryDate.value + '/';
    }
  };

  const CreatePaymentRequest = async (orderId: number) => {
    try {
      const response = await api.post('/api/payment/create-intention', {
        amount: total.value * 100, // total amount to be paid
        currency: 'EGP',
        payment_methods: [12, 'card', 5021053], // static payment methods
        items: [
          ...cartItems.value.map(item => ({
            name: item.name,
            amount: item.price * 100,
            description: item.name,
            quantity: item.quantity,
          })),
          {
            name: 'Delivery Fee', // Added delivery fee item
            amount: shipping.value * 100, // Delivery fee amount
            description: 'Shipping cost for your order', // You can change this description
            quantity: 1, // Delivery fee is a single item
          },
        ],
        billing_data: {
          apartment: '',
          first_name: userFullName.value.split(' ')[0], // Assuming full name contains first and last names
          last_name: userFullName.value.split(' ')[1] || '', // Default to empty if only one name provided
          street: userAddress.value,
          building: '',
          phone_number: userPhoneNumber.value.toString(), // Update as needed
          country: 'EGY', // Update as needed
          email: email.value,
          floor: '1',
          state: userState.value,
        },
        customer: {
          first_name: userFullName.value.split(' ')[0],
          last_name: userFullName.value.split(' ')[1] || '',
          email: email.value,
          extras: {
            re: '22',
          },
        },
        extras: {
          ee: 22,
          tiara_order_id: orderId,
        },
      });
      return response
    } catch (error) {
      console.error('Error creating payment request:', error);
      showErrorToast(`Error creating payment request: ${error}`)
    }
  };



  const CreateOrderRequest = async () => {
    try {
      var requestBody = {
        user_id: localStorage.getItem('userId'),
        status: "Pending",
        total_amount: total.value,
        shipping_address: userAddress.value,
        shipping_city: userCity.value,
        shipping_state: userState.value ? userState.value : "Cairo" ,
        shipping_postal_code: userPostalCode.value,
        shipping_country: "EGY",
        shipping_phone: userPhoneNumber.value,
        order_items: cartItems.value.map(item => ({
          product_id: item.id,
          quantity: item.quantity,
          price: item.price,

        })),

      }
      const response = await api.post('/api/order/create', requestBody, {
        headers: {
          'Content-Type': 'application/json',
        },
      });
      console.log('Order request created successfully:', response.data);
      return response
    }
    catch (error) {
      console.error('Error creating order request:', error);
    }
  }
  // Make request to backend to place order
  const placeOrder = async () => {
    validatePayment();

    if (Object.keys(errors.value).length > 0) {
      return; // Don't send request if there are validation errors
    }

    loading.value = true;
    try {

      localStorage.setItem('userFullName', userFullName.value);
      localStorage.setItem('userEmail', userEmail.value);

      var orderResponse = await CreateOrderRequest();
      if (!orderResponse) {
        console.error('Order creation failed');
        return;
      }
      if (orderResponse.status != 201 || !orderResponse.data || !orderResponse.data.orderid) {
        console.error('Invalid order response:', orderResponse);
        showErrorToast(`Invalid order response: ${orderResponse}`)
        return;
      }

      var CreatePaymentResponse = await CreatePaymentRequest(orderResponse.data.orderid);

      if (CreatePaymentResponse.status != 200 || !CreatePaymentResponse.data) {
        console.error('Invalid payment response:', orderResponse);
        showErrorToast(`Invalid payment response: ${orderResponse}`)
        return;

      }

      window.location.href = CreatePaymentResponse.data;
     
    } catch (error) {
      console.error('Error placing order:', error);
      // Show error message to the user
    }
    finally {
      loading.value = false;
    }
  };
</script>





<template>
  <div class="container mx-auto py-8 grid grid-cols-1 lg:grid-cols-2 gap-8">
    <!-- Left Column (Order Summary + User Information) -->
    <div class="space-y-8">
      <!-- Order Summary -->
      <div class="bg-white p-6 rounded-lg shadow-md">
        <h2 class="text-xl font-semibold mb-4">Order Summary</h2>
        <div v-for="item in cartItems" :key="item.id" class="flex justify-between border-b py-2">
          <span>{{ item.name }}</span>
          <span>{{ item.price }} EGP</span>
        </div>
        <div class="mt-4">
          <div class="flex justify-between"><span>Subtotal</span><span>{{ subtotal }} EGP</span></div>
          <div class="flex justify-between"><span>Shipping</span><span>{{ shipping }} EGP</span></div>
          <!--<div class="flex justify-between"><span>Tax (14%)</span><span>{{ tax.toFixed(2) }} EGP</span></div>-->
          <div class="border-t mt-2 pt-2 flex justify-between font-semibold text-lg">
            <span>Total</span>
            <span>{{ total.toFixed(2) }} EGP</span>
          </div>
        </div>
      </div>

      <!-- User Information Section -->
      <div class="bg-white p-4 rounded-lg shadow-md">
        <h2 class="text-lg font-semibold mb-3">User Information</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
          <!-- Full Name -->
          <div>
            <label class="block text-xs font-medium mb-1">Full Name</label>
            <input type="text" v-model="userFullName" class="border rounded w-full px-2 py-1 text-sm" />
            <p v-if="errors.userFullName" class="text-red-500 text-sm">{{ errors.userFullName }}</p>
          </div>

          <!-- Email -->
          <div>
            <label class="block text-xs font-medium mb-1">Email</label>
            <div v-if="userToken" class="text-sm py-1 px-2 bg-gray-100 rounded">
              {{ userEmail }}
            </div>
            <div v-else>
              <input type="email" v-model="userEmail" class="border rounded w-full px-2 py-1 text-sm" />
              <p v-if="errors.userEmail" class="text-red-500 text-sm">{{ errors.userEmail }}</p>
            </div>
          </div>

          <!-- Phone Number-->
          <div class="md:col-span-2">
            <label class="block text-xs font-medium mb-1">Phone Number</label>
            <input type="tel" v-model="userPhoneNumber" class="border rounded w-full px-2 py-1 text-sm" />
            <p v-if="errors.userPhoneNumber" class="text-red-500 text-sm">{{ errors.userPhoneNumber }}</p>
          </div>


          <!-- Address -->
          <div class="md:col-span-2">
            <label class="block text-xs font-medium mb-1">Address</label>
            <input type="text" v-model="userAddress" class="border rounded w-full px-2 py-1 text-sm" />
            <p v-if="errors.userAddress" class="text-red-500 text-sm">{{ errors.userAddress }}</p>
          </div>

          <!-- City -->
          <div>
            <label class="block text-xs font-medium mb-1">City</label>
            <input type="text" v-model="userCity" class="border rounded w-full px-2 py-1 text-sm" />
          </div>

          <!-- State -->
          <div>
            <label class="block text-xs font-medium mb-1">State</label>
            <input type="text" v-model="userState" class="border rounded w-full px-2 py-1 text-sm" />
          </div>

          <!-- ZIP Code -->
          <div>
            <label class="block text-xs font-medium mb-1">Postal Code</label>
            <input type="text" v-model="userPostalCode" class="border rounded w-full px-2 py-1 text-sm" />
          </div>
        </div>
      </div>

    </div>

    <!-- Right Column (Payment Method) -->
    <div class="bg-white p-6 rounded-lg shadow-md">
      <h2 class="text-xl font-semibold mb-4">Payment Method</h2>
      <div class="mb-4">
        <label class="flex items-center gap-2">
          <input type="radio" v-model="paymentMethod" value="credit"> Credit Card
        </label>
        <label class="flex items-center gap-2 mt-2">
          <input type="radio" v-model="paymentMethod" value="cash"> Cash on Delivery
        </label>
      </div>

      <div v-if="paymentMethod === 'credit'">
        <!--<div class="mb-4">
          <label class="block text-sm font-medium">Cardholder Name</label>
          <input type="text" v-model="cardholderName" class="border rounded w-full p-2">
          <p v-if="errors.cardholderName" class="text-red-500 text-sm">{{ errors.cardholderName }}</p>
        </div>-->
        <!--<div class="mb-4">
          <label class="block text-sm font-medium">Card Number</label>
          <input type="text" v-model="cardNumber" class="border rounded w-full p-2" maxlength="16">
          <p v-if="errors.cardNumber" class="text-red-500 text-sm">{{ errors.cardNumber }}</p>
        </div>-->
        <!--<div class="flex gap-4">
          <div class="mb-4 w-1/2">
            <label class="block text-sm font-medium">Expiry Date (MM/YY)</label>
            <input type="text" v-model="expiryDate" @input="handleExpiryDateInput" class="border rounded w-full p-2" maxlength="5">
            <p v-if="errors.expiryDate" class="text-red-500 text-sm">{{ errors.expiryDate }}</p>
          </div>
          <div class="mb-4 w-1/2">
            <label class="block text-sm font-medium">CVV</label>
            <input type="text" v-model="cvv" class="border rounded w-full p-2" maxlength="3">
            <p v-if="errors.cvv" class="text-red-500 text-sm">{{ errors.cvv }}</p>
          </div>
        </div>-->
      </div>

      <button @click="placeOrder"
              :disabled="loading"
              class="bg-blue-500 text-white w-full p-2 rounded mt-4 hover:bg-blue-600 flex items-center justify-center">
        <span v-if="loading" class="spinner"></span>
        <span v-else>Place Order</span>
      </button>
    </div>
  </div>
</template>
<style scoped>
  .spinner {
    border: 4px solid #f3f3f3;
    border-top: 4px solid #3498db;
    border-radius: 50%;
    width: 24px;
    height: 24px;
    animation: spin 2s linear infinite;
  }

  @keyframes spin {
    0% {
      transform: rotate(0deg);
    }

    100% {
      transform: rotate(360deg);
    }
  }
</style>
