# UI/UX Design Mockups - README

## Overview
This directory contains high-fidelity UI mockups for the Eventify platform. The mockups are organized by user journey steps and demonstrate the complete visual design system.

## Design System

### Color Palette
- **Primary:** `#7C5CFF` (Purple) - CTAs, links, branding
- **Secondary:** `#00D4FF` (Cyan) - Highlights, badges
- **Success:** `#00C853` (Green) - Confirmations, success states
- **Warning:** `#FFD600` (Yellow) - Alerts, badges
- **Error:** `#FF6B6B` (Red) - Errors, cancellations
- **Dark:** `#1A1A2E` - Text, headings
- **Gray:** `#6B7280` - Secondary text
- **Light Gray:** `#F3F4F6` - Backgrounds, borders
- **White:** `#FFFFFF` - Cards, surfaces

### Typography
- **Font Family:** 'Segoe UI', Arial, sans-serif
- **Headings:** 
  - H1: 56px Bold
  - H2: 36px Bold
  - H3: 28px Bold
  - H4: 22px Bold
  - H5: 18px Bold
- **Body:** 
  - Large: 18px Regular
  - Medium: 16px Regular
  - Small: 14px Regular
  - Tiny: 12px Regular

### Spacing System
- XS: 8px
- SM: 16px
- MD: 24px
- LG: 32px
- XL: 48px
- 2XL: 64px

### Border Radius
- Small: 8px
- Medium: 16px
- Large: 24px
- Pill: 50%

## Mockup Files

### Step 1: Discovery & Exploration
1. **01-homepage-ui.svg** ✅ CREATED
   - Navigation bar with logo and auth buttons
   - Hero section with gradient background
   - Search bar with category filters
   - Featured events carousel (3 cards)
   - Upcoming events grid (8 cards)
   - Footer with links and social media

2. **02-explore-events-ui.svg** (Next)
   - Advanced filters sidebar
   - Search with autocomplete
   - Event cards grid with pagination
   - Sort options (date, price, popularity)
   - Map view toggle

### Step 2: Event Details
3. **03-event-detail-ui.svg** (Next)
   - Event hero image
   - Event information panel
   - Ticket categories with pricing
   - Organizer details
   - Location map
   - Related events section
   - Booking sidebar (sticky)

### Step 3: Authentication
4. **04-auth-ui.svg** (Next)
   - Login/Register toggle tabs
   - Form fields with validation
   - Social login options (Google, Facebook)
   - Password strength indicator
   - "Remember me" checkbox
   - Forgot password link

### Step 4: Booking Process
5. **05-booking-form-ui.svg** (Next)
   - Progress indicator (4 steps)
   - Attendee information form
   - Ticket quantity selector
   - Promo code input
   - Order summary sidebar
   - Terms & conditions checkbox

6. **06-payment-checkout-ui.svg** (Next)
   - Stripe card element
   - Billing address form
   - Payment method icons
   - Security badges (SSL, PCI)
   - Order summary review
   - "Pay Now" CTA

### Step 5: Confirmation
7. **07-success-confirmation-ui.svg** (Next)
   - Success checkmark animation placeholder
   - Booking reference number
   - Ticket details with QR codes
   - Event reminder info
   - Download tickets button
   - Email confirmation notice
   - "Add to Calendar" button

### Step 6: User Management
8. **08-user-dashboard-ui.svg** (Next)
   - Profile sidebar
   - Upcoming events section
   - Past events history
   - Saved/favorite events
   - Account settings tab
   - Booking statistics

### Step 7: Organizer Features
9. **09-create-event-ui.svg** (Next)
   - Event details form
   - Image upload with preview
   - Category/tag selector
   - Ticket category management
   - Date/time picker
   - Location autocomplete
   - Preview button

### Step 8: Responsive Design
10. **10-mobile-responsive-ui.svg** (Next)
    - Mobile homepage (320px-480px width)
    - Mobile navigation (hamburger menu)
    - Mobile event card layout
    - Mobile booking form
    - Touch-optimized buttons

