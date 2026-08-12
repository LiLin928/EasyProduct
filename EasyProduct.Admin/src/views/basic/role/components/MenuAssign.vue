<template>
  <el-dialog
    :model-value="visible"
    :title="t('basic.role.assignMenus')"
    width="500px"
    @update:model-value="handleClose"
  >
    <el-tree
      ref="treeRef"
      :data="menuTree"
      :props="treeProps"
      :default-checked-keys="checkedKeys"
      show-checkbox
      node-key="id"
      default-expand-all
    />
    <template #footer>
      <el-button @click="handleClose">
        {{ t('common.cancel') }}
      </el-button>
      <el-button
        type="primary"
        :loading="loading"
        @click="handleSubmit"
      >
        {{ t('common.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import type { ElTree } from 'element-plus'
import { getUserMenuTree } from '@/api/basic/menu'
import { getRoleMenus, assignMenus } from '@/api/basic/role'
import type { Role } from '@/types/basic'
import type { MenuItem } from '@/types/basic'

interface Props {
  visible: boolean
  payload?: Role
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const { t } = useI18n()

const treeRef = ref<InstanceType<typeof ElTree>>()
const loading = ref(false)
const menuTree = ref<MenuItem[]>([])
const checkedKeys = ref<string[]>([])

const treeProps = {
  label: 'titleKey',
  children: 'children'
}

// 加载菜单树
const loadMenuTree = async (): Promise<void> => {
  try {
    const data = await getUserMenuTree()
    menuTree.value = data
  } catch (error) {
    // 加载失败
  }
}

// 加载角色的菜单ID列表
const loadRoleMenus = async (roleId: string): Promise<void> => {
  try {
    const menuIds = await getRoleMenus(roleId)
    checkedKeys.value = menuIds
  } catch (error) {
    // 加载失败
  }
}

// 监听 visible 变化，加载数据
watch(
  () => props.visible,
  async (visible) => {
    if (visible) {
      await loadMenuTree()
      if (props.payload) {
        await loadRoleMenus(props.payload.id)
      }
    }
  }
)

// 关闭弹窗
const handleClose = (): void => {
  emit('update:visible', false)
  checkedKeys.value = []
}

// 提交表单
const handleSubmit = async (): Promise<void> => {
  if (!props.payload) return

  loading.value = true
  try {
    const menuIds = treeRef.value?.getCheckedKeys(false) as string[]
    await assignMenus(props.payload.id, menuIds)
    emit('success')
    handleClose()
  } finally {
    loading.value = false
  }
}
</script>