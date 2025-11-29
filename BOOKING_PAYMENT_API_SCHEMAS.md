# Booking and Payment API - Frontend Integration Guide

## **Complete Booking and Payment Flow JSON Schemas**

### **STEP 1: Create Checkout Session (Start Booking Process)**
**Endpoint:** `POST /api/checkout`

#### Request Schema:{
  "firstName": "John",
  "lastName": "Doe",
  "emailAddress": "john.doe@example.com",
  "phoneNumber": "01012345678",
  "ticketsNum": 2,
  "totalPrice": 600.00,
  "categoryName": "Premium",
  "promoCode": null,
  "eventId": 1,
  "userId": null,
  "currency": "usd",
  "ticketStatus": 0
}
#### Success Response (200):{
  "success": true,
  "message": "Checkout session created successfully",
  "data": {
    "bookingId": 25,
    "paymentId": 25,
    "clientSecret": "pi_1234567890_secret_abcdef123456"
  },
  "errors": []
}
#### Error Response (400):{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "You Must Enter Your First Name",
    "Number of tickets is required"
  ]
}
---

### **STEP 2: Confirm Payment (Frontend confirms payment with Stripe)**
This step happens on the frontend using Stripe's JavaScript SDK with the `clientSecret`.

---

### **STEP 3: Get Booking Details**
**Endpoint:** `GET /api/booking/{id}`

#### Success Response (200):{
  "success": true,
  "message": "Booking retrieved successfully",
  "data": {
    "id": 25,
    "createdDate": "2024-11-20T15:30:00Z",
    "status": 1,
    "ticketsNum": 2,
    "userId": 10,
    "eventId": 1
  },
  "errors": []
}
#### Error Response (404):{
  "success": false,
  "message": "Booking with ID 25 not found",
  "data": null,
  "errors": []
}
---

### **STEP 4: Get Payment Details (Enhanced with Full Information)**
**Endpoint:** `GET /api/payment/{bookingId}`

#### Success Response (200):{
  "success": true,
  "message": "Payment details retrieved successfully",
  "data": {
    "bookingId": 25,
    "totalPrice": 600.00,
    "paymentMethod": "Card",
    "stripePaymentIntentId": "pi_1234567890",
    "stripeClientSecret": "pi_1234567890_secret_abcdef123456",
    "status": "Paid",
    "dateTime": "2024-11-20T15:35:00Z",
    "booking": {
      "id": 25,
      "createdDate": "2024-11-20T15:30:00Z",
      "status": 1,
      "ticketsNum": 2,
      "userId": 10,
      "eventId": 1
    },
    "customerEmail": "john.doe@example.com",
    "customerPhone": "01012345678"
  },
  "errors": []
}
---

### **STEP 5: Get All User Bookings**
**Endpoint:** `GET /api/booking`

#### Success Response (200):{
  "success": true,
  "message": "Bookings retrieved successfully",
  "data": [
    {
      "id": 25,
      "createdDate": "2024-11-20T15:30:00Z",
      "status": 1,
      "ticketsNum": 2,
      "userId": 10,
      "eventId": 1
    },
    {
      "id": 26,
      "createdDate": "2024-11-21T10:15:00Z",
      "status": 0,
      "ticketsNum": 1,
      "userId": 10,
      "eventId": 2
    }
  ],
  "errors": []
}
#### Empty Response:{
  "success": false,
  "message": "No bookings found",
  "data": [],
  "errors": []
}
---

### **STEP 6: Get All User Payments**
**Endpoint:** `GET /api/payment`

#### Success Response (200):{
  "success": true,
  "message": "Payments retrieved successfully",
  "data": [
    {
      "bookingId": 25,
      "totalPrice": 600.00,
      "paymentMethod": "Card",
      "stripePaymentIntentId": "pi_1234567890",
      "stripeClientSecret": null,
      "status": "Paid",
      "dateTime": "2024-11-20T15:35:00Z"
    },
    {
      "bookingId": 26,
      "totalPrice": 150.00,
      "paymentMethod": "Card",
      "stripePaymentIntentId": "pi_0987654321",
      "stripeClientSecret": null,
      "status": "Pending",
      "dateTime": "2024-11-21T10:15:00Z"
    }
  ],
  "errors": []
}
---

