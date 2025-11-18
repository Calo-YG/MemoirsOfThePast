<script setup lang="ts">
import { ref, computed } from 'vue'
import Chat from './Chat.vue'

type Id = string

interface MemoryEntity {
  id: Id
  userId: string
  name: string
  description: string
  avatar?: string
  background?: string
  prompt?: string | null
  createDate: string
}

interface FragmentEntity {
  id: Id
  memoryId: Id
  description: string
  occurDate: string
  location?: string
  scene?: string
  createDate: string
}

const showAddMemory = ref(false)
const showDrawer = ref(false)
const selectedMemoryId = ref<Id | null>(null)

const memories = ref<MemoryEntity[]>([
  {
    id: 'm-' + Date.now(),
    userId: 'u-demo',
    name: '示例回忆',
    description: '一段关于过去的记忆片段',
    avatar: '',
    background: '',
    prompt: null,
    createDate: new Date().toISOString()
  }
])

const fragments = ref<Record<Id, FragmentEntity[]>>({})

const selectedMemory = computed(() => memories.value.find(m => m.id === selectedMemoryId.value) || null)
const currentFragments = computed(() => selectedMemoryId.value ? (fragments.value[selectedMemoryId.value] ?? []) : [])

const memForm = ref({ name: '', description: '', avatar: '', background: '' })
const fragForm = ref({ description: '', occurDate: '', location: '', scene: '' })

function uid() { return Math.random().toString(36).slice(2) }

function addMemory() {
  const name = memForm.value.name.trim()
  const description = memForm.value.description.trim()
  if (!name || !description) return
  const id = 'm-' + uid()
  const entity: MemoryEntity = {
    id,
    userId: 'u-demo',
    name,
    description,
    avatar: memForm.value.avatar || '',
    background: memForm.value.background || '',
    prompt: null,
    createDate: new Date().toISOString()
  }
  memories.value.unshift(entity)
  showAddMemory.value = false
  memForm.value = { name: '', description: '', avatar: '', background: '' }
}

function openDrawer(memoryId: Id) {
  selectedMemoryId.value = memoryId
  showDrawer.value = true
}

function closeDrawer() { showDrawer.value = false }

function addFragment() {
  if (!selectedMemoryId.value) return
  const desc = fragForm.value.description.trim()
  if (!desc) return
  const entity: FragmentEntity = {
    id: 'f-' + uid(),
    memoryId: selectedMemoryId.value,
    description: desc,
    occurDate: fragForm.value.occurDate || new Date().toISOString(),
    location: fragForm.value.location || '',
    scene: fragForm.value.scene || '',
    createDate: new Date().toISOString()
  }
  const list = fragments.value[selectedMemoryId.value] ?? []
  fragments.value[selectedMemoryId.value] = [entity, ...list]
  fragForm.value = { description: '', occurDate: '', location: '', scene: '' }
}

/* 分栏拖拽调整宽度（左右可拖动） */
const containerRef = ref<HTMLElement | null>(null)
const leftWidth = ref<number>(58)                // 左侧初始占比（百分比）
const isResizing = ref<boolean>(false)
const rightWidth = computed(() => 100 - leftWidth.value)
const MIN_LEFT = 24                              // 左侧最小百分比
const MAX_LEFT = 76                              // 左侧最大百分比

function setLeftByClientX(clientX: number) {
  const rect = containerRef.value?.getBoundingClientRect()
  if (!rect) return
  const x = clientX - rect.left
  let pct = (x / rect.width) * 100
  pct = Math.max(MIN_LEFT, Math.min(MAX_LEFT, pct))
  leftWidth.value = Math.round(pct)
}

