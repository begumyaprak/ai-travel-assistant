import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
})

export interface Source {
  documentId: string
  fileName: string
  chunkContent: string
  pageReference: string | null
  relevanceScore: number
}

export type ConfidenceLevel = 'High' | 'Medium' | 'Low' | 'NotFound'

const CONFIDENCE_MAP: Record<number, ConfidenceLevel> = {
  0: 'High',
  1: 'Medium',
  2: 'Low',
  3: 'NotFound',
}

export interface AskQuestionResponse {
  answer: string
  sources: Source[]
  fromCache: boolean
  confidence: ConfidenceLevel
}

export function normalizeQaResponse(
  raw: AskQuestionResponse & { confidence: number | ConfidenceLevel },
): AskQuestionResponse {
  return {
    ...raw,
    confidence: typeof raw.confidence === 'number' ? (CONFIDENCE_MAP[raw.confidence] ?? 'NotFound') : raw.confidence,
  }
}

export const qaApi = {
  ask: (question: string) =>
    api.post<{ data: AskQuestionResponse }>('/api/qa/ask', { question }),
}
