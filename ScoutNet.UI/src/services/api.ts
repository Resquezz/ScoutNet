import axios from 'axios'
import type {
  AuthResponse,
  Player,
  PlayerComparison,
  PlayerDetails,
  PlayerFilters,
  ReportForm,
  Role,
  ScoutReport,
  CountryLeagues,
  TeamOption,
  UserProfile,
  WatchlistItem,
} from '../types'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5000/api',
})

export function setAuthToken(token?: string) {
  if (token) {
    api.defaults.headers.common.Authorization = `Bearer ${token}`
  } else {
    delete api.defaults.headers.common.Authorization
  }
}

function cleanParams(filters: PlayerFilters) {
  const params: Record<string, string | number> = {
    leagueId: filters.leagueId,
    season: filters.season,
  }

  Object.entries(filters).forEach(([key, value]) => {
    if (key === 'leagueId' || key === 'season') {
      return
    }

    if (value !== '') {
      params[key] = value
    }
  })

  return params
}

export const scoutNetApi = {
  async login(email: string, password: string) {
    const { data } = await api.post<AuthResponse>('/auth/login', { email, password })
    return data
  },

  async register(username: string, email: string, password: string) {
    const { data } = await api.post<AuthResponse>('/auth/register', { username, email, password })
    return data
  },

  async me() {
    const { data } = await api.get<UserProfile>('/auth/me')
    return data
  },

  async players(filters: PlayerFilters) {
    const { data } = await api.get<Player[]>('/players', { params: cleanParams(filters) })
    return data
  },

  async player(id: number) {
    const { data } = await api.get<PlayerDetails>(`/players/${id}`)
    return data
  },

  async compare(id1: number, id2: number, season: number) {
    const { data } = await api.get<PlayerComparison>('/players/compare', {
      params: { id1, id2, season },
    })
    return data
  },

  async countryLeagues() {
    const { data } = await api.get<CountryLeagues[]>('/reference/countries-leagues')
    return data
  },

  async teams(leagueId: number) {
    const { data } = await api.get<TeamOption[]>('/reference/teams', { params: { leagueId } })
    return data
  },

  async reports(playerId?: number) {
    const { data } = await api.get<ScoutReport[]>('/reports', {
      params: playerId ? { playerId } : undefined,
    })
    return data
  },

  async createReport(form: ReportForm) {
    const { data } = await api.post<ScoutReport>('/reports', form)
    return data
  },

  async updateReport(id: string, form: Omit<ReportForm, 'playerId'>) {
    const { data } = await api.put<ScoutReport>(`/reports/${id}`, form)
    return data
  },

  async deleteReport(id: string) {
    await api.delete(`/reports/${id}`)
  },

  async watchlist() {
    const { data } = await api.get<WatchlistItem[]>('/watchlist')
    return data
  },

  async addToWatchlist(playerId: number) {
    await api.post(`/watchlist/${playerId}`)
  },

  async removeFromWatchlist(playerId: number) {
    await api.delete(`/watchlist/${playerId}`)
  },

  async users() {
    const { data } = await api.get<UserProfile[]>('/admin/users')
    return data
  },

  async updateUserRole(userId: string, role: Role) {
    const { data } = await api.patch<UserProfile>(`/admin/users/${userId}/role`, { role })
    return data
  },
}
