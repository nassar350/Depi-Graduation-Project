# Event Creation API Integration

## Overview
The event creation form has been successfully integrated with the backend API at `https://localhost:7105/api/Events`.

## Changes Made

### 1. HTML Form Updates (`create-event.html`)
- **Form encoding**: Changed to `multipart/form-data` for file uploads
- **Field names**: Updated to match API schema
  - `title` → `name`
  - `location` → `address` 
  - Combined date/time into `startDate` and `endDate` with `datetime-local` inputs
  - `image` → `photo` with file input type
- **Validation**: Added proper min/max length attributes per API requirements
- **Categories section**: Replaced tickets/pricing with dynamic category management including ticket prices
- **Removed fields**: Category dropdown, format, agenda, requirements, tags (not in API)

### 2. JavaScript Updates (`create-event.js`)
- **Complete rewrite** for backend integration
- **API endpoint**: `POST https://localhost:7105/api/Events`
- **Authentication**: Uses Bearer token from localStorage
- **Form data preparation**: Properly formats data for API
  - Converts datetime-local to ISO 8601 format
  - Creates categoriesJson as stringified array with ticket prices
  - Handles file upload for photos
- **Category management**: Dynamic add/remove functionality with pricing
- **Validation**: Updated to match API requirements
- **Error handling**: Comprehensive error handling with user feedback

### 3. Event Detail Page Updates (`event.js`)
- **Updated to use `ticketPrice`** field from backend API response
- **Category display**: Shows pricing information prominently
- **Booking flow**: Passes correct price information to booking page

### 4. API Schema Compliance
The form now matches the exact API requirements:

```typescript
interface EventCreationRequest {
  name: string;           // 3-100 characters
  description: string;    // 10-1000 characters  
  address: string;        // 5-200 characters
  startDate: string;      // ISO 8601 DateTime
  endDate: string;        // ISO 8601 DateTime
  photo?: File;          // Optional image file
  categoriesJson: string; // JSON array of {title, seats, TicketPrice}
}
```

### 5. Categories Format
Categories are sent as a JSON string in the format:
```json
"[
  {\"title\": \"VIP\", \"seats\": 50, \"TicketPrice\": 150},
  {\"title\": \"Premium\", \"seats\": 100, \"TicketPrice\": 200},
  {\"title\": \"Regular\", \"seats\": 200, \"TicketPrice\": 100}
]"
```

The backend returns categories with the following structure:
```json
{
  "id": 22,
  "eventId": 26,
  "title": "VIP",
  "ticketPrice": 1500,
  "seats": 50,
  "booked": 0
}
```

## Authentication
- Requires user to be logged in
- Uses JWT token from localStorage: `eventify_token`
- Sends token in Authorization header: `Bearer {token}`

## Validation
### Client-side validation:
- Event name: 3-100 characters
- Description: 10-1000 characters
- Address: 5-200 characters
- Start date: Must be in the future
- End date: Must be after start date
- At least one category required with valid price

### Server-side validation:
The API will validate all fields according to its schema and return appropriate error messages.

## Error Handling
- Network errors (connection issues)
- Authentication errors (invalid/expired token)
- Validation errors (field requirements not met)
- Server errors (backend issues)

All errors are displayed to the user with clear, actionable messages.

## Testing
A test file `test-event-creation.html` has been created to validate the integration without the full UI flow.

## Usage
1. User must be logged in (redirects to login if not)
2. Fill out event details across 4 steps:
   - Step 1: Basic details (name, address, dates)
   - Step 2: Description  
   - Step 3: Categories (title, seats, and ticket price)
   - Step 4: Review and publish
3. Form validates input and sends to API
4. Success: Shows confirmation modal with next actions
5. Error: Shows specific error message to user

## Files Modified
- `create-event.html` - Updated form structure and fields
- `js/create-event.js` - Complete rewrite for API integration with pricing
- `js/event.js` - Updated to handle `ticketPrice` field from backend
- `css/styles.css` - Added styles for price display in categories
- Added: `test-event-creation.html` - API integration test

The integration is now ready for use with the backend API and properly handles ticket pricing!