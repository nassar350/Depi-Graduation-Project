// Booking Page JavaScript
// Handles booking form, payment processing (mock), and confirmation

class BookingPage {
  constructor() {
    this.eventId = null;
    this.event = null;
    this.currentStep = 1;
    this.formData = {};
    this.promoDiscount = 0;
    this.maxTicketsPerBooking = 10;
    
    this.init();
  }

  init() {
    this.getEventId();
    this.loadEvent();
    this.setupFormHandlers();
    this.setupCardFormatting();
    this.prefillUserData();
  }

  // Get event ID from URL parameters
  getEventId() {
    const params = app.getUrlParams();
    this.eventId = params.eventId ? parseInt(params.eventId) : null;
    
    if (!this.eventId) {
      this.showError('No event selected for booking');
      return;
    }
  }

  // Load event data
  async loadEvent() {
    try {
      this.showLoading(true);
      
      // Get event data
      this.event = EventifyData.getEventById(this.eventId);
      
      if (!this.event) {
        this.showError('Event not found');
        return;
      }

      // Check if event is available for booking
      const eventDateTime = new Date(this.event.date + 'T' + this.event.time);
      if (eventDateTime < new Date()) {
        this.showError('This event has already passed and is no longer available for booking');
        return;
      }

      const availableTickets = this.event.ticketsAvailable || 0;
      if (availableTickets <= 0) {
        this.showError('Sorry, this event is sold out');
        return;
      }

      // Update page title
      document.title = `Book ${this.event.title} - Eventify`;
      
      // Setup form
      this.setupTicketQuantityOptions();
      this.displayEventSummary();
      this.updateOrderSummary();
      
      this.showLoading(false);
      document.getElementById('bookingContent').style.display = 'block';
      
    } catch (error) {
      console.error('Error loading event:', error);
      this.showError('Error loading event details');
    }
  }

  // Show/hide loading state
  showLoading(show) {
    const loading = document.getElementById('bookingLoading');
    if (loading) {
      loading.style.display = show ? 'block' : 'none';
    }
  }

  // Show error state
  showError(message) {
    this.showLoading(false);
    const errorDiv = document.getElementById('bookingError');
    const errorMessage = document.getElementById('errorMessage');
    
    if (errorDiv) errorDiv.style.display = 'block';
    if (errorMessage) errorMessage.textContent = message;
  }

  // Setup ticket quantity dropdown
  setupTicketQuantityOptions() {
    const select = document.getElementById('ticketQuantity');
    if (!select || !this.event) return;

    const availableTickets = this.event.ticketsAvailable || 0;
    const maxTickets = Math.min(availableTickets, this.maxTicketsPerBooking);

    select.innerHTML = '<option value="">Select quantity</option>';
    
    for (let i = 1; i <= maxTickets; i++) {
      const option = document.createElement('option');
      option.value = i;
      option.textContent = `${i} ticket${i > 1 ? 's' : ''}`;
      select.appendChild(option);
    }

    // Add change handler
    select.addEventListener('change', () => {
      this.updateOrderSummary();
    });
  }

  // Display event summary in sidebar
  displayEventSummary() {
    if (!this.event) return;

    const container = document.getElementById('eventSummary');
    if (!container) return;

    const formattedDate = EventifyData.formatDate(this.event.date);
    const formattedTime = EventifyData.formatTime(this.event.time);

    container.innerHTML = `
      <div class="flex gap-md" style="margin-bottom: var(--space-md);">
        <img 
          src="${this.event.image}" 
          alt="${this.event.title}"
          style="width: 80px; height: 80px; object-fit: cover; border-radius: var(--radius-md);"
          onerror="this.src='https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=160&h=160&fit=crop'"
        >
        <div style="flex: 1;">
          <h4 style="margin: 0 0 var(--space-xs) 0;">${this.event.title}</h4>
          <p style="margin: 0; font-size: var(--font-size-sm); color: var(--muted);">
            ${formattedDate}<br>
            ${formattedTime}<br>
            ${this.event.location}
          </p>
        </div>
      </div>
    `;
  }

