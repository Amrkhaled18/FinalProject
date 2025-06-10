
/**
 * API service functions for backend communication
 */

const API_BASE_URL = 'http://localhost:5207/api'; // Updated to point to ASP.NET Core backend
    
/**
 * ========= AUTH SERVICES =========
 */

// Login admin (Placeholder for future implementation)
export const loginAdmin = async (username, password) => {
    // Replace with actual API call when available
    if (username === 'admin' && password === 'admin123') {
        const token = 'mock-jwt-token-xyz123';
        localStorage.setItem('token', token);
        localStorage.setItem('isAuthenticated', 'true');
        localStorage.setItem('userRole', 'admin');
        return { success: true, token };
    }
    return { success: false, message: 'Invalid credentials' };
};

export const logoutAdmin = async () => {
    localStorage.removeItem('token');
    localStorage.removeItem('isAuthenticated');
    localStorage.removeItem('userRole');
    return { success: true };
};

/**
 * ========= PLACES SERVICES =========
 */

export const getPlaces = async () => {
    try {
        const res = await fetch(`${API_BASE_URL}/locations`);
        if (!res.ok) throw new Error('Failed to fetch locations');
        const data = await res.json();
        return { success: true, data };
    } catch (err) {
        return { success: false, message: err.message };
    }
};

export const getPlacesByCategory = async (category) => {
    try {
        const res = await fetch(`${API_BASE_URL}/locations?category=${category}`);
        if (!res.ok) throw new Error('Failed to fetch category');
        const data = await res.json();
        return { success: true, data };
    } catch (err) {
        return { success: false, message: err.message };
    }
};

export const getPlaceById = async (placeId) => {
    try {
        const res = await fetch(`${API_BASE_URL}/locations/${placeId}`);
        if (!res.ok) throw new Error('Failed to fetch place');
        const data = await res.json();
        return { success: true, data };
    } catch (err) {
        return { success: false, message: err.message };
    }
};

/**
 * ========= ANALYTICS SERVICES (Mock) =========
 */

export const getDashboardAnalytics = async () => {
    return {
        success: true,
        data: {
            totalVisits: 0,
            newUsers: 0,
            activeUsers: 0
        }
    };
};
