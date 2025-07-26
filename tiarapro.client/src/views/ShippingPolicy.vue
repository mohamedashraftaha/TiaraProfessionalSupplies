<template>
    <div class="container-custom py-12">
        <div class="policy-card">
            <h1 class="policy-title-ar">سياسة الشحن</h1>
            <ul class="policy-list-ar">
                <li>التوصيل داخل مصر:</li>
                <li>• تجهيز الطلب: 1-2 يوم (بدون عطلات نهاية الأسبوع).</li>
                <li>• التوصيل: 2-5 أيام (القاهرة/المدن الكبرى)، حتى 7 أيام في أماكن أخرى.</li>
                <li>تتبع الشحنة: سيتم إرسال رابط التتبع عبر البريد أو الرسائل.</li>
                <li>في حال فشل التوصيل: سيحاول المندوب 2-3 مرات قبل إعادة الطلب.</li>
                <li>للمساعدة: info@tiarapro.com أو +20 24500090</li>
            </ul>
        </div>
        <div class="policy-card mt-8">
            <h2 class="policy-title-en">Shipping Policy</h2>
            <ul class="policy-list-en">
                <li>Delivery in Egypt:</li>
                <li>• Processing: 1-2 days (not counting weekends).</li>
                <li>• Delivery: 2-5 days (Cairo/major cities), up to 7 days elsewhere.</li>
                <li>Tracking: You'll get a tracking link via email/SMS.</li>
                <li>Failed Delivery? Courier will try 2-3 times before returning the order.</li>
                <li>Need help? Contact: info@tiarapro.com or +20 24500090</li>
            </ul>
        </div>
    </div>
</template>

<script setup>
// No script needed
</script>

<style scoped>
.container-custom {
    max-width: 700px;
    margin: 0 auto;
}

.policy-card {
    background: #fff;
    border-radius: 1rem;
    box-shadow: 0 2px 16px 0 rgba(0, 0, 0, 0.07);
    padding: 2rem 2rem 1.5rem 2rem;
    margin-bottom: 1.5rem;
}

.policy-title-ar {
    font-size: 1.5rem;
    font-weight: bold;
    color: #1e293b;
    text-align: right;
    margin-bottom: 1rem;
}

.policy-title-en {
    font-size: 1.25rem;
    font-weight: 600;
    color: #1e293b;
    margin-bottom: 1rem;
}

.policy-list-ar {
    direction: rtl;
    text-align: right;
    color: #334155;
    line-height: 2;
    font-size: 1.05rem;
    padding-right: 1.2rem;
}

.policy-list-en {
    color: #334155;
    line-height: 2;
    font-size: 1.05rem;
    padding-left: 1.2rem;
}
</style>