import { useState, useEffect } from "react"
import ProductList from "./ProductList"

function App() {
    const [products, setProducts] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)

    useEffect(() => {
        fetch(`${import.meta.env.VITE_API_URL}/Product`)
        .then((response) => {
            if(!response.ok){
                throw new Error('Urunler alinamadi')
            }
            return response.json()
        })
        .then((data) =>{
            setProducts(data)
            setLoading(false)
        })
        .catch((err) => {
            setError(err.message)
            setLoading(false)
        })
    }, [])

    if(loading) return <p>Yukleniyor...</p>
    if(error) return <p>Hata: {error}</p>

    return(
        <div>
            <h1>FlowCore Urunler</h1>
            <ProductList products={products} />
        </div>
    )
}


export default App