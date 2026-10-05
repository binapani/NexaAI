# NexaAI 🚀

**A Multi-Provider AI Application — Local LLMs, AWS Bedrock & Azure OpenAI**

NexaAI is a full-stack AI application designed to integrate local and cloud-based large language models (LLMs) through a unified architecture. The project focuses on practical AI engineering, provider abstraction, cloud deployment, and production-ready application design.

## 🎯 Vision

Build an extensible AI platform that can switch between different model providers without requiring major changes to the frontend or application workflow.

## 🏗️ Architecture

```text
Angular Frontend
       |
       v
ASP.NET Core Web API
       |
       v
Python FastAPI AI Service
       |
       +------ Ollama (Local LLM)
       |
       +------ Amazon Bedrock (Planned)
       |
       +------ Azure OpenAI (Planned)
```

The frontend communicates with the ASP.NET Core API. The .NET backend forwards chat requests to the Python AI service, which integrates with the selected model provider.

## 🛠️ Technology Stack

| Component | Technology |
|---|---|
| Frontend | Angular |
| Backend API | ASP.NET Core / .NET 8 |
| AI Service | Python, FastAPI |
| Local Model Runtime | Ollama |
| Local Language Model | Qwen 2.5 3B |
| AWS AI Integration | Amazon Bedrock — planned |
| Azure AI Integration | Azure OpenAI — planned |
| Cloud Deployment | AWS — planned |

## ✅ Current Capabilities

- Angular chat interface
- ASP.NET Core Web API integration
- Python FastAPI AI service
- Local language model inference through Ollama
- Qwen 2.5 3B integration
- End-to-end chat request and response flow

## 🗺️ Development Roadmap

### Phase 1 — Core Application
- [x] Angular chat interface
- [x] ASP.NET Core API
- [x] Python FastAPI service
- [x] Local Ollama integration
- [ ] Improved conversation management
- [ ] Automated tests and robust error handling

### Phase 2 — Multi-Provider AI
- [ ] Define a common AI provider interface
- [ ] Refactor Ollama behind the provider interface
- [ ] Integrate Amazon Bedrock
- [ ] Integrate Azure OpenAI
- [ ] Add provider selection to the Angular UI
- [ ] Normalize responses across providers

### Phase 3 — Production Engineering
- [ ] Secure authentication and authorization
- [ ] Configuration and secret management
- [ ] Logging, monitoring, and tracing
- [ ] Model evaluation and response quality testing
- [ ] Rate limiting and cost controls
- [ ] Conversation persistence

### Phase 4 — AWS Deployment
- [ ] Select suitable AWS hosting services
- [ ] Deploy the application services
- [ ] Configure secure service communication
- [ ] Add health checks and monitoring
- [ ] Validate cloud costs and performance

### Phase 5 — Real-World Problem Solving
- [ ] Identify and validate a real user problem
- [ ] Build a solution around the validated use case
- [ ] Test with representative users and scenarios
- [ ] Measure reliability, usefulness, and performance

## 💻 Local Development

### Prerequisites

- .NET 8 SDK
- Node.js and npm
- Angular CLI
- Python and a virtual environment
- Ollama
- Qwen 2.5 3B model

### 1. Start Ollama

Ensure Ollama is installed and running, then download the model if needed:

```bash
ollama pull qwen2.5:3b
```

### 2. Start the Python AI Service

```powershell
cd ai-service
.\.venv\Scripts\python.exe -m uvicorn main:app --reload --host 127.0.0.1 --port 8000
```

Python API documentation:

`http://127.0.0.1:8000/docs`

Health endpoint:

`http://127.0.0.1:8000/health`

### 3. Start the ASP.NET Core API

Open a separate terminal:

```powershell
cd backend/NexaAI.Api
dotnet restore
dotnet run
```

Use the API URL and port printed by the application.

### 4. Start the Angular Frontend

Open another terminal:

```powershell
cd frontend/nexa-ai-ui
npm install
npm start
```

Open `http://localhost:4200` in your browser.

Ensure the frontend API URL and backend-to-Python service URL match your local configuration.

## 🔐 Security and Configuration

- Never commit API keys, cloud credentials, passwords, or `.env` files.
- Use environment variables or appropriate secret-management services.
- Use AWS IAM roles or temporary credentials where appropriate.
- Use secure Microsoft Entra ID authentication and supported Azure credentials for Azure integrations.
- Restrict service access and validate user input.
- Review cloud pricing and model availability before enabling paid inference.

## 🧪 Engineering Principles

- Keep model-provider integrations replaceable.
- Separate application logic from model-specific code.
- Validate inputs and handle failures explicitly.
- Test each service independently and then test the complete request flow.
- Measure latency, reliability, response quality, and cost.
- Add AI capabilities only when they solve a validated user problem.

## 📌 Project Status

**Status:** Active development

The local Ollama-based chat flow is working. Amazon Bedrock, Azure OpenAI, multi-provider selection, and AWS deployment are planned development milestones.

## 🌱 Long-Term Goal

Evolve NexaAI from a working AI chat application into a reliable, extensible platform for solving real-world problems with AI.

---

Built incrementally through hands-on AI engineering and full-stack development.