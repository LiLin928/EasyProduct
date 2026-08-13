<template>
  <div class="menu-page">
    <!-- 搜索表单 -->
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <!-- 工具栏 -->
    <el-card class="menu-page__toolbar">
      <el-button
        v-permission="['basic:menu:edit']"
        type="primary"
        @click="handleAdd"
      >
        {{ t('common.add') }}
      </el-button>
      <el-button @click="handleExpandAll">
        {{ t('basic.menu.expandAll') }}
      </el-button>
      <el-button @click="handleCollapseAll">
        {{ t('basic.menu.collapseAll') }}
      </el-button>
    </el-card>

    <!-- 菜单树形表格 -->
    <el-card class="menu-page__table">
      <el-table
        v-loading="loading"
        :data="filteredTableData"
        row-key="id"
        :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
        border
        :default-expand-all="isExpandAll"
      >
        <el-table-column
          :label="t('basic.menu.titleKey')"
          prop="titleKey"
          min-width="200"
        >
          <template #default="{ row }">
            <div class="menu-title">
              <el-icon v-if="row.icon">
                <component :is="row.icon" />
              </el-icon>
              <span>{{ t(row.titleKey) }}</span>
            </div>
          </template>
        </el-table-column>
        <el-table-column
          :label="t('basic.menu.name')"
          prop="name"
          min-width="120"
        />
        <el-table-column
          :label="t('basic.menu.path')"
          prop="path"
          min-width="150"
        />
        <el-table-column
          :label="t('basic.menu.sort')"
          prop="sort"
          width="80"
          align="center"
        />
        <el-table-column
          :label="t('basic.menu.visible')"
          prop="visible"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <el-switch
              v-model="row.visible"
              :active-text="t('basic.menu.visibleLabel')"
              :inactive-text="t('basic.menu.hiddenLabel')"
              @change="handleVisibleChange(row)"
            />
          </template>
        </el-table-column>
        <el-table-column
          :label="t('basic.menu.status')"
          prop="status"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <el-switch
              v-model="row.status"
              active-value="enabled"
              inactive-value="disabled"
              :active-text="t('basic.menu.enabled')"
              :inactive-text="t('basic.menu.disabled')"
              @change="handleStatusChange(row)"
            />
          </template>
        </el-table-column>
        <el-table-column
          :label="t('basic.menu.operation')"
          width="200"
          align="center"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              v-permission="['basic:menu:edit']"
              link
              type="primary"
              size="small"
              @click="handleEdit(row)"
            >
              {{ t('common.edit') }}
            </el-button>
            <el-button
              v-permission="['basic:menu:edit']"
              link
              type="primary"
              size="small"
              @click="handleAddChild(row)"
            >
              {{ t('basic.menu.addChild') }}
            </el-button>
            <el-button
              v-permission="['basic:menu:delete']"
              link
              type="danger"
              size="small"
              @click="handleDelete(row)"
            >
              {{ t('common.delete') }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 新增/编辑弹窗 -->
    <MenuForm
      :visible="formDialog.visible.value"
      :payload="formDialog.payload.value"
      :is-edit="formDialog.isEdit.value"
      :menu-tree="menuTree"
      @update:visible="formDialog.visible.value = $event"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useDialog } from '@/composables/useDialog'
import { useSearch } from '@/composables/useSearch'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import type { SearchField } from '@/types/search'
import {
  getMenuTree,
  deleteMenu,
  updateMenuStatus,
  updateMenuVisible
} from '@/api/basic/menu'
import type { Menu } from '@/types/basic'
import MenuForm from './components/MenuForm.vue'

const { t } = useI18n()

const loading = ref(false)
const menuTree = ref<Menu[]>([])
const isExpandAll = ref(true)

// 搜索字段配置
const searchFields: SearchField[] = [
  {
    prop: 'name',
    label: 'basic.menu.searchName',
    type: 'input'
  },
  {
    prop: 'status',
    label: 'basic.menu.searchStatus',
    type: 'select',
    options: [
      { label: 'basic.menu.enabled', value: 'enabled' },
      { label: 'basic.menu.disabled', value: 'disabled' }
    ]
  }
]

