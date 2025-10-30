// User interface
export interface User {
    id: number;
    email: string;
    fullName: string;
    role: string;
    phone?: string;
    createdAt?: string;
}

// Auth response interface
export interface AuthResponse {
    token: string;
    user: User;
}

// API Error interface
export interface ApiError {
    message: string;
    status?: number;
    details?: any;
}

// Toast types
export type ToastType = "success" | "error" | "warning" | "info";

export interface ToastData {
    id: string;
    type: ToastType;
    title: string;
    message: string;
    duration?: number;
}

// Global window interface extensions
declare global {
    interface Window {
        showToast: {
            success: (title: string, message: string, duration?: number) => void;
            error: (title: string, message: string, duration?: number) => void;
            warning: (title: string, message: string, duration?: number) => void;
            info: (title: string, message: string, duration?: number) => void;
        };
    }
}

