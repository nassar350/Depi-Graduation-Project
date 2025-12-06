# UI/UX Design Documentation

## Eventify — Event Booking & Management Platform

**Last Updated:** November 29, 2025  
**Design Version:** 2.0  
**Status:** ✅ Implemented

---

## Table of Contents

1. [Design System](#design-system)
2. [Page Layouts](#page-layouts)
3. [User Flows](#user-flows)
4. [Responsive Design](#responsive-design)
5. [Accessibility Features](#accessibility-features)
6. [Animation & Transitions](#animation--transitions)
7. [Recent Updates](#recent-updates)

---

## Design System

### Color Palette

#### Dark Theme (Primary)

```css
--background: #0b0f12       /* Main background */
--card: #0f1720             /* Card/container background */
--border: #1e2329           /* Borders and dividers */
--text: #ffffff             /* Primary text */
--muted: #8b92a0            /* Secondary text */
--accent: #7C5CFF           /* Primary accent (purple) */
--accent-hover: #5B3FE5     /* Accent hover state */
--success: #00d97a          /* Success states */
--error: #ff4444            /* Error states */
--warning: #ffd700          /* Warning states */
```

#### Color Usage Guidelines

- **Background (#0b0f12)**: Main page background, creates depth
- **Card (#0f1720)**: Component backgrounds, slightly elevated from main background
- **Border (#1e2329)**: Subtle dividers, outlines, and separators
- **Text (#ffffff)**: Primary readable text, headings
- **Muted (#8b92a0)**: Secondary text, labels, placeholders
- **Accent (#7C5CFF)**: CTAs, links, brand elements, focus states
- **Success (#00d97a)**: Confirmation messages, available status, positive actions
- **Error (#ff4444)**: Error messages, warnings, delete actions
- **Warning (#ffd700)**: Alerts, pending status, important notices

### Typography

#### Font Families
- **Headings**: 'Poppins', sans-serif (600 weight)
- **Body**: 'Inter', sans-serif (400 weight)
- **Code**: 'Fira Code', monospace

#### Font Sizes
- **H1**: 36px (2.25rem) - Hero headings
- **H2**: 28px (1.75rem) - Section headings
- **H3**: 24px (1.5rem) - Subsection headings
- **H4**: 20px (1.25rem) - Card titles
- **H5**: 18px (1.125rem) - Component headings
- **Body**: 16px (1rem) - Default text
- **Small**: 14px (0.875rem) - Labels, metadata
- **Tiny**: 12px (0.75rem) - Captions, footnotes

#### Line Heights
- Headings: 1.2
- Body: 1.6
- Small: 1.4

### Component Library

#### Buttons

**Primary Button**
```css
background: linear-gradient(135deg, #7C5CFF 0%, #5B3FE5 100%);
border-radius: 8px;
padding: 12px 24px;
color: #ffffff;
font-weight: 600;
transition: transform 0.2s, box-shadow 0.2s;
```
- **Hover**: `transform: scale(1.02); box-shadow: 0 8px 16px rgba(124, 92, 255, 0.3);`
- **Active**: `transform: scale(0.98);`

**Secondary Button (Outline)**
```css
background: transparent;
border: 2px solid #7C5CFF;
border-radius: 8px;
padding: 10px 22px;
color: #7C5CFF;
```

**Disabled Button**
```css
background: #1e2329;
color: #8b92a0;
cursor: not-allowed;
opacity: 0.6;
```

#### Cards

```css
background: #0f1720;
border: 1px solid #1e2329;
border-radius: 12px;
padding: 20px;
box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
transition: transform 0.2s, box-shadow 0.2s;
```
- **Hover**: `transform: translateY(-4px); box-shadow: 0 8px 12px rgba(0, 0, 0, 0.2);`

#### Forms

**Input Fields**
```css
background: #0b0f12;
border: 1px solid #1e2329;
border-radius: 8px;
padding: 12px 16px;
color: #ffffff;
font-size: 16px;
transition: border-color 0.2s;
```
- **Focus**: `border-color: #7C5CFF; outline: none; box-shadow: 0 0 0 3px rgba(124, 92, 255, 0.1);`
- **Error**: `border-color: #ff4444;`
- **Success**: `border-color: #00d97a;`

**Labels**
```css
color: #8b92a0;
font-size: 14px;
font-weight: 500;
margin-bottom: 8px;
```

**Validation Messages**
- Error: Red text (#ff4444), appears below input
- Success: Green text (#00d97a), appears below input

#### Modals

```css
overlay: rgba(0, 0, 0, 0.8);
backdrop-filter: blur(4px);
modal-background: #0f1720;
modal-border: 1px solid #1e2329;
modal-border-radius: 12px;
modal-padding: 32px;
modal-max-width: 600px;
```

**Modal Header**
```css
font-size: 24px;
font-weight: 600;
color: #ffffff;
margin-bottom: 20px;
```

**Close Button**
- Position: Top-right corner
- Style: Transparent with hover effect
- Icon: × (times symbol)

#### Badges

**Status Badges**
```css
padding: 4px 12px;
border-radius: 4px;
font-size: 12px;
font-weight: 600;
text-transform: uppercase;
```

Badge Types:
- **Success**: `background: rgba(0, 217, 122, 0.2); color: #00d97a;`
- **Warning**: `background: rgba(255, 215, 0, 0.2); color: #ffd700;`
- **Error**: `background: rgba(255, 68, 68, 0.2); color: #ff4444;`
- **Info**: `background: rgba(124, 92, 255, 0.2); color: #7C5CFF;`

#### Spinners

**Loading Spinner**
```css
border: 4px solid #1e2329;
border-top-color: #7C5CFF;
border-radius: 50%;
width: 40px;
height: 40px;
animation: spin 1s linear infinite;
```

---

## Page Layouts

### 1. Landing Page (index.html)

**Purpose**: First impression, hero section, featured events

```svg
<svg viewBox="0 0 800 600" xmlns="http://www.w3.org/2000/svg">
  <!-- Header -->
  <rect x="0" y="0" width="800" height="80" fill="#0f1720"/>
  <text x="40" y="50" font-size="24" fill="#7C5CFF" font-weight="bold">Eventify</text>
  <text x="650" y="50" font-size="14" fill="#ffffff">Login / Register</text>
  
  <!-- Hero Section -->
  <rect x="0" y="80" width="800" height="300" fill="#0b0f12"/>
  <text x="400" y="200" font-size="36" fill="#ffffff" text-anchor="middle" font-weight="bold">Discover Amazing Events</text>
  <text x="400" y="240" font-size="18" fill="#8b92a0" text-anchor="middle">Book tickets for concerts, workshops, and more</text>
  <rect x="320" y="260" width="160" height="48" rx="8" fill="#7C5CFF"/>
  <text x="400" y="290" font-size="16" fill="#ffffff" text-anchor="middle">Explore Events</text>
  
  <!-- Featured Events Grid -->
  <rect x="40" y="420" width="220" height="160" rx="12" fill="#0f1720" stroke="#1e2329"/>
  <rect x="290" y="420" width="220" height="160" rx="12" fill="#0f1720" stroke="#1e2329"/>
  <rect x="540" y="420" width="220" height="160" rx="12" fill="#0f1720" stroke="#1e2329"/>
  
  <text x="150" y="500" font-size="16" fill="#ffffff" text-anchor="middle">Music Concert</text>
  <text x="400" y="500" font-size="16" fill="#ffffff" text-anchor="middle">Tech Workshop</text>
  <text x="650" y="500" font-size="16" fill="#ffffff" text-anchor="middle">Art Exhibition</text>
</svg>
```

**Key Elements:**
- Fixed header with logo and auth buttons
- Hero section with main heading and CTA
- Featured events grid (3 columns on desktop)
- Event cards with image, title, date, location

---

### 2. Event Details Page (event.html)

**Purpose**: Display event information and ticket categories

```svg
<svg viewBox="0 0 800 600" xmlns="http://www.w3.org/2000/svg">
  <!-- Header -->
  <rect x="0" y="0" width="800" height="80" fill="#0f1720"/>
  <text x="40" y="50" font-size="24" fill="#7C5CFF" font-weight="bold">Eventify</text>
  
  <!-- Event Banner -->
  <rect x="40" y="100" width="720" height="200" rx="12" fill="#1e2329"/>
  <text x="400" y="210" font-size="28" fill="#ffffff" text-anchor="middle" font-weight="bold">Event Title</text>
  
  <!-- Event Info Card -->
  <rect x="40" y="320" width="450" height="260" rx="12" fill="#0f1720" stroke="#1e2329"/>
  <text x="60" y="350" font-size="18" fill="#ffffff" font-weight="bold">Event Details</text>
  <text x="60" y="385" font-size="14" fill="#8b92a0">📅 Date & Time</text>
  <text x="60" y="415" font-size="14" fill="#8b92a0">📍 Location</text>
  <text x="60" y="445" font-size="14" fill="#8b92a0">👤 Organizer</text>
  <text x="60" y="480" font-size="16" fill="#ffffff">Description</text>
  <text x="60" y="510" font-size="14" fill="#8b92a0">Event description text...</text>
  
  <!-- Ticket Categories -->
  <rect x="510" y="320" width="250" height="260" rx="12" fill="#0f1720" stroke="#1e2329"/>
  <text x="530" y="350" font-size="18" fill="#ffffff" font-weight="bold">Tickets</text>
  
  <rect x="530" y="370" width="210" height="60" rx="8" fill="#0b0f12" stroke="#1e2329"/>
  <text x="545" y="395" font-size="14" fill="#ffffff">VIP - $50</text>
  <text x="545" y="415" font-size="12" fill="#00d97a">20 available</text>
  
  <rect x="530" y="445" width="210" height="60" rx="8" fill="#0b0f12" stroke="#1e2329"/>
  <text x="545" y="470" font-size="14" fill="#ffffff">General - $30</text>
  <text x="545" y="490" font-size="12" fill="#00d97a">50 available</text>
  
  <rect x="530" y="525" width="210" height="40" rx="8" fill="#7C5CFF"/>
  <text x="635" y="550" font-size="14" fill="#ffffff" text-anchor="middle">Book Now</text>
</svg>
```

**Key Elements:**
- Event banner image with title overlay
- Left column: Event details (date, location, organizer, description)
- Right column: Ticket categories with pricing and availability
- "Book Now" CTA button

---

### 3. Booking Flow (book.html)

**Purpose**: Ticket selection and Stripe payment processing

```svg
<svg viewBox="0 0 800 600" xmlns="http://www.w3.org/2000/svg">
  <!-- Header -->
  <rect x="0" y="0" width="800" height="80" fill="#0f1720"/>
  <text x="40" y="50" font-size="24" fill="#7C5CFF" font-weight="bold">Eventify</text>
  
  <!-- Progress Steps -->
  <circle cx="200" cy="130" r="30" fill="#7C5CFF"/>
  <text x="200" y="140" font-size="16" fill="#ffffff" text-anchor="middle">1</text>
  <line x1="230" y1="130" x2="370" y2="130" stroke="#1e2329" stroke-width="2"/>
  
  <circle cx="400" cy="130" r="30" fill="#1e2329"/>
  <text x="400" y="140" font-size="16" fill="#8b92a0" text-anchor="middle">2</text>
  <line x1="430" y1="130" x2="570" y2="130" stroke="#1e2329" stroke-width="2"/>
  
  <circle cx="600" cy="130" r="30" fill="#1e2329"/>
  <text x="600" y="140" font-size="16" fill="#8b92a0" text-anchor="middle">3</text>
  
  <!-- Event Summary -->
  <rect x="40" y="180" width="350" height="380" rx="12" fill="#0f1720" stroke="#1e2329"/>
  <text x="60" y="210" font-size="18" fill="#ffffff" font-weight="bold">Event Summary</text>
  
  <rect x="60" y="230" width="90" height="90" rx="8" fill="#1e2329"/>
  <text x="170" y="250" font-size="16" fill="#ffffff">Event Name</text>
  <text x="170" y="275" font-size="12" fill="#8b92a0">Date & Location</text>
  
  <line x1="60" y1="340" x2="370" y2="340" stroke="#1e2329" stroke-width="1"/>
  
  <text x="60" y="370" font-size="14" fill="#ffffff">Category: VIP</text>
  <text x="60" y="395" font-size="14" fill="#ffffff">Quantity: 2</text>
  <text x="60" y="420" font-size="14" fill="#ffffff">Price: $50 x 2</text>
  
  <line x1="60" y1="440" x2="370" y2="440" stroke="#1e2329" stroke-width="1"/>
  
  <text x="60" y="470" font-size="18" fill="#ffffff" font-weight="bold">Total</text>
  <text x="370" y="470" font-size="18" fill="#7C5CFF" font-weight="bold" text-anchor="end">$100</text>
  
  <rect x="60" y="500" width="310" height="48" rx="8" fill="#7C5CFF"/>
  <text x="215" y="530" font-size="16" fill="#ffffff" text-anchor="middle">Proceed to Payment</text>
  
  <!-- Stripe Payment Form -->
  <rect x="410" y="180" width="350" height="380" rx="12" fill="#0f1720" stroke="#1e2329"/>
  <text x="430" y="210" font-size="18" fill="#ffffff" font-weight="bold">Payment Details</text>
  
  <text x="430" y="250" font-size="12" fill="#8b92a0">Card Number</text>
  <rect x="430" y="260" width="310" height="40" rx="8" fill="#0b0f12" stroke="#1e2329"/>
  
  <text x="430" y="325" font-size="12" fill="#8b92a0">Expiry Date</text>
  <rect x="430" y="335" width="145" height="40" rx="8" fill="#0b0f12" stroke="#1e2329"/>
  
  <text x="595" y="325" font-size="12" fill="#8b92a0">CVC</text>
  <rect x="595" y="335" width="145" height="40" rx="8" fill="#0b0f12" stroke="#1e2329"/>
  
  <rect x="430" y="500" width="310" height="48" rx="8" fill="#00d97a"/>
  <text x="585" y="530" font-size="16" fill="#ffffff" text-anchor="middle">Complete Booking</text>
</svg>
```

**Key Elements:**
- Progress indicator (3 steps)
- Left column: Event summary with image, details, and pricing breakdown
- Right column: Stripe payment form (card number, expiry, CVC)
- Total price display
- "Complete Booking" CTA

---

### 4. Payment Success Page (payment-success.html)

**Purpose**: Confirmation and ticket download

```svg
<svg viewBox="0 0 800 600" xmlns="http://www.w3.org/2000/svg">
  <!-- Background -->
  <rect x="0" y="0" width="800" height="600" fill="#0b0f12"/>
  
  <!-- Success Card -->
  <rect x="200" y="100" width="400" height="400" rx="12" fill="#0f1720" stroke="#1e2329"/>
  
  <!-- Success Icon -->
  <circle cx="400" cy="200" r="50" fill="#00d97a"/>
  <path d="M 370 200 L 390 220 L 430 180" stroke="#ffffff" stroke-width="6" fill="none" stroke-linecap="round" stroke-linejoin="round"/>
  
  <text x="400" y="290" font-size="24" fill="#ffffff" text-anchor="middle" font-weight="bold">Payment Successful!</text>
  <text x="400" y="320" font-size="14" fill="#8b92a0" text-anchor="middle">Your booking has been confirmed</text>
  
  <!-- Booking Details -->
  <rect x="230" y="340" width="340" height="100" rx="8" fill="#0b0f12" stroke="#1e2329"/>
  <text x="250" y="365" font-size="12" fill="#8b92a0">Booking ID:</text>
  <text x="250" y="385" font-size="14" fill="#ffffff">#BK123456</text>
  <text x="250" y="410" font-size="12" fill="#8b92a0">Email sent to:</text>
  <text x="250" y="430" font-size="14" fill="#ffffff">user@example.com</text>
  
  <!-- Download Ticket Button -->
  <rect x="230" y="460" width="340" height="48" rx="8" fill="#7C5CFF"/>
  <text x="400" y="490" font-size="16" fill="#ffffff" text-anchor="middle">Download Ticket PDF</text>
</svg>
```

**Key Elements:**
- Large success icon (green checkmark)
- Success message
- Booking ID display
- Email confirmation notice
- "Download Ticket PDF" button

---

### 5. Ticket Verification Page (verify-ticket.html)

**Purpose**: QR code scan result display

```svg
<svg viewBox="0 0 800 600" xmlns="http://www.w3.org/2000/svg">
  <!-- Background -->
  <rect x="0" y="0" width="800" height="600" fill="#0b0f12"/>
  
  <!-- Verification Card -->
  <rect x="200" y="80" width="400" height="450" rx="12" fill="#0f1720" stroke="#1e2329"/>
  
  <text x="400" y="120" font-size="24" fill="#ffffff" text-anchor="middle" font-weight="bold">Ticket Verification</text>
  
  <!-- Valid Ticket Display -->
  <circle cx="400" cy="200" r="50" fill="#00d97a"/>
  <path d="M 370 200 L 390 220 L 430 180" stroke="#ffffff" stroke-width="6" fill="none" stroke-linecap="round" stroke-linejoin="round"/>
  
  <text x="400" y="280" font-size="20" fill="#00d97a" text-anchor="middle">✓ Valid Ticket</text>
  
  <!-- Ticket Details -->
  <rect x="230" y="300" width="340" height="200" rx="8" fill="#0b0f12" stroke="#1e2329"/>
  
  <text x="250" y="330" font-size="12" fill="#8b92a0">Event Name</text>
  <text x="250" y="350" font-size="14" fill="#ffffff">Tech Conference 2025</text>
  
  <text x="250" y="380" font-size="12" fill="#8b92a0">Category</text>
  <text x="250" y="400" font-size="14" fill="#ffffff">VIP Pass</text>
  
  <text x="250" y="430" font-size="12" fill="#8b92a0">Attendee</text>
  <text x="250" y="450" font-size="14" fill="#ffffff">John Doe</text>
  
  <text x="250" y="480" font-size="12" fill="#8b92a0">Status</text>
  <rect x="250" y="485" width="80" height="24" rx="4" fill="#00d97a" opacity="0.2"/>
  <text x="290" y="502" font-size="12" fill="#00d97a" text-anchor="middle">Upcoming</text>
</svg>
```

**Key Elements:**
- Verification status icon (checkmark for valid, X for invalid)
- Status message
- Ticket details (event, category, attendee)
- Event status badge (upcoming/ongoing/ended)
- Loading state (spinner) before verification completes

---

### 6. User Dashboard (dashboard.html)

**Purpose**: User profile and booking management

```svg
<svg viewBox="0 0 800 600" xmlns="http://www.w3.org/2000/svg">
  <!-- Header -->
  <rect x="0" y="0" width="800" height="80" fill="#0f1720"/>
  <text x="40" y="50" font-size="24" fill="#7C5CFF" font-weight="bold">Eventify</text>
  
  <!-- User Profile Card -->
  <rect x="40" y="100" width="300" height="200" rx="12" fill="#0f1720" stroke="#1e2329"/>
  <circle cx="120" cy="160" r="40" fill="#1e2329"/>
  <text x="120" y="170" font-size="24" fill="#7C5CFF" text-anchor="middle">JD</text>
  
  <text x="180" y="150" font-size="18" fill="#ffffff" font-weight="bold">John Doe</text>
  <text x="180" y="175" font-size="14" fill="#8b92a0">john@example.com</text>
  <rect x="180" y="185" width="50" height="20" rx="4" fill="#7C5CFF" opacity="0.2"/>
  <text x="205" y="199" font-size="12" fill="#7C5CFF" text-anchor="middle">User</text>
  
  <rect x="60" y="230" width="260" height="40" rx="8" fill="#7C5CFF" opacity="0.2" stroke="#7C5CFF"/>
  <text x="190" y="255" font-size="14" fill="#7C5CFF" text-anchor="middle">Edit Profile</text>
  
  <!-- Bookings List -->
  <rect x="360" y="100" width="400" height="480" rx="12" fill="#0f1720" stroke="#1e2329"/>
  <text x="380" y="130" font-size="18" fill="#ffffff" font-weight="bold">My Bookings</text>
  
  <!-- Booking Card 1 -->
  <rect x="380" y="150" width="360" height="100" rx="8" fill="#0b0f12" stroke="#1e2329"/>
  <rect x="390" y="160" width="60" height="60" rx="8" fill="#1e2329"/>
  <text x="465" y="180" font-size="14" fill="#ffffff">Music Concert</text>
  <text x="465" y="200" font-size="12" fill="#8b92a0">Dec 15, 2025</text>
  <rect x="465" y="210" width="60" height="20" rx="4" fill="#00d97a" opacity="0.2"/>
  <text x="495" y="224" font-size="11" fill="#00d97a" text-anchor="middle">Confirmed</text>
  
  <!-- Booking Card 2 -->
  <rect x="380" y="265" width="360" height="100" rx="8" fill="#0b0f12" stroke="#1e2329"/>
  <rect x="390" y="275" width="60" height="60" rx="8" fill="#1e2329"/>
  <text x="465" y="295" font-size="14" fill="#ffffff">Tech Workshop</text>
  <text x="465" y="315" font-size="12" fill="#8b92a0">Dec 20, 2025</text>
  <rect x="465" y="325" width="60" height="20" rx="4" fill="#7C5CFF" opacity="0.2"/>
  <text x="495" y="339" font-size="11" fill="#7C5CFF" text-anchor="middle">Upcoming</text>
</svg>
```

**Key Elements:**
- User profile card (avatar, name, email, role badge)
- "Edit Profile" button
- Bookings list with event cards
- Booking status badges (confirmed, upcoming, cancelled)
- Event thumbnails and basic info

---

## User Flows

### Event Discovery & Booking Flow

```svg
<svg viewBox="0 0 1000 300" xmlns="http://www.w3.org/2000/svg">
  <!-- Flow Steps -->
  <rect x="20" y="100" width="140" height="80" rx="8" fill="#0f1720" stroke="#7C5CFF" stroke-width="2"/>
  <text x="90" y="145" font-size="14" fill="#ffffff" text-anchor="middle" font-weight="bold">Landing Page</text>
  
  <path d="M 160 140 L 210 140" stroke="#7C5CFF" stroke-width="2" marker-end="url(#arrowhead)"/>
  
  <rect x="210" y="100" width="140" height="80" rx="8" fill="#0f1720" stroke="#7C5CFF" stroke-width="2"/>
  <text x="280" y="135" font-size="14" fill="#ffffff" text-anchor="middle" font-weight="bold">Browse Events</text>
  <text x="280" y="155" font-size="11" fill="#8b92a0" text-anchor="middle">(explore.html)</text>
  
  <path d="M 350 140 L 400 140" stroke="#7C5CFF" stroke-width="2" marker-end="url(#arrowhead)"/>
  
  <rect x="400" y="100" width="140" height="80" rx="8" fill="#0f1720" stroke="#7C5CFF" stroke-width="2"/>
  <text x="470" y="135" font-size="14" fill="#ffffff" text-anchor="middle" font-weight="bold">Event Details</text>
  <text x="470" y="155" font-size="11" fill="#8b92a0" text-anchor="middle">(event.html)</text>
  
  <path d="M 540 140 L 590 140" stroke="#7C5CFF" stroke-width="2" marker-end="url(#arrowhead)"/>
  
  <rect x="590" y="100" width="140" height="80" rx="8" fill="#0f1720" stroke="#7C5CFF" stroke-width="2"/>
  <text x="660" y="135" font-size="14" fill="#ffffff" text-anchor="middle" font-weight="bold">Booking</text>
  <text x="660" y="155" font-size="11" fill="#8b92a0" text-anchor="middle">(book.html)</text>
  
  <path d="M 730 140 L 780 140" stroke="#7C5CFF" stroke-width="2" marker-end="url(#arrowhead)"/>
  
  <rect x="780" y="100" width="140" height="80" rx="8" fill="#0f1720" stroke="#00d97a" stroke-width="2"/>
  <text x="850" y="135" font-size="14" fill="#ffffff" text-anchor="middle" font-weight="bold">Success</text>
  <text x="850" y="155" font-size="11" fill="#00d97a" text-anchor="middle">(payment-success)</text>
  
  <!-- Arrow marker definition -->
  <defs>
    <marker id="arrowhead" markerWidth="10" markerHeight="10" refX="9" refY="3" orient="auto">
      <polygon points="0 0, 10 3, 0 6" fill="#7C5CFF"/>
    </marker>
  </defs>
</svg>
```

**Flow Steps:**
1. **Landing Page**: User arrives, sees hero and featured events
2. **Browse Events**: User filters and searches for events
3. **Event Details**: User views full event information and ticket options
4. **Booking**: User selects tickets, enters payment info, completes purchase
5. **Success**: User sees confirmation, downloads PDF ticket, receives email

---

## Responsive Design

### Breakpoints

```css
/* Mobile */
@media (max-width: 767px) { }

/* Tablet */
@media (min-width: 768px) and (max-width: 1023px) { }

/* Desktop */
@media (min-width: 1024px) { }
```

### Mobile-First Approach

**Design Principles:**
- Stack layouts vertically on mobile
- Touch-friendly buttons (minimum 44px height)
- Collapsible navigation menu (hamburger icon)
- Single-column event grid
- Bottom-fixed CTAs for easy thumb access
- Larger font sizes for readability on small screens
- Simplified forms with one column
- Reduced padding and margins to maximize content area

**Mobile Optimizations:**
- Event cards: Full width, stacked vertically
- Navigation: Hamburger menu with slide-out drawer
- Hero section: Reduced height, larger text
- Forms: Full width inputs, stacked fields
- Booking summary: Collapsible on mobile
- Payment form: Single column layout

**Tablet Optimizations:**
- Event grid: 2 columns
- Side-by-side layouts for some sections
- Expanded navigation (not hamburger)
- Larger touch targets maintained

**Desktop Optimizations:**
- Event grid: 3-4 columns
- Two-column layouts (sidebar + main content)
- Hover effects and transitions
- Expanded information density

---

## Accessibility Features

### WCAG 2.1 AA Compliance

✅ **Keyboard Navigation**
- Tab navigation through all interactive elements
- Enter/Space to activate buttons and links
- Escape to close modals
- Arrow keys for dropdown navigation

✅ **Screen Reader Support**
- ARIA labels on all interactive elements
- ARIA live regions for dynamic content
- Semantic HTML (nav, main, section, article)
- Alt text on all images
- Form labels properly associated with inputs

✅ **Visual Accessibility**
- Color contrast ratio ≥ 4.5:1 for normal text
- Color contrast ratio ≥ 3:1 for large text
- Focus indicators on all focusable elements (2px purple outline)
- No information conveyed by color alone
- Resizable text up to 200% without loss of functionality

✅ **Interaction Accessibility**
- Touch targets minimum 44x44px
- Error messages clearly associated with form fields
- Success messages announced to screen readers
- Loading states with accessible labels
- Skip to main content link

### Accessibility Checklist

- [ ] All images have alt text
- [ ] All form inputs have labels
- [ ] All buttons have descriptive text or aria-label
- [ ] Color contrast meets WCAG AA standards
- [ ] Focus indicators are visible
- [ ] Keyboard navigation works for all interactions
- [ ] Screen reader can access all content
- [ ] Error messages are clear and helpful
- [ ] No flashing content (seizure risk)
- [ ] Page structure uses semantic HTML

---

## Animation & Transitions

### Page Load Animations

```css
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.page-content {
  animation: fadeIn 300ms ease-in-out;
}
```

### Hover States

```css
.card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
  transition: transform 200ms ease, box-shadow 200ms ease;
}

.button:hover {
  transform: scale(1.02);
  box-shadow: 0 8px 16px rgba(124, 92, 255, 0.3);
  transition: transform 200ms ease, box-shadow 200ms ease;
}
```

### Modal Open/Close

```css
@keyframes modalFadeIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.modal {
  animation: modalFadeIn 250ms ease-out;
}
```

### Loading Spinners

```css
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.spinner {
  animation: spin 1s linear infinite;
}
```

### Success/Error Toasts

```css
@keyframes slideInFromTop {
  from {
    opacity: 0;
    transform: translateY(-100%);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.toast {
  animation: slideInFromTop 300ms cubic-bezier(0.68, -0.55, 0.265, 1.55);
}
```

### Micro-interactions

- **Input Focus**: Border color transition (200ms)
- **Button Click**: Scale down slightly (0.98) for 100ms
- **Card Hover**: Lift up 4px with shadow (200ms)
- **Badge Appearance**: Fade in with scale (150ms)
- **Status Change**: Color transition (300ms)

---

## Recent Updates

### Week 6.5 Enhancements (November 27-30, 2025)

#### Dark Theme Consistency

**Updated Components:**
- Payment success page colors: `var(--success)`, `var(--card)`, `var(--muted)`
- Booking summary colors: `var(--card)`, `var(--border)`, `var(--text)`
- Modal titles: Added `color: var(--text)` for visibility in dark overlays
- All form inputs: Consistent dark backgrounds with proper contrast

**CSS Variables Usage:**
All pages now use CSS custom properties instead of hardcoded colors for easy theme switching:
```css
background-color: var(--card);
border-color: var(--border);
color: var(--text);
```

#### Enhanced Components

**Ticket Verification Page (New)**
- Status indicators: Valid (green checkmark), Invalid (red X), Loading (spinner)
- Event status badges: Upcoming, Ongoing, Ended
- Centered success icon (80px diameter)
- Detailed ticket information display
- Top-aligned layout for better mobile UX

**PDF Tickets**
- Updated QR code section with clickable verification links
- Blue underlined text for better accessibility
- Improved spacing and layout
- Clear verification instructions
- Professional design matching brand

**Email Templates**
- Professional HTML design with gradient header (#7C5CFF to #5B3FE5)
- Centered checkmark icon in success emails
- Booking details table with proper styling
- PDF attachment support
- Mobile-responsive email layout
- Consistent with web design system

**User Dashboard**
- Simplified role display: "Admin" or "User" only
- Fixed edit profile modal visibility issue
- Improved booking card layout
- Better status badge positioning
- Enhanced profile section design

#### Real-time Features

**Dynamic Data Updates:**
- Ticket availability counts fetched from API in real-time
- Dynamic "X% remaining" calculations
- Live booking status updates
- Immediate payment confirmation
- Real-time error handling and retry logic

**API Integration:**
- `/api/Tickets/available` endpoint for live counts
- `/api/Tickets/verify/{token}` for QR validation
- Proper error handling with user-friendly messages
- Loading states during API calls
- Optimistic UI updates

#### Performance Optimizations

- Reduced unnecessary re-renders
- Optimized image loading with lazy loading
- Minimized API calls with caching
- Efficient DOM manipulation
- Debounced search inputs

#### User Experience Improvements

- Better loading states across all pages
- Clearer error messages
- Success confirmations with animations
- Improved form validation feedback
- Smoother page transitions
- More intuitive navigation

---

## Design Guidelines

### Do's ✅

- Use CSS custom properties for colors
- Maintain 8px spacing grid
- Use semantic HTML elements
- Provide focus indicators
- Include loading states
- Show clear error messages
- Use consistent border radius (8px for buttons, 12px for cards)
- Add hover effects on interactive elements
- Ensure color contrast meets WCAG standards
- Test on multiple screen sizes

### Don'ts ❌

- Don't use hardcoded color values
- Don't rely on color alone to convey information
- Don't create touch targets smaller than 44x44px
- Don't use auto-playing animations
- Don't forget alt text on images
- Don't disable zoom on mobile
- Don't use low-contrast text
- Don't create keyboard traps
- Don't overuse animations
- Don't ignore loading states

---

## Future Enhancements

### Planned Features

- **Dark/Light Mode Toggle**: Allow users to switch themes
- **Advanced Animations**: More sophisticated micro-interactions
- **Skeleton Loading**: Replace spinners with skeleton screens
- **Progressive Web App**: Add PWA features for offline support
- **Custom Themes**: Allow organizers to customize event pages
- **Accessibility Audit**: Comprehensive WCAG 2.1 AAA compliance
- **Performance Monitoring**: Add analytics for user interactions
- **A/B Testing**: Test design variations for better conversions

---

## Resources

- **Figma Design Files**: [Link to design files]
- **Component Library**: [Link to Storybook]
- **Brand Assets**: [Link to logo and asset repository]
- **Style Guide**: This document
- **Accessibility Guide**: [Link to accessibility documentation]

---

**Document Version:** 2.0  
**Last Updated:** November 29, 2025  
**Maintained By:** Eventify Design Team
