<template>
    <div class="container-custom py-12">
        <div class="policy-card">
            <h1 class="policy-title-ar">سياسة الخصوصية</h1>
            <ul class="policy-list-ar">
                <li>نقوم بجمع اسمك، عنوانك، رقم هاتفك، وتفاصيل الدفع لمعالجة الطلبات.</li>
                <li>بياناتك آمنة — لا نقوم بتخزين معلومات بطاقة الائتمان.</li>
                <li>نشارك التفاصيل فقط مع شركاء التوصيل للشحن.</li>
                <li>لا رسائل مزعجة — يمكنك إلغاء الاشتراك في أي وقت.</li>
                <li>للاستفسارات: info@tiarapro.com أو +20 24500090</li>
            </ul>
        </div>
        <div class="policy-card mt-8">
            <h2 class="policy-title-en">Privacy Policy</h2>
            <ul class="policy-list-en">
                <li>We collect your name, address, phone, and payment details to process orders.</li>
                <li>Your data is secure—we do not store credit card info.</li>
                <li>We only share details with delivery partners for shipping.</li>
                <li>No spam—you can unsubscribe from emails anytime.</li>
                <li>Questions? Contact: info@tiarapro.com or +20 24500090</li>
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