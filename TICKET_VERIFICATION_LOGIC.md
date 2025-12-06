# Ticket Verification Logic Documentation

## Overview
Updated ticket verification system to comprehensively validate tickets based on payment status, booking status, and event timing.

## Validation Rules

### ✅ Valid Ticket
A ticket is considered **VALID** when **ALL** of the following conditions are met:
1. ✅ Payment Status = **Paid**
2. ✅ Booking Status = **Booked**
3. ✅ Event End Date > Current Date (event hasn't ended)

**Response:**
- `IsValid = true`
- `TicketStatus = "Valid"`
- `Message = "Ticket is valid"`

---

### ❌ Invalid Ticket Scenarios

#### 1. Payment Rejected
**Condition:** Payment Status = `Rejected`

**Response:**
- `IsValid = false`
- `TicketStatus = "PaymentRejected"`
- `InvalidReason = "Payment was rejected"`
- `Message = "Ticket is invalid - Payment was rejected"`

---

#### 2. Payment Refunded
**Condition:** Payment Status = `Refunded`

**Response:**
- `IsValid = false`
- `TicketStatus = "Refunded"`
- `InvalidReason = "Payment was refunded"`
- `Message = "Ticket is invalid - Payment was refunded"`

---

#### 3. Payment Pending
**Condition:** Payment Status = `Pending`

**Response:**
- `IsValid = false`
- `TicketStatus = "PaymentPending"`
- `InvalidReason = "Payment is still pending"`
- `Message = "Ticket is invalid - Payment is pending"`

---

#### 4. Booking Cancelled
**Condition:** Booking Status = `Cancelled`

**Response:**
- `IsValid = false`
- `TicketStatus = "Cancelled"`
- `InvalidReason = "Booking was cancelled"`
- `Message = "Ticket is invalid - Booking was cancelled"`

---

#### 5. Event Ended (Expired)
**Condition:** Event End Date < Current Date

**Response:**
- `IsValid = false`
- `TicketStatus = "Expired"`
- `InvalidReason = "Event ended on {EndDate}"`
- `Message = "Ticket has expired - Event has ended"`

---

## Validation Flow

```
┌─────────────────────────────┐
│   Decrypt Ticket Token      │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│   Load Ticket from DB       │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│   Load Booking from DB      │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│   Load Event from DB        │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────┐
│   Load Payment from DB      │
└──────────┬──────────────────┘
           │
           ▼
┌─────────────────────────────────────────────┐
│   Validation Priority (Checks in Order):    │
│                                             │
│   1. Payment Status = Rejected? ──> INVALID │
│   2. Payment Status = Refunded? ──> INVALID │
│   3. Payment Status = Pending?  ──> INVALID │
│   4. Booking Status = Cancelled? ──> INVALID│
│   5. Event End Date < Now?      ──> EXPIRED │
│   6. Payment=Paid & Booking=Booked? ──> VALID│
│   7. Other combinations         ──> INVALID │
└──────────┬──────────────────────────────────┘
           │
           ▼
┌─────────────────────────────┐
│   Return Verification       │
│   Response DTO              │
└─────────────────────────────┘
```

## Response DTO Structure

```csharp
public class TicketVerificationResponseDto
{
    public bool IsValid { get; set; }                  // Overall validity
    public int TicketId { get; set; }
    public int BookingId { get; set; }
    public string AttendeeName { get; set; }
    public string Email { get; set; }
    public string EventName { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime EventEndDate { get; set; }
    public string CategoryName { get; set; }
    
    // Status Information
    public string EventStatus { get; set; }            // "Upcoming", "Ongoing", "Ended"
    public string TicketStatus { get; set; }           // "Valid", "Expired", "Cancelled", "Refunded", "PaymentRejected", "PaymentPending"
    public string BookingStatus { get; set; }          // "Booked", "Pending", "Cancelled"
    public string PaymentStatus { get; set; }          // "Paid", "Pending", "Cancelled", "Rejected", "Refunded"
    public string InvalidReason { get; set; }          // Detailed reason if invalid
}
```

## Enum Values

### BookingStatus Enum
```csharp
public enum BookingStatus
{
    Booked = 0,      // Booking confirmed
    Pending = 1,     // Booking awaiting confirmation
    Cancelled = 2    // Booking cancelled by user or admin
}
```

### PaymentStatus Enum
```csharp
public enum PaymentStatus
{
    Paid = 0,        // Payment successful
    Pending = 1,     // Payment awaiting confirmation
    Cancelled = 2,   // Payment cancelled
    Rejected = 3,    // Payment rejected by gateway
    Refunded = 4     // Payment refunded to user
}
```

## Use Cases

### Use Case 1: Valid Entry
**Scenario:** User arrives at event with valid ticket
- Payment: Paid ✅
- Booking: Booked ✅
- Event: Ongoing/Upcoming ✅

**Result:** Gate opens, user can enter

---

### Use Case 2: Refunded Ticket
**Scenario:** User got refund but tries to enter
- Payment: Refunded ❌
- Booking: Cancelled ❌

**Result:** Access denied, "Payment was refunded"

---

### Use Case 3: Event Already Ended
**Scenario:** User tries to use ticket after event
- Payment: Paid ✅
- Booking: Booked ✅
- Event: Ended ❌

**Result:** Access denied, "Event ended on {date}"

---

### Use Case 4: Payment Still Processing
**Scenario:** User arrives but payment not confirmed
- Payment: Pending ❌

**Result:** Access denied, "Payment is pending"

---

### Use Case 5: Cancelled Booking
**Scenario:** User cancelled booking but tries to enter
- Booking: Cancelled ❌

**Result:** Access denied, "Booking was cancelled"

---

## API Endpoint

### Verify Ticket
```http
POST /api/tickets/verify
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
  "token": "encrypted_ticket_token"
}

Success Response (200 OK):
{
  "success": true,
  "message": "Ticket is valid",
  "data": {
    "isValid": true,
    "ticketId": 123,
    "bookingId": 45,
    "attendeeName": "John Doe",
    "email": "john@example.com",
    "eventName": "Tech Conference 2024",
    "eventDate": "2024-12-20T09:00:00Z",
    "eventEndDate": "2024-12-20T18:00:00Z",
    "categoryName": "VIP",
    "eventStatus": "Ongoing",
    "ticketStatus": "Valid",
    "bookingStatus": "Booked",
    "paymentStatus": "Paid",
    "invalidReason": ""
  },
  "errors": []
}

Invalid Response (200 OK):
{
  "success": false,
  "message": "Ticket is invalid - Payment was refunded",
  "data": {
    "isValid": false,
    "ticketId": 123,
    "bookingId": 45,
    "attendeeName": "John Doe",
    "email": "john@example.com",
    "eventName": "Tech Conference 2024",
    "eventDate": "2024-12-20T09:00:00Z",
    "eventEndDate": "2024-12-20T18:00:00Z",
    "categoryName": "VIP",
    "eventStatus": "Ended",
    "ticketStatus": "Refunded",
    "bookingStatus": "Cancelled",
    "paymentStatus": "Refunded",
    "invalidReason": "Payment was refunded"
  },
  "errors": ["Payment was refunded"]
}
```

## Implementation Details

### Files Modified

1. **TicketVerificationResponseDto.cs**
   - Added `TicketStatus` property
   - Added `BookingStatus` property
   - Added `PaymentStatus` property
   - Added `InvalidReason` property

2. **TicketVerificationService.cs**
   - Enhanced `VerifyTicketAsync` method
   - Added payment status checks
   - Added booking status checks
   - Added event timing checks
   - Implemented validation priority logic
   - Added detailed error messages

### Dependencies
- `IUnitOfWork` for data access
- `ITicketEncryptionService` for token decryption
- `PaymentStatus` enum from `Eventify.Core.Enums`
- `BookingStatus` enum from `Eventify.Core.Enums`

## Testing Scenarios

### Manual Testing Checklist

- [ ] Valid ticket (Paid + Booked + Event upcoming)
- [ ] Expired ticket (Event ended)
- [ ] Cancelled booking
- [ ] Refunded payment
- [ ] Rejected payment
- [ ] Pending payment
- [ ] Invalid token
- [ ] Non-existent ticket
- [ ] Non-existent booking
- [ ] Non-existent payment
- [ ] Non-existent event

## Security Considerations

1. **Token Encryption**: Tickets use encrypted tokens to prevent tampering
2. **Database Verification**: All data verified against database, not relying on token data
3. **Comprehensive Checks**: Multiple validation layers ensure security
4. **Detailed Logging**: All validation failures are logged with reasons
5. **Authorization**: Verify endpoint requires authentication

## Future Enhancements

- [ ] Add ticket usage tracking (prevent multiple entries)
- [ ] Add geolocation verification (verify ticket at event location)
- [ ] Add time-window validation (e.g., can't enter 2 hours before event)
- [ ] Add notification when ticket becomes invalid
- [ ] Add admin override for special cases
- [ ] Add analytics on verification attempts
- [ ] Add rate limiting on verification API

---

**Last Updated:** December 6, 2024
**Version:** 2.0
**Status:** Production Ready
