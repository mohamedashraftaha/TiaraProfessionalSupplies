import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import ProductList from '../views/ProductList.vue'
import ProductDetail from '../views/ProductDetail.vue'
import Cart from '../views/Cart.vue'
import Checkout from '../views/Checkout.vue'
import AboutUs from '../views/AboutUs.vue'
import Contact from '../views/Contact.vue'
import DentalTraining from '../views/DentalTraining.vue'
import Login from '../views/Login.vue'
import Signup from '../views/Signup.vue'
import PaymentConfirmation from '../views/PaymentConfirmation.vue'
import Account from '../views/Account.vue'
import TiaraAi from '../views/TiaraAi.vue'
import Events from '../views/Events.vue'
import TiaraAiCheckoutPage from '../views/TiaraAICheckoutPage.vue'
import Orders from '../views/Orders.vue'
import Notifications from '../views/Notifications.vue'
import AIScanUpload from '../views/AIScanUpload.vue'
import type { TiaraActiveSubscription } from '../interfaces/TiaraActiveSubscription'
import api from '../utils/Api'
import { showErrorToast } from '../utils/helpers'
import PrivacyPolicy from '../views/PrivacyPolicy.vue'
import RefundExchangePolicy from '../views/RefundExchangePolicy.vue'
import ShippingPolicy from '../views/ShippingPolicy.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/login',
      component: Login,
      beforeEnter: (to, from, next) => {
        const token = localStorage.getItem('token')
        if (token != "null") {
          window.location.replace('/')
        } else {
          next()
        }
      }

    },
    {
      path: '/signup',
      component: Signup,
      beforeEnter: (to, from, next) => {
        const token = localStorage.getItem('token')
        if (token != "null") {
          window.location.replace('/')
        } else {
          next()
        }
      }
    },
    {
      path: '/products',
      name: 'products',
      component: ProductList
    },
    {
      path: '/account',
      name: 'account',
      component: Account,
      beforeEnter: (to, from, next) => {
        debugger;
        const token = localStorage.getItem('token')
        if (token === "null") {
          next('/login')
        } else {
          next()
        }
      },
    },
    {
      path: '/products/:id',
      name: 'product-detail',
      component: ProductDetail,
      props: true
    },
    {
      path: '/cart',
      name: 'cart',
      component: Cart
    },
    {
      path: '/checkout',
      name: 'checkout',
      component: Checkout
    },
    // {
    //   path: '/about',
    //   name: 'about',
    //   component: AboutUs
    //  },
    {
      path: '/contact',
      name: 'contact',
      component: Contact
    },
    {
      path: '/payment/confirmation',
      name: 'payment-confirmation',
      component: PaymentConfirmation
    },
    {
      path: '/my-orders',
      name: 'my-orders',
      component: Orders
    },
    {
      path: '/notifications',
      name: 'notifications',
      component: Notifications
    },

    {
      path: '/training',
      name: 'dental-training',
      component: DentalTraining
    },
    {
      path: '/ai',
      name: 'tiara-ai',
      component: TiaraAi,
      beforeEnter: async (to, from, next) => {
        debugger;
        const userId = localStorage.getItem("userId");
        let tiaraAiSubscription = localStorage.getItem("tiaraAiSubscription");

        if (userId && userId !== "0" && (!tiaraAiSubscription || tiaraAiSubscription !== "true")) {
          await checkTiaraAISubscription(userId);
          // Re-check the subscription status after the async call
          tiaraAiSubscription = localStorage.getItem("tiaraAiSubscription");
        }

        if (tiaraAiSubscription === "true") {
          next('/ai-scan-upload')
        }
        else {
          next()
        }
      }
    },
    {
      path: '/events',
      name: 'events',
      component: Events
    },
    {
      path: '/tiara-ai-checkout',
      name: 'tiara-ai-checkout',
      component: TiaraAiCheckoutPage
    },
    {
      path: '/ai-scan-upload',
      name: 'ai-scan-upload',
      component: AIScanUpload,
      beforeEnter: (to, from, next) => {
        const tiaraAiSubscription = localStorage.getItem("tiaraAiSubscription");

        const token = localStorage.getItem('token')
        if (token === "null" || !tiaraAiSubscription || tiaraAiSubscription === "false") {
          next('/login')
        } else {
          next()
        }
      },
    },

    {
      path: '/admin/login',
      component: () => import('../admin/views/LoginView.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/admin',
      component: () => import('../admin/views/AdminLayout.vue'),
      meta: { requiresAuth: true },
      children: [
        {
          path: '',
          redirect: '/admin/dashboard'
        },
        {
          path: 'dashboard',
          component: () => import('../admin/views/DashboardView.vue')
        },
        {
          path: 'categories',
          component: () => import('../admin/views/CategoriesView.vue')
        },
        {
          path: 'products',
          component: () => import('../admin/views/ProductsView.vue')
        },
        {
          path: 'orders',
          component: () => import('../admin/views/OrdersView.vue')
        },
        {
          path: 'users',
          component: () => import('../admin/views/UsersView.vue')
        },
        {
          path: 'notifications',
          component: () => import('../admin/views/Notifications.vue')
        },
        {
          path: 'payments',
          component: () => import('../admin/views/PaymentsView.vue')
        },
        {
          path: 'events',
          component: () => import('../admin/views/EventsView.vue')
        },
        {
          path: 'dental-training',
          component: () => import('../admin/views/DentalTrainingView.vue')
        },
        {
          path: 'promocodes',
          component: () => import('../admin/views/PromoCodesView.vue')
        },
        {
          path: 'tiaraai-subscriptions',
          component: () => import('../admin/views/TiaraAISubscriptionsView.vue')
        }
      ]
    },
    {
      path: '/forgot-password',
      name: 'ForgotPassword',
      component: () => import('../views/ForgotPassword.vue'),
    },
    {
      path: '/privacy-policy',
      name: 'privacy-policy',
      component: PrivacyPolicy
    },
    {
      path: '/refund-exchange-policy',
      name: 'refund-exchange-policy',
      component: RefundExchangePolicy
    },
    {
      path: '/shipping-policy',
      name: 'shipping-policy',
      component: ShippingPolicy
    },
  ],
  scrollBehavior(to, from, savedPosition) {
    // Always scroll to top when navigating to a new route
    return { top: 0 }
  }
})