  // Update order summary
  updateOrderSummary() {
    if (!this.event) return;

    const ticketQuantity = parseInt(document.getElementById('ticketQuantity')?.value || 0);
    const ticketPrice = this.event.price;
    const subtotal = ticketPrice * ticketQuantity;
    const discount = subtotal * (this.promoDiscount / 100);
    const total = subtotal - discount;

    // Update display elements
    document.getElementById('ticketCount').textContent = ticketQuantity || '-';
    document.getElementById('ticketPrice').textContent = ticketPrice === 0 ? 'Free' : app.formatCurrency(ticketPrice);
    
    const totalElement = document.getElementById('totalPrice');
    if (ticketQuantity > 0) {
      totalElement.textContent = total === 0 ? 'Free' : app.formatCurrency(total);
    } else {
      totalElement.textContent = '-';
    }

    // Update promo discount row
    const promoRow = document.getElementById('promoDiscountRow');
    const promoDiscount = document.getElementById('promoDiscount');
    
    if (this.promoDiscount > 0 && ticketQuantity > 0) {
      promoRow.style.display = 'flex';
      promoDiscount.textContent = `-${app.formatCurrency(discount)} (${this.promoDiscount}% off)`;
    } else {
      promoRow.style.display = 'none';
    }

    // Store for later use
    this.formData.ticketQuantity = ticketQuantity;
    this.formData.subtotal = subtotal;
    this.formData.discount = discount;
    this.formData.total = total;
  }

  // Setup form event handlers
  setupFormHandlers() {
    const form = document.getElementById('bookingForm');
    if (!form) return;

    // Real-time validation is already handled by app.js
    // Add specific handlers for booking form

    // Auto-fill name on card from attendee name
    const firstNameInput = document.getElementById('firstName');
    const lastNameInput = document.getElementById('lastName');
    const cardNameInput = document.getElementById('cardName');

    [firstNameInput, lastNameInput].forEach(input => {
      if (input) {
        input.addEventListener('input', () => {
          const firstName = firstNameInput.value.trim();
          const lastName = lastNameInput.value.trim();
          if (cardNameInput && firstName && lastName) {
            cardNameInput.value = `${firstName} ${lastName}`;
          }
        });
      }
    });
  }

  // Setup credit card input formatting
  setupCardFormatting() {
    const cardNumberInput = document.getElementById('cardNumber');
    const cardExpiryInput = document.getElementById('cardExpiry');
    const cardCvcInput = document.getElementById('cardCvc');

    // Format card number (add spaces every 4 digits)
    if (cardNumberInput) {
      cardNumberInput.addEventListener('input', (e) => {
        let value = e.target.value.replace(/\D/g, '');
        value = value.replace(/(\d{4})(?=\d)/g, '$1 ');
        e.target.value = value;
      });
    }

    // Format expiry date (MM/YY)
    if (cardExpiryInput) {
      cardExpiryInput.addEventListener('input', (e) => {
        let value = e.target.value.replace(/\D/g, '');
        if (value.length >= 2) {
          value = value.substring(0, 2) + '/' + value.substring(2, 4);
        }
        e.target.value = value;
      });
    }

    // Format CVC (numbers only)
    if (cardCvcInput) {
      cardCvcInput.addEventListener('input', (e) => {
        e.target.value = e.target.value.replace(/\D/g, '');
      });
    }
  }

  // Prefill user data if logged in
  prefillUserData() {
    const user = app.getCurrentUser();
    if (!user) return;

    // Split name if available
    const nameParts = user.name ? user.name.split(' ') : [];
    
    const firstNameInput = document.getElementById('firstName');
    const lastNameInput = document.getElementById('lastName');
    const emailInput = document.getElementById('email');

    if (firstNameInput && nameParts.length > 0) {
      firstNameInput.value = nameParts[0];
    }
    
    if (lastNameInput && nameParts.length > 1) {
      lastNameInput.value = nameParts.slice(1).join(' ');
    }
    
    if (emailInput && user.email) {
      emailInput.value = user.email;
    }
  }

