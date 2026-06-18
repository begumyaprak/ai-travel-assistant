<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { documentsApi } from '@/api/documents'
import Select from 'primevue/select'
import InputText from 'primevue/inputtext'
import Message from 'primevue/message'

const router = useRouter()

const file = ref<File | null>(null)
const category = ref('')
const destination = ref('')
const language = ref('en')
const submitting = ref(false)
const error = ref<string | null>(null)
const validationErrors = ref<string[]>([])
const isDragging = ref(false)
const fileInputRef = ref<HTMLInputElement | null>(null)

const categories = [
  'HotelPolicy',
  'DestinationGuide',
  'BookingFAQ',
  'CancellationRules',
  'TransferInfo',
  'SupportNotes',
]

const languages = [
  { label: 'English', value: 'en' },
  { label: 'Turkish', value: 'tr' },
  { label: 'German', value: 'de' },
  { label: 'French', value: 'fr' },
  { label: 'Spanish', value: 'es' },
]

function pickFile(picked: File | undefined) {
  if (!picked) return
  file.value = picked
}

function onFileInput(event: Event) {
  const input = event.target as HTMLInputElement
  pickFile(input.files?.[0])
}

function onDrop(event: DragEvent) {
  isDragging.value = false
  pickFile(event.dataTransfer?.files[0])
}

function openFilePicker() {
  fileInputRef.value?.click()
}

