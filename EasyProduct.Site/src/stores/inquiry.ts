// src/stores/inquiry.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

interface InquiryItem {
  productId: string
  productName: string
  quantity: number
  unit: string
  remark?: string
}

export const useInquiryStore = defineStore('inquiry', () => {
  const items = ref<InquiryItem[]>([])

  const totalItems = computed(() =>
    items.value.reduce((sum, item) => sum + item.quantity, 0)
  )

  function addItem(item: InquiryItem) {
    const existing = items.value.find(i => i.productId === item.productId)
    if (existing) {
      existing.quantity += item.quantity
    } else {
      items.value.push(item)
    }
  }

  function removeItem(productId: string) {
    items.value = items.value.filter(i => i.productId !== productId)
  }

  function updateQuantity(productId: string, quantity: number) {
    const item = items.value.find(i => i.productId === productId)
    if (item) {
      item.quantity = quantity
    }
  }

  function clearItems() {
    items.value = []
  }

  return {
    items,
    totalItems,
    addItem,
    removeItem,
    updateQuantity,
    clearItems,
  }
})