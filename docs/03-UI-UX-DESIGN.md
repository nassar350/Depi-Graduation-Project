# UI/UX Design Documentation

## Overview

This document outlines the user interface and user experience design for Eventify, including design principles, page structures, component library, user flows, and accessibility considerations.

---

## Design Principles

### 1. Simplicity First
- **Focused Purchase Flows:** Minimize form fields and steps required to book tickets
- **Clear Hierarchy:** Important actions (Book Now, Pay) are visually prominent
- **Progressive Disclosure:** Show details only when needed

### 2. Mobile-First & Responsive
- **Mobile Priority:** Design for mobile screens first, then scale up
- **Touch-Friendly:** Buttons and interactive elements sized for fingers (min 44px)
- **Adaptive Layouts:** Single column on mobile, multi-column on desktop

### 3. Trust & Security
- **Visual Trust Signals:** SSL padlock, Stripe badge, security icons
- **Transparent Pricing:** No hidden fees, clear breakdown of costs
- **Secure Payment UX:** Stripe Elements for card input, PCI compliance messaging

### 4. Accessibility (WCAG AA)
- **Color Contrast:** Text meets AA contrast ratios (4.5:1 for normal text)
- **Keyboard Navigation:** All interactive elements accessible via keyboard
- **Screen Reader Support:** Semantic HTML, ARIA labels, alt text for images
- **Focus States:** Clear visual indication of focused elements

### 5. Performance
- **Fast Load Times:** Optimize images, minimize JS bundles
- **Progressive Enhancement:** Core functionality works without JavaScript
- **Perceived Performance:** Loading states, skeleton screens, optimistic UI

---

## User Personas

### Persona 1: Sarah — Event Attendee
- **Age:** 28, Marketing Professional
- **Goals:** Find and book tickets for weekend concerts and workshops
- **Frustrations:** Complicated checkout, hidden fees, slow mobile sites
- **Needs:** Quick search, mobile-friendly booking, instant tickets

### Persona 2: Mike — Event Organizer
- **Age:** 35, Community Manager
- **Goals:** Organize local meetups and workshops, manage attendees
- **Frustrations:** Complex event creation, unreliable payment processing
- **Needs:** Simple event creation, attendee management, timely payouts

---

## User Flows

### Flow 1: Attendee Booking Flow

```
Start
  ↓
1. Land on Homepage
  ↓
2. Browse Featured / Search Events
  ↓
3. Click Event Card → Event Detail Page
  ↓
4. View Event Info, Categories, Pricing
  ↓
5. Click "Book Now" or "Select Category"
  ↓
6. Redirect to Booking Page (eventId, categoryId in URL)
  ↓
7. Fill Booking Form (Name, Email, Phone, Quantity)
  ↓
8. Enter Payment Card (Stripe Elements)
  ↓
9. Click "Complete Booking & Pay"
  ↓
10. Payment Processing → Success/Error
  ↓
11. Show Confirmation Modal with Booking ID
  ↓
12. Email Ticket (automated via webhook)
  ↓
End
```

### Flow 2: Organizer Event Creation Flow

```
Start
  ↓
1. Login/Register
  ↓
2. Navigate to "Create Event"
  ↓
3. Step 1: Event Details (Name, Description, Date, Location)
  ↓
4. Step 2: Upload Event Image
  ↓
5. Step 3: Add Categories (Name, Price, Seats)
  ↓
6. Step 4: Review & Publish
  ↓
7. Event Published → Visible in Explore
  ↓
End
```

### Flow 3: Authentication Flow

```
Start
  ↓
1. Click "Login" or "Sign Up"
  ↓
2. Enter Email & Password
  ↓
3. Submit → API Validation
  ↓
4. Success: Store Token & User Data in localStorage
  ↓
5. Redirect to Dashboard or Original Page
  ↓
End
```

---

## Page Structures

### 1. Homepage (`index.html`)

**Purpose:** Landing page with featured events and search

**Key Sections:**
- **Hero Banner:** Full-width banner with search bar and CTA
- **Featured Events:** Carousel or grid of highlighted events
- **Upcoming Events:** Grid of upcoming events (10 cards)
- **Categories:** Quick filter chips (Concerts, Workshops, Sports, etc.)
- **Newsletter CTA:** Email signup for event updates

**Components:**
- Event card (image, title, date, location, price, "View" button)
- Search bar with autocomplete
- Featured carousel with navigation dots
- Newsletter form

