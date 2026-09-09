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
    <div>
      <h2>Kayıt Ol</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <input 
            type="text"
            placeholder="Kullanıcı adı"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
           />
        </div>
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
        <button type="submit">Kayit Ol</button>
      </form>
    </div>
  )
}

export default RegisterPage