<template>
    <div class="container-custom py-12">
        <div class="policy-card">
            <h1 class="policy-title-ar">سياسة الاسترجاع والاستبدال</h1>
            <ul class="policy-list-ar">
                <li>نقبل الإرجاع إذا كان المنتج تالفًا أو به عيب (يجب الإبلاغ خلال 3 أيام مع صورة).</li>
                <li>في حال استلام منتج خاطئ (يجب الإبلاغ خلال 48 ساعة).</li>
                <li class="font-semibold mt-2">الخطوات:</li>
                <li>1. راسلنا عبر البريد info@tiarapro.com للموافقة على الإرجاع.</li>
                <li>2. أرسل المنتج (تتحمل تكلفة الشحن إلا إذا كان الخطأ منا).</li>
                <li>3. يتم معالجة الاسترجاع/الاستبدال خلال 5-7 أيام.</li>
                <li>الاسترداد: إلى وسيلة الدفع الأصلية أو رصيد متجر.</li>
            </ul>
        </div>
        <div class="policy-card mt-8">
            <h2 class="policy-title-en">Refund & Exchange Policy</h2>
            <ul class="policy-list-en">
                <li>We Accept Returns If:</li>
                <li>• Product is damaged/defective (report within 3 days with photo proof).</li>
                <li>• Wrong item received (report within 48 hours).</li>
                <li class="font-semibold mt-2">Process:</li>
                <li>1. Email us at info@tiarapro.com for return approval.</li>
                <li>2. Send the item back (you pay return shipping unless our mistake).</li>
                <li>3. Refund/exchange processed in 5-7 days.</li>
                <li>Refunds: Back to original payment method or store credit.</li>
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