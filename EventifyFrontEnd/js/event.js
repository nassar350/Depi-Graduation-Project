// Event Detail Page JavaScript - Real API Integration
// Handles loading event details with categories, pricing, and organizer info

console.log('🚀 Event.js file is executing!');
console.log('🚀 Event.js loaded successfully');

// Test if this file is actually running
window.eventJsLoaded = true;
console.log('🚀 Event.js: Set window.eventJsLoaded = true');

class EventPage {
  constructor() {
    console.log('EventPage constructor started');
    this.apiBaseUrl = 'https://localhost:7105';
    console.log('API base URL set:', this.apiBaseUrl);
    this.eventId = null;
    this.event = null;
    this.selectedCategory = null;
    console.log('EventPage properties initialized');
    this.init();
    console.log('EventPage constructor completed');
  }

  init() {
    console.log('EventPage init() started');
    // Wait for app to be available
    if (typeof app === 'undefined') {
      console.log('App not yet available, retrying in 100ms...');
      setTimeout(() => this.init(), 100);
      return;
    }
    console.log('App is available, proceeding with initialization');
    this.getEventId();
    this.loadEvent();
    console.log('EventPage init() completed');
  }

  // Get event ID from URL parameters
  getEventId() {
    try {
      console.log('Getting event ID from URL...');
      const params = app.getUrlParams();
      console.log('URL parameters:', params);
      this.eventId = params.id ? parseInt(params.id) : null;
      console.log('Extracted event ID:', this.eventId);
      
      if (!this.eventId) {
        console.error('No event ID found in URL');
        this.showEventNotFound();
        return;
      }
    } catch (error) {
      console.error('Error getting event ID:', error);
      this.showEventNotFound();
    }
  }

  // Load event data from real API
  async loadEvent() {
    console.log('loadEvent() started');
    if (!this.eventId) {
      console.error('Cannot load event: no event ID available');
      console.log('Event ID value:', this.eventId);
      return;
    }
    
    try {
      console.log('Starting event load for ID:', this.eventId);
      this.showLoading(true);
      
      const apiUrl = `${this.apiBaseUrl}/api/events/${this.eventId}`;
      console.log('Fetching from:', apiUrl);
      
      const response = await fetch(apiUrl, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        }
      });
      
      console.log('Response received, status:', response.status);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const data = await response.json();
      console.log('Event data received:', data);

