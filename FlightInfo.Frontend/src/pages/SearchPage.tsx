/* eslint-disable @typescript-eslint/no-explicit-any */
import { useState, useEffect, useRef } from "react";
import { getAllFlightsWithPrices, createReservation, type FlightWithPrice, type FlightPrice } from "../services/flightService";
import ConfirmationModal from "../components/ConfirmationModal";
import { getAirlineLogo, getAirlineName } from "../utils/airlineUtils";
import "./SearchPage.css";

function SearchPage() {
    const [flights, setFlights] = useState<FlightWithPrice[]>([]);
    const [filteredFlights, setFilteredFlights] = useState<FlightWithPrice[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");
    const [searchTerm, setSearchTerm] = useState("");
    const [sortBy, setSortBy] = useState<"departure" | "arrival" | "status" | "price" | "duration" | "flightNumber" | "">("price");
    const [statusFilter, setStatusFilter] = useState<string>("all");
    const [priceRange, setPriceRange] = useState<{ min: number; max: number }>({ min: 0, max: 10000 });
    const [durationFilter, setDurationFilter] = useState<{ min: number; max: number }>({ min: 0, max: 24 });
    const [airlineFilter, setAirlineFilter] = useState<string>("all");
    const [classFilter, setClassFilter] = useState<string>("all");
    const [departureTimeFilter, setDepartureTimeFilter] = useState<string>("all");
    const [selectedFlight, setSelectedFlight] = useState<FlightWithPrice | null>(null);
    const [showDetailsModal, setShowDetailsModal] = useState(false);
    const [showPricesModal, setShowPricesModal] = useState(false);
    const [reservingFlights, setReservingFlights] = useState<Set<string>>(new Set());
    const [showConfirmModal, setShowConfirmModal] = useState(false);
    const [flightToReserve, setFlightToReserve] = useState<FlightWithPrice | null>(null);
    const [selectedPrice, setSelectedPrice] = useState<FlightPrice | null>(null);
    const [seatNumber] = useState('12A');
    const [showFilters, setShowFilters] = useState(false);
    const filterBtnRef = useRef<HTMLButtonElement | null>(null);
    const filterPopoverRef = useRef<HTMLDivElement | null>(null);
    // Pending filter values edited in panel
    const [pendingSortBy, setPendingSortBy] = useState<typeof sortBy>(sortBy);
    const [pendingStatus, setPendingStatus] = useState<string>(statusFilter);
    const [pendingAirline, setPendingAirline] = useState<string>(airlineFilter);
    const [pendingClass, setPendingClass] = useState<string>(classFilter);
    const [pendingDeparture, setPendingDeparture] = useState<string>(departureTimeFilter);
    const [pendingDurationMax, setPendingDurationMax] = useState<number>(durationFilter.max);

    useEffect(() => {
        const loadFlights = async () => {
            try {
                setIsLoading(true);
                const allFlights = await getAllFlightsWithPrices();
                const flightsWithDuration = allFlights.map(flight => ({
                    ...flight,
                    duration: Math.round((new Date(flight.arrivalTime).getTime() - new Date(flight.departureTime).getTime()) / (1000 * 60))
                }));
                setFlights(flightsWithDuration);
                setFilteredFlights(flightsWithDuration);
            } catch (error) {
                console.error("Uçuşlar yüklenirken hata:", error);
                setError("Uçuşlar yüklenirken bir hata oluştu");
            } finally {
                setIsLoading(false);
            }
        };

        loadFlights();
    }, []);

    // Close filters when clicking outside of icon or popover
    useEffect(() => {
        const handleClickOutside = (e: MouseEvent) => {
            if (!showFilters) return;
            const target = e.target as Node;
            const clickedInsidePopover = filterPopoverRef.current?.contains(target);
            const clickedOnButton = filterBtnRef.current?.contains(target);
            if (!clickedInsidePopover && !clickedOnButton) {
                setShowFilters(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, [showFilters]);

    useEffect(() => {
        let filtered = flights;

        if (searchTerm && searchTerm.trim()) {
            const searchLower = searchTerm.toLowerCase().trim();
            const searchWords = searchLower.includes(' ') ?
                searchLower.split(' ').filter(word => word.length > 0) : [searchLower];

            filtered = filtered.filter(flight => {
                return searchWords.every(word => {
                    const origin = flight.origin.toLowerCase();
                    const destination = flight.destination.toLowerCase();
                    const flightNumber = flight.flightNumber.toLowerCase();

                    return origin.includes(word) ||
                           destination.includes(word) ||
                           flightNumber.includes(word) ||
                           origin.startsWith(word) ||
                           destination.startsWith(word) ||
                           flightNumber.startsWith(word);
                });
            });
        }

        if (statusFilter !== "all") {
            const filterStatusLower = statusFilter.toLowerCase();
            filtered = filtered.filter(flight => {
                const flightStatus = flight.status.toLowerCase();
                return flightStatus === filterStatusLower ||
                       flightStatus.includes(filterStatusLower);
            });
        }

        if (priceRange.min > 0 || priceRange.max < 10000) {
            filtered = filtered.filter(flight => {
                if (!flight.prices || flight.prices.length === 0) return false;

                let minPrice = flight.prices[0].price;
                for (let i = 1; i < flight.prices.length; i++) {
                    if (flight.prices[i].price < minPrice) {
                        minPrice = flight.prices[i].price;
                    }
                }

                return minPrice >= priceRange.min && minPrice <= priceRange.max;
            });
        }

        if (durationFilter.min > 0 || durationFilter.max < 24) {
            filtered = filtered.filter(flight => {
                const durationHours = (flight.duration || 0) / 60;
                return durationHours >= durationFilter.min && durationHours <= durationFilter.max;
            });
        }

        // Havayolu filtresi
        if (airlineFilter !== "all") {
            filtered = filtered.filter(flight => {
                const airline = getAirlineName(flight.flightNumber);
                return airline.toLowerCase().includes(airlineFilter.toLowerCase());
            });
        }

        // Sınıf filtresi
        if (classFilter !== "all") {
            filtered = filtered.filter(flight => {
                if (!flight.prices || flight.prices.length === 0) return false;
                return flight.prices.some(price =>
                    price.class.toLowerCase() === classFilter.toLowerCase()
                );
            });
        }

        // Kalkış saati filtresi
        if (departureTimeFilter !== "all") {
            filtered = filtered.filter(flight => {
                const hour = new Date(flight.departureTime).getHours();
                switch (departureTimeFilter) {
                    case "morning": return hour >= 6 && hour < 12;
                    case "afternoon": return hour >= 12 && hour < 18;
                    case "evening": return hour >= 18 && hour < 24;
                    case "night": return hour >= 0 && hour < 6;
                    default: return true;
                }
            });
        }

        filtered.sort((a, b) => {
            let comparison = 0;

            switch (sortBy) {
                case "departure":
                    comparison = new Date(a.departureTime).getTime() - new Date(b.departureTime).getTime();
                    break;
                case "arrival":
                    comparison = new Date(a.arrivalTime).getTime() - new Date(b.arrivalTime).getTime();
                    break;
                case "price":
                    const aMinPrice = Math.min(...a.prices.map(p => p.price));
                    const bMinPrice = Math.min(...b.prices.map(p => p.price));
                    comparison = aMinPrice - bMinPrice;
                    break;
                case "duration":
                    comparison = (a.duration || 0) - (b.duration || 0);
                    break;
                case "flightNumber":
                    comparison = a.flightNumber.localeCompare(b.flightNumber);
                    break;
                case "status":
                    const statusOrder = { "Scheduled": 0, "OnTime": 1, "Boarding": 2, "InProgress": 3, "Delayed": 4, "Cancelled": 5, "Completed": 6 };
                    comparison = (statusOrder[a.status as keyof typeof statusOrder] || 99) - (statusOrder[b.status as keyof typeof statusOrder] || 99);
                    break;
                default:
                    comparison = 0;
            }

            return comparison;
        });

        setFilteredFlights(filtered);
    }, [flights, searchTerm, sortBy, statusFilter, priceRange, durationFilter, airlineFilter, classFilter, departureTimeFilter]);

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

    const getStatusColor = (status: string) => {
        switch (status) {
            case "OnTime": return "#27ae60";
            case "Delayed": return "#f39c12";
            case "Cancelled": return "#e74c3c";
            case "Boarding": return "#3498db";
            case "Scheduled": return "#9b59b6";
            case "Completed": return "#2ecc71";
            case "InProgress": return "#e67e22";
            default: return "#95a5a6";
        }
    };

    const getStatusText = (status: string) => {
        switch (status) {
            case "OnTime": return "Zamanında";
            case "Delayed": return "Gecikmeli";
            case "Cancelled": return "İptal";
            case "Boarding": return "Biniş";
            case "Scheduled": return "Planlandı";
            case "Completed": return "Tamamlandı";
            case "InProgress": return "Devam Ediyor";
            default: return "Bilinmiyor";
        }
    };

    const handleShowDetails = (flight: FlightWithPrice) => {
        setSelectedFlight(flight);
        setShowDetailsModal(true);
    };

    const handleShowPrices = (flight: FlightWithPrice) => {
        setSelectedFlight(flight);
        setShowPricesModal(true);
    };

    const handleReserveFlight = (flight: FlightWithPrice, price: FlightPrice) => {
        if (!localStorage.getItem("token")) {
            (window as any).showToast.error(
                "Giriş Gerekli",
                "Rezervasyon yapmak için giriş yapmanız gerekiyor!"
            );
            return;
        }

        if (flight.status === "Cancelled") {
            (window as any).showToast.warning(
                "Rezervasyon Yapılamaz",
                "İptal edilmiş uçuşu rezerve edemezsiniz!"
            );
            return;
        }

        setFlightToReserve(flight);
        setSelectedPrice(price);
        setShowConfirmModal(true);
    };

    const confirmReservation = async () => {
        if (!flightToReserve || !selectedPrice) {
            (window as any).showToast.error(
                "Rezervasyon Hatası",
                "Fiyat seçeneği bulunamadı. Lütfen tekrar deneyin."
            );
            return;
        }

        try {
            setReservingFlights(prev => new Set(prev).add(flightToReserve.flightNumber));
            setShowConfirmModal(false);

            await createReservation(
                flightToReserve.id,
                selectedPrice.id,
                seatNumber
            );

            setReservingFlights(prev => {
                const newSet = new Set(prev);
                newSet.delete(flightToReserve.flightNumber);
                return newSet;
            });

            (window as any).showToast.success(
                "Rezervasyon Başarılı",
                `${flightToReserve.flightNumber} uçuşu başarıyla rezerve edildi!`
            );

            window.dispatchEvent(new CustomEvent('reservationUpdated'));
        } catch (error) {
            console.error("Rezervasyon hatası:", error);
            setReservingFlights(prev => {
                const newSet = new Set(prev);
                newSet.delete(flightToReserve.flightNumber);
                return newSet;
            });
            (window as any).showToast.error(
                "Rezervasyon Hatası",
                "Rezervasyon sırasında bir hata oluştu!"
            );
        } finally {
            setFlightToReserve(null);
        }
    };

    if (isLoading) {
        return (
            <div className="search-page">
                <div className="container">
                    <div className="loading-state">
                        <div className="loading-spinner"></div>
                        <h2>Uçuşlar yükleniyor...</h2>
                        <p>Tüm uçuş bilgileri getiriliyor</p>
                    </div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="search-page">
                <div className="container">
                    <div className="error-state">
                        <div className="error-icon">⚠️</div>
                        <h2>Hata Oluştu</h2>
                        <p>{error}</p>
                        <button onClick={() => window.location.reload()} className="btn btn-primary">
                            Tekrar Dene
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="search-page">
            <div className="container">
                <div className="search-filters-section">
                    <div className="search-box">
                        <input
                            type="text"
                            placeholder="Uçuş numarası, şehir veya havalimanı ara..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="search-search-input"
                        />
                        <button
                            ref={filterBtnRef}
                            className="filter-toggle-btn"
                            title="Filtreleri Aç/Kapat"
                            onClick={() => {
                                setPendingSortBy(sortBy);
                                setPendingStatus(statusFilter);
                                setPendingAirline(airlineFilter);
                                setPendingClass(classFilter);
                                setPendingDeparture(departureTimeFilter);
                                setPendingDurationMax(durationFilter.max);
                                setShowFilters(prev => !prev);
                            }}
                        >
                            <svg
                                width="22"
                                height="22"
                                viewBox="0 0 24 24"
                                fill="none"
                                xmlns="http://www.w3.org/2000/svg"
                                aria-hidden="true"
                                focusable="false"
                            >
                                <path
                                    d="M3 4h18l-6.8 7.4c-.13.14-.2.33-.2.53V20l-4-2v-6.07c0-.2-.07-.39-.2-.53L3 4z"
                                    stroke="currentColor"
                                    strokeWidth="2"
                                    fill="none"
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                />
                            </svg>
                        </button>
                    </div>
                    {showFilters && (
                    <div ref={filterPopoverRef} className="filters-popover">
                      <div className="filters-panel">
                        <div className="filter-group">
                            <label className="filter-label">Sırala</label>
                            <select
                                value={pendingSortBy}
                                onChange={(e) => setPendingSortBy(e.target.value as "departure" | "arrival" | "status" | "price" | "duration" | "flightNumber")}
                                className="filter-select"
                            >
                                <option value="price">💰 En Ucuz</option>
                                <option value="departure">🕐 Erken Kalkış</option>
                                <option value="arrival">🛬 Erken Varış</option>
                                <option value="duration">⚡ En Kısa Süre</option>
                                <option value="flightNumber">🔤 Alfabetik</option>
                                <option value="status">✅ Durum</option>
                            </select>
                        </div>

                        <div className="filter-group">
                            <label className="filter-label">Durum</label>
                            <select
                                value={pendingStatus}
                                onChange={(e) => setPendingStatus(e.target.value)}
                                className="filter-select"
                            >
                                <option value="all">Tümü</option>
                                <option value="Scheduled">📅 Planlandı</option>
                                <option value="Boarding">🚶 Biniş</option>
                                <option value="Delayed">⏰ Gecikmeli</option>
                                <option value="Cancelled">❌ İptal</option>
                            </select>
                        </div>

                        <div className="filter-group">
                            <label className="filter-label">Havayolu</label>
                            <select
                                value={pendingAirline}
                                onChange={(e) => setPendingAirline(e.target.value)}
                                className="filter-select"
                            >
                                <option value="all">Tümü</option>
                                <option value="Turkish Airlines">🇹🇷 Turkish Airlines</option>
                                <option value="Pegasus">🦅 Pegasus</option>
                                <option value="SunExpress">☀️ SunExpress</option>
                            </select>
                        </div>

                        <div className="filter-group">
                            <label className="filter-label">Sınıf</label>
                            <select
                                value={pendingClass}
                                onChange={(e) => setPendingClass(e.target.value)}
                                className="filter-select"
                            >
                                <option value="all">Tümü</option>
                                <option value="Economy">💺 Economy</option>
                                <option value="Business">🛋️ Business</option>
                                <option value="First">👑 First</option>
                            </select>
                        </div>

                        <div className="filter-group">
                            <label className="filter-label">Kalkış Saati</label>
                            <select
                                value={pendingDeparture}
                                onChange={(e) => setPendingDeparture(e.target.value)}
                                className="filter-select"
                            >
                                <option value="all">Tümü</option>
                                <option value="morning">🌅 Sabah (06:00-12:00)</option>
                                <option value="afternoon">☀️ Öğleden Sonra (12:00-18:00)</option>
                                <option value="evening">🌆 Akşam (18:00-24:00)</option>
                                <option value="night">🌙 Gece (00:00-06:00)</option>
                            </select>
                        </div>

                        <div className="filter-group">
                            <label className="filter-label">Uçuş Süresi</label>
                            <select
                                value={pendingDurationMax}
                                onChange={(e) => setPendingDurationMax(parseInt(e.target.value))}
                                className="filter-select"
                            >
                                <option value={24}>Tümü</option>
                                <option value={2}>2 saat altı</option>
                                <option value={4}>4 saat altı</option>
                                <option value={6}>6 saat altı</option>
                                <option value={8}>8 saat altı</option>
                                <option value={12}>12 saat altı</option>
                            </select>
                        </div>
                        <div className="filter-actions">
                            <button
                                className="btn btn-primary"
                                onClick={() => {
                                    setSortBy(pendingSortBy);
                                    setStatusFilter(pendingStatus);
                                    setAirlineFilter(pendingAirline);
                                    setClassFilter(pendingClass);
                                    setDepartureTimeFilter(pendingDeparture);
                                    setDurationFilter({ min: 0, max: pendingDurationMax });
                                    setShowFilters(false);
                                }}
                            >
                                Filtrele
                            </button>
                            <button
                                className="btn btn-secondary"
                                onClick={() => {
                                    setSearchTerm("");
                                    setPendingSortBy("price");
                                    setPendingStatus("all");
                                    setPendingAirline("all");
                                    setPendingClass("all");
                                    setPendingDeparture("all");
                                    setPendingDurationMax(24);
                                    setSortBy("price");
                                    setStatusFilter("all");
                                    setAirlineFilter("all");
                                    setClassFilter("all");
                                    setDepartureTimeFilter("all");
                                    setDurationFilter({ min: 0, max: 24 });
                                    setPriceRange({ min: 0, max: 10000 });
                                }}
                            >
                                Filtreleri Temizle
                            </button>
                        </div>
                      </div>
                    </div>
                    )}
                </div>

                {filteredFlights.length === 0 ? (
                    <div className="empty-state">
                        <div className="empty-icon">✈️</div>
                        <h3>Arama kriterlerinize uygun uçuş bulunamadı</h3>
                        <p>Farklı arama terimleri deneyin veya filtreleri temizleyin</p>
                        <button
                            onClick={() => {
                                setSearchTerm("");
                                setStatusFilter("all");
                                setAirlineFilter("all");
                                setClassFilter("all");
                                setDepartureTimeFilter("all");
                                setDurationFilter({ min: 0, max: 24 });
                                setPriceRange({ min: 0, max: 10000 });
                                setSortBy("price");
                            }}
                            className="clear-filters-btn"
                        >
                            🗑️ Filtreleri Temizle
                        </button>
                    </div>
                ) : (
                    <div className="flights-grid">
                        {filteredFlights.map(flight => (
                            <div key={flight.id} className="flight-card">
                                <div className="flight-header">
                                    <div className="flight-number">
                                        <div className="airline-section">
                                            <div className="airline-logo">
                                                {getAirlineLogo(flight.flightNumber)}
                                            </div>
                                            <div className="flight-info">
                                                <span className="flight-code">{flight.flightNumber}</span>
                                                <span className="airline-name">{getAirlineName(flight.flightNumber)}</span>
                                            </div>
                                        </div>
                                        <span className="flight-route">{flight.origin} → {flight.destination}</span>
                                    </div>
                                    <div
                                        className="status-badge"
                                        style={{ backgroundColor: getStatusColor(flight.status) }}
                                    >
                                        {getStatusText(flight.status)}
                                    </div>
                                </div>
                                <div className="flight-details">
                                    <div className="time-info">
                                        <div className="departure">
                                            <span className="time-label">Kalkış</span>
                                            <span className="time-value">{formatTime(flight.departureTime)}</span>
                                            <span className="date-value">{formatDate(flight.departureTime)}</span>
                                        </div>
                                        <div className="flight-duration">
                                            <span className="duration-icon">✈️</span>
                                        </div>
                                        <div className="arrival">
                                            <span className="time-label">Varış</span>
                                            <span className="time-value">{formatTime(flight.arrivalTime)}</span>
                                            <span className="date-value">{formatDate(flight.arrivalTime)}</span>
                                        </div>
                                    </div>
                                </div>

                                <div className="flight-actions">
                                    <button
                                        className="btn btn-primary"
                                        onClick={() => handleShowPrices(flight)}
                                    >
                                        Fiyatları Gör
                                    </button>
                                    <button
                                        className="btn btn-outline"
                                        onClick={() => handleShowDetails(flight)}
                                    >
                                        Detaylar
                                    </button>
                                </div>
                            </div>
                        ))}
                    </div>
                )}

                {showDetailsModal && selectedFlight && (
                    <div className="search-modal-overlay" onClick={() => setShowDetailsModal(false)}>
                        <div className="search-modal-content compact-modal" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <h2>Uçuş Detayları</h2>
                                <button
                                    className="modal-close"
                                    onClick={() => setShowDetailsModal(false)}
                                >
                                    ×
                                </button>
                            </div>

                            <div className="modal-body">
                                <div className="flight-detail-card compact-card">
                                    <div className="flight-detail-header">
                                        <div className="flight-info">
                                            <h3>{selectedFlight.flightNumber}</h3>
                                            <p>{selectedFlight.origin} → {selectedFlight.destination}</p>
                                        </div>
                                        <div
                                            className="status-badge"
                                            style={{ backgroundColor: getStatusColor(selectedFlight.status) }}
                                        >
                                            {getStatusText(selectedFlight.status)}
                                        </div>
                                    </div>

                                    <div className="flight-detail-info compact-info">
                                        <div className="detail-section">
                                            <h4>Kalkış</h4>
                                            <div className="detail-item">
                                                <span className="label">Havalimanı:</span>
                                                <span className="value">{selectedFlight.origin}</span>
                                            </div>
                                            <div className="detail-item">
                                                <span className="label">Tarih:</span>
                                                <span className="value">{formatDate(selectedFlight.departureTime)}</span>
                                            </div>
                                            <div className="detail-item">
                                                <span className="label">Saat:</span>
                                                <span className="value">{formatTime(selectedFlight.departureTime)}</span>
                                            </div>
                                        </div>

                                        <div className="detail-section">
                                            <h4>Varış</h4>
                                            <div className="detail-item">
                                                <span className="label">Havalimanı:</span>
                                                <span className="value">{selectedFlight.destination}</span>
                                            </div>
                                            <div className="detail-item">
                                                <span className="label">Tarih:</span>
                                                <span className="value">{formatDate(selectedFlight.arrivalTime)}</span>
                                            </div>
                                            <div className="detail-item">
                                                <span className="label">Saat:</span>
                                                <span className="value">{formatTime(selectedFlight.arrivalTime)}</span>
                                            </div>
                                        </div>

                                        <div className="detail-section">
                                            <h4>Uçuş Bilgileri</h4>
                                            <div className="detail-item">
                                                <span className="label">Uçuş Numarası:</span>
                                                <span className="value">{selectedFlight.flightNumber}</span>
                                            </div>
                                            <div className="detail-item">
                                                <span className="label">Durum:</span>
                                                <span className="value">{getStatusText(selectedFlight.status)}</span>
                                            </div>
                                            <div className="detail-item">
                                                <span className="label">Uçuş Süresi:</span>
                                                <span className="value">
                                                    {Math.round((new Date(selectedFlight.arrivalTime).getTime() - new Date(selectedFlight.departureTime).getTime()) / (1000 * 60 * 60))} saat
                                                </span>
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
                            </div>
                        </div>
                    </div>
                )}

                {showPricesModal && selectedFlight && (
                    <div className="search-modal-overlay" onClick={() => setShowPricesModal(false)}>
                        <div className="search-modal-content compact-modal" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <h2>Fiyat Seçenekleri - {selectedFlight.flightNumber}</h2>
                                <button
                                    className="modal-close"
                                    onClick={() => setShowPricesModal(false)}
                                >
                                    ×
                                </button>
                            </div>

                            <div className="modal-body">
                                <div className="flight-detail-card compact-card">
                                    <div className="flight-detail-header">
                                        <div className="flight-info">
                                            <h3>{selectedFlight.flightNumber}</h3>
                                            <p>{selectedFlight.origin} → {selectedFlight.destination}</p>
                                            <p>{formatTime(selectedFlight.departureTime)} - {formatTime(selectedFlight.arrivalTime)}</p>
                                        </div>
                                    </div>

                                    <div className="price-options compact-prices">
                                        {selectedFlight.prices.map((price, index) => (
                                            <div key={`${selectedFlight.id}-${price.id}-${index}`} className="price-option compact-price-option">
                                                <div className="price-info">
                                                    <span className="price-class">{price.class}</span>
                                                    <span className="price-amount">{price.price} {price.currency == "TRY" ? "₺" : price.currency}</span>
                                                    <span className="available-seats">{price.availableSeats} koltuk</span>
                                                </div>
                                                <button
                                                    className="btn btn-primary btn-sm"
                                                    onClick={() => {
                                                        setShowPricesModal(false);
                                                        handleReserveFlight(selectedFlight, price);
                                                    }}
                                                    disabled={reservingFlights.has(selectedFlight.flightNumber) || selectedFlight.status === "Cancelled" || price.availableSeats === 0}
                                                >
                                                    {reservingFlights.has(selectedFlight.flightNumber) ? "Rezerve Ediliyor..." : "Rezerve Et"}
                                                </button>
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            </div>

                            <div className="modal-footer">
                                <button
                                    className="btn btn-secondary"
                                    onClick={() => setShowPricesModal(false)}
                                >
                                    Kapat
                                </button>
                            </div>
                        </div>
                    </div>
                )}

                <ConfirmationModal
                    isOpen={showConfirmModal}
                    title="Rezervasyon Onayı"
                    message={`${flightToReserve?.flightNumber} uçuşunu rezerve etmek istediğinizden emin misiniz?`}
                    confirmText="Rezerve Et"
                    cancelText="İptal"
                    type="info"
                    onConfirm={confirmReservation}
                    onCancel={() => {
                        setShowConfirmModal(false);
                        setFlightToReserve(null);
                    }}
                />
            </div>
        </div>
    );
}

export default SearchPage;
