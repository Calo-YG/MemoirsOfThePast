<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, RouterLink, RouterView } from 'vue-router'
import { isDark, themeMode, resolvedTheme, setTheme } from '../services/theme'

type NavItem = { name: string; path: string; icon: 'home' | 'info' }

const route = useRoute()
const mobileMenuOpen = ref(false)
const showThemePanel = ref(false)

const navItems: NavItem[] = [
  { name: '开始', path: '/', icon: 'home' },
  { name: '首页', path: 'app/home', icon: 'home' },
  { name: '往事回忆', path: 'memories', icon: 'info' },
  { name: '往事回首', path: 'chat', icon: 'info' },
  { name: '关于', path: 'about', icon: 'info' }
]

function toggleMobileMenu() {
  mobileMenuOpen.value = !mobileMenuOpen.value
}
function closeMobileMenu() {
  mobileMenuOpen.value = false
}

function pickTheme(m: 'light' | 'dark' | 'system') {
  setTheme(m)
  showThemePanel.value = false
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

          <div class="theme">
            <button
              class="ghost-btn theme-toggle"
              @click="showThemePanel = !showThemePanel"
              :title="`主题：${themeMode}`"
            >
              <!-- 动态图标：暗色显示月亮，亮色显示太阳 -->
              <svg v-if="isDark" viewBox="0 0 24 24" width="18" height="18" aria-hidden="true">
                <path fill="currentColor" d="M9.37 5.51A7 7 0 0012 19a7 7 0 006.49-9.63A9 9 0 119.37 5.51z"/>
              </svg>
              <svg v-else viewBox="0 0 24 24" width="18" height="18" aria-hidden="true">
                <path fill="currentColor" d="M6.76 4.84l-1.8-1.79-1.41 1.41 1.79 1.8 1.42-1.42zM1 13h3v-2H1v2zm10 10h2v-3h-2v3zM20 13h3v-2h-3v2zM17.66 4.46l1.79-1.8-1.41-1.41-1.8 1.79 1.42 1.42zM12 6a6 6 0 100 12 6 6 0 000-12zm7.24 12.76l1.8 1.79 1.41-1.41-1.79-1.8-1.42 1.42zM4.46 17.66l-1.79 1.8 1.41 1.41 1.8-1.79-1.42-1.42z"/>
              </svg>
            </button>
            <transition name="fade-pop">
              <div
                v-show="showThemePanel"
                class="theme-pop"
                role="menu"
                aria-label="主题选择"
              >
                <button class="ghost-btn theme-option" @click="pickTheme('light')">
                  亮色 <span class="current-badge" v-if="resolvedTheme==='light' && themeMode!=='system'">当前</span>
                </button>
                <button class="ghost-btn theme-option" @click="pickTheme('dark')">
                  暗色 <span class="current-badge" v-if="resolvedTheme==='dark' && themeMode!=='system'">当前</span>
                </button>
                <button class="ghost-btn theme-option" @click="pickTheme('system')">
                  跟随系统 <span class="current-badge" v-if="themeMode==='system'">当前</span>
                </button>
              </div>
            </transition>
          </div>

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
  background: var(--card);
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
  background: var(--card);
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

/* 主题面板样式优化 */
.theme { position: relative; }
.theme-toggle { /* 继承 ghost-btn 尺寸 */ }
.theme-pop {
  position: absolute;
  right: 0;
  top: 44px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: 12px;
  padding: 8px;
  box-shadow: 0 8px 24px rgba(0,0,0,.06);
  display: grid;
  gap: 6px;
  z-index: 50;
  min-width: 160px;
  backdrop-filter: saturate(1.06) blur(6px);
}
.theme-pop::before {
  content: '';
  position: absolute;
  top: -6px;
  right: 14px;
  width: 10px;
  height: 10px;
  background: var(--card);
  border-left: 1px solid var(--line);
  border-top: 1px solid var(--line);
  transform: rotate(45deg);
}

/* 下拉选项覆盖 ghost-btn，使其更贴近列表样式 */
.theme-option {
  width: 100%;
  justify-content: flex-start;
  background: transparent;
  border-color: transparent;
  color: var(--text);
  padding: 8px 10px;
}
.theme-option:hover {
  background: var(--brand-weak);
  color: var(--brand);
  border-color: #d6efe4;
}
.current-badge {
  margin-left: auto;
  color: var(--text-weak);
  font-size: 12px;
}

/* 下拉出现过渡动画 */
.fade-pop-enter-active,
.fade-pop-leave-active {
  transition: opacity .16s ease, transform .18s ease;
}
.fade-pop-enter-from,
.fade-pop-leave-to {
  opacity: 0;
  transform: translateY(-4px) scale(0.98);
}
@media (prefers-reduced-motion: reduce) {
  .fade-pop-enter-active,
  .fade-pop-leave-active {
    transition: none;
  }
}

/* 无障碍可见焦点 */
.theme-option:focus-visible,
.theme-toggle:focus-visible {
  outline: 2px solid var(--brand);
  outline-offset: 2px;
}
</style>
<style scoped>
/* 面包屑（breadcrumbs）视觉优化：胶囊背景、分隔符、溢出省略、暗色适配 */
.page-meta {
  margin: 12px auto 0;
}
.breadcrumbs {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 6px 12px;
  border: 1px solid var(--line);
  border-radius: 999px;
  background: var(--card);
  color: var(--text-weak);
  font-size: 12px;
  line-height: 1;
  box-shadow: 0 6px 20px rgba(0,0,0,.05);
  backdrop-filter: saturate(1.06) blur(4px);
}
/* “当前位置：”提示弱化 */
.breadcrumbs .crumb:first-child {
  color: var(--text-weak);
}
/* 动态标题溢出省略，避免长标题撑破布局 */
.breadcrumbs .crumb {
  display: inline-flex;
  align-items: center;
  max-width: 48vw;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
/* 激活态加粗与主文本色 */
.breadcrumbs .crumb.active {
  color: var(--text);
  font-weight: 600;
  max-width: 56vw;
}
/* 分隔符：为后续项添加 › 符号（不影响首项“当前位置：”） */
.breadcrumbs .crumb + .crumb::before {
  content: "›";
  margin: 0 6px;
  color: #cbd5e1;
}

/* 悬停轻微强化（非移动端） */
@media (hover: hover) and (pointer: fine) {
  .breadcrumbs:hover {
    box-shadow: 0 10px 28px rgba(0,0,0,.06);
    border-color: color-mix(in oklab, var(--line), var(--brand) 10%);
  }
}

/* 小屏收敛边距与半径 */
@media (max-width: 768px) {
  .breadcrumbs {
    padding: 6px 10px;
    border-radius: 12px;
  }
  .breadcrumbs .crumb { max-width: 66vw; }
  .breadcrumbs .crumb.active { max-width: 74vw; }
}
</style>
<style scoped>
/* 屏幕自适配增强：让主内容区域随视口高度与宽度自适应（统一对所有页面生效） */
:root { --footer-h: 56px; }

/* 主内容容器：填满可用宽度，高度基于视口与头/脚高度计算 */
.main {
  width: 100%;
  max-width: var(--container-max);
  display: grid;
  grid-auto-rows: minmax(0, auto);
  min-height: calc(100vh - var(--header-h) - var(--footer-h) - 160px);
  /* 说明：
   * 160px 为页面 meta 与上下内边距的预留空间，避免在极端小屏下高度被撑破。
   * 若后续页面顶部 meta 调整，可按需收敛该值。
   */
}

/* 现代视口单位：优先使用 100dvh/100svh，避免移动端地址栏高度跳变 */
@supports (height: 100dvh) {
  .main { min-height: calc(100dvh - var(--header-h) - var(--footer-h) - 160px); }
}
@supports (height: 100svh) {
  .main { min-height: calc(100svh - var(--header-h) - var(--footer-h) - 160px); }
}

/* 主内容内的页面根节点（section 等）默认拉伸填充 */
.main > * {
  width: 100%;
  min-width: 0;
}

/* 小屏收敛高度预留值：减少顶部/底部占位带来的拥挤感 */
@media (max-width: 768px) {
  .main {
    min-height: calc(100dvh - var(--header-h) - var(--footer-h) - 120px);
  }
}
</style>