### **STEP 7: Process Refund**
**Endpoint:** `POST /api/payment/{bookingId}/refund`

#### Success Response (200):{
  "success": true,
  "message": "Payment refunded successfully",
  "data": null,
  "errors": []
}
#### Error Response (400):{
  "success": false,
  "message": "Refund failed or payment not found",
  "data": null,
  "errors": ["Unable to process refund for this booking"]
}
---

## **Status Codes and Enums**

### **Ticket Status (TicketStatus enum):**{
  "0": "Pending",
  "1": "Confirmed", 
  "2": "Cancelled",
  "3": "Refunded"
}
### **Payment Status (PaymentStatus enum):**{
  "0": "Pending",
  "1": "Paid",
  "2": "Failed",
  "3": "Cancelled",
  "4": "Refunded"
}
---

## **Frontend Integration Examples**

### **Complete Booking Flow (JavaScript/React)**
// Configuration
const API_BASE_URL = 'https://localhost:7194/api';

// Step 1: Create checkout session
const initiateBooking = async (bookingData) => {
  try {
    const response = await fetch(`${API_BASE_URL}/checkout`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${getAuthToken()}`
      },
      body: JSON.stringify(bookingData)
    });

    const result = await response.json();
    
    if (result.success) {
      console.log('Checkout session created:', result.data);
      return {
        success: true,
        bookingId: result.data.bookingId,
        clientSecret: result.data.clientSecret
      };
    } else {
      console.error('Checkout failed:', result.errors);
      return { success: false, errors: result.errors };
    }
  } catch (error) {
    console.error('Network error:', error);
    return { success: false, errors: ['Network error occurred'] };
  }
};

// Step 2: Process payment with Stripe (frontend only)
const processPayment = async (clientSecret, stripe, elements) => {
  const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
    payment_method: {
      card: elements.getElement('card'),
      billing_details: {
        name: 'John Doe',
        email: 'john.doe@example.com'
      }
    }
  });

  if (error) {
    console.error('Payment failed:', error);
    return { success: false, error: error.message };
  }

  if (paymentIntent.status === 'succeeded') {
    console.log('Payment succeeded!');
    return { success: true, paymentIntent };
  }

  return { success: false, error: 'Payment not completed' };
};

// Step 3: Get booking details
const getBookingDetails = async (bookingId) => {
  try {
    const response = await fetch(`${API_BASE_URL}/booking/${bookingId}`, {
      headers: {
        'Authorization': `Bearer ${getAuthToken()}`
      }
    });

    const result = await response.json();
    
    if (result.success) {
      return { success: true, booking: result.data };
    } else {
      return { success: false, error: result.message };
    }
  } catch (error) {
    console.error('Error fetching booking:', error);
    return { success: false, error: 'Failed to fetch booking details' };
  }
};

// Step 4: Get payment details with full information
const getPaymentDetails = async (bookingId) => {
  try {
    const response = await fetch(`${API_BASE_URL}/payment/${bookingId}`, {
      headers: {
        'Authorization': `Bearer ${getAuthToken()}`
      }
    });

    const result = await response.json();
    
    if (result.success) {
      return { success: true, payment: result.data };
    } else {
      return { success: false, error: result.message };
    }
  } catch (error) {
    console.error('Error fetching payment:', error);
    return { success: false, error: 'Failed to fetch payment details' };
  }
};

// Step 5: Get user's all bookings
const getUserBookings = async () => {
  try {
    const response = await fetch(`${API_BASE_URL}/booking`, {
      headers: {
        'Authorization': `Bearer ${getAuthToken()}`
      }
    });

    const result = await response.json();
    
    if (result.success) {
      return { success: true, bookings: result.data };
    } else {
      return { success: false, bookings: [], message: result.message };
    }
  } catch (error) {
    console.error('Error fetching bookings:', error);
    return { success: false, bookings: [], error: 'Failed to fetch bookings' };
  }
};

// Step 6: Get user's all payments
const getUserPayments = async () => {
  try {
    const response = await fetch(`${API_BASE_URL}/payment`, {
      headers: {
        'Authorization': `Bearer ${getAuthToken()}`
      }
    });

    const result = await response.json();
    
    if (result.success) {
      return { success: true, payments: result.data };
    } else {
      return { success: false, payments: [], message: result.message };
    }
  } catch (error) {
    console.error('Error fetching payments:', error);
    return { success: false, payments: [], error: 'Failed to fetch payments' };
  }
};

// Step 7: Request refund
const requestRefund = async (bookingId) => {
  try {
    const response = await fetch(`${API_BASE_URL}/payment/${bookingId}/refund`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${getAuthToken()}`
      }
    });

    const result = await response.json();
    
    if (result.success) {
      return { success: true, message: result.message };
    } else {
      return { success: false, errors: result.errors };
    }
  } catch (error) {
    console.error('Refund error:', error);
    return { success: false, error: 'Failed to process refund' };
  }
};

