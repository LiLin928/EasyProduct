<template>
  <div class="login-page">
    <el-card class="login-page__card">
      <h2 class="login-page__title">
        {{ t('common.login.title') }}
      </h2>
      <el-form
        ref="formRef"
        :model="model"
        :rules="rules"
        label-width="0"
        size="large"
      >
        <el-form-item prop="userName">
          <el-input
            v-model="model.userName"
            :placeholder="t('common.login.userName')"
          />
        </el-form-item>
        <el-form-item prop="password">
          <el-input
            v-model="model.password"
            type="password"
            show-password
            :placeholder="t('common.login.password')"
            @keyup.enter="handleSubmit"
          />
        </el-form-item>
        <el-button
          type="primary"
          :loading="loading"
          class="login-page__submit"
          @click="handleSubmit"
        >
          {{ t('common.login.submit') }}
        </el-button>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { FormInstance, FormRules } from 'element-plus'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/stores/user'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
const userStore = useUserStore()

const formRef = ref<FormInstance>()
const loading = ref(false)
const model = reactive({ userName: 'admin', password: 'admin123' })

const rules: FormRules = {
  userName: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  password: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  loading.value = true
  try {
    await userStore.login(model.userName, model.password)
    const redirect = (route.query.redirect as string) || '/desktop'
    router.push(redirect)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped lang="scss">
.login-page {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;

  &__card {
    width: 380px;
  }

  &__title {
    text-align: center;
    margin-bottom: $spacing-lg;
  }

  &__submit {
    width: 100%;
  }
}
</style>
