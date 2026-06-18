import { defineStore } from 'pinia'
import { ref } from 'vue'
import { qaApi, normalizeQaResponse, type AskQuestionResponse } from '@/api/qa'

export const useQaStore = defineStore('qa', () => {
  const response = ref<AskQuestionResponse | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function ask(question: string) {
    loading.value = true
    error.value = null
    response.value = null
    try {
      const result = await qaApi.ask(question)
      response.value = normalizeQaResponse(result.data.data)
    } catch {
      error.value = 'Something went wrong. Please try again.'
    } finally {
      loading.value = false
    }
  }

  return { response, loading, error, ask }
})
