function ProductList({ products}) {
    return (
        <ul>
            {products.map((product) => (
                <li key={product.id}>
                    {product.name} - {product.price} TL ({product.categoryName})
                </li>
            ))}
        </ul>
    )
}

export default ProductList