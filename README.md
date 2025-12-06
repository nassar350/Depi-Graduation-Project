# Eventify - Event Management & Booking Platform

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat&logo=microsoft-sql-server)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=flat&logo=javascript&logoColor=black)
![Stripe](https://img.shields.io/badge/Stripe-008CDD?style=flat&logo=stripe)

## 📋 Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [API Documentation](#api-documentation)
- [Authentication & Authorization](#authentication--authorization)
- [Payment Integration](#payment-integration)
- [Admin Panel](#admin-panel)
- [Frontend Pages](#frontend-pages)
- [Configuration](#configuration)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License](#license)

---

## 🎯 Overview

**Eventify** is a comprehensive full-stack event management and booking platform built with ASP.NET Core and vanilla JavaScript. It enables users to discover events, book tickets, process payments securely through Stripe, and provides administrators with powerful management tools.

### Key Highlights
- 🎫 Complete event lifecycle management (create, browse, book, attend)
- 💳 Secure payment processing with Stripe integration
- 🔐 JWT-based authentication with role-based access control
- 📊 Real-time admin dashboard with statistics and analytics
- 📱 Responsive design for mobile and desktop
- 🎟️ PDF ticket generation and QR code verification
- 📧 Email notifications for bookings and payments
- 🔍 Advanced search and filtering capabilities

---

## ✨ Features

### For Users
- **Event Discovery**
  - Browse upcoming events with rich details
  - Search by name, category, location, or date
  - Filter by event categories (Music, Sports, Arts, Food, Technology, etc.)
  - View event details including photos, descriptions, and ticket availability

- **Booking System**
  - Select ticket categories (VIP, Regular, etc.) with real-time availability
  - Add multiple tickets to cart
  - Secure checkout process with payment confirmation
  - View booking history and status

- **Payment Processing**
  - Stripe integration for credit/debit card payments
  - Support for multiple currencies
  - Payment status tracking (Paid, Pending, Cancelled, Refunded)
  - Automated payment confirmations

- **Ticket Management**
  - Download tickets as PDF with QR codes
  - Email delivery of tickets
  - Ticket verification system for event organizers
  - View all purchased tickets in dashboard

- **User Dashboard**
  - View all bookings and their statuses
  - Track upcoming and past events
  - Access purchased tickets
  - Update profile information

### For Event Organizers
- **Event Management**
  - Create events with detailed information
  - Upload event photos
  - Set multiple ticket categories with pricing
  - Define capacity and availability
  - Support for online events with Zoom integration

- **Analytics**
  - Track ticket sales and revenue
  - View booking statistics
  - Monitor event attendance
  - Revenue reports per event

### For Administrators
- **Comprehensive Admin Panel**
  - Dashboard with key metrics (users, events, bookings, revenue)
  - User management (view, edit roles, delete)
  - Event management (view all events, delete)
  - Booking management (view details, cancel bookings)
  - Payment management (track all transactions)
  - Category statistics (event distribution)

- **Access Control**
  - Role-based permissions (Admin/User)
  - Secure API endpoints with authorization
  - Protected admin routes

---

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT (JSON Web Tokens)
- **Payment**: Stripe API
- **PDF Generation**: QuestPDF
- **Email**: SMTP (configurable)
- **Mapping**: AutoMapper

### Frontend
- **Core**: Vanilla JavaScript (ES6+)
- **Styling**: Custom CSS with CSS Variables
- **Icons**: Unicode Emojis
- **HTTP**: Fetch API
- **Storage**: LocalStorage for auth tokens

### Architecture Patterns
- **Repository Pattern**: Data access abstraction
- **Unit of Work Pattern**: Transaction management
- **Dependency Injection**: Service registration and lifetime management
- **DTO Pattern**: Data transfer between layers
- **Service Layer Pattern**: Business logic separation
- **Result Pattern**: Consistent error handling

---

## 🏗️ Architecture

Eventify follows a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                     Presentation Layer                   │
│                   (Eventify.APIs)                       │
│  - Controllers                                          │
│  - Middleware                                           │
│  - DTOs (Request/Response)                             │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                     Service Layer                        │
│                  (Eventify.Service)                     │
│  - Business Logic                                       │
│  - DTOs (Service Layer)                                │
│  - AutoMapper Profiles                                 │
│  - Interfaces (IService)                               │
│  - Validation Logic                                    │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  Repository Layer                        │
│                (Eventify.Repository)                    │
│  - Data Access                                         │
│  - Unit of Work                                        │
│  - Repository Implementations                          │
│  - EF Core DbContext                                   │
│  - Database Migrations                                 │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                     Domain Layer                         │
│                   (Eventify.Core)                       │
│  - Entities (Models)                                   │
│  - Enums                                               │
│  - Interfaces (IRepository)                            │
└─────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

#### 1. **Eventify.Core** (Domain Layer)
- Defines core entities and business objects
- Contains enums for event categories, booking status, payment status, etc.
- Repository interfaces
- No dependencies on other layers

#### 2. **Eventify.Repository** (Data Access Layer)
- Implements repository interfaces
- Entity Framework Core DbContext
- Database configurations and migrations
- Unit of Work pattern implementation
- Data access logic

#### 3. **Eventify.Service** (Business Logic Layer)
- Business logic and validation
- Service interfaces and implementations
- DTOs for data transfer
- AutoMapper configuration
- Authentication and authorization logic
- Payment processing logic

#### 4. **Eventify.APIs** (Presentation Layer)
- RESTful API controllers
- Request/Response handling
- JWT authentication middleware
- Dependency injection configuration
- API versioning and documentation

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express, Developer, or Full)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Node.js](https://nodejs.org/) (optional, for frontend tooling)
- [Stripe Account](https://stripe.com/) for payment integration

### Installation Steps

#### 1. Clone the Repository
```bash
git clone https://github.com/nassar350/Depi-Graduation-Project-Eventify.git
cd Eventify.APIs
```

#### 2. Database Setup

**Option A: Using SQL Server Management Studio (SSMS)**
1. Open SSMS and connect to your SQL Server instance
2. Create a new database named `EventifyDB`

**Option B: Using Command Line**
```bash
sqlcmd -S localhost -Q "CREATE DATABASE EventifyDB"
```

#### 3. Configure Connection String

Update `appsettings.json` in `Eventify.APIs`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EventifyDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For SQL Server with username/password:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EventifyDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

#### 4. Configure Application Settings

Update `appsettings.json` with your settings:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyMinimum32CharactersLong",
    "Issuer": "Eventify",
    "Audience": "EventifyUsers",
    "DurationInMinutes": 1440
  },
  "Stripe": {
    "SecretKey": "sk_test_your_stripe_secret_key",
    "PublishableKey": "pk_test_your_stripe_publishable_key",
    "WebhookSecret": "whsec_your_webhook_secret"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password",
    "SenderName": "Eventify"
  }
}
```

#### 5. Apply Database Migrations

```bash
cd Eventify.APIs
dotnet ef database update --project ../Eventify.Repository
```

If you don't have EF Core tools installed:
```bash
dotnet tool install --global dotnet-ef
```

#### 6. Build and Run Backend

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run the API
dotnet run --project Eventify.APIs
```

The API will be available at:
- `https://localhost:7001` (HTTPS)
- `http://localhost:5000` (HTTP)

#### 7. Setup Frontend

1. Navigate to the frontend folder:
```bash
cd EventifyFrontEnd
```

2. Update API base URL in `js/admin.js` and configure other JS files:
```javascript
const API_BASE_URL = 'https://localhost:7001/api';
```

3. Serve the frontend using any static file server:

**Option A: Using VS Code Live Server Extension**
- Install "Live Server" extension
- Right-click `index.html` → "Open with Live Server"

**Option B: Using Python**
```bash
python -m http.server 8000
```

**Option C: Using Node.js http-server**
```bash
npx http-server -p 8000
```

4. Access the application at `http://localhost:8000`

---

## 📁 Project Structure

```
Eventify.APIs/
├── Eventify.APIs/              # API Layer (Presentation)
│   ├── Controllers/            # API Controllers
│   │   ├── AuthController.cs
│   │   ├── EventsController.cs
│   │   ├── BookingController.cs
│   │   ├── PaymentController.cs
│   │   ├── CheckOutController.cs
│   │   ├── TicketsController.cs
│   │   ├── UserController.cs
│   │   ├── AdminController.cs
│   │   └── StripeWebhookController.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Program.cs              # App configuration & DI
│
├── Eventify.Service/           # Business Logic Layer
│   ├── Services/               # Service Implementations
│   │   ├── AuthService.cs
│   │   ├── EventService.cs
│   │   ├── BookingService.cs
│   │   ├── PaymentService.cs
│   │   ├── CheckOutService.cs
│   │   ├── TicketService.cs
│   │   ├── UserService.cs
│   │   ├── AdminService.cs
│   │   └── TicketDownloadService.cs
│   ├── Interfaces/             # Service Interfaces
│   ├── DTOs/                   # Data Transfer Objects
│   │   ├── Auth/
│   │   ├── Events/
│   │   ├── Bookings/
│   │   ├── Payments/
│   │   ├── Tickets/
│   │   ├── Users/
│   │   └── Admin/
│   ├── Mappings/               # AutoMapper Profiles
│   ├── Helpers/                # Helper Classes
│   └── Auth/                   # JWT Token Service
│
├── Eventify.Repository/        # Data Access Layer
│   ├── Repositories/           # Repository Implementations
│   │   ├── EventRepository.cs
│   │   ├── BookingRepository.cs
│   │   ├── PaymentRepository.cs
│   │   ├── TicketRepository.cs
│   │   ├── UserRepository.cs
│   │   └── CategoryRepository.cs
│   ├── Interfaces/             # Repository Interfaces
│   │   └── IUnitOfWork.cs
│   ├── Data/
│   │   ├── Contexts/
│   │   │   └── EventifyContext.cs
│   │   ├── Configurations/     # Entity Configurations
│   │   └── Migrations/         # EF Core Migrations
│   └── UnitOfWork.cs
│
├── Eventify.Core/              # Domain Layer
│   ├── Entities/               # Domain Models
│   │   ├── User.cs
│   │   ├── Event.cs
│   │   ├── Category.cs
│   │   ├── Booking.cs
│   │   ├── Ticket.cs
│   │   ├── Payment.cs
│   │   └── UserAttendEvent.cs
│   └── Enums/                  # Enumerations
│       ├── Role.cs
│       ├── EventCategory.cs
│       ├── BookingStatus.cs
│       ├── PaymentStatus.cs
│       └── TicketStatus.cs
│
└── EventifyFrontEnd/           # Frontend (SPA)
    ├── index.html              # Homepage
    ├── explore.html            # Event Browsing
    ├── event.html              # Event Details
    ├── book.html               # Booking Page
    ├── login.html              # Authentication
    ├── register.html           # User Registration
    ├── dashboard.html          # User Dashboard
    ├── admin-panel.html        # Admin Panel
    ├── booking-details.html    # Booking Details
    ├── payment-success.html    # Payment Confirmation
    ├── create-event.html       # Event Creation
    ├── about.html              # About Page
    ├── js/                     # JavaScript Modules
    │   ├── app.js              # Core App Logic
    │   ├── home.js             # Homepage Logic
    │   ├── explore.js          # Event Browsing
    │   ├── event.js            # Event Details
    │   ├── admin.js            # Admin Panel
    │   └── events-data.js      # Mock Data
    ├── css/                    # Stylesheets
    │   └── styles.css          # Main Styles
    └── assets/                 # Images & Resources
```

---

## 🗄️ Database Schema

### Core Entities

#### **User**
```csharp
- Id (int, PK)
- Name (string, 100)
- Email (string, 255, Unique)
- PhoneNumber (string, 20)
- PasswordHash (string, 500)
- Role (enum: Admin = 0, User = 1)
- CreatedAt (DateTime)
- UpdatedAt (DateTime?)
- Events (ICollection<Event>) // Organized events
- Bookings (ICollection<Booking>) // User bookings
- EventsAttended (ICollection<UserAttendEvent>)
```

#### **Event**
```csharp
- Id (int, PK)
- Name (string, 200)
- Description (string, 2000)
- Address (string, 300)
- StartDate (DateTime)
- EndDate (DateTime)
- EventCategory (enum: Music, Sports, Arts, etc.)
- OrganizerID (int, FK → User)
- PhotoUrl (string, nullable)
- IsOnline (bool)
- ZoomJoinUrl (string, nullable)
- ZoomPassword (string, nullable)
- ZoomMeetingId (string, nullable)
- Organizer (User)
- Categories (ICollection<Category>) // Ticket categories
- Tickets (ICollection<Ticket>)
- EventsAttendedByUsers (ICollection<UserAttendEvent>)
```

#### **Category** (Ticket Categories)
```csharp
- Id (int, PK)
- Title (string, 100) // e.g., "VIP", "Regular"
- TicketPrice (decimal)
- Seats (int) // Total capacity
- Booked (int) // Tickets sold
- EventId (int, FK → Event)
- Event (Event)
- Tickets (ICollection<Ticket>)
```

#### **Booking**
```csharp
- Id (int, PK)
- UserId (int, FK → User)
- EventId (int, FK → Event)
- TicketsNum (int)
- Status (enum: Booked = 0, Pending = 1, Cancelled = 2)
- CreatedAt (DateTime)
- UpdatedAt (DateTime?)
- User (User)
- Event (Event)
- Payment (Payment)
- Tickets (ICollection<Ticket>)
```

#### **Payment**
```csharp
- BookingId (int, PK, FK → Booking)
- TotalPrice (decimal)
- PaymentMethod (string, 50)
- Status (enum: Paid, Pending, Cancelled, Rejected, Refunded)
- DateTime (DateTime)
- StripePaymentIntentId (string, nullable)
- Booking (Booking)
```

#### **Ticket**
```csharp
- Id (int, PK)
- CategoryId (int, FK → Category)
- BookingId (int?, FK → Booking, nullable)
- Status (enum: Available, Booked, Used, Cancelled)
- Price (decimal)
- QRCode (string, nullable) // QR code for verification
- Category (Category)
- Booking (Booking, nullable)
```

#### **UserAttendEvent** (Many-to-Many)
```csharp
- UserId (int, PK, FK → User)
- EventId (int, PK, FK → Event)
- AttendedAt (DateTime)
- User (User)
- Event (Event)
```

### Relationships
- **User → Events**: One-to-Many (Organizer)
- **User → Bookings**: One-to-Many
- **Event → Categories**: One-to-Many
- **Event → Tickets**: One-to-Many
- **Booking → Payment**: One-to-One
- **Booking → Tickets**: One-to-Many
- **Category → Tickets**: One-to-Many
- **User ↔ Event**: Many-to-Many (UserAttendEvent)

---

## 🔌 API Documentation

### Base URL
```
https://eventify.runasp.net/api
```

### Authentication Endpoints

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "Password123!",
  "phoneNumber": "01234567890"
}

Response: 200 OK
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "user": {
      "id": 1,
      "name": "John Doe",
      "email": "john@example.com",
      "role": 1
    }
  }
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "Password123!"
}

Response: 200 OK
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "role": 1
  }
}
```

### Event Endpoints

#### Get All Events
```http
GET /api/events
Authorization: Not required

