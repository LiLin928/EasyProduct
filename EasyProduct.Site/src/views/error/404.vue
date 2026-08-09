<template>
  <div class="not-found">
    <h1>404</h1>
    <router-link to="/">
      {{ t('site.notFound.back') }}
    </router-link>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
const { t } = useI18n()
</script>

<style scoped lang="scss">
.not-found {
  padding: $spacing-xl * 2;
  text-align: center;
}
</style>
