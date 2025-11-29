# Stripe Payment Integration - Setup Guide

## Overview
The booking system now integrates with Stripe for secure payment processing. This guide explains how to configure and use the Stripe integration.

## Configuration

### 1. Stripe Publishable Key
Update the Stripe publishable key in `js/book-stripe.js`:

```javascript
const STRIPE_PUBLISHABLE_KEY = 'pk_test_YOUR_PUBLISHABLE_KEY_HERE';
```

**Where to find your key:**
1. Login to Stripe Dashboard (https://dashboard.stripe.com)
2. Go to Developers → API keys
3. Copy your **Publishable key** (starts with `pk_test_` for test mode or `pk_live_` for production)

### 2. Backend Configuration
The backend already has the webhook endpoint configured at:
```
POST /api/StripeWebhook
```

Ensure your `.env` or `appsettings.json` has:
- Stripe Secret Key
- Stripe Webhook Secret

## Payment Flow

### Step 1: User Fills Booking Form
- User selects event and category
- Fills in personal details (name, email, phone)
- Selects ticket quantity
- Enters payment card details (securely handled by Stripe)

### Step 2: Create Checkout Session
```javascript
POST /api/checkout
{
  "firstName": "John",
  "lastName": "Doe",
  "emailAddress": "john@example.com",
  "phoneNumber": "01012345678",
  "ticketsNum": 2,
  "totalPrice": 600.00,
  "categoryName": "Premium",
  "promoCode": null,
  "eventId": 1,
  "userId": 10,
  "currency": "usd",
  "ticketStatus": 0
}
```

**Response:**
```javascript
{
  "success": true,
  "data": {
    "bookingId": 25,
    "paymentId": 25,
    "clientSecret": "pi_1234567890_secret_abcdef123456"
  }
}
```

### Step 3: Confirm Payment with Stripe
Frontend uses the `clientSecret` to confirm payment with Stripe:

```javascript
const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
  payment_method: {
    card: cardElement,
    billing_details: {
      name: 'John Doe',
      email: 'john@example.com'
    }
  }
});
```

### Step 4: Webhook Processes Payment
Stripe sends webhook event to `/api/StripeWebhook` which:
- Verifies the payment
- Updates booking status to Confirmed
- Updates payment status to Paid
- Sends confirmation email (if configured)

## Testing with Stripe Test Cards

Use these test card numbers in development:

| Card Number | Description |
|------------|-------------|
| 4242 4242 4242 4242 | Successful payment |
| 4000 0000 0000 9995 | Declined payment |
| 4000 0025 0000 3155 | Requires authentication (3D Secure) |

**For all test cards:**
- Use any future expiry date (e.g., 12/25)
- Use any 3-digit CVC (e.g., 123)
- Use any postal code (e.g., 12345)

## Security Features

### 1. PCI Compliance
- Card details never touch your server
- Stripe.js tokenizes card information
- All card data is encrypted by Stripe

### 2. 3D Secure Support
The integration automatically handles 3D Secure authentication when required by the card issuer.

### 3. Webhook Signature Verification
The backend verifies all webhook requests are from Stripe using signature verification.

## Error Handling

### Common Errors

1. **"Card declined"**
   - User should try different card
   - Check with card issuer

2. **"Insufficient funds"**
   - User needs to use card with sufficient balance

3. **"Payment failed"**
   - Generic error - retry or use different payment method

### Error Display
All errors are displayed to the user in a user-friendly format with actionable messages.

## API Endpoints

### Create Checkout Session
```
POST /api/checkout
Authorization: Bearer {token}
```

### Get Booking Details
```
GET /api/booking/{id}
Authorization: Bearer {token}
```

### Get Payment Details
```
GET /api/payment/{bookingId}
Authorization: Bearer {token}
```

### Get User Bookings
```
GET /api/booking
Authorization: Bearer {token}
```

### Request Refund
```
POST /api/payment/{bookingId}/refund
Authorization: Bearer {token}
```

## Frontend Files

### Modified Files
1. **book.html** - Updated with Stripe card element
2. **book-stripe.js** - New file with complete Stripe integration
3. **payment-success.html** - Payment success confirmation page

### Key Features
- Real-time card validation
- Inline error messages
- Loading states during payment processing
- Responsive design
- Mobile-friendly payment form

## Configuration Checklist

- [ ] Add Stripe publishable key to `book-stripe.js`
- [ ] Configure Stripe secret key in backend
- [ ] Set up webhook endpoint in Stripe Dashboard
- [ ] Test with Stripe test cards
- [ ] Configure webhook secret in backend
- [ ] Test complete booking flow
- [ ] Verify email notifications work
- [ ] Test refund functionality

## Going Live

### Before Production:
1. Replace test keys with live keys
2. Test with real cards in small amounts
3. Set up proper error monitoring
4. Configure email templates
5. Add terms and conditions
6. Implement proper logging
7. Test webhook reliability
8. Set up Stripe Radar for fraud prevention

## Support

For Stripe-specific issues:
- Stripe Documentation: https://stripe.com/docs
- Stripe Support: https://support.stripe.com

For integration issues:
- Check browser console for errors
- Verify API keys are correct
- Test webhook with Stripe CLI
- Check backend logs for errors

## Additional Resources

- [Stripe.js Reference](https://stripe.com/docs/js)
- [Payment Intents API](https://stripe.com/docs/payments/payment-intents)
- [Webhook Events](https://stripe.com/docs/webhooks)
- [Testing Guide](https://stripe.com/docs/testing)
