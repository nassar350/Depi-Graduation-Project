# UI/UX Design Mockups - Step-by-Step Guide

## 📋 Mockup Generation Status

### ✅ Completed Mockups (2/10)
1. **01-homepage-ui.svg** - Landing page with hero, featured events, event grid
2. **02-explore-events-ui.svg** - Browse page with filters, search, pagination

### 🔄 Ready to Generate (8 Remaining)

Below are detailed specifications for each remaining mockup. Use these specifications to generate the mockups using design tools or convert to PNG.

---

## Step-by-Step User Journey Classification

### **Phase 1: Discovery** ✅ COMPLETED
**User Goal:** Find interesting events to attend

| Step | Mockup File | Status | Components |
|------|-------------|--------|------------|
| 1.1 | 01-homepage-ui.svg | ✅ Done | Nav, Hero, Search, Featured Cards, Event Grid, Footer |
| 1.2 | 02-explore-events-ui.svg | ✅ Done | Filters Sidebar, Sort, Event Cards, Pagination |

**Key Features Demonstrated:**
- Search and filter functionality
- Category browsing
- Event card design
- Responsive grid layout
- Price display
- Availability indicators

---

### **Phase 2: Event Details**
**User Goal:** Learn more about a specific event

| Step | Mockup File | Status | Components | Priority |
|------|-------------|--------|------------|----------|
| 2.1 | 03-event-detail-ui.svg | 🔄 TODO | Hero Image, Details Panel, Categories Table, Map, Related Events | HIGH |

**Specification for 03-event-detail-ui.svg:**
```
Layout:
- Full-width hero image (1440x500px) with event title overlay
- 2-column layout: Main content (900px) + Sidebar (460px)

Main Content:
- Event description (rich text, 400-600 words placeholder)
- "About this event" section
- Date/time/location details with icons
- Organizer information card
  - Avatar (80x80px circle)
  - Name, email, phone
  - "Contact Organizer" button
- Ticket categories table
  - Category name, description, price, available seats
  - Visual availability bar
  - "Select" button for each category
- Event highlights (bullet points)
- Location map placeholder (800x400px)
- Related events carousel (3 cards)

Sidebar (Sticky):
- Booking summary card
  - Event name
  - Selected date/time
  - Starting price
  - "Book Now" CTA (large, purple)
- Share buttons (Facebook, Twitter, LinkedIn, Copy Link)
- "Save Event" button (heart icon)
- Safety information badge
- Refund policy notice

Colors:
- Use gradients for hero overlay
- Purple CTAs
- Green for availability indicators
- Gray for secondary text

Interactions:
- Sticky booking sidebar on scroll
- Hover states on category rows
- Map interactive indicator
```

---

### **Phase 3: Authentication**
**User Goal:** Create account or login to proceed with booking

| Step | Mockup File | Status | Components | Priority |
|------|-------------|--------|------------|----------|
| 3.1 | 04-auth-ui.svg | 🔄 TODO | Login/Register Forms, Social Login, Validation | HIGH |

**Specification for 04-auth-ui.svg:**
```
Layout:
- Centered modal or split-screen design (600x800px form area)
- Toggle tabs: "Login" | "Register"
- Background: Subtle gradient or event images with overlay

Login Tab:
- Email input field
- Password input field (with show/hide toggle)
- "Remember me" checkbox
- "Forgot password?" link (right-aligned)
- "Login" button (full-width, purple)
- Divider line with "OR" text
- Social login buttons:
  - "Continue with Google" (white, Google colors)
  - "Continue with Facebook" (blue)
- "Don't have an account? Sign up" link at bottom

Register Tab:
- Full name input
- Email input
- Phone number input (with country code dropdown)
- Password input (with strength indicator)
- Confirm password input
- Role selection: Radio buttons (User | Organizer)
- Terms & conditions checkbox
- "Create Account" button
- Social registration options
- "Already have an account? Login" link

Validation States:
- Error messages in red below fields
- Success checkmarks for valid fields
- Password strength bar (red/yellow/green)
- Disabled button state when form invalid

Form Fields Styling:
- Height: 50px
- Border radius: 8px
- Border: 1px solid #D1D5DB
- Focus: 2px solid #7C5CFF
- Icon inside field (left side for email, phone, etc.)
```