**User Actions:**
- Search for events
- Browse featured/upcoming events
- Click event card → Navigate to event detail
- Filter by category
- Sign up for newsletter

---

### 2. Explore Page (`explore.html`)

**Purpose:** Browse and filter events

**Key Sections:**
- **Search & Filters:** Sidebar or top bar with filters
  - Search by name/keyword
  - Filter by date range
  - Filter by category
  - Filter by location
  - Filter by price range
- **Results Grid:** Responsive grid of event cards
- **Pagination:** Load more or page numbers

**Components:**
- Filter panel (collapsible on mobile)
- Event card grid
- Pagination controls
- Empty state (no results found)

**User Actions:**
- Apply filters
- Search events
- Sort results (date, price, popularity)
- Click event card → Navigate to event detail

---

### 3. Event Detail Page (`event.html`)

**Purpose:** Display full event information and enable booking

**Key Sections:**
- **Hero:** Event image, title, date, location, capacity
- **Description:** Full event description and details
- **Ticket Categories:** List of available categories with pricing
  - Category name
  - Price per ticket
  - Seats available
  - "Select" button
- **Organizer Info:** Organizer name, bio, contact
- **Event Info Summary:** Date/time, venue, tickets available, organizer
- **Share Options:** Copy link, social media share buttons
- **Booking Summary (Sidebar):** Sticky summary with "Book Now" CTA

**Components:**
- Event hero with overlay text
- Category cards with select buttons
- Organizer card
- Booking summary card (sticky)
- Share buttons

**User Actions:**
- View event details
- Select category → Navigate to booking page
- Click "Book Now" → Navigate to booking (first available category)
- Share event via link or social media

---

### 4. Booking Page (`book.html`)

**Purpose:** Collect booking details and process payment

**Key Sections:**
- **Progress Indicator:** Step 1: Details, Step 2: Payment, Step 3: Confirmation
- **Event Summary (Sidebar):** Event name, date, location, category, quantity, price
- **Booking Form:**
  - First Name (required)
  - Last Name (required)
  - Email Address (required)
  - Phone Number (optional)
  - Ticket Quantity (dropdown, 1-10)
  - Promo Code (optional, with "Apply" button)
- **Payment Section:**
  - Stripe card element (card number, expiry, CVC)
  - Security badge (SSL, Stripe)
- **Order Summary (Sidebar):**
  - Tickets × Price
  - Promo discount (if applied)
  - Total
- **Submit Button:** "Complete Booking & Pay"

**Components:**
- Multi-step progress indicator
- Form with validation states (error, success)
- Stripe Elements card input
- Order summary card (sticky)
- Confirmation modal

**User Actions:**
- Fill booking details
- Select ticket quantity
- Apply promo code (optional)
- Enter payment card
- Submit booking → Process payment
- View confirmation modal with booking ID

---

### 5. Dashboard (`dashboard.html`)

**Purpose:** User tickets and organizer management

**Key Sections:**
- **User Tickets Tab:**
  - List of purchased tickets
  - Download ticket button
  - Event details
- **Organizer Tab (if organizer):**
  - List of created events
  - Booking count per event
  - "Create Event" button
  - Edit/Delete event actions

**Components:**
- Tab navigation
- Ticket card
- Event management table
- Action buttons

**User Actions:**
- View purchased tickets
- Download ticket PDF
- View created events (organizer)
- Edit/delete events (organizer)
- Navigate to create event

---

### 6. Login/Register (`login.html`, `register.html`)

**Purpose:** User authentication

**Key Sections:**
- **Form:**
  - Email (required)
  - Password (required, min 8 chars)
  - Confirm Password (register only)
  - Name (register only)
- **Submit Button:** "Login" or "Sign Up"
- **Alternative:** Link to switch between login/register
- **Social Login (Optional):** Google, Facebook placeholders

**Components:**
- Auth form with validation
- Password toggle (show/hide)
- Error/success messages
- Social login buttons

**User Actions:**
- Enter credentials
- Submit form → API validation
- Toggle password visibility
- Switch between login/register
- Redirect after successful auth

---

### 7. Create Event (`create-event.html`)

**Purpose:** Organizers create new events

**Key Sections:**
- **Step 1: Event Details**
  - Name
  - Description
  - Start Date/Time
  - End Date/Time
  - Location/Address
- **Step 2: Event Image**
  - Upload image (drag & drop or file picker)
