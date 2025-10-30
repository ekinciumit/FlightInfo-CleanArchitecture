import type { ApiError } from "../types";

// API hatalarını parse et
export function parseApiError(error: any): ApiError {
    if (error.response) {
        // Axios error
        const status = error.response.status;
        const message = error.response.data?.message || error.response.data?.error || 'Sunucu hatası';

        return {
            message: getErrorMessage(status, message),
            status,
            details: error.response.data
        };
    } else if (error.request) {
        // Network error
        return {
            message: 'Sunucuya bağlanılamıyor. İnternet bağlantınızı kontrol edin.',
            status: 0
        };
    } else {
        // Diğer hatalar
        return {
            message: error.message || 'Bilinmeyen bir hata oluştu'
        };
    }
}

// HTTP status kodlarına göre kullanıcı dostu mesajlar
function getErrorMessage(status: number, originalMessage: string): string {
    switch (status) {
        case 400:
            return 'Geçersiz istek. Lütfen bilgilerinizi kontrol edin.';
        case 401:
            return 'Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.';
        case 403:
            return 'Bu işlem için yetkiniz bulunmuyor.';
        case 404:
            return 'Aranan kaynak bulunamadı.';
        case 409:
            return 'Bu işlem çakışma yaratıyor. Lütfen tekrar deneyin.';
        case 422:
            return originalMessage || 'Gönderilen veriler geçersiz.';
        case 429:
            return 'Çok fazla istek gönderdiniz. Lütfen bekleyin.';
        case 500:
            return 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.';
        case 503:
            return 'Servis geçici olarak kullanılamıyor.';
        default:
            return originalMessage || 'Bir hata oluştu.';
    }
}

// Toast ile hata göster
export function showErrorToast(error: ApiError) {
    const title = error.status ? `Hata (${error.status})` : 'Hata';
    (window as any).showToast?.error(title, error.message);
}

// Başarı mesajı göster
export function showSuccessToast(message: string, title: string = 'Başarılı') {
    (window as any).showToast?.success(title, message);
}

// Uyarı mesajı göster
export function showWarningToast(message: string, title: string = 'Uyarı') {
    (window as any).showToast?.warning(title, message);
}

