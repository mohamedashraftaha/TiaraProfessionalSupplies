<script setup lang="ts">
  import { ref, computed, onMounted } from 'vue'
  import api from '../utils/Api.ts'
  import { showErrorToast, showSuccessToast, categoriesConst } from '../utils/helpers.ts'

  interface UserNotification {
    id: number // userNotificationId
    isRead: boolean
    notification: {
      id: number
      title: string
      message: string
      category: number
      createdAt: string
    }
    icon?: string
    categoryName?: string
  }

  const notifications = ref<UserNotification[]>([])
  const userId = localStorage.getItem('userId')

  const categoryMap: Record<number, { name: string, icon: string }> = {
    0: { name: 'New Product', icon: 'fas fa-box' },
    1: { name: 'New Feature', icon: 'fas fa-star' },
    2: { name: 'New Event', icon: 'fas fa-calendar' },
    3: { name: 'New Training', icon: 'fas fa-chalkboard-teacher' },
    4: { name: 'Tiara AI Updates', icon: 'fas fa-brain' }
  }

  const fetchNotifications = async () => {
    try {
      const response = await api.get(`/api/notifications/user/${userId}`)
      notifications.value = response.data.map((un: any) => {
        const categoryInfo = categoryMap[un.notification.category] || { name: 'Unknown', icon: 'fas fa-bell' }
        // Fallback for createdAt
        const createdAt = un.notification.createdAt || un.notification.created_at || ''
        return {
          ...un,
          icon: categoryInfo.icon,
          categoryName: categoryInfo.name,
          notification: {
            ...un.notification,
            createdAt
          }
        }
      })
    } catch (error) {
      showErrorToast('Failed to fetch notifications')
    }
  }

  onMounted( async () => {

    await fetchNotifications()

  }
  )

  const formatDate = (dateString: string) => {
    const date = new Date(dateString)
    const now = new Date()
    const diffInHours = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60))
    if (diffInHours < 24) {
      return diffInHours === 0
        ? 'Just now'
        : `${diffInHours} hour${diffInHours === 1 ? '' : 's'} ago`
    }
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      hour: 'numeric',
      minute: '2-digit'
    })
  }

  // Emit event to update unread count in Navbar
  const emitUnreadUpdate = () => {
    window.dispatchEvent(new CustomEvent('refresh-unread-count'))
  }

  const markAsRead = async (userNotificationId: number) => {
    try {
      await api.post(`/api/notifications/user-notification/${userNotificationId}/read`)
      const userNotification = notifications.value.find(n => n.id === userNotificationId)
      if (userNotification) userNotification.isRead = true
      emitUnreadUpdate()
    } catch {
      showErrorToast('Failed to mark as read')
    }
  }

  const markAllAsRead = async () => {
    await Promise.all(
      notifications.value.filter(n => !n.isRead).map(n => markAsRead(n.id))
    )
    emitUnreadUpdate()
  }

  const deleteNotification = (userNotificationId: number) => {
    notifications.value = notifications.value.filter(n => n.id !== userNotificationId)
  }

  const clearAll = () => {
    notifications.value = []
  }

  const unreadCount = computed(() => notifications.value.filter(n => !n.isRead).length)
</script>


<template>
  <div class="min-h-screen bg-gray-50">

    <main class="container mx-auto px-4 py-8">
      <!-- Header -->
      <div class="flex justify-between items-center mb-8">
        <div>
          <h1 class="text-2xl font-bold">Notifications</h1>
          <p class="text-gray-600">Stay updated with your latest activities</p>
        </div>
        <div class="flex gap-4">
          <button @click="markAllAsRead"
                  class="text-[#4052B5] hover:underline"
                  :disabled="unreadCount === 0">
            Mark all as read
          </button>
          <button @click="clearAll"
                  class="text-red-600 hover:underline"
                  :disabled="notifications.length === 0">
            Clear all
          </button>
        </div>
      </div>

      <!-- Notifications List -->
      <div class="bg-white rounded-lg shadow-sm overflow-hidden">
        <div v-if="notifications.length === 0" class="p-8 text-center text-gray-500">
          <i class="fas fa-bell-slash text-4xl mb-4"></i>
          <p>No notifications to display</p>
        </div>

        <div v-else class="divide-y divide-gray-200">
          <div v-for="notification in notifications"
               :key="notification.id"
               class="p-6 hover:bg-gray-50 transition-colors"
               :class="{ 'bg-blue-50/50': !notification.isRead }">
            <div class="flex items-start gap-4">
              <div class="w-10 h-10 rounded-full flex items-center justify-center"
                   :class="{
                  'bg-blue-100 text-blue-600': notification.notification.category === 0,
                  'bg-purple-100 text-purple-600': notification.notification.category === 3,
                  'bg-green-100 text-green-600': notification.notification.category === 1,
                  'bg-orange-100 text-orange-600': notification.notification.category === 2,
                  'bg-blue-100 text-blue-500': notification.notification.category === 4,

                }">
                <i :class="notification.icon"></i>
              </div>

              <div class="flex-1">
                <div class="flex items-start justify-between">
                  <div>
                    <h3 class="font-semibold">{{ notification.notification.title }}</h3>
                    <p class="text-gray-600 mt-1">{{ notification.notification.message }}</p>
                  </div>
                  <div class="flex items-center gap-4">
                    <span class="text-sm text-gray-500">{{ formatDate(notification.notification.createdAt) }}</span>
                    <div class="flex gap-2">
                      <button v-if="!notification.isRead"
                              @click="markAsRead(notification.id)"
                              class="text-[#4052B5] hover:text-[#4052B5]/80"
                              title="Mark as read">
                        <i class="fas fa-check"></i>
                      </button>
                      <button @click="deleteNotification(notification.id)"
                              class="text-red-600 hover:text-red-700"
                              title="Delete notification">
                        <i class="fas fa-trash"></i>
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

  </div>
</template>
