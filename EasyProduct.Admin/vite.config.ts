import { fileURLToPath, URL } from 'node:url'
import vue from '@vitejs/plugin-vue'
import { defineConfig, loadEnv } from 'vite'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd())
  return {
    plugins: [vue()],
    resolve: {
      alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) },
    },
    server: {
      port: 5173,
      proxy: {
        // F0~mock 阶段指向 7700；后端联调改 .env.development 的 VITE_PROXY_TARGET=http://localhost:7600
        '/api': {
          target: env.VITE_PROXY_TARGET || 'http://localhost:7700',
          changeOrigin: true,
        },
      },
    },
    css: {
      preprocessorOptions: {
        scss: {
          additionalData: '@use "@/styles/variables.scss" as *;@use "@/styles/mixins.scss" as *;',
        },
      },
    },
  }
})