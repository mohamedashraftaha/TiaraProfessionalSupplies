<script setup lang="ts">
import { ref, onMounted } from 'vue';

const countdown = ref({ days: 0, hours: 0, minutes: 0, seconds: 0 });

const targetDate = new Date('2025-04-01T00:00:00').getTime(); // Set your launch date

const updateCountdown = () => {
    const now = new Date().getTime();
    const timeLeft = targetDate - now;

    if (timeLeft > 0) {
        countdown.value = {
            days: Math.floor(timeLeft / (1000 * 60 * 60 * 24)),
            hours: Math.floor((timeLeft % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)),
            minutes: Math.floor((timeLeft % (1000 * 60 * 60)) / (1000 * 60)),
            seconds: Math.floor((timeLeft % (1000 * 60)) / 1000),
        };
    } else {
        countdown.value = { days: 0, hours: 0, minutes: 0, seconds: 0 };
    }
};

onMounted(() => {
    updateCountdown();
    setInterval(updateCountdown, 1000);
});
</script>

<template>
    <div class="flex flex-col items-center justify-center min-h-screen bg-gray-100 text-center">
        <h1 class="text-4xl font-bold text-gray-800">🚀 Coming Soon</h1>
        <p class="text-lg text-gray-600 mt-2">We're working hard to launch something amazing.</p>

        <div class="mt-6 flex space-x-4 text-white">
            <div class="p-4 bg-primary rounded-lg">
                <span class="text-2xl font-bold">{{ countdown.days }}</span>
                <p class="text-sm">Days</p>
            </div>
            <div class="p-4 bg-primary rounded-lg">
                <span class="text-2xl font-bold">{{ countdown.hours }}</span>
                <p class="text-sm">Hours</p>
            </div>
            <div class="p-4 bg-primary rounded-lg">
                <span class="text-2xl font-bold">{{ countdown.minutes }}</span>
                <p class="text-sm">Minutes</p>
            </div>
            <div class="p-4 bg-primary rounded-lg">
                <span class="text-2xl font-bold">{{ countdown.seconds }}</span>
                <p class="text-sm">Seconds</p>
            </div>
        </div>

        <p class="mt-6 text-gray-600">Stay tuned for updates!</p>
    </div>
</template>

<style scoped>
.bg-primary {
    background-color: #ff6b6b;
    /* Customize this to match your brand */
}
</style>
