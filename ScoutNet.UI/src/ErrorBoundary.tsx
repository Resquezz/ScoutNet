import { Component, type ErrorInfo, type ReactNode } from 'react'

type Props = {
  children: ReactNode
}

type State = {
  error?: Error
}

export class ErrorBoundary extends Component<Props, State> {
  state: State = {}

  static getDerivedStateFromError(error: Error) {
    return { error }
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error(error, errorInfo)
  }

  render() {
    if (this.state.error) {
      return (
        <main className="min-h-screen bg-slate-950 p-6 text-slate-100">
          <div className="mx-auto max-w-3xl rounded-2xl border border-red-400/30 bg-red-500/10 p-6">
            <p className="text-sm font-semibold uppercase tracking-[0.3em] text-red-300">ScoutNet crashed</p>
            <h1 className="mt-3 text-2xl font-bold">Frontend runtime error</h1>
            <pre className="mt-4 overflow-auto rounded-xl bg-black/40 p-4 text-sm text-red-100">
              {this.state.error.message}
            </pre>
          </div>
        </main>
      )
    }

    return this.props.children
  }
}
