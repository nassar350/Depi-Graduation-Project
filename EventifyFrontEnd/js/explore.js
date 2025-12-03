// Explore Page JavaScript
// Handles search, filtering, sorting, and pagination of events

class ExplorePage {
  constructor() {
    this.apiBaseUrl = window.API_BASE_URL || 'https://eventify.runasp.net';
    this.allEvents = [];
    this.filteredEvents = [];
    this.currentPage = 1;
    this.eventsPerPage = 12;
    this.currentViewMode = 'grid';
    this.currentFilters = {};
    this.currentSort = 'date-asc';
    this.currentSearch = '';
    
    // EventCategory enum mapping
    this.eventCategoryMap = {
      1: 'Music',
      2: 'Sports',
      3: 'Arts',
      4: 'Food',
      5: 'Technology',
      6: 'Business',
      7: 'Education',
      8: 'Entertainment',
      9: 'Health',
      10: 'Community',
      11: 'Other'
    };
    
    this.init();
  }

  init() {
    this.loadEvents();
    this.setupFilters();
    this.setupSearch();
    this.setupSorting();
    this.setupViewToggle();
    this.setupPagination();
    this.handleUrlParameters();
    this.restorePreferences();
  }

  // Load events data from real API
  async loadEvents() {
    try {
      this.showLoading(true);
      
      // Check if we have search results from home page
      const searchResults = sessionStorage.getItem('searchResults');
      const searchQuery = sessionStorage.getItem('searchQuery');
      
      if (searchResults && searchQuery) {
        // Use search results from home page
        this.allEvents = JSON.parse(searchResults);
        this.currentSearch = searchQuery;
        
        // Update search input
        const searchInput = document.getElementById('mainSearchInput');
        if (searchInput) {
          searchInput.value = searchQuery;
        }
        
        // Clear session storage
        sessionStorage.removeItem('searchResults');
        sessionStorage.removeItem('searchQuery');
      } else {
        // Load all upcoming events
        const response = await fetch(`${this.apiBaseUrl}/api/events/upcoming?take=100`);
        const data = await response.json();
        
        if (data.success && data.data) {
          this.allEvents = data.data;
        } else {
          this.allEvents = [];
          console.log('No events found:', data.message);
        }
      }
      
      this.filteredEvents = [...this.allEvents];
      this.populateCategoryFilter();
      this.applyFiltersAndSearch();
      
    } catch (error) {
      console.error('Error loading events:', error);
      app.showNotification('Error loading events. Please try again.', 'error');
      this.allEvents = [];
      this.filteredEvents = [];
      this.displayResults();
    } finally {
      this.showLoading(false);
    }
  }

  // Show/hide loading state
  showLoading(show) {
    const loading = document.getElementById('resultsLoading');
    const container = document.getElementById('resultsContainer');
    const empty = document.getElementById('emptyResults');
    const pagination = document.getElementById('paginationContainer');
    
    if (loading) loading.style.display = show ? 'block' : 'none';
    if (container) container.style.display = show ? 'none' : 'block';
    if (empty) empty.style.display = 'none';
    if (pagination) pagination.style.display = show ? 'none' : 'block';
  }

  // Populate category filter dropdown from actual event data
  populateCategoryFilter() {
    const categorySelect = document.getElementById('categoryFilter');
    if (!categorySelect) return;

    // EventCategory enum values from backend (matching create-event.js)
    const eventCategories = [
      { value: 1, label: 'Music' },
      { value: 2, label: 'Sports' },
      { value: 3, label: 'Arts' },
      { value: 4, label: 'Food' },
      { value: 5, label: 'Technology' },
      { value: 6, label: 'Business' },
      { value: 7, label: 'Education' },
      { value: 8, label: 'Entertainment' },
      { value: 9, label: 'Health' },
      { value: 10, label: 'Community' },
      { value: 11, label: 'Other' }
    ];
    
    categorySelect.innerHTML = '<option value="">All Categories</option>' +
      eventCategories.map(category => `<option value="${category.value}">${category.label}</option>`).join('');
  }

  // Setup filter event handlers
  setupFilters() {
    const filterInputs = [
      'categoryFilter',
      'startDate', 
      'endDate',
      'minPrice',
      'maxPrice'
    ];

    filterInputs.forEach(id => {
      const element = document.getElementById(id);
      if (element) {
        element.addEventListener('change', () => {
          this.applyFiltersAndSearch();
        });
        
        // For price inputs, also listen to input events
        if (id.includes('Price')) {
          element.addEventListener('input', app.debounce(() => {
            this.applyFiltersAndSearch();
          }, 500));
        }
      }
    });
  }

