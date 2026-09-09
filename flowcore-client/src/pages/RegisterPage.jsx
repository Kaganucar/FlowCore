import { useState } from "react"
import { useAuth } from "../AuthContext"
import { useNavigate } from "react-router-dom"


function RegisterPage() {
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState(null)
  const {login} = useAuth()
  const navigate = useNavigate()

  function handleSubmit(e) {
    e.preventDefault()
    setError(null)

    fetch(`${import.meta.env.VITE_API_URL}/Auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json'},
      body: JSON.stringify({ username, email, password}),
    })
      .then((response) => {
        if(!response.ok) {
          return response.json().then((data) =>{
            throw new Error(data.error || 'Kayıt başarısız')  
          })
        }
        return response.json()
      })
      .then((data) => {
        login(data)
        navigate('/')
      })
      .catch((error) => setError(error.message))
  }

  return(
    <div className="mx-auto mt-16 max-w-sm px-4">
      <div className="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
        <h2 className="text-xl font-bold text-slate-800">Kayit Ol</h2>
        <form onSubmit={handleSubmit} className="mt-4 flex flex-col gap-3">
          <input
            type="text"
            placeholder="Kullanici adi"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            className="rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
          />
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
            Kayit Ol
          </button>
        </form>
      </div>
    </div>
  )
}

export default RegisterPage