Response: 200 OK
{
  "success": true,
  "message": "Events retrieved successfully",
  "data": [
    {
      "id": 1,
      "name": "Tech Conference 2024",
      "description": "...",
      "address": "Cairo, Egypt",
      "startDate": "2024-12-20T09:00:00Z",
      "endDate": "2024-12-20T18:00:00Z",
      "eventCategory": "Technology",
      "organizerID": 1,
      "organizerName": "John Doe",
      "photoUrl": "https://...",
      "availableTickets": 150,
      "bookedTickets": 50,
      "revenue": 5000.00,
      "isUpcoming": true,
      "status": "Upcoming"
    }
  ]
}
```

#### Get Event by ID
```http
GET /api/events/{id}
Authorization: Not required

Response: 200 OK
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Tech Conference 2024",
    "eventCategory": "Technology",
    "categories": [
      {
        "id": 1,
        "title": "VIP",
        "ticketPrice": 150.00,
        "seats": 50,
        "booked": 20
      },
      {
        "id": 2,
        "title": "Regular",
        "ticketPrice": 50.00,
        "seats": 200,
        "booked": 80
      }
    ],
    "attendees": [...],
    "tickets": [...]
  }
}
```

#### Create Event
```http
POST /api/events
Authorization: Bearer {token}
Content-Type: multipart/form-data

