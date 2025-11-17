/* Axios 基础封装：src/services/http.ts */
import axios, { type AxiosError, type AxiosInstance, type AxiosRequestConfig, type AxiosResponse } from 'axios';

/**
 * 通用接口返回包裹类型（兼容多数后端约定式返回）
 * - 支持 { code, message, data } 形式
 * - 也兼容直接返回原始数据的形式
 */
export interface ApiResponse<T = unknown> {
  code?: number | string;
  message?: string;
  data?: T;
}

/** 从环境变量读取基础配置 */
const BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api';
const TIMEOUT = Number(import.meta.env.VITE_REQUEST_TIMEOUT ?? 10000);

/** 创建 axios 实例 */
const http: AxiosInstance = axios.create({
  baseURL: BASE_URL,
  timeout: TIMEOUT,
  headers: {
    'Content-Type': 'application/json'
  }
});

/** 统一错误对象 */
export interface HttpError {
  status?: number;
  message: string;
  url?: string;
  method?: string;
  cause?: unknown;
}

/** 规范化 AxiosError -> HttpError */
function toHttpError(error: AxiosError): HttpError {
  const status = error.response?.status;
  const message =
    (error.response?.data as any)?.message ||
    error.message ||
    '网络请求发生错误';
  const url = error.config?.url;
  const method = error.config?.method?.toUpperCase();
  return {
    status,
    message,
    url,
    method,
    cause: error
  };
}

/** 请求拦截器：可在此注入 Token、TraceId 等 */
http.interceptors.request.use(
  (config) => {
    // 示例：如果有鉴权 Token，可在此附加
    const token = localStorage.getItem('ACCESS_TOKEN');
    if (token) {
      config.headers = config.headers || {};
      (config.headers as Record<string, string>)['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(toHttpError(error))
);

/**
 * 响应拦截器：
 * - 不在此直接解包，保留原始响应，统一在 extractData 中处理
 * - 统一转换错误
 */
http.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => Promise.reject(toHttpError(error))
);

/**
 * 解包响应数据：
 * - 若为 { code, message, data }，当 code 为 0 或 200 认为成功，返回 data
 * - 否则尝试直接返回 body（兼容直接返回原始数据的场景）
 */
function extractData<T>(resp: AxiosResponse<ApiResponse<T> | T>): T {
  const body = resp.data as ApiResponse<T> | T;

  if (body && typeof body === 'object' && 'data' in (body as ApiResponse<T>)) {
    const r = body as ApiResponse<T>;
    const codeNum = Number(r.code ?? 0);
    if (codeNum === 0 || codeNum === 200) {
      return r.data as T;
    }
    throw {
      message: r.message || '业务返回失败',
      cause: body
    } as HttpError;
  }

  // 兼容直接返回原始数据
  return body as T;
}

/** 通用请求方法 */
export async function request<T = unknown>(
  config: AxiosRequestConfig
): Promise<T> {
  try {
    const resp = await http.request<ApiResponse<T> | T>(config);
    return extractData<T>(resp);
  } catch (e) {
    // 保证抛出的为 HttpError
    if ((e as any).isAxiosError) {
      throw toHttpError(e as AxiosError);
    }
    throw e as HttpError;
  }
}

/** HTTP 方法便捷封装 */
export async function get<T = unknown>(
  url: string,
  config?: AxiosRequestConfig
): Promise<T> {
  const resp = await http.get<ApiResponse<T> | T>(url, config);
  return extractData<T>(resp);
}

export async function post<T = unknown, B = unknown>(
  url: string,
  data?: B,
  config?: AxiosRequestConfig
): Promise<T> {
  const resp = await http.post<ApiResponse<T> | T>(url, data, config);
  return extractData<T>(resp);
}

export async function put<T = unknown, B = unknown>(
  url: string,
  data?: B,
  config?: AxiosRequestConfig
): Promise<T> {
  const resp = await http.put<ApiResponse<T> | T>(url, data, config);
  return extractData<T>(resp);
}

export async function patch<T = unknown, B = unknown>(
  url: string,
  data?: B,
  config?: AxiosRequestConfig
): Promise<T> {
  const resp = await http.patch<ApiResponse<T> | T>(url, data, config);
  return extractData<T>(resp);
}

export async function del<T = unknown>(
  url: string,
  config?: AxiosRequestConfig
): Promise<T> {
  const resp = await http.delete<ApiResponse<T> | T>(url, config);
  return extractData<T>(resp);
}

/** 导出实例供特殊场景直接使用 */
export { http };