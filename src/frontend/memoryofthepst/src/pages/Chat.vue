<script setup lang="ts">
import { ref, nextTick, onMounted } from 'vue'
import { post } from '../services/http'

type Role = 'user' | 'assistant'
type Message = {
  id: string
  role: Role
  content: string
}

const messages = ref<Message[]>([
  {
    id: String(Date.now()),
    role: 'assistant',
    content: '你好，我是你的 AI 助手。有什么需要我帮助的吗？'
  }
])

const input = ref('')
const sending = ref(false)
const listRef = ref<HTMLDivElement | null>(null)
const inputRef = ref<HTMLTextAreaElement | null>(null)

function uid() {
  return Math.random().toString(36).slice(2)
}

async function scrollToBottom() {
  await nextTick()
  const el = listRef.value
  if (el) el.scrollTop = el.scrollHeight
}

function onKeydown(e: KeyboardEvent) {
  // Enter 发送，Shift+Enter 换行
  if (e.key === 'Enter' && !e.shiftKey) {
    e.preventDefault()
    send()
  }
}

async function send() {
  const text = input.value.trim()
  if (!text || sending.value) return

  // 1) 推入用户消息
  const userMsg: Message = { id: uid(), role: 'user', content: text }
  messages.value.push(userMsg)
  input.value = ''
  await scrollToBottom()

  // 2) 请求后端生成回复（示例接口：POST /chat）
  sending.value = true
  try {
    // 后端可根据实际约定返回 { content: string } 或其他结构
    const resp = await post<{ content: string }, { messages: Message[]; prompt: string }>(
      '/chat',
      {
        prompt: userMsg.content,
        messages: messages.value // 携带上下文，便于后端生成更合适的回复
      }
    )

    const answer = typeof resp === 'string' ? resp : resp?.content ?? '（未获取到回复内容）'
    messages.value.push({ id: uid(), role: 'assistant', content: answer })
  } catch (e) {
    const msg = (e as any)?.message ?? '请求失败，请稍后重试'
    messages.value.push({
      id: uid(),
      role: 'assistant',
      content: `抱歉，发生错误：${msg}`
    })
  } finally {
    sending.value = false
    scrollToBottom()
    inputRef.value?.focus()
  }
}

onMounted(() => {
  inputRef.value?.focus()
})
</script>

<template>
  <section class="chat-page">
    <div class="chat-card">
      <div class="chat-header">
        <div class="title">
          <span class="dot"></span>
          <h2>AI 聊天</h2>
        </div>
        <p class="desc">与 AI 助手进行对话，支持回车发送，Shift+Enter 换行。</p>
      </div>

      <div class="chat-list" ref="listRef">
        <div
          v-for="m in messages"
          :key="m.id"
          class="msg"
          :class="m.role === 'user' ? 'right' : 'left'"
        >
          <div class="avatar" :class="m.role">
            <span v-if="m.role==='assistant'">AI</span>
            <span v-else>我</span>
          </div>
          <div class="bubble" :class="m.role">
            <pre class="content">{{ m.content }}</pre>
          </div>
        </div>

        <div class="sending" v-if="sending">
          <div class="loader">
            <span class="ball"></span>
            <span class="ball"></span>
            <span class="ball"></span>
          </div>
          <span class="sending-text">正在思考…</span>
        </div>
      </div>

      <div class="chat-input">
        <textarea
          ref="inputRef"
          v-model="input"
          class="input"
          rows="3"
          placeholder="输入你的问题，回车发送，Shift+Enter 换行…"
          @keydown="onKeydown"
        ></textarea>

        <div class="tools">
          <span class="tip">回车发送 · Shift+Enter 换行</span>
          <button
            class="send-btn"
            :disabled="sending || !input.trim()"
            @click="send"
            title="发送"
          >
            <svg viewBox="0 0 24 24" width="18" height="18" aria-hidden="true">
              <path fill="currentColor" d="M2.01 21L23 12 2.01 3 2 10l15 2-15 2z"/>
            </svg>
            <span>发送</span>
          </button>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
