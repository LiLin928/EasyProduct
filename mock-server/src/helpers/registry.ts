// src/helpers/registry.ts
import { resetInquiryStore } from '../store/inquiry.js'

type Resetter = () => void
const resetters: Resetter[] = []

/** store 模块注册自己的重置函数（F1+ 各模块 store 使用） */
export function registerReset(fn: Resetter): void {
  resetters.push(fn)
}

// 注册询价 store reset
registerReset(resetInquiryStore)

export function resetAll(): void {
  resetters.forEach((fn) => fn())
}