## Converting SVG to PNG

### Option 1: Using Online Tools
1. Open [CloudConvert](https://cloudconvert.com/svg-to-png)
2. Upload the SVG file
3. Set width to 1440px (or desired resolution)
4. Download the PNG

### Option 2: Using Inkscape (Free Desktop Tool)
```bash
# Install Inkscape from https://inkscape.org/
# Convert via command line:
inkscape 01-homepage-ui.svg --export-type=png --export-filename=01-homepage-ui.png --export-width=1440
```

### Option 3: Using VS Code Extension
1. Install "SVG" extension by jock
2. Right-click on SVG file
3. Select "SVG: Export PNG"
4. Choose resolution (1x, 2x, 3x)

### Option 4: Using PowerShell (ImageMagick)
```powershell
# Install ImageMagick: choco install imagemagick
magick convert -density 300 01-homepage-ui.svg 01-homepage-ui.png
```

### Option 5: Using Browser Console
1. Open SVG file in Chrome/Edge
2. Right-click → "Inspect"
3. Run in console:
```javascript
const svg = document.querySelector('svg');
const canvas = document.createElement('canvas');
canvas.width = 1440;
canvas.height = svg.getAttribute('height');
const ctx = canvas.getContext('2d');
const img = new Image();
img.onload = () => {
  ctx.drawImage(img, 0, 0);
  canvas.toBlob(blob => {
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'homepage.png';
    a.click();
  });
};
img.src = 'data:image/svg+xml;base64,' + btoa(new XMLSerializer().serializeToString(svg));
```

## Recommended Export Settings

### For Documentation
- **Format:** PNG
- **Width:** 1440px
- **DPI:** 72
- **Background:** White
- **Compression:** Medium

### For Presentations
- **Format:** PNG
- **Width:** 1920px
- **DPI:** 96
- **Background:** Transparent
- **Compression:** Low

### For Web/Prototyping
- **Format:** PNG or WEBP
- **Width:** 1440px
- **DPI:** 72
- **Background:** White
- **Compression:** High

## Usage in Documentation

### Markdown Reference
```markdown
![Homepage UI Mockup](./ui-mockups/01-homepage-ui.png)
```

### HTML Reference
```html
<img src="./ui-mockups/01-homepage-ui.png" alt="Homepage UI" width="100%">
```

## Design Decisions

### 1. Color Scheme
- Purple primary color conveys creativity and innovation
- High contrast for accessibility (WCAG AA compliant)
- Colorful event cards to showcase diversity

### 2. Layout
- F-pattern reading flow
- Card-based design for scannability
- Generous whitespace for breathing room
- Mobile-first responsive approach

### 3. Typography
- System fonts for performance
- Clear hierarchy with size/weight variations
- 16px minimum for body text (readability)

### 4. Interactive Elements
- 48px minimum touch target size
- Clear hover/focus states
- Loading states for async operations
- Optimistic UI updates

### 5. Accessibility
- Semantic HTML structure
- ARIA labels for screen readers
- Keyboard navigation support
- Alt text for all images
- Color contrast ratio > 4.5:1

## Next Steps

1. ✅ Convert SVG mockups to PNG (1440px width)
2. 🔄 Create remaining 9 mockup files
3. 📱 Add mobile/tablet variants
4. 🎨 Create component library in Figma (optional)
5. 🖼️ Add interactive prototype links
6. 📊 Conduct usability testing
7. 🎯 Iterate based on feedback

## Tools Used
- **Design:** SVG (code-based design)
- **Icons:** Emoji (Unicode)
- **Fonts:** System fonts (Segoe UI)
- **Colors:** Hex values
- **Gradients:** Linear SVG gradients

## Notes
- SVG files are resolution-independent and can be scaled to any size
- All mockups follow the 12-column grid system
- Shadows use consistent 4px blur with 10% opacity
- Animations and micro-interactions are documented separately

---

**Last Updated:** November 26, 2024  
**Design Version:** 1.0  
**Status:** In Progress (1/10 mockups completed)
