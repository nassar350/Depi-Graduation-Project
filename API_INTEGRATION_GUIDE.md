# Eventify API - Frontend Integration Guide

## Base URL
- Development: `https://localhost:7194` or `http://localhost:5194`
- Make sure to check the actual port when running the API

## CORS Configuration
The API is configured to accept requests from common frontend development ports:
- React (Create React App): `http://localhost:3000`
- React (Vite): `http://localhost:5173`
- Angular: `http://localhost:4200`
- Vue: `http://localhost:8080`

## Authentication Endpoints

### 1. Register User
**POST** `/api/auth/register`

**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "password": "Password123",
  "phone": "01012345678"
}
```

**Validation Rules:**
- **Name**: 2-50 characters, only letters and single spaces
- **Email**: Valid email format, 5-100 characters
- **Password**: 6-50 characters, must contain at least one lowercase, uppercase, and digit
- **Phone**: Egyptian mobile number (11 digits starting with 010, 011, 012, or 015)

**Success Response (200):**
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "id": 1,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "01012345678",
    "role": "User"
  },
  "errors": []
}
```

**Error Response (400):**
```json
{
  "success": false,
  "message": "Email already registered",
  "data": null,
  "errors": ["This email is already registered."]
}
```

### 2. Login User
**POST** `/api/auth/login`

**Request Body:**
```json
{
  "email": "john.doe@example.com",
  "password": "Password123"
}
```

**Success Response (200):**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "01012345678",
    "role": "User"
  },
  "expiresAt": "2024-01-02T12:00:00Z"
}
```

**Error Response (401):**
```json
{
  "success": false,
  "message": "Invalid email or password",
  "token": null,
  "user": null,
  "expiresAt": null
}
```

### 3. Logout
**POST** `/api/auth/logout`

**Success Response (200):**
```json
{
  "success": true,
  "message": "Logout successful. Please remove the token from client storage.",
  "data": null,
  "errors": []
}
```

## Frontend Implementation Examples

### JavaScript/React Example

```javascript
const API_BASE_URL = 'https://localhost:7194/api';

// Register function
const register = async (userData) => {
  try {
    const response = await fetch(`${API_BASE_URL}/auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(userData),
    });
    
    const data = await response.json();
    
    if (data.success) {
      console.log('Registration successful:', data.data);
      return { success: true, user: data.data };
    } else {
      console.error('Registration failed:', data.errors);
      return { success: false, errors: data.errors };
    }
  } catch (error) {
    console.error('Network error:', error);
    return { success: false, errors: ['Network error occurred'] };
  }
};

// Login function
const login = async (credentials) => {
  try {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(credentials),
    });
    
    const data = await response.json();
    
    if (data.success) {
      // Store token in localStorage or secure storage
      localStorage.setItem('authToken', data.token);
      localStorage.setItem('user', JSON.stringify(data.user));
      localStorage.setItem('tokenExpiry', data.expiresAt);
      
      return { success: true, user: data.user, token: data.token };
    } else {
      console.error('Login failed:', data.message);
      return { success: false, error: data.message };
    }
  } catch (error) {
    console.error('Network error:', error);
    return { success: false, error: 'Network error occurred' };
  }
};

// Logout function
const logout = async () => {
  try {
    // Call logout endpoint (optional since JWT is stateless)
    await fetch(`${API_BASE_URL}/auth/logout`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      },
    });
  } catch (error) {
    console.error('Logout API call failed:', error);
  } finally {
    // Always clear local storage
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
    localStorage.removeItem('tokenExpiry');
  }
};

// Function to make authenticated requests
const makeAuthenticatedRequest = async (url, options = {}) => {
  const token = localStorage.getItem('authToken');
  const tokenExpiry = localStorage.getItem('tokenExpiry');
  
  // Check if token is expired
  if (tokenExpiry && new Date(tokenExpiry) < new Date()) {
    logout();
    throw new Error('Token expired');
  }
  
  const headers = {
    'Content-Type': 'application/json',
    ...options.headers,
  };
  
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }
  
  return fetch(url, {
    ...options,
    headers,
  });
};
```

### React Hook Example

```javascript
import { useState, useEffect, createContext, useContext } from 'react';

const AuthContext = createContext();

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check for existing authentication on component mount
    const storedToken = localStorage.getItem('authToken');
    const storedUser = localStorage.getItem('user');
    const tokenExpiry = localStorage.getItem('tokenExpiry');

    if (storedToken && storedUser && tokenExpiry) {
      if (new Date(tokenExpiry) > new Date()) {
        setToken(storedToken);
        setUser(JSON.parse(storedUser));
      } else {
        // Token expired, clear storage
        localStorage.removeItem('authToken');
        localStorage.removeItem('user');
        localStorage.removeItem('tokenExpiry');
      }
    }
    setLoading(false);
  }, []);

  const loginUser = async (credentials) => {
    const result = await login(credentials);
    if (result.success) {
      setUser(result.user);
      setToken(result.token);
    }
    return result;
  };

  const logoutUser = async () => {
    await logout();
    setUser(null);
    setToken(null);
  };

  const value = {
    user,
    token,
    loading,
    login: loginUser,
    logout: logoutUser,
    isAuthenticated: !!user && !!token,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
```

## Important Notes

1. **Token Storage**: Store JWT tokens securely. Consider using httpOnly cookies for production.

2. **Token Expiration**: Current token expiration is set to 24 hours (1440 minutes). Check expiration before making requests.

3. **Error Handling**: Always check the `success` field in responses and handle errors appropriately.

4. **HTTPS**: Use HTTPS in production for security.

5. **Rate Limiting**: Consider implementing rate limiting on the frontend to prevent abuse.

6. **Validation**: The API performs server-side validation, but also implement client-side validation for better UX.

## Testing the API

You can test the API endpoints using:
- Postman
- curl commands
- Browser's Network tab
- Swagger UI (available at `/swagger` when running in development)

### Example curl commands:

```bash
# Register
curl -X POST "https://localhost:7194/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john.doe@example.com",
    "password": "Password123",
    "phone": "01012345678"
  }'

# Login
curl -X POST "https://localhost:7194/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "password": "Password123"
  }'
```