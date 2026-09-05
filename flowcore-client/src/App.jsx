import { Routes, Route } from 'react-router-dom'
import Navbar from './Navbar'
import ProductsPage from './pages/ProductsPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'

function App() {
    return(
        <div>
           <Navbar />
           <Routes>
            <Route path="/" element={<ProductsPage />} />
            <Route path="/login" element = {<LoginPage />} />
            <Route path="/register" element = {<RegisterPage />} />
           </Routes>
        </div>
    )
}

export default App