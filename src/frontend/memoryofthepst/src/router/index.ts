import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import DefaultLayout from '../layouts/DefaultLayout.vue';
import Home from '../pages/Home.vue';
import About from '../pages/About.vue';
import Chat from '../pages/Chat.vue';
import Start from '../pages/Start.vue';
import Login from '../pages/Login.vue';
import Register from '../pages/Register.vue';
import Memories from '../pages/Memories.vue';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: DefaultLayout,
    children: [
      { path: '', name: 'Start', component: Start, meta: { title: '开始' } },
      { path: 'home', name: 'Home', component: Home, meta: { title: '首页' } },
      { path: 'memories', name: 'Memories', component: Memories, meta: { title: '往事回忆' } },
      { path: 'login', name: 'Login', component: Login, meta: { title: '登录' } },
      { path: 'register', name: 'Register', component: Register, meta: { title: '注册' } },
      { path: 'chat', name: 'Chat', component: Chat, meta: { title: '往事回首' } },
      { path: 'about', name: 'About', component: About, meta: { title: '关于' } }
    ]
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() {
    return { top: 0 };
  }
});

router.afterEach((to) => {
  if (to.meta?.title) {
    document.title = String(to.meta.title);
  }
});

export default router;