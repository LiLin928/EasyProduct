<!-- src/views/basic/dict/index.vue -->
<template>
  <div class="dict-container">
    <el-row :gutter="20">
      <!-- 左侧：字典类型列表 -->
      <el-col :span="6">
        <el-card
          v-loading="typeLoading"
          shadow="never"
          class="type-card"
        >
          <template #header>
            <div class="type-header">
              <span>{{ t('basic.dict.type.title') }}</span>
              <el-button
                type="primary"
                size="small"
                @click="handleAddType"
              >
                {{ t('common.add') }}
              </el-button>
            </div>
          </template>

          <el-input
            v-model="typeFilterText"
            :placeholder="t('basic.dict.type.searchPlaceholder')"
            clearable
            class="type-search"
          />

          <el-table
            :data="filteredTypeList"
            highlight-current-row
            @current-change="handleTypeSelect"
          >
            <el-table-column
              prop="name"
              :label="t('basic.dict.type.name')"
            >
              <template #default="{ row }">
                <div class="type-name">
                  <span>{{ row.name }}</span>
                  <span class="type-code">({{ row.code }})</span>
                </div>
              </template>
            </el-table-column>
            <el-table-column
              :label="t('basic.dict.type.status')"
              width="80"
              align="center"
            >
              <template #default="{ row }">
                <el-tag
                  :type="row.status === 'enabled' ? 'success' : 'danger'"
                  size="small"
                >
                  {{ row.status === 'enabled' ? t('common.status.enabled') : t('common.status.disabled') }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column
              :label="t('common.actions')"
              width="100"
              align="center"
            >
              <template #default="{ row }">
                <el-button
                  link
                  size="small"
                  type="primary"
                  @click.stop="handleEditType(row)"
                >
                  {{ t('common.edit') }}
                </el-button>
                <el-button
                  link
                  size="small"
                  type="danger"
                  @click.stop="handleDeleteType(row)"
                >
                  {{ t('common.delete') }}
                </el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>

      <!-- 右侧：字典数据表格 -->
      <el-col :span="18">
        <el-card
          shadow="never"
          class="data-card"
        >
          <template #header>
            <div class="data-header">
              <span>
                {{ currentType ? `${currentType.name} - ${t('basic.dict.data.title')}` : t('basic.dict.data.selectType') }}
              </span>
              <el-button
                v-if="currentType"
                type="primary"
                size="small"
                @click="handleAddData"
              >
                {{ t('common.add') }}
              </el-button>
            </div>
          </template>

          <!-- 字典数据表格 -->
          <div v-if="currentType">
            <el-table
              v-loading="dataLoading"
              :data="dataList"
              border
              stripe
            >
              <el-table-column
                prop="labelKey"
                :label="t('basic.dict.data.label')"
                min-width="150"
              />
              <el-table-column
                prop="value"
                :label="t('basic.dict.data.value')"
                width="120"
              />
              <el-table-column
                prop="sort"
                :label="t('basic.dict.data.sort')"
                width="100"
                align="center"
              />
              <el-table-column
                :label="t('basic.dict.data.status')"
                width="100"
                align="center"
              >
                <template #default="{ row }">
                  <el-tag
                    :type="row.status === 'enabled' ? 'success' : 'danger'"
                    size="small"
                  >
                    {{ row.status === 'enabled' ? t('common.status.enabled') : t('common.status.disabled') }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column
                :label="t('common.actions')"
                width="150"
                align="center"
              >
                <template #default="{ row }">
                  <el-button
                    link
                    size="small"
                    type="primary"
                    @click="handleEditData(row)"
                  >
                    {{ t('common.edit') }}
                  </el-button>
                  <el-button
                    link
                    size="small"
                    type="danger"
                    @click="handleDeleteData(row)"
                  >
                    {{ t('common.delete') }}
                  </el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>

          <!-- 无数据提示 -->
          <el-empty
            v-if="!currentType"
            :description="t('basic.dict.data.selectTypeTip')"
          />
        </el-card>
      </el-col>
    </el-row>

    <!-- 字典类型表单弹窗 -->
    <DictTypeFormDialog
      v-model="typeDialogVisible"
      :type-id="currentTypeId"
      @success="handleTypeDialogSuccess"
    />

    <!-- 字典数据表单弹窗 -->
    <DictDataFormDialog
      v-model="dataDialogVisible"
      :data-id="currentDataId"
      :type-code="currentType?.code || ''"
      @success="handleDataDialogSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import {
  getDictTypeList,
  getDictDataList,
  deleteDictType,
  deleteDictData
} from '@/api/basic/dict'
import type { DictType, DictData } from '@/types/basic'
import DictTypeFormDialog from './components/DictTypeFormDialog.vue'
import DictDataFormDialog from './components/DictDataFormDialog.vue'

const { t } = useLocale()

// 字典类型相关
const typeList = ref<DictType[]>([])
const typeLoading = ref(false)
const typeFilterText = ref('')

// 当前选中的字典类型
const currentType = ref<DictType | null>(null)

// 字典数据相关
const dataList = ref<DictData[]>([])
const dataLoading = ref(false)

// 弹窗相关
const typeDialogVisible = ref(false)
const currentTypeId = ref<string | undefined>(undefined)
const dataDialogVisible = ref(false)
const currentDataId = ref<string | undefined>(undefined)

// 过滤后的字典类型列表
const filteredTypeList = computed(() => {
  if (!typeFilterText.value) return typeList.value
  return typeList.value.filter(
    type =>
      type.name.includes(typeFilterText.value) ||
      type.code.includes(typeFilterText.value)
  )
})

// 页面加载时获取字典类型列表
onMounted(() => {
  loadTypeList()
})

/**
 * 加载字典类型列表
 */
const loadTypeList = async (): Promise<void> => {
  typeLoading.value = true
  try {
    const result = await getDictTypeList({ pageIndex: 1, pageSize: 1000 })
    typeList.value = result.list
  } catch (error) {
    // 错误已在拦截器处理
  } finally {
    typeLoading.value = false
  }
}

/**
 * 加载字典数据列表
 * @param typeCode 字典类型编码
 */
const loadDataList = async (typeCode: string): Promise<void> => {
  dataLoading.value = true
  try {
    const result = await getDictDataList({ typeCode, pageIndex: 1, pageSize: 1000 })
    dataList.value = result.list
  } catch (error) {
    dataList.value = []
  } finally {
    dataLoading.value = false
  }
}

/**
 * 选择字典类型
 * @param type 字典类型
 */
const handleTypeSelect = (type: DictType | null): void => {
  currentType.value = type
  if (type) {
    loadDataList(type.code)
  } else {
    dataList.value = []
  }
}

/**
 * 新增字典类型
 */
const handleAddType = (): void => {
  currentTypeId.value = undefined
  typeDialogVisible.value = true
}

/**
 * 编辑字典类型
 * @param type 字典类型
 */
const handleEditType = (type: DictType): void => {
  currentTypeId.value = type.id
  typeDialogVisible.value = true
}

/**
 * 删除字典类型
 * @param type 字典类型
 */
const handleDeleteType = async (type: DictType): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.dict.type.message.deleteConfirm', { name: type.name }),
      t('common.tips'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning'
      }
    )
    await deleteDictType(type.id)
    ElMessage.success(t('basic.dict.type.message.deleteSuccess'))
    // 如果删除的是当前选中的类型，清空右侧
    if (currentType.value?.id === type.id) {
      currentType.value = null
      dataList.value = []
    }
    loadTypeList()
  } catch (error) {
    // 用户取消或请求失败
  }
}

