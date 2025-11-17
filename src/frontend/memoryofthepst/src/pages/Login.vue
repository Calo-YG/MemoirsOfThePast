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
    const resp = await post<{ token?: string; message?: string }, { email: string; password: string }>(
      '/auth/login',
      { email: email.value.trim(), password: password.value }
    )
    const token = (resp as any)?.token
    if (token) {
      localStorage.setItem('ACCESS_TOKEN', token)
    }
    // 登录成功后跳转开始页或首页
    router.push('/start')
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

<style scoped>
:root {
  --radius: 12px;
  --card: #ffffff;
  --line: #e9edf2;
  --text: #1f2937;
  --text-weak: #6b7280;
  --brand: #42b883;
  --brand-weak: rgba(66, 184, 131, 0.12);
}

.auth-page {
  width: 100%;
  display: grid;
  place-items: center;
}

.card {
  width: 100%;
  max-width: 520px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  padding: clamp(16px, 4vw, 24px);
  box-shadow: 0 4px 18px rgba(0,0,0,.04);
}

.title {
  margin: 0;
  font-weight: 800;
  font-size: 18px;
  color: var(--text);
}
.desc {
  margin: 6px 0 12px;
  font-size: 13px;
  color: var(--text-weak);
}

.form {
  display: grid;
  gap: 12px;
}
.form-item {
  display: grid;
  gap: 6px;
}
.label {
  font-size: 13px;
  color: var(--text);
}
.input {
  width: 100%;
  height: 38px;
  padding: 0 12px;
  border-radius: 10px;
  border: 1px solid var(--line);
  font-size: 14px;
  color: var(--text);
  background: #fff;
}
.input::placeholder { color: #98a2ad; }

.error {
  margin: 2px 0 6px;
  font-size: 12px;
  color: #d93025;
}

.actions {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-top: 6px;
}
.btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  height: 38px;
  padding: 0 14px;
  border-radius: 10px;
  border: 1px solid var(--line);
  background: #fff;
  color: var(--text);
  cursor: pointer;
}
.btn.primary {
  background: var(--brand);
  color: #fff;
  border-color: transparent;
}
.btn.primary[disabled] {
  opacity: .7;
  cursor: not-allowed;
}
.link {
  color: var(--text-weak);
  text-decoration: none;
  font-size: 13px;
}
.link:hover {
  color: var(--brand);
}
</style>