import { createContext, useContext, useEffect, useState } from "react";

const CartContext = createContext(null)

export function CartProvider({ children }) {
    const [items, setItems] = useState(() => {
        const stored = localStorage.getItem('cart')
        return stored ? JSON.parse(stored) : []
    })

    function addToCart(product) {
        setItems((prev) => {
            const existing = prev.find((item) => item.productId === product.id)

            if (existing) {
                return prev.map((item) =>
                    item.productId === product.id
                        ? { ...item, quantity: item.quantity + 1 }
                        : item
                )
            }

            return [
                ...prev,
                { productId: product.id, name: product.name, price: product.price, quantity: 1 },
            ]
        })
    }

    function removeFromCart(productId) {
        setItems((prev) => prev.filter((item) => item.productId !== productId))
    }

    function updateQuantity(productId, quantity) {
        if(quantity < 1){
            removeFromCart(productId)
            return
        }
            
        setItems((prev) =>
            prev.map((item) =>
                item.productId === productId ? { ...item, quantity } : item
            )
        )
    }

    function clearCart(){
        setItems([])
    }

    useEffect(() => {
        localStorage.setItem('cart', JSON.stringify(items))
    }, [items])

    const totalCount = items.reduce((sum, item) => sum + item.quantity, 0)
    const totalPrice = items.reduce((sum, item) => sum + item.quantity * item.price, 0)

    return (
        <CartContext.Provider
            value={{items, addToCart,removeFromCart,updateQuantity,clearCart,totalCount,totalPrice}}
            >
                {children}
            </CartContext.Provider>
    )
}

// eslint-disable-next-line react-refresh/only-export-components
export function useCart(){
    return useContext(CartContext)
}