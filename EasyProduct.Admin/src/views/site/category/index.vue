<template>
  <div class="category-page">
    <el-card
      v-loading="loading"
      shadow="never"
    >
      <template #header>
        <div class="card-header">
          <span>{{ t('site.category.title') }}</span>
          <el-button
            type="primary"
            size="small"
            @click="handleAddRoot"
          >
            {{ t('site.category.add') }}
          </el-button>
        </div>
      </template>

      <el-tree
        ref="treeRef"
        :data="treeData"
        :props="{ children: 'children', label: 'name' }"
        node-key="id"
        highlight-current
        default-expand-all
        :expand-on-click-node="false"
      >
        <template #default="{ data }">
          <span class="tree-node">
            <span class="node-label">{{ data.name }}</span>
            <span class="node-en">{{ data.nameEn }}</span>
            <BaseStatusTag
              :value="data.status"
              :options="ENABLED_DISABLED_STATUS"
            />
            <span class="tree-actions">
              <el-button
                link
                size="small"
                type="primary"
                @click.stop="handleAddChild(data)"
              >
                {{ t('site.category.addChild') }}
              </el-button>
              <el-button
                link
                size="small"
                type="primary"
                @click.stop="handleEdit(data)"
              >
                {{ t('common.edit') }}
              </el-button>
              <el-button
                link
                size="small"
                type="danger"
                @click.stop="handleDelete(data)"
              >
                {{ t('common.delete') }}
              </el-button>
            </span>
          </span>
        </template>
      </el-tree>
    </el-card>

    <CategoryFormDialog
      :visible="dialogVisible"
      :id="editId"
      :is-edit="isEdit"
      :tree-data="list"
      :parent-id="currentParentId"
      @update:visible="dialogVisible = $event"
      @success="reload"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
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
const list = ref<ProductCategory[]>([])
const dialogVisible = ref(false)
const editId = ref<string | undefined>(undefined)
const isEdit = ref(false)
const currentParentId = ref('0')

interface CategoryNode extends ProductCategory {
  children?: CategoryNode[]
}

const treeData = computed<CategoryNode[]>(() => {
  return buildTree(list.value)
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
  roots.sort((a, b) => a.sort - b.sort)
  return roots
}

const reload = async (): Promise<void> => {
  loading.value = true
  try {
    const data = await getCategoryTree()
    list.value = data
  } catch {
    list.value = []
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  reload()
})

const handleAddRoot = (): void => {
  editId.value = undefined
  isEdit.value = false
  currentParentId.value = '0'
  dialogVisible.value = true
}

const handleAddChild = (data: ProductCategory): void => {
  editId.value = undefined
  isEdit.value = false
  currentParentId.value = data.id
  dialogVisible.value = true
}

const handleEdit = (data: ProductCategory): void => {
  editId.value = data.id
  isEdit.value = true
  dialogVisible.value = true
}

const handleDelete = async (data: ProductCategory): Promise<void> => {
  try {
    await ElMessageBox.confirm(t('site.category.deleteConfirm'), t('common.tips'), { type: 'warning' })
    await deleteCategory(data.id)
    ElMessage.success(t('site.category.message.deleteSuccess'))
    reload()
  } catch {
    // cancelled or failed
  }
}
</script>

<style scoped lang="scss">
.category-page {
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .tree-node {
    flex: 1;
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 14px;
    padding-right: 8px;

    .node-label {
      font-weight: 500;
    }

    .node-en {
      color: #909399;
      font-size: 12px;
    }

    .tree-actions {
      display: none;
      margin-left: auto;
    }
  }

  :deep(.el-tree-node__content:hover) .tree-actions {
    display: inline-flex;
  }
}
</style>