// Complete booking workflow
const completeBooking = async (eventId, categoryName, ticketsNum, totalPrice, userDetails) => {
  // Step 1: Create checkout session
  const checkoutData = {
    firstName: userDetails.firstName,
    lastName: userDetails.lastName,
    emailAddress: userDetails.email,
    phoneNumber: userDetails.phone,
    ticketsNum: ticketsNum,
    totalPrice: totalPrice,
    categoryName: categoryName,
    eventId: eventId,
    currency: 'usd'
  };

  const checkoutResult = await initiateBooking(checkoutData);
  
  if (!checkoutResult.success) {
    return { success: false, errors: checkoutResult.errors };
  }

  // Step 2: Process payment with Stripe
  const paymentResult = await processPayment(
    checkoutResult.clientSecret, 
    stripe, 
    elements
  );

  if (!paymentResult.success) {
    return { success: false, error: paymentResult.error };
  }

  // Step 3: Get final booking details
  const bookingDetails = await getBookingDetails(checkoutResult.bookingId);
  
  // Step 4: Get payment details
  const paymentDetails = await getPaymentDetails(checkoutResult.bookingId);
  
  return {
    success: true,
    booking: bookingDetails.booking,
    payment: paymentDetails.payment,
    paymentIntent: paymentResult.paymentIntent
  };
};
### **React Hook Example for Booking and Payment Management**
import { useState, useEffect } from 'react';

export const useBookingAndPayment = () => {
  const [bookings, setBookings] = useState([]);
  const [payments, setPayments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchBookings = async () => {
    const result = await getUserBookings();
    if (result.success) {
      setBookings(result.bookings);
    } else {
      setError(result.error || result.message);
    }
  };

  const fetchPayments = async () => {
    const result = await getUserPayments();
    if (result.success) {
      setPayments(result.payments);
    } else {
      setError(result.error || result.message);
    }
  };

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    
    await Promise.all([fetchBookings(), fetchPayments()]);
    
    setLoading(false);
  };

  const createBooking = async (bookingData) => {
    setLoading(true);
    setError(null);
    
    const result = await completeBooking(
      bookingData.eventId,
      bookingData.categoryName,
      bookingData.ticketsNum,
      bookingData.totalPrice,
      bookingData.userDetails
    );
    
    if (result.success) {
      await fetchData(); // Refresh both bookings and payments
      return result;
    } else {
      setError(result.error || result.errors?.join(', '));
      return result;
    }
    
    setLoading(false);
  };

  const refundBooking = async (bookingId) => {
    setLoading(true);
    setError(null);
    
    const result = await requestRefund(bookingId);
    
    if (result.success) {
      await fetchData(); // Refresh data
    } else {
      setError(result.error || result.errors?.join(', '));
    }
    
    setLoading(false);
    return result;
  };

  const getBookingWithPayment = (bookingId) => {
    const booking = bookings.find(b => b.id === bookingId);
    const payment = payments.find(p => p.bookingId === bookingId);
    return { booking, payment };
  };

  useEffect(() => {
    fetchData();
  }, []);

  return {
    bookings,
    payments,
    loading,
    error,
    createBooking,
    refundBooking,
    getBookingWithPayment,
    refreshData: fetchData
  };
};
### **Usage Example in React Component**
import React, { useState } from 'react';
import { useBookingAndPayment } from './hooks/useBookingAndPayment';

