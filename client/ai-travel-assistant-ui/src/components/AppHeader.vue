<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<template>
  <header class="app-header">
    <div class="header-inner">
      <RouterLink to="/documents" class="brand">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          width="22"
          height="22"
          class="brand-svg"
        >
          <path
            d="M17.8 19.2L16 11l3.5-3.5C21 6 21.5 4 21 3c-1-.5-3 0-4.5 1.5L13 8 4.8 6.2c-.5-.1-.9.1-1.1.5l-.3.5c-.2.5-.1 1 .3 1.3L9 12l-2 3H4l-1 1 3 2 2 3 1-1v-3l3-2 3.5 5.3c.3.4.8.5 1.3.3l.5-.3c.4-.2.6-.6.5-1.1z"
          />
        </svg>
        <span class="brand-name">TravelDocs <span class="brand-accent">AI</span></span>
      </RouterLink>

      <nav class="nav">
        <RouterLink to="/documents">Documents</RouterLink>
        <RouterLink to="/upload">Upload</RouterLink>
        <RouterLink to="/ask">Ask</RouterLink>
      </nav>

      <div class="header-right">
        <button class="avatar-btn" type="button" aria-label="User menu">
          <i class="pi pi-user" />
        </button>
      </div>
    </div>
  </header>
</template>

<style scoped>
.app-header {
  background: #ffffff;
  border-bottom: 1px solid #e4e1ed;
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-inner {
  display: grid;
  grid-template-columns: 1fr auto 1fr;
  align-items: center;
  height: 64px;
  max-width: 1440px;
  margin: 0 auto;
  padding: 0 2rem;
}

.brand {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  text-decoration: none;
  justify-self: start;
}

.brand-svg {
  color: #4648d4;
}

.brand-name {
  font-size: 1rem;
  font-weight: 700;
  color: #4648d4;
  letter-spacing: -0.01em;
}

.brand-accent {
  color: #4648d4;
}

.nav {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  justify-self: center;
}

.nav a {
  padding: 0.5rem 1rem;
  color: #464554;
  text-decoration: none;
  font-size: 0.875rem;
  font-weight: 500;
  border-bottom: 2px solid transparent;
  transition: color 0.15s, border-color 0.15s;
}

.nav a:hover {
  color: #1b1b23;
}

.nav a.router-link-active {
  color: #4648d4;
  border-bottom-color: #4648d4;
}

.header-right {
  display: flex;
  justify-content: flex-end;
}

.avatar-btn {
  width: 34px;
  height: 34px;
  border-radius: 50%;
  border: 1px solid #c7c4d7;
  background: #f5f2fe;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #464554;
  font-size: 0.85rem;
  transition: background 0.15s;
}

.avatar-btn:hover {
  background: #e9e6f3;
}
</style>
