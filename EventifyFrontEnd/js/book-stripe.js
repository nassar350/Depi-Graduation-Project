// Booking Page with Stripe Payment Integration
console.log('🚀 Booking with Stripe script loaded');

const API_BASE_URL = window.API_BASE_URL || 'https://localhost:7119/api';

// Initialize Stripe (you'll need to replace with your publishable key)
const STRIPE_PUBLISHABLE_KEY = 'pk_test_51QRZgFRsYKUqwGhfO1Sh36qglrV3VmXr5aHkXFrPfWU28uVW4CjsqDfZdKzrpX1Pd00ajKUJ'; // Replace with your actual key
const stripe = Stripe(STRIPE_PUBLISHABLE_KEY);
const elements = stripe.elements();

// Create card element
const cardElement = elements.create('card', {
  style: {
    base: {
      fontSize: '16px',
      color: '#32325d',
      fontFamily: '"Inter", sans-serif',
      '::placeholder': {
        color: '#aab7c4',
      },
    },
    invalid: {
      color: '#fa755a',
      iconColor: '#fa755a',
    },
  },
});

class BookingPage {
  constructor() {
    this.eventId = null;
    this.categoryId = null;
    this.event = null;
    this.category = null;
    this.promoDiscount = 0;
    this.cardMounted = false;
    
    this.init();
  }

  init() {
    console.log('🚀 BookingPage initialized with Stripe');
    
    // Check authentication first
    const token = localStorage.getItem('token');
    if (!token) {
      console.log('🚀 User not logged in');
      sessionStorage.setItem('redirectAfterLogin', window.location.href);
      window.location.href = 'login.html';
      return;
    }

    this.getEventId();
    this.loadBookingData();
    this.setupFormHandlers();
  }

  getEventId() {
    const params = new URLSearchParams(window.location.search);
    this.eventId = params.get('eventId') ? parseInt(params.get('eventId')) : null;
    this.categoryId = params.get('categoryId') ? parseInt(params.get('categoryId')) : null;
    
    console.log('🚀 Event ID:', this.eventId, 'Category ID:', this.categoryId);
    
    if (!this.eventId || !this.categoryId) {
      this.showError('Invalid booking link. Missing event or category information.');
      return;
    }
  }

