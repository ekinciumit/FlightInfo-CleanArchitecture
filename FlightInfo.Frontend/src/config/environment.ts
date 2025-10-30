// Environment configuration
export const config = {
    // API Configuration
    API_URL: import.meta.env.VITE_API_URL || '/api',
    API_BASE_URL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:7104/api',

    // App Configuration
    APP_NAME: import.meta.env.VITE_APP_NAME || 'FlightInfo',
    APP_VERSION: import.meta.env.VITE_APP_VERSION || '1.0.0',

    // Development
    IS_DEV: import.meta.env.DEV,
    IS_PROD: import.meta.env.PROD,

    // Feature flags
    ENABLE_DEBUG: import.meta.env.VITE_DEV_MODE === 'true' || import.meta.env.DEV,
};

// API helper functions
export const getApiUrl = (endpoint: string = '') => {
    return `${config.API_BASE_URL}${endpoint}`;
};

export const getFullApiUrl = (endpoint: string = '') => {
    return `${config.API_BASE_URL}${endpoint}`;
};

// Debug helper
export const debugLog = (...args: any[]) => {
    if (config.ENABLE_DEBUG) {
        console.log('[DEBUG]', ...args);
    }
};

