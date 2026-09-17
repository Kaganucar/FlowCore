import { useEffect, useState } from "react";
import ProductList from "../ProductList";
import { useAuth } from "../AuthContext";
import { useCart } from "../CartContext";

function ProductsPage() {
    const { user } = useAuth()
    const { addToCart } = useCart()
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
            .catch((err) => alert(err.message))
    }

    const filteredProducts = products.filter((product) => {
        const matchesSearch = product.name.toLowerCase().includes(searchText.toLocaleLowerCase())
        const matchesCategory = selectedCategory === '' || product.categoryName === selectedCategory
        return matchesSearch && matchesCategory
    })

    if (loading) return <p>Yukleniyor...</p>
    if (error) return <p>Hata: {error}</p>

    return (
        <div className="mx-auto max-w-5xl px-4 py-8">
      <h1 className="text-3xl font-bold text-slate-800">Urunler</h1>

      <div className="mt-4 flex flex-col gap-3 sm:flex-row">
        <input
          type="text"
          placeholder="Urun ara..."
          value={searchText}
          onChange={(e) => setSearchText(e.target.value)}
          className="flex-1 rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
        />
        <select
          value={selectedCategory}
          onChange={(e) => setSelectedCategory(e.target.value)}
          className="rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
        >
          <option value="">Tum kategoriler</option>
          {categories.map((category) => (
            <option key={category.id} value={category.name}>
              {category.name}
            </option>
          ))}
        </select>
      </div>

      {user?.role === 'Admin' && (
        <form
          onSubmit={handleCreateSubmit}
          className="mt-6 rounded-lg border border-slate-200 bg-slate-50 p-4"
        >
          <h3 className="mb-3 font-semibold text-slate-700">Yeni Urun Ekle</h3>
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-5">
            <input
              type="text"
              placeholder="Urun adi"
              value={newName}
              onChange={(e) => setNewName(e.target.value)}
              className="rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
            />
            <input
              type="text"
              placeholder="Aciklama"
              value={newDescription}
              onChange={(e) => setNewDescription(e.target.value)}
              className="rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
            />
            <input
              type="number"
              placeholder="Fiyat"
              value={newPrice}
              onChange={(e) => setNewPrice(e.target.value)}
              className="rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
            />
            <input
              type="number"
              placeholder="Stok"
              value={newStock}
              onChange={(e) => setNewStock(e.target.value)}
              className="rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
            />
            <select
              value={newCategoryId}
              onChange={(e) => setNewCategoryId(e.target.value)}
              className="rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
            >
              <option value="">Kategori sec</option>
              {categories.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.name}
                </option>
              ))}
            </select>
          </div>
          <button
            type="submit"
            className="mt-3 rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700"
          >
            Ekle
          </button>
        </form>
      )}

      <div className="mt-6">
        <ProductList 
          products={filteredProducts}
          isAdmin={user?.role === 'Admin'}
          onDelete={handleDelete}
          onAddToCart={addToCart}
        />
      </div>
    </div>
    )
}

export default ProductsPage