  // Setup search functionality
  setupSearch() {
    const searchInput = document.getElementById('mainSearchInput');
    if (!searchInput) return;

    // Debounced search
    const debouncedSearch = app.debounce(() => {
      this.applyFiltersAndSearch();
    }, 300);

    searchInput.addEventListener('input', (e) => {
      this.currentSearch = e.target.value.trim();
      debouncedSearch();
      
      // Update URL
      if (this.currentSearch) {
        app.setUrlParam('search', this.currentSearch);
      } else {
        app.removeUrlParam('search');
      }
    });

    // Clear search on escape
    searchInput.addEventListener('keydown', (e) => {
      if (e.key === 'Escape') {
        e.target.value = '';
        this.currentSearch = '';
        this.applyFiltersAndSearch();
        app.removeUrlParam('search');
      }
    });
  }

  // Setup sorting functionality
  setupSorting() {
    const sortSelect = document.getElementById('sortSelect');
    if (!sortSelect) return;

    sortSelect.addEventListener('change', (e) => {
      this.currentSort = e.target.value;
      this.sortEvents();
      this.displayResults();
      
      // Save preference
      localStorage.setItem('eventify_sort_preference', this.currentSort);
    });
  }

  // Setup view toggle
  setupViewToggle() {
    const gridBtn = document.getElementById('gridViewBtn');
    const listBtn = document.getElementById('listViewBtn');
    
    if (gridBtn) {
      gridBtn.addEventListener('click', () => this.setViewMode('grid'));
    }
    
    if (listBtn) {
      listBtn.addEventListener('click', () => this.setViewMode('list'));
    }
  }

  // Set view mode (grid/list)
  setViewMode(mode) {
    this.currentViewMode = mode;
    
    const gridBtn = document.getElementById('gridViewBtn');
    const listBtn = document.getElementById('listViewBtn');
    const resultsGrid = document.getElementById('resultsGrid');
    
    // Update button states
    if (gridBtn && listBtn) {
      gridBtn.classList.toggle('active', mode === 'grid');
      listBtn.classList.toggle('active', mode === 'list');
    }
    
    // Update grid classes
    if (resultsGrid) {
      resultsGrid.className = mode === 'grid' ? 'grid grid-3' : 'events-list';
    }
    
    // Redisplay results with new view
    this.displayResults();
    
    // Save preference
    localStorage.setItem('eventify_view_preference', mode);
  }

  // Setup pagination
  setupPagination() {
    const loadMoreBtn = document.getElementById('loadMoreBtn');
    if (!loadMoreBtn) return;

    loadMoreBtn.addEventListener('click', () => {
      this.currentPage++;
      this.displayResults(false); // Don't reset, append
      this.updatePagination();
    });
  }

  // Handle URL parameters (search, category, etc.)
  handleUrlParameters() {
    const params = app.getUrlParams();
    
    // Handle search parameter
    if (params.search) {
      const searchInput = document.getElementById('mainSearchInput');
      if (searchInput) {
        searchInput.value = params.search;
        this.currentSearch = params.search;
      }
    }
    
    // Handle category parameter
    if (params.category) {
      const categorySelect = document.getElementById('categoryFilter');
      if (categorySelect) {
        categorySelect.value = params.category;
        this.updateFilters();
      }
    }
  }

  // Restore user preferences
  restorePreferences() {
    // Restore view mode
    const savedViewMode = localStorage.getItem('eventify_view_preference');
    if (savedViewMode && ['grid', 'list'].includes(savedViewMode)) {
      this.setViewMode(savedViewMode);
    }
    
    // Restore sort preference
    const savedSort = localStorage.getItem('eventify_sort_preference');
    const sortSelect = document.getElementById('sortSelect');
    if (savedSort && sortSelect) {
      sortSelect.value = savedSort;
      this.currentSort = savedSort;
    }
    
    // Restore last search filters (optional)
    const savedFilters = localStorage.getItem('eventify_last_filters');
    if (savedFilters) {
      try {
        const filters = JSON.parse(savedFilters);
        this.restoreFilterValues(filters);
      } catch (e) {
        console.warn('Could not restore saved filters:', e);
      }
    }
  }

  // Restore filter values from saved state
  restoreFilterValues(filters) {
    Object.entries(filters).forEach(([key, value]) => {
      const element = document.getElementById(key);
      if (element && value !== undefined && value !== '') {
        element.value = value;
      }
    });
  }

  // Update current filters object
  updateFilters() {
    this.currentFilters = {
      category: document.getElementById('categoryFilter')?.value || '',
      startDate: document.getElementById('startDate')?.value || '',
      endDate: document.getElementById('endDate')?.value || '',
      minPrice: document.getElementById('minPrice')?.value || '',
      maxPrice: document.getElementById('maxPrice')?.value || ''
    };
    
    // Save filters
    localStorage.setItem('eventify_last_filters', JSON.stringify(this.currentFilters));
  }

