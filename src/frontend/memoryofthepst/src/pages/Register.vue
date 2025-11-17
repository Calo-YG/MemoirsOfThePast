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
    <div class="card">
      <h2 class="title">注册</h2>
      <p class="desc">创建你的账号以开始使用系统。</p>

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

.hint {
  margin: 4px 0 0;
  font-size: 12px;
  color: var(--text-weak);
}

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