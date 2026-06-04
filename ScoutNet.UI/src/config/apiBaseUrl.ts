const LOCAL_API_BASE_URL = 'http://localhost:5000/api'

function normalizeApiBaseUrl(url: string): string {
  const trimmed = url.trim().replace(/\/$/, '')
  return trimmed.endsWith('/api') ? trimmed : `${trimmed}/api`
}

async function loadRuntimeConfig(): Promise<string | undefined> {
  try {
    const response = await fetch('/config.json', { cache: 'no-store' })
    if (!response.ok) {
      return undefined
    }

    const config = (await response.json()) as { apiBaseUrl?: string }
    const apiBaseUrl = config.apiBaseUrl?.trim()
    return apiBaseUrl ? normalizeApiBaseUrl(apiBaseUrl) : undefined
  } catch {
    return undefined
  }
}

export async function resolveApiBaseUrl(): Promise<string> {
  const viteUrl = import.meta.env.VITE_API_URL?.trim()
  if (viteUrl) {
    return normalizeApiBaseUrl(viteUrl)
  }

  if (import.meta.env.PROD) {
    const runtimeUrl = await loadRuntimeConfig()
    if (runtimeUrl) {
      return runtimeUrl
    }

    throw new Error(
      'API URL is not configured. Set VITE_API_URL when building the UI, or deploy public/config.json with apiBaseUrl.',
    )
  }

  return LOCAL_API_BASE_URL
}