- **Step 3: Categories**
  - Add category button
  - For each category: Name, Price, Seats
- **Step 4: Review & Publish**
  - Preview event
  - Submit

**Components:**
- Multi-step wizard
- Form inputs with validation
- Image uploader
- Dynamic category list (add/remove)
- Preview card

**User Actions:**
- Fill event details
- Upload image
- Add ticket categories
- Review event
- Publish event

---

## Component Library

### Event Card

**Visual Structure:**
```
┌─────────────────────────┐
│                         │
│   [Event Image]         │
│                         │
├─────────────────────────┤
│ Event Title             │
│ 📅 Date                 │
│ 📍 Location             │
│ 💰 Price                │
│                         │
│ [View Event Button]     │
└─────────────────────────┘
```

**Variations:**
- Featured card (larger, more details)
- Grid card (compact)
- List card (horizontal layout)

**States:**
- Default
- Hover (shadow increases, scale 1.02)
- Loading (skeleton placeholder)
- Sold Out (overlay badge)

---

### Category Card

**Visual Structure:**
```
┌─────────────────────────┐
│ Category Name    $50.00 │
├─────────────────────────┤
│ Description text here   │
│ seats available.        │
│                         │
│ [Select Category]       │
└─────────────────────────┘
```

**States:**
- Available (primary button)
- Sold Out (disabled button, grey overlay)
- Selected (highlighted border)

---

### Booking Summary Card

**Visual Structure:**
```
┌─────────────────────────┐
│ Booking Summary         │
├─────────────────────────┤
│ [Event Image]           │
│ Event Name              │
│ Date • Location         │
├─────────────────────────┤
│ Tickets: 2              │
│ Price/ticket: $50       │
│ Promo: -$10             │
├─────────────────────────┤
│ Total: $90.00           │
│                         │
│ 🔒 Secure Payment       │
└─────────────────────────┘
```

**Properties:**
- Sticky positioning on desktop
- Collapsible on mobile
- Real-time updates when quantity changes

---

### Button Styles

**Primary Button:**
- Background: Gradient purple-blue
- Text: White, bold
- Hover: Darker gradient, scale 1.02
- Active: Pressed effect
- Disabled: Grey, cursor not-allowed

**Secondary Button:**
- Background: Transparent
- Border: 2px solid primary color
- Text: Primary color
- Hover: Fill with primary color, white text

**Danger Button:**
- Background: Red
- Used for delete actions

---

### Form Elements

**Input Field:**
- Border: 1px solid grey
- Focus: Blue border, box-shadow
- Error: Red border, error message below
- Success: Green border, checkmark icon
- Placeholder: Light grey text

**Select Dropdown:**
- Custom styled dropdown
- Chevron icon on right
- Options list with hover state

**Checkbox/Radio:**
- Custom styled with accent color
- Label text clickable

---

## CSS Architecture

### CSS Variables (Design Tokens)

```css
:root {
  /* Colors */
  --primary: #7C5CFF;
  --accent: #00D4FF;
  --accent-2: #00D4FF;
  --success: #28a745;
  --danger: #dc3545;
  --warning: #ffc107;
  --text: #1a1a1a;
  --text-light: #6c757d;
  --muted: #999;
  --border: #e0e0e0;
  --bg: #ffffff;
  --bg-light: #f8f9fa;
  --card: #ffffff;
  
  /* Spacing */
  --space-xs: 0.25rem;
  --space-sm: 0.5rem;
  --space-md: 1rem;
  --space-lg: 1.5rem;
  --space-xl: 2rem;
  --space-2xl: 3rem;
  --space-3xl: 4rem;
  
  /* Typography */
  --font-family: 'Inter', -apple-system, sans-serif;
  --font-size-xs: 0.75rem;
  --font-size-sm: 0.875rem;
  --font-size-base: 1rem;
  --font-size-lg: 1.125rem;
  --font-size-xl: 1.25rem;
  --font-size-2xl: 1.5rem;
  --font-size-3xl: 2rem;
  
  /* Border Radius */
  --radius-sm: 4px;
  --radius-md: 8px;
  --radius-lg: 12px;
  --radius-full: 9999px;
  
  /* Shadows */
  --shadow-sm: 0 1px 2px rgba(0,0,0,0.05);
  --shadow-md: 0 4px 6px rgba(0,0,0,0.07);
  --shadow-lg: 0 10px 20px rgba(0,0,0,0.1);
  
  /* Animation */
  --anim-speed: 0.2s;
  --anim-easing: ease-out;
  
  /* Z-index */
  --z-dropdown: 1000;
  --z-modal: 1050;
  --z-tooltip: 1060;
}
```

