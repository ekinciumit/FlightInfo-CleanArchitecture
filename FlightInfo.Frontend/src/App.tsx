import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { useEffect } from "react";
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
    // Synchronize background slideshow across all pages
    useEffect(() => {
        const IMAGE_DURATION = 15000; 
        const TOTAL_DURATION = IMAGE_DURATION * 8;
        
        const images = [
            'url("/images/1.png")',
            'url("/images/2.png")',
            'url("/images/3.png")',
            'url("/images/4.png")',
            'url("/images/5.png")',
            'url("/images/6.png")',
            'url("/images/7.png")',
            'url("/images/8.png")'
        ];

        // Get or set sync start time
        const getSyncTime = () => {
            const stored = sessionStorage.getItem('bg-slideshow-start');
            if (stored) {
                return parseInt(stored, 10);
            }
            const startTime = Date.now();
            sessionStorage.setItem('bg-slideshow-start', startTime.toString());
            return startTime;
        };

        // Analyze image brightness and set text colors accordingly
        const analyzeImageBrightness = (imagePath: string): Promise<boolean> => {
            return new Promise((resolve) => {
                const img = new Image();
                // Extract image path from CSS url() format
                let src = imagePath.replace(/^url\(["']?/, '').replace(/["']?\)$/, '');
                img.src = src;
                
                img.onload = () => {
                    try {
                        const canvas = document.createElement('canvas');
                        const ctx = canvas.getContext('2d');
                        if (!ctx) {
                            resolve(false); // Default to dark text
                            return;
                        }
                        
                        // Limit canvas size for performance
                        const maxSize = 200;
                        const scale = Math.min(maxSize / img.width, maxSize / img.height, 1);
                        canvas.width = img.width * scale;
                        canvas.height = img.height * scale;
                        
                        ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
                        
                        const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
                        const data = imageData.data;
                        let brightness = 0;
                        let pixelCount = 0;
                        
                        // Sample pixels (every 4th pixel for performance)
                        for (let i = 0; i < data.length; i += 16) {
                            const r = data[i];
                            const g = data[i + 1];
                            const b = data[i + 2];
                            // Luminance formula
                            brightness += (0.299 * r + 0.587 * g + 0.114 * b);
                            pixelCount++;
                        }
                        
                        brightness = brightness / pixelCount;
                        // If brightness > 128, image is light (use dark text), else use light text
                        resolve(brightness > 128);
                    } catch (error) {
                        console.warn('Error analyzing image brightness:', error);
                        resolve(false); // Default to dark text on error
                    }
                };
                
                img.onerror = () => {
                    resolve(false); // Default to dark text on error
                };
            });
        };

        const updateBackground = async () => {
            const startTime = getSyncTime();
            const elapsed = Date.now() - startTime;
            const currentPosition = (elapsed % TOTAL_DURATION) / IMAGE_DURATION;
            const imageIndex = Math.floor(currentPosition);
            const currentImage = images[imageIndex] || images[0];

            // Update CSS custom property for current background image
            document.documentElement.style.setProperty('--bg-current-image', currentImage);
            
            // Analyze brightness and update text colors
            const imagePath = currentImage;
            const isLight = await analyzeImageBrightness(imagePath);
            
            if (isLight) {
                // Light background - use dark text
                document.documentElement.style.setProperty('--auth-text-color', '#1a1a1a');
                document.documentElement.style.setProperty('--auth-text-secondary', '#333333');
                document.documentElement.style.setProperty('--auth-label-color', '#1a1a1a');
                document.documentElement.style.setProperty('--auth-input-text', '#1a1a1a');
                // HomePage colors
                document.documentElement.style.setProperty('--home-hero-text', '#1a1a1a');
                document.documentElement.style.setProperty('--home-hero-subtitle', '#333333');
                document.documentElement.style.setProperty('--home-section-title', '#1a1a1a');
                document.documentElement.style.setProperty('--home-feature-title', '#1a1a1a');
                document.documentElement.style.setProperty('--home-feature-text', '#333333');
                // HomePage form colors
                document.documentElement.style.setProperty('--home-form-label', '#1a1a1a');
                document.documentElement.style.setProperty('--home-form-input-text', '#1a1a1a');
                document.documentElement.style.setProperty('--home-form-select-text', '#1a1a1a');
                document.documentElement.style.setProperty('--home-form-option-bg', '#ffffff');
                document.documentElement.style.setProperty('--home-form-option-text', '#1a1a1a');
            } else {
                // Dark background - use light text
                document.documentElement.style.setProperty('--auth-text-color', '#ffffff');
                document.documentElement.style.setProperty('--auth-text-secondary', '#f0f0f0');
                document.documentElement.style.setProperty('--auth-label-color', '#ffffff');
                document.documentElement.style.setProperty('--auth-input-text', '#ffffff');
                // HomePage colors
                document.documentElement.style.setProperty('--home-hero-text', '#ffffff');
                document.documentElement.style.setProperty('--home-hero-subtitle', '#f0f0f0');
                document.documentElement.style.setProperty('--home-section-title', '#ffffff');
                document.documentElement.style.setProperty('--home-feature-title', '#ffffff');
                document.documentElement.style.setProperty('--home-feature-text', '#f0f0f0');
                // HomePage form colors
                document.documentElement.style.setProperty('--home-form-label', '#ffffff');
                document.documentElement.style.setProperty('--home-form-input-text', '#ffffff');
                document.documentElement.style.setProperty('--home-form-select-text', '#ffffff');
                document.documentElement.style.setProperty('--home-form-option-bg', '#ffffff');
                document.documentElement.style.setProperty('--home-form-option-text', '#1a1a1a');
            }
        };

        // Update immediately
        updateBackground().catch(console.error);

        // Update every second to keep in sync
        const interval = setInterval(() => {
            updateBackground().catch(console.error);
        }, 1000);

        // Also calculate animation delay for CSS fallback
        const startTime = getSyncTime();
        const elapsed = Date.now() - startTime;
        const delay = -(elapsed % TOTAL_DURATION) / 1000;
        document.documentElement.style.setProperty('--bg-animation-delay', `${delay}s`);

        return () => clearInterval(interval);
    }, []);

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
