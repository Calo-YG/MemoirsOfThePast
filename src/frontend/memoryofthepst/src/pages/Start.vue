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
  display: grid;
  gap: clamp(16px, 4vw, 24px);
}

.hero {
  display: grid;
  gap: 12px;
  padding: clamp(16px, 4vw, 24px);
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
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
}
.title {
  font-size: clamp(18px, 3vw, 24px);
  font-weight: 800;
  letter-spacing: .2px;
  color: var(--text);
  margin: 0;
}
.subtitle {
  color: var(--text-weak);
  font-size: clamp(13px, 2.2vw, 14px);
  margin: 0;
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
  height: 38px;
  padding: 0 14px;
  border-radius: 10px;
  border: 1px solid var(--line);
  background: #fff;
  color: var(--text);
  text-decoration: none;
  cursor: pointer;
  transition: background .2s ease, border-color .2s ease, color .2s ease;
}
.btn:hover {
  background: var(--brand-weak);
  border-color: #cfe8dc;
  color: var(--brand);
}
.btn.primary {
  background: var(--brand);
  color: #fff;
  border-color: transparent;
}
.btn.primary:hover {
  background: #38aa74;
}

.feature-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: clamp(10px, 3vw, 16px);
}
.card {
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  padding: clamp(12px, 2.6vw, 16px);
}
.card h3 {
  margin: 0 0 6px;
  font-size: 16px;
  color: var(--text);
}
.card p {
  margin: 0;
  color: var(--text-weak);
  font-size: 13px;
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