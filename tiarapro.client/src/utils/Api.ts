// src/utils/api.ts
import axios from 'axios'
import type { AxiosInstance } from 'axios' // ✅ use "type" when importing types

const api: AxiosInstance = axios.create({
  baseURL: 'https://tiarapro.com',
  headers: {
    'Content-Type': 'application/json'
  }
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  response => response,
  error => {
    console.error('API error:', error.response || error.message)
    return Promise.reject(error)
  }
)

export default api
