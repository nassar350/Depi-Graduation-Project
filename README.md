🎉 Eventify – Event Management System

Eventify is a complete event management platform that allows users to browse events, book tickets, and complete secure online payments.
Administrators can manage all system data, including events, categories, bookings, payments, and tickets, through a dedicated dashboard.

📚 Table of Contents

About the Project

Features

System Architecture

Tech Stack

API Modules

User Flow

Admin Flow

Database Schema

Installation & Setup

Environment Variables

Contributing

License

📘 About the Project

Eventify is designed to simplify event management and ticket booking.
It supports both regular users and administrators with a modern, scalable architecture using ASP.NET Core, Entity Framework Core, and SQL Server.

Users can explore events, book tickets, pay via Stripe, and view their booking details.
Admins can manage all content including events, tickets, bookings, payments, and categories.

✨ Features
👤 User Features

Browse all events with filtering and search

View event details (description, date, time, location, price)

Book tickets

Stripe Payment Integration

View “My Bookings”

View booking details (with QR code)

🛠️ Admin Features

Category Management (CRUD)

Event Management (CRUD + upload image)

Ticket Management per event

Booking Management

Payment Management

Dashboard with counts and overview

🏗️ System Architecture
Eventify
│
├── Eventify.APIs (Controllers)
│
├── Eventify.Core (Entities + Enums)
│
├── Eventify.Repository (Repository Layer + EF Context)
│
└── Eventify.Service (DTOs + Services + Mapping Profiles)

Layers

API Layer: Handles HTTP requests

Service Layer: Business logic

Repository Layer: Database operations using EF Core

Core Layer: Entities and enums

🛠️ Tech Stack
Layer	Technology
Backend	ASP.NET Core 8 Web API
ORM	Entity Framework Core
Database	SQL Server
Authentication	JWT
Payments	Stripe
Mapping	AutoMapper
Architecture	Clean Architecture (API → Service → Repo → Core)
🔌 API Modules
1️⃣ Categories

Create Category

Update Category

Delete Category

Get All Categories

Get Category by ID

2️⃣ Events

Create event with image

Assign category

Update event

Delete event

Get all events

Get event details

3️⃣ Tickets

Add ticket types per event

Update ticket

Delete ticket

4️⃣ Bookings

Create booking

Confirm after payment

Get bookings by user

Get booking details

5️⃣ Payments

Stripe checkout

Record payment data

View payments list

6️⃣ Auth

Register

Login

JWT Token Generation

🔵 User Flow
Homepage
  ↓
Event Details
  ↓
Book Ticket
  ↓
Stripe Payment
  ↓
Payment Success → Save Booking & Payment
  ↓
My Bookings
  ↓
Booking Details (QR)

🟣 Admin Flow
Login
  ↓
Dashboard
  ↓
Manage Categories
  ↓
Manage Events
  ↓
Manage Tickets
  ↓
Manage Bookings
  ↓
Manage Payments
  ↓
Settings

🗄️ Database Schema (Simplified)
Users
Categories
Events
Tickets
Bookings
Payments


Relationships:

Category → Event (1:M)

Event → Ticket (1:M)

Event → Booking (1:M)

Booking → Payment (1:1)

User → Booking (1:M)

⚙️ Installation & Setup
1. Clone the repository
git clone https://github.com/your-repo/Eventify.git

2. Set up database connection

In appsettings.json:

"ConnectionStrings": {
  "OnlineDbConnectionString": ""
}

3. Run Entity Framework migrations
dotnet ef database update

4. Run the project
dotnet run

🔒 Environment Variables

Add the following:

JWT__Key=
JWT__Issuer=
JWT__Audience=

STRIPE__SecretKey=
STRIPE__PublishableKey=
STRIPE__WebhookSecret=

🤝 Contributing

Contributions are welcome!
Follow standard Git flow:

Create a feature branch

Commit changes

Open a pull request

📄 License

This project is licensed under the MIT License.
