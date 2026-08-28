<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('product.spu.edit') : t('product.spu.add')"
    width="900px"
    top="5vh"
    append-to-body
    @update:model-value="handleClose"
  >
    <el-tabs v-model="activeTab">
      <el-tab-pane
        :label="t('product.spu.title')"
        name="basic"
      >
        <el-form
          ref="formRef"
          :model="model"
          :rules="rules"
          label-width="110px"
          :disabled="saving"
        >
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item
                :label="t('product.spu.code')"
                prop="code"
              >
                <el-input
                  v-model="model.code"
                  :placeholder="t('product.spu.form.codePlaceholder')"
                />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item
                :label="t('product.spu.name')"
                prop="name"
              >
                <el-input
                  v-model="model.name"
                  :placeholder="t('product.spu.form.namePlaceholder')"
                />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item
                :label="t('product.spu.nameEn')"
                prop="nameEn"
              >
                <el-input
                  v-model="model.nameEn"
                  :placeholder="t('product.spu.form.nameEnPlaceholder')"
                />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item
                :label="t('product.spu.brand')"
                prop="brand"
              >
                <el-input
                  v-model="model.brand"
                  :placeholder="t('product.spu.form.brandPlaceholder')"
                />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="8">
              <el-form-item
                :label="t('product.spu.category')"
                prop="categoryId"
              >
                <el-tree-select
                  v-model="model.categoryId"
                  :data="categoryTreeData"
                  :props="{ children: 'children', label: 'name', value: 'id' }"
                  :placeholder="t('product.spu.form.categoryPlaceholder')"
                  check-strictly
                  clearable
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item
                :label="t('product.spu.type')"
                prop="type"
              >
                <el-select
                  v-model="model.type"
                  :placeholder="t('product.spu.form.typePlaceholder')"
                  style="width: 100%"
                >
                  <el-option
                    :label="t('product.spu.typeTicket')"
                    :value="'ticket'"
                  />
                  <el-option
                    :label="t('product.spu.typeMaterial')"
                    :value="'material'"
                  />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item
                :label="t('product.spu.unit')"
                prop="unit"
              >
                <el-input
                  v-model="model.unit"
                  :placeholder="t('product.spu.form.unitPlaceholder')"
                />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item
                :label="t('product.spu.mainImage')"
                prop="mainImage"
              >
                <ImageUpload v-model="model.mainImage" />
              </el-form-item>
            </el-col>
            <el-col
              v-if="isEdit"
              :span="12"
            >
              <el-form-item
                :label="t('product.spu.status')"
                prop="status"
              >
                <el-radio-group v-model="model.status">
                  <el-radio value="active">
                    {{ t('product.spu.statusActive') }}
                  </el-radio>
                  <el-radio value="inactive">
                    {{ t('product.spu.statusInactive') }}
                  </el-radio>
                </el-radio-group>
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
      </el-tab-pane>

      <el-tab-pane
        :label="t('product.spu.description')"
        name="desc"
      >
        <RichTextEditor
          v-model="model.description"
          height="300px"
          :disabled="saving"
        />
      </el-tab-pane>

      <el-tab-pane
        :label="t('product.spu.specs')"
        name="specs"
      >
        <div class="spec-section">
          <div class="spec-section__header">
            <span>{{ t('product.spec.title') }}</span>
            <el-button
              type="primary"
              size="small"
              @click="addSpecGroup"
            >
              {{ t('product.spec.addGroup') }}
            </el-button>
          </div>

          <div
            v-if="model.specs.length === 0"
            class="spec-section__empty"
          >
            {{ t('product.spec.empty') }}
          </div>

          <div
            v-for="(group, gi) in model.specs"
            :key="gi"
            class="spec-group"
          >
            <div class="spec-group__header">
              <el-input
                v-model="group.name"
                :placeholder="t('product.spec.groupNamePlaceholder')"
                size="small"
                style="width: 200px"
              />
              <el-button
                type="danger"
                size="small"
                link
                @click="removeSpecGroup(gi)"
              >
                {{ t('product.spec.removeGroup') }}
              </el-button>
            </div>
            <div class="spec-group__values">
              <el-tag
                v-for="(val, vi) in group.values"
                :key="vi"
                closable
                style="margin-right: 8px; margin-bottom: 4px"
                @close="removeSpecValue(gi, vi)"
              >
                {{ val }}
              </el-tag>
              <el-input
                v-if="specValueInputs[gi]?.show"
                v-model="specValueInputs[gi]!.text"
                size="small"
                style="width: 120px"
                @keyup.enter="confirmSpecValue(gi)"
                @blur="confirmSpecValue(gi)"
              />
              <el-button
                v-else
                size="small"
                @click="showSpecValueInput(gi)"
              >
                {{ t('product.spec.addValue') }}
              </el-button>
            </div>
          </div>
        </div>

        <div
          v-if="skuRows.length > 0"
          class="sku-section"
        >
          <div class="sku-section__header">
            <span>{{ t('product.sku.title') }}</span>
          </div>
          <el-table
            :data="skuRows"
            border
            size="small"
            style="width: 100%"
          >
            <el-table-column
              :label="t('product.sku.specValues')"
              min-width="150"
            >
              <template #default="{ row }">
                {{ formatSpecValues(row.specValues) }}
              </template>
            </el-table-column>
            <el-table-column
              :label="t('product.sku.barcode')"
              width="130"
            >
              <template #default="{ row }">
                <el-input
                  v-model="row.barcode"
                  size="small"
                />
              </template>
            </el-table-column>
            <el-table-column
              :label="t('product.sku.retailPrice')"
              width="110"
            >
              <template #default="{ row }">
                <el-input-number
                  v-model="row.retailPrice"
                  :controls="false"
                  :min="0"
                  :precision="2"
                  size="small"
                  style="width: 90px"
                />
              </template>
            </el-table-column>
            <el-table-column
              :label="t('product.sku.memberPrice')"
              width="110"
            >
              <template #default="{ row }">
                <el-input-number
                  v-model="row.memberPrice"
                  :controls="false"
                  :min="0"
                  :precision="2"
                  size="small"
                  style="width: 90px"
                />
              </template>
            </el-table-column>
            <el-table-column
              :label="t('product.sku.b2bPrice')"
              width="110"
            >
              <template #default="{ row }">
                <el-input-number
                  v-model="row.b2bPrice"
                  :controls="false"
                  :min="0"
                  :precision="2"
                  size="small"
                  style="width: 90px"
                />
              </template>
            </el-table-column>
            <el-table-column
              :label="t('product.sku.costPrice')"
              width="110"
            >
              <template #default="{ row }">
                <el-input-number
                  v-model="row.costPrice"
                  :controls="false"
                  :min="0"
                  :precision="2"
                  size="small"
                  style="width: 90px"
                />
              </template>
            </el-table-column>
            <el-table-column
              :label="t('product.sku.stock')"
              width="90"
            >
              <template #default="{ row }">
                <el-input-number
                  v-model="row.stock"
                  :controls="false"
                  :min="0"
                  :precision="0"
                  size="small"
                  style="width: 70px"
                />
              </template>
            </el-table-column>
          </el-table>
        </div>
      </el-tab-pane>
    </el-tabs>

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
import { createSpu, updateSpu, getSpuDetail, batchUpdateSkus } from '@/api/product/spu'
import type { ProductSpu, ProductCategory, SpecGroup, SpuType, SpuStatus, SkuStatus, ProductSku } from '@/types/product'
import ImageUpload from '@/components/common/ImageUpload.vue'
import RichTextEditor from '@/components/common/RichTextEditor.vue'

