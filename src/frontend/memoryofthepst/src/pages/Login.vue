<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { post } from '../services/http'

const router = useRouter()

const email = ref('')
const password = ref('')
const loading = ref(false)
const error = ref<string>('')

function validate(): string | null {
  if (!email.value.trim()) return '请输入邮箱'
  if (!/^\S+@\S+\.\S+$/.test(email.value)) return '邮箱格式不正确'
  if (!password.value.trim()) return '请输入密码'
  if (password.value.length < 6) return '密码长度至少 6 位'
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
    const resp = await post<{ token?: string; message?: string }, { account: string; password: string }>(
      '/auth/login',
      { account: email.value.trim(), password: password.value }
    )
    const token = (resp as any)?.token
    if (token) {
      localStorage.setItem('ACCESS_TOKEN', token)
    }
    // 登录成功后跳转开始页或首页
    router.push('/app/home')
  } catch (e) {
    error.value = (e as any)?.message ?? '登录失败，请稍后重试'
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
    <div class="card">
      <h2 class="title">登录</h2>
      <p class="desc">输入你的账号信息以登录系统。</p>
      <div class="extra-desc grid-layout">
        <div class="desc-labels">
          <div class="desc-item">高效管理</div>
          <div class="desc-item">智能辅助</div>
          <div class="desc-item">简洁界面</div>
        </div>
        <div class="desc-texts">
          <div class="desc-item">快速记录灵感与回忆，保持知识持续进化。</div>
          <div class="desc-item">借助 AI 助手理解、总结与联想，提升信息价值。</div>
          <div class="desc-item">响应式设计，专注内容表达，提升使用体验。</div>
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
        </div>

        <p v-if="error" class="error">{{ error }}</p>

        <div class="actions">
          <button class="btn primary" type="submit" :disabled="loading">
            <svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M10 17l5-5-5-5v10zM4 5h4v14H4V5zm14 0h2v14h-2V5z"/></svg>
            <span>{{ loading ? '登录中…' : '登录' }}</span>
          </button>
          <RouterLink to="/register" class="link">没有账号？去注册</RouterLink>
        </div>
      </form>
    </div>
  </section>
</template>

<style src="./styles/login.css" scoped>
</style>
