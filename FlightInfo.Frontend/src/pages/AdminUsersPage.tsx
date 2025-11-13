import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import "./AdminUsersPage.css";

interface User {
    id: number;
    email: string;
    fullName: string;
    role: string;
    isActive: boolean;
    createdAt: string;
    lastLoginAt?: string;
    phone?: string;
}

function AdminUsersPage() {
    const [users, setUsers] = useState<User[]>([]);
    const [filteredUsers, setFilteredUsers] = useState<User[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");
    const [searchTerm, setSearchTerm] = useState("");
    const [roleFilter, setRoleFilter] = useState("all");
    const [statusFilter, setStatusFilter] = useState("all");
    const [sortBy, setSortBy] = useState("createdAt");
    const [selectedUser, setSelectedUser] = useState<User | null>(null);
    const [showUserModal, setShowUserModal] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [editFormData, setEditFormData] = useState({
        fullName: "",
        role: "",
        phone: ""
    });

    useEffect(() => {
        loadUsers();
    }, []);

    useEffect(() => {
        filterAndSortUsers();
    }, [users, searchTerm, roleFilter, statusFilter, sortBy]);

    const loadUsers = async () => {
        try {
            setIsLoading(true);
            const response = await fetch("http://localhost:7104/api/User", {
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                }
            });

            if (!response.ok) {
                throw new Error("Kullanıcılar yüklenirken hata oluştu");
            }

            const usersData = await response.json();
            setUsers(usersData);
        } catch (error: any) {
            console.error("Kullanıcı yükleme hatası:", error);
            setError(error.message);
        } finally {
            setIsLoading(false);
        }
    };

    const filterAndSortUsers = () => {
        let filtered = [...users];

        // Arama filtresi
        if (searchTerm) {
            filtered = filtered.filter(user =>
                user.fullName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
                (user.phone && user.phone.includes(searchTerm))
            );
        }

        // Role filtresi
        if (roleFilter !== "all") {
            filtered = filtered.filter(user => user.role === roleFilter);
        }

        // Status filtresi
        if (statusFilter !== "all") {
            if (statusFilter === "active") {
                filtered = filtered.filter(user => user.isActive);
            } else if (statusFilter === "inactive") {
                filtered = filtered.filter(user => !user.isActive);
            }
        }

        // Sıralama
        filtered.sort((a, b) => {
            let aValue: any, bValue: any;
            let isAscending: boolean = true; // Varsayılan sıralama yönü

            switch (sortBy) {
                case "fullName":
                    // Ad Soyad: Alfabetik artan sıralama (A-Z)
                    aValue = (a.fullName || "").toLowerCase();
                    bValue = (b.fullName || "").toLowerCase();
                    isAscending = true;
                    break;
                case "email":
                    // Email: Alfabetik artan sıralama (A-Z)
                    aValue = (a.email || "").toLowerCase();
                    bValue = (b.email || "").toLowerCase();
                    isAscending = true;
                    break;
                case "role":
                    // Rol: Alfabetik artan sıralama (A-Z)
                    aValue = (a.role || "").toLowerCase();
                    bValue = (b.role || "").toLowerCase();
                    isAscending = true;
                    break;
                case "createdAt":
                    // Kayıt Tarihi: Azalan sıralama (en yeni önce)
                    aValue = new Date(a.createdAt).getTime();
                    bValue = new Date(b.createdAt).getTime();
                    isAscending = false;
                    break;
                case "lastLoginAt":
                    // Son Giriş: Azalan sıralama (en yeni önce)
                    aValue = new Date(a.lastLoginAt || 0).getTime();
                    bValue = new Date(b.lastLoginAt || 0).getTime();
                    isAscending = false;
                    break;
                default:
                    // Varsayılan: Kayıt Tarihi azalan
                    aValue = new Date(a.createdAt).getTime();
                    bValue = new Date(b.createdAt).getTime();
                    isAscending = false;
            }

            // Sıralama uygula
            if (isAscending) {
                // Artan sıralama (A-Z, küçükten büyüğe)
                if (aValue < bValue) return -1;
                if (aValue > bValue) return 1;
                return 0;
            } else {
                // Azalan sıralama (Z-A, büyükten küçüğe)
                if (aValue > bValue) return -1;
                if (aValue < bValue) return 1;
                return 0;
            }
        });

        setFilteredUsers(filtered);
    };

    const getRoleColor = (role: string) => {
        switch (role.toLowerCase()) {
            case "admin":
                return "#e74c3c";
            case "user":
                return "#3498db";
            default:
                return "#95a5a6";
        }
    };

    const getRoleIcon = (role: string) => {
        switch (role.toLowerCase()) {
            case "admin":
                return "👑";
            case "user":
                return "👤";
            default:
                return "❓";
        }
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleString('tr-TR', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    const handleUserClick = (user: User) => {
        setSelectedUser(user);
        setEditFormData({
            fullName: user.fullName,
            role: user.role,
            phone: user.phone || ""
        });
        setIsEditing(false);
        setShowUserModal(true);
    };

    const handleEditClick = () => {
        setIsEditing(true);
    };

    const handleCancelEdit = () => {
        if (selectedUser) {
            setEditFormData({
                fullName: selectedUser.fullName,
                role: selectedUser.role,
                phone: selectedUser.phone || ""
            });
        }
        setIsEditing(false);
    };

    const handleSaveEdit = async () => {
        if (!selectedUser) return;

        try {
            const response = await fetch(`http://localhost:7104/api/User/${selectedUser.id}`, {
                method: "PUT",
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    id: selectedUser.id,
                    fullName: editFormData.fullName,
                    role: editFormData.role,
                    phone: editFormData.phone || null
                })
            });

            if (response.ok) {
                const updatedUser = await response.json();
                setUsers(prev => prev.map(user =>
                    user.id === selectedUser.id
                        ? { ...user, ...updatedUser }
                        : user
                ));
                setSelectedUser({ ...selectedUser, ...updatedUser });
                setIsEditing(false);
                alert("Kullanıcı bilgileri başarıyla güncellendi");
            } else {
                const errorData = await response.json();
                throw new Error(errorData.message || "Kullanıcı güncellenirken hata oluştu");
            }
        } catch (error: any) {
            console.error("Kullanıcı güncelleme hatası:", error);
            alert("Hata: " + error.message);
        }
    };

    const toggleUserStatus = async (userId: number, currentStatus: boolean) => {
        try {
            const response = await fetch(`http://localhost:7104/api/User/${userId}/toggle-status`, {
                method: "PUT",
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ isActive: !currentStatus })
            });

            if (response.ok) {
                // Kullanıcı listesini güncelle
                setUsers(prev => prev.map(user =>
                    user.id === userId
                        ? { ...user, isActive: !currentStatus }
                        : user
                ));
                alert(`Kullanıcı ${!currentStatus ? 'aktif' : 'pasif'} hale getirildi`);
            } else {
                throw new Error("Kullanıcı durumu güncellenirken hata oluştu");
            }
        } catch (error: any) {
            console.error("Kullanıcı durumu güncelleme hatası:", error);
            alert("Hata: " + error.message);
        }
    };

    const deleteUser = async (userId: number) => {
        if (window.confirm("Bu kullanıcıyı silmek istediğinizden emin misiniz? Bu işlem geri alınamaz!")) {
            try {
                const response = await fetch(`http://localhost:7104/api/User/${userId}`, {
                    method: "DELETE",
                    headers: {
                        "Authorization": `Bearer ${localStorage.getItem("token")}`,
                        "Content-Type": "application/json"
                    }
                });

                if (response.ok) {
                    setUsers(prev => prev.filter(user => user.id !== userId));
                    alert("Kullanıcı başarıyla silindi");
                } else {
                    throw new Error("Kullanıcı silinirken hata oluştu");
                }
            } catch (error: any) {
                console.error("Kullanıcı silme hatası:", error);
                alert("Hata: " + error.message);
            }
        }
    };

    if (isLoading) {
        return (
            <div className="admin-users-page">
                <div className="container">
                    <div className="loading-state">
                        <div className="loading-spinner"></div>
                        <p>Kullanıcılar yükleniyor...</p>
                    </div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="admin-users-page">
                <div className="container">
                    <div className="error-state">
                        <h2>❌ Hata</h2>
                        <p>{error}</p>
                        <button onClick={loadUsers} className="btn btn-primary">
                            Tekrar Dene
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="admin-users-page">
            <div className="container">
                {/* Header */}
                <div className="users-header">
                    <Link to="/admin" className="back-button">
                        <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M10 12L6 8L10 4" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                        </svg>
                        <span>Admin Paneli</span>
                    </Link>
                    <div className="header-main">
                        <div className="header-icon">👥</div>
                        <div className="header-text-content">
                            <h1>Kullanıcı Yönetimi</h1>
                            <div className="user-count-badge">
                                <span className="count-number">{filteredUsers.length}</span>
                                <span className="count-label">kullanıcı</span>
                            </div>
                        </div>
                    </div>
                    <button onClick={loadUsers} className="refresh-button">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M1 4V10H7" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                            <path d="M23 20V14H17" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                            <path d="M20.49 9A9 9 0 0 0 5.64 5.64L1 10M23 14L18.36 18.36A9 9 0 0 1 3.51 15" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                        </svg>
                        <span>Yenile</span>
                    </button>
                </div>

                {/* Filters */}
                <div className="filters-section">
                    <div className="filter-group">
                        <label>Ara:</label>
                        <input
                            type="text"
                            placeholder="Ad, email veya telefon ara..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="search-input"
                        />
                    </div>
                    <div className="filter-group">
                        <label>Rol:</label>
                        <select
                            value={roleFilter}
                            onChange={(e) => setRoleFilter(e.target.value)}
                            className="filter-select"
                        >
                            <option value="all">Tümü</option>
                            <option value="Admin">Admin</option>
                            <option value="User">User</option>
                        </select>
                    </div>
                    <div className="filter-group">
                        <label>Durum:</label>
                        <select
                            value={statusFilter}
                            onChange={(e) => setStatusFilter(e.target.value)}
                            className="filter-select"
                        >
                            <option value="all">Tümü</option>
                            <option value="active">Aktif</option>
                            <option value="inactive">Pasif</option>
                        </select>
                    </div>
                    <div className="filter-group">
                        <label>Sırala:</label>
                        <select
                            value={sortBy}
                            onChange={(e) => setSortBy(e.target.value)}
                            className="filter-select"
                        >
                            <option value="createdAt">Kayıt Tarihi</option>
                            <option value="fullName">Ad Soyad</option>
                            <option value="email">Email</option>
                            <option value="role">Rol</option>
                            <option value="lastLoginAt">Son Giriş</option>
                        </select>
                    </div>
                </div>

                {/* Users List */}
                <div className="users-list">
                    {filteredUsers.length === 0 ? (
                        <div className="empty-state">
                            <div className="empty-icon">👥</div>
                            <h3>Kullanıcı bulunamadı</h3>
                            <p>Arama kriterlerinize uygun kullanıcı bulunmuyor.</p>
                        </div>
                    ) : (
                        <div className="users-grid">
                            {filteredUsers.map(user => (
                                <div key={user.id} className="user-card">
                                    <div className="user-header">
                                        <div className="user-avatar">
                                            {getRoleIcon(user.role)}
                                        </div>
                                        <div className="user-info">
                                            <h3>{user.fullName}</h3>
                                            <p>{user.email}</p>
                                        </div>
                                        <div className="user-status">
                                            <span
                                                className={`status-badge ${user.isActive ? 'active' : 'inactive'}`}
                                            >
                                                {user.isActive ? '✅ Aktif' : '❌ Pasif'}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="user-details">
                                        <div className="detail-row">
                                            <span className="label">Rol:</span>
                                            <span
                                                className="role-badge"
                                                style={{ backgroundColor: getRoleColor(user.role) }}
                                            >
                                                {getRoleIcon(user.role)} {user.role}
                                            </span>
                                        </div>
                                        <div className="detail-row">
                                            <span className="label">Kayıt:</span>
                                            <span className="value">{formatDate(user.createdAt)}</span>
                                        </div>
                                        {user.lastLoginAt && (
                                            <div className="detail-row">
                                                <span className="label">Son Giriş:</span>
                                                <span className="value">{formatDate(user.lastLoginAt)}</span>
                                            </div>
                                        )}
                                        {user.phone && (
                                            <div className="detail-row">
                                                <span className="label">Telefon:</span>
                                                <span className="value">{user.phone}</span>
                                            </div>
                                        )}
                                    </div>

                                    <div className="user-actions">
                                        <button
                                            className="btn btn-primary"
                                            onClick={() => handleUserClick(user)}
                                        >
                                            👁️ Detaylar
                                        </button>
                                        <button
                                            className={`btn ${user.isActive ? 'btn-warning' : 'btn-success'}`}
                                            onClick={() => toggleUserStatus(user.id, user.isActive)}
                                        >
                                            {user.isActive ? '⏸️ Pasifleştir' : '▶️ Aktifleştir'}
                                        </button>
                                        <button
                                            className="btn btn-danger"
                                            onClick={() => deleteUser(user.id)}
                                        >
                                            🗑️ Sil
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>

                {/* User Detail Modal */}
                {showUserModal && selectedUser && (
                    <div className="modal-overlay" onClick={() => setShowUserModal(false)}>
                        <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <h2>Kullanıcı Detayları</h2>
                                <button
                                    className="modal-close"
                                    onClick={() => setShowUserModal(false)}
                                >
                                    ×
                                </button>
                            </div>

                            <div className="modal-body">
                                <div className="user-detail-card">
                                    <div className="detail-header">
                                        <div className="user-avatar-large">
                                            {getRoleIcon(selectedUser.role)}
                                        </div>
                                        <div className="user-info-large">
                                            <h3>{selectedUser.fullName}</h3>
                                            <p>{selectedUser.email}</p>
                                            <span
                                                className={`status-badge ${selectedUser.isActive ? 'active' : 'inactive'}`}
                                            >
                                                {selectedUser.isActive ? '✅ Aktif' : '❌ Pasif'}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="detail-sections">
                                        <div className="detail-section">
                                            <h4>Genel Bilgiler</h4>
                                            <div className="detail-grid">
                                                <div className="detail-item">
                                                    <span className="label">ID:</span>
                                                    <span className="value">#{selectedUser.id}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Ad Soyad:</span>
                                                    {isEditing ? (
                                                        <input
                                                            type="text"
                                                            value={editFormData.fullName}
                                                            onChange={(e) => setEditFormData({ ...editFormData, fullName: e.target.value })}
                                                            className="edit-input"
                                                        />
                                                    ) : (
                                                        <span className="value">{selectedUser.fullName}</span>
                                                    )}
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Email:</span>
                                                    <span className="value">{selectedUser.email} <span style={{ fontSize: "0.8rem", color: "#999" }}>(Değiştirilemez)</span></span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Rol:</span>
                                                    {isEditing ? (
                                                        <select
                                                            value={editFormData.role}
                                                            onChange={(e) => setEditFormData({ ...editFormData, role: e.target.value })}
                                                            className="edit-select"
                                                        >
                                                            <option value="User">User</option>
                                                            <option value="Admin">Admin</option>
                                                            <option value="Moderator">Moderator</option>
                                                        </select>
                                                    ) : (
                                                        <span
                                                            className="role-badge"
                                                            style={{ backgroundColor: getRoleColor(selectedUser.role) }}
                                                        >
                                                            {getRoleIcon(selectedUser.role)} {selectedUser.role}
                                                        </span>
                                                    )}
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Telefon:</span>
                                                    {isEditing ? (
                                                        <input
                                                            type="text"
                                                            value={editFormData.phone}
                                                            onChange={(e) => setEditFormData({ ...editFormData, phone: e.target.value })}
                                                            className="edit-input"
                                                            placeholder="+90..."
                                                        />
                                                    ) : (
                                                        <span className="value">{selectedUser.phone || "Belirtilmemiş"}</span>
                                                    )}
                                                </div>
                                            </div>
                                        </div>

                                        <div className="detail-section">
                                            <h4>Hesap Bilgileri</h4>
                                            <div className="detail-grid">
                                                <div className="detail-item">
                                                    <span className="label">Kayıt Tarihi:</span>
                                                    <span className="value">{formatDate(selectedUser.createdAt)}</span>
                                                </div>
                                                {selectedUser.lastLoginAt && (
                                                    <div className="detail-item">
                                                        <span className="label">Son Giriş:</span>
                                                        <span className="value">{formatDate(selectedUser.lastLoginAt)}</span>
                                                    </div>
                                                )}
                                                <div className="detail-item">
                                                    <span className="label">Durum:</span>
                                                    <span className={`status-badge ${selectedUser.isActive ? 'active' : 'inactive'}`}>
                                                        {selectedUser.isActive ? '✅ Aktif' : '❌ Pasif'}
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div className="modal-footer">
                                {isEditing ? (
                                    <>
                                        <button
                                            className="btn btn-secondary"
                                            onClick={handleCancelEdit}
                                        >
                                            İptal
                                        </button>
                                        <button
                                            className="btn btn-primary"
                                            onClick={handleSaveEdit}
                                        >
                                            💾 Kaydet
                                        </button>
                                    </>
                                ) : (
                                    <>
                                        <button
                                            className="btn btn-secondary"
                                            onClick={() => setShowUserModal(false)}
                                        >
                                            Kapat
                                        </button>
                                        <button
                                            className="btn btn-primary"
                                            onClick={handleEditClick}
                                        >
                                            ✏️ Düzenle
                                        </button>
                                        <button
                                            className={`btn ${selectedUser.isActive ? 'btn-warning' : 'btn-success'}`}
                                            onClick={() => {
                                                toggleUserStatus(selectedUser.id, selectedUser.isActive);
                                                setShowUserModal(false);
                                            }}
                                        >
                                            {selectedUser.isActive ? '⏸️ Pasifleştir' : '▶️ Aktifleştir'}
                                        </button>
                                        <button
                                            className="btn btn-danger"
                                            onClick={() => {
                                                deleteUser(selectedUser.id);
                                                setShowUserModal(false);
                                            }}
                                        >
                                            🗑️ Sil
                                        </button>
                                    </>
                                )}
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}

export default AdminUsersPage;