interface Props {
  visible: boolean
  isEdit: boolean
  rowData?: ProductSpu
  categories: ProductCategory[]
}

const props = withDefaults(defineProps<Props>(), {
  rowData: undefined,
})
const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)
const activeTab = ref('basic')
const loadedDetail = ref<ProductSpu | null>(null)

interface SpuFormModel {
  code: string
  name: string
  nameEn: string
  categoryId: string
  type: SpuType
  mainImage: string
  images: string[]
  description: string
  unit: string
  brand: string
  specs: SpecGroup[]
  status: SpuStatus
}

const model = reactive<SpuFormModel>({
  code: '', name: '', nameEn: '', categoryId: '', type: 'ticket',
  mainImage: '', images: [], description: '', unit: '', brand: '',
  specs: [], status: 'active',
})

interface SkuRow {
  id: string
  specValues: Record<string, string>
  barcode: string
  retailPrice: number
  memberPrice: number
  b2bPrice: number
  costPrice: number
  stock: number
  status: SkuStatus
}

const skuRows = ref<SkuRow[]>([])

const specValueInputs = ref<Array<{ text: string; show: boolean } | null>>([])

const rules: FormRules = {
  code: [{ required: true, message: t('product.spu.form.codeRequired'), trigger: 'blur' }],
  name: [{ required: true, message: t('product.spu.form.nameRequired'), trigger: 'blur' }],
  categoryId: [{ required: true, message: t('product.spu.form.categoryRequired'), trigger: 'change' }],
  type: [{ required: true, message: t('product.spu.form.typeRequired'), trigger: 'change' }],
}

