import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import type { User } from "../types";
import "./Navbar.css";

function Navbar() {
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [user, setUser] = useState<User | null>(null);

    useEffect(() => {
        // Token kontrolü
        const checkAuth = () => {
            const token = localStorage.getItem("token");
            const userData = localStorage.getItem("user");
            setIsLoggedIn(!!token);
            setUser(userData ? JSON.parse(userData) : null);
        };

        checkAuth();

        window.addEventListener('storage', checkAuth);

        window.addEventListener('authChange', checkAuth);

        return () => {
            window.removeEventListener('storage', checkAuth);
            window.removeEventListener('authChange', checkAuth);
        };
    }, []);

    // Logout işlemi profil sayfasında yönetiliyor

    return (
        <nav className="navbar">
            <div className="nav-container">
                <div className="nav-left">
                    <Link to="/" className="nav-logo">
                        <img 
                            src="/images/airplane.png.png" 
                            alt="Airplane" 
                            className="logo-icon"
                        />
                        <span className="logo-text">Skyora</span>
                    </Link>
                    {isLoggedIn && (
                        <span className="nav-user">
                            Hoş geldin, {user?.fullName || user?.email || "Kullanıcı"}!
                        </span>
                    )}
                </div>

                <div className={`nav-menu ${isMenuOpen ? 'active' : ''}`}>
                    <Link to="/" className="nav-link" onClick={() => setIsMenuOpen(false)}>
                        Ana Sayfa
                    </Link>
                    <Link to="/search" className="nav-link" onClick={() => setIsMenuOpen(false)}>
                        Uçuş Ara
                    </Link>
                    {isLoggedIn ? (
                        <>
                            <Link to="/bookings" className="nav-link" onClick={() => setIsMenuOpen(false)}>
                                Rezervasyonlarım
                            </Link>
                            <Link to="/profile" className="nav-link" onClick={() => setIsMenuOpen(false)}>
                                Profil
                            </Link>
                            {user?.role === "Admin" && (
                                <Link to="/admin" className="nav-link admin-link" onClick={() => setIsMenuOpen(false)}>
                                    🛠️ Admin Panel
                                </Link>
                            )}
                            {/* Logout sadece Profil sayfasında gösterilecek */}
                        </>
                    ) : (
                        <>
                            <Link to="/login" className="nav-link" onClick={() => setIsMenuOpen(false)}>
                                Giriş Yap
                            </Link>
                            <Link to="/register" className="nav-link register" onClick={() => setIsMenuOpen(false)}>
                                Kayıt Ol
                            </Link>
                        </>
                    )}
                </div>

                <div
                    className={`nav-toggle ${isMenuOpen ? 'active' : ''}`}
                    onClick={() => setIsMenuOpen(!isMenuOpen)}
                >
                    <span></span>
                    <span></span>
                    <span></span>
                </div>
            </div>
        </nav>
    );
}

export default Navbar;

