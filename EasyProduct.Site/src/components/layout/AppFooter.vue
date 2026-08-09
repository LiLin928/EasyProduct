<template>
  <footer class="app-footer">
    <div class="app-footer__inner">
      © {{ year }} {{ t('common.app.name') }} · {{ t('common.footer.rights') }}
    </div>
  </footer>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import dayjs from 'dayjs'

const { t } = useI18n()
const year = dayjs().year()
</script>

<style scoped lang="scss">
.app-footer {
  background: $color-bg-soft;
  border-top: 1px solid $color-border;
  color: $color-text-secondary;

  &__inner {
    @include site-container;
    padding-top: $spacing-lg;
    padding-bottom: $spacing-lg;
    text-align: center;
  }
}
</style>