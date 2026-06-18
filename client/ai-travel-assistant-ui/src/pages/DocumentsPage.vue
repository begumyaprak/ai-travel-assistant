<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useDocumentsStore } from '@/stores/documents'
import { documentsApi, type Document } from '@/api/documents'
import Button from 'primevue/button'
import ProgressSpinner from 'primevue/progressspinner'
import Message from 'primevue/message'

const store = useDocumentsStore()
const router = useRouter()

let interval: ReturnType<typeof setInterval> | null = null

onMounted(async () => {
  await store.fetchAll()
  interval = setInterval(async () => {
    if (store.hasProcessing) {
      await store.fetchAll()
    }
  }, 5000)
})

onUnmounted(() => {
  if (interval) clearInterval(interval)
})

async function retry(id: string) {
  await documentsApi.retry(id)
  await store.fetchAll()
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleString('en-US', {
    month: 'short',
    day: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    hour12: true,
  })
}

const LANGUAGE_NAMES: Record<string, string> = {
  en: 'English',
  tr: 'Turkish',
  de: 'German',
  fr: 'French',
  es: 'Spanish',
  ja: 'Japanese',
  it: 'Italian',
  pt: 'Portuguese',
  zh: 'Chinese',
  ar: 'Arabic',
}

function getLanguageName(code: string) {
  return LANGUAGE_NAMES[code.toLowerCase()] ?? code.toUpperCase()
}

function getFileExtension(fileName: string) {
  return fileName.split('.').pop()?.toLowerCase() ?? ''
}

function getFileIconColor(fileName: string) {
  const ext = getFileExtension(fileName)
  if (ext === 'pdf') return '#6366f1'
  if (ext === 'docx' || ext === 'doc') return '#0ea5e9'
  return '#f59e0b'
}

function getFileIcon(fileName: string) {
  const ext = getFileExtension(fileName)
  if (ext === 'pdf') return 'pi-file-pdf'
  if (ext === 'docx' || ext === 'doc') return 'pi-file-word'
  return 'pi-file'
}

function getStatusClass(status: Document['status']) {
  return {
    Uploaded: 'status-uploaded',
    Processing: 'status-processing',
    Processed: 'status-processed',
    Failed: 'status-failed',
  }[status]
}
</script>

<template>
  <div class="documents-page">
    <div class="page-header">
      <div>
        <h1 class="page-title">Documents</h1>
        <p class="page-subtitle">
          Manage and track your travel-related documentation and AI analysis.
        </p>
      </div>
      <Button
        label="Upload Document"
        icon="pi pi-plus"
        @click="router.push('/upload')"
      />
    </div>

    <div v-if="store.loading && store.documents.length === 0" class="loading-state">
      <ProgressSpinner stroke-width="3" />
      <p>Loading documents…</p>
    </div>

    <Message v-else-if="store.error" severity="error" :closable="false">
      {{ store.error }}
    </Message>

    <div v-else-if="store.documents.length === 0" class="empty-state">
      <div class="empty-icon">
        <i class="pi pi-folder-open" />
      </div>
      <h3>No documents yet</h3>
      <p>Upload your first travel document to get started.</p>
      <Button label="Upload Document" icon="pi pi-plus" @click="router.push('/upload')" />
    </div>

    <div v-else class="table-card">
      <table class="docs-table">
        <thead>
          <tr>
            <th>File Name</th>
            <th>Category</th>
            <th>Destination</th>
            <th>Language</th>
            <th>Status</th>
            <th>Uploaded At</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="doc in store.documents" :key="doc.id">
            <td>
              <div class="file-cell">
                <div
                  class="file-icon-box"
                  :style="{ background: getFileIconColor(doc.fileName) + '18', color: getFileIconColor(doc.fileName) }"
                >
                  <i :class="['pi', getFileIcon(doc.fileName)]" />
                </div>
                <span class="file-name">{{ doc.fileName }}</span>
              </div>
            </td>
            <td>{{ doc.category }}</td>
            <td>{{ doc.destination ?? '—' }}</td>
            <td>{{ getLanguageName(doc.language) }}</td>
            <td>
              <span :class="['status-badge', getStatusClass(doc.status)]">
                <i v-if="doc.status === 'Processing'" class="pi pi-spin pi-spinner status-spinner" />
                <i v-if="doc.status === 'Failed'" class="pi pi-exclamation-circle" />
                {{ doc.status }}
              </span>
            </td>
            <td class="date-cell">{{ formatDate(doc.uploadedAt) }}</td>
            <td>
              <button
                v-if="doc.status === 'Failed'"
                class="action-btn"
                type="button"
                title="Retry processing"
                @click="retry(doc.id)"
              >
                <i class="pi pi-refresh" />
              </button>
              <button
                v-else
                class="action-btn"
                type="button"
              >
                <i class="pi pi-ellipsis-v" />
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <div class="table-footer">
        <span class="showing-text">
          Showing 1 to {{ store.documents.length }} of {{ store.documents.length }} documents
        </span>
        <div v-if="store.hasProcessing" class="refresh-note">
          <i class="pi pi-spin pi-sync" />
          Auto-refreshing every 5 s
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.documents-page {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.page-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1rem;
}

