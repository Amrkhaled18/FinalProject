/**
 * API service functions for backend communication
 */
const API_BASE_URL = 'http://localhost:5207/api'; // Your ASP.NET Core backend

/**
 * Generic API call function with error handling
 */
const apiCall = async (endpoint, options = {}) => {
    try {
        const url = `${API_BASE_URL}${endpoint}`;
        console.log(`API Call: ${url}`);

        const response = await fetch(url, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers,
            },
            ...options,
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log(`API Response for ${endpoint}:`, data);

        // Handle both formats: direct data array or wrapped in success/data
        if (Array.isArray(data)) {
            return { success: true, data };
        } else if (data.success !== undefined) {
            return data; // Already in correct format
        } else {
            return { success: true, data };
        }
    } catch (error) {
        console.error(`API Error for ${endpoint}:`, error);
        return {
            success: false,
            message: error.message,
            data: []
        };
    }
};

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
    const response = await apiCall('/locations');

    // Transform data to match frontend expectations
    if (response.success && response.data) {
        const transformedData = response.data.map(location => ({
            id: location.locationID || location.LocationID || location.id,
            name: location.name || location.Name,
            category: mapCategoryToFrontend(location.category || location.Category),
            address: location.address || location.Address || 'Cairo, Egypt',
            description: location.description || location.Description,

            // Rating/Attributes mapping
            Attributes: location.rating || location.Rating || location.attributes || location.Attributes || 4.0,
            rating: location.rating || location.Rating || location.attributes || location.Attributes || 4.0,

            // Price level
            priceLevel: location.priceLevel || location.PriceLevel || getPriceLevel(location.category || location.Category),

            // Images - handle different image field names
            image_1: getImageUrl(location),
            image: getImageUrl(location),
            imageUrl: getImageUrl(location),

            // Status
            openStatus: location.openStatus || location.OpenStatus || 'Open Now',

            // Hotel-specific fields
            hotel_name: isHotelCategory(location.category || location.Category) ? (location.name || location.Name) : null,
            hotel_id: isHotelCategory(location.category || location.Category) ? (location.locationID || location.LocationID || location.id) : null,
            price_per_night: location.pricePerNight || location.PricePerNight || location.price_per_night,
            currency: location.currency || 'EGP',
            review_count: location.reviewCount || location.ReviewCount || location.review_count || 0,
            booking_link: location.bookingLink || location.BookingLink || location.booking_link,

            // Coordinates
            latitude: location.latitude || location.Latitude || getDefaultLatitude(),
            longitude: location.longitude || location.Longitude || getDefaultLongitude()
        }));

        return { success: true, data: transformedData };
    }

    return response;
};

export const getPlacesByCategory = async (category) => {
    if (!category) {
        return await getPlaces();
    }

    // Use specific category endpoints that match your backend
    const categoryEndpoint = getCategoryEndpoint(category);
    const response = await apiCall(categoryEndpoint);

    // Transform data to match frontend expectations
    if (response.success && response.data) {
        const transformedData = response.data.map(location => ({
            id: location.locationID || location.LocationID || location.id,
            name: location.name || location.Name,
            category: category, // Use the requested frontend category
            address: location.address || location.Address || 'Cairo, Egypt',
            description: location.description || location.Description,

            // Rating/Attributes mapping
            Attributes: location.rating || location.Rating || location.attributes || location.Attributes || 4.0,
            rating: location.rating || location.Rating || location.attributes || location.Attributes || 4.0,

            // Price level
            priceLevel: location.priceLevel || location.PriceLevel || getPriceLevel(location.category || location.Category),

            // Images
            image_1: getImageUrl(location),
            image: getImageUrl(location),
            imageUrl: getImageUrl(location),

            // Status
            openStatus: location.openStatus || location.OpenStatus || 'Open Now',

            // Hotel-specific fields
            hotel_name: category === 'hotels' ? (location.name || location.Name) : null,
            hotel_id: category === 'hotels' ? (location.locationID || location.LocationID || location.id) : null,
            price_per_night: location.pricePerNight || location.PricePerNight || location.price_per_night,
            currency: location.currency || 'EGP',
            review_count: location.reviewCount || location.ReviewCount || location.review_count || 0,
            booking_link: location.bookingLink || location.BookingLink || location.booking_link,

            // Coordinates
            latitude: location.latitude || location.Latitude || getDefaultLatitude(),
            longitude: location.longitude || location.Longitude || getDefaultLongitude()
        }));

        return { success: true, data: transformedData };
    }

    return response;
};