---

### **Phase 4: Booking Process**
**User Goal:** Select tickets and provide attendee information

| Step | Mockup File | Status | Components | Priority |
|------|-------------|--------|------------|----------|
| 4.1 | 05-booking-form-ui.svg | 🔄 TODO | Progress Steps, Form Fields, Order Summary | HIGH |
| 4.2 | 06-payment-checkout-ui.svg | 🔄 TODO | Stripe Card Element, Billing Form, Security Badges | HIGH |

**Specification for 05-booking-form-ui.svg:**
```
Layout:
- Progress indicator at top (4 steps)
  Step 1: Event Selection ✓
  Step 2: Attendee Details (active, purple)
  Step 3: Payment
  Step 4: Confirmation
- 2-column layout: Form (800px) + Order Summary Sidebar (560px)

Form Section:
- "Attendee Information" heading
- Full Name input (required)
- Email input (required, with validation)
- Phone Number input (required)
- "Ticket Selection" section
  - Category name badge
  - Price per ticket
  - Quantity selector (- button | number | + button)
  - Min: 1, Max: 10
  - Subtotal calculation
- "Promo Code" section (collapsible)
  - Input field
  - "Apply" button
  - Success/error message area
- "Special Requests" textarea (optional)
- Terms checkbox: "I agree to terms and conditions"
- "Continue to Payment" button (large, purple, disabled until valid)
- "Back to Event" link

Order Summary Sidebar (Sticky):
- Event thumbnail (100px height)
- Event name (truncated)
- Date, time, location
- Divider line
- Selected category
- Quantity × Price
- Subtotal
- Discount (if promo applied) in green
- Processing fee (small text)
- Total (large, bold)
- Security badges (SSL, Secure Checkout)
- "Need help?" support link

Responsive:
- Mobile: Summary at top, form below
- Desktop: Side-by-side

Micro-interactions:
- Quantity buttons animate
- Promo code success shows green checkmark
- Total updates dynamically
```

**Specification for 06-payment-checkout-ui.svg:**
```
Layout:
- Progress indicator (Step 3 active)
- 2-column layout: Payment Form (800px) + Order Summary (560px)

Payment Form:
- "Payment Information" heading
- Stripe card element placeholder (450x60px)
  - Card number field
  - Expiry (MM/YY)
  - CVC
  - Postal code
- "Billing Address" section
  - Country dropdown
  - Address line 1
  - Address line 2 (optional)
  - City
  - State/Province
  - Postal/ZIP code
- "Save card for future purchases" checkbox
- Payment method icons (Visa, Mastercard, Amex)
- Security badges:
  - "Secured by Stripe"
  - "256-bit SSL Encryption"
  - "PCI DSS Compliant"
- "Complete Booking" button (large, purple)
  - Show lock icon
  - Text: "Pay 1,000 EGP"
- Small text: "Your payment is processed securely. We don't store card details."
- "Back to Attendee Details" link

Order Summary (Same as booking form):
- Shows final amounts
- No editable fields
- "Edit Details" link at bottom

Footer Note:
- Cancellation policy
- Refund terms link

Error States:
- Card declined message in red
- Invalid card number feedback
- Expired card warning
```

---

### **Phase 5: Confirmation**
**User Goal:** Receive booking confirmation and tickets

| Step | Mockup File | Status | Components | Priority |
|------|-------------|--------|------------|----------|
| 5.1 | 07-success-confirmation-ui.svg | 🔄 TODO | Success Message, Ticket Details, QR Codes | MEDIUM |

