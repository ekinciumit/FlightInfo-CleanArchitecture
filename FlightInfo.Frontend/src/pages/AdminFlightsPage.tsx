import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import "./AdminFlightsPage.css";

interface Flight {
    id: number;
    flightNumber: string;
    origin: string;
    destination: string;
    departureTime: string;
    arrivalTime: string;
    status: string;
    aircraftType: string;
    capacity: number;
    availableSeats: number;
    createdAt: string;
    updatedAt: string;
    prices?: Array<{
        id: number;
        class: string;
        price: number;
        currency: string;
        availableSeats: number;
    }>;
}

function AdminFlightsPage() {
    const [flights, setFlights] = useState<Flight[]>([]);
    const [filteredFlights, setFilteredFlights] = useState<Flight[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");
    const [searchTerm, setSearchTerm] = useState("");
    const [statusFilter, setStatusFilter] = useState("all");
    const [sortBy, setSortBy] = useState("departureTime");
    const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");

    // Profesyonel sıralama sistemi
    const handleSort = (column: string) => {
        if (sortBy === column) {
            // Aynı sütuna tekrar tıklandığında sıralama yönünü değiştir
            setSortOrder(sortOrder === "asc" ? "desc" : "asc");
        } else {
            // Yeni sütuna tıklandığında varsayılan olarak artan sıralama
            setSortBy(column);
            setSortOrder("asc");
        }
    };

    const getSortIcon = (column: string) => {
        if (sortBy !== column) {
            return "↕️"; // Sıralanmamış sütunlar için
        }
        return sortOrder === "asc" ? "↑" : "↓"; // Aktif sıralama için
    };
    const [selectedFlight, setSelectedFlight] = useState<Flight | null>(null);
    const [showFlightModal, setShowFlightModal] = useState(false);
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [editingFlight, setEditingFlight] = useState<Flight | null>(null);
    const [showPriceModal, setShowPriceModal] = useState(false);
    const [selectedFlightForPrice, setSelectedFlightForPrice] = useState<Flight | null>(null);
    const [editingPrice, setEditingPrice] = useState<any>(null);

    // Para birimi sembolü fonksiyonu
    const getCurrencySymbol = (currency: string) => {
        switch (currency) {
            case "TRY": return "₺";
            case "USD": return "$";
            case "EUR": return "€";
            default: return currency;
        }
    };

    // Yeni uçuş formu
    const [newFlight, setNewFlight] = useState({
        flightNumber: "",
        origin: "",
        destination: "",
        departureTime: "",
        arrivalTime: "",
        aircraftType: "",
        capacity: 0
    });

    // Düzenleme formu
    const [editFlight, setEditFlight] = useState({
        flightNumber: "",
        origin: "",
        destination: "",
        departureTime: "",
        arrivalTime: "",
        aircraftType: "",
        capacity: 0,
        status: ""
    });

    // Fiyat formu
    const [priceForm, setPriceForm] = useState({
        class: "",
        price: 0,
        currency: "TRY",
        availableSeats: 0
    });

    useEffect(() => {
        loadFlights();
    }, []);

    useEffect(() => {
        filterAndSortFlights();
    }, [flights, searchTerm, statusFilter, sortBy, sortOrder]);

    const loadFlights = async () => {
        try {
            setIsLoading(true);
            const response = await fetch("http://localhost:7104/api/Flight/with-prices", {
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                }
            });

            if (!response.ok) {
                throw new Error("Uçuşlar yüklenirken hata oluştu");
            }

            const flightsData = await response.json();
            setFlights(flightsData);
        } catch (error: any) {
            console.error("Uçuş yükleme hatası:", error);
            setError(error.message);
        } finally {
            setIsLoading(false);
        }
    };

    const filterAndSortFlights = () => {
        let filtered = [...flights];

        // Arama filtresi
        if (searchTerm) {
            filtered = filtered.filter(flight =>
                flight.flightNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
                flight.origin.toLowerCase().includes(searchTerm.toLowerCase()) ||
                flight.destination.toLowerCase().includes(searchTerm.toLowerCase()) ||
                flight.aircraftType.toLowerCase().includes(searchTerm.toLowerCase())
            );
        }

        // Status filtresi
        if (statusFilter !== "all") {
            filtered = filtered.filter(flight => flight.status === statusFilter);
        }

        // Sıralama
        filtered.sort((a, b) => {
            let aValue: any, bValue: any;

            switch (sortBy) {
                case "flightNumber":
                    aValue = a.flightNumber;
                    bValue = b.flightNumber;
                    break;
                case "departureTime":
                    aValue = new Date(a.departureTime).getTime();
                    bValue = new Date(b.departureTime).getTime();
                    break;
                case "status":
                    aValue = a.status;
                    bValue = b.status;
                    break;
                default:
                    aValue = new Date(a.departureTime).getTime();
                    bValue = new Date(b.departureTime).getTime();
            }

            if (sortOrder === "asc") {
                return aValue > bValue ? 1 : -1;
            } else {
                return aValue < bValue ? 1 : -1;
            }
        });

        setFilteredFlights(filtered);
    };

    const getStatusColor = (status: string) => {
        switch (status.toLowerCase()) {
            case "scheduled":
                return "#3498db";
            case "boarding":
                return "#f39c12";
            case "departed":
                return "#e74c3c";
            case "arrived":
                return "#27ae60";
            case "cancelled":
                return "#95a5a6";
            default:
                return "#2c3e50";
        }
    };

    const getStatusIcon = (status: string) => {
        switch (status.toLowerCase()) {
            case "scheduled":
                return "📅";
            case "boarding":
                return "🚶";
            case "departed":
                return "✈️";
            case "arrived":
                return "✅";
            case "cancelled":
                return "❌";
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

    const formatTime = (timeString: string) => {
        return new Date(timeString).toLocaleTimeString('tr-TR', {
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    const handleFlightClick = (flight: Flight) => {
        setSelectedFlight(flight);
        setShowFlightModal(true);
    };

    const handleCreateFlight = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const response = await fetch("http://localhost:7104/api/Flight", {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(newFlight)
            });

            if (response.ok) {
                alert("Uçuş başarıyla oluşturuldu!");
                setShowCreateModal(false);
                setNewFlight({
                    flightNumber: "",
                    origin: "",
                    destination: "",
                    departureTime: "",
                    arrivalTime: "",
                    aircraftType: "",
                    capacity: 0
                });
                loadFlights();
            } else {
                throw new Error("Uçuş oluşturulurken hata oluştu");
            }
        } catch (error: any) {
            console.error("Uçuş oluşturma hatası:", error);
            alert("Hata: " + error.message);
        }
    };

    const deleteFlight = async (flightId: number) => {
        if (window.confirm("Bu uçuşu silmek istediğinizden emin misiniz? Bu işlem geri alınamaz!")) {
            try {
                const response = await fetch(`http://localhost:7104/api/Flight/${flightId}`, {
                    method: "DELETE",
                    headers: {
                        "Authorization": `Bearer ${localStorage.getItem("token")}`,
                        "Content-Type": "application/json"
                    }
                });

                if (response.ok) {
                    setFlights(prev => prev.filter(flight => flight.id !== flightId));
                    alert("Uçuş başarıyla silindi");
                } else {
                    throw new Error("Uçuş silinirken hata oluştu");
                }
            } catch (error: any) {
                console.error("Uçuş silme hatası:", error);
                alert("Hata: " + error.message);
            }
        }
    };

    const handleEditFlight = (flight: Flight) => {
        setEditingFlight(flight);

        // Datetime-local için format düzeltmesi
        const formatForInput = (dateString: string) => {
            const date = new Date(dateString);
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            const hours = String(date.getHours()).padStart(2, '0');
            const minutes = String(date.getMinutes()).padStart(2, '0');
            return `${year}-${month}-${day}T${hours}:${minutes}`;
        };

        setEditFlight({
            flightNumber: flight.flightNumber,
            origin: flight.origin,
            destination: flight.destination,
            departureTime: formatForInput(flight.departureTime),
            arrivalTime: formatForInput(flight.arrivalTime),
            aircraftType: flight.aircraftType,
            capacity: flight.capacity,
            status: flight.status
        });
        setShowEditModal(true);
    };

    const updateFlight = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!editingFlight) return;

        try {
            // Datetime-local'dan ISO formatına çevirme
            const formatToISO = (dateTimeString: string) => {
                return new Date(dateTimeString).toISOString();
            };

            const flightData = {
                ...editFlight,
                departureTime: formatToISO(editFlight.departureTime),
                arrivalTime: formatToISO(editFlight.arrivalTime)
            };

            const response = await fetch(`http://localhost:7104/api/Flight/${editingFlight.id}`, {
                method: "PUT",
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(flightData)
            });

            if (response.ok) {
                alert("Uçuş başarıyla güncellendi!");
                setShowEditModal(false);
                setEditingFlight(null);
                loadFlights();
            } else {
                throw new Error("Uçuş güncellenirken hata oluştu");
            }
        } catch (error: any) {
            console.error("Uçuş güncelleme hatası:", error);
            alert("Hata: " + error.message);
        }
    };

    // Fiyat yönetimi fonksiyonları
    const handleManagePrices = (flight: Flight) => {
        console.log('Managing prices for flight:', flight);
        console.log('Flight prices:', flight.prices);
        setSelectedFlightForPrice(flight);
        setEditingPrice(null);
        setPriceForm({
            class: "",
            price: 0,
            currency: "TRY",
            availableSeats: 0
        });
        setShowPriceModal(true);
    };

    const handleEditPrice = (price: any) => {
        console.log('Editing price:', price);
        setEditingPrice(price);
        setPriceForm({
            class: price.class,
            price: price.price,
            currency: price.currency,
            availableSeats: price.availableSeats
        });
    };

    const handleAddPrice = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedFlightForPrice) return;

        try {
            const response = await fetch(`http://localhost:7104/api/Flight/${selectedFlightForPrice.id}/price`, {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(priceForm)
            });

            if (response.ok) {
                alert("Fiyat başarıyla eklendi!");
                setShowPriceModal(false);
                loadFlights();
            } else {
                throw new Error("Fiyat eklenirken hata oluştu");
            }
        } catch (error: any) {
            console.error("Fiyat ekleme hatası:", error);
            alert("Hata: " + error.message);
        }
    };

    const handleUpdatePrice = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedFlightForPrice || !editingPrice) return;

        try {
            console.log('Updating price:', {
                flightId: selectedFlightForPrice.id,
                priceId: editingPrice.id,
                priceForm: priceForm,
                editingPrice: editingPrice
            });

            // Fiyat ID'si kontrolü
            if (!editingPrice.id) {
                throw new Error("Fiyat ID'si bulunamadı!");
            }

            // Fiyat ID'sini integer'a çevir
            const priceId = parseInt(editingPrice.id);
            if (isNaN(priceId)) {
                throw new Error("Geçersiz fiyat ID'si!");
            }

            console.log('Final request:', {
                flightId: selectedFlightForPrice.id,
                priceId: priceId,
                url: `http://localhost:7104/api/Flight/${selectedFlightForPrice.id}/price/${priceId}`
            });

            const response = await fetch(`http://localhost:7104/api/Flight/${selectedFlightForPrice.id}/price/${priceId}`, {
                method: "PUT",
                headers: {
                    "Authorization": `Bearer ${localStorage.getItem("token")}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(priceForm)
            });

            if (response.ok) {
                alert("Fiyat başarıyla güncellendi!");
                setShowPriceModal(false);
                setEditingPrice(null);
                loadFlights();
            } else {
                const errorText = await response.text();
                console.error('API Error:', response.status, errorText);
                throw new Error(`Fiyat güncellenirken hata oluştu: ${response.status}`);
            }
        } catch (error: any) {
            console.error("Fiyat güncelleme hatası:", error);
            alert("Hata: " + error.message);
        }
    };

    const handleDeletePrice = async (priceId: number) => {
        if (!selectedFlightForPrice) return;

        if (window.confirm("Bu fiyatı silmek istediğinizden emin misiniz?")) {
            try {
                console.log('Deleting price:', {
                    flightId: selectedFlightForPrice.id,
                    priceId: priceId
                });

                const response = await fetch(`http://localhost:7104/api/Flight/${selectedFlightForPrice.id}/price/${priceId}`, {
                    method: "DELETE",
                    headers: {
                        "Authorization": `Bearer ${localStorage.getItem("token")}`
                    }
                });

                if (response.ok) {
                    alert("Fiyat başarıyla silindi!");
                    loadFlights();
                } else {
                    const errorText = await response.text();
                    console.error('Delete API Error:', response.status, errorText);
                    throw new Error(`Fiyat silinirken hata oluştu: ${response.status}`);
                }
            } catch (error: any) {
                console.error("Fiyat silme hatası:", error);
                alert("Hata: " + error.message);
            }
        }
    };

    if (isLoading) {
        return (
            <div className="admin-flights-page">
                <div className="container">
                    <div className="loading-state">
                        <div className="loading-spinner"></div>
                        <p>Uçuşlar yükleniyor...</p>
                    </div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="admin-flights-page">
                <div className="container">
                    <div className="error-state">
                        <h2>❌ Hata</h2>
                        <p>{error}</p>
                        <button onClick={loadFlights} className="btn btn-primary">
                            Tekrar Dene
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="admin-flights-page">
            <div className="container">
                {/* Header */}
                <div className="flights-header">
                    <div className="header-left">
                        <Link to="/admin" className="back-button">
                            ← Admin Paneli
                        </Link>
                        <h1>✈️ Uçuş Yönetimi</h1>
                        <p>Toplam {filteredFlights.length} uçuş</p>
                    </div>
                    <div className="header-actions">
                        <button
                            onClick={() => setShowCreateModal(true)}
                            className="btn btn-success"
                        >
                            ➕ Yeni Uçuş
                        </button>
                        <button onClick={loadFlights} className="btn btn-secondary">
                            🔄 Yenile
                        </button>
                    </div>
                </div>

                {/* Filters */}
                <div className="filters-section">
                    <div className="filter-group">
                        <label>Ara:</label>
                        <input
                            type="text"
                            placeholder="Uçuş numarası, kalkış, varış ara..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="search-input"
                        />
                    </div>
                    <div className="filter-group">
                        <label>Durum:</label>
                        <select
                            value={statusFilter}
                            onChange={(e) => setStatusFilter(e.target.value)}
                            className="filter-select"
                        >
                            <option value="all">Tümü</option>
                            <option value="Scheduled">Planlandı</option>
                            <option value="Boarding">Biniş</option>
                            <option value="Departed">Kalktı</option>
                            <option value="Arrived">İndi</option>
                            <option value="Cancelled">İptal</option>
                        </select>
                    </div>
                </div>

                {/* Flights Table */}
                <div className="flights-table-container">
                    {filteredFlights.length === 0 ? (
                        <div className="empty-state">
                            <div className="empty-icon">✈️</div>
                            <h3>Uçuş bulunamadı</h3>
                            <p>Arama kriterlerinize uygun uçuş bulunmuyor.</p>
                        </div>
                    ) : (
                        <table className="flights-table">
                            <thead>
                                <tr>
                                    <th
                                        className="sortable"
                                        onClick={() => handleSort("flightNumber")}
                                        title="Uçuş numarasına göre sırala"
                                    >
                                        Uçuş No {getSortIcon("flightNumber")}
                                    </th>
                                    <th
                                        className="sortable"
                                        onClick={() => handleSort("departureTime")}
                                        title="Kalkış saatine göre sırala"
                                    >
                                        Kalkış Saati {getSortIcon("departureTime")}
                                    </th>
                                    <th>Rota</th>
                                    <th
                                        className="sortable"
                                        onClick={() => handleSort("status")}
                                        title="Duruma göre sırala"
                                    >
                                        Durum {getSortIcon("status")}
                                    </th>
                                    <th>Uçak</th>
                                    <th>Kapasite</th>
                                    <th>Fiyat</th>
                                    <th>İşlemler</th>
                                </tr>
                            </thead>
                            <tbody>
                                {filteredFlights.map(flight => (
                                    <tr key={flight.id} className="flight-row">
                                        <td className="flight-number">
                                            <strong>{flight.flightNumber}</strong>
                                        </td>
                                        <td className="departure-time">
                                            <div>{formatTime(flight.departureTime)}</div>
                                            <small>{formatDate(flight.departureTime)}</small>
                                        </td>
                                        <td className="route">
                                            <strong>{flight.origin}</strong> → <strong>{flight.destination}</strong>
                                        </td>
                                        <td className="status">
                                            <span
                                                className="status-badge"
                                                style={{ backgroundColor: getStatusColor(flight.status) }}
                                            >
                                                {getStatusIcon(flight.status)} {flight.status}
                                            </span>
                                        </td>
                                        <td className="aircraft">
                                            {flight.aircraftType || 'Bilinmiyor'}
                                        </td>
                                        <td className="capacity">
                                            <span className="capacity-info">
                                                {flight.availableSeats || 0}/{flight.capacity || 0}
                                            </span>
                                        </td>
                                        <td className="price">
                                            {flight.prices && flight.prices.length > 0 ? (
                                                <div className="price-info">
                                                    <span className="price-amount">
                                                        {flight.prices[0].price} {getCurrencySymbol(flight.prices[0].currency)}
                                                    </span>
                                                    <small>{flight.prices[0].class}</small>
                                                </div>
                                            ) : (
                                                <span className="no-price">Fiyat Yok</span>
                                            )}
                                        </td>
                                        <td className="actions">
                                            <div className="action-buttons">
                                                <button
                                                    className="btn btn-sm btn-primary"
                                                    onClick={() => handleFlightClick(flight)}
                                                    title="Detayları Görüntüle"
                                                >
                                                    Detaylar
                                                </button>
                                                <button
                                                    className="btn btn-sm btn-warning"
                                                    onClick={() => handleEditFlight(flight)}
                                                    title="Uçuşu Düzenle"
                                                >
                                                    Düzenle
                                                </button>
                                                <button
                                                    className="btn btn-sm btn-info"
                                                    onClick={() => handleManagePrices(flight)}
                                                    title="Fiyatları Yönet"
                                                >
                                                    Fiyatlar
                                                </button>
                                                <button
                                                    className="btn btn-sm btn-danger"
                                                    onClick={() => deleteFlight(flight.id)}
                                                    title="Uçuşu Sil"
                                                >
                                                    Sil
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                            ))}
                        </tbody>
                        </table>
                    )}
                </div>

                {/* Create Flight Modal */}
                {showCreateModal && (
                    <div className="modal-overlay" onClick={() => setShowCreateModal(false)}>
                        <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <h2>Yeni Uçuş Oluştur</h2>
                                <button
                                    className="modal-close"
                                    onClick={() => setShowCreateModal(false)}
                                >
                                    ×
                                </button>
                            </div>

                            <form onSubmit={handleCreateFlight} className="modal-body">
                                <div className="form-grid">
                                    <div className="form-group">
                                        <label>Uçuş Numarası *</label>
                                        <input
                                            type="text"
                                            value={newFlight.flightNumber}
                                            onChange={(e) => setNewFlight({...newFlight, flightNumber: e.target.value})}
                                            required
                                            placeholder="TK001"
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Kalkış *</label>
                                        <input
                                            type="text"
                                            value={newFlight.origin}
                                            onChange={(e) => setNewFlight({...newFlight, origin: e.target.value})}
                                            required
                                            placeholder="İstanbul"
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Varış *</label>
                                        <input
                                            type="text"
                                            value={newFlight.destination}
                                            onChange={(e) => setNewFlight({...newFlight, destination: e.target.value})}
                                            required
                                            placeholder="New York"
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Kalkış Saati *</label>
                                        <input
                                            type="datetime-local"
                                            value={newFlight.departureTime}
                                            onChange={(e) => setNewFlight({...newFlight, departureTime: e.target.value})}
                                            required
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Varış Saati *</label>
                                        <input
                                            type="datetime-local"
                                            value={newFlight.arrivalTime}
                                            onChange={(e) => setNewFlight({...newFlight, arrivalTime: e.target.value})}
                                            required
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Uçak Tipi *</label>
                                        <select
                                            value={newFlight.aircraftType}
                                            onChange={(e) => setNewFlight({...newFlight, aircraftType: e.target.value})}
                                            required
                                        >
                                            <option value="">Uçak tipi seçin</option>
                                            <option value="Boeing 737">Boeing 737</option>
                                            <option value="Boeing 777">Boeing 777</option>
                                            <option value="Boeing 787">Boeing 787</option>
                                            <option value="Airbus A320">Airbus A320</option>
                                            <option value="Airbus A330">Airbus A330</option>
                                            <option value="Airbus A350">Airbus A350</option>
                                            <option value="Airbus A380">Airbus A380</option>
                                            <option value="Embraer E190">Embraer E190</option>
                                            <option value="Bombardier CRJ900">Bombardier CRJ900</option>
                                            <option value="ATR 72">ATR 72</option>
                                            <option value="Cessna Citation">Cessna Citation</option>
                                        </select>
                                    </div>
                                    <div className="form-group">
                                        <label>Kapasite *</label>
                                        <input
                                            type="number"
                                            value={newFlight.capacity || ''}
                                            onChange={(e) => setNewFlight({...newFlight, capacity: parseInt(e.target.value) || 0})}
                                            required
                                            min="1"
                                            placeholder="200"
                                        />
                                    </div>
                                </div>
                            </form>

                            <div className="modal-footer">
                                <button
                                    type="button"
                                    className="btn btn-secondary"
                                    onClick={() => setShowCreateModal(false)}
                                >
                                    İptal
                                </button>
                                <button
                                    type="submit"
                                    className="btn btn-success"
                                    onClick={handleCreateFlight}
                                >
                                    ➕ Uçuş Oluştur
                                </button>
                            </div>
                        </div>
                    </div>
                )}

                {/* Flight Detail Modal */}
                {showFlightModal && selectedFlight && (
                    <div className="modal-overlay" onClick={() => setShowFlightModal(false)}>
                        <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <h2>Uçuş Detayları</h2>
                                <button
                                    className="modal-close"
                                    onClick={() => setShowFlightModal(false)}
                                >
                                    ×
                                </button>
                            </div>

                            <div className="modal-body">
                                <div className="flight-detail-card">
                                    <div className="detail-header">
                                        <h3>{selectedFlight.flightNumber}</h3>
                                        <p>{selectedFlight.origin} → {selectedFlight.destination}</p>
                                        <span
                                            className="status-badge"
                                            style={{ backgroundColor: getStatusColor(selectedFlight.status) }}
                                        >
                                            {getStatusIcon(selectedFlight.status)} {selectedFlight.status}
                                        </span>
                                    </div>

                                    <div className="detail-sections">
                                        <div className="detail-section">
                                            <h4>Uçuş Bilgileri</h4>
                                            <div className="detail-grid">
                                                <div className="detail-item">
                                                    <span className="label">Uçuş Numarası:</span>
                                                    <span className="value">{selectedFlight.flightNumber}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Kalkış:</span>
                                                    <span className="value">{selectedFlight.origin}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Varış:</span>
                                                    <span className="value">{selectedFlight.destination}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Kalkış Saati:</span>
                                                    <span className="value">{formatTime(selectedFlight.departureTime)}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Varış Saati:</span>
                                                    <span className="value">{formatTime(selectedFlight.arrivalTime)}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Uçuş Tarihi:</span>
                                                    <span className="value">{formatDate(selectedFlight.departureTime)}</span>
                                                </div>
                                            </div>
                                        </div>

                                        <div className="detail-section">
                                            <h4>Teknik Bilgiler</h4>
                                            <div className="detail-grid">
                                                <div className="detail-item">
                                                    <span className="label">Uçak Tipi:</span>
                                                    <span className="value">{selectedFlight.aircraftType}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Kapasite:</span>
                                                    <span className="value">{selectedFlight.capacity}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Müsait Koltuk:</span>
                                                    <span className="value">{selectedFlight.availableSeats}</span>
                                                </div>
                                                <div className="detail-item">
                                                    <span className="label">Durum:</span>
                                                    <span className="value">{selectedFlight.status}</span>
                                                </div>
                                            </div>
                                        </div>

                                        {selectedFlight.prices && selectedFlight.prices.length > 0 && (
                                            <div className="detail-section">
                                                <h4>Fiyat Bilgileri</h4>
                                                <div className="prices-grid">
                                                    {selectedFlight.prices.map(price => (
                                                        <div key={price.id} className="price-card">
                                                            <div className="price-class">{price.class}</div>
                                                            <div className="price-amount">{price.price} {getCurrencySymbol(price.currency)}</div>
                                                            <div className="price-seats">{price.availableSeats} koltuk</div>
                                                        </div>
                                                    ))}
                                                </div>
                                            </div>
                                        )}
                                    </div>
                                </div>
                            </div>

                            <div className="modal-footer">
                                <button
                                    className="btn btn-secondary"
                                    onClick={() => setShowFlightModal(false)}
                                >
                                    Kapat
                                </button>
                                <button
                                    className="btn btn-warning"
                                    onClick={() => {
                                        setShowFlightModal(false);
                                        handleEditFlight(selectedFlight);
                                    }}
                                >
                                    Düzenle
                                </button>
                                <button
                                    className="btn btn-danger"
                                    onClick={() => {
                                        deleteFlight(selectedFlight.id);
                                        setShowFlightModal(false);
                                    }}
                                >
                                    Sil
                                </button>
                            </div>
                        </div>
                    </div>
                )}

                {/* Edit Flight Modal */}
                {showEditModal && editingFlight && (
                    <div className="modal-overlay" onClick={() => setShowEditModal(false)}>
                        <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <h2>Uçuş Düzenle</h2>
                                <button
                                    className="modal-close"
                                    onClick={() => setShowEditModal(false)}
                                >
                                    ×
                                </button>
                            </div>

                            <form onSubmit={updateFlight}>
                                <div className="modal-body">
                                    <div className="form-group">
                                        <label>Uçuş Numarası:</label>
                                        <input
                                            type="text"
                                            value={editFlight.flightNumber}
                                            onChange={(e) => setEditFlight({...editFlight, flightNumber: e.target.value})}
                                            required
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label>Kalkış Havalimanı:</label>
                                        <input
                                            type="text"
                                            value={editFlight.origin}
                                            onChange={(e) => setEditFlight({...editFlight, origin: e.target.value})}
                                            required
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label>Varış Havalimanı:</label>
                                        <input
                                            type="text"
                                            value={editFlight.destination}
                                            onChange={(e) => setEditFlight({...editFlight, destination: e.target.value})}
                                            required
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label>Kalkış Zamanı:</label>
                                        <input
                                            type="datetime-local"
                                            value={editFlight.departureTime}
                                            onChange={(e) => setEditFlight({...editFlight, departureTime: e.target.value})}
                                            required
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label>Varış Zamanı:</label>
                                        <input
                                            type="datetime-local"
                                            value={editFlight.arrivalTime}
                                            onChange={(e) => setEditFlight({...editFlight, arrivalTime: e.target.value})}
                                            required
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label>Uçak Tipi:</label>
                                        <select
                                            value={editFlight.aircraftType}
                                            onChange={(e) => setEditFlight({...editFlight, aircraftType: e.target.value})}
                                            required
                                        >
                                            <option value="">Uçak tipi seçin</option>
                                            <option value="Boeing 737">Boeing 737</option>
                                            <option value="Boeing 777">Boeing 777</option>
                                            <option value="Boeing 787">Boeing 787</option>
                                            <option value="Airbus A320">Airbus A320</option>
                                            <option value="Airbus A330">Airbus A330</option>
                                            <option value="Airbus A350">Airbus A350</option>
                                            <option value="Airbus A380">Airbus A380</option>
                                            <option value="Embraer E190">Embraer E190</option>
                                            <option value="Bombardier CRJ900">Bombardier CRJ900</option>
                                            <option value="ATR 72">ATR 72</option>
                                            <option value="Cessna Citation">Cessna Citation</option>
                                        </select>
                                    </div>

                                    <div className="form-group">
                                        <label>Kapasite:</label>
                                        <input
                                            type="number"
                                            value={editFlight.capacity || ''}
                                            onChange={(e) => setEditFlight({...editFlight, capacity: parseInt(e.target.value) || 0})}
                                            required
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label>Durum:</label>
                                        <select
                                            value={editFlight.status}
                                            onChange={(e) => setEditFlight({...editFlight, status: e.target.value})}
                                            required
                                        >
                                            <option value="Scheduled">Planlandı</option>
                                            <option value="Boarding">Biniş</option>
                                            <option value="Departed">Kalktı</option>
                                            <option value="Arrived">Vardı</option>
                                            <option value="Cancelled">İptal</option>
                                        </select>
                                    </div>
                                </div>

                                <div className="modal-footer">
                                    <button
                                        type="button"
                                        className="btn btn-secondary"
                                        onClick={() => setShowEditModal(false)}
                                    >
                                        İptal
                                    </button>
                                    <button
                                        type="submit"
                                        className="btn btn-primary"
                                    >
                                        Güncelle
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                )}

                {/* Price Management Modal */}
                {showPriceModal && selectedFlightForPrice && (
                    <div className="modal-overlay" onClick={() => setShowPriceModal(false)}>
                        <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <h2>Fiyat Yönetimi - {selectedFlightForPrice.flightNumber}</h2>
                                <button
                                    className="modal-close"
                                    onClick={() => setShowPriceModal(false)}
                                >
                                    ×
                                </button>
                            </div>

                            <div className="modal-body">
                                {/* Mevcut Fiyatlar */}
                                <div className="price-management-section">
                                    <h3>Mevcut Fiyatlar</h3>
                                    {selectedFlightForPrice.prices && selectedFlightForPrice.prices.length > 0 ? (
                                        <div className="existing-prices">
                                            {selectedFlightForPrice.prices.map(price => (
                                                <div key={price.id} className="price-item-admin">
                                                    <div className="price-info">
                                                        <span className="price-class">{price.class}</span>
                                                        <span className="price-amount">{price.price} {getCurrencySymbol(price.currency)}</span>
                                                        <span className="price-seats">{price.availableSeats} koltuk</span>
                                                        <span className="price-id">ID: {price.id}</span>
                                                    </div>
                                                    <div className="price-actions">
                                                        <button
                                                            className="btn btn-sm btn-warning"
                                                            onClick={() => handleEditPrice(price)}
                                                        >
                                                            Düzenle
                                                        </button>
                                                        <button
                                                            className="btn btn-sm btn-danger"
                                                            onClick={() => handleDeletePrice(price.id)}
                                                        >
                                                            Sil
                                                        </button>
                                                    </div>
                                                </div>
                                            ))}
                                        </div>
                                    ) : (
                                        <p className="no-prices">Bu uçuş için henüz fiyat tanımlanmamış.</p>
                                    )}
                                </div>

                                {/* Fiyat Ekleme/Düzenleme Formu */}
                                <div className="price-form-section">
                                    <h3>{editingPrice ? 'Fiyat Düzenle' : 'Yeni Fiyat Ekle'}</h3>
                                    <form onSubmit={editingPrice ? handleUpdatePrice : handleAddPrice}>
                                        <div className="form-grid">
                                            <div className="form-group">
                                                <label>Sınıf:</label>
                                                <select
                                                    value={priceForm.class}
                                                    onChange={(e) => setPriceForm({...priceForm, class: e.target.value})}
                                                    required
                                                >
                                                    <option value="">Sınıf seçin</option>
                                                    <option value="Economy">Economy</option>
                                                    <option value="Business">Business</option>
                                                    <option value="First">First</option>
                                                </select>
                                            </div>
                                            <div className="form-group">
                                                <label>Fiyat:</label>
                                                <input
                                                    type="number"
                                                    value={priceForm.price || ''}
                                                    onChange={(e) => setPriceForm({...priceForm, price: parseFloat(e.target.value) || 0})}
                                                    required
                                                    min="0"
                                                    step="0.01"
                                                />
                                            </div>
                                            <div className="form-group">
                                                <label>Para Birimi:</label>
                                                <select
                                                    value={priceForm.currency}
                                                    onChange={(e) => setPriceForm({...priceForm, currency: e.target.value})}
                                                    required
                                                >
                                                    <option value="TRY">TRY</option>
                                                    <option value="USD">USD</option>
                                                    <option value="EUR">EUR</option>
                                                </select>
                                            </div>
                                            <div className="form-group">
                                                <label>Müsait Koltuk Sayısı:</label>
                                                <input
                                                    type="number"
                                                    value={priceForm.availableSeats || ''}
                                                    onChange={(e) => setPriceForm({...priceForm, availableSeats: parseInt(e.target.value) || 0})}
                                                    required
                                                    min="0"
                                                />
                                            </div>
                                        </div>
                                        <div className="modal-footer">
                                            <button
                                                type="button"
                                                className="btn btn-secondary"
                                                onClick={() => {
                                                    setEditingPrice(null);
                                                    setPriceForm({
                                                        class: "",
                                                        price: 0,
                                                        currency: "TRY",
                                                        availableSeats: 0
                                                    });
                                                }}
                                            >
                                                İptal
                                            </button>
                                            <button
                                                type="submit"
                                                className="btn btn-primary"
                                            >
                                                {editingPrice ? 'Güncelle' : 'Ekle'}
                                            </button>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}

export default AdminFlightsPage;

