# Available Tickets Count API - JSON Schema

## **Endpoint:** `GET /api/tickets/available`

### **Query Parameters:**
- `eventId` (required): The ID of the event
- `categoryName` (required): The name of the category

### **Authorization:** Not required (public endpoint)

---

## **Request Examples**

### **URL Format:**
```
GET /api/tickets/available?eventId=1&categoryName=Premium
```

### **JavaScript Example:**
```javascript
const getAvailableTickets = async (eventId, categoryName) => {
  try {
    const params = new URLSearchParams({
      eventId: eventId,
      categoryName: categoryName
    });

    const response = await fetch(`/api/tickets/available?${params}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    });

    const result = await response.json();
    
    if (result.success) {
      console.log(`Available tickets: ${result.data.availableTickets}`);
      return result.data.availableTickets;
    } else {
      console.error('Error:', result.message);
      return null;
    }
  } catch (error) {
    console.error('Network error:', error);
    return null;
  }
};

// Usage
const availableTickets = await getAvailableTickets(1, 'Premium');
```

---

## **Response Schemas**

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Available tickets count retrieved successfully",
  "data": {
    "eventId": 1,
    "categoryName": "Premium",
    "availableTickets": 45
  },
  "errors": []
}
```

### **Validation Error - Missing Event ID (400 Bad Request):**
```json
{
  "success": false,
  "message": "Invalid event ID",
  "data": null,
  "errors": ["Event ID must be a positive number"]
}
```

### **Validation Error - Missing Category Name (400 Bad Request):**
```json
{
  "success": false,
  "message": "Invalid category name",
  "data": null,
  "errors": ["Category name is required"]
}
```

### **Server Error Response (500 Internal Server Error):**
```json
{
  "success": false,
  "message": "An error occurred while retrieving available tickets count",
  "data": null,
  "errors": ["Database connection failed"]
}
```

---

## **Frontend Integration Examples**

### **React Component Example:**
```javascript
import React, { useState, useEffect } from 'react';

const TicketAvailability = ({ eventId, categoryName }) => {
  const [availableTickets, setAvailableTickets] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchAvailableTickets = async () => {
      setLoading(true);
      setError(null);

      try {
        const params = new URLSearchParams({
          eventId: eventId,
          categoryName: categoryName
        });

        const response = await fetch(`/api/tickets/available?${params}`);
        const result = await response.json();

        if (result.success) {
          setAvailableTickets(result.data.availableTickets);
        } else {
          setError(result.message);
        }
      } catch (err) {
        setError('Failed to fetch available tickets');
      } finally {
        setLoading(false);
      }
    };

    if (eventId && categoryName) {
      fetchAvailableTickets();
    }
  }, [eventId, categoryName]);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div className="ticket-availability">
      <h3>{categoryName} Category</h3>
      <p>
        <strong>Available Tickets:</strong> {availableTickets}
      </p>
      {availableTickets === 0 && (
        <p className="sold-out">SOLD OUT</p>
      )}
      {availableTickets > 0 && availableTickets <= 10 && (
        <p className="low-availability">Only {availableTickets} tickets left!</p>
      )}
    </div>
  );
};

export default TicketAvailability;
```

### **Vanilla JavaScript Example:**
```javascript
// Function to update UI with available tickets
async function updateTicketAvailability(eventId, categoryName) {
  const params = new URLSearchParams({
    eventId: eventId,
    categoryName: categoryName
  });

  try {
    const response = await fetch(`/api/tickets/available?${params}`);
    const result = await response.json();

    if (result.success) {
      const count = result.data.availableTickets;
      
      // Update UI
      const ticketCountElement = document.getElementById('ticket-count');
      ticketCountElement.textContent = count;

      // Show warning if low availability
      if (count <= 10 && count > 0) {
        showLowAvailabilityWarning(count);
      } else if (count === 0) {
        showSoldOutMessage();
      }

      return count;
    } else {
      console.error('Error:', result.message);
      showErrorMessage(result.message);
      return null;
    }
  } catch (error) {
    console.error('Network error:', error);
    showErrorMessage('Failed to fetch ticket availability');
    return null;
  }
}

function showLowAvailabilityWarning(count) {
  const warningElement = document.getElementById('availability-warning');
  warningElement.textContent = `Only ${count} tickets left!`;
  warningElement.classList.add('warning');
  warningElement.style.display = 'block';
}

function showSoldOutMessage() {
  const soldOutElement = document.getElementById('sold-out-message');
  soldOutElement.textContent = 'SOLD OUT';
  soldOutElement.classList.add('sold-out');
  soldOutElement.style.display = 'block';

  // Disable booking button
  const bookButton = document.getElementById('book-button');
  bookButton.disabled = true;
}

function showErrorMessage(message) {
  const errorElement = document.getElementById('error-message');
  errorElement.textContent = message;
  errorElement.style.display = 'block';
}
```

