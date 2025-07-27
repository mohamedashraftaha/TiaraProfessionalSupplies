<script setup lang="ts">
import { computed, ref } from 'vue';
import { useCartStore } from '../stores/CartStore'; // adjust path if needed
import { showErrorToast } from '../utils/helpers.ts'
import api from '../utils/Api.ts'
import { useRouter } from 'vue-router';

const router = useRouter();

const cartStore = useCartStore();
const cartItems = computed(() => cartStore.cart);

const userToken = ref<string | null>(localStorage.getItem('userToken'));
const userEmail = ref<string | null>(localStorage.getItem('email'));

const loading = ref(false);

const userAddress = ref(localStorage.getItem("Address") || "");
const userPhoneNumber = ref(localStorage.getItem("PhoneNumber") || ""); // Fixed duplicate key
const userCity = ref(localStorage.getItem("City") || "");
const userState = ref(localStorage.getItem("State") || ""); // Default already an empty string
const userPostalCode = ref("");
const subtotal = computed(() =>
  cartItems.value.reduce((total, item) => total + item.price * item.quantity, 0)
);

const hasOnlySubscriptions = computed(() =>
  cartItems.value.length > 0 && cartItems.value.every(item => item.id > 1000000 && item.id <= 1000003)
);

const hasOnlyDentalTrainings = computed(() =>
  cartItems.value.length > 0 && cartItems.value.every(item => item.id >= 2000000 && item.id < 3000000)
);


const shipping = computed(() => hasOnlySubscriptions.value || hasOnlyDentalTrainings.value ? 0 : 150);
const tax = computed(() => subtotal.value * 0.14);
const discount = computed(() => cartStore.promoCode?.discount_amount || 0);

const promoCodeApplied = discount.value > 0 ? localStorage.setItem("promoCodeApplied", 'true') : 'false';

const promoCodeId = computed(() => cartStore.promoCode?.promo_code_id || -1);
localStorage.setItem("promoCodeId", promoCodeId.value.toString());
const total = computed(() => subtotal.value + shipping.value - discount.value);

const paymentMethod = ref('credit');
const cardholderName = ref('');
const cardNumber = ref('');
const expiryDate = ref('');
const cvv = ref('');
const email = ref(userEmail.value || '');
const errors = ref<{ [key: string]: string }>({});


const userFirstName = ref(localStorage.getItem("firstName") || "");
const userMiddleName = ref(localStorage.getItem("middleName") || "");
const userLastName = ref(localStorage.getItem("lastName") || "");

const userFullName = () => {
  return `${localStorage.getItem("firstName") || userFirstName.value} ${localStorage.getItem("MiddleName") || userMiddleName.value} ${localStorage.getItem("lastName") || userLastName.value}`.trim()

}

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
  return cityMap[userState.value] || []
})

// Validate Egyptian phone number format
const validateEgyptianPhoneNumber = (phoneNumber: string): boolean => {
  // Remove any spaces, dashes, or plus signs
  const cleanNumber = phoneNumber.replace(/[\s\-\+]/g, '');

  // Egyptian mobile numbers: 11 digits starting with 010, 011, 012, or 015
  const egyptianMobileRegex = /^01[0125]\d{8}$/;

  return egyptianMobileRegex.test(cleanNumber);
};

