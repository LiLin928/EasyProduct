<template>
  <div class="user-page">
    <!-- 搜索表单 -->
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    >
      <template #toolbar>
        <el-button
          v-permission="['basic:user:edit']"
          type="primary"
          @click="handleAdd"
        >
          {{ t('common.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <!-- 列表 -->
    <el-card class="user-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="userName"
          :label="t('basic.user.userName')"
          min-width="120"
        />
        <el-table-column
          prop="realName"
          :label="t('basic.user.realName')"
          min-width="120"
        />
        <el-table-column
          prop="email"
          :label="t('basic.user.email')"
          min-width="180"
        />
        <el-table-column
          prop="phone"
          :label="t('basic.user.phone')"
          min-width="130"
        />
        <el-table-column
          prop="status"
          :label="t('basic.user.status')"
          width="100"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="ENABLED_DISABLED_STATUS"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('basic.user.createdAt')"
          width="180"
        />
        <el-table-column
          :label="t('common.actions')"
          width="200"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              v-permission="['basic:user:edit']"
              link
              type="primary"
              @click="handleEdit(row)"
            >
              {{ t('common.edit') }}
            </el-button>
            <el-button
              v-permission="['basic:user:edit']"
              link
              type="warning"
              @click="handleResetPassword(row)"
            >
              {{ t('basic.user.resetPassword') }}
            </el-button>
            <el-button
              v-permission="['basic:user:delete']"
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
    <UserForm
      :visible="formDialog.visible.value"
      :payload="formDialog.payload.value"
      :is-edit="formDialog.isEdit.value"
      @update:visible="formDialog.visible.value = $event"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useTable } from '@/composables/useTable'
import { useDialog } from '@/composables/useDialog'
import { useSearch } from '@/composables/useSearch'
import { getUserList, deleteUser, resetPassword } from '@/api/basic/user'
import type { User } from '@/types/basic'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import { ENABLED_DISABLED_STATUS } from '@/constants/status'
import UserForm from './components/UserForm.vue'

const { t } = useI18n()

// 搜索字段配置
const searchFields: SearchField[] = [
  {
    prop: 'userName',
    label: 'basic.user.userName',
    type: 'input'
  },
  {
    prop: 'realName',
    label: 'basic.user.realName',
    type: 'input'
  },
  {
    prop: 'status',
    label: 'basic.user.status',
    type: 'select',
    options: [
      { label: 'basic.user.enabled', value: 'enabled' },
      { label: 'basic.user.disabled', value: 'disabled' }
    ]
  }
]

// 搜索逻辑
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: {
    userName: '',
    realName: '',
    status: ''
  }
})

// 列表状态
const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable(
  getUserList,
  { immediate: true }
)

// 搜索（合并搜索参数到查询）
const handleSearch = (): void => {
  Object.assign(query, getSearchParams())
  tableSearch()
}

// 重置（清空搜索参数并重置）
const handleReset = (): void => {
  resetModel() // 清空 UI
  delete query.userName
  delete query.realName
  delete query.status
  tableReset()
}

// 表单弹窗
const formDialog = useDialog<User>()

// 新增用户
const handleAdd = (): void => {
  formDialog.open()
}

// 编辑用户
const handleEdit = (row: User): void => {
  formDialog.open(row)
}

// 表单提交成功
const handleFormSuccess = (): void => {
  reload()
}

// 重置密码
const handleResetPassword = async (row: User): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.user.resetPasswordConfirm'),
      t('common.tips'),
      { type: 'warning' }
    )
    await resetPassword(row.id)
    ElMessage.success(t('common.success'))
  } catch {
    // 用户取消或请求失败
  }
}

// 删除用户
const handleDelete = async (row: User): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('common.deleteConfirm'),
      t('common.tips'),
      { type: 'warning' }
    )
    await deleteUser(row.id)
    ElMessage.success(t('common.deleteSuccess'))
    reload()
  } catch {
    // 用户取消或请求失败
  }
}
</script>

<style scoped lang="scss">
.user-page {
}
</style>