### **React Hook for Multiple Categories:**
```javascript
import { useState, useEffect } from 'react';

export const useTicketAvailability = (eventId, categories) => {
  const [availability, setAvailability] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchAllAvailability = async () => {
      setLoading(true);
      setError(null);

      try {
        const availabilityData = {};

        // Fetch availability for all categories
        await Promise.all(
          categories.map(async (category) => {
            const params = new URLSearchParams({
              eventId: eventId,
              categoryName: category.name
            });

            const response = await fetch(`/api/tickets/available?${params}`);
            const result = await response.json();

            if (result.success) {
              availabilityData[category.name] = result.data.availableTickets;
            }
          })
        );

        setAvailability(availabilityData);
      } catch (err) {
        setError('Failed to fetch ticket availability');
      } finally {
        setLoading(false);
      }
    };

    if (eventId && categories && categories.length > 0) {
      fetchAllAvailability();
    }
  }, [eventId, categories]);

  return { availability, loading, error };
};

// Usage in component
const EventCategories = ({ eventId }) => {
  const categories = [
    { name: 'VIP', price: 500 },
    { name: 'Premium', price: 300 },
    { name: 'Regular', price: 150 }
  ];

  const { availability, loading, error } = useTicketAvailability(eventId, categories);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div className="event-categories">
      {categories.map(category => (
        <div key={category.name} className="category-card">
          <h3>{category.name}</h3>
          <p>Price: ${category.price}</p>
          <p>Available: {availability[category.name] || 0} tickets</p>
          <button 
            disabled={!availability[category.name] || availability[category.name] === 0}
          >
            {availability[category.name] === 0 ? 'Sold Out' : 'Book Now'}
          </button>
        </div>
      ))}
    </div>
  );
};
```

### **Real-time Availability Check:**
```javascript
// Check availability before allowing user to proceed with booking
const validateBookingAvailability = async (eventId, categoryName, requestedTickets) => {
  const params = new URLSearchParams({
    eventId: eventId,
    categoryName: categoryName
  });

  try {
    const response = await fetch(`/api/tickets/available?${params}`);
    const result = await response.json();

    if (result.success) {
      const availableTickets = result.data.availableTickets;

      if (availableTickets === 0) {
        return {
          canProceed: false,
          message: 'Sorry, this category is sold out.'
        };
      }

      if (requestedTickets > availableTickets) {
        return {
          canProceed: false,
          message: `Only ${availableTickets} tickets available. Please adjust your selection.`
        };
      }

      return {
        canProceed: true,
        message: 'Tickets available',
        availableTickets: availableTickets
      };
    } else {
      return {
        canProceed: false,
        message: result.message
      };
    }
  } catch (error) {
    return {
      canProceed: false,
      message: 'Failed to check ticket availability'
    };
  }
};

// Usage in booking flow
const handleBookingSubmit = async (bookingData) => {
  // Check availability before proceeding
  const availabilityCheck = await validateBookingAvailability(
    bookingData.eventId,
    bookingData.categoryName,
    bookingData.ticketsNum
  );

  if (!availabilityCheck.canProceed) {
    alert(availabilityCheck.message);
    return;
  }

  // Proceed with booking
  await createBooking(bookingData);
};
```

---

## **Use Cases**

### **1. Event Details Page:**
Display available tickets for each category to help users make informed decisions.

### **2. Booking Form:**
Validate ticket availability before allowing users to proceed with checkout.

### **3. Real-time Updates:**
Poll the endpoint periodically to show live ticket availability.

### **4. Low Availability Warnings:**
Show urgency messages when tickets are running low.

### **5. Sold Out Indicators:**
Disable booking options for categories that are sold out.

---

## **Query Parameters Validation**

| Parameter | Type | Required | Validation | Description |
|-----------|------|----------|------------|-------------|
| `eventId` | integer | Yes | > 0 | The ID of the event |
| `categoryName` | string | Yes | Not empty | The name of the category (e.g., "VIP", "Premium", "Regular") |

---

## **Response Data Structure**

```typescript
interface AvailableTicketsResponse {
  success: boolean;
  message: string;
  data: {
    eventId: number;
    categoryName: string;
    availableTickets: number;
  } | null;
  errors: string[];
}
```

This endpoint provides real-time ticket availability information that's essential for the booking flow!