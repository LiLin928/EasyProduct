<template>
  <footer class="footer">
    <div class="footer-content">
      <div class="footer-section">
        <h3>{{ t('site.footer.about') }}</h3>
        <p>{{ t('site.footer.aboutDesc') }}</p>
      </div>

      <div class="footer-section">
        <h3>{{ t('site.footer.contact') }}</h3>
        <p>📧 support@easyproduct.com</p>
        <p>📞 +86 400-123-4567</p>
      </div>
    </div>

    <div class="footer-bottom">
      <p>© {{ year }} {{ t('common.app.name') }}. {{ t('site.footer.rights') }}</p>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const year = new Date().getFullYear()
</script>

<style scoped lang="scss">
@use '@/assets/styles/variables' as *;
@use '@/assets/styles/mixins' as *;

.footer {
  background: $color-text-primary;
  color: white;
  /* Reduce top padding to decrease overall height */
  padding: $spacing-xl 0 0;
  margin-top: auto;

  @include compact {
    padding: $spacing-lg 0 0;
  }
}

.footer-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 $spacing-xl;
  /* Two-column layout (About + Contact) */
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: $spacing-2xl;

  @include compact {
    grid-template-columns: 1fr;
    padding: 0 $spacing-md;
    gap: $spacing-lg;
  }
}

.footer-section {
  h3 {
    font-family: $font-heading;
    font-size: 18px;
    margin-bottom: $spacing-md;
    color: white;
  }

  p {
    font-size: 14px;
    color: rgb(255 255 255 / 70%);
    margin-bottom: $spacing-sm;
    line-height: 1.6;
  }

  ul {
    list-style: none;
    margin: 0;
    padding: 0;

    li {
      margin-bottom: $spacing-xs;

      a {
        display: inline-flex;
        align-items: center;
        min-height: 36px;
        color: rgb(255 255 255 / 70%);
        text-decoration: none;
        transition: color $transition-fast;

        &:hover {
          color: white;
        }
      }
    }
  }
}

.footer-bottom {
  text-align: center;
  /* Reduce padding and margin to decrease footer height */
  padding: $spacing-md;
  margin-top: $spacing-lg;
  border-top: 1px solid rgb(255 255 255 / 10%);

  @include compact {
    margin-top: $spacing-md;
    padding: $spacing-sm;
  }

  p {
    font-size: 13px;
    color: rgb(255 255 255 / 50%);
  }
}
</style>