**Specification for 07-success-confirmation-ui.svg:**
```
Layout:
- Centered content (900px max-width)
- Celebration confetti graphic (optional)

Success Section:
- Large green checkmark icon (120px)
- "Booking Confirmed!" heading (48px)
- "Your tickets have been sent to john.doe@example.com" subtext
- Booking reference: "REF: EVT-2024-042-XYZ" (monospace font, copyable)

Ticket Section:
- "Your Tickets" heading
- For each ticket:
  - Ticket card (400px width)
  - Event name
  - Category: VIP
  - Ticket number: VIP-046
  - QR code (200x200px placeholder)
  - Date, time, venue
  - Attendee name
  - "Important: Show this QR code at the entrance"

Action Buttons:
- "Download Tickets (PDF)" (primary button)
- "Add to Calendar" (secondary button)
- "Email Tickets" (secondary button)
- "Share on Social Media" (icon buttons)

Event Details Card:
- Event thumbnail
- Full event information
- Organizer contact
- Directions link

Next Steps:
- "What's Next?" section
  - Check your email for confirmation
  - Download the Eventify app (optional)
  - Save QR codes offline
  - Arrive 30 minutes early

Footer Actions:
- "View My Bookings" button
- "Browse More Events" link
- "Need Help? Contact Support"

Design:
- Use success green (#00C853) for checkmark and accents
- Cards with shadows
- Print-friendly layout consideration
```

---

### **Phase 6: User Dashboard**
**User Goal:** Manage bookings and account

| Step | Mockup File | Status | Components | Priority |
|------|-------------|--------|------------|----------|
| 6.1 | 08-user-dashboard-ui.svg | 🔄 TODO | Profile, Booking History, Settings | MEDIUM |

**Specification for 08-user-dashboard-ui.svg:**
```
Layout:
- Sidebar (280px) + Main content (1080px)

Sidebar:
- Profile section
  - Avatar (120px circle)
  - Name
  - Email
  - "Edit Profile" link
- Navigation menu:
  - Overview (active)
  - My Bookings
  - Saved Events
  - Account Settings
  - Logout

Main Content - Overview Tab:
- Welcome message: "Welcome back, John!"
- Statistics cards (3 across):
  - Total Bookings: 12
  - Upcoming Events: 3
  - Total Spent: 4,500 EGP
- "Upcoming Events" section
  - Event cards (horizontal layout)
  - Event thumbnail (150x150px)
  - Event name, date, time
  - Category badge
  - "View Ticket" button
  - "Cancel Booking" link (if cancellable)
- "Past Events" section (collapsed by default)
  - Similar cards
  - "Rate Event" button
- "Saved Events" section
  - Grid of saved event cards
  - Heart icon to unsave

Bookings Tab View:
- Filter: All | Upcoming | Past | Cancelled
- Sort: Date | Name | Price
- Booking cards list:
  - Booking ID
  - Event thumbnail
  - Event name
  - Date, time, venue
  - Status badge (Confirmed/Pending/Cancelled)
  - Payment status
  - Number of tickets
  - Total amount
  - Actions: View, Download, Cancel

Account Settings:
- Personal Information form
- Password change
- Email preferences checkboxes
- Notification settings
- Delete account button (red, bottom)

Design:
- Clean, dashboard aesthetic
- Data visualizations (optional charts)
- Status badges with colors
- Hover effects on cards
```

---

### **Phase 7: Organizer Features**
**User Goal:** Create and manage events

| Step | Mockup File | Status | Components | Priority |
|------|-------------|--------|------------|----------|
| 7.1 | 09-create-event-ui.svg | 🔄 TODO | Event Form, Image Upload, Category Management | LOW |

