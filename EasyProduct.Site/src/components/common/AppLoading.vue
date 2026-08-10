<!-- src/components/common/AppLoading.vue -->
<template>
  <div
    v-if="loading"
    class="app-loading"
  >
    <div class="app-loading__spinner" />
    <p class="app-loading__text">
      {{ t('common.loading') }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

defineProps<{ loading: boolean }>()
</script>

<style scoped lang="scss">
.app-loading {
  padding: $spacing-xl;
  text-align: center;

  &__spinner {
    width: 32px;
    height: 32px;
    margin: 0 auto $spacing-md;
    border: 3px solid $color-border;
    border-top-color: $color-primary;
    border-radius: 50%;
    animation: spin 1s linear infinite;
  }

  &__text {
    color: $color-text-secondary;
  }
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>