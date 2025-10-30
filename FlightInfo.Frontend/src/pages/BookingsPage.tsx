/* eslint-disable @typescript-eslint/no-explicit-any */
import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { getUserReservationsWithFlights, cancelReservation, restoreReservation, type Reservation } from "../services/flightService";
import { useToast } from "../contexts/ToastContext";
import ConfirmDialog from "../components/ConfirmDialog";
import "./BookingsPage.css";

function BookingsPage() {
    const { showToast } = useToast();
    const [reservations, setReservations] = useState<Reservation[]>([]);
    const [filteredReservations, setFilteredReservations] = useState<Reservation[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");
    const [cancellingId, setCancellingId] = useState<number | null>(null);
    const [restoringId, setRestoringId] = useState<number | null>(null);
    const [activeFilter, setActiveFilter] = useState<"all" | "active" | "cancelled">("all");
    const [selectedReservation, setSelectedReservation] = useState<Reservation | null>(null);
    const [showDetailsModal, setShowDetailsModal] = useState(false);
    const [filterCounts, setFilterCounts] = useState({ active: 0, cancelled: 0 });

    // Para birimi sembolü fonksiyonu
    const getCurrencySymbol = (currency: string) => {
        switch (currency) {
            case "TRY": return "₺";
            case "USD": return "$";
            case "EUR": return "€";
            default: return currency;
        }
    };

    // Confirm Dialog States
    const [showCancelConfirm, setShowCancelConfirm] = useState(false);
    const [showRestoreConfirm, setShowRestoreConfirm] = useState(false);
    const [pendingReservationId, setPendingReservationId] = useState<number | null>(null);

    useEffect(() => {
        const loadReservations = async () => {
            try {
                setIsLoading(true);

                // Token kontrolü
                const token = localStorage.getItem("token");
                const user = localStorage.getItem("user");
                console.log("🔍 AUTH DEBUG:");
                console.log("Token:", token ? token.substring(0, 20) + "..." : "YOK");
                console.log("User:", user);

                if (!token) {
                    setError("Giriş yapmanız gerekiyor. Lütfen tekrar giriş yapın.");
                    return;
                }

                const userReservations = await getUserReservationsWithFlights();
                console.log("Rezervasyonlar yüklendi:", userReservations.length, "adet");
                setReservations(userReservations);

                // Varsayılan olarak aktif rezervasyonları göster
                const activeReservations = userReservations.filter(r =>
                    r.status === "Confirmed" || r.status === "confirmed"
                );
                setFilteredReservations(activeReservations);
        } catch (error: any) {
            console.error("Rezervasyonlar yüklenirken hata:", error);

            if (error.response?.status === 401) {
                setError("Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.");
                // Token'ı temizle
                localStorage.removeItem("token");
                localStorage.removeItem("user");
                // Navbar'ı güncelle
                window.dispatchEvent(new Event('authChange'));
            } else if (error.response?.status === 403) {
                setError("Bu sayfaya erişim yetkiniz bulunmuyor.");
            } else if (error.response?.status >= 500) {
                setError("Sunucu hatası. Lütfen daha sonra tekrar deneyin.");
            } else {
                setError("Rezervasyonlar yüklenirken bir hata oluştu. Lütfen sayfayı yenileyin.");
            }
            } finally {
                setIsLoading(false);
            }
        };

        loadReservations();

        // Rezervasyon yapıldığında sayfayı yenile
        const handleReservationUpdate = () => {
            loadReservations();
        };

        // Event listener'ları ekle
        window.addEventListener('reservationUpdated', handleReservationUpdate);
        window.addEventListener('authChange', handleReservationUpdate);

        return () => {
            // Cleanup: Event listener'ları temizle
            window.removeEventListener('reservationUpdated', handleReservationUpdate);
            window.removeEventListener('authChange', handleReservationUpdate);
        };
    }, []);

    // Rezervasyonlar değiştiğinde filtreleme sayılarını güncelle
    useEffect(() => {
        if (reservations.length > 0) {
            // Mevcut filtreye göre güncellenmiş rezervasyonları filtrele
            let filtered = reservations;
            if (activeFilter === "all") {
                filtered = reservations; // Tüm rezervasyonlar
            } else if (activeFilter === "active") {
                filtered = reservations.filter(r =>
                    r.status === "Confirmed" || r.status === "confirmed"
                );
            } else if (activeFilter === "cancelled") {
                filtered = reservations.filter(r =>
                    r.status === "Cancelled" || r.status === "cancelled"
                );
            }
            setFilteredReservations(filtered);

        // Debug için
        console.log("🔍 useEffect FİLTRELEME DEBUG:");
        console.log("Mevcut filtre:", activeFilter);
        console.log("Toplam rezervasyon:", reservations.length);
        console.log("Filtrelenmiş rezervasyon:", filtered.length);
        console.log("Aktif sayısı:", reservations.filter(r => r.status === "Confirmed" || r.status === "confirmed").length);
        console.log("İptal sayısı:", reservations.filter(r => r.status === "Cancelled" || r.status === "cancelled").length);

        // Her rezervasyonun durumunu detaylı göster
        reservations.forEach((reservation, index) => {
            console.log(`Rezervasyon ${index + 1} (ID: ${reservation.id}):`, {
                id: reservation.id,
                status: reservation.status,
                statusType: typeof reservation.status,
                statusValue: reservation.status
            });
        });

            // Filtre sayaçlarını güncelle
            setFilterCounts({
                active: reservations.filter(r => r.status === "Confirmed" || r.status === "confirmed").length,
                cancelled: reservations.filter(r => r.status === "Cancelled" || r.status === "cancelled").length
            });
        }
    }, [reservations, activeFilter]);

    // Filtreleme fonksiyonu
    const filterReservations = (filter: "all" | "active" | "cancelled") => {
        console.log("🔍 FİLTRELEME DEBUG:");
        console.log("Filtre:", filter);
        console.log("Toplam rezervasyon:", reservations.length);
        console.log("Rezervasyon status'ları:", reservations.map(r => ({ id: r.id, status: r.status })));

        setActiveFilter(filter);
        let filtered = reservations;

        if (filter === "all") {
            filtered = reservations; // Tüm rezervasyonlar
            console.log("Tüm filtre sonucu:", filtered.length, "rezervasyon");
        } else if (filter === "active") {
            filtered = reservations.filter(r =>
                r.status === "Confirmed" ||
                r.status === "confirmed"
            );
            console.log("Aktif filtre sonucu:", filtered.length, "rezervasyon");
        } else if (filter === "cancelled") {
            filtered = reservations.filter(r =>
                r.status === "Cancelled" ||
                r.status === "cancelled"
            );
            console.log("İptal filtre sonucu:", filtered.length, "rezervasyon");
        }

        setFilteredReservations(filtered);

        // Anlık güncelleme için debug
        console.log("🔍 ANLIK GÜNCELLEME DEBUG:");
        console.log("Filtrelenmiş rezervasyonlar:", filtered.length);
        console.log("Aktif rezervasyon sayısı:", reservations.filter(r => r.status === "Confirmed" || r.status === "confirmed").length);
        console.log("İptal edilen rezervasyon sayısı:", reservations.filter(r => r.status === "Cancelled" || r.status === "cancelled").length);

        // Force re-render için state güncelleme
        setActiveFilter(filter);
    };

    const getStatusColor = (status: string | number) => {
        if (status === "Confirmed" || status === "confirmed") {
            return "#27ae60"; // Confirmed
        } else if (status === "Cancelled" || status === "cancelled") {
            return "#e74c3c"; // Cancelled
        } else {
            return "#95a5a6"; // Unknown
        }
    };

    const getStatusText = (status: string | number) => {
        if (status === "Confirmed" || status === "confirmed") {
            return "Onaylandı";
        } else if (status === "Cancelled" || status === "cancelled") {
            return "İptal Edildi";
        } else {
            return "Bilinmiyor";
        }
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString('tr-TR', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    };

    const formatTime = (timeString: string) => {
        return new Date(timeString).toLocaleTimeString('tr-TR', {
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    const handleCancelReservation = (reservationId: number) => {
        setPendingReservationId(reservationId);
        setShowCancelConfirm(true);
    };

    const confirmCancelReservation = async () => {
        if (!pendingReservationId) return;

        try {
            setCancellingId(pendingReservationId);
            await cancelReservation(pendingReservationId);

            // ANLIK GÜNCELLEME: Rezervasyonu hemen iptal olarak işaretle
            setReservations(prev => prev.map(r =>
                r.id === pendingReservationId
                    ? { ...r, status: "Cancelled" as any }
                    : r
            ));

            // Filtreleme yap - mevcut filtreye göre
            let filtered = reservations.map(r =>
                r.id === pendingReservationId
                    ? { ...r, status: "Cancelled" as any }
                    : r
            );

            if (activeFilter === "active") {
                filtered = filtered.filter(r =>
                    r.status === "Confirmed" || r.status === "confirmed"
                );
            } else if (activeFilter === "cancelled") {
                filtered = filtered.filter(r =>
                    r.status === "Cancelled" || r.status === "cancelled"
                );
            }
            setFilteredReservations(filtered);

            // Filtre sayaçlarını güncelle ed
            const updatedCounts = {
                active: reservations.filter(r =>
                    r.id === pendingReservationId
                        ? false  // İptal edilen rezervasyon
                        : (r.status === "Confirmed" || r.status === "confirmed")
                ).length,
                cancelled: reservations.filter(r =>
                    r.id === pendingReservationId
                        ? true   // İptal edilen rezervasyon
                        : (r.status === "Cancelled" || r.status === "cancelled")
                ).length
            };
            setFilterCounts(updatedCounts);

            // Rezervasyonları yeniden yükle (arka planda)
            setTimeout(async () => {
                try {
                    const updatedReservations = await getUserReservationsWithFlights();
                    setReservations(updatedReservations);
                } catch (error) {
                    console.error("Rezervasyonları yeniden yükleme hatası:", error);
                }
            }, 1000);

            // Başarılı iptal - toast mesajı
            showToast("✅ Rezervasyon başarıyla iptal edildi!", "success", 2000);

            // Event dispatch ederek diğer bileşenleri bilgilendir
            window.dispatchEvent(new CustomEvent('reservationUpdated', {
                detail: { action: 'cancel', reservationId: pendingReservationId }
            }));
        } catch (error: any) {
            console.error("Rezervasyon iptal hatası:", error);

            // API'den gelen hata mesajını göster
            const errorMessage = error.response?.data?.error?.message || error.message || "Rezervasyon iptal edilirken bir hata oluştu";
            showToast(`❌ ${errorMessage}`, "error", 4000);
        } finally {
            setCancellingId(null);
            setShowCancelConfirm(false);
            setPendingReservationId(null);
        }
    };

    const handleRestoreReservation = (reservationId: number) => {
        setPendingReservationId(reservationId);
        setShowRestoreConfirm(true);
    };

    const confirmRestoreReservation = async () => {
        if (!pendingReservationId) return;

        try {
            setRestoringId(pendingReservationId);

            // Rezervasyon durumunu kontrol et
            const reservation = reservations.find(r => r.id === pendingReservationId);
            if (reservation) {
                console.log("🔄 RESTORE DEBUG: Rezervasyon durumu:", {
                    id: reservation.id,
                    status: reservation.status,
                    statusType: typeof reservation.status,
                    isCancelled: (typeof reservation.status === 'number' && reservation.status === 2) ||
                                (typeof reservation.status === 'string' && (reservation.status === "Cancelled" || reservation.status === "cancelled"))
                });
            }

            // Gerçek API çağrısı
            await restoreReservation(pendingReservationId);

            // ANLIK GÜNCELLEME: Rezervasyonu hemen aktif olarak işaretle
            setReservations(prev => prev.map(r =>
                r.id === pendingReservationId
                    ? { ...r, status: "Confirmed" as any }
                    : r
            ));

            // Filtreleme yap - mevcut filtreye göre
            let filtered = reservations.map(r =>
                r.id === pendingReservationId
                    ? { ...r, status: "Confirmed" as any }
                    : r
            );

            if (activeFilter === "active") {
                filtered = filtered.filter(r =>
                    r.status === "Confirmed" || r.status === "confirmed"
                );
            } else if (activeFilter === "cancelled") {
                filtered = filtered.filter(r =>
                    r.status === "Cancelled" || r.status === "cancelled"
                );
            }
            setFilteredReservations(filtered);

            // Filtre sayaçlarını güncelle
            const updatedCounts = {
                active: reservations.filter(r =>
                    r.id === pendingReservationId
                        ? true   // Geri alınan rezervasyon
                        : (r.status === "Confirmed" || r.status === "confirmed")
                ).length,
                cancelled: reservations.filter(r =>
                    r.id === pendingReservationId
                        ? false  // Geri alınan rezervasyon
                        : (r.status === "Cancelled" || r.status === "cancelled")
                ).length
            };
            setFilterCounts(updatedCounts);

            // Rezervasyonları yeniden yükle (arka planda)
            setTimeout(async () => {
                try {
                    const updatedReservations = await getUserReservationsWithFlights();
                    setReservations(updatedReservations);
                } catch (error) {
                    console.error("Rezervasyonları yeniden yükleme hatası:", error);
                }
            }, 1000);

            // Başarılı geri alma - toast mesajı
            showToast("🔄 Rezervasyon başarıyla geri alındı!", "success", 2000);

            // Event dispatch ederek diğer bileşenleri bilgilendir
            window.dispatchEvent(new CustomEvent('reservationUpdated', {
                detail: { action: 'restore', reservationId: pendingReservationId }
            }));
        } catch (error: any) {
            console.error("Rezervasyon geri alma hatası:", error);

            // API'den gelen hata mesajını göster
            const errorMessage = error.response?.data?.error?.message || error.message || "Rezervasyon geri alınırken bir hata oluştu";
            showToast(`❌ ${errorMessage}`, "error", 4000);
        } finally {
            setRestoringId(null);
            setShowRestoreConfirm(false);
            setPendingReservationId(null);
        }
    };

    const handleShowDetails = (reservation: Reservation) => {
        setSelectedReservation(reservation);
        setShowDetailsModal(true);
    };

    if (isLoading) {
        return (
            <div className="bookings-page">
                <div className="container">
                    <div className="bookings-header">
                        <h1>Rezervasyonlarım</h1>
                        <p>Rezervasyonlarınız yükleniyor...</p>
                    </div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="bookings-page">
                <div className="container">
                    <div className="bookings-header">
                        <h1>Rezervasyonlarım</h1>
                        <div className="error-message">
                            <p>{error}</p>
                            <div className="error-actions">
                                <button onClick={() => window.location.reload()} className="btn btn-primary">
                                    Tekrar Dene
                                </button>
                                {error.includes("giriş") && (
                                    <Link to="/login" className="btn btn-secondary">
                                        Giriş Yap
                                    </Link>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <>
        <div className="bookings-page">
            <div className="container">
                  {/* Header */}
                  <div className="bookings-header">
                      <h1>Rezervasyonlarım</h1>
                      <p>Geçmiş ve aktif rezervasyonlarınız</p>
                  </div>


                {/* Filtreleme Butonları */}
                <div className="filter-tabs">
                    <button
                        className={`filter-tab ${activeFilter === "all" ? "active" : ""}`}
                        onClick={() => filterReservations("all")}
                    >
                        <div className="tab-content">
                            <span className="tab-icon">📋</span>
                            <div className="tab-text">
                                <span className="tab-title">Tümü</span>
                                <span className="tab-count">{reservations.length} rezervasyon</span>
                            </div>
                        </div>
                    </button>
                    <button
                        className={`filter-tab ${activeFilter === "active" ? "active" : ""}`}
                        onClick={() => filterReservations("active")}
                    >
                        <div className="tab-content">
                            <span className="tab-icon">✅</span>
                            <div className="tab-text">
                                <span className="tab-title">Aktif</span>
                                <span className="tab-count">{filterCounts.active} rezervasyon</span>
                            </div>
                        </div>
                    </button>
                    <button
                        className={`filter-tab ${activeFilter === "cancelled" ? "active" : ""}`}
                        onClick={() => filterReservations("cancelled")}
                    >
                        <div className="tab-content">
                            <span className="tab-icon">❌</span>
                            <div className="tab-text">
                                <span className="tab-title">İptal Edilen</span>
                                <span className="tab-count">{filterCounts.cancelled} rezervasyon</span>
                            </div>
                        </div>
                    </button>
                </div>


                {/* Reservations List */}
                {filteredReservations.length === 0 ? (
                    <div className="empty-state">
                        <div className="empty-icon">📋</div>
                        <h3>
                            {activeFilter === "all" ? "Henüz rezervasyonunuz yok" :
                             activeFilter === "active" ? "Aktif rezervasyonunuz yok" :
                             "İptal edilen rezervasyonunuz yok"}
                        </h3>
                        <p>
                            {activeFilter === "all" ? "İlk rezervasyonunuzu yapmak için uçuş aramaya başlayın" :
                             activeFilter === "active" ? "Aktif rezervasyonlarınız burada görünecek" :
                             "İptal ettiğiniz rezervasyonlar burada görünecek"}
                        </p>
                        {activeFilter === "all" && (
                            <Link to="/" className="btn btn-primary">Uçuş Ara</Link>
                        )}
                    </div>
                ) : (
                    <div className="reservations-list">
                        {filteredReservations.map(reservation => (
                            <div key={reservation.id} className={`reservation-card ${cancellingId === reservation.id ? 'updating' : ''} ${restoringId === reservation.id ? 'updating' : ''}`}>
                                <div className="reservation-header">
                                    <div className="flight-info">
                                        <h3>{reservation.flightNumber || 'Bilinmiyor'}</h3>
                                        <p>{reservation.origin || 'Bilinmiyor'} → {reservation.destination || 'Bilinmiyor'}</p>
                                    </div>
                                    <div
                                        className="status-badge"
                                        style={{ backgroundColor: getStatusColor(reservation.status) }}
                                        title={getStatusText(reservation.status)}
                                    >
                                        {getStatusText(reservation.status)}
                                    </div>
                                </div>

                                {/* Progress indicator for updating reservations */}
                                {(cancellingId === reservation.id || restoringId === reservation.id) && (
                                    <div className="update-progress">
                                        <div className="progress-bar">
                                            <div className="progress-fill"></div>
                                        </div>
                                        <span className="progress-text">
                                            {cancellingId === reservation.id ? "İptal ediliyor..." : "Geri alınıyor..."}
                                        </span>
                                    </div>
                                )}

                                <div className="reservation-details">
                                    {/* Uçuş Zamanları - Daha büyük ve belirgin */}
                                    <div className="flight-times">
                                        <div className="time-section">
                                            <div className="time-info">
                                                <span className="time-label">Kalkış</span>
                                                <span className="time-value">{reservation.departureTime ? formatTime(reservation.departureTime) : '--:--'}</span>
                                                <span className="date-value">{reservation.departureTime ? formatDate(reservation.departureTime) : 'Bilinmiyor'}</span>
                                            </div>
                                        </div>
                                        <div className="time-section">
                                            <div className="time-info">
                                                <span className="time-label">Varış</span>
                                                <span className="time-value">{reservation.arrivalTime ? formatTime(reservation.arrivalTime) : '--:--'}</span>
                                                <span className="date-value">{reservation.arrivalTime ? formatDate(reservation.arrivalTime) : 'Bilinmiyor'}</span>
                                            </div>
                                        </div>
                                    </div>

                                    {/* Ek Bilgiler - Daha kompakt */}
                                    <div className="additional-info">
                                        <div className="info-item">
                                            <span className="info-label">Rezervasyon:</span>
                                            <span className="info-value">{formatDate(reservation.createdAt)}</span>
                                        </div>
                                        {reservation.cancelledAt && (
                                            <div className="info-item">
                                                <span className="info-label">İptal:</span>
                                                <span className="info-value">{formatDate(reservation.cancelledAt)}</span>
                                            </div>
                                        )}
                                        {reservation.seatNumber && (
                                            <div className="info-item">
                                                <span className="info-label">Koltuk:</span>
                                                <span className="info-value">{reservation.seatNumber}</span>
                                            </div>
                                        )}
                                        {reservation.class && (
                                            <div className="info-item">
                                                <span className="info-label">Sınıf:</span>
                                                <span className="info-value">{reservation.class}</span>
                                            </div>
                                        )}
                                    </div>
                                </div>

                                <div className="reservation-actions">
                                    <button
                                        className="btn btn-secondary"
                                        onClick={() => handleShowDetails(reservation)}
                                    >
                                        Detaylar
                                    </button>
                                    {(reservation.status === 1 || reservation.status === "Active" || reservation.status === "active" || reservation.status === "Confirmed" || reservation.status === "confirmed") && (
                                        <button
                                            className={`btn btn-danger ${cancellingId === reservation.id ? 'loading' : ''}`}
                                            onClick={() => handleCancelReservation(reservation.id)}
                                            disabled={cancellingId === reservation.id}
                                        >
                                            {cancellingId === reservation.id ? (
                                                <>
                                                    <span className="spinner"></span>
                                                    İptal Ediliyor...
                                                </>
                                            ) : (
                                                "İptal Et"
                                            )}
                                        </button>
                                    )}
                                    {((typeof reservation.status === 'number' && reservation.status === 2) ||
                          (typeof reservation.status === 'string' && (reservation.status === "Cancelled" || reservation.status === "cancelled"))) && (
                                        <button
                                            className={`btn btn-success ${restoringId === reservation.id ? 'loading' : ''}`}
                                            onClick={() => handleRestoreReservation(reservation.id)}
                                            disabled={restoringId === reservation.id}
                                        >
                                            {restoringId === reservation.id ? (
                                                <>
                                                    <span className="spinner"></span>
                                                    Geri Alınıyor...
                                                </>
                                            ) : (
                                                "Geri Al"
                                            )}
                                        </button>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>
                )}

                {/* Rezervasyon Detayları Modal */}
                {showDetailsModal && selectedReservation && (
                    <div className="modal-overlay" onClick={() => setShowDetailsModal(false)}>
                        <div className="modal-content compact-reservation-modal" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <h2>Rezervasyon Detayları</h2>
                            </div>

                            <div className="modal-body">
                                <div className="reservation-detail-card">
                                    <div className="detail-header">
                                        <h3>{selectedReservation.flightNumber || 'Bilinmiyor'}</h3>
                                        <p>{selectedReservation.origin || 'Bilinmiyor'} → {selectedReservation.destination || 'Bilinmiyor'}</p>
                                    </div>

                                    <div className="detail-sections">
                                        <div className="detail-section">
                                            <h4>Uçuş Bilgileri</h4>
                                            <div className="detail-grid">
                                                <div className="detail-item">
                                                    <span className="label">Uçuş Numarası:</span>
                                                    <span className="value">{selectedReservation.flightNumber || 'Bilinmiyor'}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Kalkış:</span>
                                                    <span className="value">{selectedReservation.origin || 'Bilinmiyor'}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Varış:</span>
                                                    <span className="value">{selectedReservation.destination || 'Bilinmiyor'}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Kalkış Saati:</span>
                                                    <span className="value">
                                                        {selectedReservation.departureTime ? formatTime(selectedReservation.departureTime) : 'Bilinmiyor'}
                                                    </span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Varış Saati:</span>
                                                    <span className="value">
                                                        {selectedReservation.arrivalTime ? formatTime(selectedReservation.arrivalTime) : 'Bilinmiyor'}
                                                    </span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Uçuş Tarihi:</span>
                                                    <span className="value">
                                                        {selectedReservation.departureTime ? formatDate(selectedReservation.departureTime) : 'Bilinmiyor'}
                                                    </span>
                                                </div>
                                            </div>
                                        </div>

                                        <div className="detail-section">
                                            <h4>Rezervasyon Bilgileri</h4>
                                            <div className="detail-grid">
                                                <div className="detail-item">
                                                    <span className="label">Rezervasyon ID:</span>
                                                    <span className="value">#{selectedReservation.id}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Durum:</span>
                                                    <span className="value">{getStatusText(selectedReservation.status)}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Oluşturulma Tarihi:</span>
                                                    <span className="value">{formatDate(selectedReservation.createdAt)}</span>
                                                </div>
                                                {selectedReservation.cancelledAt && (
                                                    <div className="detail-item">
                                                        <span className="label">İptal Tarihi:</span>
                                                        <span className="value">{formatDate(selectedReservation.cancelledAt)}</span>
                                                    </div>
                                                )}
                                            </div>
                                        </div>

                                        <div className="detail-section">
                                            <h4>Yolcu Bilgileri</h4>
                                            <div className="detail-grid">
                                                <div className="detail-item">
                                                    <span className="label">Yolcu Adı:</span>
                                                    <span className="value">{selectedReservation.passengerName || 'Bilinmiyor'}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">E-posta:</span>
                                                    <span className="value">{selectedReservation.passengerEmail || 'Bilinmiyor'}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Telefon:</span>
                                                    <span className="value">{selectedReservation.passengerPhone || 'Bilinmiyor'}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Koltuk:</span>
                                                    <span className="value">{selectedReservation.seatNumber || 'Bilinmiyor'}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Sınıf:</span>
                                                    <span className="value">{selectedReservation.class || 'Bilinmiyor'}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Toplam Fiyat:</span>
                                                    <span className="value">{selectedReservation.totalPrice ? `${selectedReservation.totalPrice} ${getCurrencySymbol(selectedReservation.currency || 'TRY')}` : 'Bilinmiyor'}</span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div className="modal-footer">
                                <button
                                    className="btn btn-secondary"
                                    onClick={() => setShowDetailsModal(false)}
                                >
                                    Kapat
                                </button>
                                {(selectedReservation.status === "Confirmed" || selectedReservation.status === "confirmed") && (
                                    <button
                                        className="btn btn-danger"
                                        onClick={() => {
                                            setShowDetailsModal(false);
                                            handleCancelReservation(selectedReservation.id);
                                        }}
                                        disabled={cancellingId === selectedReservation.id}
                                    >
                                        {cancellingId === selectedReservation.id ? "İptal Ediliyor..." : "İptal Et"}
                                    </button>
                                )}
                                {(selectedReservation.status === "Cancelled" || selectedReservation.status === "cancelled") && (
                                    <button
                                        className="btn btn-success"
                                        onClick={() => {
                                            setShowDetailsModal(false);
                                            handleRestoreReservation(selectedReservation.id);
                                        }}
                                        disabled={restoringId === selectedReservation.id}
                                    >
                                        {restoringId === selectedReservation.id ? "Geri Alınıyor..." : "Geri Al"}
                                    </button>
                                )}
                            </div>
                        </div>
                    </div>
                )}

            </div>
        </div>

        {/* Cancel Confirmation Dialog */}
        <ConfirmDialog
            isOpen={showCancelConfirm}
            title="Rezervasyonu İptal Et"
            message="Bu rezervasyonu iptal etmek istediğinizden emin misiniz? Bu işlem geri alınamaz."
            confirmText="Evet, İptal Et"
            cancelText="Hayır"
            onConfirm={confirmCancelReservation}
            onCancel={() => {
                setShowCancelConfirm(false);
                setPendingReservationId(null);
            }}
            type="warning"
        />

        {/* Restore Confirmation Dialog */}
        <ConfirmDialog
            isOpen={showRestoreConfirm}
            title="Rezervasyonu Geri Al"
            message="Bu rezervasyonu geri almak istediğinizden emin misiniz? Rezervasyon tekrar aktif hale gelecektir."
            confirmText="Evet, Geri Al"
            cancelText="Hayır"
            onConfirm={confirmRestoreReservation}
            onCancel={() => {
                setShowRestoreConfirm(false);
                setPendingReservationId(null);
            }}
            type="info"
        />
        </>
    );
}

export default BookingsPage;