.page-title {
  font-size: 2rem;
  font-weight: 700;
  color: #1b1b23;
  margin: 0 0 0.25rem;
  letter-spacing: -0.02em;
}

.page-subtitle {
  font-size: 0.875rem;
  color: #767586;
  margin: 0;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  padding: 5rem 0;
  color: #767586;
  font-size: 0.9rem;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  padding: 5rem 2rem;
  text-align: center;
  background: #ffffff;
  border: 1px solid #e4e1ed;
  border-radius: 12px;
}

.empty-icon {
  width: 64px;
  height: 64px;
  background: #f5f2fe;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.75rem;
  color: #6366f1;
  margin-bottom: 0.25rem;
}

.empty-state h3 {
  font-size: 1.1rem;
  font-weight: 600;
  color: #1b1b23;
  margin: 0;
}

.empty-state p {
  font-size: 0.875rem;
  color: #767586;
  margin: 0 0 0.5rem;
}

.table-card {
  background: #ffffff;
  border: 1px solid #e4e1ed;
  border-radius: 12px;
  overflow: hidden;
}

.docs-table {
  width: 100%;
  border-collapse: collapse;
}

.docs-table thead tr {
  background: #f5f2fe;
}

.docs-table th {
  padding: 0.75rem 1rem;
  text-align: left;
  font-size: 0.78rem;
  font-weight: 600;
  color: #767586;
  letter-spacing: 0.02em;
  text-transform: uppercase;
  white-space: nowrap;
}

.docs-table td {
  padding: 1rem;
  font-size: 0.875rem;
  color: #1b1b23;
  border-top: 1px solid #f0eefb;
  vertical-align: middle;
}

.docs-table tbody tr:hover {
  background: #faf9ff;
}

.file-cell {
  display: flex;
  align-items: center;
  gap: 0.625rem;
}

.file-icon-box {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.85rem;
  flex-shrink: 0;
}

.file-name {
  font-weight: 500;
  color: #1b1b23;
  word-break: break-word;
}

.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.3rem;
  font-size: 0.8rem;
  font-weight: 600;
  padding: 0.25rem 0.6rem;
  border-radius: 999px;
}

.status-processed {
  color: #15803d;
  background: #dcfce7;
}

.status-processing {
  color: #1d4ed8;
  background: #dbeafe;
}

.status-uploaded {
  color: #b45309;
  background: #fef3c7;
}

.status-failed {
  color: #b91c1c;
  background: #fee2e2;
}

.status-spinner {
  font-size: 0.7rem;
}

.date-cell {
  color: #767586;
  white-space: nowrap;
}

.action-btn {
  width: 32px;
  height: 32px;
  border: none;
  background: transparent;
  border-radius: 6px;
  cursor: pointer;
  color: #767586;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.15s, color 0.15s;
}

.action-btn:hover {
  background: #f0eefb;
  color: #1b1b23;
}

.table-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.875rem 1rem;
  border-top: 1px solid #f0eefb;
  background: #faf9ff;
}

.showing-text {
  font-size: 0.8rem;
  color: #767586;
}

.refresh-note {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 0.78rem;
  color: #4648d4;
}
</style>
