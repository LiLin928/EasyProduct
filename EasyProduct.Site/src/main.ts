import { createPinia } from 'pinia'
import { createApp } from 'vue'
import App from './App.vue'
import { i18n, refreshRemoteOverrides } from './i18n'
import { router } from './router'
import './assets/styles/reset.scss'
import './assets/styles/global.scss'

const app = createApp(App)
app.use(createPinia())
app.use(i18n)
app.use(router)
app.mount('#app')

refreshRemoteOverrides()
