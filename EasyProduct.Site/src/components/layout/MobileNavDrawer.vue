<template>
  <Teleport to="body">
    <div
      class="mobile-nav"
      :class="{ open: open }"
    >
      <div
        class="mobile-nav__overlay"
        aria-hidden="true"
        @click="emit('close')"
      />

      <aside
        id="mobile-nav-drawer"
        ref="drawerRef"
        class="mobile-nav__drawer"
        role="dialog"
        aria-modal="true"
        :aria-label="t('nav.menu')"
        tabindex="-1"
      >
        <div class="mobile-nav__header">
          <span class="mobile-nav__title">{{ t('nav.menu') }}</span>
          <button
            type="button"
            class="mobile-nav__close"
            :aria-label="t('nav.closeMenu')"
            @click="emit('close')"
          >
            <span
              class="icon-close"
              aria-hidden="true"
            />
          </button>
        </div>

        <nav
          class="mobile-nav__body"
          aria-label="Main"
        >
          <ul class="mobile-nav__list">
            <li
              v-for="item in menuItems"
              :key="item.to"
            >
              <router-link
                :to="item.to"
                class="mobile-nav__link"
                active-class=""
                exact-active-class=""
                :class="{ 'is-active': isActive(item.to) }"
                @click="emit('close')"
              >
                {{ t(item.labelKey) }}
              </router-link>
            </li>
          </ul>
        </nav>
      </aside>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { nextTick, onMounted, onUnmounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router'

interface Props {
  open: boolean
}

const props = defineProps<Props>()

interface Emits {
  (e: 'close'): void
}

const emit = defineEmits<Emits>()

const { t } = useI18n()
const route = useRoute()
const drawerRef = ref<HTMLElement | null>(null)

const menuItems = [
  { to: '/', labelKey: 'common.nav.home' },
  { to: '/products', labelKey: 'common.nav.products' },
  { to: '/about', labelKey: 'common.nav.about' },
  { to: '/news', labelKey: 'common.nav.news' },
  { to: '/videos', labelKey: 'common.nav.videos' },
  { to: '/downloads', labelKey: 'common.nav.downloads' },
  { to: '/contact', labelKey: 'common.nav.contact' }
] as const

const isActive = (path: string) => {
  if (path === '/') {
    return route.path === '/'
  }
  return route.path === path || route.path.startsWith(`${path}/`)
}

const lockBodyScroll = (locked: boolean) => {
  document.body.style.overflow = locked ? 'hidden' : ''
}

const onKeydown = (event: KeyboardEvent) => {
  if (!props.open) return
  if (event.key === 'Escape') {
    event.preventDefault()
    emit('close')
  }
}

watch(
  () => props.open,
  async open => {
    lockBodyScroll(open)
    if (open) {
      await nextTick()
      drawerRef.value?.focus()
    }
  }
)

watch(
  () => route.fullPath,
  () => {
    if (props.open) {
      emit('close')
    }
  }
)

onMounted(() => {
  window.addEventListener('keydown', onKeydown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', onKeydown)
  lockBodyScroll(false)
})
</script>

<style scoped lang="scss">
.mobile-nav {
  position: fixed;
  inset: 0;
  z-index: $z-index-sidebar;
  pointer-events: none;
  visibility: hidden;

  &.open {
    pointer-events: auto;
    visibility: visible;

    .mobile-nav__overlay {
      opacity: 1;
    }

    .mobile-nav__drawer {
      transform: translateX(0);
    }
  }
}

.mobile-nav__overlay {
  position: absolute;
  inset: 0;
  background: rgba(15, 23, 42, 0.45);
  opacity: 0;
  transition: opacity 280ms ease;
}

.mobile-nav__drawer {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  width: min(300px, 86vw);
  background: $color-bg-primary;
  box-shadow: $shadow-xl;
  display: flex;
  flex-direction: column;
  transform: translateX(100%);
  transition: transform 280ms cubic-bezier(0.4, 0, 0.2, 1);
  outline: none;
  padding-top: env(safe-area-inset-top);
  padding-bottom: env(safe-area-inset-bottom);
}

.mobile-nav__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  min-height: 56px;
  padding: 0 16px;
  border-bottom: 1px solid $color-border;
}

.mobile-nav__title {
  font-family: $font-heading;
  font-size: 16px;
  font-weight: 700;
  color: $color-text-primary;
}

.mobile-nav__close {
  width: 40px;
  height: 40px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: none;
  background: transparent;
  border-radius: $radius-sm;
  cursor: pointer;
  color: $color-text-secondary;
  transition: background $transition-fast, color $transition-fast;

  &:hover {
    background: $color-bg-secondary;
    color: $color-primary;
  }
}

.icon-close {
  position: relative;
  width: 18px;
  height: 18px;

  &::before,
  &::after {
    content: '';
    position: absolute;
    top: 50%;
    left: 0;
    width: 100%;
    height: 2px;
    background: currentColor;
    border-radius: 2px;
  }

  &::before {
    transform: translateY(-50%) rotate(45deg);
  }

  &::after {
    transform: translateY(-50%) rotate(-45deg);
  }
}

.mobile-nav__body {
  flex: 1;
  overflow-y: auto;
  padding: 8px 0;
}

.mobile-nav__list {
  list-style: none;
  margin: 0;
  padding: 0;
}

.mobile-nav__link {
  display: flex;
  align-items: center;
  min-height: 48px;
  padding: 0 20px;
  font-family: $font-heading;
  font-size: 16px;
  font-weight: 500;
  color: $color-text-secondary;
  text-decoration: none;
  transition: background $transition-fast, color $transition-fast;

  &:hover {
    background: $color-bg-secondary;
    color: $color-primary;
  }

  &.is-active {
    color: $color-primary;
    font-weight: 700;
    background: rgba($color-primary, 0.06);
    box-shadow: inset 3px 0 0 $color-primary;
  }
}
</style>
