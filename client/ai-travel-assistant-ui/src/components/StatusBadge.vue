<script setup lang="ts">
import Tag from 'primevue/tag'

defineProps<{ status: string }>()

const severityMap: Record<string, 'warn' | 'info' | 'success' | 'danger'> = {
  Uploaded: 'warn',
  Processing: 'info',
  Processed: 'success',
  Failed: 'danger',
}
</script>

<template>
  <Tag :severity="severityMap[status]">
    <i v-if="status === 'Processing'" class="pi pi-spin pi-spinner" style="margin-right: 4px" />
    {{ status }}
  </Tag>
</template>
