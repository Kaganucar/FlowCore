import { useState } from "react"
import { useAuth } from "../AuthContext"
import { data, useNavigate } from "react-router-dom"

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
    <div>
      <h2>Giriş Yap</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <input 
            type="email" 
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            />
        </div>
        <div>
          <input 
            type="password" 
            placeholder="Sifre"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            />
        </div>
        {error && <p style={{color: 'red'}}>{error}</p>}
        <button type="submit">Giriş Yap</button>
      </form>
    </div>
  )
}

export default LoginPage