  async loadBookingData() {
    try {
      this.showLoading(true);
      
      const token = localStorage.getItem('token');
      
      // Load event details
      const eventResponse = await fetch(`${API_BASE_URL}/Events/${this.eventId}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      const eventData = await eventResponse.json();
      console.log('🚀 Event data:', eventData);
      
      if (!eventData.success || !eventData.data) {
        this.showError('Event not found');
        return;
      }
      
      this.event = eventData.data;
      
      // Load category details
      const categoryResponse = await fetch(`${API_BASE_URL}/Categories/${this.categoryId}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      const categoryData = await categoryResponse.json();
      console.log('🚀 Category data:', categoryData);
      
      if (!categoryData.success || !categoryData.data) {
        this.showError('Category not found');
        return;
      }
      
      this.category = categoryData.data;
      
      // Load available tickets for this category
      await this.loadAvailableTickets();
      
      // Check if tickets are available
      const availableCount = this.category.availableTickets?.length || 0;
      if (availableCount <= 0) {
        this.showError('Sorry, this category is sold out');
        return;
      }
      
      // Update page title
      document.title = `Book ${this.event.name} - Eventify`;
      
      // Display booking interface
      this.displayEventSummary();
      this.setupTicketQuantityOptions();
      this.prefillUserData();
      this.updateOrderSummary();
      
      // Mount Stripe card element
      this.mountCardElement();
      
      this.showLoading(false);
      document.getElementById('bookingContent').style.display = 'block';
      
    } catch (error) {
      console.error('🚀 Error loading booking data:', error);
      this.showError('Error loading booking information. Please try again.');
    }
  }

  async loadAvailableTickets() {
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`${API_BASE_URL}/Tickets/category/${this.categoryId}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      const data = await response.json();
      console.log('🚀 Tickets data:', data);

      if (data.success && data.data) {
        this.category.availableTickets = data.data.filter(ticket => 
          ticket.status === 0 || ticket.status === 'Available'
        );
        this.category.totalTickets = data.data.length;
        
        console.log('🚀 Available tickets:', this.category.availableTickets.length);
      } else {
        this.category.availableTickets = [];
        this.category.totalTickets = 0;
      }
    } catch (error) {
      console.error('🚀 Error loading tickets:', error);
      this.category.availableTickets = [];
    }
  }

  mountCardElement() {
    const cardElementContainer = document.getElementById('card-element');
    if (cardElementContainer && !this.cardMounted) {
      cardElement.mount('#card-element');
      this.cardMounted = true;
      
      // Handle real-time validation errors from the card Element
      cardElement.on('change', (event) => {
        const displayError = document.getElementById('card-errors');
        if (event.error) {
          displayError.textContent = event.error.message;
        } else {
          displayError.textContent = '';
        }
      });
      
      console.log('🚀 Stripe card element mounted');
    }
  }

  showLoading(show) {
    const loading = document.getElementById('bookingLoading');
    const content = document.getElementById('bookingContent');
    
    if (loading) loading.style.display = show ? 'block' : 'none';
    if (content) content.style.display = show ? 'none' : 'block';
  }

  showError(message) {
    this.showLoading(false);
    const errorDiv = document.getElementById('bookingError');
    const errorMessage = document.getElementById('errorMessage');
    
    if (errorDiv) errorDiv.style.display = 'block';
    if (errorMessage) errorMessage.textContent = message;
  }

  setupTicketQuantityOptions() {
    const select = document.getElementById('ticketQuantity');
    if (!select || !this.category) return;

    const availableTickets = this.category.availableTickets?.length || 0;
    const maxTickets = Math.min(availableTickets, 10);

    select.innerHTML = '<option value="">Select quantity</option>';
    
    for (let i = 1; i <= maxTickets; i++) {
      const option = document.createElement('option');
      option.value = i;
      option.textContent = `${i} ticket${i > 1 ? 's' : ''}`;
      select.appendChild(option);
    }

    select.addEventListener('change', () => {
      this.updateOrderSummary();
    });
  }

  displayEventSummary() {
    if (!this.event || !this.category) return;

    const container = document.getElementById('eventSummary');
    if (!container) return;

    const eventDate = new Date(this.event.startDate);
    const formattedDate = eventDate.toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
    const formattedTime = eventDate.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit'
    });

    let imageUrl = 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=160&h=160&fit=crop';
    if (this.event.photoBase64) {
      imageUrl = `data:image/jpeg;base64,${this.event.photoBase64}`;
    }

    container.innerHTML = `
      <div class="flex gap-md" style="margin-bottom: var(--space-md);">
        <img 
          src="${imageUrl}" 
          alt="${this.event.name}"
          style="width: 80px; height: 80px; object-fit: cover; border-radius: var(--radius-md);"
          onerror="this.src='https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=160&h=160&fit=crop'"
        >
        <div style="flex: 1;">
          <h4 style="margin: 0 0 var(--space-xs) 0;">${this.event.name}</h4>
          <p style="margin: 0; font-size: var(--font-size-sm); color: var(--muted);">
            ${formattedDate}<br>
            ${formattedTime}<br>
            ${this.event.address}
          </p>
        </div>
      </div>
      <div style="padding: var(--space-md); background: #f8f9fa; border-radius: var(--radius-md);">
        <p style="margin: 0; font-weight: 600;">${this.category.name}</p>
        <p style="margin: var(--space-xs) 0 0 0; color: var(--muted);">
          ${this.category.availableTickets?.length || 0} tickets available
        </p>
      </div>
    `;
  }

  updateOrderSummary() {
    if (!this.category) return;

    const ticketQuantity = parseInt(document.getElementById('ticketQuantity')?.value || 0);
    const ticketPrice = this.category.price || 0;
    const subtotal = ticketPrice * ticketQuantity;
    const discount = subtotal * (this.promoDiscount / 100);
    const total = subtotal - discount;

    const ticketCountEl = document.getElementById('ticketCount');
    const ticketPriceEl = document.getElementById('ticketPrice');
    const totalElement = document.getElementById('totalPrice');
    
    if (ticketCountEl) ticketCountEl.textContent = ticketQuantity || '-';
    if (ticketPriceEl) ticketPriceEl.textContent = ticketPrice === 0 ? 'Free' : `${ticketPrice} EGP`;
    
    if (totalElement) {
      if (ticketQuantity > 0) {
        totalElement.textContent = total === 0 ? 'Free' : `${total.toFixed(2)} EGP`;
      } else {
        totalElement.textContent = '-';
      }
    }

    const promoRow = document.getElementById('promoDiscountRow');
    const promoDiscount = document.getElementById('promoDiscount');
    
    if (this.promoDiscount > 0 && ticketQuantity > 0) {
      if (promoRow) promoRow.style.display = 'flex';
      if (promoDiscount) promoDiscount.textContent = `-${discount.toFixed(2)} EGP (${this.promoDiscount}% off)`;
    } else {
      if (promoRow) promoRow.style.display = 'none';
    }

    this.formData = {
      ticketQuantity,
      subtotal,
      discount,
      total
    };
  }

  setupFormHandlers() {
    const form = document.getElementById('bookingForm');
    if (!form) return;

    form.addEventListener('submit', async (e) => {
      e.preventDefault();
      await this.processBookingWithPayment();
    });
  }

  prefillUserData() {
    const userName = localStorage.getItem('userName') || '';
    const userEmail = localStorage.getItem('userEmail') || '';

    const nameParts = userName ? userName.split(' ') : [];
    
    const firstNameInput = document.getElementById('firstName');
    const lastNameInput = document.getElementById('lastName');
    const emailInput = document.getElementById('email');

    if (firstNameInput && nameParts.length > 0) {
      firstNameInput.value = nameParts[0];
    }
    
    if (lastNameInput && nameParts.length > 1) {
      lastNameInput.value = nameParts.slice(1).join(' ');
    }
    
    if (emailInput && userEmail) {
      emailInput.value = userEmail;
    }
  }

  async processBookingWithPayment() {
    const submitBtn = document.getElementById('confirmBookingBtn');
    const buttonText = document.getElementById('button-text');
    const spinner = document.getElementById('spinner');
    const errorDiv = document.getElementById('bookingError');
    
    if (submitBtn) submitBtn.disabled = true;
    if (buttonText) buttonText.style.display = 'none';
    if (spinner) spinner.style.display = 'inline';
    if (errorDiv) errorDiv.style.display = 'none';

    try {
      // Validate form
      const firstName = document.getElementById('firstName')?.value.trim();
      const lastName = document.getElementById('lastName')?.value.trim();
      const email = document.getElementById('email')?.value.trim();
      const phone = document.getElementById('phone')?.value.trim();
      const ticketQuantity = parseInt(document.getElementById('ticketQuantity')?.value || 0);
      const promoCode = document.getElementById('promoCode')?.value?.trim() || null;

      if (!firstName || !lastName || !email || ticketQuantity <= 0) {
        throw new Error('Please fill in all required fields');
      }

      const availableCount = this.category.availableTickets?.length || 0;
      if (ticketQuantity > availableCount) {
        throw new Error('Not enough tickets available');
      }

      const token = localStorage.getItem('token');
      const userId = localStorage.getItem('userId');

      if (!token || !userId) {
        throw new Error('Please login to continue');
      }

      const totalPrice = this.formData.total;

      // Step 1: Create checkout session
      console.log('🚀 Step 1: Creating checkout session...');
      
      const checkoutData = {
        firstName: firstName,
        lastName: lastName,
        emailAddress: email,
        phoneNumber: phone || '',
        ticketsNum: ticketQuantity,
        totalPrice: totalPrice,
        categoryName: this.category.name,
        promoCode: promoCode,
        eventId: this.eventId,
        userId: parseInt(userId),
        currency: 'usd',
        ticketStatus: 0 // Pending
      };

      console.log('🚀 Checkout data:', checkoutData);

      const checkoutResponse = await fetch(`${API_BASE_URL}/checkout`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(checkoutData)
      });

      const checkoutResult = await checkoutResponse.json();
      console.log('🚀 Checkout response:', checkoutResult);

      if (!checkoutResponse.ok || !checkoutResult.success) {
        const errorMsg = checkoutResult.errors?.join(', ') || checkoutResult.message || 'Failed to create checkout session';
        throw new Error(errorMsg);
      }

      const { bookingId, paymentId, clientSecret } = checkoutResult.data;
      console.log('🚀 Checkout session created. Booking ID:', bookingId);

      // Step 2: Confirm payment with Stripe
      console.log('🚀 Step 2: Confirming payment with Stripe...');
      
      const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
        payment_method: {
          card: cardElement,
          billing_details: {
            name: `${firstName} ${lastName}`,
            email: email,
            phone: phone
          }
        }
      });

      if (error) {
        console.error('🚀 Payment error:', error);
        throw new Error(error.message || 'Payment failed');
      }

      if (paymentIntent.status === 'succeeded') {
        console.log('🚀 Payment succeeded!', paymentIntent);
        
        // Show success message
        alert(`Booking confirmed! Your booking ID is: ${bookingId}\n\nPayment successful!\n\nThank you for booking with Eventify!`);
        
        // Redirect to event page or dashboard
        window.location.href = `event.html?id=${this.eventId}`;
      } else {
        throw new Error('Payment was not completed');
      }

    } catch (error) {
      console.error('🚀 Booking error:', error);
      if (errorDiv) {
        errorDiv.textContent = error.message || 'Failed to process booking. Please try again.';
        errorDiv.style.display = 'block';
      }
      alert(error.message || 'Failed to process booking. Please try again.');
    } finally {
      if (submitBtn) submitBtn.disabled = false;
      if (buttonText) buttonText.style.display = 'inline';
      if (spinner) spinner.style.display = 'none';
    }
  }

  applyPromoCode() {
    const promoInput = document.getElementById('promoCode');
    const promoCode = promoInput?.value.trim().toUpperCase();

    if (!promoCode) {
      alert('Please enter a promo code');
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
      alert(`Promo code applied! ${this.promoDiscount}% discount`);
      
      if (promoInput) promoInput.disabled = true;
      const applyBtn = document.getElementById('applyPromoBtn');
      if (applyBtn) {
        applyBtn.textContent = 'Applied';
        applyBtn.disabled = true;
      }
    } else {
      alert('Invalid promo code');
    }
  }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
  console.log('🚀 Initializing booking page with Stripe');
  window.bookingPage = new BookingPage();
});

// Export for use in other scripts
window.BookingPage = BookingPage;
