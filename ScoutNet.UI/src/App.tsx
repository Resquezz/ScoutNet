import { useEffect, useMemo, useState } from 'react'
import { scoutNetApi, setAuthToken } from './services/api'
import type {
  AuthResponse,
  CountryLeagues,
  League,
  Player,
  PlayerComparison,
  PlayerDetails,
  PlayerFilters,
  ReportForm,
  Role,
  ScoutReport,
  TeamOption,
  UserProfile,
  WatchlistItem,
} from './types'

const defaultFilters: PlayerFilters = {
  leagueId: 39,
  teamId: '',
  season: 2024,
  searchTerm: '',
  minAge: '',
  maxAge: '',
  position: '',
  nationality: '',
  minAppearances: '',
  minGoals: '',
  minAssists: '',
  minPassAccuracy: '',
  minTackles: '',
  minInterceptions: '',
}

const emptyReportForm: ReportForm = {
  playerId: 0,
  currentForm: 7,
  potential: 7,
  pros: '',
  cons: '',
  summary: '',
}

const positions = [
  ['Goalkeeper', 0],
  ['Defender', 10],
  ['Midfielder', 22],
  ['Attacker', 35],
] as const

function App() {
  const [auth, setAuth] = useState<AuthResponse | null>(() => {
    try {
      const stored = localStorage.getItem('scoutnet.auth')
      return stored ? (JSON.parse(stored) as AuthResponse) : null
    } catch {
      localStorage.removeItem('scoutnet.auth')
      return null
    }
  })
  const [activeTab, setActiveTab] = useState('search')
  const [filters, setFilters] = useState<PlayerFilters>(defaultFilters)
  const [players, setPlayers] = useState<Player[]>([])
  const [selectedPlayer, setSelectedPlayer] = useState<PlayerDetails | null>(null)
  const [selectedPlayerSeason, setSelectedPlayerSeason] = useState<number | null>(null)
  const [isPlayerCardOpen, setIsPlayerCardOpen] = useState(false)
  const [compareIds, setCompareIds] = useState<[number | '', number | '']>(['', ''])
  const [comparison, setComparison] = useState<PlayerComparison | null>(null)
  const [reports, setReports] = useState<ScoutReport[]>([])
  const [editingReport, setEditingReport] = useState<ScoutReport | null>(null)
  const [reportForm, setReportForm] = useState<ReportForm>(emptyReportForm)
  const [watchlist, setWatchlist] = useState<WatchlistItem[]>([])
  const [users, setUsers] = useState<UserProfile[]>([])
  const [countriesLeagues, setCountriesLeagues] = useState<CountryLeagues[]>([])
  const [teamOptions, setTeamOptions] = useState<TeamOption[]>([])
  const [selectedCountry, setSelectedCountry] = useState<string>('')
  const [filtersPanelHeight, setFiltersPanelHeight] = useState<number>(0)
  const [loading, setLoading] = useState(false)
  const [message, setMessage] = useState('')

  const isScout = auth?.role === 'Scout' || auth?.role === 'Admin'
  const isAdmin = auth?.role === 'Admin'

  const availableLeagues = useMemo(
    () => countriesLeagues.find((entry) => entry.country === selectedCountry)?.leagues ?? [],
    [countriesLeagues, selectedCountry],
  )

  const sortedPlayers = useMemo(
    () => [...players].sort((left, right) => left.name.localeCompare(right.name)),
    [players],
  )

  useEffect(() => {
    setAuthToken(auth?.token)
    if (auth) {
      localStorage.setItem('scoutnet.auth', JSON.stringify(auth))
    } else {
      localStorage.removeItem('scoutnet.auth')
    }
  }, [auth])

  useEffect(() => {
    void loadCountryLeagues()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  useEffect(() => {
    if (!auth?.token) {
      return
    }

    if (isScout) {
      void loadReports().catch(handleProtectedDataError)
      void loadWatchlist().catch(handleProtectedDataError)
    }
    if (isAdmin) {
      void loadUsers().catch(handleProtectedDataError)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [auth?.token])

  async function run(task: () => Promise<void>) {
    setLoading(true)
    setMessage('')
    try {
      await task()
    } catch (error) {
      setMessage(getErrorMessage(error))
    } finally {
      setLoading(false)
    }
  }

  async function loadCountryLeagues() {
    setLoading(true)
    try {
      const data = await scoutNetApi.countryLeagues()
      if (data.length === 0) {
        setCountriesLeagues([
          {
            country: 'England',
            flag: null,
            leagues: [{ externalId: 39, name: 'Premier League', country: 'England' }],
          },
        ])
        setSelectedCountry('England')
        setFilters((current) => ({ ...current, leagueId: 39, teamId: '' }))
        await loadTeams(39)
        return
      }

      setCountriesLeagues(data)
      const defaultCountry = data.find((entry) => entry.country === 'England')?.country ?? data[0].country
      setSelectedCountry(defaultCountry)

      const defaultLeague =
        data.find((entry) => entry.country === defaultCountry)?.leagues.find((league) => league.externalId === 39)
        ?? data.find((entry) => entry.country === defaultCountry)?.leagues[0]

      if (defaultLeague) {
        setFilters((current) => ({ ...current, leagueId: defaultLeague.externalId, teamId: '' }))
        await loadTeams(defaultLeague.externalId)
      }
    } catch {
      setCountriesLeagues([
        {
          country: 'England',
          flag: null,
          leagues: [{ externalId: 39, name: 'Premier League', country: 'England' }],
        },
      ])
      setSelectedCountry('England')
      setFilters((current) => ({ ...current, leagueId: 39, teamId: '' }))
      setTeamOptions([])
    } finally {
      setLoading(false)
    }
  }

  async function loadTeams(leagueId: number) {
    try {
      const teams = await scoutNetApi.teams(leagueId)
      setTeamOptions(teams)
    } catch {
      setTeamOptions([])
    }
  }

  async function loadPlayers() {
    if (!filters.leagueId) {
      setMessage('Select a country and league first.')
      return
    }

    await run(async () => {
      const requestFilters: PlayerFilters = isScout
        ? filters
        : {
            ...filters,
            searchTerm: '',
            minAge: '',
            maxAge: '',
            position: '',
            nationality: '',
            minAppearances: '',
            minGoals: '',
            minAssists: '',
            minPassAccuracy: '',
            minTackles: '',
            minInterceptions: '',
          }

      const data = await scoutNetApi.players(requestFilters)
      setPlayers(data)
    })
  }

  async function loadPlayer(id: number) {
    await run(async () => {
      const data = await scoutNetApi.player(id)
      setSelectedPlayer(data)
      setSelectedPlayerSeason(data.statistics[0]?.seasonYear ?? null)
      setIsPlayerCardOpen(true)
      setReportForm((current) => ({ ...current, playerId: id }))
    })
  }

  async function loadReports(playerId?: number) {
    if (!isScout) return
    const data = await scoutNetApi.reports(playerId)
    setReports(data)
  }

  async function loadWatchlist() {
    if (!isScout) return
    const data = await scoutNetApi.watchlist()
    setWatchlist(data)
  }

  async function loadUsers() {
    if (!isAdmin) return
    const data = await scoutNetApi.users()
    setUsers(data)
  }

  function handleProtectedDataError(error: unknown) {
    if (isUnauthorizedError(error)) {
      setAuth(null)
      setReports([])
      setWatchlist([])
      setUsers([])
      setMessage('Session expired. Please login again.')
      return
    }

    setMessage(getErrorMessage(error))
  }

  async function addToWatchlist(playerId: number) {
    await run(async () => {
      await scoutNetApi.addToWatchlist(playerId)
      await loadWatchlist()
      setMessage('Player added to watchlist.')
    })
  }

  async function removeFromWatchlist(playerId: number) {
    await run(async () => {
      await scoutNetApi.removeFromWatchlist(playerId)
      await loadWatchlist()
      setMessage('Player removed from watchlist.')
    })
  }

  async function comparePlayers() {
    if (!isScout) return
    if (!compareIds[0] || !compareIds[1]) {
      setMessage('Select two players first.')
      return
    }

    await run(async () => {
      const data = await scoutNetApi.compare(Number(compareIds[0]), Number(compareIds[1]), filters.season)
      setComparison(data)
      setActiveTab('comparison')
    })
  }

  async function saveReport() {
    await run(async () => {
      if (editingReport) {
        const updated = await scoutNetApi.updateReport(editingReport.id, {
          currentForm: reportForm.currentForm,
          potential: reportForm.potential,
          pros: reportForm.pros,
          cons: reportForm.cons,
          summary: reportForm.summary,
        })
        setReports((current) => current.map((report) => (report.id === updated.id ? updated : report)))
        setEditingReport(null)
      } else {
        const created = await scoutNetApi.createReport(reportForm)
        setReports((current) => [created, ...current])
      }
      setReportForm(emptyReportForm)
      setMessage('Report saved.')
    })
  }

  async function deleteReport(id: string) {
    await run(async () => {
      await scoutNetApi.deleteReport(id)
      setReports((current) => current.filter((report) => report.id !== id))
      setMessage('Report deleted.')
    })
  }

  function setCompareSlot(playerId: number) {
    setCompareIds(([first, second]) => (first === '' ? [playerId, second] : [first, playerId]))
  }

  function openReportFor(player: Player) {
    setReportForm({ ...emptyReportForm, playerId: player.id })
    setEditingReport(null)
    setActiveTab('reports')
  }

  function editReport(report: ScoutReport) {
    setEditingReport(report)
    setReportForm({
      playerId: report.playerExternalId,
      currentForm: report.currentForm,
      potential: report.potential,
      pros: report.pros,
      cons: report.cons,
      summary: report.summary,
    })
  }

  const tabs = ['search', ...(isScout ? ['comparison', 'reports', 'watchlist'] : []), ...(isAdmin ? ['admin'] : []), 'about']

  return (
    <main className="min-h-screen bg-slate-950 text-slate-100">
      <header className="border-b border-white/10 bg-slate-950/90">
        <div className="mx-auto flex max-w-7xl flex-col gap-6 px-4 py-6 lg:flex-row lg:items-center lg:justify-between">
          <div>
            <p className="text-sm font-semibold uppercase tracking-[0.3em] text-sky-400">ScoutNet</p>
            <h1 className="mt-2 text-3xl font-bold">Football scouting workspace</h1>
          </div>
          <AuthPanel auth={auth} onAuth={setAuth} />
        </div>
      </header>

      <section className="mx-auto max-w-7xl px-4 py-6">
        <div className="mb-6 flex flex-wrap gap-2">
          {tabs.map((tab) => (
            <button key={tab} className={activeTab === tab ? 'tab-active' : 'tab'} onClick={() => setActiveTab(tab)}>
              {tab}
            </button>
          ))}
        </div>

        {message && <div className="mb-4 rounded-xl border border-amber-400/30 bg-amber-400/10 p-3 text-sm text-amber-100">{message}</div>}
        {loading && <div className="mb-4 rounded-xl border border-sky-400/30 bg-sky-400/10 p-3 text-sm text-sky-100">Loading...</div>}

        {activeTab === 'search' && (
          <div className="grid gap-6 lg:grid-cols-[360px_1fr]">
            <SearchPanel
              isScout={isScout}
              filters={filters}
              setFilters={setFilters}
              onSearch={loadPlayers}
              countriesLeagues={countriesLeagues}
              selectedCountry={selectedCountry}
              setSelectedCountry={setSelectedCountry}
              availableLeagues={availableLeagues}
              teamOptions={teamOptions}
              onLeagueChange={async (leagueId) => {
                setFilters((current) => ({ ...current, leagueId, teamId: '' }))
                await loadTeams(leagueId)
              }}
              onHeightChange={setFiltersPanelHeight}
            />
            <section
              className="card flex min-h-0 flex-col overflow-hidden"
              style={filtersPanelHeight > 0 ? { height: `${filtersPanelHeight}px` } : undefined}
            >
              <div className="flex flex-col gap-3 border-b border-white/10 p-4 lg:flex-row lg:items-center lg:justify-between">
                <div>
                  <h2 className="section-title">Player Database</h2>
                  <p className="text-sm text-slate-400">{players.length} players loaded</p>
                </div>
                {isScout && (
                  <div className="flex flex-wrap items-center gap-2 text-sm">
                    <span className="text-slate-400">Compare:</span>
                    <input
                      className="input w-24 min-w-0"
                      value={compareIds[0]}
                      onChange={(event) => setCompareIds([Number(event.target.value) || '', compareIds[1]])}
                      placeholder="ID 1"
                    />
                    <input
                      className="input w-24 min-w-0"
                      value={compareIds[1]}
                      onChange={(event) => setCompareIds([compareIds[0], Number(event.target.value) || ''])}
                      placeholder="ID 2"
                    />
                    <button className="btn-primary" onClick={comparePlayers}>
                      Radar
                    </button>
                  </div>
                )}
              </div>
              <PlayerTable
                players={sortedPlayers}
                isScout={isScout}
                onSelect={loadPlayer}
                onCompare={setCompareSlot}
                onWatch={addToWatchlist}
                onReport={openReportFor}
              />
            </section>
          </div>
        )}

        {activeTab === 'comparison' && isScout && <ComparisonPanel comparison={comparison} />}

        {activeTab === 'reports' && isScout && (
          <ReportsPanel
            isScout={isScout}
            selectedPlayer={selectedPlayer}
            form={reportForm}
            setForm={setReportForm}
            reports={reports}
            editingReport={editingReport}
            onSave={saveReport}
            onEdit={editReport}
            onDelete={deleteReport}
            onCancelEdit={() => {
              setEditingReport(null)
              setReportForm(emptyReportForm)
            }}
          />
        )}

        {activeTab === 'watchlist' && isScout && (
          <WatchlistPanel
            isScout={isScout}
            items={watchlist}
            onRemove={removeFromWatchlist}
            onReport={openReportFor}
            onOpenPlayer={loadPlayer}
          />
        )}

        {activeTab === 'admin' && isAdmin && (
          <AdminPanel
            users={users}
            onRoleChange={async (id, role) => {
              await scoutNetApi.updateUserRole(id, role)
              await loadUsers()
            }}
          />
        )}

        {activeTab === 'about' && <AboutPanel />}
      </section>

      <PlayerCardModal
        isOpen={isPlayerCardOpen}
        player={selectedPlayer}
        selectedSeason={selectedPlayerSeason}
        onSeasonChange={setSelectedPlayerSeason}
        onClose={() => setIsPlayerCardOpen(false)}
      />
    </main>
  )
}

function AuthPanel({ auth, onAuth }: { auth: AuthResponse | null; onAuth: (auth: AuthResponse | null) => void }) {
  const [mode, setMode] = useState<'login' | 'register'>('login')
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [error, setError] = useState('')

  if (auth) {
    return (
      <div className="card min-w-72 p-4">
        <p className="text-sm text-slate-400">Signed in as</p>
        <p className="font-semibold">{auth.username}</p>
        <p className="text-sm text-sky-300">{auth.role}</p>
        <button className="btn-secondary mt-4 w-full" onClick={() => onAuth(null)}>
          Logout
        </button>
      </div>
    )
  }

  async function submit() {
    setError('')
    try {
      const response = mode === 'login' ? await scoutNetApi.login(email, password) : await scoutNetApi.register(username, email, password)
      onAuth(response)
    } catch (err) {
      setError(getErrorMessage(err))
    }
  }

  return (
    <div className="card w-full max-w-sm p-4">
      <div className="mb-3 flex gap-2">
        <button className={mode === 'login' ? 'tab-active' : 'tab'} onClick={() => setMode('login')}>Login</button>
        <button className={mode === 'register' ? 'tab-active' : 'tab'} onClick={() => setMode('register')}>Register</button>
      </div>
      {mode === 'register' && <input className="input mb-2 w-full" value={username} onChange={(event) => setUsername(event.target.value)} placeholder="Username" />}
      <input className="input mb-2 w-full" value={email} onChange={(event) => setEmail(event.target.value)} placeholder="Email" />
      <div className="relative mb-3">
        <input
          className="input w-full pr-12"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          placeholder="Password"
          type={showPassword ? 'text' : 'password'}
        />
        <button
          type="button"
          className="absolute right-2 top-1/2 -translate-y-1/2 rounded-lg px-2 py-1 text-xs text-slate-300 hover:bg-white/10"
          onClick={() => setShowPassword((value) => !value)}
          aria-label={showPassword ? 'Hide password' : 'Show password'}
        >
          {showPassword ? '🙈' : '👁️'}
        </button>
      </div>
      {error && <p className="mb-3 text-sm text-red-300">{error}</p>}
      <button className="btn-primary w-full" onClick={submit}>{mode === 'login' ? 'Login' : 'Create Scout Account'}</button>
    </div>
  )
}

function SearchPanel({
  isScout,
  filters,
  setFilters,
  onSearch,
  countriesLeagues,
  selectedCountry,
  setSelectedCountry,
  availableLeagues,
  teamOptions,
  onLeagueChange,
  onHeightChange,
}: {
  isScout: boolean
  filters: PlayerFilters
  setFilters: (filters: PlayerFilters) => void
  onSearch: () => void
  countriesLeagues: CountryLeagues[]
  selectedCountry: string
  setSelectedCountry: (country: string) => void
  availableLeagues: League[]
  teamOptions: TeamOption[]
  onLeagueChange: (leagueId: number) => Promise<void>
  onHeightChange: (height: number) => void
}) {
  const [panelElement, setPanelElement] = useState<HTMLElement | null>(null)

  useEffect(() => {
    if (!panelElement) return

    const observer = new ResizeObserver(() => {
      onHeightChange(panelElement.offsetHeight)
    })

    observer.observe(panelElement)
    onHeightChange(panelElement.offsetHeight)

    return () => observer.disconnect()
  }, [onHeightChange, panelElement])

  function update(key: keyof PlayerFilters, value: string | number) {
    setFilters({ ...filters, [key]: value })
  }

  const hasLeague = Boolean(filters.leagueId)
  const selectedLeague = availableLeagues.find((league) => league.externalId === filters.leagueId)

  return (
    <aside className="card p-4" ref={(node) => setPanelElement(node)}>
      <h2 className="section-title">Advanced Search</h2>

      <div className="mb-4">
        <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-slate-400">Country</p>
        <div className="grid grid-cols-3 gap-2">
          {countriesLeagues.map((entry) => (
            <button
              key={entry.country}
              className={selectedCountry === entry.country ? 'tab-active w-full' : 'tab w-full'}
              onClick={() => {
                setSelectedCountry(entry.country)
                const firstLeague = entry.leagues[0]
                if (firstLeague) {
                  void onLeagueChange(firstLeague.externalId)
                }
              }}
              title={entry.country}
            >
              <span className="flex items-center justify-center">
                {entry.flag ? <img src={entry.flag} alt={entry.country} className="h-6 w-9 rounded object-cover" /> : <span>{entry.country}</span>}
              </span>
            </button>
          ))}
        </div>
      </div>

      <div className="mb-4">
        <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-slate-400">League</p>
        <div className="grid grid-cols-2 gap-2">
          {availableLeagues.map((league) => {
            const active = filters.leagueId === league.externalId
            return (
              <button
                key={league.externalId}
                className={active ? 'tab-active w-full justify-start' : 'tab w-full justify-start'}
                onClick={() => void onLeagueChange(league.externalId)}
                title={league.name}
              >
                <span className="flex items-center gap-2">
                  {league.logo ? (
                    <img src={league.logo} alt={league.name} className="h-5 w-5 rounded object-contain bg-white" />
                  ) : (
                    <span className="text-xs">🏆</span>
                  )}
                  <span className="truncate text-xs">{league.name}</span>
                </span>
              </button>
            )
          })}
        </div>
      </div>

      {hasLeague && selectedLeague && (
        <div className="grid gap-3">
          <div>
            <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-slate-400">Club</p>
            <div className="max-h-48 space-y-2 overflow-y-auto rounded-xl border border-white/10 bg-slate-950/40 p-2">
              <button
                className={filters.teamId === '' ? 'tab-active w-full justify-start' : 'tab w-full justify-start'}
                onClick={() => update('teamId', '')}
              >
                <span className="flex items-center gap-2">
                  <span className="text-xs">🏟️</span>
                  <span className="truncate text-xs">All clubs</span>
                </span>
              </button>
              {teamOptions.map((team) => (
                <button
                  key={team.externalId}
                  className={filters.teamId === String(team.externalId) ? 'tab-active w-full justify-start' : 'tab w-full justify-start'}
                  onClick={() => update('teamId', String(team.externalId))}
                  title={team.name}
                >
                  <span className="flex items-center gap-2">
                    {team.logo ? (
                      <img src={team.logo} alt={team.name} className="h-5 w-5 rounded object-contain bg-white" />
                    ) : (
                      <span className="text-xs">🏟️</span>
                    )}
                    <span className="truncate text-xs">{team.name}</span>
                  </span>
                </button>
              ))}
            </div>
          </div>
          <label className="label">Season<input className="input w-full" type="number" value={filters.season} onChange={(event) => update('season', Number(event.target.value))} /></label>
          {isScout && (
            <>
              <label className="label">Search<input className="input w-full" value={filters.searchTerm} onChange={(event) => update('searchTerm', event.target.value)} placeholder="Name" /></label>
              <div className="grid grid-cols-2 gap-3">
                <label className="label min-w-0">Min age<input className="input w-full min-w-0" type="number" value={filters.minAge} onChange={(event) => update('minAge', event.target.value)} /></label>
                <label className="label min-w-0">Max age<input className="input w-full min-w-0" type="number" value={filters.maxAge} onChange={(event) => update('maxAge', event.target.value)} /></label>
              </div>
              <label className="label">Position
                <select className="input w-full" value={filters.position} onChange={(event) => update('position', event.target.value)}>
                  <option value="">Any</option>
                  {positions.map(([label, value]) => <option key={value} value={value}>{label}</option>)}
                </select>
              </label>
              <label className="label">Nationality<input className="input w-full" value={filters.nationality} onChange={(event) => update('nationality', event.target.value)} /></label>
              <div className="grid grid-cols-2 gap-3">
                <label className="label min-w-0">Apps<input className="input w-full min-w-0" type="number" value={filters.minAppearances} onChange={(event) => update('minAppearances', event.target.value)} /></label>
                <label className="label min-w-0">Goals<input className="input w-full min-w-0" type="number" value={filters.minGoals} onChange={(event) => update('minGoals', event.target.value)} /></label>
                <label className="label min-w-0">Assists<input className="input w-full min-w-0" type="number" value={filters.minAssists} onChange={(event) => update('minAssists', event.target.value)} /></label>
                <label className="label min-w-0">Pass %<input className="input w-full min-w-0" type="number" value={filters.minPassAccuracy} onChange={(event) => update('minPassAccuracy', event.target.value)} /></label>
                <label className="label min-w-0">Tackles<input className="input w-full min-w-0" type="number" value={filters.minTackles} onChange={(event) => update('minTackles', event.target.value)} /></label>
                <label className="label min-w-0">Intercept.<input className="input w-full min-w-0" type="number" value={filters.minInterceptions} onChange={(event) => update('minInterceptions', event.target.value)} /></label>
              </div>
            </>
          )}
          <button className="btn-primary mt-2" onClick={onSearch}>Search Players</button>
        </div>
      )}
    </aside>
  )
}

function PlayerTable({
  players,
  isScout,
  onSelect,
  onCompare,
  onWatch,
  onReport,
}: {
  players: Player[]
  isScout: boolean
  onSelect: (id: number) => void
  onCompare: (id: number) => void
  onWatch: (id: number) => void
  onReport: (player: Player) => void
}) {
  return (
    <div className="player-list-scroll min-h-0 flex-1 overflow-y-auto">
      <table className="w-full table-fixed text-left text-sm">
        <thead className="bg-slate-900 text-slate-300">
          <tr>
            <th className="px-4 py-3">Player</th>
            <th className="px-2 py-3">Age</th>
            <th className="px-2 py-3">Pos</th>
            <th className="px-2 py-3">Nation</th>
            <th className="px-2 py-3">Club</th>
            {isScout && <th className="px-2 py-3">Actions</th>}
          </tr>
        </thead>
        <tbody>
          {players.map((player) => (
            <tr key={player.id} className="border-t border-white/10 hover:bg-white/[0.03]">
              <td className="px-4 py-3">
                <button className="max-w-[180px] truncate text-left font-semibold text-sky-300 hover:text-sky-100" onClick={() => onSelect(player.id)}>
                  {player.name}
                </button>
              </td>
              <td className="px-2 py-3">{player.age ?? '-'}</td>
              <td className="px-2 py-3">{positionLabel(player.position)}</td>
              <td className="px-2 py-3 truncate">{player.nationality}</td>
              <td className="px-2 py-3 truncate">{player.currentClub}</td>
              {isScout && (
                <td className="px-2 py-3">
                  <div className="flex flex-wrap gap-1">
                    <button className="btn-secondary px-2 py-1 text-xs" onClick={() => onCompare(player.id)}>Compare</button>
                    <button className="btn-secondary px-2 py-1 text-xs" onClick={() => onWatch(player.id)}>Watch</button>
                    <button className="btn-secondary px-2 py-1 text-xs" onClick={() => onReport(player)}>Report</button>
                  </div>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function ComparisonPanel({ comparison }: { comparison: PlayerComparison | null }) {
  if (!comparison) {
    return <EmptyState title="No comparison yet" text="Pick two players from search and click Radar." />
  }

  const [first, second] = comparison.players

  return (
    <section className="grid gap-6 lg:grid-cols-[1fr_380px]">
      <div className="card p-4">
        <h2 className="section-title">Radar Comparison</h2>
        <div className="h-[520px]">
          <SkillsRadar first={first} second={second} />
        </div>
      </div>
      <div className="grid gap-4">
        {comparison.players.map((entry) => (
          <div key={entry.profile.id} className="card p-4">
            <h3 className="text-lg font-semibold">{entry.profile.name}</h3>
            <p className="text-sm text-slate-400">{entry.profile.currentClub} · {positionLabel(entry.profile.position)}</p>
            <div className="mt-4 grid grid-cols-2 gap-2 text-sm">
              <Metric label="Goals" value={entry.seasonStatistics?.goalsTotal} />
              <Metric label="Assists" value={entry.seasonStatistics?.assists} />
              <Metric label="Pass %" value={entry.seasonStatistics?.passAccuracy} />
              <Metric label="Tackles" value={entry.seasonStatistics?.tacklesTotal} />
            </div>
          </div>
        ))}
      </div>
    </section>
  )
}

function SkillsRadar({
  first,
  second,
}: {
  first: PlayerComparison['players'][number]
  second: PlayerComparison['players'][number]
}) {
  const metrics: Array<keyof typeof first.skills> = ['pace', 'shooting', 'passing', 'dribbling', 'defending', 'physicality']
  const center = 220
  const radius = 160
  const rings = [20, 40, 60, 80, 100]

  const point = (index: number, value: number) => {
    const angle = (Math.PI * 2 * index) / metrics.length - Math.PI / 2
    const distance = (Math.max(0, Math.min(100, value)) / 100) * radius
    return { x: center + Math.cos(angle) * distance, y: center + Math.sin(angle) * distance }
  }

  const polygon = (entry: PlayerComparison['players'][number]) =>
    metrics
      .map((metric, index) => {
        const { x, y } = point(index, entry.skills[metric])
        return `${x},${y}`
      })
      .join(' ')

  return (
    <div className="flex h-full flex-col items-center justify-center">
      <svg viewBox="0 0 440 440" className="h-full w-full max-w-[520px]">
        {rings.map((ring) => {
          const points = metrics.map((_, index) => {
            const { x, y } = point(index, ring)
            return `${x},${y}`
          }).join(' ')
          return <polygon key={ring} points={points} fill="none" stroke="rgba(148,163,184,.22)" />
        })}
        {metrics.map((metric, index) => {
          const outer = point(index, 100)
          const label = point(index, 115)
          return (
            <g key={metric}>
              <line x1={center} y1={center} x2={outer.x} y2={outer.y} stroke="rgba(148,163,184,.22)" />
              <text x={label.x} y={label.y} textAnchor="middle" dominantBaseline="middle" className="fill-slate-300 text-xs capitalize">
                {metric}
              </text>
            </g>
          )
        })}
        <polygon points={polygon(first)} fill="rgba(56,189,248,.32)" stroke="#38bdf8" strokeWidth="3" />
        <polygon points={polygon(second)} fill="rgba(249,115,22,.24)" stroke="#f97316" strokeWidth="3" />
      </svg>
      <div className="flex flex-wrap justify-center gap-4 text-sm">
        <span className="text-sky-300">{first.profile.name}</span>
        <span className="text-orange-300">{second.profile.name}</span>
      </div>
    </div>
  )
}

function PlayerCardModal({
  isOpen,
  player,
  selectedSeason,
  onSeasonChange,
  onClose,
}: {
  isOpen: boolean
  player: PlayerDetails | null
  selectedSeason: number | null
  onSeasonChange: (season: number) => void
  onClose: () => void
}) {
  if (!isOpen || !player) return null

  const currentStats = player.statistics.find((entry) => entry.seasonYear === selectedSeason) ?? player.statistics[0]
  const profileRows: Array<[string, string | number | null | undefined]> = [
    ['Player name', player.name],
    ['Firstname', player.firstname],
    ['Lastname', player.lastname],
    ['Age', player.age],
    ['Birth date', player.birthDate],
    ['Birth place', player.birthPlace],
    ['Birth country', player.birthCountry],
    ['Nationality', player.nationality],
    ['Height', player.height],
    ['Weight', player.weight],
    ['Injured', player.injured ? 'Yes' : 'No'],
    ['Current club', player.currentClub],
    ['Position', positionLabel(player.position)],
  ]

  const fullStatRows = currentStats
    ? [
        ['Appearances', currentStats.appearances],
        ['Lineups', currentStats.lineups],
        ['Minutes', currentStats.minutes],
        ['Position (season)', currentStats.position],
        ['Rating', currentStats.rating],
        ['Captain', currentStats.captain ? 'Yes' : 'No'],
        ['Substitutes in', currentStats.substitutesIn],
        ['Substitutes out', currentStats.substitutesOut],
        ['Substitutes bench', currentStats.substitutesBench],
        ['Shots total', currentStats.shotsTotal],
        ['Shots on', currentStats.shotsOn],
        ['Goals total', currentStats.goalsTotal],
        ['Goals conceded', currentStats.goalsConceded],
        ['Assists', currentStats.assists],
        ['Saves', currentStats.saves],
        ['Passes total', currentStats.passesTotal],
        ['Key passes', currentStats.keyPasses],
        ['Pass accuracy', currentStats.passAccuracy],
        ['Tackles total', currentStats.tacklesTotal],
        ['Blocks', currentStats.blocks],
        ['Interceptions', currentStats.interceptions],
        ['Duels total', currentStats.duelsTotal],
        ['Duels won', currentStats.duelsWon],
        ['Dribbles attempts', currentStats.dribblesAttempts],
        ['Dribbles success', currentStats.dribblesSuccess],
        ['Dribbled past', currentStats.dribblesPast],
        ['Fouls drawn', currentStats.foulsDrawn],
        ['Fouls committed', currentStats.foulsCommitted],
        ['Yellow cards', currentStats.yellowCards],
        ['Red cards', currentStats.redCards],
        ['Penalty won', currentStats.penaltyWon],
        ['Penalty committed', currentStats.penaltyCommitted],
        ['Penalty scored', currentStats.penaltyScored],
        ['Penalty missed', currentStats.penaltyMissed],
        ['Penalty saved', currentStats.penaltySaved],
      ] as Array<[string, number | string | null | undefined]>
    : []

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/80 p-4">
      <div className="card max-h-[90vh] w-full max-w-4xl overflow-auto p-6">
        <div className="mb-4 flex items-start justify-between">
          <div className="flex items-center gap-4">
            {player.photoUrl && <img src={player.photoUrl} alt={player.name} className="h-24 w-24 rounded-xl object-cover" />}
            <div>
              <h2 className="text-2xl font-bold">{player.name}</h2>
              <p className="text-slate-400">{player.currentClub} · {player.nationality} · {positionLabel(player.position)}</p>
            </div>
          </div>
          <button className="btn-secondary" onClick={onClose}>Close</button>
        </div>

        <div className="mb-4">
          <label className="label">Season
            <select
              className="input w-full max-w-xs"
              value={currentStats?.seasonYear ?? ''}
              onChange={(event) => onSeasonChange(Number(event.target.value))}
            >
              {player.statistics.map((entry) => (
                <option key={entry.id} value={entry.seasonYear}>{entry.season}</option>
              ))}
            </select>
          </label>
        </div>

        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
          <Metric label="Appearances" value={currentStats?.appearances} />
          <Metric label="Goals" value={currentStats?.goalsTotal} />
          <Metric label="Assists" value={currentStats?.assists} />
          <Metric label="Shots on" value={currentStats?.shotsOn} />
          <Metric label="Pass accuracy" value={currentStats?.passAccuracy} />
          <Metric label="Tackles" value={currentStats?.tacklesTotal} />
          <Metric label="Interceptions" value={currentStats?.interceptions} />
          <Metric label="Rating" value={currentStats?.rating ? Number(currentStats.rating) : undefined} />
        </div>

        <div className="mt-6 overflow-auto rounded-xl border border-white/10">
          <div className="border-b border-white/10 bg-white/5 px-3 py-2 text-sm font-semibold text-slate-200">
            Player Personal Information
          </div>
          <table className="w-full min-w-[640px] text-sm">
            <thead className="bg-white/5 text-slate-300">
              <tr>
                <th className="px-3 py-2 text-left">Field</th>
                <th className="px-3 py-2 text-left">Value</th>
              </tr>
            </thead>
            <tbody>
              {profileRows.map(([label, value]) => (
                <tr key={label} className="border-t border-white/10">
                  <td className="px-3 py-2 text-slate-400">{label}</td>
                  <td className="px-3 py-2">{value ?? '-'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="mt-6 overflow-auto rounded-xl border border-white/10">
          <div className="border-b border-white/10 bg-white/5 px-3 py-2 text-sm font-semibold text-slate-200">
            Player Statistics
          </div>
          <table className="w-full min-w-[640px] text-sm">
            <thead className="bg-white/5 text-slate-300">
              <tr>
                <th className="px-3 py-2 text-left">Metric</th>
                <th className="px-3 py-2 text-left">Value</th>
              </tr>
            </thead>
            <tbody>
              {fullStatRows.map(([label, value]) => (
                <tr key={label} className="border-t border-white/10">
                  <td className="px-3 py-2 text-slate-400">{label}</td>
                  <td className="px-3 py-2">{value ?? '-'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}

function ReportsPanel(props: {
  isScout: boolean
  selectedPlayer: PlayerDetails | null
  form: ReportForm
  setForm: (form: ReportForm) => void
  reports: ScoutReport[]
  editingReport: ScoutReport | null
  onSave: () => void
  onEdit: (report: ScoutReport) => void
  onDelete: (id: string) => void
  onCancelEdit: () => void
}) {
  if (!props.isScout) {
    return <EmptyState title="Reports require auth" text="Login as Scout or Admin to create and manage scout reports." />
  }

  const form = props.form
  const update = (key: keyof ReportForm, value: string | number) => props.setForm({ ...form, [key]: value })

  return (
    <section className="grid gap-6 lg:grid-cols-[420px_1fr]">
      <div className="card p-4">
        <h2 className="section-title">{props.editingReport ? 'Edit Report' : 'Create Scout Report'}</h2>
        {props.selectedPlayer && <p className="mb-3 text-sm text-slate-400">Selected: {props.selectedPlayer.name}</p>}
        <div className="grid gap-3">
          <label className="label">Player external ID<input className="input w-full" type="number" value={form.playerId} onChange={(event) => update('playerId', Number(event.target.value))} disabled={!!props.editingReport} /></label>
          <div className="grid grid-cols-2 gap-3">
            <label className="label min-w-0">Current form<input className="input w-full min-w-0" min={1} max={10} type="number" value={form.currentForm} onChange={(event) => update('currentForm', Number(event.target.value))} /></label>
            <label className="label min-w-0">Potential<input className="input w-full min-w-0" min={1} max={10} type="number" value={form.potential} onChange={(event) => update('potential', Number(event.target.value))} /></label>
          </div>
          <label className="label">Pros<textarea className="input min-h-24 w-full" value={form.pros} onChange={(event) => update('pros', event.target.value)} /></label>
          <label className="label">Cons<textarea className="input min-h-24 w-full" value={form.cons} onChange={(event) => update('cons', event.target.value)} /></label>
          <label className="label">Transfer recommendation<textarea className="input min-h-28 w-full" value={form.summary} onChange={(event) => update('summary', event.target.value)} /></label>
          <div className="flex gap-2">
            <button className="btn-primary" onClick={props.onSave}>{props.editingReport ? 'Update' : 'Create'} Report</button>
            {props.editingReport && <button className="btn-secondary" onClick={props.onCancelEdit}>Cancel</button>}
          </div>
        </div>
      </div>
      <div className="card p-4">
        <h2 className="section-title">Reports CRM</h2>
        <div className="mt-4 grid gap-3">
          {props.reports.map((report) => (
            <article key={report.id} className="rounded-xl border border-white/10 bg-white/[0.03] p-4">
              <div className="flex flex-col gap-3 lg:flex-row lg:items-start lg:justify-between">
                <div>
                  <h3 className="font-semibold">{report.playerName || `Player ${report.playerExternalId}`}</h3>
                  <p className="text-sm text-slate-400">Form {report.currentForm}/10 · Potential {report.potential}/10 · {new Date(report.createdAt).toLocaleDateString()}</p>
                </div>
                <div className="flex gap-2">
                  <button className="btn-secondary" onClick={() => props.onEdit(report)}>Edit</button>
                  <button className="btn-danger" onClick={() => props.onDelete(report.id)}>Delete</button>
                </div>
              </div>
              <p className="mt-3 text-sm text-slate-300">{report.summary}</p>
            </article>
          ))}
        </div>
      </div>
    </section>
  )
}

function WatchlistPanel({ isScout, items, onRemove, onReport }: {
  isScout: boolean
  items: WatchlistItem[]
  onRemove: (playerId: number) => void
  onReport: (player: Player) => void
  onOpenPlayer: (id: number) => void
}) {
  if (!isScout) {
    return <EmptyState title="Watchlist requires auth" text="Login as Scout or Admin to save players." />
  }

  return (
    <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
      {items.map(({ player }) => (
        <article key={player.id} className="card p-4">
          <div className="flex items-start gap-3">
            <button onClick={() => onOpenPlayer(player.id)} title="Open player card">
              {player.photoUrl && <img className="h-16 w-16 rounded-full object-cover" src={player.photoUrl} alt={player.name} />}
            </button>
            <div>
              <button className="font-semibold text-left hover:text-sky-300" onClick={() => onOpenPlayer(player.id)}>
                {player.name}
              </button>
              <p className="text-sm text-slate-400">{player.currentClub} · {player.nationality}</p>
              <p className="text-sm text-sky-300">{positionLabel(player.position)}</p>
            </div>
          </div>
          <div className="mt-4 flex gap-2">
            <button className="btn-secondary" onClick={() => onReport(player)}>Report</button>
            <button className="btn-danger" onClick={() => onRemove(player.id)}>Remove</button>
          </div>
        </article>
      ))}
    </section>
  )
}

function AboutPanel() {
  return (
    <section className="grid gap-6 lg:grid-cols-2">
      <article className="card p-5">
        <h2 className="section-title">About ScoutNet</h2>
        <p className="mt-2 text-sm text-slate-300">
          ScoutNet is a scouting workspace for comparing players, creating scout reports, and managing a personal watchlist.
        </p>
        <div className="mt-4 space-y-2 text-sm text-slate-300">
          <p><strong>Data source:</strong> API-Football + locally synced database.</p>
          <p><strong>Public access:</strong> guests can browse basic player list only.</p>
          <p><strong>Scout/Admin access:</strong> advanced filters, radar comparison, reports, watchlist.</p>
          <p><strong>Supported basic positions:</strong> Goalkeeper, Defender, Midfielder, Attacker.</p>
          <p><strong>Seasons:</strong> available data depends on your API-Football plan and imported seasons.</p>
        </div>
      </article>

      <article className="card p-5">
        <h2 className="section-title">How Comparison Stats Work</h2>
        <p className="mt-2 text-sm text-slate-300">
          Radar scores are calculated from season statistics and normalized to 1..100.
        </p>
        <div className="mt-4 space-y-2 text-sm text-slate-300">
          <p><strong>Shooting:</strong> blend of goals per game and shots on target per game.</p>
          <p><strong>Passing:</strong> pass accuracy + assists per game.</p>
          <p><strong>Defending:</strong> interceptions per game + tackles per game.</p>
          <p><strong>Pace:</strong> dribble success rate + age pace factor.</p>
          <p><strong>Physicality:</strong> tackles/interceptions + age physical factor.</p>
          <p><strong>Dribbling:</strong> direct dribble success rate.</p>
        </div>
      </article>
    </section>
  )
}

function AdminPanel({ users, onRoleChange }: { users: UserProfile[]; onRoleChange: (id: string, role: Role) => Promise<void> }) {
  return (
    <section className="card p-4">
      <h2 className="section-title">Admin Users</h2>
      <div className="mt-4 grid gap-3">
        {users.map((user) => (
          <div key={user.userId} className="flex flex-col gap-3 rounded-xl border border-white/10 bg-white/[0.03] p-4 md:flex-row md:items-center md:justify-between">
            <div>
              <p className="font-semibold">{user.username}</p>
              <p className="text-sm text-slate-400">{user.email}</p>
            </div>
            <select className="input md:w-40" value={user.role} onChange={(event) => onRoleChange(user.userId, event.target.value as Role)}>
              <option value="Scout">Scout</option>
              <option value="Admin">Admin</option>
            </select>
          </div>
        ))}
      </div>
    </section>
  )
}

function Metric({ label, value }: { label: string; value?: number | null }) {
  return (
    <div className="rounded-lg bg-white/5 p-3">
      <p className="text-xs uppercase tracking-wide text-slate-500">{label}</p>
      <p className="text-lg font-semibold">{value ?? '-'}</p>
    </div>
  )
}

function EmptyState({ title, text }: { title: string; text: string }) {
  return (
    <div className="card p-10 text-center">
      <h2 className="text-xl font-semibold">{title}</h2>
      <p className="mt-2 text-slate-400">{text}</p>
    </div>
  )
}

function positionLabel(position: number) {
  return positions.find(([, value]) => value === position)?.[0] ?? String(position)
}

function getErrorMessage(error: unknown) {
  if (typeof error === 'object' && error !== null && 'response' in error) {
    const response = (error as { response?: { status?: number; data?: { detail?: string; title?: string } } }).response
    const detail = response?.data?.detail ?? response?.data?.title ?? ''

    if (response?.status === 401) {
      return 'Сесія закінчилась або токен невалідний. Увійди ще раз.'
    }

    if (detail.includes('Free plans do not have access to this season')) {
      return 'Твій API-Football план не дає доступ до цього сезону. Обери сезон 2022-2024 або онови план.'
    }

    if (detail.startsWith('API-Football error')) {
      return `Помилка зовнішнього API: ${detail.replace('API-Football error:', '').trim()}`
    }

    return detail || 'Request failed.'
  }
  return error instanceof Error ? error.message : 'Request failed.'
}

function isUnauthorizedError(error: unknown) {
  if (typeof error !== 'object' || error === null || !('response' in error)) {
    return false
  }

  const response = (error as { response?: { status?: number } }).response
  return response?.status === 401
}

export default App
