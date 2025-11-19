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

<style scoped>
:root {
  --radius: 16px;
  --card: rgba(255 255 255 / 0.9);
  --line: #d0e6d9;
  --text: #1f2937;
  --text-weak: #6b7280;
  --brand: #42b883;
  --brand-weak: rgba(66, 184, 131, 0.12);
}

.auth-page {
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 95vh;
  padding: 40px 20px;
  /* 添加渐变背景色 */
  background: linear-gradient(135deg, #74ebd5 0%, #ACB6E5 100%);
}

.card {
  width: 100%;
  max-width: 420px;
  background: var(--card);
  border-radius: var(--radius);
  padding: 36px 28px;
  box-shadow: 0 12px 36px rgba(0,0,0,0.15);
  border: 1px solid var(--line);
  backdrop-filter: saturate(180%) blur(8px);
}

.title {
  font-size: 26px;
  color: #fff;
  margin-bottom: 12px;
  text-shadow: 0 1px 6px rgba(0,0,0,0.25);
  font-weight: 900;
}

.desc {
  font-size: 15px;
  color: #e0ecef;
  margin-bottom: 14px;
  font-weight: 500;
  text-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

.extra-desc {
  display: grid;
  grid-template-columns: 130px 1fr;
  gap: 16px 28px;
  margin-bottom: 32px;
  align-items: start;
}

.desc-labels {
  display: flex;
  flex-direction: column;
  gap: 18px;
  font-weight: 700;
  font-size: 18px;
  color: #057a55;
}

.desc-texts {
  display: flex;
  flex-direction: column;
  gap: 18px;
  font-size: 15px;
  color: #1c3d2e;
}

.desc-item {
  margin: 0;
}

.form {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 24px;
}
.form-item {
  display: flex;
  flex-direction: column;
  gap: 18px;
}
.label {
  font-weight: 700;
  font-size: 17px;
  color: #2e5831;
}
.input {
  height: 52px;
  padding: 0 20px;
  border-radius: 14px;
  border: 1px solid #a3c4a5;
  font-size: 18px;
  color: #254021;
  background: #e9f1e7;
  transition: border-color 0.3s ease;
}
.input::placeholder {
  color: #97b49f;
}
.input:focus {
  border-color: #47b37b;
  outline: none;
  box-shadow: 0 0 8px 4px rgba(71, 179, 123, 0.45);
}
.error {
  color: #bb2a2a;
  font-size: 16px;
  margin-top: -10px;
  margin-bottom: 14px;
  font-weight: 700;
}
.actions {
  display: flex;
  flex-direction: column;
  gap: 20px;
  margin-top: 24px;
}
.btn {
  height: 52px;
  border-radius: 14px;
  font-weight: 700;
  font-size: 18px;
  cursor: pointer;
  border: none;
  transition: background-color 0.3s ease, transform 0.15s ease;
}
.btn.primary {
  background-color: #2e8a57;
  color: white;
}
.btn.primary:hover:not([disabled]) {
  background-color: #1c5f34;
  transform: translateY(-2px);
}
.btn.primary:disabled {
  background-color: #b1cfbc;
  cursor: not-allowed;
}
.link {
  text-align: center;
  color: #4a6e54;
  font-size: 16px;
  text-decoration: underline;
  cursor: pointer;
  line-height: 1.6;
  user-select: none;
}
.link:hover {
  color: #2e8a57;
}
</style>