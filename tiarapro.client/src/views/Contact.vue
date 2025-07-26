<script setup lang="ts">
import { ref } from 'vue';
// import emailjs from 'emailjs-com';

const name = ref('');
const email = ref('');
const message = ref('');
const status = ref('');

const sendEmail = async () => {
    try {
        const templateParams = {
            from_name: name.value,
            from_email: email.value,
            message: message.value,
        };

        const serviceID = 'your_service_id';
        const templateID = 'your_template_id';
        const userID = 'your_public_key';

        // await emailjs.send(serviceID, templateID, templateParams, userID);
        status.value = 'Message sent successfully!';
        name.value = '';
        email.value = '';
        message.value = '';
    } catch (error) {
        console.error(error);
        status.value = 'Failed to send message. Please try again.';
    }
};
</script>

<template>
    <div class="max-w-xl mx-auto p-4">
        <h1 class="text-2xl font-bold mb-4">Contact Us</h1>
        <form @submit.prevent="sendEmail" class="space-y-4">
            <div>
                <label class="block mb-1">Name</label>
                <input v-model="name" type="text" required class="w-full border p-2 rounded" />
            </div>
            <div>
                <label class="block mb-1">Email</label>
                <input v-model="email" type="email" required class="w-full border p-2 rounded" />
            </div>
            <div>
                <label class="block mb-1">Message</label>
                <textarea v-model="message" required rows="5" class="w-full border p-2 rounded"></textarea>
            </div>
            <button type="submit" class="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600">
                Send
            </button>
            <p v-if="status" class="mt-2 text-sm text-green-600">{{ status }}</p>
        </form>
    </div>
</template>

<style scoped>
/* You can enhance styles with Tailwind or custom CSS */
</style>
