<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '../../utils/Api'
import { showErrorToast, showSuccessToast } from '../../utils/helpers'

const promoCodes = ref<any[]>([])
const loading = ref(false)
const editingId = ref<number | null>(null)
const newPromo = ref({
    code: '',
    discount_amount: 0,
    start_date: '',
    end_date: '',
    is_active: true,
    max_uses: 1,
    minimum_order_amount: null
})

const fetchPromoCodes = async () => {
    loading.value = true
    try {
        const res = await api.get('/api/promocode')
        promoCodes.value = res.data || []
    } catch (e) {
        console.error('Failed to fetch promo codes')
    } finally {
        loading.value = false
    }
}

const startEdit = (promo: any) => {
    editingId.value = promo.id
    newPromo.value = {
        code: promo.code,
        discount_amount: promo.discount_amount,
        start_date: promo.start_date.substring(0, 10),
        end_date: promo.end_date.substring(0, 10),
        is_active: promo.is_active,
        max_uses: promo.max_uses,
        minimum_order_amount: promo.minimum_order_amount
    }
}

const cancelEdit = () => {
    editingId.value = null
    resetForm()
}

const resetForm = () => {
    newPromo.value = {
        code: '',
        discount_amount: 0,
        start_date: '',
        end_date: '',
        is_active: true,
        max_uses: 1,
        minimum_order_amount: null
    }
}

const addPromoCode = async () => {
    if (!newPromo.value.code || !newPromo.value.discount_amount || !newPromo.value.start_date || !newPromo.value.end_date) {
        showErrorToast('Please fill all required fields')
        return
    }
    if (newPromo.value.discount_amount < 1 || newPromo.value.discount_amount > 99) {
        showErrorToast('Discount amount must be between 1 and 99')
        return
    }
    try {
        if (editingId.value) {
            const res = await api.put(`/api/promocode/${editingId.value}`, newPromo.value)
            if (res.status !== 200) {
                showErrorToast(res.data.message)
                return
            }
            showSuccessToast('Promo code updated!')
        } else {
            const res = await api.post('/api/promocode', newPromo.value)
            if (res.status !== 200) {
                showErrorToast(res.data.message)
                return
            }
            showSuccessToast('Promo code added!')
        }
        fetchPromoCodes()
        editingId.value = null
        resetForm()
    } catch (e) {
        showErrorToast(editingId.value ? 'Failed to update promo code' : 'Failed to add promo code')
    }
}

const deletePromoCode = async (id: number) => {
    if (!confirm('Are you sure you want to delete this promo code?')) return
    try {
        await api.delete(`/api/promocode/${id}`)
        showSuccessToast('Promo code deleted!')
        fetchPromoCodes()
    } catch (e) {
        showErrorToast('Failed to delete promo code')
    }
}

onMounted(fetchPromoCodes)
</script>

