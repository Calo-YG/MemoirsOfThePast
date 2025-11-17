<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, RouterLink, RouterView } from 'vue-router'

type NavItem = { name: string; path: string; icon: 'home' | 'info' }

const route = useRoute()
const mobileMenuOpen = ref(false)

const navItems: NavItem[] = [
  { name: '开始', path: '/', icon: 'home' },
  { name: '首页', path: '/home', icon: 'home' },
  { name: 'AI 聊天', path: '/chat', icon: 'info' },
  { name: '关于', path: '/about', icon: 'info' }
]

function toggleMobileMenu() {
  mobileMenuOpen.value = !mobileMenuOpen.value
}
function closeMobileMenu() {
  mobileMenuOpen.value = false
}
</script>

<template>
  <div class="app-layout">
    <header class="header">
      <div class="container">
        <div class="brand">
          <div class="logo" aria-label="Logo">
            <svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true">
              <defs>
                <linearGradient id="g" x1="0" y1="0" x2="1" y2="1">
                  <stop offset="0" stop-color="#42b883" />
                  <stop offset="1" stop-color="#2c3e50" />
                </linearGradient>
              </defs>
              <path d="M12 2l9 5v10l-9 5-9-5V7l9-5z" fill="url(#g)" />
              <path d="M12 6l5.5 3v6L12 18l-5.5-3V9L12 6z" fill="#fff" opacity=".9" />
            </svg>
          </div>
          <span class="title">Memoirs Of The Past</span>
        </div>

        <nav class="nav desktop" aria-label="主导航">
          <ul class="nav-list">
            <li v-for="item in navItems" :key="item.path">
              <RouterLink :to="item.path" class="nav-link" exact-active-class="active">
                <span class="icon" aria-hidden="true">
                  <svg v-if="item.icon === 'home'" viewBox="0 0 24 24" width="18" height="18">
                    <path fill="currentColor" d="M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z"/>
                  </svg>
                  <svg v-else viewBox="0 0 24 24" width="18" height="18">
                    <path fill="currentColor" d="M12 2a10 10 0 100 20 10 10 0 000-20zm-1 7h2V7h-2v2zm0 8h2v-6h-2v6z"/>
                  </svg>
                </span>
                <span class="text">{{ item.name }}</span>
              </RouterLink>
            </li>
          </ul>
        </nav>

        <div class="actions">
          <div class="search">
            <svg viewBox="0 0 24 24" width="18" height="18" class="search-icon">
              <path fill="currentColor" d="M15.5 14h-.79l-.28-.27A6.471 6.471 0 0016 9.5 6.5 6.5 0 109.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79L20 21.5 21.5 20l-6-6zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/>
            </svg>
            <input class="search-input" placeholder="搜索…" />
          </div>

          <button class="ghost-btn" title="切换主题（示例）">
            <svg viewBox="0 0 24 24" width="18" height="18">
              <path fill="currentColor" d="M9.37 5.51A7 7 0 0012 19a7 7 0 006.49-9.63A9 9 0 119.37 5.51z"/>
            </svg>
          </button>

          <div class="avatar" title="用户">
            <span>MO</span>
          </div>

          <button class="menu-btn" @click="toggleMobileMenu" aria-label="打开/收起菜单">
            <svg viewBox="0 0 24 24" width="20" height="20">
              <path fill="currentColor" d="M3 6h18v2H3V6zm0 5h18v2H3v-2zm0 5h18v2H3v-2z"/>
            </svg>
          </button>
        </div>
      </div>

      <nav class="nav mobile" aria-label="主导航（移动端）" v-show="mobileMenuOpen">
        <ul class="mobile-nav-list">
          <li v-for="item in navItems" :key="item.path">
            <RouterLink :to="item.path" class="mobile-nav-link" exact-active-class="active" @click="closeMobileMenu">
              <span class="icon" aria-hidden="true">
                <svg v-if="item.icon === 'home'" viewBox="0 0 24 24" width="18" height="18">
                  <path fill="currentColor" d="M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z"/>
                </svg>
                <svg v-else viewBox="0 0 24 24" width="18" height="18">
                  <path fill="currentColor" d="M12 2a10 10 0 100 20 10 10 0 000-20zm-1 7h2V7h-2v2zm0 8h2v-6h-2v6z"/>
                </svg>
              </span>
              <span class="text">{{ item.name }}</span>
            </RouterLink>
          </li>
        </ul>
      </nav>
    </header>

    <div class="page-meta">
      <div class="breadcrumbs">
        <span class="crumb">当前位置：</span>
        <span class="crumb active">{{ route.meta?.title ?? '页面' }}</span>
      </div>
    </div>

    <main class="main">
      <RouterView />
    </main>

    <footer class="footer">
      <div class="container footer-inner">
        <span>© {{ new Date().getFullYear() }} Memoirs Of The Past</span>
        <span class="dot">·</span>
        <span>Vue 3 + Vite</span>
      </div>
    </footer>
  </div>
</template>

