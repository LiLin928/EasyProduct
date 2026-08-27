<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('basic.announcement.edit') : t('basic.announcement.add')"
    width="80%"
    top="5vh"
    @update:model-value="handleClose"
  >
    <el-form
      ref="formRef"
      :model="model"
      :rules="rules"
      label-width="120px"
    >
      <el-form-item
        :label="t('basic.announcement.announcementTitle')"
        prop="title"
      >
        <el-input
          v-model="model.title"
          :placeholder="t('basic.announcement.form.titlePlaceholder')"
          maxlength="200"
          show-word-limit
        />
      </el-form-item>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item
            :label="t('basic.announcement.type')"
            prop="type"
          >
            <el-radio-group
              v-model="model.type"
              @change="handleTypeChange"
            >
              <el-radio value="all">
                {{ t('basic.announcement.typeAll') }}
              </el-radio>
              <el-radio value="targeted">
                {{ t('basic.announcement.typeTargeted') }}
              </el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item
            :label="t('basic.announcement.level')"
            prop="level"
          >
            <el-radio-group v-model="model.level">
              <el-radio value="normal">
                {{ t('basic.announcement.levelNormal') }}
              </el-radio>
              <el-radio value="important">
                {{ t('basic.announcement.levelImportant') }}
              </el-radio>
              <el-radio value="urgent">
                {{ t('basic.announcement.levelUrgent') }}
              </el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item
        v-if="model.type === 'targeted'"
        :label="t('basic.announcement.targetRoles')"
        prop="targetRoleIds"
      >
        <el-select
          v-model="model.targetRoleIds"
          multiple
          :placeholder="t('basic.announcement.targetRolesPlaceholder')"
          style="width: 100%"
        >
          <el-option
            v-for="role in roleList"
            :key="role.id"
            :label="role.name"
            :value="role.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item
        :label="t('basic.announcement.content')"
        prop="content"
      >
        <div style="width: 100%; border: 1px solid #ccc; border-radius: 4px;">
          <Toolbar
            style="border-bottom: 1px solid #ccc"
            :editor="editorRef"
            :default-config="toolbarConfig"
            mode="default"
          />
          <Editor
            v-model="model.content"
            style="height: 400px; overflow-y: hidden"
            :default-config="editorConfig"
            mode="default"
            @onCreated="handleEditorCreated"
          />
        </div>
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose">
        {{ t('common.cancel') }}
      </el-button>
      <el-button
        type="info"
        :loading="draftLoading"
        @click="handleSaveDraft"
      >
        {{ t('basic.announcement.saveDraft') }}
      </el-button>
      <el-button
        type="primary"
        :loading="publishLoading"
        @click="handlePublish"
      >
        {{ t('basic.announcement.publishNow') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, shallowRef, onBeforeUnmount } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import '@wangeditor/editor/dist/css/style.css'
import type { IDomEditor, IEditorConfig, IToolbarConfig } from '@wangeditor/editor'
import {
  createAnnouncement,
  updateAnnouncement,
  getAnnouncementById
} from '@/api/basic/announcement'
import { getRoleList } from '@/api/basic/role'
import type { Announcement, AnnouncementType, AnnouncementLevel } from '@/types/announcement'
import type { Role } from '@/types/basic'

interface Props {
  visible: boolean
  id?: string
  isEdit: boolean
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const editorRef = shallowRef<IDomEditor>()
const roleList = ref<Role[]>([])
const draftLoading = ref(false)
const publishLoading = ref(false)

// 表单模型
const model = reactive<{
  title: string
  type: AnnouncementType
  level: AnnouncementLevel
  targetRoleIds: string[]
  content: string
}>({
  title: '',
  type: 'all',
  level: 'normal',
  targetRoleIds: [],
  content: ''
})

// 表单验证规则
const rules: FormRules = {
  title: [
    { required: true, message: t('basic.announcement.form.titleRequired'), trigger: 'blur' },
    { min: 2, max: 200, message: t('basic.announcement.form.titleLength'), trigger: 'blur' }
  ],
  type: [
    { required: true, message: t('basic.announcement.form.typeRequired'), trigger: 'change' }
  ],
  level: [
    { required: true, message: t('basic.announcement.form.levelRequired'), trigger: 'change' }
  ],
  targetRoleIds: [
    {
      validator: (rule, value, callback) => {
        if (model.type === 'targeted' && (!value || value.length === 0)) {
          callback(new Error(t('basic.announcement.form.targetRolesRequired')))
        } else {
          callback()
        }
      },
      trigger: 'change'
    }
  ],
  content: [
    { required: true, message: t('basic.announcement.form.contentRequired'), trigger: 'blur' }
  ]
}

// 编辑器配置
const toolbarConfig: Partial<IToolbarConfig> = {
  excludeKeys: [
    'group-video'
  ]
}

const editorConfig: Partial<IEditorConfig> = {
  placeholder: t('basic.announcement.form.contentRequired'),
  MENU_CONF: {
    uploadImage: {
      fieldName: 'file',
      server: '/api/admin/file/upload',
      // 自定义插入图片
      customInsert(res: any, insertFn: any) {
        if (res.code === 200 && res.data) {
          insertFn(res.data.url, res.data.name, res.data.url)
        }
      }
    }
  }
}

// 编辑器创建完成
const handleEditorCreated = (editor: IDomEditor) => {
  editorRef.value = editor
}

// 公告类型变化
const handleTypeChange = () => {
  if (model.type === 'all') {
    model.targetRoleIds = []
  }
}

// 加载角色列表
const loadRoleList = async () => {
  try {
    const res = await getRoleList({ pageIndex: 1, pageSize: 1000 })
    roleList.value = res.list
  } catch (error) {
    console.error('加载角色列表失败:', error)
  }
}

// 加载公告详情
const loadAnnouncementDetail = async () => {
  if (!props.id) return

  try {
    const data = await getAnnouncementById(props.id)
    model.title = data.title
    model.type = data.type
    model.level = data.level
    model.targetRoleIds = data.targetRoleIds || []
    model.content = data.content
  } catch (error) {
    console.error('加载公告详情失败:', error)
  }
}

// 保存草稿
const handleSaveDraft = async () => {
  try {
    await formRef.value?.validate()
    draftLoading.value = true

    if (props.isEdit && props.id) {
      await updateAnnouncement(props.id, {
        title: model.title,
        content: model.content,
        type: model.type,
        level: model.level,
        targetRoleIds: model.targetRoleIds
      })
      ElMessage.success(t('basic.announcement.message.updateSuccess'))
    } else {
      await createAnnouncement({
        title: model.title,
        content: model.content,
        type: model.type,
        level: model.level,
        targetRoleIds: model.targetRoleIds
      })
      ElMessage.success(t('basic.announcement.message.createSuccess'))
    }

    emit('success')
    handleClose()
  } catch (error) {
    console.error('保存失败:', error)
  } finally {
    draftLoading.value = false
  }
}

// 立即发布
const handlePublish = async () => {
  try {
    await formRef.value?.validate()
    publishLoading.value = true

    // 先创建/更新，再发布
    if (props.isEdit && props.id) {
      await updateAnnouncement(props.id, {
        title: model.title,
        content: model.content,
        type: model.type,
        level: model.level,
        targetRoleIds: model.targetRoleIds
      })
    } else {
      await createAnnouncement({
        title: model.title,
        content: model.content,
        type: model.type,
        level: model.level,
        targetRoleIds: model.targetRoleIds
      })
    }

    ElMessage.success(t('basic.announcement.message.publishSuccess'))
    emit('success')
    handleClose()
  } catch (error) {
    console.error('发布失败:', error)
  } finally {
    publishLoading.value = false
  }
}

// 关闭弹窗
const handleClose = () => {
  formRef.value?.resetFields()
  model.title = ''
  model.type = 'all'
  model.level = 'normal'
  model.targetRoleIds = []
  model.content = ''
  emit('update:visible', false)
}

// 监听 visible 变化
watch(
  () => props.visible,
  async (visible) => {
    if (visible) {
      await loadRoleList()
      if (props.isEdit && props.id) {
        await loadAnnouncementDetail()
      }
    }
  }
)

// 组件销毁前清理编辑器
onBeforeUnmount(() => {
  const editor = editorRef.value
  if (editor) {
    editor.destroy()
  }
})
</script>

<style scoped lang="scss">
</style>