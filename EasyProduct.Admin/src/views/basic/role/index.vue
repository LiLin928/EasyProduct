<template>
  <div class="role-page">
    <!-- 搜索栏 -->
    <el-card class="role-page__search">
      <el-form
        :model="query"
        :inline="true"
        label-width="auto"
      >
        <el-form-item :label="t('basic.role.name')">
          <el-input
            v-model="query.name"
            :placeholder="t('common.inputPlaceholder')"
            clearable
          />
        </el-form-item>
        <el-form-item :label="t('basic.role.code')">
          <el-input
            v-model="query.code"
            :placeholder="t('common.inputPlaceholder')"
            clearable
          />
        </el-form-item>
        <el-form-item :label="t('basic.role.status')">
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
import { getRoleList, deleteRole } from '@/api/basic/role'
import type { Role } from '@/types/basic'
import BaseTable from '@/components/common/BaseTable.vue'
import RoleForm from './components/RoleForm.vue'
import MenuAssign from './components/MenuAssign.vue'

const { t } = useI18n()

// 列表状态
const { loading, list, total, query, handleSearch, handleReset, handlePageChange, reload } = useTable(
  getRoleList,
  { immediate: true }
)

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
  &__search,
  &__toolbar {
    margin-bottom: $spacing-md;
  }
}
</style>