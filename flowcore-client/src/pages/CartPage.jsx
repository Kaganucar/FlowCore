import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useCart } from "../CartContext";
import { useAuth } from "../AuthContext";

function CartPage(){
    const {items, updateQuantity, removeFromCart, clearCart, totalPrice} = useCart()
    const { user } = useAuth()
    const navigate = useNavigate()
    const [submitting, setSubmitting] = useState(false)
    const [error, setError] = useState(null)
    const [createdOrder, setCreateOrder] = useState(null)

    async function handleCheckout() {
        if(!user){
            navigate('/login')
            return
        }
        
        setSubmitting(true)
        setError(null)

        try{
            const response = await fetch(`${import.meta.env.VITE_API_URL}/Order`, {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${user.accessToken}`,
                },
                body: JSON.stringify({
                    items: items.map((item) => ({
                        productId: item.productId,
                        quantity: item.quantity,
                    })),
                }),
            })

            if(response.status === 401){
                throw new Error('Oturumunuz sona ermiş. Lütfen tekrar giriş yapın.')
            }

            if(!response.ok){
                const body = await response.json().catch(() => null)
                throw new Error(body?.error || 'Sipariş oluşturulamadı')
            }

            const order = await response.json()
            clearCart()
            setCreateOrder(order)
        } catch(err){
            setError(err.message)
        } finally {
            setSubmitting(false)
        }
    }

    if (createdOrder) {
        return (
            <div className="mx-auto max-w-5xl px-4 py-8">
                <div className="rounded-lg border border-emerald-200 bg-emerald-50 p-6">
                    <h1 className="text-2xl font-bold text-emerald-800">Siparisiniz olusturuldu</h1>
                    <p className="mt-2 text-sm text-emerald-700">
                        Siparis No: <span className="font-mono">{createdOrder.id}</span>
                    </p>
                    <p className="mt-1 text-sm text-emerald-700">
                        Toplam: <strong>{createdOrder.totalAmount} TL</strong> &middot; Durum: {createdOrder.status}
                    </p>
                    <Link
                        to="/"
                        className="mt-4 inline-block rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700"
                    >
                        Alisverise devam et
                    </Link>
                </div>
            </div>
        )
    }

    if (items.length === 0) {
        return (
            <div className="mx-auto max-w-5xl px-4 py-8">
                <h1 className="text-3xl font-bold text-slate-800">Sepetim</h1>
                <p className="mt-4 text-slate-500">Sepetiniz bos.</p>
                <Link
                    to="/"
                    className="mt-4 inline-block rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700"
                >
                    Urunlere git
                </Link>
            </div>
        )
    }

    return (
        <div className="mx-auto max-w-5xl px-4 py-8">
            <h1 className="text-3xl font-bold text-slate-800">Sepetim</h1>

            {error && (
                <p className="mt-4 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
                    {error}
                </p>
            )}

            <div className="mt-6 divide-y divide-slate-200 rounded-lg border border-slate-200 bg-white">
                {items.map((item) => (
                    <div key={item.productId} className="flex items-center justify-between gap-4 p-4">
                        <div className="min-w-0">
                            <h3 className="truncate font-semibold text-slate-800">{item.name}</h3>
                            <p className="text-sm text-slate-500">{item.price} TL</p>
                        </div>
                        <div className="flex items-center gap-3">
                            <div className="flex items-center rounded-md border border-slate-300">
                                <button
                                    onClick={() => updateQuantity(item.productId, item.quantity - 1)}
                                    className="px-3 py-1 text-slate-600 hover:bg-slate-100"
                                >
                                    -
                                </button>
                                <span className="w-10 text-center text-sm font-medium">{item.quantity}</span>
                                <button
                                    onClick={() => updateQuantity(item.productId, item.quantity + 1)}
                                    className="px-3 py-1 text-slate-600 hover:bg-slate-100"
                                >
                                    +
                                </button>
                            </div>
                            <span className="w-24 text-right font-bold text-indigo-600">
                                {item.price * item.quantity} TL
                            </span>
                            <button
                                onClick={() => removeFromCart(item.productId)}
                                className="rounded-md bg-red-50 px-3 py-1.5 text-sm font-medium text-red-600 hover:bg-red-100"
                            >
                                Sil
                            </button>
                        </div>
                    </div>
                ))}
            </div>

            <div className="mt-6 flex flex-col items-end gap-3">
                <p className="text-lg text-slate-700">
                    Toplam: <strong className="text-indigo-600">{totalPrice} TL</strong>
                </p>
                <button
                    onClick={handleCheckout}
                    disabled={submitting}
                    className="rounded-md bg-indigo-600 px-5 py-2.5 font-medium text-white hover:bg-indigo-700 disabled:cursor-not-allowed disabled:bg-slate-300"
                >
                    {submitting ? 'Gönderiliyor...' : user ? 'Siparişi Tamamla' : 'Giriş yapip tamamla'}
                </button>
            </div>
        </div>
    )
}

export default CartPage
