import { createRouter, createWebHistory } from 'vue-router'
import DocumentsPage from '@/pages/DocumentsPage.vue'
import UploadPage from '@/pages/UploadPage.vue'
import AskPage from '@/pages/AskPage.vue'

export default createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', redirect: '/documents' },
    { path: '/documents', component: DocumentsPage },
    { path: '/upload', component: UploadPage },
    { path: '/ask', component: AskPage },
  ],
})
