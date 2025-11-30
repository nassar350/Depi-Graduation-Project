# API Reference Documentation

## Base URL
**Production:** `https://eventify.runasp.net/api`  
**Development:** `https://localhost:7105/api`

All endpoints require HTTPS. The API follows RESTful conventions and returns JSON responses.

---

## Table of Contents
1. [Authentication](#authentication)
2. [Events](#events)
3. [Categories](#categories)
4. [Tickets](#tickets)
5. [Checkout & Payments](#checkout--payments)
6. [Users](#users)
7. [Error Handling](#error-handling)
8. [Response Codes](#response-codes)

---

## Authentication

### POST /api/Auth/register
Register a new user account.

**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!",
  "confirmPassword": "SecurePassword123!",
  "phoneNumber": "01012345678",
  "role": "User"
}
```

**Success Response (200 OK):**
```json
{
  "message": "User registered successfully",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "John Doe",
  "email": "john.doe@example.com"
}
```

**Error Response (400 Bad Request):**
```json
{
  "errors": {
    "Email": ["Email is already registered"],
    "Password": ["Password must contain at least one uppercase letter"]
  }
}
```

---

### POST /api/Auth/login
Authenticate user and receive JWT token.

**Request Body:**
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

**Success Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "John Doe",
  "email": "john.doe@example.com",
  "role": "User",
  "expiresAt": "2024-11-27T10:30:00Z"
}
```

**Error Response (401 Unauthorized):**
```json
{
  "message": "Invalid email or password"
}
```

**Authorization Header:**
For protected endpoints, include the JWT token:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Events

### GET /api/Events
Retrieve all events with pagination support.

**Query Parameters:**
- `page` (optional, default: 1) - Page number
- `pageSize` (optional, default: 10) - Items per page
- `categoryId` (optional) - Filter by category
- `search` (optional) - Search in event name/description

**Example Request:**
```
GET /api/Events?page=1&pageSize=10&search=music
```

**Success Response (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "name": "Summer Music Festival 2024",
      "description": "The biggest music event of the summer featuring top artists",
      "location": "Cairo Stadium",
      "startDate": "2024-07-15T18:00:00Z",
      "endDate": "2024-07-15T23:59:00Z",
      "imageUrl": "https://example.com/images/event1.jpg",
      "organizerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "organizerName": "EventPro Ltd",
      "categories": [
        {
          "id": 1,
          "name": "VIP",
          "price": 500.00,
          "seats": 100,
          "booked": 45
        },
        {
          "id": 2,
          "name": "Regular",
          "price": 200.00,
          "seats": 500,
          "booked": 320
        }
      ]
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 10,
  "totalPages": 5
}
```

---

### GET /api/Events/{id}
Retrieve a specific event by ID.

**Path Parameters:**
- `id` (integer, required) - Event ID

**Example Request:**
```
GET /api/Events/1
```

**Success Response (200 OK):**
```json
{
  "id": 1,
  "name": "Summer Music Festival 2024",
  "description": "The biggest music event of the summer featuring top artists from around the world.",
  "location": "Cairo Stadium",
  "startDate": "2024-07-15T18:00:00Z",
  "endDate": "2024-07-15T23:59:00Z",
  "imageUrl": "https://example.com/images/event1.jpg",
  "organizerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "organizerName": "EventPro Ltd",
  "organizerEmail": "contact@eventpro.com",
  "organizerPhone": "01012345678",
  "categories": [
    {
      "id": 1,
      "name": "VIP",
      "description": "Premium seating with exclusive perks",
      "price": 500.00,
      "seats": 100,
      "booked": 45,
      "availableSeats": 55
    },
    {
      "id": 2,
      "name": "Regular",
      "description": "Standard admission",
      "price": 200.00,
      "seats": 500,
      "booked": 320,
      "availableSeats": 180
    }
  ],
  "totalSeats": 600,
  "totalBooked": 365,
  "totalAvailable": 235
}
```

**Error Response (404 Not Found):**
```json
{
  "message": "Event not found",
  "eventId": 1
}
```

---

### POST /api/Events
Create a new event (Organizer only).

**Authorization Required:** Yes (Organizer role)

**Request Body:**
```json
{
  "name": "Tech Conference 2024",
  "description": "Annual technology conference featuring industry leaders",
  "location": "Cairo International Convention Center",
  "startDate": "2024-12-10T09:00:00Z",
  "endDate": "2024-12-10T18:00:00Z",
  "imageUrl": "https://example.com/images/tech-conf.jpg",
  "categories": [
    {
      "name": "Early Bird",
      "description": "Limited time discount tickets",
      "price": 150.00,
      "seats": 50
    },
    {
      "name": "Standard",
      "description": "Regular admission",
      "price": 250.00,
      "seats": 200
    }
  ]
}
```

**Success Response (201 Created):**
```json
{
  "id": 42,
  "message": "Event created successfully",
  "eventUrl": "/api/Events/42"
}
```

**Error Response (400 Bad Request):**
```json
{
  "errors": {
    "Name": ["Event name is required"],
    "StartDate": ["Start date must be in the future"],
    "Categories": ["At least one category is required"]
  }
}
```

---

## Categories

### GET /api/Categories/{id}
Retrieve a specific ticket category by ID.

**Path Parameters:**
- `id` (integer, required) - Category ID

**Example Request:**
```
GET /api/Categories/1
```

**Success Response (200 OK):**
```json
{
  "id": 1,
  "name": "VIP",
  "description": "Premium seating with exclusive perks",
  "price": 500.00,
  "seats": 100,
  "booked": 45,
  "availableSeats": 55,
  "eventId": 1,
  "eventName": "Summer Music Festival 2024"
}
```

**Error Response (404 Not Found):**
```json
{
  "message": "Category not found",
  "categoryId": 1
}
```

---

## Tickets

### GET /api/Tickets/category/{categoryId}
Retrieve all tickets for a specific category.

**Path Parameters:**
- `categoryId` (integer, required) - Category ID

**Example Request:**
```
GET /api/Tickets/category/1
```

**Success Response (200 OK):**
```json
{
  "categoryId": 1,
  "categoryName": "VIP",
  "totalSeats": 100,
  "totalBooked": 45,
  "tickets": [
    {
      "id": 1,
      "ticketNumber": "VIP-001",
      "status": "Available",
      "categoryId": 1,
      "categoryName": "VIP",
      "price": 500.00
    },
    {
      "id": 2,
      "ticketNumber": "VIP-002",
      "status": "Booked",
      "categoryId": 1,
      "categoryName": "VIP",
      "price": 500.00,
      "bookingId": 15,
      "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }
  ]
}
```

**Error Response (404 Not Found):**
```json
{
  "message": "No tickets found for this category",
  "categoryId": 1
}
```

---

## Checkout & Payments

### POST /api/CheckOut
Create a booking and initiate Stripe payment.

**Authorization Required:** Yes

**Request Body:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "eventId": 1,
  "categoryId": 1,
  "categoryName": "VIP",
  "quantity": 2,
  "totalAmount": 1000.00,
  "customerName": "John Doe",
  "customerEmail": "john.doe@example.com",
  "customerPhone": "01012345678",
  "promoCode": "SUMMER2024"
}
```

**Field Descriptions:**
- `userId` (string, required) - User's unique identifier
- `eventId` (integer, required) - Event ID to book
- `categoryId` (integer, required) - Ticket category ID
- `categoryName` (string, required) - Category name (e.g., "VIP", "Regular")
- `quantity` (integer, required) - Number of tickets (min: 1, max: 10)
- `totalAmount` (decimal, required) - Total price (quantity × ticket price)
- `customerName` (string, required) - Attendee full name
- `customerEmail` (string, required) - Attendee email for confirmation
- `customerPhone` (string, required) - Contact phone number
- `promoCode` (string, optional) - Promotional discount code

**Success Response (200 OK):**
```json
{
  "success": true,
  "bookingId": 42,
  "clientSecret": "pi_3Abc123_secret_XyZ789",
  "stripePublishableKey": "pk_test_51Abc...",
  "amount": 1000.00,
  "currency": "egp",
  "message": "Booking created successfully. Complete payment to confirm.",
  "booking": {
    "id": 42,
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "eventId": 1,
    "eventName": "Summer Music Festival 2024",
    "categoryId": 1,
    "categoryName": "VIP",
    "quantity": 2,
    "totalAmount": 1000.00,
    "status": "Pending",
    "bookingDate": "2024-11-26T14:30:00Z",
    "tickets": [
      {
        "ticketNumber": "VIP-046",
        "status": "Reserved"
      },
      {
        "ticketNumber": "VIP-047",
        "status": "Reserved"
      }
    ]
  }
}
```

**Error Response (400 Bad Request - Validation):**
```json
{
  "success": false,
  "errors": {
    "Quantity": ["Quantity must be between 1 and 10"],
    "CustomerEmail": ["Invalid email format"],
    "TotalAmount": ["Amount mismatch: expected 1000.00, received 950.00"]
  }
}
```

**Error Response (400 Bad Request - Business Logic):**
```json
{
  "success": false,
  "message": "Not enough available seats",
  "details": {
    "requested": 5,
    "available": 3,
    "categoryName": "VIP"
  }
}
```

**Error Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Event or category not found",
  "eventId": 1,
  "categoryId": 99
}
```

**Error Response (401 Unauthorized):**
```json
{
  "message": "Authentication required. Please log in."
}
```

---

### POST /api/Payments/confirm
Confirm payment after Stripe payment intent succeeds.

**Authorization Required:** Yes

**Request Body:**
```json
{
  "bookingId": 42,
  "paymentIntentId": "pi_3Abc123XyZ789"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Payment confirmed successfully",
  "bookingId": 42,
  "status": "Confirmed",
  "tickets": [
    {
      "ticketNumber": "VIP-046",
      "status": "Confirmed",
      "qrCode": "https://example.com/qr/VIP-046.png"
    },
    {
      "ticketNumber": "VIP-047",
      "status": "Confirmed",
      "qrCode": "https://example.com/qr/VIP-047.png"
    }
  ],
  "confirmationEmail": "Sent to john.doe@example.com"
}
```

---

### POST /api/stripe/webhook
Stripe webhook endpoint for payment events (internal use).

**Headers:**
```
Stripe-Signature: t=1234567890,v1=abc123...
```

**Event Types Handled:**
- `payment_intent.succeeded` - Payment completed successfully
- `payment_intent.payment_failed` - Payment failed
- `payment_intent.canceled` - Payment canceled by user

**Response (200 OK):**
```json
{
  "received": true
}
```

---

## Users

### GET /api/User/profile
Get current user's profile (authenticated user).

**Authorization Required:** Yes

**Success Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "John Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "01012345678",
  "role": "User",
  "createdAt": "2024-10-15T12:00:00Z",
  "bookings": [
    {
      "id": 42,
      "eventName": "Summer Music Festival 2024",
      "categoryName": "VIP",
      "quantity": 2,
      "totalAmount": 1000.00,
      "status": "Confirmed",
      "bookingDate": "2024-11-26T14:30:00Z"
    }
  ],
  "totalBookings": 5,
  "totalSpent": 4500.00
}
```

---

### GET /api/User/bookings
Get all bookings for the authenticated user.

**Authorization Required:** Yes

**Query Parameters:**
- `status` (optional) - Filter by status: "Pending", "Confirmed", "Cancelled"

**Example Request:**
```
GET /api/User/bookings?status=Confirmed
```

**Success Response (200 OK):**
```json
{
  "bookings": [
    {
      "id": 42,
      "eventId": 1,
      "eventName": "Summer Music Festival 2024",
      "eventDate": "2024-07-15T18:00:00Z",
      "categoryName": "VIP",
      "quantity": 2,
      "totalAmount": 1000.00,
      "status": "Confirmed",
      "bookingDate": "2024-11-26T14:30:00Z",
      "paymentStatus": "Paid",
      "tickets": [
        {
          "ticketNumber": "VIP-046",
          "qrCode": "https://example.com/qr/VIP-046.png"
        },
        {
          "ticketNumber": "VIP-047",
          "qrCode": "https://example.com/qr/VIP-047.png"
        }
      ]
    }
  ],
  "totalCount": 1
}
```

---

## Error Handling

### Standard Error Response Format

All error responses follow this structure:

```json
{
  "success": false,
  "message": "Human-readable error message",
  "errors": {
    "FieldName": ["Validation error message 1", "Validation error message 2"]
  },
  "timestamp": "2024-11-26T14:30:00Z",
  "path": "/api/CheckOut"
}
```

### Validation Errors

When request validation fails (400 Bad Request):

```json
{
  "errors": {
    "Email": ["Email is required", "Email format is invalid"],
    "Password": ["Password must be at least 8 characters"],
    "Quantity": ["Quantity must be between 1 and 10"]
  }
}
```

### Common Error Messages

| Error | Code | Message |
|-------|------|---------|
| Missing Auth Token | 401 | "Authentication required. Please log in." |
| Invalid Token | 401 | "Invalid or expired token." |
| Insufficient Permissions | 403 | "You don't have permission to access this resource." |
| Resource Not Found | 404 | "The requested resource was not found." |
| Validation Failed | 400 | "Validation failed. Check errors field." |
| Server Error | 500 | "An unexpected error occurred. Please try again." |
| Payment Failed | 400 | "Payment processing failed. Please try again." |
| Sold Out | 400 | "Not enough available seats for this category." |

---

## Response Codes

| Status Code | Meaning | Usage |
|-------------|---------|-------|
| 200 OK | Success | Request completed successfully |
| 201 Created | Resource Created | New resource created (e.g., event, booking) |
| 400 Bad Request | Invalid Input | Validation errors or business logic failures |
| 401 Unauthorized | Not Authenticated | Missing or invalid JWT token |
| 403 Forbidden | Access Denied | User lacks required permissions |
| 404 Not Found | Resource Missing | Event, category, or ticket not found |
| 409 Conflict | Duplicate/Conflict | Email already exists, booking conflict |
| 500 Server Error | Internal Error | Unexpected server-side error |

---

## Rate Limiting

- **Public endpoints:** 100 requests per minute per IP
- **Authenticated endpoints:** 200 requests per minute per user
- **Checkout endpoint:** 10 requests per minute per user

**Rate Limit Headers:**
```
X-RateLimit-Limit: 200
X-RateLimit-Remaining: 150
X-RateLimit-Reset: 1732632600
```

**Rate Limit Exceeded Response (429 Too Many Requests):**
```json
{
  "message": "Rate limit exceeded. Please try again later.",
  "retryAfter": 60
}
```

---

## CORS Configuration

**Allowed Origins:**
- `https://eventiifyy.netlify.app` (Production Frontend)
- `http://localhost:5500` (Development)
- `http://127.0.0.1:5500` (Development)

**Allowed Methods:** GET, POST, PUT, DELETE, OPTIONS

**Allowed Headers:** Content-Type, Authorization

---

## Frontend Integration Examples

### Example: Fetch Events with Error Handling

```javascript
const API_BASE_URL = 'https://eventify.runasp.net/api';

async function fetchEvents(page = 1) {
  try {
    const response = await fetch(`${API_BASE_URL}/Events?page=${page}&pageSize=10`);
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching events:', error);
    return { data: [], totalCount: 0 };
  }
}
```

### Example: Checkout with Authentication

```javascript
async function createBooking(bookingData) {
  const token = localStorage.getItem('token') || 
                localStorage.getItem('eventify_token') || 
                localStorage.getItem('authToken');
  
  if (!token) {
    throw new Error('Please log in to complete booking');
  }
  
  try {
    const response = await fetch(`${API_BASE_URL}/CheckOut`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(bookingData)
    });
    
    const result = await response.json();
    
    if (!response.ok) {
      // Handle validation errors
      if (result.errors) {
        const errorMessages = Object.values(result.errors).flat().join(', ');
        throw new Error(errorMessages);
      }
      throw new Error(result.message || 'Booking failed');
    }
    
    return result;
  } catch (error) {
    console.error('Checkout error:', error);
    throw error;
  }
}
```

### Example: Stripe Payment Integration

```javascript
async function processPayment(clientSecret) {
  const stripe = Stripe('pk_test_51Abc...');
  const elements = stripe.elements();
  const cardElement = elements.create('card');
  cardElement.mount('#card-element');
  
  const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
    payment_method: {
      card: cardElement,
      billing_details: {
        name: document.getElementById('customerName').value,
        email: document.getElementById('customerEmail').value
      }
    }
  });
  
  if (error) {
    throw new Error(error.message);
  }
  
  if (paymentIntent.status === 'succeeded') {
    return { success: true, paymentIntentId: paymentIntent.id };
  }
}
```

---

## Notes

- All dates are in ISO 8601 format (UTC): `YYYY-MM-DDTHH:mm:ssZ`
- Prices are in Egyptian Pounds (EGP) with 2 decimal places
- JWT tokens expire after 24 hours
- Payment processing uses Stripe API v2023-10-16
- QR codes are generated for confirmed tickets
- Email confirmations are sent asynchronously

---

**Last Updated:** November 26, 2024  
**API Version:** 1.0  
**Support:** For API issues, contact the development team or check the project repository.