  // Navigate to specific step
  goToStep(step) {
    if (step < 1 || step > 3) return;

    // Validate current step before proceeding
    if (step > this.currentStep && !this.validateCurrentStep()) {
      return;
    }

    // Update progress indicators
    document.querySelectorAll('.step').forEach(stepEl => {
      const stepNum = parseInt(stepEl.dataset.step);
      stepEl.classList.toggle('active', stepNum === step);
      stepEl.classList.toggle('completed', stepNum < step);
    });

    // Show/hide step content
    document.querySelectorAll('.step-content').forEach(content => {
      const stepNum = parseInt(content.dataset.step);
      content.style.display = stepNum === step ? 'block' : 'none';
    });

    this.currentStep = step;

    // Update form data when moving to payment step
    if (step === 2) {
      this.collectFormData();
    }
  }

  // Validate current step
  validateCurrentStep() {
    if (this.currentStep === 1) {
      return this.validateBookingDetails();
    } else if (this.currentStep === 2) {
      return this.validatePaymentDetails();
    }
    return true;
  }

  // Validate booking details (step 1)
  validateBookingDetails() {
    const requiredFields = ['firstName', 'lastName', 'email', 'ticketQuantity'];
    let isValid = true;

    requiredFields.forEach(fieldId => {
      const field = document.getElementById(fieldId);
      if (field && !app.validateField(field)) {
        isValid = false;
      }
    });

    // Additional validation for ticket quantity
    const ticketQuantity = parseInt(document.getElementById('ticketQuantity').value || 0);
    if (ticketQuantity <= 0) {
      app.showNotification('Please select the number of tickets', 'warning');
      isValid = false;
    }

    if (ticketQuantity > (this.event.ticketsAvailable || 0)) {
      app.showNotification('Not enough tickets available', 'error');
      isValid = false;
    }

    return isValid;
  }

  // Validate payment details (step 2)
  validatePaymentDetails() {
    const requiredFields = ['cardNumber', 'cardName', 'cardExpiry', 'cardCvc'];
    let isValid = true;

    requiredFields.forEach(fieldId => {
      const field = document.getElementById(fieldId);
      if (field && !app.validateField(field)) {
        isValid = false;
      }
    });

    // Additional card validation
    const cardNumber = document.getElementById('cardNumber').value.replace(/\s/g, '');
    if (cardNumber.length < 13 || cardNumber.length > 19) {
      app.showNotification('Please enter a valid card number', 'error');
      isValid = false;
    }

    const cardExpiry = document.getElementById('cardExpiry').value;
    if (!/^\d{2}\/\d{2}$/.test(cardExpiry)) {
      app.showNotification('Please enter a valid expiry date (MM/YY)', 'error');
      isValid = false;
    }

    const cardCvc = document.getElementById('cardCvc').value;
    if (cardCvc.length < 3 || cardCvc.length > 4) {
      app.showNotification('Please enter a valid CVC', 'error');
      isValid = false;
    }

    return isValid;
  }

  // Collect form data
  collectFormData() {
    const formElements = document.getElementById('bookingForm').elements;
    
    for (let element of formElements) {
      if (element.name) {
        this.formData[element.name] = element.value;
      }
    }
  }

