import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
// import { useEffect } from "react"; // Kullanılmıyor
import Navbar from "./components/Navbar";
import ToastContainer from "./components/ToastContainer";
import { ToastProvider } from "./contexts/ToastContext";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import SearchPage from "./pages/SearchPage";
import SearchResults from "./pages/SearchResults";
import BookingsPage from "./pages/BookingsPage";
import ProfilePage from "./pages/ProfilePage";
import AdminDashboard from "./pages/AdminDashboard";
import AdminLogsPage from "./pages/AdminLogsPage";
import AdminUsersPage from "./pages/AdminUsersPage";
import AdminFlightsPage from "./pages/AdminFlightsPage";
import "./App.css";

function App() {

    return (
        <ToastProvider>
            <Router>
                <Navbar />
                <ToastContainer />

                <Routes>
                {/* Ana sayfa */}
                <Route path="/" element={<HomePage />} />

                {/* Auth sayfaları */}
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />


                {/* Uçuş arama sonuçları */}
                <Route path="/search-results" element={<SearchResults />} />

                {/* Rezervasyonlarım - Ana Panel */}
                <Route path="/bookings" element={<BookingsPage />} />
                <Route path="/dashboard" element={<BookingsPage />} />

                {/* Profil */}
                <Route path="/profile" element={<ProfilePage />} />

                {/* Tüm uçuşlar sayfası */}
                <Route path="/search" element={<SearchPage />} />

                {/* Admin sayfaları */}
                <Route path="/admin" element={<AdminDashboard />} />
                <Route path="/admin/logs" element={<AdminLogsPage />} />
                <Route path="/admin/users" element={<AdminUsersPage />} />
                <Route path="/admin/flights" element={<AdminFlightsPage />} />
                </Routes>
            </Router>
        </ToastProvider>
    );
}

export default App;
