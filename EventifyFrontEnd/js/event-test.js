// Test file to check if JS execution works
console.log('🚀 TEST FILE LOADED SUCCESSFULLY!');
window.testLoaded = true;

class EventPage {
  constructor() {
    console.log('🚀 EventPage constructor working!');
    // Use global API base if available (set in pages), fallback to production
    this.apiBaseUrl = (window.API_BASE_URL) ? window.API_BASE_URL : 'https://eventify.runasp.net';
    this.eventId = this.getEventId();
    this.loadEvent();
  }

  getEventId() {
    const params = new URLSearchParams(window.location.search);
    const id = params.get('id');
    console.log('🚀 Event ID extracted:', id);
    return id ? parseInt(id) : null;
  }

  async loadEvent() {
    if (!this.eventId) {
      console.log('🚀 No event ID, showing not found');
      this.showEventNotFound();
      return;
    }

    console.log('🚀 Loading event:', this.eventId);
    this.showLoading(true);
    
    try {
      const apiUrl = `${this.apiBaseUrl}/api/events/${this.eventId}`;
      console.log('🚀 Fetching event from:', apiUrl);
      
      const response = await fetch(apiUrl, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        },
        mode: 'cors',
        credentials: 'omit'
      });
      
      console.log('🚀 Response status:', response.status);
      console.log('🚀 Response headers:', [...response.headers.entries()]);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const data = await response.json();
      console.log('🚀 Event data:', data);
      
