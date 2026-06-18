<script setup lang="ts">
import type { Source } from '@/api/qa'

defineProps<{ source: Source }>()
</script>

<template>
  <div class="source-card">
    <div class="source-header">
      <i class="pi pi-file" />
      <span class="file-name">{{ source.fileName }}</span>
      <span class="score">score: {{ source.relevanceScore.toFixed(2) }}</span>
    </div>
    <p v-if="source.pageReference" class="page-ref">{{ source.pageReference }}</p>
  </div>
</template>

<style scoped>
.source-card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 1rem;
}

.source-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.source-header .pi-file {
  color: #3b82f6;
}

.file-name {
  font-size: 0.875rem;
  font-weight: 500;
  flex: 1;
}

.score {
  font-size: 0.75rem;
  color: #64748b;
}

.page-ref {
  font-size: 0.75rem;
  color: #94a3b8;
  margin: 0.5rem 0 0;
}
</style>
