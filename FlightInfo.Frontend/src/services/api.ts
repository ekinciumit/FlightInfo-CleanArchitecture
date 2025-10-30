import axios from "axios";
import type { InternalAxiosRequestConfig } from "axios";
import { getApiUrl } from "../config/environment";

// 🔹 Axios instance oluştur
const api = axios.create({
    baseURL: getApiUrl(),
    headers: {
        "Content-Type": "application/json",
    },
});

// 🔹 Her istekten önce token ekle
api.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
        const token = localStorage.getItem("token");
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

export default api;