router.beforeEach(async (to, from, next) => {
  if (!localStorage.getItem('token')) {
    localStorage.setItem('token', "null")
  }

  if (!localStorage.getItem('userId')) {
    localStorage.setItem('userId', '0')
  }

  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
  if (requiresAuth) {
    debugger;

    if (localStorage.getItem('token') !== 'null') {
      const isValid = await validateToken(localStorage.getItem('token') || '');
      if (!isValid) {
        return next('/login');
      }
    }
    if (localStorage.getItem('token') === 'null') {
      return next('/admin/login'); // Not logged in
    }
    if (localStorage.getItem('userRole') !== 'admin' && localStorage.getItem('userRole') !== 'Admin') {
      return next('/admin/login'); // Logged in but not an admin
    }
  }

  next()
})


const validateToken = async (token: string) => {
  try {
    const response = await api.post('/api/user/validateToken', token)
    if (response.status === 200) {
      return true;
    }
    else {
      return false;
    }

  } catch (error) {
    console.error("Error validating token:", error);
    showErrorToast("Unauthorized Access");

    setTimeout(() => {
      window.location.replace('/login');
    }, 1000);
  }

}

const checkTiaraAISubscription = async (userId: string) => {

  try {
    const response = await api.get<TiaraActiveSubscription>(`/api/tiaraaisubscription/user/${userId}/active`);
    if (response.status === 200) {
      const subscription = response.data;
      if (subscription.is_active) {
        console.log("Tiara AI subscription is active");
        localStorage.setItem("tiaraAiSubscription", subscription.is_active.toString());
        localStorage.setItem("segmentationsAllowed", subscription.segmentations_allowed.toString());
      }
      else {
        console.log("Tiara AI subscription is not active");
        localStorage.setItem("tiaraAiSubscription", "false");
      }
    }

  }
  catch (error) {
    console.error("Error checking Tiara AI subscription:", error);
    localStorage.setItem("tiaraAiSubscription", "false");
  }
  return false;



}


export default router
