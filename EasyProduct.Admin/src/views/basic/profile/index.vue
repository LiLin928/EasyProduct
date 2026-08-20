<!-- src/views/basic/profile/index.vue -->
<template>
  <div class="profile-page">
    <el-card
      shadow="never"
      class="profile-page__card"
    >
      <el-row :gutter="16">
        <!-- 左侧：头像 -->
        <el-col
          :span="6"
          class="profile-page__avatar"
        >
          <ImageUpload
            v-model="basicForm.avatar"
            circle
          />
          <div class="avatar-name">
            {{ userStore.realName }}
          </div>
        </el-col>

        <!-- 右侧：tab -->
        <el-col :span="18">
          <el-tabs v-model="activeTab">
            <!-- 基本信息 -->
            <el-tab-pane
              :label="t('basic.profile.basicInfo')"
              name="basic"
            >
              <el-form
                ref="basicFormRef"
                :model="basicForm"
                :rules="basicRules"
                label-width="100px"
              >
                <el-form-item :label="t('basic.profile.userName')">
                  <el-input
                    :model-value="basicForm.userName"
                    disabled
                  />
                </el-form-item>
                <el-form-item
                  :label="t('basic.profile.realName')"
                  prop="realName"
                >
                  <el-input v-model="basicForm.realName" />
                </el-form-item>
                <el-form-item
                  :label="t('basic.profile.phone')"
                  prop="phone"
                >
                  <el-input v-model="basicForm.phone" />
                </el-form-item>
                <el-form-item
                  :label="t('basic.profile.email')"
                  prop="email"
                >
                  <el-input v-model="basicForm.email" />
                </el-form-item>
                <el-form-item>
                  <el-button
                    type="primary"
                    :loading="saving"
                    @click="handleSaveBasic"
                  >
                    {{ t('common.button.confirm') }}
                  </el-button>
                </el-form-item>
              </el-form>
            </el-tab-pane>

            <!-- 修改密码 -->
            <el-tab-pane
              :label="t('basic.profile.changePassword')"
              name="pwd"
            >
              <el-form
                ref="pwdFormRef"
                :model="pwdForm"
                :rules="pwdRules"
                label-width="120px"
              >
                <el-form-item
                  :label="t('basic.profile.oldPassword')"
                  prop="oldPassword"
                >
                  <el-input
                    v-model="pwdForm.oldPassword"
                    type="password"
                    show-password
                  />
                </el-form-item>
                <el-form-item
                  :label="t('basic.profile.newPassword')"
                  prop="newPassword"
                >
                  <el-input
                    v-model="pwdForm.newPassword"
                    type="password"
                    show-password
                  />
                </el-form-item>
                <el-form-item
                  :label="t('basic.profile.confirmPassword')"
                  prop="confirmPassword"
                >
                  <el-input
                    v-model="pwdForm.confirmPassword"
                    type="password"
                    show-password
                  />
                </el-form-item>
                <el-form-item>
                  <el-button
                    type="primary"
                    :loading="pwdSaving"
                    @click="handleChangePwd"
                  >
                    {{ t('common.button.confirm') }}
                  </el-button>
                </el-form-item>
              </el-form>
            </el-tab-pane>
          </el-tabs>
        </el-col>
      </el-row>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { useUserStore } from '@/stores/user'
import { getProfile, updateProfile, changePassword } from '@/api/basic/profile'
import ImageUpload from '@/components/common/ImageUpload.vue'
import type { ProfileInfo, ChangePwdParams } from '@/types/basic'

const { t } = useLocale()
const router = useRouter()
const userStore = useUserStore()

const activeTab = ref<'basic' | 'pwd'>('basic')
const saving = ref(false)
const pwdSaving = ref(false)

const basicFormRef = ref<FormInstance | null>(null)
const pwdFormRef = ref<FormInstance | null>(null)

const basicForm = reactive({
  userName: '',
  realName: '',
  phone: '',
  email: '',
  avatar: '',
})

const pwdForm = reactive<ChangePwdParams & { confirmPassword: string }>({
  oldPassword: '',
  newPassword: '',
  confirmPassword: '',
})

const basicRules: FormRules = {
  realName: [{ required: true, message: () => t('basic.profile.realNameRequired'), trigger: 'blur' }],
}

const pwdRules: FormRules = {
  oldPassword: [{ required: true, message: () => t('basic.profile.oldPasswordRequired'), trigger: 'blur' }],
  newPassword: [
    { required: true, message: () => t('basic.profile.newPasswordRequired'), trigger: 'blur' },
    { min: 6, message: () => t('basic.profile.newPasswordLength'), trigger: 'blur' },
  ],
  confirmPassword: [
    { required: true, message: () => t('basic.profile.confirmPasswordRequired'), trigger: 'blur' },
    {
      validator: (_rule, value, callback) => {
        if (value !== pwdForm.newPassword) {
          callback(new Error(t('basic.profile.confirmPasswordMismatch')))
        } else {
          callback()
        }
      },
      trigger: 'blur',
    },
  ],
}

const loadProfile = async (): Promise<void> => {
  try {
    const data: ProfileInfo = await getProfile()
    basicForm.userName = data.userName
    basicForm.realName = data.realName
    basicForm.phone = data.phone ?? ''
    basicForm.email = data.email ?? ''
    basicForm.avatar = data.avatar ?? ''
  } catch {
    // 错误已由拦截器处理
  }
}

const handleSaveBasic = async (): Promise<void> => {
  if (!basicFormRef.value) return
  try {
    await basicFormRef.value.validate()
  } catch {
    return
  }
  saving.value = true
  try {
    await updateProfile({
      realName: basicForm.realName,
      phone: basicForm.phone,
      email: basicForm.email,
      avatar: basicForm.avatar,
    })
    userStore.realName = basicForm.realName
    ElMessage.success(t('basic.profile.message.updateSuccess'))
  } catch {
    // 错误已由拦截器处理
  } finally {
    saving.value = false
  }
}

const handleChangePwd = async (): Promise<void> => {
  if (!pwdFormRef.value) return
  try {
    await pwdFormRef.value.validate()
  } catch {
    return
  }
  pwdSaving.value = true
  try {
    await changePassword({
      oldPassword: pwdForm.oldPassword,
      newPassword: pwdForm.newPassword,
    })
    ElMessage.success(t('basic.profile.message.changePwdSuccess'))
    userStore.logout()
    router.push('/login')
  } catch {
    // 错误已由拦截器处理
  } finally {
    pwdSaving.value = false
  }
}

onMounted(() => {
  loadProfile()
})
</script>

<style scoped lang="scss">
.profile-page {
  padding: $spacing-md;
  max-width: 800px;

  &__avatar {
    text-align: center;

    .avatar-name {
      margin-top: $spacing-md;
      font-size: $font-size-lg;
      font-weight: 500;
    }
  }
}
</style>