      if (data.success && data.data) {
        this.event = data.data;
        
        // Ensure categories exist
        if (!this.event.categories || !Array.isArray(this.event.categories)) {
          this.event.categories = [];
        }
        
        // Fetch real-time available ticket counts for each category
        await this.fetchAvailableTicketCounts();
        
        this.displayEvent();
        this.showLoading(false);
      } else {
        this.showEventNotFound();
      }
    } catch (error) {
      console.error('🚀 Load error:', error);
      this.showEventNotFound();
    }
  }

  async fetchAvailableTicketCounts() {
    if (!this.event.categories || this.event.categories.length === 0) {
      return;
    }

    console.log('🚀 Fetching available ticket counts for categories');
    
    for (const category of this.event.categories) {
      try {
        const response = await fetch(
          `${this.apiBaseUrl}/api/Tickets/available?eventId=${this.eventId}&categoryName=${encodeURIComponent(category.title)}`,
          {
            method: 'GET',
            headers: {
              'Content-Type': 'application/json',
              'Accept': 'application/json'
            }
          }
        );

        if (response.ok) {
          const data = await response.json();
          if (data.success && data.data) {
            category.availableTicketsCount = data.data.availableTickets;
            console.log(`🚀 Category "${category.title}" available tickets:`, category.availableTicketsCount);
          }
        }
      } catch (error) {
        console.error(`🚀 Error fetching ticket count for category "${category.title}":`, error);
      }
    }
  }

  showLoading(show) {
    const loading = document.getElementById('eventLoading');
    if (loading) {
      loading.style.display = show ? 'block' : 'none';
    }
  }

  showEventNotFound() {
    this.showLoading(false);
    const notFound = document.getElementById('eventNotFound');
    if (notFound) {
      notFound.style.display = 'block';
    }
  }

  displayEvent() {
    console.log('🚀 Displaying event:', this.event.name);
    
    // Update page title
    document.title = `${this.event.name} - Eventify`;
    
    // Display hero image
    const heroImage = document.getElementById('eventHeroImage');
    if (heroImage) {
      let imageUrl = 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=1200&h=600&fit=crop';
      if (this.event.photoUrl) {
        imageUrl = this.event.photoUrl;
      }
      heroImage.src = imageUrl;
      heroImage.alt = this.event.name;
    }
    
    // Display event title
    const eventTitle = document.getElementById('eventTitle');
    if (eventTitle) eventTitle.textContent = this.event.name;
    
    // Display event date
    const eventDate = document.getElementById('eventDate');
    if (eventDate && this.event.startDate) {
      const date = new Date(this.event.startDate);
      eventDate.textContent = date.toLocaleDateString('en-US', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    }
    
    // Display location
    const eventLocation = document.getElementById('eventLocation');
    if (eventLocation) eventLocation.textContent = this.event.address || 'Location TBA';
    
    // Display capacity
    const eventCapacity = document.getElementById('eventCapacity');
    if (eventCapacity) {
      let totalCapacity = 0;
      if (this.event.categories && Array.isArray(this.event.categories)) {
        totalCapacity = this.event.categories.reduce((sum, cat) => sum + (cat.seats || 0), 0);
      }
      eventCapacity.textContent = totalCapacity > 0 ? `Total Capacity: ${totalCapacity}` : 'Capacity TBA';
    }
    
    // Display description
    const eventDescription = document.getElementById('eventDescription');
    if (eventDescription) {
      eventDescription.textContent = this.event.description || 'No description available.';
    }
    
    // Display categories
    this.displayCategories();
    
    // Display organizer information
    console.log('🚀 About to display organizer');
    this.displayOrganizer();
    
    // Display event details
    console.log('🚀 About to display event info');
    this.displayEventInfo();
    
    // Show event content
    const eventContent = document.getElementById('eventContent');
    if (eventContent) {
      eventContent.style.display = 'block';
      console.log('🚀 Event content displayed');
    }
    
    // Add Book Now button functionality
    this.initializeBookButton();
  }
  
  initializeBookButton() {
    const bookNowBtn = document.getElementById('bookNowBtn');
    if (bookNowBtn) {
      bookNowBtn.addEventListener('click', (e) => {
        e.preventDefault();
        
        // Add visual feedback
        const originalText = bookNowBtn.textContent;
        bookNowBtn.textContent = 'Processing...';
        bookNowBtn.disabled = true;
        
        console.log('🚀 Book Now button clicked');
        
        setTimeout(() => {
          this.bookNow();
          
          // Reset button after delay if still on page
          setTimeout(() => {
            if (bookNowBtn) {
              bookNowBtn.textContent = originalText;
              bookNowBtn.disabled = false;
            }
          }, 2000);
        }, 100);
      });
      
      console.log('🚀 Book Now button initialized');
    } else {
      console.error('🚀 Book Now button not found in DOM');
    }
  }
  
  selectCategory(categoryId) {
    console.log('🚀 Category selected:', categoryId);
    
    if (!this.event || !this.event.categories) {
      console.error('🚀 No event or categories data available');
      alert('Event data not available. Please refresh the page.');
      return;
    }
    
    const category = this.event.categories.find(cat => cat.id === categoryId);
    if (!category) {
      console.error('🚀 Category not found:', categoryId);
      alert('Selected category not found.');
      return;
    }
    
    const categoryName = category.name || category.title || category.categoryName || 'Standard';
    const categorySeats = category.seats || category.capacity || 0;
    const categoryBooked = category.booked || category.sold || 0;
    const availableSeats = categorySeats - categoryBooked;
    
    if (availableSeats <= 0) {
      alert(`${categoryName} category is sold out`);
      return;
    }
    
    console.log('🚀 Navigating to booking for category:', categoryName);
    console.log('🚀 Available seats:', availableSeats);
    
    // Store booking data in session storage for the booking page
    const bookingData = {
      eventId: this.eventId,
      eventName: this.event.name,
      categoryId: categoryId,
      categoryName: categoryName,
      price: category.ticketPrice || 0,
      availableSeats: availableSeats,
      eventDate: this.event.startDate,
      eventLocation: this.event.address
    };
    
    sessionStorage.setItem('bookingData', JSON.stringify(bookingData));
    
    // Navigate to booking page
    const bookingUrl = `book.html?eventId=${this.eventId}&categoryId=${categoryId}&eventName=${encodeURIComponent(this.event.name)}&categoryName=${encodeURIComponent(categoryName)}&price=${category.ticketPrice || 0}`;
    console.log('🚀 Redirecting to:', bookingUrl);
    window.location.href = bookingUrl;
  }
  
  bookNow() {
    console.log('🚀 Book Now clicked');
    
    if (!this.event) {
      console.error('🚀 No event data available for booking');
      alert('Event data not loaded. Please refresh the page.');
      return;
    }
    
    // Check if there are available categories
    if (!this.event.categories || this.event.categories.length === 0) {
      alert('No ticket categories available for this event');
      return;
    }
    
    console.log('🚀 Checking for available categories...');
    
    // Find the first available category
    const availableCategory = this.event.categories.find(cat => {
      const seats = cat.seats || cat.capacity || 0;
      const booked = cat.booked || cat.sold || 0;
      const available = seats - booked;
      console.log(`🚀 Category ${cat.name || cat.title}: ${available} available`);
      return available > 0;
    });
    
    if (!availableCategory) {
      alert('This event is sold out');
      return;
    }
    
    console.log('🚀 Book Now redirecting to first available category:', availableCategory.name || availableCategory.title);
    this.selectCategory(availableCategory.id);
  }
  
  async displayCategories() {
    if (!this.event.categories || this.event.categories.length === 0) {
      console.log('🚀 No categories to display');
      return;
    }
    
    const categoriesContainer = document.getElementById('ticketCategories');
    if (!categoriesContainer) {
      console.log('🚀 No categories container found');
      return;
    }
    
    console.log('🚀 Raw categories data:', this.event.categories);
    
    // Fetch available seats for each category
    const categoriesWithAvailability = await Promise.all(
      this.event.categories.map(async (category) => {
        console.log('🚀 Processing category:', category);
        
        const categoryName = category.name || category.title || category.categoryName || 'Standard';
        const categoryPrice = category.ticketPrice || 0;
        const categoryDescription = category.description || category.desc || 'Standard seating';
        
        // Calculate fallback available seats from category data
        const categorySeats = category.seats || category.capacity || 0;
        const categoryBooked = category.booked || category.sold || 0;
        const fallbackAvailableSeats = categorySeats - categoryBooked;
        
        // Fetch real-time available seats from API
        let availableSeats = fallbackAvailableSeats;
        let isAvailable = availableSeats > 0;
        
        try {
          const apiUrl = `${this.apiBaseUrl}/api/tickets/available?eventId=${this.eventId}&categoryName=${encodeURIComponent(categoryName)}`;
          console.log(`🚀 Fetching available seats from: ${apiUrl}`);
          
          const response = await fetch(apiUrl, {
            method: 'GET',
            headers: {
              'Content-Type': 'application/json',
              'Accept': 'application/json'
            },
            mode: 'cors'
          });
          const data = await response.json();
          
          console.log(`🚀 API response for ${categoryName}:`, data);
          
          if (data.success && data.data && typeof data.data.availableTickets === 'number') {
            availableSeats = data.data.availableTickets;
            isAvailable = availableSeats > 0;
            console.log(`🚀 Category: ${categoryName}, Available seats from API: ${availableSeats}`);
          } else {
            console.warn(`🚀 Using fallback seats for ${categoryName}. API response:`, data);
          }
        } catch (error) {
          console.error(`🚀 Error fetching available seats for ${categoryName}, using fallback:`, error);
        }
        
        return {
          ...category,
          categoryName,
          categoryPrice,
          categoryDescription,
          availableSeats,
          isAvailable
        };
      })
    );
    
    const categoriesHtml = categoriesWithAvailability.map(category => {
      return `
        <div class="ticket-category ${!category.isAvailable ? 'sold-out' : ''}" data-category-id="${category.id}">
          <div class="category-header">
            <h3>${category.categoryName}</h3>
            <div class="category-price">${category.categoryPrice} EGP</div>
          </div>
          <div class="category-details">
            <p class="category-description">${category.categoryDescription}</p>
            <div class="category-info">
              <span class="seats-available">
                ${category.isAvailable ? 
                  `${category.availableSeats} seats available` : 
                  '<span class="sold-out-text">Sold Out</span>'
                }
              </span>
            </div>
          </div>
          <div class="category-actions">
            ${category.isAvailable ? 
              `<button class="btn btn-primary category-select-btn" data-category-id="${category.id}">Select ${category.categoryName}</button>` : 
              '<button class="btn btn-secondary" disabled>Sold Out</button>'
            }
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
    
    // Add event listeners for category buttons
    const categoryButtons = categoriesContainer.querySelectorAll('.category-select-btn');
    categoryButtons.forEach(button => {
      button.addEventListener('click', (e) => {
        e.stopPropagation();
        e.preventDefault();
        
        // Add visual feedback
        button.textContent = 'Redirecting...';
        button.disabled = true;
        
        const categoryId = parseInt(button.getAttribute('data-category-id'));
        console.log('🚀 Category button clicked, ID:', categoryId);
        
        setTimeout(() => {
          this.selectCategory(categoryId);
        }, 100);
      });
      
      // Add hover effect
      button.addEventListener('mouseenter', () => {
        button.style.transform = 'scale(1.05)';
      });
      
      button.addEventListener('mouseleave', () => {
        button.style.transform = 'scale(1)';
      });
    });
    
    console.log('🚀 Categories displayed:', this.event.categories.length, 'with', categoryButtons.length, 'buttons');
  }
  
  displayOrganizer() {
    console.log('🚀 Displaying organizer information');
    console.log('🚀 Event data for organizer:', {
      hasOrganizer: !!this.event.organizer,
      hasOrganizerName: !!this.event.organizerName,
      organizer: this.event.organizer,
      organizerName: this.event.organizerName
    });
    
    const organizerContainer = document.getElementById('organizerInfo');
    if (!organizerContainer) {
      console.log('🚀 No organizer container found');
      return;
    }
    
    // Handle different organizer data formats
    const organizer = this.event.organizer || this.event.organizerName || null;
    const organizerName = typeof organizer === 'string' ? organizer : 
                         (organizer && organizer.name) ? organizer.name : 
                         this.event.organizerName || 'Event Organizer';
    const organizerEmail = (organizer && organizer.email) ? organizer.email : 'contact@eventify.com';
    const organizerPhone = (organizer && organizer.phone) ? organizer.phone : '';
    const organizerBio = (organizer && organizer.bio) ? organizer.bio : 
                        (organizer && organizer.description) ? organizer.description :
                        'Professional event organizer committed to creating memorable experiences.';
    
    console.log('🚀 Organizer details:', { organizerName, organizerEmail, organizerBio });
    
    organizerContainer.innerHTML = `
      <div class="organizer-card">
        <div class="organizer-avatar">
          <div class="avatar-placeholder">
            ${organizerName.charAt(0).toUpperCase()}
          </div>
        </div>
        <div class="organizer-details">
          <h3 class="organizer-name">${organizerName}</h3>
          <p class="organizer-title">Event Organizer</p>
          <p class="organizer-bio">${organizerBio}</p>
          <div class="organizer-contact">
            <div class="contact-item">
              <span class="contact-icon">📧</span>
              <a href="mailto:${organizerEmail}" class="contact-link">${organizerEmail}</a>
            </div>
            ${organizerPhone ? `
              <div class="contact-item">
                <span class="contact-icon">📞</span>
                <span class="contact-text">${organizerPhone}</span>
              </div>
            ` : ''}
          </div>
        </div>
      </div>
    `;
    
    console.log('🚀 Organizer information displayed');
  }
  
  displayEventInfo() {
    console.log('🚀 Displaying event details');
    console.log('🚀 Event data for info:', {
      startDate: this.event.startDate,
      endDate: this.event.endDate,
      address: this.event.address,
      categories: this.event.categories?.length || 0
    });
    
    const eventDetailsContainer = document.getElementById('eventInfoSummary');
    if (!eventDetailsContainer) {
      console.log('🚀 No event info summary container found');
      return;
    }
    
    const startDate = new Date(this.event.startDate);
    const endDate = this.event.endDate ? new Date(this.event.endDate) : null;
    const formattedStartDate = startDate.toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
    const startTime = startDate.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
    const endTime = endDate ? endDate.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    }) : null;
    
    const totalSeats = this.event.categories ? 
      this.event.categories.reduce((sum, cat) => sum + (cat.seats || 0), 0) : 0;
    
    // Calculate available seats from real-time ticket data
    let availableSeats = 0;
    if (this.event.categories && Array.isArray(this.event.categories)) {
      for (const cat of this.event.categories) {
        // Check if we have real-time available ticket count
        if (cat.availableTicketsCount !== undefined) {
          availableSeats += cat.availableTicketsCount;
        } else {
          // Fallback to seats - booked calculation
          availableSeats += (cat.seats || 0) - (cat.booked || 0);
        }
      }
    }
    
    const percentageRemaining = totalSeats > 0 ? ((availableSeats / totalSeats) * 100).toFixed(0) : 0;
    
    eventDetailsContainer.innerHTML = `
      <div class="event-info-grid">
        <div class="info-item">
          <div class="info-icon">📅</div>
          <div class="info-content">
            <h4>Date & Time</h4>
            <p class="info-primary">${formattedStartDate}</p>
            <p class="info-secondary">${startTime}${endTime ? ` - ${endTime}` : ''}</p>
          </div>
        </div>
        
        <div class="info-item">
          <div class="info-icon">📍</div>
          <div class="info-content">
            <h4>Venue</h4>
            <p class="info-primary">${this.event.address || 'Venue to be announced'}</p>
          </div>
        </div>
        
        <div class="info-item">
          <div class="info-icon">🎫</div>
          <div class="info-content">
            <h4>Tickets Available</h4>
            <p class="info-primary">${availableSeats} of ${totalSeats} seats</p>
            <p class="info-secondary">${percentageRemaining}% remaining</p>
          </div>
        </div>
        
        <div class="info-item">
          <div class="info-icon">👥</div>
          <div class="info-content">
            <h4>Organized by</h4>
            <p class="info-primary">${this.event.organizerName || 'Event Organizer'}</p>
          </div>
        </div>
      </div>
    `;
    
    console.log('🚀 Event details displayed');
  }
  
  // Share event functionality
  shareEvent(platform) {
    console.log('🚀 Share event called with platform:', platform);
    
    if (!this.event) {
      console.error('🚀 No event data available for sharing');
      alert('Event data not loaded');
      return;
    }

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
      default:
        console.error('🚀 Unknown share platform:', platform);
    }
  }

  // Copy URL to clipboard
  async copyToClipboard(text) {
    try {
      await navigator.clipboard.writeText(text);
      console.log('🚀 Link copied to clipboard');
      alert('Event link copied to clipboard!');
    } catch (err) {
      console.error('🚀 Failed to copy:', err);
      // Fallback method
      const textArea = document.createElement('textarea');
      textArea.value = text;
      textArea.style.position = 'fixed';
      textArea.style.left = '-999999px';
      document.body.appendChild(textArea);
      textArea.select();
      try {
        document.execCommand('copy');
        console.log('🚀 Link copied using fallback method');
        alert('Event link copied to clipboard!');
      } catch (err2) {
        console.error('🚀 Fallback copy failed:', err2);
        alert('Failed to copy link');
      }
      document.body.removeChild(textArea);
    }
  }
}

// Initialize
document.addEventListener('DOMContentLoaded', () => {
  console.log('🚀 DOM ready, creating EventPage');
  window.eventPage = new EventPage();
});

// Add test functionality for debugging
window.testBooking = function() {
  console.log('🚀 Test booking function called');
  if (window.eventPage) {
    window.eventPage.bookNow();
  } else {
    console.error('🚀 EventPage not initialized');
  }
};

window.testCategorySelection = function(categoryId) {
  console.log('🚀 Test category selection:', categoryId);
  if (window.eventPage) {
    window.eventPage.selectCategory(categoryId);
  } else {
    console.error('🚀 EventPage not initialized');
  }
};

console.log('🚀 Event test file end reached');