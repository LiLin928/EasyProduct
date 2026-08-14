<!-- src/views/basic/dept/index.vue -->
<template>
  <div class="dept-container">
    <el-row :gutter="20">
      <!-- 左侧：部门树 -->
      <el-col :span="5">
        <el-card
          v-loading="loading"
          shadow="never"
          class="tree-card"
        >
          <template #header>
            <div class="tree-header">
              <span>{{ t('basic.dept.tree.title') }}</span>
              <el-button
                type="primary"
                size="small"
                @click="handleAddRoot"
              >
                {{ t('basic.dept.tree.addRoot') }}
              </el-button>
            </div>
          </template>

          <el-input
            v-model="filterText"
            :placeholder="t('basic.dept.tree.searchPlaceholder')"
            clearable
            class="tree-search"
          />

          <el-tree
            ref="treeRef"
            :data="treeData"
            :props="{ children: 'children', label: 'name' }"
            node-key="id"
            highlight-current
            :expand-on-click-node="false"
            :filter-node-method="filterNode"
            default-expand-all
            @node-click="handleNodeClick"
          >
            <template #default="{ data }">
              <span class="tree-node">
                <span class="node-label">{{ data.name }}</span>
                <span
                  v-if="data.memberCount > 0"
                  class="node-count"
                >
                  ({{ data.memberCount }})
                </span>
                <span class="tree-actions">
                  <el-button
                    link
                    size="small"
                    type="primary"
                    @click.stop="handleAdd(data)"
                  >
                    {{ t('basic.dept.tree.addChild') }}
                  </el-button>
                  <el-button
                    link
                    size="small"
                    type="primary"
                    @click.stop="handleEdit(data)"
                  >
                    {{ t('basic.dept.tree.edit') }}
                  </el-button>
                  <el-button
                    link
                    size="small"
                    type="danger"
                    :disabled="data.children && data.children.length > 0"
                    @click.stop="handleDelete(data)"
                  >
                    {{ t('basic.dept.tree.delete') }}
                  </el-button>
                </span>
              </span>
            </template>
          </el-tree>
        </el-card>
      </el-col>

      <!-- 右侧：部门详情/成员列表 -->
      <el-col :span="19">
        <el-card
          shadow="never"
          class="detail-card"
        >
          <template #header>
            <div class="detail-header">
              <span>{{ currentDept?.name || t('basic.dept.detail.selectDept') }}</span>
              <el-button
                v-if="currentDept"
                type="primary"
                size="small"
                @click="handleEditDetail"
              >
                {{ t('basic.dept.tree.edit') }}
              </el-button>
            </div>
          </template>

          <!-- 部门详情 -->
          <div
            v-if="currentDept"
            class="dept-info"
          >
            <el-descriptions
              :column="2"
              border
            >
              <el-descriptions-item :label="t('basic.dept.detail.name')">
                {{ currentDept.name }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.code')">
                {{ currentDept.code || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.path')">
                {{ currentDept.fullPath || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.level')">
                {{ currentDept.level }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.leader')">
                {{ currentDept.leaderName || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.phone')">
                {{ currentDept.phone || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.email')">
                {{ currentDept.email || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.memberCount')">
                {{ currentDept.memberCount }}
              </el-descriptions-item>
              <el-descriptions-item
                :label="t('basic.dept.detail.description')"
                :span="2"
              >
                {{ currentDept.description || '-' }}
              </el-descriptions-item>
            </el-descriptions>
          </div>

          <!-- 成员列表 -->
          <div
            v-if="currentDept"
            class="member-section"
          >
            <div class="section-header">
              <span>{{ t('basic.dept.member.title') }}</span>
            </div>

            <el-table
              v-loading="memberLoading"
              :data="memberList"
              border
              stripe
            >
              <el-table-column
                prop="userName"
                :label="t('basic.dept.member.userName')"
                width="120"
              />
              <el-table-column
                prop="realName"
                :label="t('basic.dept.member.realName')"
                width="120"
              />
              <el-table-column
                prop="phone"
                :label="t('basic.dept.member.phone')"
                width="150"
              />
              <el-table-column
                prop="email"
                :label="t('basic.dept.member.email')"
                width="200"
              />
              <el-table-column
                :label="t('basic.dept.member.status')"
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
                :label="t('basic.dept.member.roles')"
                min-width="150"
              >
                <template #default="{ row }">
                  <el-tag
                    v-for="role in row.roleNames"
                    :key="role"
                    type="primary"
                    effect="plain"
                    size="small"
                    style="margin-right: 4px"
                  >
                    {{ role }}
                  </el-tag>
                  <span v-if="!row.roleNames?.length">-</span>
                </template>
              </el-table-column>
            </el-table>
          </div>

          <!-- 无数据提示 -->
          <el-empty
            v-if="!currentDept"
            :description="t('basic.dept.detail.selectDeptTip')"
          />
        </el-card>
      </el-col>
    </el-row>

    <!-- 部门表单弹窗 -->
    <DeptFormDialog
      v-model="dialogVisible"
      :dept-id="currentDeptId"
      :parent-id="currentParentId"
      :tree-data="treeData"
      @success="handleDialogSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, nextTick } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { ElTree } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { getDeptTree, getDeptUsers, deleteDept } from '@/api/basic/dept'
import type { Dept, User } from '@/types/basic'
import DeptFormDialog from './components/DeptFormDialog.vue'

const { t } = useLocale()

// 树相关
const treeRef = ref<InstanceType<typeof ElTree>>()
const treeData = ref<Dept[]>([])
const filterText = ref('')
const loading = ref(false)

// 当前选中的部门
const currentDept = ref<Dept | null>(null)
const memberList = ref<User[]>([])
const memberLoading = ref(false)

// 弹窗相关
const dialogVisible = ref(false)
const currentDeptId = ref<string | undefined>(undefined)
const currentParentId = ref<string | undefined>(undefined)

// 保存后需要重新选中的部门ID
const refreshSelectDeptId = ref<string | null>(null)

/**
 * 过滤树节点
 * @param value 搜索值
 * @param data 节点数据
 * @returns 是否匹配
 */
const filterNode = (value: string, data: Dept): boolean => {
  if (!value) return true
  return data.name?.includes(value)
}

// 监听搜索框变化，触发树过滤
watch(filterText, (val) => {
  treeRef.value?.filter(val)
})

// 页面加载时获取部门树
onMounted(() => {
  loadTree()
})

/**
 * 加载部门树
 */
const loadTree = async (): Promise<void> => {
  loading.value = true
  try {
    const data = await getDeptTree()
    treeData.value = data

    // 如果有需要重新选中的部门ID，刷新后重新选中
    if (refreshSelectDeptId.value) {
      // 使用 nextTick 确保 DOM 更新后再选中
      await nextTick()
      treeRef.value?.setCurrentKey(refreshSelectDeptId.value)
      // 查找并设置当前部门详情
      const node = findDeptById(treeData.value, refreshSelectDeptId.value!)
      if (node) {
        currentDept.value = node
        // 重新加载成员列表
        loadMembers(node.id)
      }
      refreshSelectDeptId.value = null
    }
  } catch (error) {
    // 错误已在拦截器处理
  } finally {
    loading.value = false
  }
}

/**
 * 根据ID在树中查找部门
 * @param list 部门列表
 * @param id 部门ID
 * @returns 部门对象或null
 */
const findDeptById = (list: Dept[], id: string): Dept | null => {
  for (const item of list) {
    if (item.id === id) return item
    if (item.children) {
      const found = findDeptById(item.children, id)
      if (found) return found
    }
  }
  return null
}

/**
 * 加载部门成员
 * @param deptId 部门ID
 */
const loadMembers = async (deptId: string): Promise<void> => {
  memberLoading.value = true
  try {
    const users = await getDeptUsers(deptId)
    memberList.value = users
  } catch (error) {
    memberList.value = []
  } finally {
    memberLoading.value = false
  }
}

/**
 * 点击树节点
 * @param data 部门数据
 */
const handleNodeClick = (data: Dept): void => {
  currentDept.value = data
  loadMembers(data.id)
}

/**
 * 新增根部门
 */
const handleAddRoot = (): void => {
  currentDeptId.value = undefined
  currentParentId.value = undefined
  dialogVisible.value = true
}

/**
 * 新增子部门
 * @param data 父部门数据
 */
const handleAdd = (data: Dept): void => {
  currentDeptId.value = undefined
  currentParentId.value = data.id
  dialogVisible.value = true
}

/**
 * 编辑部门
 * @param data 部门数据
 */
const handleEdit = (data: Dept): void => {
  currentDeptId.value = data.id
  currentParentId.value = undefined
  refreshSelectDeptId.value = data.id
  dialogVisible.value = true
}

/**
 * 从右侧详情区编辑当前部门
 */
const handleEditDetail = (): void => {
  if (currentDept.value) {
    currentDeptId.value = currentDept.value.id
    currentParentId.value = undefined
    refreshSelectDeptId.value = currentDept.value.id
    dialogVisible.value = true
  }
}

/**
 * 弹窗保存成功后的处理
 */
const handleDialogSuccess = (): void => {
  loadTree()
}

/**
 * 删除部门
 * @param data 部门数据
 */
const handleDelete = async (data: Dept): Promise<void> => {
  if (data.children && data.children.length > 0) {
    ElMessage.warning(t('basic.dept.message.hasChildren'))
    return
  }

  try {
    await ElMessageBox.confirm(
      t('basic.dept.message.deleteConfirm', { name: data.name }),
      t('common.tips'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteDept(data.id)
    ElMessage.success(t('basic.dept.message.deleteSuccess'))
    // 如果删除的是当前选中的部门，清空右侧
    if (currentDept.value?.id === data.id) {
      currentDept.value = null
      memberList.value = []
    }
    loadTree()
  } catch (error) {
    // 用户取消或请求失败
  }
}
</script>

<style scoped lang="scss">
.dept-container {
  padding: 20px;
  height: calc(100vh - 100px);

  .el-row {
    height: 100%;
  }

  .tree-card {
    height: 100%;

    .tree-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .tree-search {
      margin-bottom: 12px;
    }

    .tree-node {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: space-between;
      font-size: 14px;
      padding-right: 8px;

      .node-label {
        flex: 1;
      }

      .node-count {
        color: #909399;
        font-size: 12px;
        margin-left: 4px;
      }

      .tree-actions {
        display: none;
      }
    }

    :deep(.el-tree-node__content:hover) .tree-actions {
      display: inline-flex;
    }
  }

  .detail-card {
    height: 100%;

    .detail-header {
      font-size: 16px;
      font-weight: 500;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .dept-info {
      margin-bottom: 20px;
    }

    .member-section {
      .section-header {
        font-size: 14px;
        font-weight: 500;
        margin-bottom: 12px;
        padding-bottom: 8px;
        border-bottom: 1px solid #ebeef5;
      }
    }
  }
}
</style>