export const getPlaceById = async (placeId) => {
    const response = await apiCall(`/locations/${placeId}`);

    // Transform single location data
    if (response.success && response.data) {
        const location = response.data;
        const transformedData = {
            id: location.locationID || location.LocationID || location.id,
            name: location.name || location.Name,
            category: mapCategoryToFrontend(location.category || location.Category),
            address: location.address || location.Address || 'Cairo, Egypt',
            description: location.description || location.Description,

            Attributes: location.rating || location.Rating || location.attributes || location.Attributes || 4.0,
            rating: location.rating || location.Rating || location.attributes || location.Attributes || 4.0,
            priceLevel: location.priceLevel || location.PriceLevel || getPriceLevel(location.category || location.Category),

            image_1: getImageUrl(location),
            image: getImageUrl(location),
            imageUrl: getImageUrl(location),

            openStatus: location.openStatus || location.OpenStatus || 'Open Now',

            hotel_name: isHotelCategory(location.category || location.Category) ? (location.name || location.Name) : null,
            hotel_id: isHotelCategory(location.category || location.Category) ? (location.locationID || location.LocationID || location.id) : null,
            price_per_night: location.pricePerNight || location.PricePerNight || location.price_per_night,
            currency: location.currency || 'EGP',
            review_count: location.reviewCount || location.ReviewCount || location.review_count || 0,
            booking_link: location.bookingLink || location.BookingLink || location.booking_link,

            latitude: location.latitude || location.Latitude || getDefaultLatitude(),
            longitude: location.longitude || location.Longitude || getDefaultLongitude()
        };

        return { success: true, data: transformedData };
    }

    return response;
};

export const getFirstImagesPerLocation = async () => {
    return await apiCall('/images/first');
};

/**
 * ========= HELPER FUNCTIONS =========
 */

// Map backend categories to frontend categories
const mapCategoryToFrontend = (backendCategory) => {
    if (!backendCategory) return 'monuments';

    const categoryMap = {
        'Restaurant': 'restaurants',
        'Cafe': 'cafes',
        'Hotel': 'hotels',
        'Church': 'monuments',
        'Mosque': 'monuments',
        'Historical': 'monuments',
        'Museum': 'monuments',
        'Palace': 'monuments',
        'Fortress': 'monuments',
        'Shrine': 'monuments',
        'Fountain': 'monuments',
        'Market': 'monuments',
        'School': 'monuments'
    };

    return categoryMap[backendCategory] || 'monuments';
};

// Get category endpoint for API calls
const getCategoryEndpoint = (frontendCategory) => {
    const endpointMap = {
        'restaurants': '/locations?category=Restaurant',
        'cafes': '/locations?category=Cafe',
        'hotels': '/locations?category=Hotel',
        'monuments': '/locations' // Get all and filter monuments client-side
    };

    return endpointMap[frontendCategory] || '/locations';
};

// Check if category is hotel-related
const isHotelCategory = (category) => {
    return category && (category.toLowerCase() === 'hotel' || category.toLowerCase() === 'hotels');
};

// Get price level based on category
const getPriceLevel = (category) => {
    if (!category) return 2;

    const priceLevelMap = {
        'Hotel': 3,
        'Restaurant': 2,
        'Cafe': 1,
        'Historical': 1,
        'Museum': 1,
        'Church': 1,
        'Mosque': 1,
        'Palace': 2,
        'Fortress': 1
    };

    return priceLevelMap[category] || 2;
};

// Get image URL from location object
const getImageUrl = (location) => {
    // Check multiple possible image field names
    if (location.images && location.images.length > 0) {
        return location.images[0].imageURL || location.images[0].ImageURL;
    }

    // Check for direct image fields
    const imageFields = ['imageURL', 'ImageURL', 'image_1', 'image', 'imageUrl'];
    for (const field of imageFields) {
        if (location[field]) {
            return location[field];
        }
    }

    // Fallback to default image
    const category = location.category || location.Category || 'place';
    return getDefaultPlaceImage(mapCategoryToFrontend(category));
};

// Get default place image based on category
const getDefaultPlaceImage = (category = 'place') => {
    const defaultImages = {
        'restaurants': 'https://source.unsplash.com/300x200/?restaurant,food,cairo',
        'cafes': 'https://source.unsplash.com/300x200/?cafe,coffee,cairo',
        'hotels': 'https://source.unsplash.com/300x200/?hotel,bedroom,cairo',
        'monuments': 'https://source.unsplash.com/300x200/?monument,historical,cairo'
    };

    return defaultImages[category] || `https://source.unsplash.com/300x200/?${category},cairo`;
};

// Default coordinates (Cairo center)
const getDefaultLatitude = () => 30.0444;
const getDefaultLongitude = () => 31.2357;

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

/**
 * ========= ADDITIONAL HELPER EXPORTS =========
 */
export const validateApiResponse = (response) => {
    if (!response) {
        return { success: false, data: [], message: 'No response received' };
    }

    if (!response.success) {
        return { success: false, data: [], message: response.message || 'API call failed' };
    }

    if (!Array.isArray(response.data)) {
        console.warn('API response data is not an array:', response.data);
        return { success: true, data: [], message: 'Invalid data format' };
    }

    return response;
};

// Export all functions as default
export default {
    getPlaces,
    getPlacesByCategory,
    getPlaceById,
    getFirstImagesPerLocation,
    validateApiResponse,
    mapCategoryToFrontend,
    getDefaultPlaceImage
};