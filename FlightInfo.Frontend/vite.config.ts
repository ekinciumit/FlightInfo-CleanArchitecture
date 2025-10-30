import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
    plugins: [
        react({
            babel: {
                plugins: [['babel-plugin-react-compiler']],
            },
        }),
    ],

    // 🔹 Backend proxy ayarı
    server: {
        proxy: {
            // Frontend'te /api ile başlayan tüm istekleri yönlendir
            '/api': {
                target: 'http://localhost:7104', // Backend adresin
                changeOrigin: true,               // Host header'ını backend'e uygun hale getir
                secure: false,                    // HTTPS sertifikasını development'ta kontrol etme
            },
        },
    },
})

