<script setup lang="ts">
import { RouterView } from 'vue-router'
import AppHeader from '@/components/AppHeader.vue'
</script>

<template>
  <AppHeader />
  <main class="main-content">
    <RouterView />
  </main>
</template>

<style>
.main-content {
  max-width: 1440px;
  margin: 0 auto;
  padding: 2rem 2rem;
}
</style>
