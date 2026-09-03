<template>
  <div class="downloads-page">
    <div class="downloads-container">
      <!-- Header -->
      <div class="downloads-header">
        <h1>{{ t("site.downloads.title") }}</h1>
        <p>{{ t("site.downloads.subtitle") }}</p>
      </div>

      <div class="downloads-content">
        <!-- Category filter sidebar (desktop) -->
        <aside class="downloads-sidebar">
          <div class="filter-section">
            <h3 class="filter-title">
              {{ t("site.downloads.filter") }}
            </h3>
            <ul class="category-list">
              <li>
                <button
                  class="category-btn"
                  :class="{ active: !selectedCategory }"
                  @click="selectCategory(undefined)"
                >
                  {{ t("site.downloads.all") }}
                </button>
              </li>
              <li
                v-for="cat in categories"
                :key="cat.id"
              >
                <button
                  class="category-btn"
                  :class="{ active: selectedCategory === cat.id }"
                  @click="selectCategory(cat.id)"
                >
                  {{ categoryName(cat) }}
                </button>
              </li>
            </ul>
          </div>
        </aside>

        <!-- Main content area -->
        <main class="downloads-main">
          <!-- Category selection (mobile) -->
          <div class="category-tabs-mobile">
            <button
              class="tab-btn"
              :class="{ active: !selectedCategory }"
              @click="selectCategory(undefined)"
            >
              {{ t("site.downloads.all") }}
            </button>
            <button
              v-for="cat in categories"
              :key="cat.id"
              class="tab-btn"
              :class="{ active: selectedCategory === cat.id }"
              @click="selectCategory(cat.id)"
            >
              {{ categoryName(cat) }}
            </button>
          </div>

          <!-- Search -->
          <div class="downloads-toolbar">
            <input
              v-model="keyword"
              type="search"
              class="search-input"
              :placeholder="t('site.downloads.searchPlaceholder')"
            >
          </div>

          <!-- Loading -->
          <div
            v-if="loading"
            class="loading"
          >
            {{ t("common.loading") }}
          </div>

          <!-- Empty -->
          <div
            v-else-if="items.length === 0"
            class="empty"
          >
            {{ t("common.empty") }}
          </div>

          <!-- Download List -->
          <div
            v-else
            class="downloads-list"
          >
            <article
              v-for="item in items"
              :key="item.id"
              class="download-card"
            >
              <div class="download-icon">
                📄
              </div>
              <div class="download-body">
                <h3>{{ downloadTitle(item) }}</h3>
                <div class="meta">
                  <span class="file-size">{{ formatFileSize(item.fileSize) }}</span>
                  <span class="download-count">{{ item.downloadCount }} {{ t("site.downloads.downloads") }}</span>
                </div>
              </div>
              <div class="download-action">
                <a
                  class="download-btn"
                  :href="item.fileUrl"
                  :download="downloadTitle(item)"
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  {{ t("site.downloads.download") }}
                </a>
              </div>
            </article>
          </div>

          <!-- Pagination -->
          <div
            v-if="totalPages > 1"
            class="pagination"
          >
            <button
              class="page-btn"
              :disabled="query.pageIndex === 1"
              @click="goToPage(query.pageIndex - 1)"
            >
              {{ t("common.previous") }}
            </button>
            <span class="page-info">{{ query.pageIndex }} / {{ totalPages }}</span>
            <button
              class="page-btn"
              :disabled="query.pageIndex === totalPages"
              @click="goToPage(query.pageIndex + 1)"
            >
              {{ t("common.next") }}
            </button>
          </div>
        </main>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { getDownloadList, getDownloadCategoryList } from "@/api/site/download";
import type { Download, DownloadCategory } from "@/types/site";

const { t, locale } = useI18n();

const items = ref<Download[]>([]);
const categories = ref<DownloadCategory[]>([]);
const loading = ref(true);
const keyword = ref("");
const total = ref(0);
const selectedCategory = ref<string | undefined>(undefined);
const query = ref({
  pageIndex: 1,
  pageSize: 10,
});
let searchTimer: ReturnType<typeof setTimeout> | null = null;

const totalPages = computed(() => Math.max(1, Math.ceil(total.value / query.value.pageSize)));

const downloadTitle = (item: Download) =>
  locale.value === "zh-CN" ? item.title : item.titleEn;

const categoryName = (cat: DownloadCategory) =>
  locale.value === "zh-CN" ? cat.name : cat.nameEn;

const formatFileSize = (bytes: number) => {
  if (bytes < 1024) return bytes + " B";
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + " KB";
  return (bytes / (1024 * 1024)).toFixed(1) + " MB";
};

const selectCategory = (id: string | undefined) => {
  selectedCategory.value = id;
  query.value.pageIndex = 1;
  loadDownloads();
};

const goToPage = (page: number) => {
  if (page < 1 || page > totalPages.value) return;
  query.value.pageIndex = page;
  loadDownloads();
  window.scrollTo({ top: 0, behavior: "smooth" });
};

