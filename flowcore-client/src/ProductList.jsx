function ProductList({ products, isAdmin, onDelete}) {
    return (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {products.map((product) => (
        <div
          key={product.id}
          className="flex flex-col justify-between rounded-lg border border-slate-200 bg-white p-4 shadow-sm transition hover:shadow-md"
        >
          <div>
            <div className="flex items-start justify-between gap-2">
              <h3 className="font-semibold text-slate-800">{product.name}</h3>
              <span className="whitespace-nowrap rounded-full bg-slate-100 px-2 py-0.5 text-xs font-medium text-slate-600">
                {product.categoryName}
              </span>
            </div>
            <p className="mt-2 text-lg font-bold text-indigo-600">{product.price} TL</p>
          </div>
          {isAdmin && (
            <button
              onClick={() => onDelete(product.id)}
              className="mt-4 w-full rounded-md bg-red-50 px-3 py-1.5 text-sm font-medium text-red-600 hover:bg-red-100"
            >
              Sil
            </button>
          )}
        </div>
      ))}
    </div>
    )
}

export default ProductList