// 搜索逻辑
const { searchModel, handleSearch, handleReset } = useSearch({
  defaultModel: {
    name: '',
    status: ''
  }
})

// 过滤后的表格数据
const filteredTableData = computed(() => {
  if (!searchModel.name && !searchModel.status) {
    return menuTree.value
  }
  return filterMenuTree(menuTree.value, searchModel)
})

// 过滤菜单树（递归）
const filterMenuTree = (
  menus: Menu[],
  params: { name?: string; status?: string }
): Menu[] => {
  return menus
    .filter((menu) => {
      const nameMatch =
        !params.name || menu.titleKey.toLowerCase().includes(params.name.toLowerCase())
      const statusMatch = !params.status || menu.status === params.status

      // 递归过滤子菜单
      if (menu.children && menu.children.length > 0) {
        const filteredChildren = filterMenuTree(menu.children, params)
        return (nameMatch && statusMatch) || filteredChildren.length > 0
      }

      return nameMatch && statusMatch
    })
    .map((menu) => ({
      ...menu,
      children: menu.children ? filterMenuTree(menu.children, params) : undefined
    }))
}

// 表单弹窗
const formDialog = useDialog<Menu>()

// 加载菜单树
const loadMenuTree = async (): Promise<void> => {
  loading.value = true
  try {
    const data = await getMenuTree()
    menuTree.value = data
  } catch (error) {
    // 加载失败
  } finally {
    loading.value = false
  }
}

// 新增根菜单
const handleAdd = (): void => {
  formDialog.open()
}

// 新增子菜单
const handleAddChild = (row: Menu): void => {
  formDialog.open({ parentId: row.id } as Menu)
}

// 编辑菜单
const handleEdit = (row: Menu): void => {
  formDialog.open(row)
}

// 表单提交成功
const handleFormSuccess = (): void => {
  loadMenuTree()
}

// 展开所有节点
const handleExpandAll = (): void => {
  isExpandAll.value = true
}

// 折叠所有节点
const handleCollapseAll = (): void => {
  isExpandAll.value = false
}

// 状态切换
const handleStatusChange = async (row: Menu): Promise<void> => {
  const oldStatus = row.status === 'enabled' ? 'disabled' : 'enabled'
  try {
    await updateMenuStatus(row.id, row.status)
    ElMessage.success(t('basic.menu.updateSuccess'))
  } catch (error) {
    // 恢复原值
    row.status = oldStatus
    ElMessage.error(t('basic.menu.updateFailed'))
  }
}

// 可见性切换
const handleVisibleChange = async (row: Menu): Promise<void> => {
  const oldVisible = !row.visible
  try {
    await updateMenuVisible(row.id, row.visible)
    ElMessage.success(t('basic.menu.updateSuccess'))
  } catch (error) {
    // 恢复原值
    row.visible = oldVisible
    ElMessage.error(t('basic.menu.updateFailed'))
  }
}

// 删除菜单
const handleDelete = async (row: Menu): Promise<void> => {
  // 检查是否有子菜单
  if (row.children && row.children.length > 0) {
    ElMessage.warning(t('basic.menu.hasChildren'))
    return
  }

  try {
    await ElMessageBox.confirm(t('common.deleteConfirm'), t('common.tips'), {
      type: 'warning'
    })
    await deleteMenu(row.id)
    ElMessage.success(t('common.deleteSuccess'))
    loadMenuTree()
  } catch (error) {
    // 用户取消或请求失败
  }
}

// 初始化
onMounted(() => {
  loadMenuTree()
})
</script>

<style scoped lang="scss">
.menu-page {
  &__toolbar {
    margin-bottom: $spacing-md;
  }

  &__table {
    .menu-title {
      display: flex;
      align-items: center;
      gap: 8px;
    }
  }
}
</style>