import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import type { User } from "../types";
import "./AdminDashboard.css";

function AdminDashboard() {
    const [user, setUser] = useState<User | null>(null);

    useEffect(() => {
        // Kullanıcı bilgilerini al
        const userData = localStorage.getItem("user");
        if (userData) {
            setUser(JSON.parse(userData));
        }
    }, []);

    return (
        <div className="admin-dashboard">
            <div className="container">
                {/* Header */}
                <div className="admin-header">
                    <div className="admin-title">
                        <h1>🛠️ Admin Paneli</h1>
                        <p>Hoş geldin, {user?.fullName || "Admin"}!</p>
                    </div>
                    <div className="admin-actions">
                        <Link to="/" className="btn btn-secondary">
                            ← Ana Sayfaya Dön
                        </Link>
                    </div>
                </div>

                {/* Admin Actions */}
                <div className="admin-actions-grid">
                    <div className="action-card">
                        <div className="action-card-header">
                            <div className="action-icon">👥</div>
                            <div className="action-card-title">
                                <h3>Kullanıcı Yönetimi</h3>
                                <p>Kullanıcıları görüntüle, düzenle ve yönet</p>
                            </div>
                        </div>
                        <div className="action-card-footer">
                            <Link to="/admin/users" className="btn btn-primary">
                                👥 Kullanıcıları Yönet
                            </Link>
                        </div>
                    </div>

                    <div className="action-card">
                        <div className="action-card-header">
                            <div className="action-icon">✈️</div>
                            <div className="action-card-title">
                                <h3>Uçuş Yönetimi</h3>
                                <p>Uçuşları ekle, düzenle ve yönet</p>
                            </div>
                        </div>
                        <div className="action-card-footer">
                            <Link to="/admin/flights" className="btn btn-primary">
                                ✈️ Uçuşları Yönet
                            </Link>
                        </div>
                    </div>

                    <div className="action-card">
                        <div className="action-card-header">
                            <div className="action-icon">📊</div>
                            <div className="action-card-title">
                                <h3>Sistem Logları</h3>
                                <p>Sistem loglarını görüntüle ve analiz et</p>
                            </div>
                        </div>
                        <div className="action-card-footer">
                            <Link to="/admin/logs" className="btn btn-primary">
                                📊 Logları Görüntüle
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default AdminDashboard;

