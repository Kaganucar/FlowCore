import { Link } from "react-router-dom";

function Navbar() {
    return (
        <nav>
            <Link to="/">Urunler</Link>
            {' | '}
            <Link to="/login">Giriş</Link>
            {' | '}
            <Link to="/register">Kayıt Ol</Link>
        </nav>
    )
}

export default Navbar