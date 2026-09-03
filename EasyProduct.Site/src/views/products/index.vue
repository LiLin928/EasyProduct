<template>
  <div class="products-page">
    <div class="products-container">
      <!-- Header: Title + Subtitle -->
      <div class="products-header">
        <h1>{{ t('site.products.title') }}</h1>
        <p>{{ t('site.products.subtitle') }}</p>
      </div>

      <div class="products-content">
        <!-- Sidebar with Category Filter -->
        <aside class="products-sidebar">
          <div class="filter-section">
            <h3 class="filter-title">
              {{ t('site.products.filter') }}
            </h3>
            <ul class="category-list">
              <li>
                <button
                  class="category-btn"
                  :class="{ active: !query.categoryId }"
                  @click="filterByCategory(undefined)"
                >
                  {{ t('site.products.allProduct') }}
                </button>
              </li>
              <li
                v-for="cat in categories"
                :key="cat.id"
              >
                <button
                  class="category-btn"
                  :class="{ active: query.categoryId === cat.id }"
                  @click="filterByCategory(cat.id)"
                >
                  {{ cat.name }}
                </button>
              </li>
            </ul>
          </div>
        </aside>

        <!-- Main Content -->
        <main class="products-main">
          <!-- Search Bar -->
          <div class="search-bar">
            <input
              v-model="searchKeyword"
              type="text"
              :placeholder="t('site.products.searchPlaceholder')"
              class="search-input"
            >
          </div>

          <!-- Loading State -->
          <div
            v-if="loading"
            class="loading"
          >
            {{ t('common.loading') }}
          </div>

          <!-- Empty State -->
          <div
            v-else-if="!products.length"
            class="empty"
          >
            {{ t('common.empty') }}
          </div>

          <!-- Product Grid -->
          <ProductGrid
            v-else
            :products="products"
          />

          <!-- Custom Pagination -->
          <div
            v-if="totalPages > 1"
            class="pagination"
          >
            <button
              class="page-btn"
              :disabled="query.pageIndex === 1"
              @click="goToPage(query.pageIndex - 1)"
            >
              {{ t('common.previous') }}
            </button>
            <span class="page-info">{{ query.pageIndex }} / {{ totalPages }}</span>
            <button
              class="page-btn"
              :disabled="query.pageIndex === totalPages"
              @click="goToPage(query.pageIndex + 1)"
            >
              {{ t('common.next') }}
            </button>
          </div>
        </main>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import ProductGrid from '@/components/product/ProductGrid.vue'
import { getCategoryList } from '@/api/site/category'
import { getProductList } from '@/api/site/product'
import type { ProductCategory, Product } from '@/types/site'

const { t } = useI18n()

const categories = ref<ProductCategory[]>([])
const products = ref<Product[]>([])
const loading = ref(false)
const total = ref(0)
const searchKeyword = ref('')
const query = ref({
  pageIndex: 1,
  pageSize: 12,
  categoryId: undefined as string | undefined
})

const totalPages = computed(() => Math.max(1, Math.ceil(total.value / query.value.pageSize)))

let searchTimer: ReturnType<typeof setTimeout> | null = null

const filterByCategory = (categoryId?: string) => {
  query.value.categoryId = categoryId
  query.value.pageIndex = 1
  loadProducts()
}

const goToPage = (page: number) => {
  if (page < 1 || page > totalPages.value) return
  query.value.pageIndex = page
  window.scrollTo({ top: 0, behavior: 'smooth' })
  loadProducts()
}

const loadCategories = async () => {
  if (categories.value.length > 0) return
  try {
    const res = await getCategoryList()
    categories.value = res
  } catch {
    // silently fail
  }
}

const loadProducts = async () => {
  loading.value = true
  try {
    await loadCategories()
    const res = await getProductList({
      pageIndex: query.value.pageIndex,
      pageSize: query.value.pageSize,
      categoryId: query.value.categoryId,
      keyword: searchKeyword.value?.trim() || undefined
    })
    products.value = res.list
    total.value = res.total
  } finally {
    loading.value = false
  }
}

// Watch search keyword with debounce
watch(searchKeyword, () => {
  if (searchTimer) clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    query.value.pageIndex = 1
    loadProducts()
  }, 300)
})

onMounted(() => {
  loadProducts()
})
</script>

<style scoped lang="scss">
.products-page {
  min-height: 100vh;
  background: $color-bg-secondary;
  padding-top: 80px;

  @include compact {
    padding-top: 56px;
  }
}