  // Apply promo code
  applyPromoCode() {
    const promoInput = document.getElementById('promoCode');
    const promoCode = promoInput.value.trim().toUpperCase();

    if (!promoCode) {
      app.showNotification('Please enter a promo code', 'warning');
      return;
    }

    // Mock promo codes
    const validPromoCodes = {
      'SAVE10': 10,
      'WELCOME': 15,
      'EARLYBIRD': 20,
      'STUDENT': 25
    };

    if (validPromoCodes[promoCode]) {
      this.promoDiscount = validPromoCodes[promoCode];
      this.updateOrderSummary();
      app.showNotification(`Promo code applied! ${this.promoDiscount}% discount`, 'success');
      
      // Disable promo input
      promoInput.disabled = true;
      document.getElementById('applyPromoBtn').textContent = 'Applied';
      document.getElementById('applyPromoBtn').disabled = true;
    } else {
      app.showNotification('Invalid promo code', 'error');
    }
  }

  // Process payment (mock)
  async processPayment() {
    if (!this.validatePaymentDetails()) {
      return;
    }

    const processBtn = document.getElementById('processPaymentBtn');
    const originalText = processBtn.textContent;
    
    try {
      // Show loading state
      processBtn.textContent = 'Processing...';
      processBtn.disabled = true;

      // Collect all form data
      this.collectFormData();

      // Simulate payment processing delay
      await new Promise(resolve => setTimeout(resolve, 2000));

      // Mock payment validation (always succeeds in demo)
      const isPaymentSuccessful = true;

      if (isPaymentSuccessful) {
        // Generate booking
        const booking = this.generateBooking();
        
        // Save booking to localStorage
        this.saveBooking(booking);
        
        // Show confirmation
        this.showConfirmation(booking);
      } else {
        throw new Error('Payment failed');
      }

    } catch (error) {
      console.error('Payment error:', error);
      app.showNotification('Payment failed. Please try again.', 'error');
      
      // Reset button
      processBtn.textContent = originalText;
      processBtn.disabled = false;
    }
  }

  // Generate booking object
  generateBooking() {
    const bookingId = EventifyData.generateBookingId();
    
    return {
      id: bookingId,
      eventId: this.event.id,
      eventTitle: this.event.title,
      eventDate: this.event.date,
      eventTime: this.event.time,
      eventLocation: this.event.location,
      attendeeInfo: {
        name: `${this.formData.firstName} ${this.formData.lastName}`,
        email: this.formData.email,
        phone: this.formData.phone || ''
      },
      tickets: parseInt(this.formData.ticketQuantity),
      pricePerTicket: this.event.price,
      subtotal: this.formData.subtotal,
      discount: this.formData.discount,
      total: this.formData.total,
      promoCode: this.formData.promoCode || null,
      bookingDate: new Date().toISOString(),
      paymentStatus: 'paid',
      paymentMethod: 'card',
      cardLast4: this.formData.cardNumber.slice(-4).replace(/\s/g, ''),
      status: 'confirmed'
    };
  }

  // Save booking to localStorage
  saveBooking(booking) {
    // Get existing bookings
    const existingBookings = JSON.parse(localStorage.getItem('eventify_bookings') || '[]');
    
    // Add new booking
    existingBookings.push(booking);
    
    // Save back to localStorage
    localStorage.setItem('eventify_bookings', JSON.stringify(existingBookings));
    
    // Update event ticket availability (mock)
    const events = EventifyData.getAllEvents();
    const eventIndex = events.findIndex(e => e.id === this.event.id);
    if (eventIndex !== -1) {
      events[eventIndex].ticketsAvailable -= booking.tickets;
      
      // Save updated events if it's a custom event
      const customEvents = JSON.parse(localStorage.getItem('customEvents') || '[]');
      const customEventIndex = customEvents.findIndex(e => e.id === this.event.id);
      if (customEventIndex !== -1) {
        customEvents[customEventIndex].ticketsAvailable -= booking.tickets;
        localStorage.setItem('customEvents', JSON.stringify(customEvents));
      }
    }
  }

