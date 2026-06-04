import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App'
import { ErrorBoundary } from './ErrorBoundary'
import { resolveApiBaseUrl } from './config/apiBaseUrl'
import { configureApiBaseUrl } from './services/api'

async function bootstrap() {
  const rootElement = document.getElementById('root')
  if (!rootElement) {
    throw new Error('Root element #root was not found.')
  }

  try {
    configureApiBaseUrl(await resolveApiBaseUrl())
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Failed to configure API URL.'
    rootElement.innerHTML = `
      <div style="min-height:100vh;background:#020617;color:#fecaca;font-family:system-ui;padding:32px">
        <h1 style="font-size:28px;margin:0 0 12px">ScoutNet configuration error</h1>
        <pre style="white-space:pre-wrap;background:rgba(0,0,0,.35);padding:16px;border-radius:12px">${message}</pre>
      </div>`
    return
  }

  createRoot(rootElement).render(
    <StrictMode>
      <ErrorBoundary>
        <App />
      </ErrorBoundary>
    </StrictMode>,
  )
}

void bootstrap()
