import { useState } from "react"
import { useAuth } from "../AuthContext"
import { useNavigate } from "react-router-dom"

function LoginPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState(null)
  const {login} = useAuth()
  const navigate = useNavigate()

  function handleSubmit(e) {
    e.preventDefault()
    setError(null)

    fetch(`${import.meta.env.VITE_API_URL}/Auth/login`, {
      method: 'POST',
      headers: {'Content-Type': 'application/json'},
      body: JSON.stringify({email,password}),
    })
    .then((response) => {
      if(!response.ok) {
        return response.json().then((data) => {
          throw new Error(data.error || 'Giriş Başarısız')
        })
      }
      return response.json()
    })
    .then((data) => {
      login(data)
      navigate('/')
    })
    .catch((err) => setError(err.message))
  }

  return (
    <div className="mx-auto mt-16 max-w-sm px-4">
      <div className="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
        <h2 className="text-xl font-bold text-slate-800">Giris Yap</h2>
        <form onSubmit={handleSubmit} className="mt-4 flex flex-col gap-3">
          <input
            type="email"
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
          />
          <input
            type="password"
            placeholder="Sifre"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
          />
          {error && <p className="text-sm text-red-600">{error}</p>}
          <button
            type="submit"
            className="mt-2 rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700"
          >
            Giris Yap
          </button>
        </form>
      </div>
    </div>
  )
}

export default LoginPage