const BookingComponent = () => {
  const {
    bookings,
    payments,
    loading,
    error,
    createBooking,
    refundBooking,
    getBookingWithPayment
  } = useBookingAndPayment();

  const [selectedBooking, setSelectedBooking] = useState(null);

  const handleRefund = async (bookingId) => {
    const confirmed = window.confirm('Are you sure you want to request a refund?');
    if (confirmed) {
      const result = await refundBooking(bookingId);
      if (result.success) {
        alert('Refund processed successfully!');
      } else {
        alert('Failed to process refund: ' + result.error);
      }
    }
  };

  const getStatusText = (status) => {
    const statusMap = {
      0: 'Pending',
      1: 'Confirmed',
      2: 'Cancelled',
      3: 'Refunded'
    };
    return statusMap[status] || 'Unknown';
  };

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div className="booking-management">
      <h2>My Bookings</h2>
      
      {bookings.length === 0 ? (
        <p>No bookings found.</p>
      ) : (
        <div className="bookings-list">
          {bookings.map(booking => {
            const { payment } = getBookingWithPayment(booking.id);
            
            return (
              <div key={booking.id} className="booking-card">
                <h3>Booking #{booking.id}</h3>
                <p><strong>Status:</strong> {getStatusText(booking.status)}</p>
                <p><strong>Tickets:</strong> {booking.ticketsNum}</p>
                <p><strong>Created:</strong> {new Date(booking.createdDate).toLocaleDateString()}</p>
                
                {payment && (
                  <div className="payment-info">
                    <p><strong>Total Price:</strong> ${payment.totalPrice}</p>
                    <p><strong>Payment Status:</strong> {payment.status}</p>
                    <p><strong>Payment Method:</strong> {payment.paymentMethod}</p>
                    
                    {payment.status === 'Paid' && booking.status === 1 && (
                      <button 
                        onClick={() => handleRefund(booking.id)}
                        className="refund-btn"
                      >
                        Request Refund
                      </button>
                    )}
                  </div>
                )}
                
                <button onClick={() => setSelectedBooking(booking)}>
                  View Details
                </button>
              </div>
            );
          })}
        </div>
      )}
      
      {selectedBooking && (
        <div className="booking-details-modal">
          {/* Detailed booking view */}
        </div>
      )}
    </div>
  );
};

export default BookingComponent;
---

## **Key Points for Frontend Integration**

### **Authentication:**
- All endpoints require JWT authentication
- Include `Authorization: Bearer {token}` header in requests

### **Error Handling:**
- Always check the `success` field in responses
- Display appropriate error messages from `errors` array
- Handle network errors gracefully

### **Stripe Integration:**
- Use the `clientSecret` from checkout response
- Implement Stripe Elements for secure payment processing
- Handle payment confirmation on frontend

### **Status Management:**
- Use enum values for booking and payment status
- Display appropriate status indicators to users
- Allow actions based on current status (e.g., refund only for paid bookings)

### **Real-time Updates:**
- Consider implementing webhooks for payment status updates
- Refresh booking and payment data after successful payments
- Show loading states during API calls

### **Data Relationships:**
- Each booking has a corresponding payment
- Use bookingId to link bookings and payments
- Payment details include booking information for convenience

This comprehensive schema provides everything needed for a complete booking and payment system integration with enhanced payment details!