  // Apply filters and search with real API
  async applyFiltersAndSearch() {
    this.updateFilters();
    this.currentPage = 1; // Reset to first page
    
    let results = [...this.allEvents];
    
    // Apply search with API if we have a search term
    if (this.currentSearch && this.currentSearch.trim()) {
      try {
        const response = await fetch(`${this.apiBaseUrl}/api/events/search?searchTerm=${encodeURIComponent(this.currentSearch)}`);
        const data = await response.json();
        
        if (data.success && data.data) {
          results = data.data;
        } else {
          results = [];
        }
      } catch (error) {
        console.error('Search error:', error);
        app.showNotification('Search failed. Please try again.', 'error');
        results = [];
      }
    }
    
    // Apply local filters
    results = this.filterResults(results);
    
    // Sort results
    this.filteredEvents = results;
    this.sortEvents();
    
    // Display results
    this.displayResults();
    this.updateResultsInfo();
    this.updatePagination();
  }

  // Filter results based on current filters
  filterResults(events) {
    return events.filter(event => {
      // Category filter (using EventCategory enum from backend)
      if (this.currentFilters.category) {
        // event.eventCategory should contain the enum value (1-11) from backend
        // Convert filter value to number for comparison
        const filterCategory = parseInt(this.currentFilters.category);
        // Handle both number and string format from backend
        const eventCat = typeof event.eventCategory === 'number' ? event.eventCategory : parseInt(event.eventCategory);
        if (eventCat !== filterCategory) {
          return false;
        }
      }
      
      // Date range filter
      const eventDate = new Date(event.startDate);
      
      if (this.currentFilters.startDate) {
        const startDate = new Date(this.currentFilters.startDate);
        if (eventDate < startDate) return false;
      }
      
      if (this.currentFilters.endDate) {
        const endDate = new Date(this.currentFilters.endDate);
        if (eventDate > endDate) return false;
      }
      
      // Note: Price filtering removed as API doesn't provide price data
      // This can be re-added when pricing information is available
      
      return true;
    });
  }

  // Sort events based on current sort option
  sortEvents() {
    this.filteredEvents.sort((a, b) => {
      switch (this.currentSort) {
        case 'date-asc':
          return new Date(a.startDate) - new Date(b.startDate);
        case 'date-desc':
          return new Date(b.startDate) - new Date(a.startDate);
        case 'title-asc':
          return a.name.localeCompare(b.name);
        case 'title-desc':
          return b.name.localeCompare(a.name);
        // Price sorting removed as not available in API
        default:
          return 0;
      }
    });
  }

  // Display filtered and sorted results
  displayResults(reset = true) {
    const resultsGrid = document.getElementById('resultsGrid');
    const emptyResults = document.getElementById('emptyResults');
    const container = document.getElementById('resultsContainer');
    
    if (!resultsGrid) return;

    // Reset page if needed
    if (reset) {
      this.currentPage = 1;
    }

    // Show/hide empty state
    if (this.filteredEvents.length === 0) {
      if (emptyResults) emptyResults.style.display = 'block';
      if (container) container.style.display = 'none';
      if (resultsGrid) resultsGrid.style.display = 'none';
      return;
    } else {
      if (emptyResults) emptyResults.style.display = 'none';
      if (container) container.style.display = 'block';
    }

    // Calculate events to show
    const startIndex = reset ? 0 : (this.currentPage - 1) * this.eventsPerPage;
    const endIndex = this.currentPage * this.eventsPerPage;
    const eventsToShow = this.filteredEvents.slice(
      reset ? 0 : startIndex,
      reset ? endIndex : endIndex
    );

    // Generate HTML
    const eventsHtml = eventsToShow.map(event => this.createEventCard(event)).join('');
    
    if (reset || this.currentPage === 1) {
      resultsGrid.innerHTML = eventsHtml;
    } else {
      resultsGrid.innerHTML += eventsHtml;
    }

    // Ensure grid is visible
    resultsGrid.style.display = this.currentViewMode === 'grid' ? 'grid' : 'block';
  }

