// Booking Page with Stripe Payment Integration
console.log('🚀 Booking with Stripe script loaded');

// Prefer global API_BASE_URL set by pages; fallback to production default
const API_BASE_URL = window.API_BASE_URL || 'http://eventify.runasp.net/api';

// Initialize Stripe (you'll need to replace with your publishable key)
const STRIPE_PUBLISHABLE_KEY = 'pk_test_51SSOEXK93KknicwiI9yfrki50Y5mUj1rhwwL1brt2VaoVZqfafneC0naPTuTXLufC8Ero4hQpcFNjyh28rfc344V00RDIkQ0ie'; // Replace with your actual key
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

// Helper functions for auth (compatible with multiple storage keys)
function getAuthToken() {
  // Check multiple possible localStorage keys
  return localStorage.getItem('token') 
      || localStorage.getItem('eventify_token') 
      || localStorage.getItem('authToken')
      || null;
}

function getUserId() {
  // Try direct userId keys first
  let userId = localStorage.getItem('userId') || localStorage.getItem('user_id');
  
  if (!userId) {
    // Fallback to parsing from stored user object
    const userJson = localStorage.getItem('eventifyUser');
    if (userJson) {
      try {
        const user = JSON.parse(userJson);
        userId = user.id || user.userId;
      } catch (e) {
        console.error('Error parsing eventifyUser:', e);
      }
    }
  }
  
  return userId;
}

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
    console.log('🚀 Current URL:', window.location.href);
    
    // Check authentication first
    const token = getAuthToken();
    console.log('🚀 Auth token check:', token ? 'Token found' : 'No token');
    
    if (!token) {
      console.log('🚀 User not logged in - redirecting to login');
      sessionStorage.setItem('redirectAfterLogin', window.location.href);
      setTimeout(() => {
        window.location.href = 'login.html';
      }, 1000);
      return;
    }

    this.getEventId();
    if (!this.eventId || !this.categoryId) {
      console.error('🚀 Missing eventId or categoryId - cannot proceed');
      return;
    }
    
    this.loadBookingData();
    this.setupFormHandlers();
  }

  getEventId() {
    const params = new URLSearchParams(window.location.search);
    this.eventId = params.get('eventId') ? parseInt(params.get('eventId')) : null;
    this.categoryId = params.get('categoryId') ? parseInt(params.get('categoryId')) : null;
    this.categoryNameFromUrl = params.get('categoryName') || null;
    
    console.log('🚀 Event ID:', this.eventId, 'Category ID:', this.categoryId, 'Category Name:', this.categoryNameFromUrl);
    
    if (!this.eventId || !this.categoryId) {
      this.showError('Invalid booking link. Missing event or category information.');
      return;
    }
  }

  async loadBookingData() {
    try {
      this.showLoading(true);
      
      const token = getAuthToken();
      console.log('🚀 Loading booking data with token:', token ? 'present' : 'missing');
      console.log('🚀 Event ID:', this.eventId, 'Category ID:', this.categoryId);
      
      // Load event details
      console.log('🚀 Fetching event from:', `${API_BASE_URL}/Events/${this.eventId}`);
      const eventResponse = await fetch(`${API_BASE_URL}/Events/${this.eventId}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      console.log('🚀 Event response status:', eventResponse.status);
      const eventData = await eventResponse.json();
      console.log('🚀 Event data:', eventData);
      
      if (!eventData.success || !eventData.data) {
        this.showError(`Event not found (status: ${eventResponse.status})`);
        return;
      }
      
      this.event = eventData.data;
      
      // Try to find category from event data first (fallback approach)
      let categoryFromEvent = null;
      if (this.event.categories && Array.isArray(this.event.categories)) {
        categoryFromEvent = this.event.categories.find(cat => cat.id === this.categoryId);
        console.log('🚀 Category from event data:', categoryFromEvent);
      }
      
      // Try to load category details from API
      try {
        console.log('🚀 Fetching category from:', `${API_BASE_URL}/Categories/${this.categoryId}`);
        const categoryResponse = await fetch(`${API_BASE_URL}/Categories/${this.categoryId}`, {
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        });
        
        console.log('🚀 Category response status:', categoryResponse.status);
        const categoryData = await categoryResponse.json();
        console.log('🚀 Category data from API:', categoryData);
        
        if (categoryData.success && categoryData.data) {
          this.category = categoryData.data;
        } else if (categoryFromEvent) {
          console.log('🚀 Using category from event data as fallback');
          this.category = categoryFromEvent;
        } else {
          this.showError('Category not found');
          return;
        }
      } catch (categoryError) {
        console.error('🚀 Category API error:', categoryError);
        if (categoryFromEvent) {
          console.log('🚀 Using category from event data as fallback after error');
          this.category = categoryFromEvent;
        } else {
          this.showError('Category not found');
          return;
        }
      }
      
      // Load available tickets for this category (with automatic fallback to category capacity)
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
      console.error('🚀 Error details:', error.message, error.stack);
      this.showError(`Error loading booking information: ${error.message}. Please try again.`);
    }
  }

  async loadAvailableTickets() {
    try {
      const token = getAuthToken();
      console.log('🚀 Fetching tickets from:', `${API_BASE_URL}/Tickets/category/${this.categoryId}`);
      
      const response = await fetch(`${API_BASE_URL}/Tickets/category/${this.categoryId}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      console.log('🚀 Tickets response status:', response.status);

      // If API doesn't exist (404) or fails, use category capacity as fallback
      if (!response.ok) {
        console.log('🚀 Tickets API not available (status:', response.status, ') - using category capacity fallback');
        this.createMockTicketsFromCategory();
        return;
      }

      const data = await response.json();
      console.log('🚀 Tickets data:', data);

      if (data.success && data.data) {
        this.category.availableTickets = data.data.filter(ticket => 
          ticket.status === 0 || ticket.status === 'Available'
        );
        this.category.totalTickets = data.data.length;
        
        console.log('🚀 Available tickets from API:', this.category.availableTickets.length);
      } else {
        console.log('🚀 No valid ticket data - using category capacity fallback');
        this.createMockTicketsFromCategory();
      }
    } catch (error) {
      console.error('🚀 Error loading tickets:', error);
      console.log('🚀 Using category capacity fallback due to error');
      this.createMockTicketsFromCategory();
    }
  }

  createMockTicketsFromCategory() {
    // Calculate available seats from category data
    const seats = this.category.seats || this.category.capacity || 100;
    const booked = this.category.booked || this.category.sold || 0;
    const available = Math.max(0, seats - booked);
    
    console.log('🚀 Creating mock tickets - Total seats:', seats, 'Booked:', booked, 'Available:', available);
    
    // Create mock tickets array
    this.category.availableTickets = [];
    for (let i = 0; i < available; i++) {
      this.category.availableTickets.push({ 
        id: `mock-${i}`, 
        status: 0,
        categoryId: this.categoryId
      });
    }
    
    this.category.totalTickets = seats;
    console.log('🚀 Mock tickets created:', this.category.availableTickets.length);
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
    console.error('🚀 Showing error to user:', message);
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
    const ticketPrice = this.category.ticketPrice || 0;
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
    // Try multiple localStorage keys for name/email
    let userName = localStorage.getItem('userName') || localStorage.getItem('user_name') || '';
    let userEmail = localStorage.getItem('userEmail') || localStorage.getItem('user_email') || '';
    
    // Fallback to stored user object
    if (!userName || !userEmail) {
      const userJson = localStorage.getItem('eventifyUser');
      if (userJson) {
        try {
          const user = JSON.parse(userJson);
          if (!userName && user.name) userName = user.name;
          if (!userEmail && user.email) userEmail = user.email;
        } catch (e) {
          console.error('Error parsing eventifyUser for prefill:', e);
        }
      }
    }

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

      const token = getAuthToken();
      const userId = getUserId();

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
        categoryName: this.category.name || this.categoryNameFromUrl || 'Standard',
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
        // Handle errors array properly - check if it exists and is an array
        let errorMsg = 'Failed to create checkout session';
        
        if (checkoutResult.errors) {
          if (Array.isArray(checkoutResult.errors)) {
            errorMsg = checkoutResult.errors.join(', ');
          } else if (typeof checkoutResult.errors === 'string') {
            errorMsg = checkoutResult.errors;
          } else if (typeof checkoutResult.errors === 'object') {
            // Handle validation errors object format
            errorMsg = Object.values(checkoutResult.errors).flat().join(', ');
          }
        } else if (checkoutResult.message) {
          errorMsg = checkoutResult.message;
        }
        
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
