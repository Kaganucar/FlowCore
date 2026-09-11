import { createContext, useContext, useState } from "react";

const CartContext = createContext(null)

export function CartProvider({ children }) {
    const [items, setItems] = useState([])

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
        setItems((prev) => prev.filter((item) => item.productId !== product.id))
    }

    function updateQuantity(productId, quantity) {
        setItems((prev) =>
            prev.map((item) =>
                item.productId === productId ? { ...item, quantity } : item
            )
        )
    }

    function clearCart(){
        setItems([])
    }

    const totalCount = item.reduce((sum, item) => sum + item.quantity, 0)
    const totalPrice = item.reduce((sum, item) => sum + item.quantity * item.price, 0)

    return (
        <CartContext.Provider
            value={{items, addToCart,removeFromCart,updateQuantity,clearCart,totalCount,totalPrice}}
            >
                {children}
            </CartContext.Provider>
    )
}

export function useCart(){
    return useContext(CartContext)
}