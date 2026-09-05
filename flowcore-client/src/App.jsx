import { useState, useEffect, use } from "react"
import ProductList from "./ProductList"

function App() {
    const [products, setProducts] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)
    const [searchText, setSearchText] = useState('')
    const [categories, setCategories] = useState([])
    const [selectedCategory, setSelectedCategory] = useState('')

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

    useEffect(() => {
        fetch(`${import.meta.env.VITE_API_URL}/Category`)
        .then((response) => response.json())
        .then((data) => setCategories(data))
        .catch((err) => console.error(err))
    }, [])

    if(loading) return <p>Yukleniyor...</p>
    if(error) return <p>Hata: {error}</p>

    const filteredProducts = products.filter((product) =>{
        const matchesSearch = product.name.toLowerCase().includes(searchText.toLowerCase())
        const matchesCategory = selectedCategory === ''|| product.categoryName === selectedCategory
        return matchesSearch && matchesCategory
    })

    return(
        <div>
            <h1>FlowCore Urunler</h1>
            <input type="text"
            placeholder="Urun ara..."
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)} 
            />
            <select
                value={selectedCategory}
                onChange={(e) => setSelectedCategory(e.target.value)}
            >
                <option value="">Tum kategoriler</option>
                {categories.map((category) => (
                    <option key={category.id} value={category.name}>
                        {category.name}
                    </option>
                ))}
            </select>
            <ProductList products={filteredProducts} />
        </div>
    )
}


export default App