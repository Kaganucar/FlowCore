import { useState, useEffect } from "react"
import ProductList from "./ProductList"

function App() {
    const [products, setProducts] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)
    const [searchText, setSearchText] = useState('')

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

    const filteredProducts = products.filter((product) =>
        product.name.toLowerCase().includes(searchText.toLowerCase())
    )

    return(
        <div>
            <h1>FlowCore Urunler</h1>
            <input type="text"
            placeholder="Urun ara..."
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)} 
            />
            <ProductList products={filteredProducts} />
        </div>
    )
}


export default App