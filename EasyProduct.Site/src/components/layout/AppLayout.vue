<template>
  <div class="app-layout">
    <AppNavbar />
    <main class="app-layout__main">
      <router-view />
    </main>
    <AppFooter />
  </div>
</template>

<script setup lang="ts">
import AppFooter from './AppFooter.vue'
import AppNavbar from './AppNavbar.vue'
</script>

<style scoped lang="scss">
@use '@/assets/styles/variables' as *;
@use '@/assets/styles/mixins' as *;

.app-layout {
  display: flex;
  flex-direction: column;
  min-height: 100vh;

  &__main {
    flex: 1;
    padding-top: 80px; // 导航栏高度

    @include compact {
      padding-top: 56px;
    }
  }
}
</style>