FormData:
- Name: Tech Conference 2024
- Description: A gathering of tech enthusiasts
- Address: Cairo, Egypt
- StartDate: 2024-12-20T09:00:00Z
- EndDate: 2024-12-20T18:00:00Z
- EventCategory: 5 (Technology)
- Photo: (file upload)
- CategoryIds: [1, 2]

Response: 201 Created
{
  "success": true,
  "message": "Event created successfully",
  "data": { ... }
}
```

#### Get Upcoming Events
```http
GET /api/events/upcoming?take=10
Authorization: Not required

Response: 200 OK
{
  "success": true,
  "data": [ ... ]
}
```

### Booking Endpoints

#### Get User Bookings
```http
GET /api/booking
Authorization: Bearer {token}

Response: 200 OK
{
  "success": true,
  "data": [
    {
      "id": 1,
      "userId": 1,
      "eventId": 1,
      "ticketsNum": 2,
      "status": "Booked",
      "createdAt": "2024-12-01T10:00:00Z"
    }
  ]
}
```

#### Get Booking by ID
```http
GET /api/booking/{id}
Authorization: Bearer {token}

Response: 200 OK
{
  "success": true,
  "data": { ... }
}
```

#### Cancel Booking
```http
DELETE /api/booking/{id}
Authorization: Bearer {token}

