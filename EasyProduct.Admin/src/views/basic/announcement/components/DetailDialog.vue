<template>
  <el-dialog
    :model-value="visible"
    :title="t('basic.announcement.view')"
    width="60%"
    @update:model-value="handleClose"
  >
    <el-descriptions
      v-if="announcement"
      :column="2"
      border
    >
      <el-descriptions-item
        :label="t('basic.announcement.announcementTitle')"
        :span="2"
      >
        {{ announcement.title }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('basic.announcement.type')">
        {{ announcement.type === 'all' ? t('basic.announcement.typeAll') : t('basic.announcement.typeTargeted') }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('basic.announcement.level')">
        <el-tag :type="getLevelType(announcement.level)">
          {{ getLevelLabel(announcement.level) }}
        </el-tag>
      </el-descriptions-item>
      <el-descriptions-item :label="t('basic.announcement.status')">
        <el-tag :type="getStatusType(announcement.status)">
          {{ getStatusLabel(announcement.status) }}
        </el-tag>
      </el-descriptions-item>
      <el-descriptions-item :label="t('basic.announcement.isTop')">
        {{ announcement.isTop ? t('common.yes') : t('common.no') }}
      </el-descriptions-item>
      <el-descriptions-item
        :label="t('basic.announcement.content')"
        :span="2"
      >
        <div v-html="announcement.content" />
      </el-descriptions-item>
      <el-descriptions-item
        v-if="announcement.publishTime"
        :label="t('basic.announcement.publishTime')"
      >
        {{ announcement.publishTime }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('basic.announcement.creator')">
        {{ announcement.creatorName }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('basic.announcement.createdAt')">
        {{ announcement.createdAt }}
      </el-descriptions-item>
    </el-descriptions>

    <template #footer>
      <el-button @click="handleClose">
        {{ t('common.close') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { getAnnouncementById } from '@/api/basic/announcement'
import type { Announcement, AnnouncementLevel, AnnouncementStatus } from '@/types/announcement'

interface Props {
  visible: boolean
  id?: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
}>()

const { t } = useI18n()

const announcement = ref<Announcement | null>(null)

// 监听 visible 变化，加载详情
watch(
  () => props.visible,
  async (visible) => {
    if (visible && props.id) {
      try {
        announcement.value = await getAnnouncementById(props.id)
      } catch (error) {
        announcement.value = null
      }
    } else {
      announcement.value = null
    }
  }
)

// 关闭弹窗
const handleClose = (): void => {
  emit('update:visible', false)
}

// 获取级别标签类型
const getLevelType = (level: AnnouncementLevel): 'info' | 'warning' | 'danger' => {
  const map: Record<AnnouncementLevel, 'info' | 'warning' | 'danger'> = {
    normal: 'info',
    important: 'warning',
    urgent: 'danger'
  }
  return map[level] || 'info'
}

// 获取级别标签文本
const getLevelLabel = (level: AnnouncementLevel): string => {
  const map: Record<AnnouncementLevel, string> = {
    normal: t('basic.announcement.levelNormal'),
    important: t('basic.announcement.levelImportant'),
    urgent: t('basic.announcement.levelUrgent')
  }
  return map[level] || level
}

// 获取状态标签类型
const getStatusType = (status: AnnouncementStatus): 'info' | 'success' | 'info' => {
  const map: Record<AnnouncementStatus, 'info' | 'success' | 'info'> = {
    draft: 'info',
    published: 'success',
    recalled: 'info'
  }
  return map[status] || 'info'
}

// 获取状态标签文本
const getStatusLabel = (status: AnnouncementStatus): string => {
  const map: Record<AnnouncementStatus, string> = {
    draft: t('basic.announcement.statusDraft'),
    published: t('basic.announcement.statusPublished'),
    recalled: t('basic.announcement.statusRecalled')
  }
  return map[status] || status
}
</script>

<style scoped lang="scss">
</style>