<template>
    <div>
        <h2 class="text-2xl font-bold mb-6">Promo Codes Dashboard</h2>
        <div class="bg-white p-6 rounded-lg shadow mb-8">
            <h3 class="font-semibold mb-4">{{ editingId ? 'Edit Promo Code' : 'Add New Promo Code' }}</h3>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Promo Code</label>
                    <input v-model="newPromo.code" placeholder="Enter promo code" class="input" />
                </div>
                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Discount Percentage</label>
                    <div class="flex items-center">
                        <input v-model.number="newPromo.discount_amount" type="number" min="1" max="99" maxlength="2"
                            placeholder="Enter discount %" class="input" />
                        <span class="ml-2">%</span>
                    </div>
                </div>
                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Start Date</label>
                    <input v-model="newPromo.start_date" type="date" class="input" />
                </div>
                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">End Date</label>
                    <input v-model="newPromo.end_date" type="date" class="input" />
                </div>
                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Minimum Order Amount</label>
                    <input v-model.number="newPromo.minimum_order_amount" type="number" placeholder="Optional"
                        class="input" />
                </div>
                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Status</label>
                    <label class="flex items-center mt-2">
                        <input v-model="newPromo.is_active" type="checkbox" class="mr-2" /> Active
                    </label>
                </div>
            </div>
            <div class="flex gap-2">
                <button @click="addPromoCode" class="btn btn-primary">
                    {{ editingId ? 'Update Promo Code' : 'Add Promo Code' }}
                </button>
                <button v-if="editingId" @click="cancelEdit" class="btn btn-secondary">Cancel</button>
            </div>
        </div>

        <div class="bg-white p-6 rounded-lg shadow">
            <h3 class="font-semibold mb-4">All Promo Codes</h3>
            <div class="overflow-x-auto">
                <table class="min-w-full divide-y divide-gray-200">
                    <thead class="bg-gray-50">
                        <tr>
                            <th scope="col"
                                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                Code</th>
                            <th scope="col"
                                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                Discount</th>
                            <th scope="col"
                                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                Validity Period</th>
                            <th scope="col"
                                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                Status</th>
                            <th scope="col"
                                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                Min Order</th>
                            <th scope="col"
                                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                Usage</th>
                            <th scope="col"
                                class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                Actions</th>
                        </tr>
                    </thead>
                    <tbody class="bg-white divide-y divide-gray-200">
                        <tr v-for="promo in promoCodes" :key="promo.id" class="hover:bg-gray-50">
                            <td class="px-6 py-4 whitespace-nowrap">
                                <div class="text-sm font-medium text-gray-900">{{ promo.code }}</div>
                            </td>
                            <td class="px-6 py-4 whitespace-nowrap">
                                <div class="text-sm text-gray-900">{{ promo.discount_amount }}%</div>
                            </td>
                            <td class="px-6 py-4 whitespace-nowrap">
                                <div class="text-sm text-gray-900">
                                    {{ promo.start_date?.substring(0, 10) }} to {{ promo.end_date?.substring(0, 10) }}
                                </div>
                            </td>
                            <td class="px-6 py-4 whitespace-nowrap">
                                <span :class="[
                                    promo.is_active ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800',
                                    'px-2 inline-flex text-xs leading-5 font-semibold rounded-full'
                                ]">
                                    {{ promo.is_active ? 'Active' : 'Inactive' }}
                                </span>
                            </td>
                            <td class="px-6 py-4 whitespace-nowrap">
                                <div class="text-sm text-gray-900">
                                    {{ promo.minimum_order_amount ? `$${promo.minimum_order_amount}` : '-' }}
                                </div>
                            </td>
                            <td class="px-6 py-4 whitespace-nowrap">
                                <div class="text-sm text-gray-900">
                                    <span :class="[
                                        promo.current_uses >= promo.max_uses ? 'text-red-600' : 'text-gray-900',
                                        'font-medium'
                                    ]">
                                        {{ promo.current_uses }}/{{ promo.max_uses }}
                                    </span>
                                </div>
                            </td>
                            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                <div class="flex justify-end gap-2">
                                    <button @click="startEdit(promo)"
                                        class="text-indigo-600 hover:text-indigo-900 bg-indigo-50 hover:bg-indigo-100 px-3 py-1 rounded-md transition-colors">
                                        Edit
                                    </button>
                                    <button @click="deletePromoCode(promo.id)"
                                        class="text-red-600 hover:text-red-900 bg-red-50 hover:bg-red-100 px-3 py-1 rounded-md transition-colors">
                                        Delete
                                    </button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div v-if="loading" class="text-center py-4">
                <div
                    class="inline-block animate-spin rounded-full h-8 w-8 border-4 border-primary border-t-transparent">
                </div>
                <p class="mt-2 text-gray-500">Loading promo codes...</p>
            </div>
            <div v-if="!loading && promoCodes.length === 0" class="text-center py-8">
                <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                <h3 class="mt-2 text-sm font-medium text-gray-900">No promo codes</h3>
                <p class="mt-1 text-sm text-gray-500">Get started by creating a new promo code.</p>
            </div>
        </div>
    </div>
</template>

<style scoped>
.input {
    @apply border rounded px-3 py-2 w-full;
}

.btn {
    @apply px-4 py-2 rounded text-white transition;
}

.btn-primary {
    @apply bg-primary hover:bg-primary/90;
}

.btn-secondary {
    @apply bg-gray-600 hover:bg-gray-700;
}

.btn-danger {
    @apply bg-red-600 hover:bg-red-700;
}

.btn-sm {
    @apply px-2 py-1 text-xs;
}
</style>
