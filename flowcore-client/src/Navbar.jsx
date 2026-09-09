import { Link } from "react-router-dom";
import { useAuth } from "./AuthContext";

function Navbar() {
    const {user, logout} = useAuth()

    return (
        <nav>
            <Link to="/">Urunler</Link>
            {' | '}
            {user ? (
                <>
                    <span>Merhaba, {user.userName} ({user.role})</span>
                    {'|'}
                    <button onClick={logout}>Çıkış yap</button>
                </>
            ) : (
                <>
            <Link to="/login">Giriş</Link>
            {' | '}
            <Link to="/register">Kayıt Ol</Link>
            </>
            )}
        </nav>
    )
}

export default Navbar