function onSplitMouseDown(e: MouseEvent) {
  isResizing.value = true
  setLeftByClientX(e.clientX)
  window.addEventListener('mousemove', onSplitMouseMove)
  window.addEventListener('mouseup', onSplitMouseUp, { once: true })
}
function onSplitMouseMove(e: MouseEvent) {
  if (!isResizing.value) return
  setLeftByClientX(e.clientX)
}
function onSplitMouseUp() {
  isResizing.value = false
  window.removeEventListener('mousemove', onSplitMouseMove)
}

function onSplitTouchStart(e: TouchEvent) {
  isResizing.value = true
  const t = e.touches && e.touches.length ? e.touches[0] : null
  if (t) setLeftByClientX(t.clientX)
  window.addEventListener('touchmove', onSplitTouchMove, { passive: false })
  window.addEventListener('touchend', onSplitTouchEnd, { once: true })
}
function onSplitTouchMove(e: TouchEvent) {
  if (!isResizing.value) return
  const t = e.touches && e.touches.length ? e.touches[0] : null
  if (t) setLeftByClientX(t.clientX)
}
function onSplitTouchEnd() {
  isResizing.value = false
  window.removeEventListener('touchmove', onSplitTouchMove)
}

function resetSplit() {
  leftWidth.value = 58
}
</script>

<template>
  <section class="memories-page" ref="containerRef" :style="{ gridTemplateColumns: `${leftWidth}% 6px ${100 - leftWidth}%` }">
    <div class="left">
      <div class="card header">
        <div class="title">
          <span class="dot"></span>
          <h2>往事回忆</h2>
        </div>
        <div class="tools">
          <button class="ghost-btn add-btn" @click="showAddMemory = !showAddMemory" :title="showAddMemory ? '收起' : '添加回忆'">
            <svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M19 13H13v6h-2v-6H5v-2h6V5h2v6h6v2z"/></svg>
            <span>添加回忆</span>
          </button>
        </div>
      </div>
      <div class="card" v-show="showAddMemory">
        <div class="form">
          <label class="row">
            <span class="label">人物名称</span>
            <input class="input" v-model="memForm.name" placeholder="如：张三"/>
          </label>
          <label class="row">
            <span class="label">描述</span>
            <textarea class="input" v-model="memForm.description" rows="3" placeholder="这段回忆的简要描述"/>
          </label>
          <label class="row">
            <span class="label">头像</span>
            <input class="input" v-model="memForm.avatar" placeholder="头像 URL（可选）"/>
          </label>
          <label class="row">
            <span class="label">背景</span>
            <input class="input" v-model="memForm.background" placeholder="背景描述或 URL（可选）"/>
          </label>
          <div class="actions">
            <button class="ghost-btn primary" @click="addMemory">保存</button>
            <button class="ghost-btn" @click="showAddMemory=false">取消</button>
          </div>
        </div>
      </div>
      <div class="card list">
        <ul class="memory-list">
          <li v-for="m in memories" :key="m.id" class="memory-item">
            <div class="avatar" :style="m.avatar ? { backgroundImage: 'url('+m.avatar+')' } : {}">
              <span v-if="!m.avatar">{{ m.name.slice(0,2).toUpperCase() }}</span>
            </div>
            <div class="info">
              <div class="name">{{ m.name }}</div>
              <div class="desc">{{ m.description }}</div>
              <div class="meta">创建于 {{ new Date(m.createDate).toLocaleString() }}</div>
            </div>
            <div class="ops">
              <button class="ghost-btn small" @click="openDrawer(m.id)" title="构建回忆碎片">
                <svg viewBox="0 0 24 24" width="16" height="16" aria-hidden="true"><path fill="currentColor" d="M12 3l9 5-9 5-9-5 9-5zm0 8l9 5-9 5-9-5 9-5z"/></svg>
                <span class="text">构建回忆碎片</span>
              </button>
            </div>
          </li>
        </ul>
      </div>
    </div>
    <div
      class="split-resizer"
      @mousedown="onSplitMouseDown"
      @touchstart.prevent="onSplitTouchStart"
      @dblclick="resetSplit"
      :class="{ active: isResizing }"
      aria-label="拖动调整宽度"
      role="separator"
      aria-orientation="vertical"
    ></div>
    <div class="right">
      <Chat />
    </div>
    <transition name="drawer">
      <div class="drawer" v-show="showDrawer" role="dialog" aria-label="构建回忆碎片">
        <div class="drawer-header">
          <h3>构建回忆碎片</h3>
          <button class="ghost-btn" @click="closeDrawer" title="关闭">
            <svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M19 6.41 17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/></svg>
          </button>
        </div>
        <div class="drawer-body" v-if="selectedMemory">
          <div class="mem-brief">
            <div class="avatar small"><span>{{ selectedMemory.name.slice(0,2).toUpperCase() }}</span></div>
            <div class="name">{{ selectedMemory.name }}</div>
            <div class="desc">{{ selectedMemory.description }}</div>
          </div>
          <div class="form">
            <label class="row">
              <span class="label">碎片描述</span>
              <textarea class="input" v-model="fragForm.description" rows="3" placeholder="这一幕发生了什么？"/>
            </label>
            <label class="row">
              <span class="label">发生时间</span>
              <input class="input" type="datetime-local" v-model="fragForm.occurDate"/>
            </label>
            <label class="row">
              <span class="label">地点</span>
              <input class="input" v-model="fragForm.location" placeholder="如：上海·外滩"/>
            </label>
            <label class="row">
              <span class="label">场景</span>
              <input class="input" v-model="fragForm.scene" placeholder="如：夜晚，灯火阑珊"/>
            </label>
            <div class="actions">
              <button class="ghost-btn primary" @click="addFragment">添加碎片</button>
            </div>
          </div>
          <div class="frag-list">
            <h4>碎片列表（不可删除）</h4>
            <ul>
              <li v-for="f in currentFragments" :key="f.id" class="frag-item">
                <div class="line">
                  <span class="time">{{ new Date(f.occurDate).toLocaleString() }}</span>
                  <span class="loc" v-if="f.location">{{ f.location }}</span>
                </div>
                <div class="desc">{{ f.description }}</div>
                <div class="scene" v-if="f.scene">{{ f.scene }}</div>
              </li>
            </ul>
          </div>
        </div>
        <div class="drawer-body" v-else>
          <p class="hint">请选择左侧回忆条目进行碎片构建。</p>
        </div>
      </div>
    </transition>
  </section>