// Validate form fields
const validatePayment = () => {

  errors.value = {};

  // Validate name, email, and address

  if (!userFirstName.value) errors.value.userFirstName = 'First name is required';
  if (!userMiddleName.value) errors.value.userMiddleName = 'Middle name is required';
  if (!userLastName.value) errors.value.userLastName = 'Last name is required';
  if (!userEmail.value) errors.value.userEmail = 'Email is required';
  else if (!/^\S+@\S+\.\S+$/.test(userEmail.value)) errors.value.userEmail = 'Enter a valid email address';
  if (!userAddress.value) errors.value.userAddress = 'Address is required';
  if (!userPhoneNumber.value) {
    errors.value.userPhoneNumber = 'Phone Number is required';
  } else if (!validateEgyptianPhoneNumber(userPhoneNumber.value)) {
    errors.value.userPhoneNumber = 'Please enter a valid Egyptian phone number (e.g., 01012345678)';
  }
  // if (!userCity.value) errors.value.userCity = 'City is required';
  if (!userState.value) errors.value.userState = 'Governorate is required';

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

const CreatePaymentRequest = async (orderId: number | string) => {
  try {
    debugger;
    const response = await api.post('/api/payment/create-intention', {
      amount: total.value * 100, // total amount to be paid
      currency: 'EGP',
      payment_methods: [12, 'card', 5193419], // static payment methods Test: 5021053
      items: [
        ...cartItems.value.map(item => ({
          name: (item.name != null && item.name.length > 50) ? item.name.substring(0, 50) : item.name,
          amount: item.price * 100,
          description: item.name,
          quantity: item.quantity,
        })),
        ...(shipping.value > 0 ? [{
          name: 'Delivery Fee', // Added delivery fee item
          amount: shipping.value * 100, // Delivery fee amount
          description: 'Shipping cost for your order', // You can change this description
          quantity: 1, // Delivery fee is a single item
        }] : []),
        ...(discount.value > 0 ? [{
          name: 'Discount', // Added delivery fee item
          amount: discount.value * -100, // Delivery fee amount
          description: 'Discount for your order', // You can change this description
          quantity: 1, // Delivery fee is a single item
        }] : []),
      ],
      billing_data: {
        apartment: '',
        first_name: userFirstName.value.split(' ')[0], // Assuming full name contains first and last names
        last_name: userLastName.value.split(' ')[0], // Default to empty if only one name provided
        street: userAddress.value,
        building: '',
        phone_number: userPhoneNumber.value.toString(), // Update as needed
        country: 'EGY', // Update as needed
        email: email.value,
        floor: '1',
        state: userState.value,
      },
      customer: {
        first_name: userFirstName.value.split(' ')[0],
        last_name: userLastName.value.split(' ')[0],
        email: email.value,
        extras: {
          re: '22',
        },
      },
      extras: {
        ee: 22,
        tiara_order_id: orderId,
      },
      special_reference: orderId
    });
    return response
  } catch (error) {
    console.error('Error creating payment request:', error);
    showErrorToast(`Error creating payment request: ${error}`)
  }
};



const CreateOrderRequest = async () => {
  try {
    debugger;
    var requestBody = {
      user_id: localStorage.getItem('userId'),
      status: "Pending",
      total_amount: total.value,
      shipping_address: userAddress.value,
      shipping_city: userCity.value,
      shipping_state: userState.value ? userState.value : "Cairo",
      shipping_postal_code: userPostalCode.value,
      shipping_country: "EGY",
      shipping_phone: userPhoneNumber.value,
      shipping_email: userEmail.value,
      shipping_user_first_name: userFirstName.value,
      shipping_user_last_name: userLastName.value,
      shipping_user_middle_name: userMiddleName.value,
      promo_code_id: cartStore.promoCode?.promo_code_id,
      order_items: cartItems.value.map(item => ({
        product_name: item.name,
        product_id: item.id,
        quantity: item.quantity,
        price: item.price,
        product_image: item.image,
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
  debugger;
  validatePayment();
  if (subtotal.value === 0) {
    showErrorToast("Your cart is empty. Please add items before proceeding to checkout.");
    return;
  }

  if (Object.keys(errors.value).length > 0) {
    return; // Don't send request if there are validation errors
  }

  loading.value = true;
  try {


    localStorage.setItem('userFullName', userFullName());
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
    debugger;

    const hasTiaraAISubscription = cartItems.value.some(item => item.id > 1000000 && item.id <= 1000003);
    if (hasTiaraAISubscription && orderResponse.data.orderid) {
      const subscriptionItem = cartItems.value.find(item => item.id > 1000000 && item.id <= 1000003);
      if (subscriptionItem) {
        const subscriptionId = subscriptionItem.id - 1000000;
        try {
          await api.post('/api/tiaraaisubscription/user/' + localStorage.getItem('userId') + '/subscribe', {
            subscription_id: subscriptionId,
            order_id: orderResponse.data.orderid
          });
        } catch (subscriptionError) {
          console.error('Error creating subscription record:', subscriptionError);
        }
      }
    }

    const hasDentalTrainingRegistration = cartItems.value.some(item => item.id >= 2000000 && item.id < 3000000);

    if (paymentMethod.value === 'credit') {
      var tiaraId = hasTiaraAISubscription ? `tiaraai-${orderResponse.data.orderid}` : orderResponse.data.orderid;
      if (hasDentalTrainingRegistration) {
        tiaraId = `tiaradentaltraining-${orderResponse.data.orderid}`;
        const dentalTrainingItem = cartItems.value.find(item => item.id >= 2000000 && item.id < 3000000);
        if (dentalTrainingItem) {
          const dentalTrainingId = dentalTrainingItem.id - 2000000;
          try {
            var response = await api.post('/api/dentaltraining/' + dentalTrainingId + '/register/' + localStorage.getItem('userId') + '/' + orderResponse.data.orderid);
            if (response.status != 200) {
              console.error('Error creating dental training registration record:', response);
              showErrorToast(`Error creating dental training registration record: ${response}`)
              return;
            }
          } catch (dentalTrainingError) {
            console.error('Error creating dental training registration record:', dentalTrainingError);
          }
        }
      }

      var CreatePaymentResponse = await CreatePaymentRequest(tiaraId);

      if (CreatePaymentResponse.status != 200 || !CreatePaymentResponse.data) {
        console.error('Invalid payment response:', orderResponse);
        showErrorToast(`Invalid payment response: ${orderResponse}`)
        return;

      }

      // Clear cart before redirecting to payment
      cartStore.clearCart();
      window.location.href = CreatePaymentResponse.data;
    }
    else {
      // Clear cart for cash on delivery
      cartStore.clearCart();
      router.push({ name: 'payment-confirmation', query: { success: 'true', order: orderResponse.data.orderid, merchant_order_id: orderResponse.data.orderid, payment_method: paymentMethod.value } });
    }

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
          <div v-if="shipping > 0" class="flex justify-between"><span>Shipping</span><span>{{ shipping }} EGP</span>
          </div>
          <div v-if="discount > 0" class="flex justify-between"><span>Discount</span><span class="text-green-600">-{{
            discount }} EGP</span></div>
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
          <!-- First Name -->
          <div>
            <label class="block text-xs font-medium mb-1">First Name</label>
            <input type="text" v-model="userFirstName" class="border rounded w-full px-2 py-1 text-sm" />
            <p v-if="errors.userFirstName" class="text-red-500 text-sm">{{ errors.userFirstName }}</p>
          </div>

          <!-- Middle Name -->
          <div>
            <label class="block text-xs font-medium mb-1">Middle Name</label>
            <input type="text" v-model="userMiddleName" class="border rounded w-full px-2 py-1 text-sm" />
            <p v-if="errors.userMiddleName" class="text-red-500 text-sm">{{ errors.userMiddleName }}</p>

          </div>

          <!-- Last Name -->
          <div>
            <label class="block text-xs font-medium mb-1">Last Name</label>
            <input type="text" v-model="userLastName" class="border rounded w-full px-2 py-1 text-sm" />
            <p v-if="errors.userLastName" class="text-red-500 text-sm">{{ errors.userLastName }}</p>
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
            <input type="tel" v-model="userPhoneNumber" placeholder="01012345678"
              class="border rounded w-full px-2 py-1 text-sm" />
            <p v-if="errors.userPhoneNumber" class="text-red-500 text-sm">{{ errors.userPhoneNumber }}</p>
          </div>


          <!-- Address -->
          <div class="md:col-span-2">
            <label class="block text-xs font-medium mb-1">Address</label>
            <input type="text" v-model="userAddress" class="border rounded w-full px-2 py-1 text-sm" />
            <p v-if="errors.userAddress" class="text-red-500 text-sm">{{ errors.userAddress }}</p>
          </div>

          <!-- State/Governorate -->
          <div>
            <label class="block text-xs font-medium mb-1">Governorate</label>
            <select v-model="userState" class="border rounded w-full px-2 py-1 text-sm">
              <option value="">Select Governorate</option>
              <option v-for="gov in governorates" :key="gov" :value="gov">{{ gov }}</option>
            </select>
            <p v-if="errors.userState" class="text-red-500 text-sm">{{ errors.userState }}</p>
          </div>


          <!-- City -->
          <div>
            <label class="block text-xs font-medium mb-1">City</label>
            <select v-model="userCity" class="border rounded w-full px-2 py-1 text-sm">
              <option value="">Select City</option>
              <option v-for="city in cities" :key="city" :value="city">{{ city }}</option>
            </select>
            <p v-if="errors.userCity" class="text-red-500 text-sm">{{ errors.userCity }}</p>
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

      <!-- Payment options for regular products (with shipping) -->
      <div v-if="!hasOnlySubscriptions && !hasOnlyDentalTrainings" class="mb-4">
        <label class="flex items-center gap-2">
          <input type="radio" v-model="paymentMethod" value="credit"> Credit Card
        </label>
        <label class="flex items-center gap-2 mt-2">
          <input type="radio" v-model="paymentMethod" value="cash"> Cash on Delivery
        </label>
      </div>

      <!-- Payment options for TiaraAI subscriptions (credit card only) -->
      <div v-else class="mb-4">
        <label class="flex items-center gap-2">
          <input type="radio" v-model="paymentMethod" value="credit"> Credit Card
        </label>
        <p class="text-sm text-gray-600 mt-2">
          <i class="fas fa-info-circle mr-1"></i>
          Subscriptions require online payment
        </p>
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

      <button @click="placeOrder" :disabled="loading"
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
