<template>
    <div class="px-2 sm:px-4 lg:px-8 py-8 w-full max-w-full mx-auto">
        <div class="mb-8">
            <h1 class="text-2xl md:text-3xl font-extrabold text-slate-800">TiaraAI Subscriptions</h1>
        </div>
        <div v-if="loading" class="flex justify-center py-12">
            <svg class="animate-spin h-8 w-8 text-cyan-500" xmlns="http://www.w3.org/2000/svg" fill="none"
                viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
            </svg>
        </div>
        <div v-else>
            <div v-if="error" class="text-red-500 mb-4">{{ error }}</div>
            <div v-if="subscriptions.length === 0" class="text-slate-500 text-center py-8">No TiaraAI subscriptions
                found.</div>
            <div v-else class="bg-white rounded-xl shadow border border-slate-200 overflow-x-auto">
                <table class="min-w-[1200px] w-full divide-y divide-slate-200">
                    <thead class="bg-slate-50">
                        <tr>
                            <th
                                class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                User Name</th>
                            <th
                                class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                Email</th>
                            <th
                                class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                Package Name</th>
                            <th
                                class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                Order ID</th>
                            <th
                                class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                Subscribed At</th>
                            <th
                                class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                Expires At</th>
                            <th
                                class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                Segmentations</th>
                            <th
                                class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                Status</th>
                            <th
                                class="px-6 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                Transactions</th>
                        </tr>
                    </thead>
                    <tbody class="bg-white divide-y divide-slate-200">
                        <template v-for="sub in subscriptions" :key="sub.id">
                            <tr>
                                <td class="px-6 py-4">{{ sub.user_name || sub.user_id }}</td>
                                <td class="px-6 py-4">{{ sub.email || '—' }}</td>
                                <td class="px-6 py-4">{{ getPackageName(sub.subscription_id) }}</td>
                                <td class="px-6 py-4">{{ sub.order_id }}</td>
                                <td class="px-6 py-4">{{ formatDate(sub.subscribed_at) }}</td>
                                <td class="px-6 py-4">{{ formatDate(sub.expires_at) }}</td>
                                <td class="px-6 py-4">{{ sub.segmentations_used }} / {{ sub.segmentations_allowed }}
                                </td>
                                <td class="px-6 py-4">
                                    <span :class="sub.is_active ? 'text-green-600 font-bold' : 'text-slate-500'">
                                        {{ sub.is_active ? 'Active' : 'Inactive' }}
                                    </span>
                                </td>
                                <td class="px-6 py-4">
                                    <button @click="toggleTransactions(sub.id)"
                                        class="text-cyan-700 hover:underline text-xs font-semibold">
                                        {{ expanded[sub.id] ? 'Hide' : 'Show' }} ({{ sub.transactions?.length || 0 }})
                                    </button>
                                </td>
                            </tr>
                            <tr v-if="expanded[sub.id]">
                                <td colspan="9" class="bg-slate-50 px-6 py-4">
                                    <div v-if="!sub.transactions || sub.transactions.length === 0"
                                        class="text-slate-400 text-sm">No transactions found for this user.</div>
                                    <div v-else>
                                        <table class="min-w-full divide-y divide-slate-200">
                                            <thead>
                                                <tr>
                                                    <th
                                                        class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                                        ID</th>
                                                    <th
                                                        class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                                        GUID</th>
                                                    <th
                                                        class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                                        User Uploaded STL Folder</th>
                                                    <th
                                                        class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                                        Date Created</th>
                                                    <th
                                                        class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                                        Status</th>
                                                    <th
                                                        class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                                        Dental Mesh STL Folder</th>
                                                    <th
                                                        class="px-4 py-2 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                                        Dental Mesh STL Viewer</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr v-for="tx in sub.transactions" :key="tx.id">
                                                    <td class="px-4 py-2">{{ tx.id }}</td>
                                                    <td class="px-4 py-2">{{ tx.transaction_guid }}</td>
                                                    <td class="px-4 py-2"><a :href="tx.s3_url" target="_blank"
                                                            class="text-cyan-700 hover:underline">File</a></td>
                                                    <td class="px-4 py-2">{{ formatDate(tx.date_created) }}</td>
                                                    <td class="px-4 py-2">{{ tx.status }}</td>
                                                    <td class="px-4 py-2">
                                                        <a v-if="tx.dental_mesh_response_stl_folder && tx.dental_mesh_response_stl_folder !== ''"
                                                            :href="tx.dental_mesh_response_stl_folder" target="_blank"
                                                            class="text-cyan-700 hover:underline">Download STL
                                                            Folder</a>
                                                        <span v-else>null</span>
                                                    </td>
                                                    <td class="px-4 py-2">
                                                        <a v-if="tx.dental_mesh_response_stl_viewer && tx.dental_mesh_response_stl_viewer !== ''"
                                                            :href="tx.dental_mesh_response_stl_viewer" target="_blank"
                                                            class="text-cyan-700 hover:underline">View STL</a>
                                                        <span v-else>null</span>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </template>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import api from '../../utils/Api';

interface Transaction {
    id: number;
    transaction_guid: string;
    s3_url: string;
    date_created: string;
    status: string;
    dental_mesh_response_stl_folder: string;
    dental_mesh_response_stl_viewer: string;
}

interface TiaraAISubscription {
    id: string;
    user_id: string;
    user_name?: string;
    email?: string;
    subscription_id: string;
    order_id: string;
    segmentations_used: number;
    segmentations_allowed: number;
    subscribed_at: string;
    expires_at: string;
    is_active: boolean;
    transactions?: Transaction[];
}

const subscriptions = ref<TiaraAISubscription[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const expanded = ref<Record<string, boolean>>({});

const subscriptionPlans = [
    { id: 1, name: 'Light Package', price: 2900, segmentations: 10 },
    { id: 2, name: 'Pro', price: 6100, segmentations: 25 },
    { id: 3, name: 'Premium', price: 9800, segmentations: 50 }
];

function getPackageName(subscriptionId: string) {
    const plan = subscriptionPlans.find(p => String(p.id) === String(subscriptionId));
    return plan ? plan.name : subscriptionId;
}

const fetchSubscriptions = async () => {
    loading.value = true;
    error.value = null;
    try {
        const res = await api.get('/api/tiaraaisubscription/all-with-users');
        subscriptions.value = res.data || [];
    } catch (e: any) {
        error.value = 'Failed to load TiaraAI subscriptions.';
        subscriptions.value = [];
    } finally {
        loading.value = false;
    }
};

const formatDate = (dateStr: string) => {
    if (!dateStr) return '—';
    const d = new Date(dateStr);
    if (isNaN(d.getTime())) return 'Invalid date';
    return d.toLocaleString();
};

const toggleTransactions = (id: string) => {
    expanded.value[id] = !expanded.value[id];
};

onMounted(fetchSubscriptions);
</script>