  // Show booking confirmation
  showConfirmation(booking) {
    const modal = document.getElementById('confirmationModal');
    const details = document.getElementById('confirmationDetails');
    
    if (!modal || !details) return;

    const formattedDate = EventifyData.formatDate(booking.eventDate);
    const formattedTime = EventifyData.formatTime(booking.eventTime);

    details.innerHTML = `
      <div style="text-align: center; margin-bottom: var(--space-lg);">
        <h3>Booking ID: ${booking.id}</h3>
      </div>
      
      <div style="display: grid; gap: var(--space-md);">
        <div>
          <h4 style="margin: 0 0 var(--space-xs) 0; color: var(--muted);">Event</h4>
          <p style="margin: 0; font-weight: 500;">${booking.eventTitle}</p>
        </div>
        
        <div>
          <h4 style="margin: 0 0 var(--space-xs) 0; color: var(--muted);">Date & Time</h4>
          <p style="margin: 0;">${formattedDate} at ${formattedTime}</p>
        </div>
        
        <div>
          <h4 style="margin: 0 0 var(--space-xs) 0; color: var(--muted);">Location</h4>
          <p style="margin: 0;">${booking.eventLocation}</p>
        </div>
        
        <div>
          <h4 style="margin: 0 0 var(--space-xs) 0; color: var(--muted);">Tickets</h4>
          <p style="margin: 0;">${booking.tickets} ticket${booking.tickets > 1 ? 's' : ''}</p>
        </div>
        
        <div>
          <h4 style="margin: 0 0 var(--space-xs) 0; color: var(--muted);">Total Paid</h4>
          <p style="margin: 0; font-weight: 600; font-size: var(--font-size-lg); color: var(--accent);">
            ${booking.total === 0 ? 'Free' : app.formatCurrency(booking.total)}
          </p>
        </div>
        
        <div>
          <h4 style="margin: 0 0 var(--space-xs) 0; color: var(--muted);">Payment Method</h4>
          <p style="margin: 0;">Card ending in ${booking.cardLast4}</p>
        </div>
      </div>
    `;

    // Store booking for modal actions
    this.currentBooking = booking;

    // Show modal
    modal.classList.add('active');
    document.body.style.overflow = 'hidden';
  }

  // Close confirmation modal
  closeConfirmation() {
    const modal = document.getElementById('confirmationModal');
    if (modal) {
      modal.classList.remove('active');
      document.body.style.overflow = '';
    }

    // Redirect to home page
    setTimeout(() => {
      window.location.href = 'index.html';
    }, 500);
  }

  // Download ticket (mock)
  downloadTicket() {
    if (!this.currentBooking) return;

    // Create a simple text "ticket"
    const ticketContent = `
EVENTIFY TICKET
===============

Booking ID: ${this.currentBooking.id}
Event: ${this.currentBooking.eventTitle}
Date: ${EventifyData.formatDate(this.currentBooking.eventDate)}
Time: ${EventifyData.formatTime(this.currentBooking.eventTime)}
Location: ${this.currentBooking.eventLocation}
Attendee: ${this.currentBooking.attendeeInfo.name}
Tickets: ${this.currentBooking.tickets}
Total: ${this.currentBooking.total === 0 ? 'Free' : app.formatCurrency(this.currentBooking.total)}

Thank you for booking with Eventify!
    `;

    // Create and download file
    const blob = new Blob([ticketContent], { type: 'text/plain' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `eventify-ticket-${this.currentBooking.id}.txt`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);

    app.showNotification('Ticket downloaded!', 'success');
  }

  // View dashboard
  viewDashboard() {
    const user = app.getCurrentUser();
    
    if (user) {
      window.location.href = 'dashboard.html';
    } else {
      // If not logged in, suggest login
      app.showNotification('Login to view all your tickets', 'info');
      setTimeout(() => {
        window.location.href = 'login.html?redirect=dashboard.html';
      }, 1000);
    }
  }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
  if (window.EventifyData) {
    window.bookingPage = new BookingPage();
  } else {
    console.error('EventifyData not loaded');
    app.showNotification('Error loading booking system', 'error');
  }
});

// Export for use in other scripts
window.BookingPage = BookingPage;