function formatBytes(bytes: number) {
  return bytes < 1024 * 1024
    ? `${(bytes / 1024).toFixed(1)} KB`
    : `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

async function submit() {
  validationErrors.value = []
  error.value = null

  if (!file.value) validationErrors.value.push('Please select a file.')
  if (!category.value) validationErrors.value.push('Please select a category.')
  if (file.value && file.value.size > 10 * 1024 * 1024) {
    validationErrors.value.push('File size must not exceed 10 MB.')
  }

  if (validationErrors.value.length > 0) return

  submitting.value = true
  try {
    const formData = new FormData()
    formData.append('file', file.value!)
    formData.append('category', category.value)
    formData.append('language', language.value)
    formData.append('uploadedBy', 'anonymous')
    if (destination.value.trim()) {
      formData.append('destination', destination.value.trim())
    }

    await documentsApi.upload(formData)
    router.push('/documents')
  } catch {
    error.value = 'Upload failed. Please try again.'
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div class="upload-page">
    <div class="page-header">
      <h1 class="page-title">Upload Travel Documents</h1>
      <p class="page-subtitle">
        Our AI will process your documents to extract itineraries, confirmation codes, and key
        travel dates automatically.
      </p>
    </div>

    <div class="upload-card">
      <div
        class="dropzone"
        :class="{ 'dropzone--active': isDragging, 'dropzone--filled': !!file }"
        @click="openFilePicker"
        @dragover.prevent="isDragging = true"
        @dragleave.prevent="isDragging = false"
        @drop.prevent="onDrop"
      >
        <input
          ref="fileInputRef"
          type="file"
          accept=".pdf,.docx"
          class="file-input-hidden"
          @change="onFileInput"
        />

        <template v-if="!file">
          <div class="dropzone-icon-wrap">
            <i class="pi pi-file-arrow-up dropzone-icon" />
          </div>
          <p class="dropzone-primary">Click to upload or drag and drop</p>
          <p class="dropzone-hint">Supported formats: PDF, DOCX (Max 10MB)</p>
        </template>

        <template v-else>
          <div class="file-preview">
            <div class="file-preview-icon">
              <i class="pi pi-file-pdf" />
            </div>
            <div class="file-preview-info">
              <span class="file-preview-name">{{ file.name }}</span>
              <span class="file-preview-size">{{ formatBytes(file.size) }}</span>
            </div>
            <button
              class="file-remove-btn"
              type="button"
              @click.stop="file = null"
            >
              <i class="pi pi-times" />
            </button>
          </div>
        </template>
      </div>

      <div class="form-grid">
        <div class="field">
          <label class="field-label">Document Category</label>
          <Select
            v-model="category"
            :options="categories"
            placeholder="Select category"
            class="w-full"
          />
        </div>

        <div class="field">
          <label class="field-label">Destination</label>
          <InputText
            v-model="destination"
            placeholder="e.g. Tokyo, Japan"
            class="w-full"
          />
        </div>

        <div class="field">
          <label class="field-label">Document Language</label>
          <Select
            v-model="language"
            :options="languages"
            option-label="label"
            option-value="value"
            class="w-full"
          />
        </div>
      </div>

      <div v-if="validationErrors.length > 0" class="validation-errors">
        <Message
          v-for="msg in validationErrors"
          :key="msg"
          severity="error"
          :closable="false"
          size="small"
        >
          {{ msg }}
        </Message>
      </div>

      <Message v-if="error" severity="error" :closable="false">{{ error }}</Message>

      <div class="card-divider" />

      <div class="actions">
        <button
          class="cancel-btn"
          type="button"
          :disabled="submitting"
          @click="router.push('/documents')"
        >
          Cancel
        </button>
        <button
          class="upload-btn"
          type="button"
          :disabled="submitting"
          @click="submit"
        >
          <i v-if="submitting" class="pi pi-spin pi-spinner" />
          <span v-else>Upload <i class="pi pi-send" /></span>
        </button>
      </div>
    </div>

    <div class="features">
      <div class="feature">
        <div class="feature-icon">
          <i class="pi pi-shield" />
        </div>
        <h4 class="feature-title">Secure Storage</h4>
        <p class="feature-desc">End-to-end encryption for all documents.</p>
      </div>
      <div class="feature">
        <div class="feature-icon">
          <i class="pi pi-cog" />
        </div>
        <h4 class="feature-title">AI Extraction</h4>
        <p class="feature-desc">Intelligent data parsing in seconds.</p>
      </div>
      <div class="feature">
        <div class="feature-icon">
          <i class="pi pi-sync" />
        </div>
        <h4 class="feature-title">Real-time Sync</h4>
        <p class="feature-desc">Available across all your devices.</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.upload-page {
  display: flex;
  flex-direction: column;
  gap: 2rem;
  max-width: 640px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.page-title {
  font-size: 2rem;
  font-weight: 700;
  color: #1b1b23;
  margin: 0;
  letter-spacing: -0.02em;
}

.page-subtitle {
  font-size: 0.875rem;
  color: #767586;
  margin: 0;
  line-height: 1.6;
}

.upload-card {
  background: #ffffff;
  border: 1px solid #e4e1ed;
  border-radius: 12px;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.dropzone {
  border: 1.5px dashed #c7c4d7;
  border-radius: 10px;
  padding: 2.5rem 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  transition:
    border-color 0.15s,
    background 0.15s;
  text-align: center;
  min-height: 160px;
  justify-content: center;
}

.dropzone:hover,
.dropzone--active {
  border-color: #4648d4;
  background: #f5f2fe;
}

.dropzone--filled {
  padding: 1.25rem 1.5rem;
  border-color: #4648d4;
  background: #f5f2fe;
  min-height: auto;
}

.file-input-hidden {
  display: none;
}

.dropzone-icon-wrap {
  width: 48px;
  height: 48px;
  background: #e1e0ff;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 0.25rem;
}

.dropzone-icon {
  font-size: 1.25rem;
  color: #4648d4;
}

.dropzone-primary {
  font-size: 0.9rem;
  font-weight: 500;
  color: #1b1b23;
  margin: 0;
}

.dropzone-hint {
  font-size: 0.8rem;
  color: #767586;
  margin: 0;
}

.file-preview {
  display: flex;
  align-items: center;
  gap: 0.875rem;
  width: 100%;
}

.file-preview-icon {
  width: 40px;
  height: 40px;
  background: #e1e0ff;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.1rem;
  color: #4648d4;
  flex-shrink: 0;
}

.file-preview-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
  text-align: left;
  min-width: 0;
}

.file-preview-name {
  font-size: 0.875rem;
  font-weight: 600;
  color: #1b1b23;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.file-preview-size {
  font-size: 0.75rem;
  color: #767586;
}

.file-remove-btn {
  width: 28px;
  height: 28px;
  background: #ffffff;
  border: 1px solid #e4e1ed;
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #767586;
  flex-shrink: 0;
  transition: background 0.15s;
  font-size: 0.75rem;
}

.file-remove-btn:hover {
  background: #fee2e2;
  color: #b91c1c;
  border-color: #fca5a5;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-grid .field:last-child {
  grid-column: 1;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.field-label {
  font-size: 0.8rem;
  font-weight: 500;
  color: #464554;
}

.w-full {
  width: 100%;
}

.validation-errors {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.card-divider {
  height: 1px;
  background: #f0eefb;
  margin: 0 -1.5rem;
}

.actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 0.75rem;
}

.cancel-btn {
  padding: 0.55rem 1.25rem;
  background: transparent;
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 500;
  color: #464554;
  cursor: pointer;
  font-family: inherit;
  transition: background 0.15s;
}

.cancel-btn:hover:not(:disabled) {
  background: #f0eefb;
}

.cancel-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.upload-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.55rem 1.5rem;
  background: #4648d4;
  color: #ffffff;
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: inherit;
  cursor: pointer;
  transition: background 0.15s;
}

.upload-btn:hover:not(:disabled) {
  background: #3a3cb8;
}

.upload-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.upload-btn .pi-send {
  font-size: 0.8rem;
}

.features {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 2rem;
  text-align: center;
  padding: 0.5rem 0;
}

.feature {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.feature-icon {
  width: 40px;
  height: 40px;
  background: #e1e0ff;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
  color: #4648d4;
  margin-bottom: 0.25rem;
}

.feature-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: #1b1b23;
  margin: 0;
}

.feature-desc {
  font-size: 0.78rem;
  color: #767586;
  margin: 0;
  line-height: 1.5;
}
</style>
