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

      <div class="center-content">
        <p class="subtitle">探索你的灵感宝库，连接过去与未来，激发无限可能。</p>
 
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
  --radius: 16px;
  --bg: #e9f3ef;
  --card: #ffffff;
  --line: #cfe8dc;
  --text: #1c3d2e;
  --text-weak: #4a6f5a;
  --brand: #2d8c59;
  --brand-weak: rgba(45, 140, 89, 0.15);
}

.start-page {
  width: 100%;
  min-height: 100vh;
  padding: 40px 24px;
  background: linear-gradient(135deg, #d9efe7 0%, #e4f3f1 100%);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 36px;
  box-sizing: border-box;
}

.hero {
  display: flex;
  align-items: center;
  gap: 24px;
  padding: 24px 32px;
  background: var(--card);
  border-radius: var(--radius);
  box-shadow: 0 12px 36px rgba(0,0,0,0.15);
  max-width: 900px;
  width: 100%;
  transition: box-shadow 0.3s ease;
}

.hero:hover {
  box-shadow: 0 18px 48px rgba(0,0,0,0.25);
}

.brand {
  display: flex;
  align-items: center;
  gap: 22px;
}

.logo {
  width: 56px;
  height: 56px;
  border-radius: 20px;
  background: var(--brand-weak);
  color: var(--brand);
  display: grid;
  place-items: center;
  box-shadow: 0 0 0 12px var(--brand-weak) inset, 0 10px 30px rgba(0,0,0,0.12);
  transition: box-shadow 0.3s ease;
}

.logo:hover {
  box-shadow: 0 0 0 12px var(--brand-weak) inset, 0 15px 40px rgba(0,0,0,0.2);
}

.title {
  font-size: clamp(28px, 5vw, 36px);
  font-weight: 900;
  color: var(--text);
  margin: 0;
  text-shadow: 0 0 3px rgba(0,0,0,0.04);
}

.center-content {
  flex: 1;
  max-width: 700px;
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.subtitle {
  color: var(--text-weak);
  font-size: clamp(16px, 3vw, 20px);
  line-height: 1.8;
  margin: 0;
  max-width: 100%;
  user-select: none;
}

.actions {
  display: flex;
  gap: 20px;
  margin-top: 18px;
  flex-wrap: nowrap;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  height: 48px;
  padding: 0 20px;
  border-radius: 16px;
  border: 1px solid var(--line);
  background: var(--card);
  color: var(--text);
  text-decoration: none;
  cursor: pointer;
  transition: background 0.2s ease, border-color 0.2s ease, color 0.2s ease, box-shadow 0.2s ease, transform 0.15s ease;
  font-weight: 600;
  font-size: 16px;
  user-select: none;
}

.btn:hover {
  background: var(--brand-weak);
  border-color: #a7d1b7;
  color: var(--brand);
  transform: translateY(-2px);
  box-shadow: 0 8px 28px rgba(0,0,0,0.10);
}

.btn.primary {
  background: var(--brand);
  color: #fff;
  border-color: #34c48a;
  font-weight: 700;
  font-size: 18px;
}

.btn.primary:hover {
  background: #34c48a;
  color: #fff;
  box-shadow: 0 10px 32px rgba(0,0,0,0.15);
}

.feature-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: clamp(18px, 3vw, 24px);
  width: 100%;
  max-width: 960px;
  align-items: stretch;
}

.card {
  background: var(--card);
  border-radius: var(--radius);
  border: 1px solid var(--line);
  box-shadow: 0 16px 48px rgba(0,0,0,0.1);
  padding: clamp(20px, 4vw, 28px);
  display: flex;
  flex-direction: column;
  gap: 14px;
  min-height: 200px;
  transition: box-shadow 0.3s ease, border-color 0.3s ease, transform 0.3s ease;
}

.card:hover {
  box-shadow: 0 20px 60px rgba(0,0,0,0.18);
  border-color: #abccb8;
  transform: translateY(-4px);
}

.card h3 {
  font-weight: 700;
  font-size: clamp(20px, 4vw, 22px);
  margin: 0 0 8px;
  color: var(--text);
  display: inline-flex;
  align-items: center;
  gap: 12px;
}

.card h3::before {
  content: '';
  width: 14px;
  height: 14px;
  border-radius: 50%;
  background: var(--brand);
  box-shadow: 0 0 0 8px var(--brand-weak);
}

.card p {
  color: var(--text-weak);
  font-size: 15px;
  line-height: 1.9;
  margin: 0;
  user-select: none;
}

@media (max-width: 900px) {
  .hero {
    flex-direction: column;
    align-items: flex-start;
  }
  .actions {
    flex-wrap: wrap;
    gap: 14px;
  }
  .feature-grid {
    grid-template-columns: 1fr 1fr;
  }
}

@media (max-width: 600px) {
  .feature-grid {
    grid-template-columns: 1fr;
  }
  .btn {
    width: 100%;
    justify-content: center;
  }
}
</style>