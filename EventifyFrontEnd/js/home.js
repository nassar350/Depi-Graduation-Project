// Home Page JavaScript - Real API Integration
// Handles home page functionality with real backend API

class HomePage {
  constructor() {
    this.apiBaseUrl = window.API_BASE_URL || 'https://eventify.runasp.net';
    this.eventsPerPage = 10;
    this.currentPage = 1;
    this.allEvents = [];
    this.init();
  }

  init() {
    this.loadUpcomingEvents();
    this.setupSearch();
  }

  // Load upcoming events from real API
  async loadUpcomingEvents() {
    try {
      // Show loading state
      const loading = document.getElementById('eventsLoading');
      const eventsGrid = document.getElementById('upcomingEventsGrid');
      const emptyState = document.getElementById('emptyState');
      
      if (loading) loading.style.display = 'block';
      if (eventsGrid) eventsGrid.style.display = 'none';
      if (emptyState) emptyState.style.display = 'none';

      // Fetch upcoming events from API
      const response = await fetch(`${this.apiBaseUrl}/api/events/upcoming?take=${this.eventsPerPage}`);
      const data = await response.json();

      // Hide loading
      if (loading) loading.style.display = 'none';

      if (data.success && data.data && data.data.length > 0) {
        this.allEvents = data.data;
        this.displayUpcomingEvents();
        if (eventsGrid) eventsGrid.style.display = 'grid';
      } else {
        // Show empty state
        if (emptyState) emptyState.style.display = 'block';
        console.log('No upcoming events:', data.message);
      }
      
    } catch (error) {
      console.error('Error loading events:', error);
      
      // Hide loading and show error
      const loading = document.getElementById('eventsLoading');
      if (loading) loading.style.display = 'none';
      
      app.showNotification('Error loading events. Please check your connection.', 'error');
    }
  }

  // Display upcoming events
  displayUpcomingEvents() {
    const eventsGrid = document.getElementById('upcomingEventsGrid');
    if (eventsGrid && this.allEvents.length > 0) {
      eventsGrid.innerHTML = this.allEvents.map(event => this.createEventCard(event)).join('');
    }
  }

  // Create event card HTML with real API data
  createEventCard(event) {
    const startDate = new Date(event.startDate);
    const formattedDate = this.formatDate(startDate);
    const formattedTime = this.formatTime(startDate);
    
    // Handle photo - use photoUrl if available, otherwise placeholder
    const imageUrl = event.photoUrl 
      ? event.photoUrl
      : 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&h=400&fit=crop';

    return `
      <article class="event-card" onclick="window.location.href='event.html?id=${event.id}'" style="cursor: pointer;">
        <img 
          src="${imageUrl}" 
          alt="${event.name}"
          class="event-card-image"
          onerror="this.src='https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&h=400&fit=crop'"
        >
        <div class="event-card-content">
          <div class="event-card-header">
            <h3 class="event-card-title">${event.name}</h3>
            <div class="event-card-status">
              <span class="status-badge status-${event.status.toLowerCase()}">${event.status}</span>
            </div>
          </div>
          
          <p class="event-card-description">${this.truncateText(event.description, 100)}</p>
          
          <div class="event-card-meta">
            <div class="event-card-date">
              <span>📅</span>
              <span>${formattedDate} at ${formattedTime}</span>
            </div>
            <div class="event-card-location">
              <span>📍</span>
              <span>${event.address}</span>
            </div>
          </div>
          
          <div class="event-card-footer">
            <div class="event-organizer">
              <span>👤 by ${event.organizerName}</span>
            </div>
            <button 
              class="btn btn-sm btn-primary" 
              onclick="event.stopPropagation(); window.location.href='event.html?id=${event.id}'"
              aria-label="View ${event.name} details"
            >
              View Details
            </button>
          </div>
        </div>
      </article>
    `;
  }

  // Setup search functionality with real API
  setupSearch() {
    const searchInput = document.getElementById('heroSearchInput');
    if (!searchInput) return;

    // Search on Enter key
    searchInput.addEventListener('keypress', (e) => {
      if (e.key === 'Enter') {
        const query = e.target.value.trim();
        if (query) {
          this.performSearch(query);
        }
      }
    });

    // Add search button functionality
    const searchIcon = document.querySelector('.search-icon');
    if (searchIcon) {
      searchIcon.style.cursor = 'pointer';
      searchIcon.addEventListener('click', () => {
        const query = searchInput.value.trim();
        if (query) {
          this.performSearch(query);
        }
      });
    }
  }

  // Perform search using real API
  async performSearch(query) {
    try {
      // Show loading in search results
      app.showNotification('Searching events...', 'info');
      
      const response = await fetch(`${this.apiBaseUrl}/api/events/search?searchTerm=${encodeURIComponent(query)}`);
      const data = await response.json();
      
      if (data.success && data.data && data.data.length > 0) {
        // Store search results and redirect to explore page
        sessionStorage.setItem('searchResults', JSON.stringify(data.data));
        sessionStorage.setItem('searchQuery', query);
        window.location.href = 'explore.html';
      } else {
        app.showNotification(`No events found for "${query}"`, 'warning');
      }
      
    } catch (error) {
      console.error('Search error:', error);
      app.showNotification('Search failed. Please try again.', 'error');
    }
  }

  // Utility function to format date
  formatDate(date) {
    const options = { 
      weekday: 'short', 
      year: 'numeric', 
      month: 'short', 
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

  // Utility function to truncate text
  truncateText(text, maxLength) {
    if (text.length <= maxLength) return text;
    return text.substring(0, maxLength).trim() + '...';
  }

  // Handle responsive grid adjustments
  handleResize() {
    const grids = document.querySelectorAll('.grid-3');
    grids.forEach(grid => {
      const width = window.innerWidth;
      
      if (width <= 600) {
        grid.style.gridTemplateColumns = '1fr';
      } else if (width <= 900) {
        grid.style.gridTemplateColumns = 'repeat(2, 1fr)';
      } else {
        grid.style.gridTemplateColumns = 'repeat(3, 1fr)';
      }
    });
  }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
  window.homePage = new HomePage();
  
  // Handle window resize
  window.addEventListener('resize', () => {
    window.homePage.handleResize();
  });
  
  // Initial resize check
  window.homePage.handleResize();
});

// Export for use in other scripts
window.HomePage = HomePage;