**Specification for 09-create-event-ui.svg:**
```
Layout:
- Full-width form (max 1200px centered)
- Progress steps: Basic Info → Details → Tickets → Publish

Basic Info Section:
- "Create New Event" heading
- Event name input (large, 24px font)
- Event category dropdown (multiselect)
- Event type: Radio buttons (Online | In-Person | Hybrid)
- "Event Image" upload area
  - Drag-and-drop zone (600x300px)
  - Image preview
  - "Upload Image" button
  - Recommended size: 1920x1080px
  - Max size: 5MB

Event Details:
- Description textarea (rich text editor)
  - Bold, italic, lists, links
  - Character count: 0/5000
- Start date/time picker
- End date/time picker
- Location input (with Google Maps autocomplete)
- Venue name (if in-person)
- Online meeting link (if online)

Ticket Categories:
- "Add Ticket Category" button
- For each category:
  - Category name (e.g., VIP, Regular, Student)
  - Description
  - Price input (EGP)
  - Number of seats
  - Early bird option checkbox
  - Sales start/end dates
  - "Remove Category" button

Advanced Settings (Collapsible):
- Age restriction
- Dress code
- Accessibility options
- Parking information
- Refund policy
- Contact information

Preview Section (Sidebar):
- Live preview of event card
- Shows how event will appear in listings
- "Preview Full Page" button

Action Buttons:
- "Save as Draft" (secondary)
- "Preview" (secondary)
- "Publish Event" (primary, purple)

Validation:
- Required fields marked with *
- Real-time validation feedback
- Error summary at top if submission fails

Design:
- Form fields with clear labels
- Help text below each field
- Icons for each section
- Smooth scroll to errors
```

---

### **Phase 8: Mobile Responsive**
**User Goal:** Use Eventify on mobile devices

| Step | Mockup File | Status | Components | Priority |
|------|-------------|--------|------------|----------|
| 8.1 | 10-mobile-responsive-ui.svg | 🔄 TODO | Mobile Views of Key Screens | MEDIUM |

**Specification for 10-mobile-responsive-ui.svg:**
```
Layout:
- Show 4-5 mobile screens side by side (375px width each)
- iPhone/Android frame around each screen

Screen 1: Mobile Homepage
- Hamburger menu (top left)
- Logo (center)
- Search icon (top right)
- Hero section (compressed)
- Search bar (full width)
- Category pills (horizontal scroll)
- Featured events (vertical stack)
- Event cards (full width)
- Bottom navigation bar:
  - Home | Explore | Tickets | Profile

Screen 2: Mobile Event Detail
- Back button (top left)
- Share button (top right)
- Hero image (full width, 250px height)
- Event name (large)
- Date/time/location (stacked)
- "Book Now" sticky button at bottom
- Ticket categories (accordion style)
- Organizer info (collapsible)
- Map (collapsible)

Screen 3: Mobile Booking Form
- Progress dots (4 dots)
- Form fields (full width, stacked)
- Large touch targets (56px minimum)
- Quantity selector (larger buttons)
- Order summary (collapsible accordion)
- Sticky "Continue" button at bottom

Screen 4: Mobile Menu
- Overlay menu (slide from left)
- Profile section at top
- Menu items (large touch targets):
  - Home
  - Explore Events
  - My Bookings
  - Saved Events
  - Account Settings
  - Help & Support
  - Logout
- Close button (X icon, top right)

Screen 5 (Optional): Mobile Ticket
- QR code centered (300px)
- Ticket details below
- "Add to Wallet" button
- "Show Brightness" helper text

Design Considerations:
- Touch targets minimum 48px
- Font sizes 16px minimum (body text)
- Single column layouts
- Bottom navigation for primary actions
- Swipe gestures indicated
- Loading states optimized for mobile
- Native app-like feel

Responsive Breakpoints:
- Mobile: 320px - 480px
- Tablet: 481px - 768px
- Desktop: 769px+
```

---

## 🎨 Design System Reference

Use these consistent values across ALL mockups:

### Colors (Hex Codes)
```css
--primary: #7C5CFF;
--primary-dark: #6844FF;
--secondary: #00D4FF;
--success: #00C853;
--warning: #FFD600;
--error: #FF6B6B;
--dark: #1A1A2E;
--gray-900: #1F2937;
--gray-700: #374151;
--gray-600: #4B5563;
--gray-500: #6B7280;
--gray-400: #9CA3AF;
--gray-300: #D1D5DB;
--gray-200: #E5E7EB;
--gray-100: #F3F4F6;
--gray-50: #FAFBFC;
--white: #FFFFFF;
```