interface CategoryNode extends ProductCategory {
  children?: CategoryNode[]
}

const categoryTreeData = computed<CategoryNode[]>(() => {
  return buildTree(props.categories)
})

function buildTree(list: ProductCategory[]): CategoryNode[] {
  const map = new Map<string, CategoryNode>()
  const roots: CategoryNode[] = []
  list.forEach(item => { map.set(item.id, { ...item, children: [] }) })
  map.forEach(node => {
    if (node.parentId && node.parentId !== '0') {
      const parent = map.get(node.parentId)
      if (parent) parent.children!.push(node)
      else roots.push(node)
    } else {
      roots.push(node)
    }
  })
  return roots
}

// ---- Spec management ----
const addSpecGroup = (): void => {
  model.specs.push({ name: '', values: [] })
  specValueInputs.value.push(null)
}

const removeSpecGroup = (gi: number): void => {
  model.specs.splice(gi, 1)
  specValueInputs.value.splice(gi, 1)
  regenerateSkus()
}

const showSpecValueInput = (gi: number): void => {
  specValueInputs.value[gi] = { text: '', show: true }
}

const confirmSpecValue = (gi: number): void => {
  const input = specValueInputs.value[gi]
  if (!input || !input.text.trim()) {
    if (input) input.show = false
    return
  }
  const group = model.specs[gi]
  if (!group.values.includes(input.text.trim())) {
    group.values.push(input.text.trim())
    regenerateSkus()
  }
  input.text = ''
  input.show = false
}

const removeSpecValue = (gi: number, vi: number): void => {
  model.specs[gi].values.splice(vi, 1)
  regenerateSkus()
}

function generateCombinations(specs: SpecGroup[]): Record<string, string>[] {
  const valid = specs.filter(s => s.name && s.values.length > 0)
  if (valid.length === 0) return []
  let result: Record<string, string>[] = [{}]
  for (const spec of valid) {
    const next: Record<string, string>[] = []
    for (const prev of result) {
      for (const val of spec.values) {
        next.push({ ...prev, [spec.name]: val })
      }
    }
    result = next
  }
  return result
}

const regenerateSkus = (): void => {
  const combos = generateCombinations(model.specs)
  const oldMap = new Map(skuRows.value.map(k => [JSON.stringify(k.specValues), k]))
  skuRows.value = combos.map(combo => {
    const existing = oldMap.get(JSON.stringify(combo))
    if (existing) return existing
    return {
      id: '',
      specValues: combo,
      barcode: '',
      retailPrice: 0,
      memberPrice: 0,
      b2bPrice: 0,
      costPrice: 0,
      stock: 0,
      status: 'active' as SkuStatus,
    }
  })
}

