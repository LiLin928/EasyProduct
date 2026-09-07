<!-- src/views/basic/dept/components/DeptFormDialog.vue -->
<template>
  <BaseFormDialog
    :visible="modelValue"
    :title="isEdit ? t('dept.form.editTitle') : t('dept.form.addTitle')"
    :model="formData"
    :rules="formRules"
    :loading="saving"
    width="500px"
    @update:visible="emit('update:modelValue', $event)"
    @close="handleClose"
    @submit="handleSave"
  >
    <!-- 上级部门 -->
    <el-form-item
      :label="t('dept.form.parent')"
      prop="parentId"
    >
      <el-tree-select
        v-model="formData.parentId"
        :data="treeDataForSelect"
        :props="treeSelectProps"
        check-strictly
        clearable
        :placeholder="t('dept.form.parentPlaceholder')"
        style="width: 100%"
      />
    </el-form-item>

    <!-- 部门名称 -->
    <el-form-item
      :label="t('dept.form.name')"
      prop="name"
    >
      <el-input
        v-model="formData.name"
        maxlength="100"
        show-word-limit
        :placeholder="t('dept.form.namePlaceholder')"
      />
    </el-form-item>

    <!-- 部门编码 -->
    <el-form-item
      :label="t('dept.form.code')"
      prop="code"
    >
      <el-input
        v-model="formData.code"
        maxlength="50"
        show-word-limit
        :placeholder="t('dept.form.codePlaceholder')"
      />
    </el-form-item>

    <!-- 排序 -->
    <el-form-item
      :label="t('dept.form.sort')"
      prop="sort"
    >
      <el-input-number
        v-model="formData.sort"
        :min="0"
        :max="999"
      />
    </el-form-item>

    <!-- 状态 -->
    <el-form-item
      :label="t('dept.form.status')"
      prop="status"
    >
      <el-radio-group v-model="formData.status">
        <el-radio :value="1">
          {{ t('common.status.enabled') }}
        </el-radio>
        <el-radio :value="0">
          {{ t('common.status.disabled') }}
        </el-radio>
      </el-radio-group>
    </el-form-item>

    <!-- 部门负责人 -->
    <el-form-item
      :label="t('dept.form.leaderName')"
      prop="leaderName"
    >
      <el-input
        v-model="formData.leaderName"
        maxlength="50"
        :placeholder="t('dept.form.leaderNamePlaceholder')"
      />
    </el-form-item>

    <!-- 联系电话 -->
    <el-form-item
      :label="t('dept.form.phone')"
      prop="phone"
    >
      <el-input
        v-model="formData.phone"
        maxlength="20"
        :placeholder="t('dept.form.phonePlaceholder')"
      />
    </el-form-item>

    <!-- 邮箱 -->
    <el-form-item
      :label="t('dept.form.email')"
      prop="email"
    >
      <el-input
        v-model="formData.email"
        maxlength="100"
        :placeholder="t('dept.form.emailPlaceholder')"
      />
    </el-form-item>

    <!-- 描述 -->
    <el-form-item
      :label="t('dept.form.description')"
      prop="description"
    >
      <el-input
        v-model="formData.description"
        type="textarea"
        :rows="3"
        maxlength="500"
        show-word-limit
        :placeholder="t('dept.form.descriptionPlaceholder')"
      />
    </el-form-item>
  </BaseFormDialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { getDeptDetail, createDept, updateDept } from '@/api/basic/dept'
import type { Dept, DeptCreateParams, DeptUpdateParams } from '@/types/basic'
import BaseFormDialog from '@/components/common/BaseFormDialog.vue'