const loadCategories = async () => {
  try {
    const res = await getDownloadCategoryList();
    categories.value = res || [];
  } catch {
    categories.value = [];
  }
};

const loadDownloads = async () => {
  try {
    loading.value = true;
    const params = {
      pageIndex: query.value.pageIndex,
      pageSize: query.value.pageSize,
      categoryId: selectedCategory.value,
      keyword: keyword.value.trim() || undefined,
    };
    const res = await getDownloadList(params);
    items.value = res.list || [];
    total.value = res.total || 0;
  } catch {
    items.value = [];
    total.value = 0;
  } finally {
    loading.value = false;
  }
};

watch(keyword, () => {
  if (searchTimer) clearTimeout(searchTimer);
  searchTimer = setTimeout(() => {
    query.value.pageIndex = 1;
    loadDownloads();
  }, 300);
});

watch(selectedCategory, () => {
  query.value.pageIndex = 1;
});

onMounted(() => {
  loadCategories();
  loadDownloads();
});
</script>

<style scoped lang="scss">
.downloads-page {
  min-height: 100vh;
  background: $color-bg-secondary;
  padding-top: 80px;

  @include compact {
    padding-top: 56px;
  }
}

.downloads-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: $spacing-xl;

  @include compact {
    padding: $spacing-md;
  }
}

.downloads-header {
  text-align: center;
  margin-bottom: $spacing-2xl;

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

.downloads-content {
  display: grid;
  grid-template-columns: 220px 1fr;
  gap: -xl;

  @include compact {
    grid-template-columns: 1fr;
  }
}

.downloads-sidebar {
  @include compact {
    display: none;
  }
}

.filter-section {
  background: white;
  border-radius: $radius-lg;
  padding: $spacing-lg;
  box-shadow: $shadow-sm;
}

.filter-title {
  font-size: 16px;
  font-weight: 600;
  color: $color-text-primary;
  margin: 0 0 $spacing-md;
  padding-bottom: $spacing-sm;
  border-bottom: 1px solid $color-border;
}

.category-list {
  list-style: none;
  margin: 0;
  padding: 0;

  li {
    margin-bottom: $spacing-xs;
  }
}

.category-btn {
  width: 100%;
  text-align: left;
  padding: $spacing-sm $spacing-md;
  border: 1px solid transparent;
  border-radius: $radius-md;
  background: transparent;
  cursor: pointer;
  font-size: 14px;
  color: $color-text-secondary;
  transition: all $transition-fast;

  &:hover {
    background: $color-bg-secondary;
    color: $color-text-primary;
  }

  &.active {
    background: $color-primary;
    color: white;
    border-color: $color-primary;
  }
}

.downloads-main {
  flex: 1;
  min-width: 0;
}

.category-tabs-mobile {
  display: none;

  @include compact {
    display: flex;
    gap: $spacing-sm;
    overflow-x: auto;
    margin-bottom: $spacing-md;
    padding-bottom: 2px;
    -webkit-overflow-scrolling: touch;
    scrollbar-width: none;

    &::-webkit-scrollbar {
      display: none;
    }

    .tab-btn {
      flex-shrink: 0;
      white-space: nowrap;
      padding: 8px 16px;
      border-radius: 999px;
      border: 1px solid $color-border;
      background: white;
      cursor: pointer;
      font-size: 14px;
      color: $color-text-secondary;
      transition: all $transition-fast;

      &.active {
        background: $color-primary;
        color: white;
        border-color: $color-primary;
      }
    }
  }
}

.downloads-toolbar {
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

.downloads-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-md;
}

.download-card {
  display: grid;
  grid-template-columns: 64px 1fr auto;
  gap: $spacing-md;
  align-items: center;
  background: white;
  border-radius: $radius-lg;
  padding: $spacing-md;
  box-shadow: $shadow-sm;
  transition: transform $transition-base;

  &:hover {
    transform: translateY(-4px);
    box-shadow: $shadow-md;
  }

  @include mobile {
    grid-template-columns: 1fr;
    text-align: center;
  }
}

.download-icon {
  font-size: 32px;
  flex-shrink: 0;

  @include mobile {
    font-size: 48px;
  }
}

.download-body {
  flex: 1;
  min-width: 0;

  h3 {
    margin: 0 0 8px;
    font-size: 16px;
    color: $color-text-primary;
    line-height: 1.4;
  }

  .meta {
    display: flex;
    gap: $spacing-md;
    font-size: 13px;
    color: $color-text-tertiary;

    @include mobile {
      justify-content: center;
    }
  }

  .file-size {
    font-weight: 500;
    color: $color-text-secondary;
  }
}

.download-action {
  flex-shrink: 0;

  @include mobile {
    width: 100%;
  }
}

.download-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 100px;
  height: 40px;
  padding: 0 16px;
  border-radius: $radius-md;
  background: $color-primary;
  color: white;
  text-decoration: none;
  font-size: 14px;
  font-weight: 500;
  transition: all $transition-base;

  &:hover {
    background: $color-primary-dark;
    transform: translateY(-2px);
    box-shadow: $shadow-md;
  }

  @include mobile {
    width: 100%;
  }
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

