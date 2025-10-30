import api from "./api";

// Login response tipi
export interface LoginResponse {
    token: string;
    user: {
        id: number;
        email: string;
        fullName: string;
        role: string;
    };
}

// 🔑 Login isteği
export async function login(email: string, password: string): Promise<LoginResponse> {
    const response = await api.post<LoginResponse>("/auth/login", { email, password });

    const { token, user } = response.data;

    // ✅ Token & user localStorage’a kaydediliyor
    localStorage.setItem("token", token);
    localStorage.setItem("user", JSON.stringify(user));

    return response.data;
}

// 📝 Register isteği
export async function register(
    fullName: string,
    email: string,
    password: string
): Promise<void> {
    await api.post("/auth/register", { fullName, email, password });
}

// 🚪 Logout işlemi (bonus)
export function logout(): void {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
}