Response: 200 OK
{
  "success": true,
  "message": "Booking cancelled successfully"
}
```

### Payment & Checkout Endpoints

#### Create Checkout Session
```http
POST /api/checkout
Authorization: Bearer {token}
Content-Type: application/json

{
  "eventId": 1,
  "userId": 1,
  "tickets": [
    {
      "categoryId": 1,
      "quantity": 2
    }
  ]
}

Response: 200 OK
{
  "success": true,
  "data": {
    "sessionId": "cs_test_...",
    "url": "https://checkout.stripe.com/..."
  }
}
```

#### Stripe Webhook
```http
POST /api/stripe/webhook
Stripe-Signature: {signature}
Content-Type: application/json

(Stripe sends payment events)
```

#### Get Payment by Booking ID
```http
GET /api/payment/{bookingId}
Authorization: Bearer {token}

Response: 200 OK
{
  "success": true,
  "data": {
    "bookingId": 1,
    "totalPrice": 300.00,
    "paymentMethod": "Credit Card",
    "status": "Paid",
    "dateTime": "2024-12-01T10:05:00Z"
  }
}
```

### Ticket Endpoints

#### Get Available Tickets
```http
GET /api/tickets/available?eventId={id}&categoryName={name}
Authorization: Not required

Response: 200 OK
{
  "success": true,
  "data": {
    "availableTickets": 45
  }
}
```

#### Download Tickets PDF
```http
GET /api/checkout/tickets/{bookingId}/pdf
Authorization: Bearer {token}

