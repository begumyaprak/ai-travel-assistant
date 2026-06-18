import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { documentsApi, normalizeDocument, type Document } from '@/api/documents'

export const useDocumentsStore = defineStore('documents', () => {
  const documents = ref<Document[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchAll() {
    loading.value = true
    error.value = null
    try {
      const response = await documentsApi.getAll()
      documents.value = response.data.data.map(normalizeDocument)
    } catch {
      error.value = 'Failed to load documents.'
    } finally {
      loading.value = false
    }
  }

  const hasProcessing = computed(() =>
    documents.value.some((d) => d.status === 'Processing' || d.status === 'Uploaded'),
  )

  return { documents, loading, error, fetchAll, hasProcessing }
})
