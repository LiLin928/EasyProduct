<template>
  <BaseFormDialog
    :visible="visible"
    :title="isEdit ? t('basic.menu.edit') : t('basic.menu.add')"
    :model="model"
    :rules="rules"
    :loading="loading"
    width="600px"
    @update:visible="handleClose"
    @submit="handleSubmit"
  >
    <el-form-item
      :label="t('basic.menu.parent')"
      prop="parentId"
    >
      <el-cascader
        v-model="model.parentId"
        :options="menuTreeData"
        :props="{
          label: 'title',
          value: 'id',
          children: 'children',
          checkStrictly: true,
          emitPath: false
        }"
        :placeholder="t('common.selectPlaceholder')"
        clearable
        filterable
      />
    </el-form-item>
    <el-form-item
      :label="t('basic.menu.name')"
      prop="name"
    >
      <el-input
        v-model="model.name"
        :placeholder="t('common.inputPlaceholder')"
      />
    </el-form-item>
    <el-form-item
      :label="t('basic.menu.path')"
      prop="path"
    >
      <el-input
        v-model="model.path"
        :placeholder="t('common.inputPlaceholder')"
      />
    </el-form-item>
    <el-form-item
      :label="t('basic.menu.titleKey')"
      prop="titleKey"
    >
      <el-input
        v-model="model.titleKey"
        :placeholder="t('common.inputPlaceholder')"
      />
    </el-form-item>
    <el-form-item
      :label="t('basic.menu.icon')"
      prop="icon"
    >
      <el-input
        v-model="model.icon"
        readonly
        @click="iconSelectVisible = true"
      >
        <template #prefix>
          <el-icon v-if="model.icon">
            <component :is="model.icon" />
          </el-icon>
        </template>
        <template #suffix>
          <el-icon
            class="el-input__icon"
            style="cursor: pointer"
          >
            <Search />
          </el-icon>
        </template>
      </el-input>
    </el-form-item>
    <el-form-item
      :label="t('basic.menu.sort')"
      prop="sort"
    >
      <el-input-number
        v-model="model.sort"
        :min="1"
        :max="999"
      />
    </el-form-item>
    <el-form-item
      :label="t('basic.menu.permission')"
      prop="permission"
    >
      <el-input
        v-model="model.permission"
        :placeholder="t('common.inputPlaceholder')"
      />
    </el-form-item>
    <el-form-item
      :label="t('basic.menu.component')"
      prop="component"
    >
      <el-input
        v-model="model.component"
        :placeholder="t('common.inputPlaceholder')"
      />
    </el-form-item>
    <el-form-item
      :label="t('basic.menu.visible')"
      prop="visible"
    >
      <el-switch
        v-model="model.visible"
        :active-value="1"
        :inactive-value="0"
      />
    </el-form-item>
    <el-form-item
      :label="t('basic.menu.status')"
      prop="status"
    >
      <el-radio-group v-model="model.status">
        <el-radio :value="1">
          {{ t('common.status.1') }}
        </el-radio>
        <el-radio :value="0">
          {{ t('common.status.0') }}
        </el-radio>
      </el-radio-group>
    </el-form-item>

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

      <!-- 图标选择器（嵌在 footer 旁，保持原有交互） -->
      <IconSelectDialog
        v-model="model.icon"
        v-model:visible="iconSelectVisible"
      />
    </template>
  </BaseFormDialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { Search } from '@element-plus/icons-vue'
import type { FormRules } from 'element-plus'
import { createMenu, updateMenu, getMenuDetail } from '@/api/basic/menu'
import type { Menu, MenuCreateParams } from '@/types/basic'
import BaseFormDialog from '@/components/common/BaseFormDialog.vue'
import IconSelectDialog from './IconSelectDialog.vue'

interface Props {
  visible: boolean
  payload?: Menu
  isEdit: boolean
  menuTree: Menu[]
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const { t } = useI18n()

const loading = ref(false)
const iconSelectVisible = ref(false)

// 表单模型
const model = reactive<MenuCreateParams>({
  parentId: '0',
  name: '',
  path: '',
  titleKey: '',
  icon: '',
  sort: 1,
  permission: '',
  component: '',
  visible: 1,
  status: 1
})

// 表单验证规则
const rules: FormRules = {
  parentId: [
    {
      required: true,
      message: () => t('common.required'),
      trigger: 'change'
    }
  ],
  name: [
    {
      required: true,
      message: () => t('common.required'),
      trigger: 'blur'
    },
    {
      pattern: /^[a-zA-Z][a-zA-Z0-9-]*$/,
      message: () => t('basic.menu.form.nameFormat'),
      trigger: 'blur'
    }
  ],
  path: [
    {
      required: true,
      message: () => t('common.required'),
      trigger: 'blur'
    },
    {
      pattern: /^\//,
      message: () => t('basic.menu.form.pathFormat'),
      trigger: 'blur'
    }
  ],
  titleKey: [
    {
      required: true,
      message: () => t('common.required'),
      trigger: 'blur'
    }
  ],
  sort: [
    {
      required: true,
      message: () => t('common.required'),
      trigger: 'blur'
    }
  ]
}

interface MenuTreeNode {
  id: string
  title: string
  children?: MenuTreeNode[]
}

// 菜单树数据（添加根节点并转换为 Cascader 格式）
const menuTreeData = computed(() => {
  const transformMenu = (menus: Menu[]): MenuTreeNode[] => {
    return menus.map((menu) => ({
      id: menu.id,
      title: t(menu.titleKey),
      children: menu.children ? transformMenu(menu.children) : undefined
    }))
  }

  return [
    {
      id: '0',
      title: t('basic.menu.root'),
      children: props.menuTree ? transformMenu(props.menuTree) : []
    }
  ]
})

// 重置表单
const resetForm = (): void => {
  Object.assign(model, {
    parentId: '0',
    name: '',
    path: '',
    titleKey: '',
    icon: '',
    sort: 1,
    permission: '',
    component: '',
    visible: 1,
    status: 1
  })
}

// 监听 payload 变化，填充表单
watch(
  () => props.payload,
  async (payload) => {
    if (payload && payload.id) {
      // 编辑模式，加载详情
      try {
        const detail = await getMenuDetail(payload.id)
        Object.assign(model, {
          parentId: detail.parentId,
          name: detail.name,
          path: detail.path,
          titleKey: detail.titleKey,
          icon: detail.icon,
          sort: detail.sort,
          permission: detail.permission || '',
          component: detail.component || '',
          visible: detail.visible,
          status: detail.status
        })
      } catch {
        // 加载失败
      }
    } else if (payload && payload.parentId) {
      // 新增子菜单模式
      resetForm()
      model.parentId = payload.parentId
    } else {
      // 新增根菜单模式
      resetForm()
    }
  },
  { immediate: true }
)

// 关闭弹窗
const handleClose = (): void => {
  emit('update:visible', false)
  resetForm()
}

// 提交表单（验证由 BaseFormDialog 处理，但此处 IconSelectDialog 需要 footer slot，所以自定义 footer）
const handleSubmit = async (): Promise<void> => {
  loading.value = true
  try {
    if (props.isEdit && props.payload?.id) {
      // 编辑菜单
      await updateMenu(props.payload.id, model)
    } else {
      // 新增菜单
      await createMenu(model)
    }
    emit('success')
    handleClose()
  } finally {
    loading.value = false
  }
}
</script>
