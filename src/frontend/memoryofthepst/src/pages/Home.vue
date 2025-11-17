<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { get } from '../services/http'

const loading = ref(false)
const result = ref<any>(null)
const error = ref<string>('')

onMounted(async () => {
  loading.value = true
  try {
    // 演示 GET 请求：具体接口路径可根据后端服务调整
    // 例如：/ping、/health 或 /api/v1/demo
    result.value = await get<any>('/ping')
  } catch (e) {
    error.value = (e as any)?.message ?? '请求失败'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <section class="page">
    <h2>首页</h2>
    <p class="desc">演示 axios 封装：GET /ping</p>
    <div class="box">
      <p v-if="loading">加载中...</p>
      <pre v-else-if="result">{{ result }}</pre>
      <p v-else-if="error" class="error">{{ error }}</p>
      <p v-else>暂无数据</p>
    </div>
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
.box {
  padding: 12px;
  border: 1px solid #eee;
  border-radius: 8px;
  background: #fafafa;
}
.error {
  color: #d93025;
}
</style>