import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'
import './assets/fonts/sequel-sans.css'
import { createPinia } from 'pinia'
//import '@fortawesome/fontawesome-free/css/all.css'
import 'vue3-toastify/dist/index.css';

const pinia = createPinia();
createApp(App)
  .use(pinia)
  .use(router)
  .mount('#app')
