# MemoirsOfThePast 前端（Vue 3 + TypeScript + Vite）

本工程已集成以下基础能力：
- 提交规范：commitlint + cz-git + husky + lint-staged
- 网络层：Axios 封装，支持统一错误与环境变量配置
- 布局与路由：默认布局 + vue-router 多页面
- 开发代理：Vite devServer 代理到 VITE_API_BASE_URL

---

## 快速开始

1. 安装依赖
   ```bash
   npm install
   ```

2. 启动开发
   ```bash
   npm run dev
   ```

3. 预览生产构建
   ```bash
   npm run build
   npm run preview
   ```

---

## 环境变量

示例文件：`.env.example`，可复制为 `.env` 并按需修改。

- `VITE_API_BASE_URL`：后端服务地址（如 https://api.example.com）
- `VITE_REQUEST_TIMEOUT`：请求超时时间（毫秒）

开发代理规则见 `vite.config.ts`，当 `VITE_API_BASE_URL` 存在时，开发环境将把以 `/api` 开头的请求代理到目标地址并去除 `/api` 前缀。

---

## Axios 封装使用

封装位置：`src/services/http.ts`。

支持：
- axios 实例统一配置（baseURL、timeout、默认头）
- 请求拦截器（自动附加 Authorization 等）
- 响应拦截器（规范化错误为 HttpError）
- 通用方法：`request`、`get`、`post`、`put`、`patch`、`del`

示例（在页面中发起请求）：
```ts
import { get } from '@/services/http'
// 或相对路径：import { get } from '../services/http'

const data = await get<any>('/ping')
```

错误处理：
```ts
try {
  const data = await get('/ping')
} catch (e) {
  // e 可能为 HttpError，包含 status、message、url、method 等
  console.error((e as any)?.message || '请求失败')
}
```

---

## 路由与布局

- 路由定义：`src/router/index.ts`
- 默认布局：`src/layouts/DefaultLayout.vue`
- 页面示例：`src/pages/Home.vue`、`src/pages/About.vue`

入口中接入路由：
- `src/main.ts` 挂载路由实例（`createApp(App).use(router).mount('#app')`）
- `src/App.vue` 通过 `<RouterView />` 渲染实际页面（默认由 `DefaultLayout` 承载）

---

## 开发代理说明

`vite.config.ts` 中在开发环境读取 `VITE_API_BASE_URL`，若存在则启用：
- 以 `/api` 开头的请求将被代理到目标地址
- 代理时会移除 `/api` 前缀，例如：`/api/ping -> https://api.example.com/ping`

这与 `src/services/http.ts` 中的 `baseURL`（默认 `/api`）配合，从而在本地直接请求后端服务。

---

## 提交规范（Conventional Commits）

本工程采用约定式提交并通过 commitlint 校验；使用 cz-git 辅助生成 commit message。

常见类型：
- `feat`：新功能
- `fix`：修复缺陷
- `docs`：文档
- `style`：代码格式（不影响逻辑）
- `refactor`：重构
- `perf`：性能优化
- `test`：测试相关
- `build`：构建系统或依赖变更
- `ci`：CI 配置变更
- `chore`：其他杂项
- `revert`：回滚提交

示例：
```
feat(router): 添加默认布局与基础页面
fix(http): 修复 Axios 类型导入与错误对象规范化
docs(readme): 补充提交规范与使用说明
```

---

## Husky 与 lint-staged

- `commit-msg`：在提交时执行 commitlint 校验
- `pre-commit`：在暂存文件上执行 prettier 格式化

安装与初始化（已在脚本中配置）：
```bash
npm run prepare
# 初始化后已创建钩子：
# .husky/commit-msg -> npx --no-install commitlint --edit $1
# .husky/pre-commit  -> npx --no-install lint-staged
```

使用 cz-git 创建提交：
```bash
npm run commit
# 按提示选择 type、scope、subject 等
```

---

## 分支策略建议

- `main`：稳定发布分支
- `develop`：日常开发集成分支
- `feature/*`：功能分支（合入 develop）
- `hotfix/*`：紧急修复分支（合入 main 与 develop）

---

## 常用命令

- 格式检查：`npm run lint`
- 自动格式化：`npm run format`
- 交互式提交：`npm run commit`
- 开发启动：`npm run dev`
- 生产构建：`npm run build`
- 本地预览：`npm run preview`

---

## 目录结构（关键部分）

```
src/
  assets/                 静态资源
  components/             组件示例（HelloWorld）
  layouts/
    DefaultLayout.vue     默认布局（导航 + 内容区域）
  pages/
    Home.vue              首页示例（演示 GET 请求）
    About.vue             关于页示例
  router/
    index.ts              路由配置（含标题与滚动行为）
  services/
    http.ts               Axios 基础封装
  env.d.ts                Vue SFC 类型声明
main.ts                   入口挂载与路由接入
App.vue                   顶层视图（RouterView）
vite.config.ts            Vite 配置与开发代理
```

---

## 备注

- 如果你的后端不希望去除 `/api` 前缀，可调整 `vite.config.ts` 中的 `rewrite` 规则或修改 `src/services/http.ts` 的 `baseURL`。
- 使用 token 鉴权时，请在请求拦截器中设置 `Authorization` 头部（已留示例）。
