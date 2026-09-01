<!-- src/views/site/category/index.vue -->
<template>
  <div class="category-container">
    <el-row :gutter="20">
      <!-- 左侧：分类树 -->
      <el-col :span="5">
        <el-card
          v-loading="loading"
          shadow="never"
          class="tree-card"
        >
          <template #header>
            <div class="tree-header">
              <span>{{ t('site.category.tree.title') }}</span>
              <el-button
                type="primary"
                size="small"
                @click="handleAddRoot"
              >
                {{ t('site.category.tree.addRoot') }}
              </el-button>
            </div>
          </template>

          <el-input
            v-model="filterText"
            :placeholder="t('site.category.tree.searchPlaceholder')"
            clearable
            class="tree-search"
          />

          <el-tree
            ref="treeRef"
            :data="treeData"
            :props="{ children: 'children', label: 'name' }"
            node-key="id"
            highlight-current
            :expand-on-click-node="false"
            :filter-node-method="filterNode"
            default-expand-all
            @node-click="handleNodeClick"
          >
            <template #default="{ data }">
              <span class="tree-node">
                <span class="node-label">{{ data.name }}</span>
                <span
                  v-if="data.children && data.children.length > 0"
                  class="node-count"
                >
                  ({{ data.children.length }})
                </span>
                <span class="tree-actions">
                  <el-button
                    link
                    size="small"
                    type="primary"
                    @click.stop="handleAddChild(data)"
                  >
                    {{ t('site.category.tree.addChild') }}
                  </el-button>
                  <el-button
                    link
                    size="small"
                    type="primary"
                    @click.stop="handleEdit(data)"
                  >
                    {{ t('site.category.tree.edit') }}
                  </el-button>
                  <el-button
                    link
                    size="small"
                    type="danger"
                    :disabled="data.children && data.children.length > 0"
                    @click.stop="handleDelete(data)"
                  >
                    {{ t('site.category.tree.delete') }}
                  </el-button>
                </span>
              </span>
            </template>
          </el-tree>
        </el-card>
      </el-col>

      <!-- 右侧：分类详情/子分类列表 -->
      <el-col :span="19">
        <el-card
          shadow="never"
          class="detail-card"
        >
          <template #header>
            <div class="detail-header">
              <span>{{ currentCategory?.name || t('site.category.detail.selectCategory') }}</span>
              <el-button
                v-if="currentCategory"
                type="primary"
                size="small"
                @click="handleEditDetail"
              >
                {{ t('common.edit') }}
              </el-button>
            </div>
          </template>

          <!-- 未选择分类时显示提示 -->
          <div
            v-if="!currentCategory"
            class="empty-tip"
          >
            <el-empty :description="t('site.category.detail.selectCategoryTip')" />
          </div>

          <!-- 分类详情 -->
          <div
            v-if="currentCategory"
            class="category-info"
          >
            <el-descriptions
              :column="2"
              border
            >
              <el-descriptions-item :label="t('site.category.detail.name')">
                {{ currentCategory.name }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('site.category.detail.nameEn')">
                {{ currentCategory.nameEn || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('site.category.detail.parent')">
                {{ getParentName(currentCategory.parentId) }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('site.category.detail.sort')">
                {{ currentCategory.sort }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('site.category.detail.status')">
                <BaseStatusTag
                  :value="currentCategory.status"
                  :options="ENABLED_DISABLED_STATUS"
                />
              </el-descriptions-item>
              <el-descriptions-item :label="t('site.category.detail.createdAt')">
                {{ currentCategory.createdAt }}
              </el-descriptions-item>
            </el-descriptions>
          </div>

          <!-- 子分类列表 -->
          <div
            v-if="currentCategory && subCategories.length > 0"
            class="subcategory-section"
          >
            <div class="section-header">
              {{ t('site.category.subcategory.title') }}
            </div>
            <el-table
              :data="subCategories"
              size="small"
              border
            >
              <el-table-column
                prop="name"
                :label="t('site.category.detail.name')"
              />
              <el-table-column
                prop="nameEn"
                :label="t('site.category.detail.nameEn')"
              />
              <el-table-column
                prop="sort"
                :label="t('site.category.detail.sort')"
                width="80"
              />
              <el-table-column
                prop="status"
                :label="t('site.category.detail.status')"
                width="80"
              >
                <template #default="{ row }">
                  <BaseStatusTag
                    :value="row.status"
                    :options="ENABLED_DISABLED_STATUS"
                  />
                </template>
              </el-table-column>
              <el-table-column
                :label="t('common.operation')"
                width="150"
              >
                <template #default="{ row }">
                  <el-button
                    link
                    size="small"
                    type="primary"
                    @click="handleEdit(row)"
                  >
                    {{ t('common.edit') }}
                  </el-button>
                  <el-button
                    link
                    size="small"
                    type="danger"
                    @click="handleDelete(row)"
                  >
                    {{ t('common.delete') }}
                  </el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <CategoryFormDialog
      :id="editId"
      :visible="dialogVisible"
      :is-edit="isEdit"
      :tree-data="flatList"
      :parent-id="currentParentId"
      @update:visible="dialogVisible = $event"
      @success="handleDialogSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, nextTick } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { ElTree } from 'element-plus'
import { getCategoryTree, deleteCategory } from '@/api/site/category'
import type { ProductCategory } from '@/types/site'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import { ENABLED_DISABLED_STATUS } from '@/constants/status'
import CategoryFormDialog from './components/CategoryFormDialog.vue'

const { t } = useI18n()

const treeRef = ref<InstanceType<typeof ElTree>>()
const loading = ref(false)
const filterText = ref('')
const list = ref<ProductCategory[]>([])
const currentCategory = ref<ProductCategory | null>(null)
const dialogVisible = ref(false)
const editId = ref<string | undefined>(undefined)
const isEdit = ref(false)
const currentParentId = ref('0')
const refreshSelectId = ref<string | null>(null)

interface CategoryNode extends ProductCategory {
  children?: CategoryNode[]
}

// 扁平化列表（用于下拉选择）
const flatList = computed(() => list.value)

// 构建树形结构
const treeData = computed<CategoryNode[]>(() => {
  return buildTree(list.value)
})

// 当前选中分类的子分类
const subCategories = computed(() => {
  if (!currentCategory.value) return []
  return list.value
    .filter(item => item.parentId === currentCategory.value!.id)
    .sort((a, b) => a.sort - b.sort)
})

function buildTree(items: ProductCategory[]): CategoryNode[] {
  const map = new Map<string, CategoryNode>()
  const roots: CategoryNode[] = []
  
  items.forEach(item => {
    map.set(item.id, { ...item, children: [] })
  })
  
  map.forEach(node => {
    if (node.parentId && node.parentId !== '0') {
      const parent = map.get(node.parentId)
      if (parent) {
        parent.children!.push(node)
      } else {
        roots.push(node)
      }
    } else {
      roots.push(node)
    }
  })
  
  // 排序
  roots.sort((a, b) => a.sort - b.sort)
  roots.forEach(node => {
    if (node.children) {
      node.children.sort((a, b) => a.sort - b.sort)
    }
  })
  
  return roots
}

// 根据ID查找分类
const findCategoryById = (id: string): ProductCategory | null => {
  return list.value.find(item => item.id === id) || null
}

// 获取父分类名称
const getParentName = (parentId: string): string => {
  if (!parentId || parentId === '0') return t('site.category.rootName')
  const parent = findCategoryById(parentId)
  return parent?.name || '-'
}

// 树节点过滤
const filterNode = (value: string, data: CategoryNode): boolean => {
  if (!value) return true
  return data.name.toLowerCase().includes(value.toLowerCase())
}

// 监听搜索文本变化
watch(filterText, (val) => {
  treeRef.value?.filter(val)
})

// 加载分类数据
const loadTree = async (): Promise<void> => {
  loading.value = true
  try {
    const data = await getCategoryTree()
    list.value = data

    // 如果有需要重新选中的分类ID，刷新后重新选中
    if (refreshSelectId.value) {
      await nextTick()
      treeRef.value?.setCurrentKey(refreshSelectId.value)
      const node = findCategoryById(refreshSelectId.value)
      if (node) {
        currentCategory.value = node
      }
      refreshSelectId.value = null
    }
  } catch (error) {
    // 错误已在拦截器处理
  } finally {
    loading.value = false
  }
}

// 点击树节点
const handleNodeClick = (data: ProductCategory): void => {
  currentCategory.value = data
}

// 新增根分类
const handleAddRoot = (): void => {
  editId.value = undefined
  isEdit.value = false
  currentParentId.value = '0'
  dialogVisible.value = true
}

// 新增子分类
const handleAddChild = (data: ProductCategory): void => {
  editId.value = undefined
  isEdit.value = false
  currentParentId.value = data.id
  dialogVisible.value = true
}

// 编辑分类
const handleEdit = (data: ProductCategory): void => {
  editId.value = data.id
  isEdit.value = true
  currentParentId.value = data.parentId || '0'
  refreshSelectId.value = data.id
  dialogVisible.value = true
}

// 从右侧详情区编辑
const handleEditDetail = (): void => {
  if (currentCategory.value) {
    handleEdit(currentCategory.value)
  }
}

// 弹窗保存成功
const handleDialogSuccess = (): void => {
  loadTree()
}

// 删除分类
const handleDelete = async (data: ProductCategory): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('site.category.deleteConfirm'),
      t('common.tips'),
      { type: 'warning' }
    )
    await deleteCategory(data.id)
    ElMessage.success(t('site.category.message.deleteSuccess'))
    
    // 如果删除的是当前选中的分类，清空右侧
    if (currentCategory.value?.id === data.id) {
      currentCategory.value = null
    }
    
    loadTree()
  } catch {
    // 取消或失败
  }
}

onMounted(() => {
  loadTree()
})
</script>

<style scoped lang="scss">
.category-container {
  padding: 20px;
  height: calc(100vh - 100px);

  .el-row {
    height: 100%;
  }

  .tree-card {
    height: 100%;

    .tree-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .tree-search {
      margin-bottom: 12px;
    }

    .tree-node {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: space-between;
      font-size: 14px;
      padding-right: 8px;

      .node-label {
        flex: 1;
      }

      .node-count {
        color: #909399;
        font-size: 12px;
        margin-left: 4px;
      }

      .tree-actions {
        display: none;
      }
    }

    :deep(.el-tree-node__content:hover) .tree-actions {
      display: inline-flex;
    }
  }

  .detail-card {
    height: 100%;

    .detail-header {
      font-size: 16px;
      font-weight: 500;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .empty-tip {
      padding: 40px 0;
    }

    .category-info {
      margin-bottom: 20px;
    }

    .subcategory-section {
      .section-header {
        font-size: 14px;
        font-weight: 500;
        margin-bottom: 12px;
        padding-bottom: 8px;
        border-bottom: 1px solid #ebeef5;
      }
    }
  }
}
</style>
