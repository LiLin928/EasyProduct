<template>
  <div class="user-page">
    <!-- 搜索栏 -->
    <el-card class="user-page__search">
      <el-form
        :model="query"
        :inline="true"
        label-width="auto"
      >
        <el-form-item :label="t('basic.user.userName')">
          <el-input
            v-model="query.userName"
            :placeholder="t('common.inputPlaceholder')"
            clearable
          />
        </el-form-item>
        <el-form-item :label="t('basic.user.realName')">
          <el-input
            v-model="query.realName"
            :placeholder="t('common.inputPlaceholder')"
            clearable
          />
        </el-form-item>
        <el-form-item :label="t('basic.user.status')">
          <el-select
            v-model="query.status"
            :placeholder="t('common.selectPlaceholder')"
            clearable
          >
            <el-option
              label="启用"
              value="enabled"
            />
            <el-option
              label="禁用"
              value="disabled"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button
            type="primary"
            @click="handleSearch"
          >
            {{ t('common.search') }}
          </el-button>
          <el-button @click="handleReset">
            {{ t('common.reset') }}
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 工具栏 -->
    <el-card class="user-page__toolbar">
      <el-button
        v-permission="['basic:user:edit']"
        type="primary"
        @click="handleAdd"
      >
        {{ t('common.add') }}
      </el-button>
    </el-card>

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
            <el-tag :type="row.status === 'enabled' ? 'success' : 'danger'">
              {{ row.status === 'enabled' ? '启用' : '禁用' }}
            </el-tag>
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
import { getUserList, deleteUser, resetPassword } from '@/api/basic/user'
import type { User } from '@/types/basic'
import BaseTable from '@/components/common/BaseTable.vue'
import UserForm from './components/UserForm.vue'

const { t } = useI18n()

// 列表状态
const { loading, list, total, query, handleSearch, handleReset, handlePageChange, reload } = useTable(
  getUserList,
  { immediate: true }
)

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
  } catch (error) {
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
  } catch (error) {
    // 用户取消或请求失败
  }
}
</script>

<style scoped lang="scss">
.user-page {
  &__search,
  &__toolbar {
    margin-bottom: $spacing-md;
  }
}
</style>