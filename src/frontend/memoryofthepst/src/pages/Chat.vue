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
          <h2>往事回首</h2>
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

<style src="./styles/chat.css" scoped></style>