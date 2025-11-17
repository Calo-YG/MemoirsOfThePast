<script setup lang="ts">
</script>

<template>
  <section class="page">
    <h2>关于</h2>
    <p class="desc">这是一个使用 Vue 3 + TypeScript + Vite 的示例项目，演示提交规范、Axios 封装与基础布局。</p>
    <ul class="list">
      <li>提交规范：commitlint + cz-git + husky + lint-staged</li>
      <li>网络封装：src/services/http.ts</li>
      <li>布局与路由：src/layouts/DefaultLayout.vue + vue-router</li>
    </ul>
  </section>
</template>

<style scoped>
.page {
  display: grid;
  gap: 12px;
}
.desc {
  color: #666;
}
.list {
  padding-left: 18px;
}
</style>