const props = defineProps<{
  modelValue: boolean
  deptId?: string
  parentId?: string
  treeData: Dept[]
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

const saving = ref(false)

// 是否是编辑模式
const isEdit = computed(() => !!props.deptId)

// 表单数据
const formData = reactive<DeptCreateParams>({
  parentId: '0',
  name: '',
  code: '',
  sort: 0,
  status: 1,
  leaderName: '',
  phone: '',
  email: '',
  description: '',
})

// 表单验证规则
const formRules: FormRules = {
  name: [
    { required: true, message: () => t('dept.form.nameRequired'), trigger: 'blur' },
    { min: 2, max: 100, message: () => t('dept.form.nameLength'), trigger: 'blur' },
  ],
  code: [
    { required: true, message: () => t('dept.form.codeRequired'), trigger: 'blur' },
  ],
  email: [
    { type: 'email', message: () => t('common.emailFormat'), trigger: 'blur' },
  ],
}

/**
 * 递归排除当前部门及其子部门
 * @param depts 部门列表
 * @param excludeId 要排除的部门 ID
 * @returns 过滤后的部门列表
 */
const excludeDeptAndChildren = (depts: Dept[], excludeId: string): Dept[] => {
  return depts
    .filter((dept) => dept.id !== excludeId)
    .map((dept) => ({
      ...dept,
      children: dept.children ? excludeDeptAndChildren(dept.children, excludeId) : undefined,
    }))
}

// 为 TreeSelect 准备的数据（添加一个"无"选项）
// 编辑模式下排除当前部门及其子部门，防止循环引用
const treeDataForSelect = computed(() => {
  let filteredData = props.treeData

  // 编辑模式：排除当前部门及其子部门
  if (props.deptId) {
    filteredData = excludeDeptAndChildren(filteredData, props.deptId)
  }

  return [
    { id: '0', name: t('dept.form.noParent'), children: [] },
    ...filteredData,
  ]
})

// TreeSelect props 配置
const treeSelectProps = {
  children: 'children',
  label: 'name',
  value: 'id',
}

// 监听弹窗打开
watch(
  () => props.modelValue,
  async (val) => {
    if (val) {
      if (props.deptId) {
        // 编辑模式：加载部门详情
        await loadDetail(props.deptId)
      } else {
        // 新增模式：设置默认上级
        resetForm()
        formData.parentId = props.parentId || '0'
      }
    }
  }
)

// 加载部门详情
const loadDetail = async (id: string) => {
  try {
    const dept = await getDeptDetail(id)
    if (dept) {
      formData.parentId = dept.parentId
      formData.name = dept.name
      formData.code = dept.code || ''
      formData.sort = dept.sort ?? 0
      formData.status = dept.status
      formData.leaderName = dept.leaderName || ''
      formData.phone = dept.phone || ''
      formData.email = dept.email || ''
      formData.description = dept.description || ''
    }
  } catch {
    ElMessage.error(t('common.error.request'))
  }
}

// 重置表单
const resetForm = () => {
  formData.parentId = '0'
  formData.name = ''
  formData.code = ''
  formData.sort = 0
  formData.status = 1
  formData.leaderName = ''
  formData.phone = ''
  formData.email = ''
  formData.description = ''
}

// 关闭弹窗
const handleClose = () => {
  resetForm()
}

// 保存（验证由 BaseFormDialog 处理）
const handleSave = async () => {
  saving.value = true
  try {
    if (isEdit.value) {
      const params: DeptUpdateParams = {
        parentId: formData.parentId,
        name: formData.name,
        code: formData.code,
        sort: formData.sort,
        status: formData.status,
        leaderName: formData.leaderName || undefined,
        phone: formData.phone || undefined,
        email: formData.email || undefined,
        description: formData.description || undefined,
      }
      await updateDept(props.deptId!, params)
      ElMessage.success(t('dept.message.updateSuccess'))
    } else {
      await createDept(formData)
      ElMessage.success(t('dept.message.addSuccess'))
    }
    emit('update:modelValue', false)
    emit('success')
  } catch {
    ElMessage.error(t('common.error.request'))
  } finally {
    saving.value = false
  }
}
</script>
