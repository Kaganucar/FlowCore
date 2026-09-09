function ProductList({ products, isAdmin, onDelete}) {
    return (
        <ul>
            {products.map((product) => (
                <li key={product.id}>
                    {product.name} - {product.price} TL ({product.categoryName})
                    {isAdmin && (
                        <button onClick={()=> onDelete(product.id)}>Sil</button>
                    )}
                </li>
            ))}
        </ul>
    )
}

export default ProductList