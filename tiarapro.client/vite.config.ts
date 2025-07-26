import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    port: 3000,
    allowedHosts: ['s78b6r5jagfc.share.zrok.io'],
  },
})