/**
 * 字典类型弹窗保存成功后的处理
 */
const handleTypeDialogSuccess = (): void => {
  loadTypeList()
}

/**
 * 新增字典数据
 */
const handleAddData = (): void => {
  currentDataId.value = undefined
  dataDialogVisible.value = true
}

/**
 * 编辑字典数据
 * @param data 字典数据
 */
const handleEditData = (data: DictData): void => {
  currentDataId.value = data.id
  dataDialogVisible.value = true
}

/**
 * 删除字典数据
 * @param data 字典数据
 */
const handleDeleteData = async (data: DictData): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.dict.data.message.deleteConfirm', { label: data.labelKey }),
      t('common.tips'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning'
      }
    )
    await deleteDictData(data.id)
    ElMessage.success(t('basic.dict.data.message.deleteSuccess'))
    // 重新加载数据列表
    if (currentType.value) {
      loadDataList(currentType.value.code)
    }
  } catch (error) {
    // 用户取消或请求失败
  }
}

/**
 * 字典数据弹窗保存成功后的处理
 */
const handleDataDialogSuccess = (): void => {
  if (currentType.value) {
    loadDataList(currentType.value.code)
  }
}
</script>

<style scoped lang="scss">
.dict-container {
  padding: 20px;
  height: calc(100vh - 100px);

  .el-row {
    height: 100%;
  }

  .type-card {
    height: 100%;

    .type-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .type-search {
      margin-bottom: 12px;
    }

    .type-name {
      display: flex;
      flex-direction: column;

      .type-code {
        font-size: 12px;
        color: #909399;
      }
    }
  }

  .data-card {
    height: 100%;

    .data-header {
      font-size: 16px;
      font-weight: 500;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
  }
}
</style>