# AI Travel Assistant

A cloud-native .NET backend with a Vue.js frontend that lets you upload travel documents and ask natural language questions against them. Answers are always grounded in the uploaded documents — never hallucinated.

Built as a portfolio project demonstrating **RAG (Retrieval Augmented Generation)**, **Clean Architecture**, and **Azure AI** integration.

---

## What It Does

1. **Upload** travel documents (PDF or DOCX) — hotel policies, destination guides, insurance documents
2. **Process** — the system parses, chunks, and generates embeddings for each document in the background
3. **Ask** — ask any natural language question; the system searches the documents and generates a grounded answer with source citations

---

## Tech Stack

### Backend
| Layer | Technology |
|---|---|
| Framework | .NET 8 Web API (Clean Architecture) |
| AI / Embeddings | Azure OpenAI (`text-embedding-3-small` + `gpt-4o`) |
| Vector Search | Azure AI Search (semantic + vector hybrid) |
| Background Jobs | Hangfire + PostgreSQL |
| Database | PostgreSQL (EF Core) |
| Caching | Redis (Q&A response cache) |
| Document Parsing | PdfPig (PDF), DocumentFormat.OpenXml (DOCX) |

### Frontend
| Layer | Technology |
|---|---|
| Framework | Vue 3 (Composition API + TypeScript) |
| Build Tool | Vite |
| UI Library | PrimeVue (Aura theme) |
| State Management | Pinia |
| HTTP Client | Axios |
| Routing | Vue Router |

---

## Architecture

```
src/
  AiTravelAssistant.API/            → Controllers, Middleware, DI wiring
  AiTravelAssistant.Application/    → Use Cases, CQRS Handlers, Interfaces
  AiTravelAssistant.Domain/         → Entities, Enums, Domain Logic
  AiTravelAssistant.Infrastructure/ → EF Core, Azure AI, Redis, Hangfire

client/
  ai-travel-assistant-ui/           → Vue 3 frontend

docs/
  how-it-works.md                   → Full pipeline walkthrough
  architecture.md                   → Deep dive: backend + frontend structure
```

Clean Architecture with strict layer boundaries — Domain has zero external dependencies, Application depends only on Domain, Infrastructure implements Application interfaces.

---

## RAG Pipeline

```
Question
  → Check Redis cache
  → Embed question (Azure OpenAI)
  → Hybrid search (Azure AI Search: vector + semantic)
  → Retrieve top-5 chunks
  → Build grounded prompt
  → Generate answer (gpt-4o)
  → Cache result + return answer with sources
```

If the documents don't contain enough information, the system returns a "not found" response instead of hallucinating.

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker Desktop
- Node.js 18+
- Azure OpenAI resource (with `text-embedding-3-small` and `gpt-4o` deployments)
- Azure AI Search resource

### 1. Clone the repo

```bash
git clone https://github.com/your-username/ai-travel-assistant.git
cd ai-travel-assistant
```

### 2. Start infrastructure

```bash
docker compose up -d
```

Starts PostgreSQL (port 5432) and Redis (port 6379).

### 3. Configure the backend

Create `src/AiTravelAssistant.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=aitravelassistant;Username=postgres;Password=postgres"
  },
  "AzureOpenAi": {
    "Endpoint": "YOUR_AZURE_OPENAI_ENDPOINT",
    "ApiKey": "YOUR_AZURE_OPENAI_KEY",
    "DeploymentEmbedding": "text-embedding-3-small",
    "DeploymentChat": "gpt-4o"
  },
  "AzureSearch": {
    "Endpoint": "YOUR_AZURE_SEARCH_ENDPOINT",
    "ApiKey": "YOUR_AZURE_SEARCH_KEY",
    "IndexName": "travel-documents",
    "SemanticConfigurationName": "travel-semantic-config"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Storage": {
    "BasePath": "./uploads"
  }
}
```

### 4. Create the Azure AI Search index

```bash
cd test-documents
python3 create-index.py
```

### 5. Run database migrations

```bash
dotnet ef database update --project src/AiTravelAssistant.Infrastructure --startup-project src/AiTravelAssistant.API
```

### 6. Start the backend

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/AiTravelAssistant.API --no-launch-profile
```

API available at `http://localhost:5168`  
Swagger UI at `http://localhost:5168/swagger`  
Hangfire Dashboard at `http://localhost:5168/hangfire`

### 7. Start the frontend

```bash
cd client/ai-travel-assistant-ui
npm install
npm run dev
```

Frontend available at `http://localhost:5173`

---

## API Endpoints

```
POST   /api/documents/upload          Upload a document
GET    /api/documents                 List all documents
GET    /api/documents/{id}/status     Get processing status
POST   /api/documents/{id}/retry      Retry failed document

POST   /api/qa/ask                    Ask a question

GET    /health                        Health check
```

---

## Key Design Decisions

**Result\<T\> pattern** — handlers never throw for expected failures. Every handler returns `Result<T>` which the controller maps to the appropriate HTTP status code.

**CQRS** — commands (write) and queries (read) are strictly separated. No handler does both.

**Grounding rule** — if the top search result has a relevance score below the threshold, the system returns "not found" without calling gpt-4o. No hallucinations.

**Redis caching** — identical questions (after normalization) hit the cache instead of Azure OpenAI. Saves cost and drops response time from ~2s to ~50ms.

**Chunking with overlap** — documents are split into 2000-character chunks with 200-character overlap. Overlap prevents context loss at chunk boundaries.

---

## Portfolio Context

This project demonstrates:

| Skill | Implementation |
|---|---|
| Clean Architecture | 4-layer separation with strict dependency inversion |
| RAG Pipeline | Full document pipeline: chunking → embedding → vector search → grounded generation |
| Azure AI Integration | Azure OpenAI + Azure AI Search, not a wrapper |
| Async Processing | Hangfire background jobs with retry logic and status tracking |
| Caching Strategy | Redis with SHA256-keyed Q&A response cache |
| API Design | Consistent envelope, typed error codes, trace IDs, health checks |
| Vue 3 Frontend | Composition API, Pinia, reactive auto-refresh, PrimeVue |
