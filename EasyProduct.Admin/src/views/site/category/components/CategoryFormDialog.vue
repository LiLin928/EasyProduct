<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('site.category.edit') : t('site.category.add')"
    width="600px"
    append-to-body
    @update:model-value="handleClose"
  >
    <el-form
      ref="formRef"
      :model="model"
      :rules="rules"
      label-width="100px"
      :disabled="saving"
    >
      <el-form-item
        :label="t('site.category.parentId')"
        prop="parentId"
      >
        <el-tree-select
          v-model="model.parentId"
          :data="treeOptions"
          :props="{ children: 'children', label: 'name', value: 'id' }"
          :placeholder="t('site.category.form.parentIdPlaceholder')"
          check-strictly
          clearable
          style="width: 100%"
        />
      </el-form-item>

      <el-form-item
        :label="t('site.category.name')"
        prop="name"
      >
        <el-input
          v-model="model.name"
          :placeholder="t('site.category.form.namePlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('site.category.nameEn')"
        prop="nameEn"
      >
        <el-input
          v-model="model.nameEn"
          :placeholder="t('site.category.form.nameEnPlaceholder')"
        />
      </el-form-item>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item
            :label="t('site.category.sort')"
            prop="sort"
          >
            <el-input-number
              v-model="model.sort"
              :min="0"
              :placeholder="t('site.category.form.sortPlaceholder')"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col
          v-if="isEdit"
          :span="12"
        >
          <el-form-item
            :label="t('site.category.status')"
            prop="status"
          >
            <el-radio-group v-model="model.status">
              <el-radio value="enabled">
                {{ t('site.category.statusEnabled') }}
              </el-radio>
              <el-radio value="disabled">
                {{ t('site.category.statusDisabled') }}
              </el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>

    <template #footer>
      <el-button @click="handleClose">
        {{ t('common.cancel') }}
      </el-button>
      <el-button
        type="primary"
        :loading="saving"
        @click="handleSubmit"
      >
        {{ t('common.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { createCategory, updateCategory } from '@/api/site/category'
import type { ProductCategory, CategoryStatus } from '@/types/site'

interface Props {
  visible: boolean
  id?: string
  isEdit: boolean
  treeData: ProductCategory[]
  parentId?: string
}

const props = withDefaults(defineProps<Props>(), {
  parentId: '0',
})
const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

interface CategoryNode extends ProductCategory {
  children?: CategoryNode[]
}

interface CategoryFormModel {
  name: string
  nameEn: string
  parentId: string
  sort: number
  status: CategoryStatus
}

const model = reactive<CategoryFormModel>({
  name: '',
  nameEn: '',
  parentId: '0',
  sort: 1,
  status: 'enabled',
})

const rules: FormRules = {
  name: [
    { required: true, message: t('site.category.form.nameRequired'), trigger: 'blur' },
  ],
}

const treeOptions = computed<CategoryNode[]>(() => {
  return buildTree(props.treeData)
})

function buildTree(list: ProductCategory[]): CategoryNode[] {
  const map = new Map<string, CategoryNode>()
  const roots: CategoryNode[] = []
  list.forEach(item => {
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
  return roots
}

const handleSubmit = async (): Promise<void> => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  saving.value = true
  try {
    if (props.isEdit && props.id) {
      await updateCategory(props.id, model)
      ElMessage.success(t('site.category.message.updateSuccess'))
    } else {
      await createCategory(model)
      ElMessage.success(t('site.category.message.createSuccess'))
    }
    emit('success')
    handleClose()
  } catch {
    // handled by interceptor
  } finally {
    saving.value = false
  }
}

const handleClose = (): void => {
  formRef.value?.resetFields()
  model.name = ''
  model.nameEn = ''
  model.parentId = '0'
  model.sort = 1
  model.status = 'enabled'
  emit('update:visible', false)
}

watch(
  () => props.visible,
  (val) => {
    if (val && !props.isEdit) {
      model.parentId = props.parentId
    }
    if (val && props.isEdit && props.id) {
      const found = props.treeData.find(c => c.id === props.id)
      if (found) {
        model.name = found.name
        model.nameEn = found.nameEn
        model.parentId = found.parentId
        model.sort = found.sort
        model.status = found.status
      }
    }
  },
)
</script>

<style scoped lang="scss">
</style>