</template>

<style scoped>
:root {
  --radius: 12px;
  --line: var(--line);
  --card: var(--card);
  --text: var(--text);
  --text-weak: var(--text-weak);
  --brand: var(--brand);
  --brand-weak: var(--brand-weak);
}
* { box-sizing: border-box; }
.memories-page {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}
.left, .right { min-width: 0; }
.card {
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  padding: 12px;
}
.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
}
.title { display: flex; align-items: center; gap: 10px; }
.title .dot {
  width: 10px; height: 10px; border-radius: 50%;
  background: var(--brand); box-shadow: 0 0 0 6px var(--brand-weak);
}
.title h2 { margin: 0; font-size: 16px; font-weight: 700; color: var(--text); }
.tools { display: flex; align-items: center; gap: 8px; }
.ghost-btn {
  height: 32px; padding: 0 10px; display: inline-flex; align-items: center; gap: 6px;
  border: 1px solid var(--line); border-radius: 10px; background: var(--card); color: var(--text);
}
.ghost-btn:hover { background: var(--brand-weak); border-color: #cfe8dc; color: var(--brand); }
.ghost-btn.primary { color: #fff; background: var(--brand); border-color: #34c48a; }
.ghost-btn.primary:hover { background: #34c48a; }
.ghost-btn.small { height: 28px; padding: 0 8px; }
.add-btn span { font-size: 14px; }

.form { display: grid; gap: 10px; }
.row { display: grid; gap: 6px; }
.label { font-size: 12px; color: var(--text-weak); }
.input {
  width: 100%; border: 1px solid var(--line); border-radius: 10px; padding: 8px 10px; font-size: 14px; color: var(--text);
  background: var(--card);
}
.actions { display: flex; gap: 8px; }

.list { padding: 0; }
.memory-list { list-style: none; margin: 0; padding: 0; display: grid; gap: 8px; }
.memory-item {
  display: grid; grid-template-columns: 40px 1fr auto; gap: 10px; align-items: center;
  padding: 10px 12px; border: 1px solid var(--line); border-radius: 10px; background: #fff;
}
.memory-item:hover { background: #f7faf9; border-color: #e6f3ed; }
.avatar {
  width: 36px; height: 36px; border-radius: 50%; background: var(--brand-weak);
  display: grid; place-items: center; color: var(--brand); font-weight: 700; font-size: 12px;
  background-size: cover; background-position: center;
}
.info .name { font-weight: 600; color: var(--text); }
.info .desc { font-size: 13px; color: var(--text-weak); margin-top: 2px; }
.info .meta { font-size: 12px; color: var(--text-weak); margin-top: 4px; }
.ops { display: flex; align-items: center; gap: 6px; }
.ops .text { font-size: 12px; }

.right { overflow: hidden; }

/* 抽屉 */
.drawer {
  position: fixed;
  top: var(--header-h, 64px);
  right: 0;
  width: clamp(640px, 52vw, 980px);
  height: calc(100vh - var(--header-h, 64px));
  background: var(--card);
  border-left: 1px solid var(--line);
  box-shadow: -12px 0 28px rgba(0,0,0,.08);
  z-index: 60;
  display: grid;
  grid-template-rows: auto 1fr;
}
@supports (height: 100dvh) {
  .drawer {
    height: calc(100dvh - var(--header-h, 64px));
  }
}
@media (max-width: 768px) {
  .drawer {
    width: 100vw;
  }
}
.drawer-header {
  display: flex; align-items: center; justify-content: space-between; padding: 10px 12px; border-bottom: 1px solid var(--line);
}
.drawer-body { padding: 12px; overflow: auto; display: grid; gap: 12px; }
.mem-brief { display: grid; grid-template-columns: 36px 1fr; gap: 10px; align-items: center; }
.avatar.small { width: 32px; height: 32px; }
.frag-list h4 { margin: 0 0 6px; font-size: 14px; color: var(--text); }
.frag-item { border: 1px solid var(--line); border-radius: 10px; padding: 10px; display: grid; gap: 6px; background: #fff; }
.frag-item .line { display: flex; gap: 8px; font-size: 12px; color: var(--text-weak); }
.frag-item .desc { font-size: 14px; color: var(--text); white-space: pre-wrap; }
.scene { font-size: 12px; color: var(--text-weak); }
.hint { color: var(--text-weak); }

/* 过渡 */
.drawer-enter-active, .drawer-leave-active { transition: transform .22s ease, opacity .18s ease; }
.drawer-enter-from, .drawer-leave-to { transform: translateX(8px); opacity: 0; }

/* 响应式 */
@media (max-width: 1024px) {
  .memories-page { grid-template-columns: 1fr; }
  .right { order: 2; }
}
</style>
<style scoped>
/* 视觉优化覆盖：提升卡片质感、列表条目交互与右侧聊天布局适配 */

/* 布局与容器 */
.memories-page {
  grid-template-columns: 1.1fr 0.9fr;
  gap: 18px;
  min-height: calc(100vh - var(--header-h, 64px) - 160px);
}
.card {
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: 0 6px 24px rgba(0,0,0,.06);
  transition: border-color .18s ease, box-shadow .18s ease, background .18s ease;
}
.header {
  padding: 10px 12px;
  border: 1px solid var(--line);
  border-radius: 12px;
  background: linear-gradient(180deg, var(--card), color-mix(in oklab, var(--card), #000 4%));
}

/* 标题与按钮 */
.title h2 { font-size: 18px; }
.ghost-btn {
  height: 34px;
  padding: 0 12px;
}
.ghost-btn.primary {
  background: var(--brand);
  color: #fff;
  border-color: #34c48a;
}
.ghost-btn.primary:hover {
  background: #34c48a;
}

/* 列表与条目优化 */
.memory-list { gap: 10px; }
.memory-item {
  position: relative;
  grid-template-columns: 40px 1fr;
  padding: 12px;
  border-radius: 12px;
  background: var(--card);
  transition: background .18s ease, border-color .18s ease, transform .18s ease;
}
.memory-item:hover {
  border-color: #cfe8dc;
  background: var(--brand-weak);
  transform: translateY(-1px);
}
.avatar {
  box-shadow: 0 0 0 6px var(--brand-weak) inset;
}
.info .name { font-size: 15px; }
.info .desc { font-size: 13px; color: var(--text-weak); }
.info .meta { font-size: 12px; color: var(--text-weak); }

/* 悬浮操作按钮：在条目右上角浮现 */
.ops {
  position: absolute;
  top: 8px;
  right: 10px;
  display: inline-flex;
  gap: 6px;
  opacity: 0;
  transition: opacity .18s ease;
}
.memory-item:hover .ops { opacity: 1; }
.ops .ghost-btn.small { background: var(--card); }
.ops .ghost-btn.small:hover {
  background: var(--brand-weak);
  border-color: #cfe8dc;
  color: var(--brand);
}

/* 表单输入优化 */
.input {
  background: var(--card);
  border: 1px solid var(--line);
}
.input:focus {
  outline: 2px solid var(--brand);
  outline-offset: 2px;
  border-color: #cfe8dc;
}

/* 右侧聊天承载容器：不下钻修改子组件，避免覆盖发送按钮等样式 */
.right { display: block; }
.right > * { width: 100%; }

/* 抽屉质感提升 */
.drawer {
  border-top-left-radius: 12px;
  border-bottom-left-radius: 12px;
  backdrop-filter: saturate(1.06) blur(8px);
  box-shadow: -12px 0 24px rgba(0,0,0,.08);
}
.drawer-header {
  background: linear-gradient(180deg, var(--card), color-mix(in oklab, var(--card), #000 4%));
}
.frag-item {
  background: var(--card);
  border-radius: 12px;
  box-shadow: 0 4px 16px rgba(0,0,0,.04);
}
.frag-item .desc { font-size: 14px; }

/* 轻动画与无障碍 */
.memory-item,
.ghost-btn,
.drawer {
  will-change: transform, opacity;
}
.ghost-btn:focus-visible,
.theme-option:focus-visible,
.theme-toggle:focus-visible {
  outline: 2px solid var(--brand);
  outline-offset: 2px;
}

/* 响应式 */
@media (max-width: 1024px) {
  .memories-page { grid-template-columns: 1fr; gap: 12px; }
  .right :deep(.chat-list) { max-height: 42vh; }
}

/* 拖拽分割线样式与容器拉伸 */
.split-resizer {
  width: 6px;
  background: var(--line);
  border-radius: 6px;
  cursor: col-resize;
  position: relative;
  align-self: stretch;
}
.split-resizer::before {
  content: '';
  position: absolute;
  top: 0; bottom: 0;
  left: -6px; right: -6px; /* 扩大可点击/触控区域 */
}
.split-resizer:hover,
.split-resizer.active {
  background: #cfe8dc;
}
@media (prefers-color-scheme: dark) {
  .split-resizer:hover,
  .split-resizer.active {
    background: color-mix(in oklab, var(--line), var(--brand) 30%);
  }
}

/* 容器拉伸与滚动优化 */
.memories-page { align-items: stretch; }
.left, .right { min-width: 0; overflow: auto; }
</style>