Response: 200 OK (PDF file)
Content-Type: application/pdf
```

#### Verify Ticket
```http
POST /api/tickets/verify
Authorization: Bearer {token}
Content-Type: application/json

{
  "qrCode": "TICKET-123-ABC"
}

Response: 200 OK
{
  "success": true,
  "data": {
    "isValid": true,
    "ticketId": 123,
    "eventName": "Tech Conference 2024",
    "userName": "John Doe"
  }
}
```

### Admin Endpoints
All admin endpoints require `Authorization: Bearer {token}` with Admin role.

#### Get Statistics
```http
GET /api/admin/statistics
Authorization: Bearer {admin_token}

Response: 200 OK
{
  "success": true,
  "data": {
    "totalUsers": 150,
    "totalEvents": 45,
    "totalBookings": 320,
    "totalRevenue": 45000.00,
    "pendingPayments": 12,
    "activeEvents": 25,
    "totalCategories": 11
  }
}
```

#### Get All Users
```http
GET /api/admin/users?searchTerm={query}
Authorization: Bearer {admin_token}

Response: 200 OK
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "John Doe",
      "email": "john@example.com",
      "phoneNumber": "01234567890",
      "role": 1,
      "eventsCreated": 3,
      "bookingsCount": 5
    }
  ]
}
```

#### Update User
```http
PUT /api/admin/users/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "John Doe Updated",
  "email": "john@example.com",
  "phoneNumber": "01234567890",
  "role": 0
}

Response: 200 OK
{
  "success": true,
  "message": "User updated successfully"
}
```

#### Delete User
```http
DELETE /api/admin/users/{id}
Authorization: Bearer {admin_token}

Response: 200 OK
{
  "success": true,
  "message": "User deleted successfully"
}
```

#### Get All Events (Admin)
```http
GET /api/admin/events?searchTerm={query}
Authorization: Bearer {admin_token}

Response: 200 OK
{
  "success": true,
  "data": [...]
}
```

#### Delete Event (Admin)
```http
DELETE /api/admin/events/{id}
Authorization: Bearer {admin_token}

Response: 200 OK
```

#### Get All Bookings (Admin)
```http
GET /api/admin/bookings?searchTerm={query}
Authorization: Bearer {admin_token}

Response: 200 OK
```

#### Cancel Booking (Admin)
```http
PUT /api/admin/bookings/{id}/cancel
Authorization: Bearer {admin_token}

Response: 200 OK
```

#### Get All Payments (Admin)
```http
GET /api/admin/payments?searchTerm={query}
Authorization: Bearer {admin_token}

Response: 200 OK
```

#### Get Event Categories (Admin)
```http
GET /api/admin/eventcategories
Authorization: Bearer {admin_token}

