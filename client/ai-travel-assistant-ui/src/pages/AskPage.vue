<script setup lang="ts">
import { ref } from 'vue'
import { useQaStore } from '@/stores/qa'
import SourceCard from '@/components/SourceCard.vue'
import ProgressSpinner from 'primevue/progressspinner'
import Message from 'primevue/message'
import Tag from 'primevue/tag'

const store = useQaStore()
const question = ref('')
const validationError = ref('')

const EXAMPLE_QUESTIONS = [
  '"Hotel checkout time in Kyoto"',
  '"Visa requirements for Japan"',
  '"Total trip expenses so far"',
]

const confidenceMap: Record<string, 'success' | 'info' | 'warn' | 'danger'> = {
  High: 'success',
  Medium: 'info',
  Low: 'warn',
  NotFound: 'danger',
}

async function ask() {
  validationError.value = ''
  if (question.value.trim().length < 3) {
    validationError.value = 'Please enter at least 3 characters.'
    return
  }
  await store.ask(question.value.trim())
}

function useExample(q: string) {
  question.value = q.replace(/^"|"$/g, '')
}

function onKeydown(event: KeyboardEvent) {
  if (event.key === 'Enter' && (event.ctrlKey || event.metaKey)) {
    ask()
  }
}
</script>

<template>
  <div class="ask-page">
    <div class="hero">
      <h1 class="hero-title">Travel Assistant</h1>
      <p class="hero-subtitle">Your travel documentation, intelligently queried.</p>
    </div>

    <div class="search-card">
      <textarea
        v-model="question"
        class="question-textarea"
        placeholder="e.g., What time is my flight to Tokyo?"
        rows="4"
        @keydown="onKeydown"
      />
      <div class="search-footer">
        <div class="footer-left">
          <i class="pi pi-info-circle footer-info-icon" />
          <span class="footer-hint">AI will search all uploaded documents</span>
        </div>
        <button
          class="ask-btn"
          type="button"
          :disabled="store.loading"
          @click="ask"
        >
          <span v-if="store.loading" class="ask-btn-inner">
            <i class="pi pi-spin pi-spinner" /> Searching…
          </span>
          <span v-else class="ask-btn-inner">
            Ask <i class="pi pi-send" />
          </span>
        </button>
      </div>
    </div>

    <p v-if="validationError" class="validation-error">
      <i class="pi pi-exclamation-circle" /> {{ validationError }}
    </p>

    <div v-if="!store.loading && !store.response && !store.error" class="examples">
      <span class="examples-label">Try asking:</span>
      <div class="examples-chips">
        <button
          v-for="q in EXAMPLE_QUESTIONS"
          :key="q"
          class="example-chip"
          type="button"
          @click="useExample(q)"
        >
          {{ q }}
        </button>
      </div>
    </div>

    <div v-if="store.loading" class="loading-state">
      <ProgressSpinner stroke-width="3" />
      <p>Searching your documents…</p>
    </div>

    <Message v-else-if="store.error" severity="error" :closable="false">
      {{ store.error }}
    </Message>

    <div v-else-if="store.response" class="results">
      <div class="answer-card">
        <div class="answer-header">
          <h2 class="answer-title">Answer</h2>
          <div class="badges">
            <Tag
              :value="store.response.confidence"
              :severity="confidenceMap[store.response.confidence]"
            />
            <Tag
              :value="store.response.fromCache ? 'Cached' : 'Fresh'"
              :severity="store.response.fromCache ? 'success' : 'secondary'"
            />
          </div>
        </div>

        <p v-if="store.response.confidence === 'NotFound'" class="not-found">
          The uploaded documents do not contain enough information to answer this question.
        </p>
        <p v-else class="answer-text">{{ store.response.answer }}</p>
      </div>

      <div v-if="store.response.sources.length > 0" class="sources-section">
        <h3 class="sources-title">
          <i class="pi pi-book" />
          Sources
          <span class="sources-count">{{ store.response.sources.length }}</span>
        </h3>
        <div class="sources-list">
          <SourceCard
            v-for="(source, index) in store.response.sources"
            :key="index"
            :source="source"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.ask-page {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
  max-width: 700px;
  margin: 0 auto;
}

.hero {
  text-align: center;
  padding: 1.5rem 0 0;
}

.hero-title {
  font-size: 2rem;
  font-weight: 700;
  color: #1b1b23;
  margin: 0 0 0.4rem;
  letter-spacing: -0.02em;
}

.hero-subtitle {
  font-size: 0.9rem;
  color: #767586;
  margin: 0;
}

.search-card {
  width: 100%;
  background: #ffffff;
  border: 1px solid #e4e1ed;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.06);
}

.question-textarea {
  width: 100%;
  border: none;
  outline: none;
  resize: none;
  padding: 1.25rem;
  font-size: 0.9375rem;
  font-family: inherit;
  color: #1b1b23;
  background: transparent;
  line-height: 1.6;
  box-sizing: border-box;
}

.question-textarea::placeholder {
  color: #c7c4d7;
}

.search-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1.25rem;
  border-top: 1px solid #f0eefb;
  background: #faf9ff;
}

.footer-left {
  display: flex;
  align-items: center;
  gap: 0.4rem;
}

.footer-info-icon {
  color: #c7c4d7;
  font-size: 0.85rem;
}

.footer-hint {
  font-size: 0.8rem;
  color: #767586;
}

.ask-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.55rem 1.25rem;
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

.ask-btn:hover:not(:disabled) {
  background: #3a3cb8;
}

.ask-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.ask-btn-inner {
  display: flex;
  align-items: center;
  gap: 0.4rem;
}

.validation-error {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  font-size: 0.8rem;
  color: #b91c1c;
  margin: 0;
}

.examples {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  width: 100%;
}

.examples-label {
  font-size: 0.8rem;
  color: #767586;
}

.examples-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  justify-content: center;
}

.example-chip {
  padding: 0.45rem 1rem;
  border: 1px solid #e4e1ed;
  border-radius: 999px;
  background: #ffffff;
  font-size: 0.8rem;
  color: #464554;
  cursor: pointer;
  font-family: inherit;
  transition: border-color 0.15s, background 0.15s, color 0.15s;
}

.example-chip:hover {
  border-color: #4648d4;
  color: #4648d4;
  background: #f5f2fe;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  padding: 3rem;
  color: #767586;
  font-size: 0.9rem;
}

.results {
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.answer-card {
  background: #ffffff;
  border: 1px solid #e4e1ed;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.answer-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.answer-title {
  font-size: 1rem;
  font-weight: 600;
  color: #1b1b23;
  margin: 0;
}

.badges {
  display: flex;
  gap: 0.5rem;
}

.answer-text {
  color: #1b1b23;
  line-height: 1.7;
  margin: 0;
  font-size: 0.9375rem;
}

.not-found {
  color: #767586;
  font-style: italic;
  margin: 0;
  line-height: 1.6;
}

.sources-section {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.sources-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  font-weight: 600;
  color: #1b1b23;
  margin: 0;
}

.sources-title .pi-book {
  color: #4648d4;
  font-size: 0.85rem;
}

.sources-count {
  font-size: 0.72rem;
  font-weight: 700;
  background: #e1e0ff;
  color: #4648d4;
  border-radius: 999px;
  padding: 0.1rem 0.45rem;
}

.sources-list {
  display: flex;
  flex-direction: column;
  gap: 0.625rem;
}
</style>
