import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import ConfirmationModal from "../components/ConfirmationModal";
import "./AdminLogsPage.css";

interface Log {
    id: number;
    level: string;
    message: string;
    action: string;
    details: string;
    data: string;
    exception: string;
    timestamp: string;
    createdAt: string;
    userId?: number;
    flightId?: number;
    userName?: string;
    flightNumber?: string;
    user?: {
        id: number;
        fullName: string;
        email: string;
    };
    flight?: {
        id: number;
        flightNumber: string;
        origin: string;
        destination: string;
    };
}

function AdminLogsPage() {
    const [logs, setLogs] = useState<Log[]>([]);
    const [filteredLogs, setFilteredLogs] = useState<Log[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");
    const [searchTerm, setSearchTerm] = useState("");
    const [levelFilter, setLevelFilter] = useState("all");
    const [sortBy, setSortBy] = useState("timestamp");
    const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc");
    const [showClearConfirm, setShowClearConfirm] = useState(false);

    useEffect(() => {
        loadLogs();
    }, []);

    useEffect(() => {
        filterAndSortLogs();
    }, [logs, searchTerm, levelFilter, sortBy, sortOrder]);

    const loadLogs = async () => {
        try {
            setIsLoading(true);
            const response = await fetch("http://localhost:7104/api/Log", {
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                }
            });

            if (!response.ok) {
                throw new Error("Loglar yüklenirken hata oluştu");
            }

            const logsData = await response.json();
            setLogs(logsData);
        } catch (error: any) {
            console.error("Log yükleme hatası:", error);
            setError(error.message);
        } finally {
            setIsLoading(false);
        }
    };

    const filterAndSortLogs = () => {
        let filtered = [...logs];

        // Arama filtresi
        if (searchTerm) {
            filtered = filtered.filter(log =>
                (log.message && log.message.toLowerCase().includes(searchTerm.toLowerCase())) ||
                (log.action && log.action.toLowerCase().includes(searchTerm.toLowerCase())) ||
                (log.details && log.details.toLowerCase().includes(searchTerm.toLowerCase())) ||
                (log.user?.fullName && log.user.fullName.toLowerCase().includes(searchTerm.toLowerCase())) ||
                (log.flight?.flightNumber && log.flight.flightNumber.toLowerCase().includes(searchTerm.toLowerCase()))
            );
        }

        // Level filtresi
        if (levelFilter !== "all") {
            filtered = filtered.filter(log => log.level === levelFilter);
        }

        // Sıralama
        filtered.sort((a, b) => {
            let aValue: any, bValue: any;

            switch (sortBy) {
                case "timestamp":
                    aValue = new Date(a.timestamp || a.createdAt);
                    bValue = new Date(b.timestamp || b.createdAt);
                    break;
                case "level":
                    aValue = a.level;
                    bValue = b.level;
                    break;
                case "message":
                    aValue = a.message;
                    bValue = b.message;
                    break;
                case "action":
                    aValue = a.action;
                    bValue = b.action;
                    break;
                default:
                    aValue = new Date(a.timestamp || a.createdAt);
                    bValue = new Date(b.timestamp || b.createdAt);
            }

            if (sortOrder === "asc") {
                return aValue > bValue ? 1 : -1;
            } else {
                return aValue < bValue ? 1 : -1;
            }
        });

        setFilteredLogs(filtered);
    };

    const getActionText = (action: string | null | undefined): string => {
        if (!action) return "Bilinmeyen İşlem";
        
        switch (action.toUpperCase()) {
            case "USER_LOGIN":
                return "Kullanıcı Girişi";
            case "USER_LOGOUT":
                return "Kullanıcı Çıkışı";
            case "USER_CREATED":
                return "Kullanıcı Oluşturuldu";
            case "USER_UPDATED":
                return "Kullanıcı Güncellendi";
            case "USER_DELETED":
                return "Kullanıcı Silindi";
            case "FLIGHT_CREATED":
                return "Uçuş Oluşturuldu";
            case "FLIGHT_UPDATED":
                return "Uçuş Güncellendi";
            case "FLIGHT_DELETED":
                return "Uçuş Silindi";
            case "RESERVATION_CREATED":
                return "Rezervasyon Oluşturuldu";
            case "RESERVATION_CANCELLED":
                return "Rezervasyon İptal Edildi";
            case "RESERVATION_UPDATED":
                return "Rezervasyon Güncellendi";
            case "PASSWORD_CHANGED":
                return "Şifre Değiştirildi";
            case "PROFILE_UPDATED":
                return "Profil Güncellendi";
            default:
                // Eğer tanımlı değilse, action'ı daha okunabilir hale getir
                return action
                    .replace(/_/g, ' ')
                    .toLowerCase()
                    .split(' ')
                    .map(word => word.charAt(0).toUpperCase() + word.slice(1))
                    .join(' ');
        }
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleString('tr-TR', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });
    };

    const formatTime = (dateString: string) => {
        return new Date(dateString).toLocaleTimeString('tr-TR', {
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });
    };

    const formatLogDetails = (data: string, action: string, userName?: string): string => {
        try {
            const parsed = JSON.parse(data);
            const displayName = parsed.Email || parsed.UserName || userName || "Bilinmeyen Kullanıcı";
            
            switch (action?.toUpperCase()) {
                case "USER_LOGIN":
                    const ip = parsed.IPAddress || parsed.IpAddress || "Bilinmeyen IP";
                    const userAgent = parsed.UserAgent || parsed.Browser || "Web Tarayıcı";
                    return `${displayName} (${ip}) IP adresinden "${userAgent}" ile giriş yaptı`;
                
                case "USER_LOGOUT":
                    return `${displayName} sistemden çıkış yaptı`;
                
                case "FLIGHT_CREATED":
                case "FLIGHT_UPDATED":
                    const flightNum = parsed.FlightNumber || parsed.flightNumber || "Bilinmeyen";
                    const origin = parsed.Origin || parsed.origin || "";
                    const dest = parsed.Destination || parsed.destination || "";
                    const route = origin && dest ? ` (${origin} → ${dest})` : "";
                    return `${displayName}, ${flightNum}${route} uçuşunu ${action?.includes("CREATED") ? "oluşturdu" : "güncelledi"}`;
                
                case "RESERVATION_CREATED":
                case "RESERVATION_CANCELLED":
                    const flightNumRes = parsed.FlightNumber || parsed.flightNumber || "Bilinmeyen";
                    return `${displayName}, ${flightNumRes} uçuşu için rezervasyon ${action?.includes("CANCELLED") ? "iptal etti" : "oluşturdu"}`;
                
                case "USER_CREATED":
                case "USER_UPDATED":
                    const targetUser = parsed.FullName || parsed.fullName || parsed.Email || "Kullanıcı";
                    return `${displayName}, ${targetUser} kullanıcısını ${action?.includes("CREATED") ? "oluşturdu" : "güncelledi"}`;
                
                default:
                    // Genel format: Key-value çiftlerini okunabilir hale getir
                    const parts: string[] = [];
                    Object.keys(parsed).forEach(key => {
                        if (key !== "Email" && key !== "UserName" && key !== "IPAddress") {
                            const value = parsed[key];
                            if (value && typeof value === 'string' && value.length < 100) {
                                parts.push(`${key}: ${value}`);
                            }
                        }
                    });
                    return parts.length > 0 ? parts.join(", ") : `${displayName} işlem gerçekleştirdi`;
            }
        } catch {
            // JSON parse edilemezse direkt göster
            return data.length > 150 ? data.substring(0, 150) + "..." : data;
        }
    };

    const handleClearClick = () => {
        setShowClearConfirm(true);
    };

    const handleClearConfirm = async () => {
        setShowClearConfirm(false);
        try {
            const response = await fetch("http://localhost:7104/api/Log/clear", {
                method: "DELETE",
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                }
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({ message: "Loglar temizlenirken hata oluştu" }));
                throw new Error(errorData.message || "Loglar temizlenirken hata oluştu");
            }

            const result = await response.json();
            
            // Başarı mesajı göster
            if ((window as any).showToast) {
                (window as any).showToast.success(
                    "Başarılı",
                    `${result.deletedCount || 0} log kaydı başarıyla silindi.`
                );
            }
            
            // Logları yeniden yükle
            await loadLogs();
        } catch (error: any) {
            console.error("Log temizleme hatası:", error);
            if ((window as any).showToast) {
                (window as any).showToast.error(
                    "Hata",
                    error.message || "Loglar temizlenirken bir hata oluştu"
                );
            }
        }
    };

    const handleClearCancel = () => {
        setShowClearConfirm(false);
    };

    if (isLoading) {
        return (
            <div className="admin-logs-page">
                <div className="container">
                    <div className="loading-state">
                        <div className="loading-spinner"></div>
                        <p>Loglar yükleniyor...</p>
                    </div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="admin-logs-page">
                <div className="container">
                    <div className="error-state">
                        <h2>❌ Hata</h2>
                        <p>{error}</p>
                        <button onClick={loadLogs} className="btn btn-primary">
                            Tekrar Dene
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="admin-logs-page">
            <div className="container">
                {/* Header */}
                <div className="logs-header">
                    <Link to="/admin" className="back-button">
                        <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M10 12L6 8L10 4" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                        </svg>
                        <span>Admin Paneli</span>
                    </Link>
                    <div className="header-main">
                        <div className="header-icon">📊</div>
                        <div className="header-text-content">
                            <h1>Sistem Logları</h1>
                            <div className="log-count-badge">
                                <span className="count-number">{filteredLogs.length}</span>
                                <span className="count-label">log kaydı</span>
                            </div>
                        </div>
                    </div>
                    <div className="header-actions">
                        <button onClick={loadLogs} className="refresh-button">
                            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M1 4V10H7" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                                <path d="M23 20V14H17" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                                <path d="M20.49 9A9 9 0 0 0 5.64 5.64L1 10M23 14L18.36 18.36A9 9 0 0 1 3.51 15" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                            </svg>
                            <span>Yenile</span>
                        </button>
                        <button onClick={handleClearClick} className="btn btn-danger">
                            🗑️ Temizle
                        </button>
                    </div>
                </div>

                {/* Filters */}
                <div className="filters-section">
                    <div className="filter-group">
                        <label>Ara:</label>
                        <input
                            type="text"
                            placeholder="Mesaj, aksiyon veya kullanıcı ara..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="search-input"
                        />
                    </div>
                    <div className="filter-group">
                        <label>Level:</label>
                        <select
                            value={levelFilter}
                            onChange={(e) => setLevelFilter(e.target.value)}
                            className="filter-select"
                        >
                            <option value="all">Tümü</option>
                            <option value="Error">Error</option>
                            <option value="Warning">Warning</option>
                            <option value="Info">Info</option>
                            <option value="Debug">Debug</option>
                        </select>
                    </div>
                    <div className="filter-group">
                        <label>Sırala:</label>
                        <select
                            value={sortBy}
                            onChange={(e) => setSortBy(e.target.value)}
                            className="filter-select"
                        >
                            <option value="timestamp">Tarih</option>
                            <option value="level">Level</option>
                            <option value="message">Mesaj</option>
                            <option value="action">Aksiyon</option>
                        </select>
                    </div>
                    <div className="filter-group">
                        <label>Sıra:</label>
                        <select
                            value={sortOrder}
                            onChange={(e) => setSortOrder(e.target.value as "asc" | "desc")}
                            className="filter-select"
                        >
                            <option value="desc">Azalan</option>
                            <option value="asc">Artan</option>
                        </select>
                    </div>
                </div>

                {/* Logs List */}
                <div className="logs-list">
                    {filteredLogs.length === 0 ? (
                        <div className="empty-state">
                            <div className="empty-icon">📝</div>
                            <h3>Log bulunamadı</h3>
                            <p>Arama kriterlerinize uygun log kaydı bulunmuyor.</p>
                        </div>
                    ) : (
                        filteredLogs.map(log => (
                            <div key={log.id} className="log-card">
                                <div className="log-header">
                                    <div className="log-timestamp">
                                        {formatDate(log.timestamp || log.createdAt)}
                                    </div>
                                </div>

                                <div className="log-content">
                                    <div className="log-message">
                                        <strong>Aksiyon:</strong> {getActionText(log.action)}
                                    </div>

                                    {log.flightNumber && log.flightNumber !== "Flight Info Not Available" && (
                                        <div style={{ marginTop: '4px' }}>
                                            <div className="log-flight" style={{ margin: '0' }}>
                                                <strong>Uçuş:</strong> {log.flightNumber}
                                            </div>
                                        </div>
                                    )}

                                    {log.data && (
                                        <div className="log-details">
                                            <div className="log-details-content">
                                                {formatLogDetails(log.data, log.action, log.userName)}
                                            </div>
                                        </div>
                                    )}

                                    {log.exception && (
                                        <div className="log-exception">
                                            <strong>Hata Detayı:</strong>
                                            <pre className="log-exception-content">{log.exception}</pre>
                                        </div>
                                    )}
                                </div>

                                <div className="log-footer">
                                    <div className="log-relations">
                                        {log.userName && (
                                            <span className="relation-badge user">
                                                👤 {log.userName}
                                            </span>
                                        )}
                                        {log.flightNumber && log.flightNumber !== "Flight Info Not Available" && (
                                            <span className="relation-badge flight">
                                                ✈️ {log.flightNumber}
                                            </span>
                                        )}
                                    </div>
                                    <div className="log-id">
                                        #{log.id} • {formatTime(log.timestamp || log.createdAt)}
                                    </div>
                                </div>
                            </div>
                        ))
                    )}
                </div>

                {/* Confirmation Modal */}
                <ConfirmationModal
                    isOpen={showClearConfirm}
                    title="Logları Temizle"
                    message="Tüm logları silmek istediğinizden emin misiniz? Bu işlem geri alınamaz!"
                    confirmText="Evet, Temizle"
                    cancelText="İptal"
                    type="danger"
                    onConfirm={handleClearConfirm}
                    onCancel={handleClearCancel}
                />
            </div>
        </div>
    );
}

export default AdminLogsPage;