### Utility Classes

- **Spacing:** `.mt-{size}`, `.mb-{size}`, `.p-{size}`
- **Flexbox:** `.flex`, `.flex-column`, `.flex-between`, `.gap-{size}`
- **Text:** `.text-center`, `.text-muted`, `.text-bold`
- **Colors:** `.text-primary`, `.bg-primary`
- **Responsive:** `.hide-mobile`, `.hide-desktop`

---

## Responsive Breakpoints

```css
/* Mobile First Approach */
/* Base styles: 0-640px */

/* Tablet */
@media (min-width: 641px) { ... }

/* Desktop */
@media (min-width: 1024px) { ... }

/* Large Desktop */
@media (min-width: 1280px) { ... }
```

### Layout Behavior

| Screen Size | Navbar | Event Grid | Booking Layout |
|-------------|--------|------------|----------------|
| Mobile (≤640px) | Hamburger menu | 1 column | 1 column, sticky summary |
| Tablet (641-1024px) | Full nav | 2 columns | 1 column stacked |
| Desktop (>1024px) | Full nav | 3-4 columns | 2 columns (form + summary) |

---

## Accessibility Features

### Keyboard Navigation
- **Tab Order:** Logical flow through interactive elements
- **Focus Indicators:** Clear blue outline on focused elements
- **Skip Links:** "Skip to main content" link for screen readers
- **Escape Key:** Close modals and dropdowns

### Screen Reader Support
- **Semantic HTML:** `<header>`, `<nav>`, `<main>`, `<article>`, `<aside>`, `<footer>`
- **ARIA Labels:** `aria-label`, `aria-describedby`, `aria-live` for dynamic content
- **Alt Text:** Descriptive alt attributes for all images
- **Form Labels:** Explicit `<label for="...">` associations

### Color & Contrast
- **Text Contrast:** 4.5:1 for normal text, 3:1 for large text
- **Link Indicators:** Not solely reliant on color (underline on hover)
- **Error States:** Icon + color + text message

### Motion & Animation
- **Reduced Motion:** Respect `prefers-reduced-motion` media query
- **No Auto-play:** Videos and carousels require user interaction

---

## Interaction Patterns

### Loading States
- **Page Load:** Full-page spinner or skeleton screens
- **Button Actions:** Spinner inside button, disabled state, text changes
- **Infinite Scroll:** Loading indicator at bottom

### Error Handling
- **Form Validation:** Inline error messages below fields
- **API Errors:** Toast notifications with retry option
- **404 Pages:** Friendly message with navigation options

### Success Feedback
- **Form Submission:** Success message, checkmark icon, auto-redirect
- **Actions:** Toast notification (top-right)
- **Payment Success:** Modal with confirmation and next steps

---

## Wireframes

See [`/docs/wireframes/`](./wireframes/) folder for visual wireframes:
- `01-homepage.svg`
- `02-event-detail.svg`
- `03-booking-flow.svg`
- `04-user-flow-diagram.svg`

---

## Design Handoff Checklist

- [ ] All pages designed with consistent component library
- [ ] Responsive layouts for mobile, tablet, desktop
- [ ] Accessibility audit completed (WCAG AA)
- [ ] Design assets exported (icons, images, logos)
- [ ] CSS variables documented
- [ ] Component variations documented (states, sizes)
- [ ] Animation specifications provided
- [ ] Error and empty states designed
- [ ] Loading states designed
- [ ] Dark mode considerations (optional, future phase)

---

## Future Enhancements

### Phase 2 (Post-MVP)
- Dark mode theme
- Advanced search with autocomplete
- Event recommendations (AI-driven)
- Social features (share, invite friends)
- Calendar integration (Google Calendar, iCal)
- Multi-language support (i18n)

### Phase 3 (Long-term)
- Mobile app (React Native or Flutter)
- Organizer analytics dashboard
- A/B testing framework
- Personalized homepage
- Event check-in app with QR scanner

---

## Conclusion

This UI/UX design documentation provides a comprehensive guide for implementing Eventify's user interface. By adhering to these principles, patterns, and specifications, we ensure a consistent, accessible, and delightful experience for all users.

For questions or clarifications, contact the design team or refer to the [component demo page](../EventifyFrontEnd/index.html).
