<!-- src/components/common/AppSkeleton.vue -->
<template>
  <div class="app-skeleton">
    <div
      v-for="i in count"
      :key="i"
      class="app-skeleton__item"
    >
      <div class="app-skeleton__image" />
      <div class="app-skeleton__text" />
      <div class="app-skeleton__text app-skeleton__text--short" />
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{ count: number }>()
</script>

<style scoped lang="scss">
.app-skeleton {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: $spacing-md;

  &__item {
    padding: $spacing-md;
    background: $color-bg-card;
    border-radius: $radius-md;
  }

  &__image {
    width: 100%;
    height: 150px;
    margin-bottom: $spacing-sm;
    background: linear-gradient(90deg, $color-bg-soft 25%, lighten($color-bg-soft, 5%) 50%, $color-bg-soft 75%);
    background-size: 200% 100%;
    animation: skeleton-loading 1.5s infinite;
    border-radius: $radius-sm;
  }

  &__text {
    height: 16px;
    margin-bottom: $spacing-xs;
    background: $color-bg-soft;
    border-radius: $radius-sm;

    &--short {
      width: 60%;
    }
  }
}

@keyframes skeleton-loading {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}
</style>