<style scoped>
:root {
  --header-h: 64px;
  --radius: 12px;
  --bg: #f6f7f9;
  --card: #ffffff;
  --line: #e9edf2;
  --text: #1f2937;
  --text-weak: #6b7280;
  --brand: #42b883;
  --brand-weak: rgba(66, 184, 131, 0.12);
}

* {
  box-sizing: border-box;
}

.app-layout {
  min-height: 100vh;
  background: var(--bg);
  display: flex;
  flex-direction: column;
}
@supports (height: 100dvh) {
  .app-layout {
    min-height: 100dvh;
  }
}

.header {
  position: sticky;
  top: 0;
  z-index: 20;
  background: var(--card);
  border-bottom: 1px solid var(--line);
  box-shadow: 0 2px 8px rgba(0,0,0,.02);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  height: var(--header-h);
  padding: 0 16px;
  display: flex;
  align-items: center;
  gap: 16px;
}

.brand {
  display: flex;
  align-items: center;
  gap: 10px;
}
.logo {
  display: grid;
  place-items: center;
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background: var(--brand-weak);
  color: var(--brand);
}
.title {
  font-weight: 700;
  color: var(--text);
  letter-spacing: .2px;
  white-space: nowrap;
}

.nav.desktop {
  display: none;
}

.actions {
  margin-left: auto;
  display: flex;
  align-items: center;
  gap: 8px;
}

.search {
  display: none;
  align-items: center;
  gap: 6px;
  padding: 0 10px;
  height: 36px;
  border: 1px solid var(--line);
  border-radius: 10px;
  background: #fff;
}
.search-input {
  border: none;
  outline: none;
  font-size: 14px;
  color: var(--text);
  width: 180px;
  background: transparent;
}
.search-icon {
  color: var(--text-weak);
}

.ghost-btn,
.menu-btn {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  border: 1px solid var(--line);
  background: #fff;
  color: var(--text-weak);
  display: grid;
  place-items: center;
  cursor: pointer;
}
.ghost-btn:hover,
.menu-btn:hover {
  color: var(--brand);
  border-color: #cfe8dc;
  background: var(--brand-weak);
}

.avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: linear-gradient(135deg, #42b883, #2c8b64);
  color: #fff;
  font-size: 12px;
  font-weight: 700;
  display: grid;
  place-items: center;
}

/* 并排导航（桌面端） */
.nav-list {
  display: flex;
  align-items: center;
  gap: 6px;
  list-style: none;
  margin: 0;
  padding: 0;
}
.nav-link {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  border-radius: 10px;
  color: var(--text);
  text-decoration: none;
  border: 1px solid transparent;
  transition: background .2s ease, color .2s ease, border-color .2s ease;
}
.nav-link:hover {
  background: #f2f6f3;
  color: #219e6f;
  border-color: #d9efe5;
}
.nav-link.active,
.router-link-exact-active.nav-link {
  background: var(--brand-weak);
  color: var(--brand);
  border-color: #d6efe4;
}
.icon {
  width: 20px;
  height: 20px;
  display: grid;
  place-items: center;
  color: currentColor;
}
.text {
  font-size: 14px;
}

/* 移动端下拉导航 */
.nav.mobile {
  display: block;
  border-top: 1px solid var(--line);
  background: var(--card);
  box-shadow: 0 8px 20px rgba(0,0,0,.05);
}
.mobile-nav-list {
  display: grid;
  gap: 6px;
  padding: 8px 16px 12px;
  list-style: none;
  margin: 0;
}
.mobile-nav-link {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  border-radius: 10px;
  color: var(--text);
  text-decoration: none;
  border: 1px solid transparent;
}
.mobile-nav-link:hover {
  background: #f7faf9;
  border-color: #e6f3ed;
}
.mobile-nav-link.active,
.router-link-exact-active.mobile-nav-link {
  background: var(--brand-weak);
  color: var(--brand);
  border-color: #d6efe4;
}

.page-meta {
  max-width: 1200px;
  margin: 12px auto 0;
  padding: 0 16px;
}
.breadcrumbs {
  color: var(--text-weak);
  font-size: 12px;
}
.breadcrumbs .active {
  color: var(--text);
  font-weight: 600;
}

.main {
  max-width: 1200px;
  width: 100%;
  margin: 12px auto 24px;
  padding: 18px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  min-height: calc(100vh - var(--header-h) - 160px);
}

