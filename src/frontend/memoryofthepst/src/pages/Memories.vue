<script setup lang="ts">
import { ref, computed,onMounted } from 'vue'
import Chat from './Chat.vue'
import { get } from '../services/http'

type Id = string

interface MemoryList {
  id: Id
  name: string
  description: string
  avatar?: string
  background?: string
}

interface FragmentList {
  id: Id
  memoryId: Id
  description: string
  occurDate: string
  location?: string
  scene?: string
}

const showAddMemory = ref(false)
const showDrawer = ref(false)
const selectedMemoryId = ref<Id | null>(null)

const memories = ref<MemoryList[]>([])

const fragments = ref<Record<Id, FragmentList[]>>({})

const selectedMemory = computed(() => memories.value.find(m => m.id === selectedMemoryId.value) || null)
const currentFragments = computed(() => selectedMemoryId.value ? (fragments.value[selectedMemoryId.value] ?? []) : [])

const memForm = ref({ name: '', description: '', avatar: '', background: '' })
const fragForm = ref({ description: '', occurDate: '', location: '', scene: '' })

onMounted(() => loadMemories())

function uid() { return Math.random().toString(36).slice(2) }

function addMemory() {
  const name = memForm.value.name.trim()
  const description = memForm.value.description.trim()
  if (!name || !description) return
  const id = 'm-' + uid()
  const entity: MemoryList = {
    id,
    name,
    description,
    avatar: memForm.value.avatar || '',
    background: memForm.value.background || '',
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
  const entity: FragmentList = {
    id: 'f-' + uid(),
    memoryId: selectedMemoryId.value,
    description: desc,
    occurDate: fragForm.value.occurDate || new Date().toISOString(),
    location: fragForm.value.location || '',
    scene: fragForm.value.scene || '',
  }
  const list = fragments.value[selectedMemoryId.value] ?? []
  fragments.value[selectedMemoryId.value] = [entity, ...list]
  fragForm.value = { description: '', occurDate: '', location: '', scene: '' }
}

async function loadMemories() {
  const result = await get<MemoryList[]>('/memory/getList')
  if(result.length > 0){
     memories.value = result
  }
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

<style lang="css" scoped src="./styles/memories.css"></style>