import { useState, useEffect } from "react";
import type { User } from "../types";
import "./ProfilePage.css";

function ProfilePage() {
    const [user, setUser] = useState<User | null>(null);
    const [isEditing, setIsEditing] = useState(false);
    const [formData, setFormData] = useState({
        fullName: "",
        email: "",
        phone: ""
    });

    useEffect(() => {
        const loadUserData = () => {
            const userData = localStorage.getItem("user");
            if (userData) {
                const parsedUser = JSON.parse(userData);
                setUser(parsedUser);
                setFormData({
                    fullName: parsedUser.fullName || "",
                    email: parsedUser.email || "",
                    phone: parsedUser.phone || ""
                });
            }
        };

        loadUserData();
    }, []);

    const handleEdit = () => {
        setIsEditing(true);
    };

    const handleSave = async () => {
        if (!user) return;

        try {
            const token = localStorage.getItem("token");
            const response = await fetch("/api/User/profile", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    fullName: formData.fullName,
                    phone: formData.phone
                })
            });

            if (!response.ok) {
                if (response.status === 401 || response.status === 403) {
                    window.showToast?.warning?.("Oturum", "Oturum süresi doldu, lütfen tekrar giriş yapın.");
                    localStorage.removeItem("token");
                    localStorage.removeItem("user");
                    window.location.href = "/login";
                    return;
                }
                const text = await response.text();
                throw new Error(text || "Profil güncellenemedi");
            }

            const updated = await response.json();

            const updatedUser: User = {
                ...user,
                fullName: updated.fullName ?? formData.fullName,
                email: updated.email ?? formData.email,
                phone: formData.phone
            };

            localStorage.setItem("user", JSON.stringify(updatedUser));
            setUser(updatedUser);
            setIsEditing(false);
            if (window.showToast?.success) {
                window.showToast.success("Profil", "Profil başarıyla güncellendi!");
            } else {
                alert("Profil başarıyla güncellendi!");
            }
        } catch (err: any) {
            if (window.showToast?.error) {
                window.showToast.error("Hata", err.message || "Bir hata oluştu");
            } else {
                alert(err.message || "Bir hata oluştu");
            }
        }
    };

    const handleCancel = () => {
        setFormData({
            fullName: user?.fullName || "",
            email: user?.email || "",
            phone: user?.phone || ""
        });
        setIsEditing(false);
    };

    const handleLogout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        window.location.href = "/";
    };

    if (!user) {
        return (
            <div className="profile-page">
                <div className="container">
                    <div className="profile-header">
                        <h1>Profil</h1>
                        <p>Kullanıcı bilgileri yükleniyor...</p>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="profile-page">
            <div className="container">
                {/* Header */}
                <div className="profile-header">
                    <h1>Profil Bilgileri</h1>
                    <p>Hesap Ayarlarınızı Yönetin</p>
                </div>

                {/* Profile Card */}
                <div className="profile-card">
                    <div className="profile-avatar">
                        <div className="avatar-circle">
                            {user.fullName?.charAt(0) || user.email?.charAt(0) || "U"}
                        </div>
                        <h2>{user.fullName || "Kullanıcı"}</h2>

                    </div>

                    <div className="profile-info">
                        {isEditing ? (
                            <div className="edit-form">
                                <div className="form-group">
                                    <label htmlFor="fullName">Ad Soyad</label>
                                    <input
                                        type="text"
                                        id="fullName"
                                        value={formData.fullName}
                                        onChange={(e) => setFormData({...formData, fullName: e.target.value})}
                                        className="form-input"
                                    />
                                </div>

                                <div className="form-group">
                                    <label htmlFor="email">E-posta</label>
                                    <input
                                        type="email"
                                        id="email"
                                        value={formData.email}
                                        readOnly
                                        className="form-input"
                                    />
                                </div>

                                <div className="form-group">
                                    <label htmlFor="phone">Telefon</label>
                                    <input
                                        type="tel"
                                        id="phone"
                                        value={formData.phone}
                                        onChange={(e) => setFormData({...formData, phone: e.target.value})}
                                        className="form-input"
                                    />
                                </div>

                                <div className="form-actions">
                                    <button onClick={handleSave} className="btn btn-primary">
                                        Kaydet
                                    </button>
                                    <button onClick={handleCancel} className="btn btn-secondary">
                                        İptal
                                    </button>
                                </div>
                            </div>
                        ) : (
                            <div className="info-display">
                                <div className="info-item">
                                    <span className="label">Ad Soyad:</span>
                                    <span className="value">{user.fullName || "Belirtilmemiş"}</span>
                                </div>

                                <div className="info-item">
                                    <span className="label">E-posta:</span>
                                    <span className="value">{user.email}</span>
                                </div>

                                <div className="info-item">
                                    <span className="label">Telefon:</span>
                                    <span className="value">{user.phone || "Belirtilmemiş"}</span>
                                </div>

                                <div className="profile-actions">
                                    <button onClick={handleEdit} className="btn btn-primary">
                                        Düzenle
                                    </button>
                                    <button onClick={handleLogout} className="btn btn-danger">
                                        Çıkış Yap
                                    </button>
                                </div>
                            </div>
                        )}
                    </div>
                </div>

            </div>
        </div>
    );
}

export default ProfilePage;

