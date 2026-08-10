<!-- src/components/common/AppEmpty.vue -->
<template>
  <div class="app-empty">
    <div class="app-empty__icon">
      📭
    </div>
    <p class="app-empty__text">
      {{ text || t('common.empty') }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

defineProps<{ text?: string }>()
</script>

<style scoped lang="scss">
.app-empty {
  padding: $spacing-xl * 2;
  text-align: center;

  &__icon {
    font-size: 48px;
    margin-bottom: $spacing-md;
  }

  &__text {
    color: $color-text-secondary;
  }
}
</style>