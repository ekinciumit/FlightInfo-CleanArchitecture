import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import type { User } from "../types";
import "./AdminDashboard.css";

function AdminDashboard() {
    const [user, setUser] = useState<User | null>(null);
    const [stats, setStats] = useState({
        totalUsers: 0,
        totalFlights: 0,
        totalReservations: 0,
        totalLogs: 0
    });
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        // Kullanıcı bilgilerini al
        const userData = localStorage.getItem("user");
        if (userData) {
            setUser(JSON.parse(userData));
        }

        // Admin istatistiklerini yükle
        loadAdminStats();
    }, []);

    const loadAdminStats = async () => {
        try {
            setIsLoading(true);

            // Paralel olarak tüm istatistikleri yükle
            const [usersRes, flightsRes, reservationsRes, logsRes] = await Promise.all([
                fetch("http://localhost:7104/api/User", {
                    headers: {
                        "Authorization": `Bearer ${localStorage.getItem("token")}`,
                        "Content-Type": "application/json"
                    }
                }),
                fetch("http://localhost:7104/api/Flight", {
                    headers: {
                        "Authorization": `Bearer ${localStorage.getItem("token")}`,
                        "Content-Type": "application/json"
                    }
                }),
                fetch("http://localhost:7104/api/Reservation", {
                    headers: {
                        "Authorization": `Bearer ${localStorage.getItem("token")}`,
                        "Content-Type": "application/json"
                    }
                }),
                fetch("http://localhost:7104/api/Log", {
                    headers: {
                        "Authorization": `Bearer ${localStorage.getItem("token")}`,
                        "Content-Type": "application/json"
                    }
                })
            ]);

            const [users, flights, reservations, logs] = await Promise.all([
                usersRes.json(),
                flightsRes.json(),
                reservationsRes.json(),
                logsRes.json()
            ]);

            setStats({
                totalUsers: users.length,
                totalFlights: flights.length,
                totalReservations: reservations.length,
                totalLogs: logs.length
            });
        } catch (error) {
            console.error("Admin istatistikleri yüklenirken hata:", error);
        } finally {
            setIsLoading(false);
        }
    };

    if (isLoading) {
        return (
            <div className="admin-dashboard">
                <div className="container">
                    <div className="loading-state">
                        <div className="loading-spinner"></div>
                        <p>Admin paneli yükleniyor...</p>
                    </div>
                </div>
            </div>
        );
    }

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

                {/* Stats Cards */}
                <div className="stats-grid">
                    <div className="stat-card">
                        <div className="stat-icon">👥</div>
                        <div className="stat-content">
                            <h3>{stats.totalUsers}</h3>
                            <p>Toplam Kullanıcı</p>
                        </div>
                    </div>
                    <div className="stat-card">
                        <div className="stat-icon">✈️</div>
                        <div className="stat-content">
                            <h3>{stats.totalFlights}</h3>
                            <p>Toplam Uçuş</p>
                        </div>
                    </div>
                    <div className="stat-card">
                        <div className="stat-icon">📋</div>
                        <div className="stat-content">
                            <h3>{stats.totalReservations}</h3>
                            <p>Toplam Rezervasyon</p>
                        </div>
                    </div>
                    <div className="stat-card">
                        <div className="stat-icon">📊</div>
                        <div className="stat-content">
                            <h3>{stats.totalLogs}</h3>
                            <p>Toplam Log</p>
                        </div>
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
                            <div className="action-icon">📋</div>
                            <div className="action-card-title">
                                <h3>Rezervasyon Yönetimi</h3>
                                <p>Tüm rezervasyonları görüntüle ve yönet</p>
                            </div>
                        </div>
                        <div className="action-card-footer">
                            <Link to="/bookings" className="btn btn-primary">
                                📋 Rezervasyonları Yönet
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

                {/* Quick Stats */}
                <div className="quick-stats">
                    <h2>Hızlı İstatistikler</h2>
                    <div className="quick-stats-grid">
                        <div className="quick-stat">
                            <span className="label">Aktif Kullanıcılar:</span>
                            <span className="value">{stats.totalUsers}</span>
                        </div>
                        <div className="quick-stat">
                            <span className="label">Planlanan Uçuşlar:</span>
                            <span className="value">{stats.totalFlights}</span>
                        </div>
                        <div className="quick-stat">
                            <span className="label">Toplam Rezervasyon:</span>
                            <span className="value">{stats.totalReservations}</span>
                        </div>
                        <div className="quick-stat">
                            <span className="label">Sistem Logları:</span>
                            <span className="value">{stats.totalLogs}</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default AdminDashboard;

