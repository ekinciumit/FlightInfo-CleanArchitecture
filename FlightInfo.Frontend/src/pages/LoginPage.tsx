import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import TwoFactorVerify from "./TwoFactorVerify";
import "./LoginPage.css";

function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [requiresTwoFactor, setRequiresTwoFactor] = useState(false);
    const [twoFactorData, setTwoFactorData] = useState<{
        userId: number;
        twoFactorType: 'Email';
    } | null>(null);
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");

        try {
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),
            });

            const data = await response.json();

            if (response.ok) {
                if (data.requiresTwoFactor) {
                    // 2FA gerekli - doğrulama ekranına git
                    setRequiresTwoFactor(true);
                    setTwoFactorData({
                        userId: data.userId,
                        twoFactorType: 'Email'
                    });
                } else {
                    // Direkt giriş yap
                    localStorage.setItem('token', data.token);
                    localStorage.setItem('user', JSON.stringify(data.user));

                    (window as any).showToast.success("Giriş Başarılı", "Hoş geldiniz!");

                    // Navbar'ı güncelle
                    window.dispatchEvent(new Event('authChange'));

                    // Başarılı giriş sonrası yönlendirme
                    navigate("/bookings");
                }
            } else {
                (window as any).showToast.error("Giriş Hatası", data.message || "Email veya şifre hatalı!");
                setError(data.message || "Email veya şifre hatalı!");
            }
        } catch (error) {
            (window as any).showToast.error("Giriş Hatası", "Bir hata oluştu!");
            setError("Bir hata oluştu!");
        }
    };

    const handleTwoFactorSuccess = (token: string, user: any) => {
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(user));

        (window as any).showToast.success("2FA Doğrulama Başarılı", "Hoş geldiniz!");

        // Navbar'ı güncelle
        window.dispatchEvent(new Event('authChange'));

        // Başarılı giriş sonrası yönlendirme
        navigate("/bookings");
    };

    // 2FA doğrulama ekranını göster
    if (requiresTwoFactor && twoFactorData) {
        return (
            <TwoFactorVerify
                userId={twoFactorData.userId}
                twoFactorType={twoFactorData.twoFactorType}
                onVerificationSuccess={handleTwoFactorSuccess}
            />
        );
    }

    return (
        <div className="auth-container">
            <div className="auth-card">
                <div className="auth-header">
                    <div className="auth-logo">
                        <span className="logo-icon">✈️</span>
                        <span className="logo-text">FlightInfo</span>
                    </div>
                    <h2>Giriş Yap</h2>
                    <p>Hesabınıza giriş yapın</p>
                </div>

                <form onSubmit={handleSubmit} className="auth-form">
                    <div className="form-group">
                        <label>Email</label>
                        <input
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="email@example.com"
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label>Şifre</label>
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="••••••••"
                            required
                        />
                    </div>

                    <button type="submit" className="auth-button">
                        Giriş Yap
                    </button>
                </form>

                {error && <div className="error-message">{error}</div>}

                <div className="auth-footer">
                    <p>
                        Hesabınız yok mu? <Link to="/register" className="auth-link">Kayıt olun</Link>
                    </p>
                </div>
            </div>
        </div>
    );
}

export default LoginPage;