Response: 200 OK
{
  "success": true,
  "data": [
    {
      "value": 1,
      "name": "Music",
      "eventCount": 15
    },
    {
      "value": 2,
      "name": "Sports",
      "eventCount": 8
    }
    // ... 9 more categories
  ]
}
```

---

## 🔐 Authentication & Authorization

### JWT Implementation

Eventify uses **JWT (JSON Web Tokens)** for secure authentication.

#### Token Structure
```json
{
  "sub": "1", // User ID
  "email": "john@example.com",
  "name": "John Doe",
  "role": "User", // or "Admin"
  "jti": "unique-jwt-id",
  "exp": 1703001600, // Expiration timestamp
  "iss": "Eventify",
  "aud": "EventifyUsers"
}
```

#### Token Configuration
- **Algorithm**: HMAC-SHA256
- **Expiration**: 24 hours (configurable)
- **Storage**: LocalStorage (frontend)
- **Header**: `Authorization: Bearer {token}`

### Role-Based Access Control

#### Roles
1. **User** (Role = 1)
   - Create and manage own events
   - Book tickets for events
   - View own bookings and tickets
   - Update own profile

2. **Admin** (Role = 0)
   - All User permissions
   - Access admin panel
   - Manage all users
   - Manage all events
   - View all bookings and payments
   - Access statistics and analytics

#### Protected Endpoints

**User Authentication Required** (`[Authorize]`):
- `/api/booking/*`
- `/api/payment/*`
- `/api/checkout`
- `/api/user/profile`

**Admin Role Required** (`[Authorize(Roles = "Admin")]`):
- `/api/admin/*`

**Event Organizer** (Owner verification in code):
- `/api/events/{id}` (PUT/DELETE - only event owner)

### Frontend Authentication Flow

```javascript
// 1. Login
const response = await fetch(`${API_BASE_URL}/auth/login`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, password })
});
const result = await response.json();

// 2. Store token
localStorage.setItem('eventify_token', result.token);
localStorage.setItem('eventifyUser', JSON.stringify(result.user));

// 3. Include token in requests
const response = await fetch(`${API_BASE_URL}/booking`, {
  headers: {
    'Authorization': `Bearer ${localStorage.getItem('eventify_token')}`
  }
});

// 4. Handle 401/403 errors
if (response.status === 401) {
  // Redirect to login
  window.location.href = 'login.html';
} else if (response.status === 403) {
  // Show access denied
  alert('Access denied');
}
```

---

## 💳 Payment Integration

### Stripe Setup

#### 1. Get Stripe API Keys
1. Create account at [stripe.com](https://stripe.com)
2. Navigate to Developers → API Keys
3. Copy **Publishable Key** and **Secret Key**

#### 2. Configure Webhook
1. Go to Developers → Webhooks
2. Add endpoint: `https://your-domain.com/api/stripe/webhook`
3. Select events: `payment_intent.succeeded`, `payment_intent.payment_failed`
4. Copy **Webhook Secret**

#### 3. Update Configuration
```json
{
  "Stripe": {
    "SecretKey": "sk_test_your_secret_key",
    "PublishableKey": "pk_test_your_publishable_key",
    "WebhookSecret": "whsec_your_webhook_secret"
  }
}
```

### Payment Flow

```
┌─────────┐      ┌─────────┐      ┌────────┐      ┌────────┐
│  User   │      │ Frontend│      │  API   │      │ Stripe │
└────┬────┘      └────┬────┘      └───┬────┘      └───┬────┘
     │ Select Tickets │               │               │
     │───────────────>│               │               │
     │                │               │               │
     │   Click Checkout               │               │
     │───────────────>│               │               │
     │                │ POST /checkout│               │
     │                │──────────────>│               │
     │                │               │ Create Session│
     │                │               │──────────────>│
     │                │               │<──────────────│
     │                │<──────────────│ Session URL   │
     │  Redirect to Stripe            │               │
     │<───────────────│               │               │
     │                │               │               │
     │  Enter Payment Details         │               │
     │───────────────────────────────>│               │
     │                │               │               │
     │                │               │  Webhook Event│
     │                │               │<──────────────│
     │                │               │ Update Payment│
     │                │               │               │
     │  Redirect Success              │               │
     │<───────────────────────────────│               │
     │                │               │               │
     │  View Tickets  │               │               │
     │───────────────>│ GET /tickets  │               │
     │                │──────────────>│               │
     │<───────────────│<──────────────│               │
     │   PDF Tickets  │               │               │
```

### Payment Status Tracking

```csharp
public enum PaymentStatus
{
    Paid = 0,       // Payment successful
    Pending = 1,    // Awaiting payment
    Cancelled = 2,  // User cancelled
    Rejected = 3,   // Payment failed
    Refunded = 4    // Refund processed
}
```

---

## 📊 Admin Panel

### Dashboard Features

#### Statistics Overview
- **Total Users**: Count of registered users
- **Total Events**: All events in system
- **Total Bookings**: Completed bookings
- **Platform Revenue**: Sum of all paid payments
- **Pending Payments**: Payments awaiting confirmation
- **Active Events**: Upcoming events
- **Total Categories**: EventCategory enum count (11)

#### User Management
- View all users with search
- Edit user details and roles
- Delete users
- Track user activity (events created, bookings made)

#### Event Management
- View all events with search
- See event details (bookings, revenue, capacity)
- Delete events
- Monitor event status

#### Booking Management
- View all bookings across system
- Search by user, event, or email
- View booking details
- Cancel bookings
- Track booking status

#### Payment Management
- View all payment transactions
- Search payments
- Track payment status
- View payment methods
- Monitor revenue

#### Category Statistics
- View EventCategory distribution
- See event count per category
- Read-only (enum-based categories)

### Access Control
- Requires Admin role (Role = 0)
- 403 Forbidden for non-admin users
- 401 Unauthorized without valid token
- Client-side role verification
- Server-side authorization checks

---

## 🌐 Frontend Pages

### Public Pages

#### Homepage (`index.html`)
- Hero section with search
- Featured upcoming events
- Categories showcase
- How it works section

#### Explore (`explore.html`)
- Event grid with filtering
- Search by name/location
- Filter by category
- Sort by date/price/popularity
- Pagination

#### Event Details (`event.html`)
- Event information and photos
- Organizer details
- Ticket categories with pricing
- Real-time availability
- Location and date/time
- Book now button

#### About (`about.html`)
- Platform information
- Mission and values
- Contact details

### Authentication Pages

#### Login (`login.html`)
- Email/password login
- Remember me option
- Redirect to intended page
- Link to registration

#### Register (`register.html`)
- User registration form
- Name, email, phone, password
- Validation and error handling
- Auto-login after registration

### Protected Pages

#### User Dashboard (`dashboard.html`)
- Overview of user bookings
- Upcoming and past events
- Booking status tracking
- Quick access to tickets
- Profile management

#### Create Event (`create-event.html`)
- Event creation form
- Photo upload
- Category selection
- Ticket category configuration
- Zoom integration (optional)

#### Booking Details (`booking-details.html`)
- Detailed booking information
- Event details
- Ticket information
- Payment status
- Download tickets
- Cancel booking option

#### Payment Success (`payment-success.html`)
- Confirmation message
- Booking summary
- Download tickets
- Return to dashboard

### Admin Pages

#### Admin Panel (`admin-panel.html`)
- Dashboard with statistics
- User management tab
- Event management tab
- Booking management tab
- Payment management tab
- Category statistics tab
- Search and filter capabilities
- CRUD operations (where applicable)

---

## ⚙️ Configuration

### Environment Variables

Create `appsettings.Development.json` for local development:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EventifyDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YourDevelopmentSecretKeyMinimum32Characters",
    "Issuer": "Eventify",
    "Audience": "EventifyUsers",
    "DurationInMinutes": 1440
  },
  "Stripe": {
    "SecretKey": "sk_test_your_test_key",
    "PublishableKey": "pk_test_your_test_key",
    "WebhookSecret": "whsec_your_test_webhook_secret"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-test-email@gmail.com",
    "SenderPassword": "your-app-password",
    "SenderName": "Eventify Dev"
  },
  "AllowedHosts": "*",
  "Urls": "https://localhost:7001;http://localhost:5000"
}
```

### CORS Configuration

Already configured in `Program.cs` to allow frontend:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

For production, restrict to specific origins:

```csharp
policy.WithOrigins("https://yourdomain.com")
      .AllowAnyMethod()
      .AllowAnyHeader();
```

---

## 🚢 Deployment

### Prerequisites
- Azure account (or any hosting provider)
- SQL Server database (Azure SQL or on-premises)
- Domain name (optional)

### Backend Deployment (Azure App Service)

#### 1. Publish Application
```bash
dotnet publish -c Release -o ./publish
```

#### 2. Create Azure Resources
```bash
# Create resource group
az group create --name EventifyRG --location eastus

# Create App Service plan
az appservice plan create --name EventifyPlan --resource-group EventifyRG --sku B1

# Create web app
az webapp create --name eventify-api --resource-group EventifyRG --plan EventifyPlan

# Create Azure SQL Database
az sql server create --name eventify-sql --resource-group EventifyRG --admin-user sqladmin --admin-password YourPassword123!
az sql db create --server eventify-sql --resource-group EventifyRG --name EventifyDB --service-objective S0
```

#### 3. Configure App Settings
```bash
az webapp config appsettings set --name eventify-api --resource-group EventifyRG --settings \
  ConnectionStrings__DefaultConnection="Server=tcp:eventify-sql.database.windows.net,1433;Database=EventifyDB;User ID=sqladmin;Password=YourPassword123!;" \
  Jwt__Key="YourProductionSecretKeyMinimum32Characters" \
  Stripe__SecretKey="sk_live_your_live_key" \
  Stripe__WebhookSecret="whsec_your_live_webhook_secret"
```

#### 4. Deploy
```bash
az webapp deployment source config-zip --name eventify-api --resource-group EventifyRG --src ./publish.zip
```

### Frontend Deployment

#### Option 1: Azure Static Web Apps
```bash
# Deploy frontend
az staticwebapp create --name eventify-frontend --resource-group EventifyRG --location eastus --source EventifyFrontEnd
```

#### Option 2: GitHub Pages
1. Push frontend to GitHub repository
2. Enable GitHub Pages in repository settings
3. Select branch and folder
4. Access at `https://username.github.io/repository`

#### Option 3: Netlify/Vercel
1. Connect Git repository
2. Configure build settings (none needed for static site)
3. Deploy automatically on push

### Database Migration

Run migrations on production database:

```bash
dotnet ef database update --connection "your-production-connection-string"
```

### Post-Deployment

1. Update frontend API URLs to production:
```javascript
const API_BASE_URL = 'https://eventify-api.azurewebsites.net/api';
```

2. Configure Stripe webhook URL to production endpoint

3. Test all functionality:
   - Registration and login
   - Event creation
   - Booking flow
   - Payment processing
   - Admin panel access

---

## 🤝 Contributing

We welcome contributions to Eventify! Here's how you can help:

### Development Setup

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/AmazingFeature`
3. Make your changes
4. Run tests (if available)
5. Commit: `git commit -m 'Add AmazingFeature'`
6. Push: `git push origin feature/AmazingFeature`
7. Open a Pull Request

### Coding Standards

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML comments for public APIs
- Write clean, readable code
- Add unit tests for new features

### Pull Request Process

1. Update README.md with details of changes if needed
2. Update API documentation if endpoints change
3. Ensure all tests pass
4. Request review from maintainers
5. Address feedback and update PR

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👥 Team & Acknowledgments

### DEPI Graduation Project Team
- **Developer**: Nassar350 and team members
- **Project Type**: Full Stack .NET Graduation Project
- **Institution**: DEPI (Digital Egypt Pioneers Initiative)
- **Year**: 2024

### Technologies Used
- ASP.NET Core Team
- Entity Framework Core Team
- Stripe for payment processing
- Microsoft SQL Server
- AutoMapper
- QuestPDF

---

## 📞 Support & Contact

### Issues
Report bugs or request features: [GitHub Issues](https://github.com/nassar350/Depi-Graduation-Project-Eventify/issues)

### Documentation
- [API Integration Guide](API_INTEGRATION_GUIDE.md)
- [Authentication Architecture](AUTH_LAYERED_ARCHITECTURE_SUMMARY.md)
- [Booking & Payment Schemas](BOOKING_PAYMENT_API_SCHEMAS.md)
- [Stripe Integration Guide](STRIPE_INTEGRATION_GUIDE.md)

### Connect
- GitHub: [@nassar350](https://github.com/nassar350)
- Project Repository: [Eventify](https://github.com/nassar350/Depi-Graduation-Project-Eventify)

---

## 🎯 Future Enhancements

### Planned Features
- [ ] Email notification system for bookings
- [ ] SMS notifications for event reminders
- [ ] Social media integration for event sharing
- [ ] Advanced analytics dashboard
- [ ] Mobile app (iOS/Android)
- [ ] Event recommendations based on user preferences
- [ ] Multi-language support
- [ ] Dark mode theme
- [ ] Event reviews and ratings
- [ ] Waiting list for sold-out events
- [ ] Recurring events support
- [ ] Calendar integration (Google Calendar, iCal)

### Technical Improvements
- [ ] Unit and integration tests
- [ ] Performance optimization
- [ ] Caching layer (Redis)
- [ ] API rate limiting
- [ ] GraphQL support
- [ ] WebSocket for real-time updates
- [ ] Dockerization
- [ ] CI/CD pipeline
- [ ] Monitoring and logging (Application Insights)

---

**Made with ❤️ by DEPI Graduation Project Team**

*Eventify - Bringing people together through events*
