import { defineStore } from "pinia";
import { ref, computed } from "vue";

interface CartItem {
  id: number;
  name: string;
  price: number;
  image: string;
  quantity: number;
  sku?: string;
  side?: number;
  size?: number;
}

interface PromoCode {
  code: string;
  discount_amount: number;
  promo_code_id: number;
}

export const useCartStore = defineStore("cart", () => {
  const cart = ref<CartItem[]>([]);
  const promoCode = ref<PromoCode | null>(null);

  // Compute total items in the cart
  const totalItems = computed(() => cart.value.reduce((sum, item) => sum + item.quantity, 0));

  // Add to cart
  const addToCart = (product: CartItem) => {
    console.log("PRODUCT", product);
    if (product.sku) {
      // For variants, use id+sku as unique key
      const existingItem = cart.value.find((item) => item.id === product.id && item.sku === product.sku);
      if (existingItem) {
        if (product.quantity > 1) {
          existingItem.quantity += product.quantity;
        } else {
          existingItem.quantity++;
        }
      } else {
        cart.value.push({ ...product, quantity: product.quantity });
      }
    } else {
      // For non-variants, use id only
      const existingItem = cart.value.find((item) => item.id === product.id);
      if (existingItem) {
        existingItem.quantity++;
      } else {
        cart.value.push({ ...product, quantity: product.quantity });
      }
    }
  };

  // Remove from cart
  const removeFromCart = (productId: number) => {
    const index = cart.value.findIndex((item) => item.id === productId);
    if (index !== -1) {
      if (cart.value[index].quantity > 1) {
        cart.value[index].quantity--;
      } else {
        cart.value.splice(index, 1);
      }
    }
  };

  // Clear cart
  const clearCart = () => {
    cart.value = [];
    promoCode.value = null;
  };

  const setPromoCode = (promo: PromoCode | null) => {
    promoCode.value = promo;
  };

  return {
    cart,
    totalItems,
    addToCart,
    removeFromCart,
    clearCart,
    setPromoCode,
    promoCode
  };
});
