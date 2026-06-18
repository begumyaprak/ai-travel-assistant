import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
})

export type DocumentStatus = 'Uploaded' | 'Processing' | 'Processed' | 'Failed'

const STATUS_MAP: Record<number, DocumentStatus> = {
  0: 'Uploaded',
  1: 'Processing',
  2: 'Processed',
  3: 'Failed',
}

export interface Document {
  id: string
  fileName: string
  category: string
  destination: string | null
  language: string
  status: DocumentStatus
  failureReason: string | null
  uploadedAt: string
  uploadedBy: string
}

export function normalizeDocument(raw: Document & { status: number | DocumentStatus }): Document {
  return {
    ...raw,
    status: typeof raw.status === 'number' ? (STATUS_MAP[raw.status] ?? 'Uploaded') : raw.status,
  }
}

export const documentsApi = {
  getAll: () => api.get<{ data: Document[] }>('/api/documents'),
  getStatus: (id: string) => api.get<{ data: Document }>(`/api/documents/${id}/status`),
  upload: (formData: FormData) => api.post('/api/documents/upload', formData),
  retry: (id: string) => api.post(`/api/documents/${id}/retry`),
}
