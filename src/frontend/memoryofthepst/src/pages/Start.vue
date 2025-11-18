<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<template>
  <section class="start-page">
    <div class="hero">
      <div class="brand">
        <div class="logo" aria-label="Logo">
          <svg viewBox="0 0 24 24" width="28" height="28" aria-hidden="true">
            <defs>
              <linearGradient id="g-start" x1="0" y1="0" x2="1" y2="1">
                <stop offset="0" stop-color="#42b883" />
                <stop offset="1" stop-color="#2c3e50" />
              </linearGradient>
            </defs>
            <path d="M12 2l9 5v10l-9 5-9-5V7l9-5z" fill="url(#g-start)" />
            <path d="M12 6l5.5 3v6L12 18l-5.5-3V9L12 6z" fill="#fff" opacity=".9" />
          </svg>
        </div>
        <h1 class="title">Memoirs Of The Past</h1>
      </div>

      <p class="subtitle">记录与探索你的灵感与过往，连接现在与未来。</p>

      <div class="actions">
        <RouterLink to="/login" class="btn primary">
          <svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M10 17l5-5-5-5v10zM4 5h4v14H4V5zm14 0h2v14h-2V5z"/></svg>
          <span>登录</span>
        </RouterLink>
        <RouterLink to="/register" class="btn">
          <svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M12 12c2.76 0 5-2.24 5-5S14.76 2 12 2 7 4.24 7 7s2.24 5 5 5zm0 2c-4.42 0-8 1.79-8 4v2h16v-2c0-2.21-3.58-4-8-4z"/></svg>
          <span>注册</span>
        </RouterLink>
        <RouterLink to="/chat" class="btn">
          <svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M4 4h16v12H5.17L4 17.17V4zm0 14h16v2H4v-2z"/></svg>
          <span>AI 聊天</span>
        </RouterLink>
      </div>
    </div>

    <div class="feature-grid">
      <article class="card">
        <h3>快速记录</h3>
        <p>轻松创建与管理你的条目，保持知识持续进化。</p>
      </article>
      <article class="card">
        <h3>智能助手</h3>
        <p>借助 AI 帮助理解、总结与联想，让信息更有价值。</p>
      </article>
      <article class="card">
        <h3>整洁布局</h3>
        <p>响应式与美观设计，专注内容本身的表达。</p>
      </article>
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
}

.start-page {
  width: 100%;
  min-height: 100%;
  display: grid;
  gap: clamp(16px, 4vw, 24px);
  grid-template-rows: auto auto 1fr; /* 英雄区 / 操作 / 特性栅格 */
  align-content: start;
}

.hero {
  display: grid;
  gap: 14px;
  padding: clamp(18px, 4.2vw, 28px);
  background: linear-gradient(180deg, var(--card), color-mix(in oklab, var(--card), #000 4%));
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: 0 6px 24px rgba(0,0,0,.06);
  min-height: clamp(220px, 36vh, 480px);
}
.brand {
  display: flex;
  align-items: center;
  gap: 10px;
}
.logo {
  display: grid;
  place-items: center;
  width: 42px;
  height: 42px;
  border-radius: 12px;
  background: var(--brand-weak);
  color: var(--brand);
  box-shadow: 0 0 0 8px var(--brand-weak) inset, 0 6px 18px rgba(0,0,0,.06);
}
.title {
  font-size: clamp(20px, 3.4vw, 28px);
  font-weight: 800;
  letter-spacing: .3px;
  color: var(--text);
  margin: 0;
}
.subtitle {
  color: var(--text-weak);
  font-size: clamp(14px, 2.3vw, 16px);
  margin: 0;
  max-width: 72ch; /* 控制行宽提升可读性 */
}

.actions {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-top: 6px;
}
.btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  height: 40px;
  padding: 0 16px;
  border-radius: 12px;
  border: 1px solid var(--line);
  background: var(--card);
  color: var(--text);
  text-decoration: none;
  cursor: pointer;
  transition: background .18s ease, border-color .18s ease, color .18s ease, box-shadow .18s ease, transform .18s ease;
  box-shadow: 0 4px 16px rgba(0,0,0,.04);
}
.btn:hover {
  background: var(--brand-weak);
  border-color: #cfe8dc;
  color: var(--brand);
  transform: translateY(-1px);
}
.btn:focus-visible {
  outline: 2px solid var(--brand);
  outline-offset: 2px;
}
.btn.primary {
  background: var(--brand);
  color: #fff;
  border-color: #34c48a;
}
.btn.primary:hover {
  background: #34c48a;
  color: #fff;
}

.feature-grid {
  position: relative;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  grid-auto-rows: 1fr; /* 让卡片在同一行等高 */
  gap: clamp(14px, 3vw, 20px);
  align-items: stretch;
}
.card {
  position: relative;
  display: grid;
  gap: 8px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  padding: clamp(14px, 2.8vw, 18px);
  box-shadow: 0 6px 24px rgba(0,0,0,.06);
  transition: box-shadow .18s ease, border-color .18s ease, background .18s ease, transform .18s ease;
  min-height: clamp(120px, 28vh, 220px);
}
/* 顶部渐变强调条，提升辨识度与层次 */
.card::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 4px;
  background: linear-gradient(90deg, var(--brand), color-mix(in oklab, var(--brand), #000 20%));
  border-top-left-radius: var(--radius);
  border-top-right-radius: var(--radius);
  opacity: .16;
  pointer-events: none;
}
.card:hover {
  transform: translateY(-1px);
  border-color: #cfe8dc;
  background: color-mix(in oklab, var(--card), #000 2%);
  box-shadow: 0 10px 28px rgba(0,0,0,.08);
}
.card:focus-within {
  outline: 2px solid var(--brand);
  outline-offset: 2px;
}
.card h3 {
  margin: 0 0 6px;
  font-size: clamp(16px, 2vw, 18px);
  color: var(--text);
  display: inline-flex;
  align-items: center;
  gap: 8px;
}
/* 标题前置小圆点增强视觉层次 */
.card h3::before {
  content: '';
  width: 8px; height: 8px;
  border-radius: 50%;
  background: var(--brand);
  box-shadow: 0 0 0 6px var(--brand-weak);
}
.card p {
  margin: 0;
  color: var(--text-weak);
  font-size: 13px;
  line-height: 1.6;
}

@media (max-width: 900px) {
  .feature-grid {
    grid-template-columns: 1fr 1fr;
  }
}
@media (max-width: 640px) {
  .feature-grid {
    grid-template-columns: 1fr;
  }
}
</style>