:root {
  --radius: 12px;
  --bg: #f6f7f9;
  --card: #ffffff;
  --line: #e9edf2;
  --text: #1f2937;
  --text-weak: #6b7280;
  --brand: #42b883;
  --brand-weak: rgba(66, 184, 131, 0.12);
  --user: #2563eb;
  --assistant: #334155;
}

* { box-sizing: border-box; }

.chat-page {
  width: 100%;
  display: grid;
  place-items: center;
}

.chat-card {
  width: 100%;
  max-width: 900px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  overflow: hidden;
}

.chat-header {
  padding: 14px 16px 8px;
  border-bottom: 1px solid var(--line);
  background: linear-gradient(180deg, #fff, #fbfbfb);
}
.title {
  display: flex;
  align-items: center;
  gap: 10px;
}
.title .dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: var(--brand);
  box-shadow: 0 0 0 6px var(--brand-weak);
}
.title h2 {
  font-size: 16px;
  font-weight: 700;
  color: var(--text);
  margin: 0;
}
.desc {
  font-size: 12px;
  color: var(--text-weak);
  margin: 6px 0 0;
}

.chat-list {
  max-height: 56vh;
  min-height: 280px;
  overflow-y: auto;
  padding: 16px;
  display: grid;
  gap: 12px;
  background: #fafbfc;
}

.msg {
  display: grid;
  grid-template-columns: 40px 1fr;
  gap: 10px;
  align-items: flex-start;
}
.msg.right {
  grid-template-columns: 1fr 40px;
}
.msg.right .avatar { grid-column: 2 / 3; }
.msg.right .bubble { grid-column: 1 / 2; justify-self: end; }

.avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: var(--brand-weak);
  color: var(--assistant);
  display: grid;
  place-items: center;
  font-size: 12px;
  font-weight: 700;
}
.avatar.user {
  background: rgba(37, 99, 235, .12);
  color: var(--user);
}
.avatar.assistant {
  background: rgba(51, 65, 85, .12);
  color: var(--assistant);
}

.bubble {
  max-width: 680px;
  border: 1px solid var(--line);
  background: #fff;
  border-radius: 14px;
  padding: 10px 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,.02);
}
.bubble.assistant {
  border-color: #e4e7eb;
}
.bubble.user {
  border-color: #dbeafe;
  background: #eef6ff;
}
.content {
  white-space: pre-wrap;
  word-break: break-word;
  font-size: 14px;
  color: var(--text);
  margin: 0;
}

.sending {
  display: flex;
  align-items: center;
  gap: 8px;
  color: var(--text-weak);
  font-size: 12px;
}
.loader {
  display: inline-flex;
  gap: 4px;
}
.loader .ball {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: #cbd5e1;
  animation: pulse 1s infinite ease-in-out;
}
.loader .ball:nth-child(2) { animation-delay: .2s; }
.loader .ball:nth-child(3) { animation-delay: .4s; }
@keyframes pulse {
  0%, 100% { transform: scale(1); opacity: .6; }
  50% { transform: scale(1.3); opacity: 1; }
}

.chat-input {
  border-top: 1px solid var(--line);
  background: #fff;
  padding: 12px;
  display: grid;
  gap: 10px;
}
.input {
  width: 100%;
  border: 1px solid var(--line);
  border-radius: 12px;
  padding: 10px 12px;
  resize: vertical;
  min-height: 72px;
  font-size: 14px;
  color: var(--text);
}
.input::placeholder { color: #9aa1ab; }

.tools {
  display: flex;
  align-items: center;
  gap: 10px;
}
.tip {
  font-size: 12px;
  color: var(--text-weak);
}
.send-btn {
  margin-left: auto;
  height: 36px;
  padding: 0 12px;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  border: 1px solid var(--line);
  border-radius: 10px;
  background: #fff;
  color: var(--text);
  cursor: pointer;
  transition: background .2s ease, border-color .2s ease, color .2s ease;
}
.send-btn:hover {
  background: var(--brand-weak);
  border-color: #cfe8dc;
  color: var(--brand);
}
.send-btn:disabled {
  cursor: not-allowed;
  opacity: .6;
}

/* 响应式适配 */
@media (max-width: 768px) {
  .chat-card { border-radius: 0; }
  .chat-list { max-height: 48vh; }
  .bubble { max-width: 86vw; }
}
</style>