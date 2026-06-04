export type Role = 'Guest' | 'Scout' | 'Admin'

export type AuthResponse = {
  token: string
  userId: string
  username: string
  email: string
  role: Role
}

export type UserProfile = {
  userId: string
  username: string
  email: string
  role: Role
}

export type Team = {
  externalId: number
  name: string
  logo?: string | null
}

export type League = {
  externalId: number
  name: string
  country?: string | null
  logo?: string | null
  flag?: string | null
}

export type CountryLeagues = {
  country: string
  flag?: string | null
  leagues: League[]
}

export type TeamOption = {
  externalId: number
  name: string
  logo?: string | null
}

export type Player = {
  id: number
  name: string
  firstname?: string | null
  lastname?: string | null
  age?: number | null
  birthDate?: string | null
  birthPlace?: string | null
  birthCountry?: string | null
  nationality: string
  height?: string | null
  weight?: string | null
  injured: boolean
  photoUrl?: string | null
  currentClub: string
  position: number
  team: Team
  league: League
}

export type PlayerStatistics = {
  id: string
  season: string
  seasonYear: number
  lineups?: number | null
  minutes?: number | null
  shirtNumber?: number | null
  position?: string | null
  rating?: number | null
  captain?: boolean
  substitutesIn?: number | null
  substitutesOut?: number | null
  substitutesBench?: number | null
  shotsTotal?: number | null
  shotsOn?: number | null
  appearances?: number | null
  goalsTotal?: number | null
  goalsConceded?: number | null
  assists?: number | null
  saves?: number | null
  passesTotal?: number | null
  keyPasses?: number | null
  passAccuracy?: number | null
  blocks?: number | null
  tacklesTotal?: number | null
  interceptions?: number | null
  duelsTotal?: number | null
  duelsWon?: number | null
  dribblesAttempts?: number | null
  dribblesSuccess?: number | null
  dribblesPast?: number | null
  foulsDrawn?: number | null
  foulsCommitted?: number | null
  yellowCards?: number | null
  redCards?: number | null
  penaltyWon?: number | null
  penaltyCommitted?: number | null
  penaltyScored?: number | null
  penaltyMissed?: number | null
  penaltySaved?: number | null
}

export type PlayerDetails = Player & {
  statistics: PlayerStatistics[]
}

export type PlayerSkills = {
  pace: number
  shooting: number
  passing: number
  dribbling: number
  defending: number
  physicality: number
}

export type PlayerComparison = {
  seasonYear: number
  players: Array<{
    profile: Player
    seasonStatistics?: PlayerStatistics | null
    skills: PlayerSkills
  }>
  metricsDelta: Record<string, number | null>
}

export type ScoutReport = {
  id: string
  scoutId: string
  playerId: string
  playerExternalId: number
  playerName: string
  scoutUsername: string
  currentForm: number
  potential: number
  pros: string
  cons: string
  summary: string
  createdAt: string
}

export type ReportForm = {
  playerId: number
  currentForm: number
  potential: number
  pros: string
  cons: string
  summary: string
}

export type WatchlistItem = {
  playerId: number
  player: Player
}

export type PlayerFilters = {
  leagueId: number
  teamId: string
  season: number
  searchTerm: string
  minAge: string
  maxAge: string
  position: string
  nationality: string
  minAppearances: string
  minGoals: string
  minAssists: string
  minPassAccuracy: string
  minTackles: string
  minInterceptions: string
}