.footer {
  margin-top: auto;
  color: var(--text-weak);
  background: transparent;
}
.footer-inner {
  height: 56px;
  display: flex;
  align-items: center;
  gap: 8px;
  border-top: 1px solid var(--line);
}
.footer .dot { color: #cbd5e1; }

/* 响应式规则 */
@media (min-width: 768px) {
  .nav.desktop {
    display: block;
  }
  .menu-btn {
    display: none;
  }
  .search {
    display: inline-flex;
  }
  .nav.mobile {
    display: none;
  }
}

@media (max-width: 1024px) {
  .nav-list {
    overflow-x: auto;
    padding-bottom: 6px;
  }
  .nav-list::-webkit-scrollbar {
    height: 6px;
  }
  .nav-list::-webkit-scrollbar-thumb {
    background: #e2e8f0;
    border-radius: 6px;
  }
}
</style>
<!-- 响应式增强覆盖：顶部并排菜单与整体适配（追加样式覆盖） -->
<style scoped>
:root {
  --container-max: 1280px;
  --pad-inline: clamp(12px, 3vw, 24px);
}

/* 容器宽度采用流式 + clamp 边距 */
.container {
  width: 100%;
  max-width: var(--container-max);
  padding: 0 var(--pad-inline);
}

/* 并排菜单在桌面端允许换行，避免溢出；保持行间距 */
.nav-list {
  flex-wrap: wrap;
  row-gap: 6px;
}

/* 使用现代视口单位，避免移动端地址栏影响高度计算 */
@supports (height: 100dvh) {
  .main {
    min-height: calc(100dvh - var(--header-h) - 160px);
  }
}

/* 小屏适配：容器可换行，降低内边距，主要卡片铺满屏宽 */
@media (max-width: 768px) {
  .header .container {
    height: auto;
    padding: 8px var(--pad-inline);
    flex-wrap: wrap;
    gap: 10px;
  }
  .title {
    font-size: 15px;
  }
  .main {
    margin: 8px 0 16px;
    border-radius: 0;
    padding: 14px;
    min-height: auto;
  }
  .page-meta {
    margin: 8px 0 0;
    padding: 0 var(--pad-inline);
  }
  .footer-inner {
    padding: 0 var(--pad-inline);
  }
}

/* 中等屏幕收敛搜索（保留桌面导航） */
@media (max-width: 1024px) {
  .search { display: none; }
}
/* 小屏切换为移动菜单 */
@media (max-width: 768px) {
  .nav.desktop { display: none; }
  .nav.mobile { display: block; }
}

/* 超宽屏：增大容器最大宽度 */
@media (min-width: 1280px) {
  :root { --container-max: 1400px; }
}
</style>
<!-- 自适配增强：覆盖容器、导航与视口单位，进一步优化不同屏幕下的排版 -->
<style scoped>
:root {
  --container-max: 1280px;
  --pad-inline: clamp(12px, 3vw, 24px);
}

/* 顶部容器采用网格布局，自适应分配品牌/导航/操作区 */
.header .container {
  display: grid;
  grid-template-columns: auto 1fr auto;
  align-items: center;
  gap: clamp(10px, 2vw, 16px);
  height: var(--header-h);
  width: 100%;
  max-width: var(--container-max);
  margin: 0 auto;
  padding-left: max(var(--pad-inline), env(safe-area-inset-left));
  padding-right: max(var(--pad-inline), env(safe-area-inset-right));
}

/* 现代视口单位支持：优先使用 100svh/100dvh，避免移动端地址栏影响 */
@supports (height: 100svh) {
  .app-layout { min-height: 100svh; }
}
@supports (height: 100dvh) {
  .app-layout { min-height: 100dvh; }
}

/* 桌面导航占满中列，允许换行/横向滚动 */
.nav.desktop {
  width: 100%;
  min-width: 0;
}
.nav-list {
  flex-wrap: wrap;
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
  row-gap: 6px;
}

/* 主内容与页眉/页脚容器统一宽度与边距策略 */
.page-meta {
  width: 100%;
  max-width: var(--container-max);
  margin: 12px auto 0;
  padding-left: max(var(--pad-inline), env(safe-area-inset-left));
  padding-right: max(var(--pad-inline), env(safe-area-inset-right));
}
.main {
  width: 100%;
  max-width: var(--container-max);
  margin: 12px auto 24px;
  padding-left: max(var(--pad-inline), env(safe-area-inset-left));
  padding-right: max(var(--pad-inline), env(safe-area-inset-right));
}
.footer .container {
  width: 100%;
  max-width: var(--container-max);
  margin: 0 auto;
  padding-left: max(var(--pad-inline), env(safe-area-inset-left));
  padding-right: max(var(--pad-inline), env(safe-area-inset-right));
}

/* 文本与元素在不同屏幕下的缩放优化 */
.title { font-size: clamp(14px, 2.2vw, 18px); }
.nav-link .text { font-size: clamp(13px, 1.8vw, 14px); }

/* 断点策略：中等屏保留桌面导航，小屏切换为移动菜单，卡片铺满屏宽 */
@media (max-width: 1024px) {
  .search { display: none; }
}
@media (max-width: 768px) {
  .header .container {
    grid-template-columns: 1fr auto;
    height: auto;
    gap: 10px;
  }
  .brand .title { display: none; }
  .nav.desktop { display: none; }
  .nav.mobile { display: block; }

  .main {
    border-radius: 0;
    margin: 8px 0 16px;
    padding-left: calc(var(--pad-inline) * 0.75);
    padding-right: calc(var(--pad-inline) * 0.75);
  }
  .page-meta {
    margin: 8px 0 0;
  }
  .footer-inner {
    padding-left: var(--pad-inline);
    padding-right: var(--pad-inline);
  }
}

/* 超宽屏提升容器最大宽度 */
@media (min-width: 1440px) {
  :root { --container-max: 1440px; }
}
</style>