  // Create event card HTML
  createEventCard(event) {
    const startDate = new Date(event.startDate);
    const formattedDate = this.formatDate(startDate);
    const formattedTime = this.formatTime(startDate);
    
    // Handle photo - use photoUrl if available, otherwise placeholder
    const imageUrl = event.photoUrl 
      ? event.photoUrl
      : 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&h=400&fit=crop';
    
    const isListView = this.currentViewMode === 'list';
    
    if (isListView) {
      return `
        <article class="event-card" onclick="window.location.href='event.html?id=${event.id}'" style="margin-bottom: var(--space-md);">
          <img 
            src="${imageUrl}" 
            alt="${event.name}"
            class="event-card-image"
            onerror="this.src='https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&h=400&fit=crop'"
          >
          <div class="event-card-content" style="flex: 1; display: flex; justify-content: space-between; align-items: center;">
            <div style="flex: 1;">
              <h3 class="event-card-title" style="margin-bottom: var(--space-xs);">${event.name}</h3>
              <p class="event-card-description">${this.truncateText(event.description, 100)}</p>
              <div class="event-card-meta" style="margin-bottom: var(--space-sm);">
                <div class="event-card-date">
                  <span>📅</span>
                  <span>${formattedDate} at ${formattedTime}</span>
                </div>
                <div class="event-card-location">
                  <span>📍</span>
                  <span>${event.address}</span>
                </div>
                <div class="event-card-tickets">
                  <span>🎫</span>
                  <span>${event.availableTickets} tickets available</span>
                </div>
              </div>
            </div>
            <div style="text-align: right;">
              <div class="event-organizer" style="margin-bottom: var(--space-sm);">by ${event.organizerName}</div>
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
    } else {
      return `
        <article class="event-card" onclick="window.location.href='event.html?id=${event.id}'">
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
            
            <p class="event-card-description">${this.truncateText(event.description, 80)}</p>
            
            <div class="event-card-meta">
              <div class="event-card-date">
                <span>📅</span>
                <span>${formattedDate} at ${formattedTime}</span>
              </div>
              <div class="event-card-location">
                <span>📍</span>
                <span>${event.address}</span>
              </div>
              <div class="event-card-tickets">
                <span>🎫</span>
                <span>${event.availableTickets} available</span>
              </div>
            </div>
            
            <div class="event-card-footer">
              <div class="event-organizer">
                👤 by ${event.organizerName}
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

  // Update results information
  updateResultsInfo() {
    const resultsTitle = document.getElementById('resultsTitle');
    const resultsCount = document.getElementById('resultsCount');
    
    let title = 'All Events';
    
    // Update title based on current state
    if (this.currentSearch) {
      title = `Search Results for "${this.currentSearch}"`;
    } else if (this.currentFilters.category) {
      const categoryLabel = this.eventCategoryMap[parseInt(this.currentFilters.category)] || 'Unknown';
      title = `${categoryLabel} Events`;
    }
    
    if (resultsTitle) resultsTitle.textContent = title;
    
    const totalResults = this.filteredEvents.length;
    const displayedResults = Math.min(this.currentPage * this.eventsPerPage, totalResults);
    
    if (resultsCount) {
      if (totalResults === 0) {
        resultsCount.textContent = 'No events found';
      } else {
        resultsCount.textContent = `Showing ${displayedResults} of ${totalResults} event${totalResults !== 1 ? 's' : ''}`;
      }
    }
  }

  // Update pagination controls
  updatePagination() {
    const loadMoreBtn = document.getElementById('loadMoreBtn');
    const paginationInfo = document.getElementById('paginationInfo');
    const paginationContainer = document.getElementById('paginationContainer');
    
    const totalEvents = this.filteredEvents.length;
    const displayedEvents = this.currentPage * this.eventsPerPage;
    const hasMore = displayedEvents < totalEvents;
    
    if (paginationContainer) {
      paginationContainer.style.display = totalEvents > 0 ? 'block' : 'none';
    }
    
    if (loadMoreBtn) {
      if (hasMore) {
        loadMoreBtn.style.display = 'inline-flex';
        const remaining = totalEvents - displayedEvents;
        loadMoreBtn.textContent = `Load More (${remaining} remaining)`;
      } else {
        loadMoreBtn.style.display = 'none';
      }
    }
    
    if (paginationInfo) {
      paginationInfo.textContent = `Showing ${Math.min(displayedEvents, totalEvents)} of ${totalEvents} events`;
    }
  }

  // Clear all filters
  clearFilters() {
    // Clear form inputs
    const inputs = ['categoryFilter', 'startDate', 'endDate', 'minPrice', 'maxPrice'];
    inputs.forEach(id => {
      const element = document.getElementById(id);
      if (element) {
        element.value = '';
      }
    });
    
    // Clear search
    const searchInput = document.getElementById('mainSearchInput');
    if (searchInput) {
      searchInput.value = '';
      this.currentSearch = '';
    }
    
    // Clear URL parameters
    ['search', 'category'].forEach(param => app.removeUrlParam(param));
    
    // Clear saved filters
    localStorage.removeItem('eventify_last_filters');
    
    // Reapply (which will show all events)
    this.applyFiltersAndSearch();
    
    app.showNotification('Filters cleared', 'info');
  }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
  window.exploreePage = new ExplorePage();
});

// Export for use in other scripts
window.ExplorePage = ExplorePage;