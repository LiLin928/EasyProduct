<template>
  <div class="role-page">
    <!-- 搜索表单 -->
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <!-- 工具栏 -->
    <el-card class="role-page__toolbar">
      <el-button
        v-permission="['basic:role:edit']"
        type="primary"
        @click="handleAdd"
      >
        {{ t('common.add') }}
      </el-button>
    </el-card>

    <!-- 列表 -->
    <el-card class="role-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="name"
          :label="t('basic.role.name')"
          min-width="120"
        />
        <el-table-column
          prop="code"
          :label="t('basic.role.code')"
          min-width="120"
        />
        <el-table-column
          prop="sort"
          :label="t('basic.role.sort')"
          width="80"
        />
        <el-table-column
          prop="status"
          :label="t('basic.role.status')"
          width="100"
        >
          <template #default="{ row }">
            <el-tag :type="row.status === 'enabled' ? 'success' : 'danger'">
              {{ row.status === 'enabled' ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="remark"
          :label="t('basic.role.remark')"
          min-width="180"
        />
        <el-table-column
          prop="createdAt"
          :label="t('basic.role.createdAt')"
          width="180"
        />
        <el-table-column
          :label="t('common.actions')"
          width="250"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              v-permission="['basic:role:edit']"
              link
              type="primary"
              @click="handleEdit(row)"
            >
              {{ t('common.edit') }}
            </el-button>
            <el-button
              v-permission="['basic:role:edit']"
              link
              type="primary"
              @click="handleAssignMenus(row)"
            >
              {{ t('basic.role.assignMenus') }}
            </el-button>
            <el-button
              v-permission="['basic:role:delete']"
              link
              type="danger"
              @click="handleDelete(row)"
            >
              {{ t('common.delete') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <!-- 新增/编辑弹窗 -->
    <RoleForm
      :visible="formDialog.visible.value"
      :payload="formDialog.payload.value"
      :is-edit="formDialog.isEdit.value"
      @update:visible="formDialog.visible.value = $event"
      @success="handleFormSuccess"
    />

    <!-- 菜单分配弹窗 -->
    <MenuAssign
      :visible="menuDialog.visible.value"
      :payload="menuDialog.payload.value"
      @update:visible="menuDialog.visible.value = $event"
      @success="handleMenuSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useTable } from '@/composables/useTable'
import { useDialog } from '@/composables/useDialog'
import { useSearch } from '@/composables/useSearch'
import { getRoleList, deleteRole } from '@/api/basic/role'
import type { Role } from '@/types/basic'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import RoleForm from './components/RoleForm.vue'
import MenuAssign from './components/MenuAssign.vue'

const { t } = useI18n()

// 搜索字段配置
const searchFields: SearchField[] = [
  {
    prop: 'name',
    label: 'basic.role.name',
    type: 'input'
  },
  {
    prop: 'code',
    label: 'basic.role.code',
    type: 'input'
  },
  {
    prop: 'status',
    label: 'basic.role.status',
    type: 'select',
    options: [
      { label: '启用', value: 'enabled' },
      { label: '禁用', value: 'disabled' }
    ]
  }
]

// 搜索逻辑
const { searchModel, getSearchParams } = useSearch({
  defaultModel: {
    name: '',
    code: '',
    status: ''
  }
})

// 列表状态
const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable(
  getRoleList,
  { immediate: true }
)

// 搜索（合并搜索参数到查询）
const handleSearch = (): void => {
  Object.assign(query, getSearchParams())
  tableSearch()
}

// 重置（清空搜索参数并重置）
const handleReset = (): void => {
  // 清空查询中的搜索字段
  delete query.name
  delete query.code
  delete query.status
  tableReset()
}

// 表单弹窗
const formDialog = useDialog<Role>()

// 菜单分配弹窗
const menuDialog = useDialog<Role>()

// 新增角色
const handleAdd = (): void => {
  formDialog.open()
}

// 编辑角色
const handleEdit = (row: Role): void => {
  formDialog.open(row)
}

// 表单提交成功
const handleFormSuccess = (): void => {
  reload()
}

// 分配菜单
const handleAssignMenus = (row: Role): void => {
  menuDialog.open(row)
}

// 菜单分配成功
const handleMenuSuccess = (): void => {
  ElMessage.success(t('common.success'))
}

// 删除角色
const handleDelete = async (row: Role): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('common.deleteConfirm'),
      t('common.tips'),
      { type: 'warning' }
    )
    await deleteRole(row.id)
    ElMessage.success(t('common.deleteSuccess'))
    reload()
  } catch (error) {
    // 用户取消或请求失败
  }
}
</script>

<style scoped lang="scss">
.role-page {
  &__toolbar {
    margin-bottom: $spacing-md;
  }
}
</style>