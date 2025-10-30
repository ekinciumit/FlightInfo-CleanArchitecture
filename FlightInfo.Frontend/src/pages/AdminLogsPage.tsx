import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
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

    const getLevelColor = (level: string) => {
        if (!level) return "#2c3e50";
        switch (level.toLowerCase()) {
            case "error":
                return "#e74c3c";
            case "warning":
                return "#f39c12";
            case "info":
                return "#3498db";
            case "debug":
                return "#95a5a6";
            default:
                return "#2c3e50";
        }
    };

    const getLevelIcon = (level: string) => {
        if (!level) return "📝";
        switch (level.toLowerCase()) {
            case "error":
                return "❌";
            case "warning":
                return "⚠️";
            case "info":
                return "ℹ️";
            case "debug":
                return "🔍";
            default:
                return "📝";
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

    const clearLogs = async () => {
        if (window.confirm("Tüm logları silmek istediğinizden emin misiniz? Bu işlem geri alınamaz!")) {
            try {
                // Bu endpoint mevcut değil, sadece UI'da gösterelim
                console.log("Log temizleme işlemi - Backend endpoint gerekli");
                alert("Log temizleme özelliği henüz backend'de mevcut değil");
            } catch (error) {
                console.error("Log temizleme hatası:", error);
            }
        }
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
                    <div className="header-left">
                        <Link to="/admin" className="back-button">
                            ← Admin Paneli
                        </Link>
                        <h1>📊 Sistem Logları</h1>
                        <p>Toplam {filteredLogs.length} log kaydı</p>
                    </div>
                    <div className="header-actions">
                        <button onClick={loadLogs} className="btn btn-secondary">
                            🔄 Yenile
                        </button>
                        <button onClick={clearLogs} className="btn btn-danger">
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
                                    <div className="log-level">
                                        <span
                                            className="level-badge"
                                            style={{ backgroundColor: getLevelColor(log.level) }}
                                        >
                                            {getLevelIcon(log.level)} {log.level}
                                        </span>
                                    </div>
                                    <div className="log-timestamp">
                                        {formatDate(log.timestamp || log.createdAt)}
                                    </div>
                                </div>

                                <div className="log-content">
                                    <div className="log-message">
                                        <strong>Aksiyon:</strong> {log.action || log.message || 'N/A'}
                                    </div>

                                    <div style={{ display: 'flex', gap: '8px', flexWrap: 'wrap', marginTop: '4px' }}>
                                        {log.userName && (
                                            <div className="log-user" style={{ margin: '0', flex: '1', minWidth: '120px' }}>
                                                <strong>Kullanıcı:</strong> {log.userName}
                                            </div>
                                        )}

                                        {log.flightNumber && log.flightNumber !== "Flight Info Not Available" && (
                                            <div className="log-flight" style={{ margin: '0', flex: '1', minWidth: '120px' }}>
                                                <strong>Uçuş:</strong> {log.flightNumber}
                                            </div>
                                        )}
                                    </div>

                                    {log.data && (
                                        <div className="log-data">
                                            <strong>Detaylar:</strong>
                                            <pre className="log-data-content">{(() => {
                                                try {
                                                    return JSON.stringify(JSON.parse(log.data), null, 2);
                                                } catch {
                                                    return log.data;
                                                }
                                            })()}</pre>
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
            </div>
        </div>
    );
}

export default AdminLogsPage;

