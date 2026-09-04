// stores/base.store.ts

type Listener<T> = (state: T) => void

/** 全局状态基类：getState/setState/subscribe（F3 cart/member store 继承它） */
export class BaseStore<T> {
  protected state: T
  private listeners: Array<Listener<T>> = []

  constructor(initial: T) {
    this.state = initial
  }

  getState(): T {
    return this.state
  }

  setState(partial: Partial<T>): void {
    this.state = { ...this.state, ...partial }
    this.listeners.forEach((fn) => fn(this.state))
  }

  subscribe(fn: Listener<T>): () => void {
    this.listeners.push(fn)
    return () => {
      this.listeners = this.listeners.filter((l) => l !== fn)
    }
  }
}
