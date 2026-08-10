<!-- src/components/common/AppCarousel.vue -->
<template>
  <div class="app-carousel">
    <div
      v-for="(item, index) in items"
      v-show="index === currentIndex"
      :key="index"
      class="app-carousel__item"
    >
      <img
        :src="item.imageUrl"
        :alt="item.title"
        class="app-carousel__image"
      >
    </div>
    <div
      v-if="items.length > 1"
      class="app-carousel__dots"
    >
      <button
        v-for="(_, index) in items"
        :key="index"
        class="app-carousel__dot"
        :class="{ 'app-carousel__dot--active': index === currentIndex }"
        @click="currentIndex = index"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

interface CarouselItem {
  imageUrl: string
  title: string
}

const props = defineProps<{ items: CarouselItem[] }>()

const currentIndex = ref(0)
let timer: ReturnType<typeof setInterval> | null = null

onMounted(() => {
  if (props.items.length > 1) {
    timer = setInterval(() => {
      currentIndex.value = (currentIndex.value + 1) % props.items.length
    }, 5000)
  }
})

onUnmounted(() => {
  if (timer) clearInterval(timer)
})
</script>

<style scoped lang="scss">
.app-carousel {
  position: relative;
  width: 100%;
  border-radius: $radius-md;
  overflow: hidden;

  &__item {
    width: 100%;
  }

  &__image {
    width: 100%;
    display: block;
  }

  &__dots {
    position: absolute;
    bottom: $spacing-md;
    left: 50%;
    transform: translateX(-50%);
    display: flex;
    gap: $spacing-xs;
  }

  &__dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    border: none;
    background: rgba(255, 255, 255, 0.5);
    cursor: pointer;
    transition: background 0.3s;

    &--active {
      background: $color-primary;
    }

    &:hover {
      background: rgba(255, 255, 255, 0.8);
    }
  }
}
</style>