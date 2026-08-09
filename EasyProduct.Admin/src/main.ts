import { createPinia } from 'pinia'
import { createApp } from 'vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import App from './App.vue'
import { i18n } from './i18n'
import { setupRouter } from './router'
import './styles/index.scss'

const app = createApp(App)
app.use(createPinia())
app.use(i18n)
app.use(ElementPlus)
setupRouter(app)
app.mount('#app')