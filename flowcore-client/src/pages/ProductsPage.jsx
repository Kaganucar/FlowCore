import { useEffect, useState } from "react";
import ProductList from "../ProductList";
import { data, Form } from "react-router-dom";
import { useAuth } from "../AuthContext";

function ProductPage() {
    const { user } = useAuth()
    const [products, setProducts] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)
    const [searchText, setSearchText] = useState('')
    const [categories, setCategories] = useState([])
    const [selectedCategory, setSelectedCategory] = useState('')
    const [newName, setNewName] = useState('')
    const [newDescription, setNewDescription] = useState('')
    const [newPrice, setNewPrice] = useState('')
    const [newStock, setNewStock] = useState('')
    const [newCategoryId, setNewCategoryId] = useState('')

    useEffect(() => {
        fetch(`${import.meta.env.VITE_API_URL}/Product`)
            .then((response) => {
                if (!response.ok) {
                    throw new Error('Urunler alinamadi')
                }
                return response.json()
            })
            .then((data) => {
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

    function handleCreateSubmit(e) {
        e.preventDefault()

        fetch(`${import.meta.env.VITE_API_URL}/Product`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${user.accessToken}`,
            },
            body: JSON.stringify({
                name: newName,
                description: newDescription,
                price: Number(newPrice),
                stock: Number(newStock),
                categoryId: newCategoryId,
            }),
        })
            .then((response) => {
                if (!response.ok) {
                    return response.json().then((data) => {
                        throw new Error(data.error || 'Urun Eklenmedi')
                    })
                }
                return response.json()
            })
            .then((createdProduct) => {
                setProducts((prev) => [...prev, createdProduct])
                setNewName('')
                setNewDescription('')
                setNewPrice('')
                setNewStock('')
                setNewCategoryId('')
            })
            .catch((err) => alert(err.message))
    }

    function handleDelete(productId) {
        fetch(`${import.meta.env.VITE_API_URL}/Product/${productId}`, {
            method: 'DELETE',
            headers: {
                Authorization: `Bearer ${user.accessToken}`,
            },
        })
            .then((response) => {
                if (!response.ok) {
                    throw new Error('Silme Başarısız')
                }
                setProducts((prev) => prev.filter((p) => p.id !== productId))
            })
            .cath((err) => alert(err.message))
    }

    const filteredProducts = products.filter((product) => {
        const matchesSearch = product.name.toLowerCase().includes(searchText.toLocaleLowerCase())
        const matchesCategory = selectedCategory === '' || product.categoryName === selectedCategory
        return matchesSearch && matchesCategory
    })

    if (loading) return <p>Yukleniyor...</p>
    if (error) return <p>Hata: {error}</p>

    return (
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
                <option value="">Tüm kategoriler</option>
                {categories.map((category) => (
                    <option key={category.id} value={category.name}>
                        {category.name}
                    </option>
                ))}
            </select>
            {user?.role === 'Admin' && (
                <form onSubmit={handleCreateSubmit}>
                    <h3>Yeni Urun Ekle</h3>
                    <input
                        type="text"
                        placeholder="Urun adi"
                        value={newName}
                        onChange={(e) => setNewName(e.target.value)}
                    />
                    <input
                        type="text"
                        placeholder="Aciklama"
                        value={newDescription}
                        onChange={(e) => setNewDescription(e.target.value)}
                    />
                    <input
                        type="number"
                        placeholder="Fiyat"
                        value={newPrice}
                        onChange={(e) => setNewPrice(e.target.value)}
                    />
                    <input
                        type="number"
                        placeholder="Stok"
                        value={newStock}
                        onChange={(e) => setNewStock(e.target.value)}
                    />
                    <select
                        value={newCategoryId}
                        onChange={(e) => setNewCategoryId(e.target.value)}
                    >
                        <option value="">Kategori sec</option>
                        {categories.map((category) => (
                            <option key={category.id} value={category.id}>
                                {category.name}
                            </option>
                        ))}
                    </select>
                    <button type="submit">Ekle</button>
                </form>
            )}
            <ProductList
                products={filteredProducts}
                isAdmin={user?.role === 'Admin'}
                onDelete={handleDelete}
            />
        </div>
    )
}

export default ProductPage