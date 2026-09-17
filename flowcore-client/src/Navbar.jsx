import { Link } from "react-router-dom";
import { useAuth } from "./AuthContext";
import { useCart } from "./CartContext";

function Navbar() {
    const {user, logout} = useAuth()
    const {totalCount} = useCart()


    return (
        <nav className="sticky top-0 z-10 border-b border-slate-200 bg-white/80 backdrop-blur">
      <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-3">
        <Link to="/" className="text-xl font-bold text-indigo-600">
          FlowCore
        </Link>
        <div className="flex items-center gap-4 text-sm">
          <Link to="/" className="text-slate-600 hover:text-indigo-600">
            Urunler
          </Link>
          <Link to="/cart" className="relative text-slate-600 hover:text-indigo-600">
            Sepet
            {totalCount > 0 && (
              <span className="absolute -right-4 -top-2 rounded-full bg-indigo-600 px-1.5 py-0.5 text-xs font-medium text-white">
                {totalCount}
              </span>
            )}
          </Link>
          {user ? (
            <>
              <span className="text-slate-500">
                {user.userName}{' '}
                <span className="rounded-full bg-indigo-100 px-2 py-0.5 text-xs font-medium text-indigo-700">
                  {user.role}
                </span>
              </span>
              <button
                onClick={logout}
                className="rounded-md bg-slate-100 px-3 py-1.5 font-medium text-slate-700 hover:bg-slate-200"
              >
                Cikis Yap
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="text-slate-600 hover:text-indigo-600">
                Giris
              </Link>
              <Link
                to="/register"
                className="rounded-md bg-indigo-600 px-3 py-1.5 font-medium text-white hover:bg-indigo-700"
              >
                Kayit Ol
              </Link>
            </>
          )}
        </div>
      </div>
    </nav>
    )
}

export default Navbar