### Typography
```css
--font-family: 'Segoe UI', 'Roboto', 'Helvetica Neue', Arial, sans-serif;

--text-xs: 12px;
--text-sm: 14px;
--text-base: 16px;
--text-lg: 18px;
--text-xl: 20px;
--text-2xl: 24px;
--text-3xl: 28px;
--text-4xl: 36px;
--text-5xl: 48px;
--text-6xl: 56px;

--font-normal: 400;
--font-medium: 500;
--font-semibold: 600;
--font-bold: 700;
```

### Spacing
```css
--space-xs: 4px;
--space-sm: 8px;
--space-md: 16px;
--space-lg: 24px;
--space-xl: 32px;
--space-2xl: 48px;
--space-3xl: 64px;
```

### Border Radius
```css
--radius-sm: 8px;
--radius-md: 12px;
--radius-lg: 16px;
--radius-xl: 24px;
--radius-full: 9999px;
```

### Shadows
```css
--shadow-sm: 0 1px 2px rgba(0,0,0,0.05);
--shadow-md: 0 4px 6px rgba(0,0,0,0.07);
--shadow-lg: 0 10px 15px rgba(0,0,0,0.1);
--shadow-xl: 0 20px 25px rgba(0,0,0,0.15);
```

---

## 🛠️ How to Generate Remaining Mockups

### Option 1: Use Figma (Recommended)
1. Create new Figma file
2. Set up color styles and typography from design system above
3. Create components for reusable elements (buttons, cards, inputs)
4. Build each mockup following specifications
5. Export as PNG (2x resolution, 1440px width)

### Option 2: Convert SVG to PNG
1. Complete the SVG code following the specs above
2. Use any of the conversion methods from README.md
3. Ensure 1440px width for desktop mockups
4. Use 375px width for mobile mockups

### Option 3: Use Online Tools
1. **Canva** - Use templates and customize
2. **Adobe XD** - Professional UI/UX tool
3. **Sketch** - Mac only, industry standard
4. **Penpot** - Free, open-source alternative

---

## 📊 Priority Order for Completion

### HIGH Priority (Complete First)
1. ✅ 01-homepage-ui.svg
2. ✅ 02-explore-events-ui.svg
3. 🔄 03-event-detail-ui.svg
4. 🔄 04-auth-ui.svg
5. 🔄 05-booking-form-ui.svg
6. 🔄 06-payment-checkout-ui.svg

### MEDIUM Priority (Complete Second)
7. 🔄 07-success-confirmation-ui.svg
8. 🔄 08-user-dashboard-ui.svg
9. 🔄 10-mobile-responsive-ui.svg

### LOW Priority (Complete Last)
10. 🔄 09-create-event-ui.svg

---

## ✅ Completion Checklist

Use this to track progress:

- [x] Phase 1: Discovery mockups (2/2)
- [ ] Phase 2: Event details mockup (0/1)
- [ ] Phase 3: Authentication mockup (0/1)
- [ ] Phase 4: Booking process mockups (0/2)
- [ ] Phase 5: Confirmation mockup (0/1)
- [ ] Phase 6: Dashboard mockup (0/1)
- [ ] Phase 7: Organizer mockup (0/1)
- [ ] Phase 8: Mobile mockup (0/1)

**Overall Progress: 2/10 (20%)**

---

## 📝 Notes

- Each mockup should be created at 1440px width for desktop
- Mobile mockups should be 375px width
- Use consistent spacing and components across all mockups
- All text should be readable and professional
- Include realistic placeholder content
- Show both active and inactive states where applicable
- Consider accessibility (color contrast, touch targets)

---

**Last Updated:** November 26, 2024  
**Next Action:** Generate mockups 3-6 (HIGH priority)