      if (data.success && data.data) {
        this.event = data.data;
        console.log('Event set successfully');
        
        // Ensure categories array exists
        if (!this.event.categories || !Array.isArray(this.event.categories)) {
          console.warn('Event missing categories, setting empty array');
          this.event.categories = [];
        }
        
        // Update page title
        document.title = `${this.event.name} - Eventify`;
        
        console.log('Starting to display event components...');
        
        // Display event
        this.displayEvent();
        this.displayCategories();
        this.displayOrganizer();
        this.displayEventInfo();
        
        console.log('Event display completed');
        
        this.showLoading(false);
        const eventContent = document.getElementById('eventContent');
        if (eventContent) {
          eventContent.style.display = 'block';
          console.log('Event content shown');
        }
        
        // Add event listeners (CSP-safe)
        this.initializeEventListeners();
        console.log('Event listeners initialized');
        
      } else {
        console.log('API returned unsuccessful response:', data);
        this.showEventNotFound();
      }
      
    } catch (error) {
      console.error('Error loading event:', error);
      
      // More specific error messages
      let errorMessage = 'Error loading event details';
      if (error.name === 'TypeError' && error.message.includes('Failed to fetch')) {
        errorMessage = 'Unable to connect to the server. Please check if the backend is running.';
      } else if (error.message.includes('CORS')) {
        errorMessage = 'Cross-origin request blocked. Please check CORS configuration.';
      } else if (error.message.includes('HTTP error')) {
        errorMessage = `Server error: ${error.message}`;
      }
      
      app.showNotification(errorMessage, 'error');
      this.showEventNotFound();
    }
  }

  // Initialize all event listeners (CSP-safe)
  initializeEventListeners() {
    // Book Now button
    const bookNowBtn = document.getElementById('bookNowBtn');
    if (bookNowBtn) {
      bookNowBtn.addEventListener('click', () => {
        this.bookNow();
      });
    }
    
    // Share buttons (if any exist)
    const shareButtons = document.querySelectorAll('.share-btn');
    shareButtons.forEach(button => {
      button.addEventListener('click', () => {
        const platform = button.getAttribute('data-platform');
        this.shareEvent(platform);
      });
    });
  }

  // Show/hide loading state
  showLoading(show) {
    const loading = document.getElementById('eventLoading');
    if (loading) {
      loading.style.display = show ? 'block' : 'none';
    }
  }

  // Show event not found state
  showEventNotFound() {
    this.showLoading(false);
    const notFound = document.getElementById('eventNotFound');
    if (notFound) {
      notFound.style.display = 'block';
    } else {
      app.showNotification('Event not found', 'error');
      setTimeout(() => {
        window.location.href = 'index.html';
      }, 2000);
    }
  }

  // Display main event information
  displayEvent() {
    if (!this.event) {
      console.error('No event data to display');
      return;
    }
    
    console.log('Displaying event:', this.event.name);
    
    // Validate required fields
    if (!this.event.name) {
      console.error('Event missing name field');
      app.showNotification('Invalid event data', 'error');
      return;
    }
    
    if (!this.event.startDate) {
      console.error('Event missing startDate field');
      this.event.startDate = new Date().toISOString();
    }
    
    if (!this.event.endDate) {
      console.error('Event missing endDate field');
      this.event.endDate = this.event.startDate;
    }

    const startDate = new Date(this.event.startDate);
    const endDate = new Date(this.event.endDate);
    const formattedDate = this.formatDate(startDate);
    const formattedTime = this.formatTime(startDate);
    const endTime = this.formatTime(endDate);

    // Handle photo - comprehensive image handling
    let imageUrl;
    console.log('Processing event photo:', {
      hasPhoto: !!this.event.photo,
      photoType: typeof this.event.photo,
      photoLength: this.event.photo ? this.event.photo.length : 0
    });
    
    if (this.event.photo && this.event.photo.length > 0) {
      try {
        if (typeof this.event.photo === 'string') {
          // Handle string format (URL or base64)
          if (this.event.photo.startsWith('http') || this.event.photo.startsWith('data:')) {
            imageUrl = this.event.photo;
          } else {
            // Assume it's base64 without prefix
            imageUrl = `data:image/jpeg;base64,${this.event.photo}`;
          }
        } else if (Array.isArray(this.event.photo) || this.event.photo instanceof Uint8Array) {
          // Handle byte array with CSP-safe conversion
          const bytes = new Uint8Array(this.event.photo);
          const base64String = this.arrayBufferToBase64(bytes);
          imageUrl = `data:image/jpeg;base64,${base64String}`;
        } else {
          console.warn('Unknown photo format, using placeholder');
          imageUrl = 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=1200&h=600&fit=crop';
        }
        console.log('Image URL generated:', imageUrl.substring(0, 50) + '...');
      } catch (error) {
        console.error('Error processing event photo:', error);
        imageUrl = 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=1200&h=600&fit=crop';
      }
    } else {
      console.log('No photo data, using placeholder');
      imageUrl = 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=1200&h=600&fit=crop';
    }

    // Hero section
    const heroImage = document.getElementById('eventHeroImage');
    if (heroImage) {
      heroImage.src = imageUrl;
      heroImage.alt = this.event.name;
      heroImage.onerror = function() {
        console.log('Failed to load image, using fallback');
        this.src = 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=1200&h=600&fit=crop';
      };
      heroImage.onload = function() {
        console.log('Image loaded successfully');
      };
    }
    
    const eventTitle = document.getElementById('eventTitle');
    if (eventTitle) eventTitle.textContent = this.event.name;
    
    const eventDate = document.getElementById('eventDate');
    if (eventDate) eventDate.textContent = `${formattedDate} at ${formattedTime}`;
    
    const eventLocation = document.getElementById('eventLocation');
    if (eventLocation) eventLocation.textContent = this.event.address;
    
    // Calculate and display total capacity
    const eventCapacity = document.getElementById('eventCapacity');
    if (eventCapacity) {
      let totalCapacity = 0;
      if (this.event.categories && Array.isArray(this.event.categories)) {
        totalCapacity = this.event.categories.reduce((sum, cat) => sum + (cat.seats || 0), 0);
      }
      eventCapacity.textContent = totalCapacity > 0 ? `${totalCapacity} Capacity` : 'Capacity TBA';
      console.log('Total capacity calculated:', totalCapacity);
    }
    
    // Calculate and display total capacity
    const eventCapacity = document.getElementById('eventCapacity');
    if (eventCapacity) {
      let totalCapacity = 0;
      if (this.event.categories && Array.isArray(this.event.categories)) {
        totalCapacity = this.event.categories.reduce((sum, cat) => sum + (cat.seats || 0), 0);
      }
      eventCapacity.textContent = totalCapacity > 0 ? totalCapacity : 'TBA';
      console.log('Total capacity calculated:', totalCapacity);
    }
    
    // Calculate and display total capacity
    const eventCapacity = document.getElementById('eventCapacity');
    if (eventCapacity) {
      let totalCapacity = 0;
      if (this.event.categories && Array.isArray(this.event.categories)) {
        totalCapacity = this.event.categories.reduce((sum, cat) => sum + (cat.seats || 0), 0);
      }
      eventCapacity.textContent = totalCapacity > 0 ? totalCapacity : 'TBA';
      console.log('Total capacity calculated:', totalCapacity);
    }

    // Description
    const eventDescription = document.getElementById('eventDescription');
    if (eventDescription) {
      eventDescription.innerHTML = this.formatDescription(this.event.description);
    }
  }

  // Display ticket categories with pricing
  displayCategories() {
    if (!this.event) {
      console.error('No event data for categories display');
      return;
    }
    
    if (!this.event.categories) {
      console.warn('Event has no categories property');
      return;
    }
    
    if (!Array.isArray(this.event.categories)) {
      console.error('Event categories is not an array:', typeof this.event.categories);
      return;
    }
    
    console.log('Displaying categories:', this.event.categories.length);

    const categoriesContainer = document.getElementById('ticketCategories');
    if (!categoriesContainer) {
      console.error('ticketCategories container not found in DOM');
      return;
    }

    const categoriesHtml = this.event.categories.map(category => {
      const availableSeats = category.seats - category.booked;
      const isAvailable = availableSeats > 0;
      
      return `
        <div class="ticket-category ${!isAvailable ? 'sold-out' : ''}" 
             data-category-id="${category.id}"
             ${isAvailable ? `onclick="eventPage.selectCategory(${category.id})"` : ''}>
          <div class="category-header">
            <h3 class="category-title">${category.title}</h3>
            <div class="category-capacity">
              <div class="capacity-available">${availableSeats} available</div>
              <div class="capacity-total">of ${category.seats} total</div>
            </div>
          </div>
          
          <div class="category-details">
            <div class="seats-info">
              <div class="seats-row">
                <span>Total Seats:</span>
                <span>${category.seats}</span>
              </div>
              <div class="seats-row">
                <span>Booked:</span>
                <span>${category.booked}</span>
              </div>
              <div class="seats-row">
                <span>Available:</span>
                <span class="${isAvailable ? 'text-success' : 'text-danger'}">
                  ${availableSeats}
                </span>
              </div>
            </div>
            
            ${isAvailable ? `
              <button class="btn btn-primary category-select-btn" 
                      data-category-id="${category.id}">
                Select ${category.title}
              </button>
            ` : `
              <button class="btn btn-secondary" disabled>
                Sold Out
              </button>
            `}
          </div>
        </div>
      `;
    }).join('');

    categoriesContainer.innerHTML = `
      <div class="section-header left-align">
        <h2>Ticket Categories</h2>
        <p class="section-subtitle">Choose your preferred seating option</p>
      </div>
      <div class="categories-grid">
        ${categoriesHtml}
      </div>
    `;
    
    // Add event listeners for category selection (CSP-safe)
    const categoryButtons = categoriesContainer.querySelectorAll('.category-select-btn');
    categoryButtons.forEach(button => {
      button.addEventListener('click', (e) => {
        e.stopPropagation();
        const categoryId = parseInt(button.getAttribute('data-category-id'));
        this.selectCategory(categoryId);
      });
    });
    
    // Add event listeners for category selection (CSP-safe)
    const categoryButtons = categoriesContainer.querySelectorAll('.category-select-btn');
    categoryButtons.forEach(button => {
      button.addEventListener('click', (e) => {
        e.stopPropagation();
        const categoryId = parseInt(button.getAttribute('data-category-id'));
        this.selectCategory(categoryId);
      });
    });
  }

  // Handle category selection
  selectCategory(categoryId) {
    if (!this.event || !this.event.categories) {
      console.error('No event or categories data available');
      app.showNotification('Unable to proceed with booking', 'error');
      return;
    }
    
    const category = this.event.categories.find(cat => cat.id === categoryId);
    if (!category) {
      console.error('Category not found:', categoryId);
      app.showNotification('Selected category not found', 'error');
      return;
    }
    
    const availableSeats = (category.seats || 0) - (category.booked || 0);
    if (availableSeats <= 0) {
      app.showNotification('This category is sold out', 'error');
      return;
    }
    
    console.log('Navigating to booking with:', {
      eventId: this.eventId,
      categoryId: categoryId,
      eventName: this.event.name,
      categoryName: category.name
    });
    
    // Navigate to booking page with event and category info
    const bookingUrl = `book.html?eventId=${this.eventId}&categoryId=${categoryId}&eventName=${encodeURIComponent(this.event.name)}&categoryName=${encodeURIComponent(category.name)}&price=${category.price}`;
    window.location.href = bookingUrl;
  }
    
    // Update visual selection
    document.querySelectorAll('.ticket-category').forEach(el => {
      el.classList.remove('selected');
    });
    
    const selectedElement = document.querySelector(`[data-category-id="${categoryId}"]`);
    if (selectedElement) {
      selectedElement.classList.add('selected');
    }

    // Show booking section
    this.displayBookingSummary();
    
    app.showNotification(`Selected ${category.title} category`, 'success');
  }

  // Display organizer information
  displayOrganizer() {
    if (!this.event) {
      console.error('No event data for organizer display');
      return;
    }
    
    if (!this.event.organizer) {
      console.warn('Event has no organizer property');
      return;
    }
    
    console.log('Displaying organizer:', this.event.organizer);

    const organizerContainer = document.getElementById('organizerInfo');
    if (!organizerContainer) {
      console.error('organizerInfo container not found in DOM');
      return;
    }

    const organizer = this.event.organizer;

    organizerContainer.innerHTML = `
      <div class="organizer-card">
        <div class="organizer-header">
          <div class="organizer-avatar">
            <img 
              src="https://ui-avatars.com/api/?name=${encodeURIComponent(organizer.name)}&background=7c5cff&color=ffffff&size=80" 
              alt="${organizer.name}"
              onerror="this.src='https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=80&h=80&fit=crop&crop=face'"
            >
          </div>
          <div class="organizer-details">
            <h4>${organizer.name}</h4>
            <p class="organizer-role">${organizer.role}</p>
            <p class="organizer-contact">${organizer.email}</p>
          </div>
        </div>
        
        <div class="organizer-actions">
          <a href="mailto:${organizer.email}" class="btn btn-outline btn-sm">
            ✉️ Contact Organizer
          </a>
          <a href="tel:${organizer.phoneNumber}" class="btn btn-outline btn-sm">
            📞 Call
          </a>
        </div>
      </div>
    `;
  }

  // Display event info summary
  displayEventInfo() {
    if (!this.event) {
      console.error('No event data for info display');
      return;
    }
    
    console.log('Event info - checking description field:', {
      description: this.event.description,
      hasDescription: !!this.event.description,
      descriptionLength: this.event.description ? this.event.description.length : 0
    });
    
    // Update event description
    const eventDescription = document.getElementById('eventDescription');
    if (eventDescription) {
      const description = this.event.description || 'No description available for this event.';
      eventDescription.innerHTML = description;
      console.log('Set event description:', description.substring(0, 100) + '...');
    } else {
      console.error('eventDescription element not found in DOM');
    }

    const infoContainer = document.getElementById('eventInfoSummary');
    if (!infoContainer) {
      console.error('eventInfoSummary container not found');
      return;
    }

    const startDate = new Date(this.event.startDate);
    const endDate = new Date(this.event.endDate);
    const formattedDate = this.formatDate(startDate);
    const startTime = this.formatTime(startDate);
    const endTime = this.formatTime(endDate);
    const duration = `${startTime} - ${endTime}`;

    const isUpcoming = startDate > new Date();
    const totalSeats = this.event.categories.reduce((sum, cat) => sum + cat.seats, 0);
    const bookedSeats = this.event.categories.reduce((sum, cat) => sum + cat.booked, 0);
    const availableSeats = totalSeats - bookedSeats;

    infoContainer.innerHTML = `
      <div class="info-grid">
        <div class="info-item">
          <h5>📅 Date & Time</h5>
          <p>${formattedDate}</p>
          <p class="info-subtitle">${duration}</p>
        </div>
        
        <div class="info-item">
          <h5>📍 Location</h5>
          <p>${this.event.address}</p>
        </div>
        
        <div class="info-item">
          <h5>🎫 Availability</h5>
          <p>${availableSeats > 0 
            ? `${availableSeats} tickets available` 
            : '<span class="text-danger">Sold Out</span>'
          }</p>
          <p class="info-subtitle">Total capacity: ${totalSeats}</p>
        </div>
        
        <div class="info-item">
          <h5>📊 Status</h5>
          <p class="${isUpcoming ? 'text-success' : 'text-muted'}">
            ${isUpcoming ? 'Upcoming Event' : 'Past Event'}
          </p>
        </div>
        
        <div class="info-item">
          <h5>👤 Organizer</h5>
          <p>${this.event.organizer.name}</p>
          <p class="info-subtitle">${this.event.organizer.role}</p>
        </div>
        
        <div class="info-item">
          <h5>📈 Attendees</h5>
          <p>${this.event.attendees ? this.event.attendees.length : 0} registered</p>
        </div>
      </div>
    `;
  }

  // Display booking summary for selected category
  displayBookingSummary() {
    if (!this.selectedCategory) return;

    const summaryContainer = document.getElementById('bookingSummary');
    if (!summaryContainer) return;

    const availableSeats = this.selectedCategory.seats - this.selectedCategory.booked;

    summaryContainer.innerHTML = `
      <div class="booking-summary-card">
        <h4>Booking Summary</h4>
        
        <div class="summary-row">
          <span>Category:</span>
          <span class="summary-value">${this.selectedCategory.title}</span>
        </div>
        
        <div class="summary-row">
          <span>Available Seats:</span>
          <span class="summary-value text-success">${availableSeats}</span>
        </div>
        
        <div class="summary-actions">
          ${availableSeats > 0 ? `
            <button class="btn btn-primary btn-block" onclick="eventPage.proceedToBooking()">
              Proceed to Booking
            </button>
          ` : `
            <button class="btn btn-secondary btn-block" disabled>
              Category Sold Out
            </button>
          `}
          
          <button class="btn btn-outline btn-block" onclick="eventPage.clearSelection()">
            Choose Different Category
          </button>
        </div>
      </div>
    `;

    // Scroll to booking summary
    summaryContainer.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
  }

  // Proceed to booking page
  proceedToBooking() {
    if (!this.selectedCategory) {
      app.showNotification('Please select a ticket category first', 'warning');
      return;
    }

    // Check if user is logged in
    const user = app.getCurrentUser();
    if (!user) {
      app.showNotification('Please log in to book tickets', 'warning');
      setTimeout(() => {
        window.location.href = `login.html?redirect=${encodeURIComponent('event.html?id=' + this.eventId)}`;
      }, 1500);
      return;
    }

    // Redirect to booking page with category info
    const bookingUrl = `book.html?eventId=${this.eventId}&categoryId=${this.selectedCategory.id}&categoryTitle=${encodeURIComponent(this.selectedCategory.title)}`;
    window.location.href = bookingUrl;
  }

  // Clear category selection
  clearSelection() {
    this.selectedCategory = null;
    
    // Remove visual selection
    document.querySelectorAll('.ticket-category').forEach(el => {
      el.classList.remove('selected');
    });
    
    // Hide booking summary
    const summaryContainer = document.getElementById('bookingSummary');
    if (summaryContainer) {
      summaryContainer.innerHTML = '';
    }
  }

  // Format event description with paragraphs
  formatDescription(description) {
    return description
      .split('\n\n')
      .map(paragraph => `<p>${paragraph.trim()}</p>`)
      .join('');
  }

  // Utility function to format date
  formatDate(date) {
    const options = { 
      weekday: 'long', 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    };
    return date.toLocaleDateString('en-US', options);
  }

  // Utility function to format time
  formatTime(date) {
    const options = { 
      hour: '2-digit', 
      minute: '2-digit', 
      hour12: true 
    };
    return date.toLocaleTimeString('en-US', options);
  }

  // Share event functionality
  shareEvent(platform) {
    if (!this.event) return;

    const eventUrl = window.location.href;
    const eventTitle = this.event.name;

    switch (platform) {
      case 'copy':
        this.copyToClipboard(eventUrl);
        break;
      case 'twitter':
        const twitterUrl = `https://twitter.com/intent/tweet?text=${encodeURIComponent(`Check out this event: ${eventTitle}`)}&url=${encodeURIComponent(eventUrl)}`;
        window.open(twitterUrl, '_blank', 'noopener,noreferrer');
        break;
      case 'facebook':
        const facebookUrl = `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(eventUrl)}`;
        window.open(facebookUrl, '_blank', 'noopener,noreferrer');
        break;
    }
  }

  // Copy URL to clipboard
  async copyToClipboard(text) {
    try {
      await navigator.clipboard.writeText(text);
      app.showNotification('Event link copied to clipboard!', 'success');
    } catch (err) {
      app.showNotification('Failed to copy link', 'error');
    }
  }

  // CSP-safe Base64 conversion for byte arrays
  arrayBufferToBase64(buffer) {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    let result = '';
    let i = 0;
    
    while (i < buffer.length) {
      const a = buffer[i++];
      const b = i < buffer.length ? buffer[i++] : 0;
      const c = i < buffer.length ? buffer[i++] : 0;
      
      const bitmap = (a << 16) | (b << 8) | c;
      
      result += chars.charAt((bitmap >> 18) & 63);
      result += chars.charAt((bitmap >> 12) & 63);
      result += i - 2 < buffer.length ? chars.charAt((bitmap >> 6) & 63) : '=';
      result += i - 1 < buffer.length ? chars.charAt(bitmap & 63) : '=';
    }
    
    return result;
  }

  // Book Now button functionality
  bookNow() {
    if (!this.event) {
      console.error('No event data available for booking');
      app.showNotification('Event data not loaded', 'error');
      return;
    }
    
    // Check if there are available categories
    if (!this.event.categories || this.event.categories.length === 0) {
      app.showNotification('No ticket categories available for this event', 'error');
      return;
    }
    
    // Find the first available category
    const availableCategory = this.event.categories.find(cat => {
      const available = (cat.seats || 0) - (cat.booked || 0);
      return available > 0;
    });
    
    if (!availableCategory) {
      app.showNotification('This event is sold out', 'error');
      return;
    }
    
    console.log('Book Now clicked - redirecting to booking with first available category');
    
    // Navigate to booking page with first available category
    const bookingUrl = `book.html?eventId=${this.eventId}&categoryId=${availableCategory.id}&eventName=${encodeURIComponent(this.event.name)}&categoryName=${encodeURIComponent(availableCategory.name)}&price=${availableCategory.price}`;
    window.location.href = bookingUrl;
  }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
  console.log('=== EVENT PAGE INITIALIZATION START ===');
  console.log('DOM Content Loaded - initializing EventPage');
  console.log('App object available:', typeof app !== 'undefined');
  console.log('Current URL:', window.location.href);
  console.log('URL search params:', window.location.search);
  
  try {
    console.log('Creating new EventPage instance...');
    window.eventPage = new EventPage();
    console.log('EventPage initialized successfully:', window.eventPage);
    console.log('=== EVENT PAGE INITIALIZATION COMPLETE ===');
  } catch (error) {
    console.error('Error initializing EventPage:', error);
    console.error('Error stack:', error.stack);
  }
});

// Immediate test - run this code right away
console.log('Event.js: Adding DOM listener and running immediate test');
console.log('Event.js: Document ready state:', document.readyState);

// Try immediate initialization if DOM is already ready
if (document.readyState === 'loading') {
  console.log('Event.js: DOM is still loading, waiting for DOMContentLoaded');
} else {
  console.log('Event.js: DOM already ready, initializing immediately');
  setTimeout(() => {
    if (!window.eventPage) {
      console.log('Event.js: No eventPage found, creating now');
      try {
        window.eventPage = new EventPage();
        console.log('Event.js: EventPage created successfully via immediate init');
      } catch (error) {
        console.error('Event.js: Error in immediate initialization:', error);
      }
    }
  }, 100);
}

// Fallback initialization
window.addEventListener('load', () => {
  if (!window.eventPage) {
    console.log('Fallback initialization of EventPage');
    try {
      window.eventPage = new EventPage();
    } catch (error) {
      console.error('Error in fallback initialization:', error);
    }
  }
});

// Export for use in other scripts
window.EventPage = EventPage;