.products-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: $spacing-xl;
  min-width: 0;
  width: 100%;
  box-sizing: border-box;

  @include compact {
    padding: $spacing-md;
    overflow-x: clip;
  }
}

.products-header {
  text-align: center;
  margin-bottom: $spacing-2xl;

  @include compact {
    margin-bottom: $spacing-md;
  }

  h1 {
    font-size: 36px;
    font-weight: 700;
    color: $color-text-primary;
    margin-bottom: $spacing-sm;

    @include compact {
      font-size: 24px;
    }
  }

  p {
    font-size: 16px;
    color: $color-text-secondary;

    @include compact {
      font-size: 14px;
    }
  }
}

.products-content {
  display: grid;
  grid-template-columns: 250px 1fr;
  gap: $spacing-xl;
  min-width: 0;

  @include compact {
    grid-template-columns: minmax(0, 1fr);
    gap: $spacing-md;
  }
}

.products-sidebar {
  min-width: 0;
  max-width: 100%;

  @include compact {
    order: -1;
  }
}

.filter-section {
  background: white;
  padding: $spacing-md;
  border-radius: $radius-lg;
  box-shadow: $shadow-sm;
  min-width: 0;
  max-width: 100%;

  @include compact {
    padding: $spacing-sm;
    overflow: hidden;
  }
}

.filter-title {
  font-size: 16px;
  font-weight: 600;
  color: $color-text-primary;
  margin-bottom: $spacing-md;

  @include compact {
    display: none;
  }
}

.category-list {
  list-style: none;
  padding: 0;
  margin: 0;

  li {
    margin-bottom: $spacing-xs;
  }

  @include compact {
    display: flex;
    gap: $spacing-sm;
    overflow-x: auto;
    overflow-y: hidden;
    -webkit-overflow-scrolling: touch;
    scrollbar-width: none;
    padding-bottom: 2px;
    max-width: 100%;
    width: 100%;
    overscroll-behavior-x: contain;

    &::-webkit-scrollbar {
      display: none;
    }

    li {
      margin-bottom: 0;
      flex-shrink: 0;
    }
  }
}

.category-btn {
  width: 100%;
  padding: $spacing-sm $spacing-md;
  background: transparent;
  border: none;
  border-radius: $radius-sm;
  text-align: left;
  cursor: pointer;
  transition: all $transition-fast;
  color: $color-text-secondary;
  font-size: 14px;
  min-height: 40px;

  &:hover {
    background: $color-bg-secondary;
    color: $color-primary;
  }

  &.active {
    background: $color-primary;
    color: white;
    font-weight: 600;
  }

  @include compact {
    width: auto;
    white-space: nowrap;
    text-align: center;
    padding: 8px 14px;
    border: 1px solid $color-border;
    border-radius: 999px;
    background: $color-bg-secondary;
    min-height: 36px;

    &.active {
      border-color: $color-primary;
      background: $color-primary;
      color: white;
    }
  }
}

.products-main {
  min-width: 0;
}

.search-bar {
  margin-bottom: $spacing-lg;

  @include compact {
    margin-bottom: $spacing-md;
  }
}

.search-input {
  width: 100%;
  padding: $spacing-sm $spacing-md;
  border: 1px solid $color-border;
  border-radius: $radius-md;
  font-size: 16px;
  transition: all $transition-fast;
  box-sizing: border-box;

  &:focus {
    outline: none;
    border-color: $color-primary;
    box-shadow: 0 0 0 3px rgba($color-primary, 0.1);
  }
}

.loading,
.empty {
  text-align: center;
  padding: $spacing-3xl;
  color: $color-text-tertiary;
  font-size: 16px;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: $spacing-md;
  margin-top: $spacing-2xl;
  flex-wrap: wrap;

  @include compact {
    margin-top: $spacing-lg;
    gap: $spacing-sm;
  }
}

.page-btn {
  padding: $spacing-sm $spacing-lg;
  background: white;
  border: 1px solid $color-border;
  border-radius: $radius-md;
  cursor: pointer;
  transition: all $transition-fast;
  font-size: 14px;
  color: $color-text-secondary;
  min-height: 40px;
  min-width: 88px;

  @include compact {
    min-width: 72px;
    padding: 10px 14px;
  }

  &:hover:not(:disabled) {
    background: $color-primary;
    color: white;
    border-color: $color-primary;
  }

  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }
}

.page-info {
  font-size: 14px;
  color: $color-text-secondary;
  min-width: 56px;
  text-align: center;
}
</style>