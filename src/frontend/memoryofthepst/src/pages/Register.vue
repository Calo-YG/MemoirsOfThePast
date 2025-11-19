<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { post } from '../services/http'

const router = useRouter()

const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const error = ref<string>('')

function validate(): string | null {
  if (!email.value.trim()) return '请输入邮箱'
  if (!/^\S+@\S+\.\S+$/.test(email.value)) return '邮箱格式不正确'
  if (!password.value.trim()) return '请输入密码'
  if (password.value.length < 6) return '密码长度至少 6 位'
  if (confirmPassword.value !== password.value) return '两次输入的密码不一致'
  return null
}

async function onSubmit() {
  error.value = ''
  const msg = validate()
  if (msg) {
    error.value = msg
    return
  }
  loading.value = true
  try {
    const resp = await post<{ message?: string }, { email: string; password: string }>(
      '/auth/register',
      { email: email.value.trim(), password: password.value }
    )
    // 注册成功后跳转到登录页
    router.push('/login')
  } catch (e) {
    error.value = (e as any)?.message ?? '注册失败，请稍后重试'
  } finally {
    loading.value = false
  }
}

function onKeydown(e: KeyboardEvent) {
  if (e.key === 'Enter') {
    e.preventDefault()
    onSubmit()
  }
}
</script>

<template>
  <section class="auth-page">
    <div class="card layout-horizontal">
      <div class="extra-desc horizontal-layout">
        <div class="desc-labels">
          <div class="desc-item">轻松记录</div>
          <div class="desc-item">智能助手</div>
          <div class="desc-item">简洁设计</div>
        </div>
        <div class="desc-texts">
          <div class="desc-item">快速捕捉灵感与回忆，保持知识的持续积累。</div>
          <div class="desc-item">利用 AI 技术帮助理解、总结和联想，提升信息价值。</div>
          <div class="desc-item">响应式界面，专注内容表达，提升用户体验。</div>
        </div>
      </div>

      <form class="form" @submit.prevent="onSubmit">
        <div class="form-item">
          <label class="label">邮箱</label>
          <input
            class="input"
            type="email"
            placeholder="your@email.com"
            v-model="email"
            @keydown="onKeydown"
          />
        </div>

        <div class="form-item">
          <label class="label">密码</label>
          <input
            class="input"
            type="password"
            placeholder="至少 6 位密码"
            v-model="password"
            @keydown="onKeydown"
          />
          <p class="hint">建议使用大小写字母、数字与符号的组合，提高密码强度。</p>
        </div>

        <div class="form-item">
          <label class="label">确认密码</label>
          <input
            class="input"
            type="password"
            placeholder="再次输入密码"
            v-model="confirmPassword"
            @keydown="onKeydown"
          />
        </div>

        <p v-if="error" class="error">{{ error }}</p>

        <div class="actions">
          <button class="btn primary" type="submit" :disabled="loading">
            <svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M10 17l5-5-5-5v10zM4 5h4v14H4V5zm14 0h2v14h-2v-2z"/></svg>
            <span>{{ loading ? '注册中…' : '注册' }}</span>
          </button>
          <RouterLink to="/login" class="link">已有账号？去登录</RouterLink>
        </div>
      </form>
    </div>
  </section>
</template>

<style src="./styles/register.css" scoped>
</style>