const formatSpecValues = (specValues: Record<string, string>): string => {
  return Object.values(specValues).join(' / ')
}

// ---- Submit ----
const handleSubmit = async (): Promise<void> => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) {
    activeTab.value = 'basic'
    return
  }
  saving.value = true
  try {
    if (props.isEdit && props.rowData) {
      await updateSpu(props.rowData.id, model)
      ElMessage.success(t('product.spu.message.updateSuccess'))
      await syncSkusAfterSave(props.rowData.id)
    } else {
      const result = await createSpu(model)
      ElMessage.success(t('product.spu.message.createSuccess'))
      await syncSkusAfterSave(result.id)
    }
    emit('success')
    handleClose()
  } catch {
    // handled by interceptor
  } finally {
    saving.value = false
  }
}

const syncSkusAfterSave = async (spuId: string): Promise<void> => {
  if (skuRows.value.length === 0) return
  try {
    const detail = await getSpuDetail(spuId)
    const serverSkus = detail.skus || []
    const toUpdate: ProductSku[] = []
    for (const row of skuRows.value) {
      const match = serverSkus.find(s => JSON.stringify(s.specValues) === JSON.stringify(row.specValues))
      if (match) {
        toUpdate.push({
          ...match,
          barcode: row.barcode,
          retailPrice: row.retailPrice,
          memberPrice: row.memberPrice,
          b2bPrice: row.b2bPrice,
          costPrice: row.costPrice,
          stock: row.stock,
          status: row.status,
        })
      }
    }
    if (toUpdate.length > 0) {
      await batchUpdateSkus(toUpdate)
    }
  } catch {
    // SKU sync is best-effort
  }
}

const handleClose = (): void => {
  formRef.value?.resetFields()
  Object.assign(model, {
    code: '', name: '', nameEn: '', categoryId: '', type: 'ticket',
    mainImage: '', images: [], description: '', unit: '', brand: '',
    specs: [], status: 'active',
  })
  skuRows.value = []
  specValueInputs.value = []
  activeTab.value = 'basic'
  loadedDetail.value = null
  emit('update:visible', false)
}

// ---- Watch visible ----
watch(
  () => props.visible,
  async (val) => {
    if (!val) return
    if (props.isEdit && props.rowData) {
      const row = props.rowData
      model.code = row.code
      model.name = row.name
      model.nameEn = row.nameEn
      model.categoryId = row.categoryId
      model.type = row.type
      model.mainImage = row.mainImage
      model.images = [...row.images]
      model.description = row.description
      model.unit = row.unit
      model.brand = row.brand
      model.specs = row.specs.map(s => ({ ...s, values: [...s.values] }))
      model.status = row.status
      try {
        const detail = await getSpuDetail(row.id)
        loadedDetail.value = detail
        skuRows.value = (detail.skus || []).map(s => ({
          id: s.id,
          specValues: { ...s.specValues },
          barcode: s.barcode,
          retailPrice: s.retailPrice,
          memberPrice: s.memberPrice,
          b2bPrice: s.b2bPrice,
          costPrice: s.costPrice,
          stock: s.stock,
          status: s.status,
        }))
      } catch {
        skuRows.value = []
      }
    }
  },
)
</script>

<style scoped lang="scss">
.spec-section {
  &__header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 12px;
    font-weight: 600;
  }

  &__empty {
    color: #909399;
    text-align: center;
    padding: 24px 0;
  }
}

.spec-group {
  border: 1px solid var(--el-border-color-lighter);
  border-radius: 4px;
  padding: 12px;
  margin-bottom: 12px;

  &__header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 8px;
  }

  &__values {
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    gap: 4px;
  }
}

.sku-section {
  margin-top: 20px;

  &__header {
    font-weight: 600;
    margin-bottom: 8px;
  }
}
</style>
