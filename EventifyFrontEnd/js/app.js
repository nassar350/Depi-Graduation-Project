// Eventify Global App JavaScript
// Handles navigation, mobile menu, utilities, and global behaviors

console.log('App.js loaded successfully');

class EventifyApp {
  constructor() {
    this.init();
  }

  init() {
    this.setupNavigation();
    this.setupMobileMenu();
    this.setupScrollEffects();
    this.setupModalHandlers();
    this.setupFormValidation();
    this.checkAuthState();
    this.handlePageLoad();
  }

  // Navigation setup
  setupNavigation() {
    const navbar = document.querySelector('.navbar');
    const navLinks = document.querySelectorAll('.navbar-nav a');
    
    // Set active navigation link based on current page
    const currentPage = window.location.pathname.split('/').pop() || 'index.html';
    navLinks.forEach(link => {
      const href = link.getAttribute('href');
      if (href === currentPage || (currentPage === '' && href === 'index.html')) {
        link.classList.add('active');
      }
    });

    // Navbar scroll effect
    if (navbar) {
      window.addEventListener('scroll', () => {
        if (window.scrollY > 100) {
          navbar.classList.add('scrolled');
        } else {
          navbar.classList.remove('scrolled');
        }
      });
    }
  }

  // Mobile menu functionality
  setupMobileMenu() {
    const menuToggle = document.querySelector('.navbar-toggle');
    const navMenu = document.querySelector('.navbar-nav');
    
    if (menuToggle && navMenu) {
      menuToggle.addEventListener('click', () => {
        navMenu.classList.toggle('active');
        
        // Update aria-expanded for accessibility
        const isExpanded = navMenu.classList.contains('active');
        menuToggle.setAttribute('aria-expanded', isExpanded);
      });

      // Close menu when clicking on a link
      navMenu.querySelectorAll('a').forEach(link => {
        link.addEventListener('click', () => {
          navMenu.classList.remove('active');
          menuToggle.setAttribute('aria-expanded', 'false');
        });
      });

      // Close menu when clicking outside
      document.addEventListener('click', (e) => {
        if (!menuToggle.contains(e.target) && !navMenu.contains(e.target)) {
          navMenu.classList.remove('active');
          menuToggle.setAttribute('aria-expanded', 'false');
        }
      });
    }
  }

  // Scroll effects (fade-in animations)
  setupScrollEffects() {
    const observerOptions = {
      threshold: 0.1,
      rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('fade-in-up');
        }
      });
    }, observerOptions);

    // Observe elements that should animate on scroll
    document.querySelectorAll('.event-card, .card, .section-header').forEach(el => {
      observer.observe(el);
    });
  }

  // Modal functionality
  setupModalHandlers() {
    // Close modal when clicking backdrop or close button
    document.addEventListener('click', (e) => {
      if (e.target.classList.contains('modal-overlay')) {
        this.closeModal(e.target);
      }
      
      if (e.target.classList.contains('modal-close')) {
        const modal = e.target.closest('.modal-overlay');
        this.closeModal(modal);
      }
    });

    // Close modal with Escape key
    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape') {
        const activeModal = document.querySelector('.modal-overlay.active');
        if (activeModal) {
          this.closeModal(activeModal);
        }
      }
    });
  }

  // Open modal
  openModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
      modal.classList.add('active');
      document.body.style.overflow = 'hidden';
      
      // Focus first focusable element in modal
      const focusableElements = modal.querySelectorAll(
        'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
      );
      if (focusableElements.length > 0) {
        focusableElements[0].focus();
      }
    }
  }

  // Close modal
  closeModal(modal) {
    if (modal) {
      modal.classList.remove('active');
      document.body.style.overflow = '';
    }
  }

  // Form validation utilities
  setupFormValidation() {
    // Real-time validation for form inputs
    document.addEventListener('input', (e) => {
      if (e.target.classList.contains('form-input') || 
          e.target.classList.contains('form-select') || 
          e.target.classList.contains('form-textarea')) {
        this.validateField(e.target);
      }
    });

    // Validate on blur
    document.addEventListener('blur', (e) => {
      if (e.target.classList.contains('form-input') || 
          e.target.classList.contains('form-select') || 
          e.target.classList.contains('form-textarea')) {
        this.validateField(e.target);
      }
    }, true);
  }

  // Validate individual form field
  validateField(field) {
    const value = field.value.trim();
    const type = field.type;
    const required = field.hasAttribute('required');
    let isValid = true;
    let errorMessage = '';

    // Remove existing validation classes
    field.classList.remove('error', 'success');
    
    // Remove existing error message
    const existingError = field.parentNode.querySelector('.form-error');
    if (existingError) {
      existingError.remove();
    }

    // Required field validation
    if (required && !value) {
      isValid = false;
      errorMessage = 'This field is required';
    }
    // Email validation
    else if (type === 'email' && value) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(value)) {
        isValid = false;
        errorMessage = 'Please enter a valid email address';
      }
    }
    // Password validation (minimum 8 characters)
    else if (type === 'password' && value) {
      if (value.length < 8) {
        isValid = false;
        errorMessage = 'Password must be at least 8 characters long';
      }
    }
    // Number validation
    else if (type === 'number' && value) {
      const min = field.getAttribute('min');
      const max = field.getAttribute('max');
      const numValue = parseFloat(value);
      
      if (isNaN(numValue)) {
        isValid = false;
        errorMessage = 'Please enter a valid number';
      } else if (min && numValue < parseFloat(min)) {
        isValid = false;
        errorMessage = `Value must be at least ${min}`;
      } else if (max && numValue > parseFloat(max)) {
        isValid = false;
        errorMessage = `Value must be no more than ${max}`;
      }
    }

    // Apply validation state
    if (isValid && value) {
      field.classList.add('success');
    } else if (!isValid) {
      field.classList.add('error');
      
      // Show error message
      const errorElement = document.createElement('span');
      errorElement.className = 'form-error';
      errorElement.textContent = errorMessage;
      field.parentNode.appendChild(errorElement);
    }

    return isValid;
  }

  // Validate entire form
  validateForm(form) {
    const fields = form.querySelectorAll('.form-input, .form-select, .form-textarea');
    let isValid = true;

    fields.forEach(field => {
      if (!this.validateField(field)) {
        isValid = false;
      }
    });

    return isValid;
  }

  // Authentication state management
  checkAuthState() {
    const user = this.getCurrentUser();
    const authLinks = document.querySelectorAll('[data-auth-required]');
    const guestLinks = document.querySelectorAll('[data-guest-only]');
    const userInfo = document.querySelector('.user-info');

    if (user) {
      // User is logged in
      authLinks.forEach(link => link.style.display = '');
      guestLinks.forEach(link => link.style.display = 'none');
      
      if (userInfo) {
        userInfo.innerHTML = `
          <span>Welcome, ${user.name}</span>
          <a href="dashboard.html" class="btn btn-sm btn-primary">Dashboard</a>
          <button onclick="app.logout()" class="btn btn-sm btn-secondary">Logout</button>
        `;
      }
    } else {
      // User is not logged in
      authLinks.forEach(link => link.style.display = 'none');
      guestLinks.forEach(link => link.style.display = '');
      
      if (userInfo) {
        userInfo.innerHTML = `
          <a href="login.html" class="btn btn-sm btn-outline">Login</a>
          <a href="register.html" class="btn btn-sm btn-primary">Sign Up</a>
        `;
      }
    }
  }

  // Get current user from localStorage (with JWT token validation)
  getCurrentUser() {
    const userJson = localStorage.getItem('eventifyUser');
    const token = localStorage.getItem('eventify_token');
    
    if (!userJson || !token) {
      return null;
    }

    try {
      const user = JSON.parse(userJson);
      
      // Check if token is expired
      if (this.isTokenExpired(token)) {
        this.logout();
        return null;
      }
      
      return user;
    } catch (error) {
      console.error('Error parsing user data:', error);
      this.logout();
      return null;
    }
  }

  // Set current user in localStorage
  setCurrentUser(user) {
    localStorage.setItem('eventifyUser', JSON.stringify(user));
    if (user.token) {
      localStorage.setItem('eventify_token', user.token);
    }
    this.checkAuthState();
  }

  // Check if JWT token is expired
  isTokenExpired(token) {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Math.floor(Date.now() / 1000);
      return payload.exp && payload.exp < currentTime;
    } catch (error) {
      console.error('Error checking token expiration:', error);
      return true; // Treat invalid tokens as expired
    }
  }

  // Get JWT token from localStorage
  getToken() {
    return localStorage.getItem('eventify_token');
  }

  // Logout functionality
  logout() {
    localStorage.removeItem('eventifyUser');
    localStorage.removeItem('eventify_token');
    this.checkAuthState();
    
    this.showNotification('You have been logged out successfully', 'success');
    
    // Redirect to home page
    if (window.location.pathname !== '/index.html' && window.location.pathname !== '/') {
      window.location.href = 'index.html';
    }
  }

  // Utility: Show notification/toast
  showNotification(message, type = 'info', duration = 5000) {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
      <span>${message}</span>
      <button class="notification-close">&times;</button>
    `;

    // Add to page
    document.body.appendChild(notification);

    // Position notification
    const notifications = document.querySelectorAll('.notification');
    const offset = (notifications.length - 1) * 70;
    notification.style.cssText = `
      position: fixed;
      top: ${20 + offset}px;
      right: 20px;
      background: var(--card);
      border: 1px solid var(--border);
      border-radius: var(--radius-md);
      padding: var(--space-md) var(--space-lg);
      box-shadow: var(--shadow-lg);
      z-index: var(--z-tooltip);
      transform: translateX(100%);
      transition: transform var(--anim-speed) ease;
      max-width: 400px;
    `;

    // Different colors for different types
    switch (type) {
      case 'success':
        notification.style.borderColor = 'var(--success)';
        notification.style.color = 'var(--success)';
        break;
      case 'error':
        notification.style.borderColor = 'var(--danger)';
        notification.style.color = 'var(--danger)';
        break;
      case 'warning':
        notification.style.borderColor = 'var(--warning)';
        notification.style.color = 'var(--warning)';
        break;
    }

    // Animate in
    setTimeout(() => {
      notification.style.transform = 'translateX(0)';
    }, 10);

    // Handle close button
    const closeBtn = notification.querySelector('.notification-close');
    closeBtn.addEventListener('click', () => {
      this.closeNotification(notification);
    });

    // Auto close
    if (duration > 0) {
      setTimeout(() => {
        this.closeNotification(notification);
      }, duration);
    }

    return notification;
  }

  // Close notification
  closeNotification(notification) {
    notification.style.transform = 'translateX(100%)';
    setTimeout(() => {
      if (notification.parentNode) {
        notification.parentNode.removeChild(notification);
      }
    }, 300);
  }

  // Utility: Format currency
  formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  // Utility: Debounce function
  debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
      const later = () => {
        clearTimeout(timeout);
        func(...args);
      };
      clearTimeout(timeout);
      timeout = setTimeout(later, wait);
    };
  }

  // Utility: Get URL parameters
  getUrlParams() {
    const params = new URLSearchParams(window.location.search);
    const result = {};
    for (const [key, value] of params) {
      result[key] = value;
    }
    return result;
  }

  // Utility: Set URL parameter without page reload
  setUrlParam(key, value) {
    const url = new URL(window.location);
    url.searchParams.set(key, value);
    window.history.pushState({}, '', url);
  }

  // Utility: Remove URL parameter
  removeUrlParam(key) {
    const url = new URL(window.location);
    url.searchParams.delete(key);
    window.history.pushState({}, '', url);
  }

  // Handle page-specific initialization
  handlePageLoad() {
    const currentPage = window.location.pathname.split('/').pop() || 'index.html';
    
    // Page-specific initialization
    switch (currentPage) {
      case 'index.html':
      case '':
        this.initHomePage();
        break;
      case 'explore.html':
        this.initExplorePage();
        break;
      case 'event.html':
        this.initEventPage();
        break;
      case 'book.html':
        this.initBookingPage();
        break;
      case 'dashboard.html':
        this.initDashboardPage();
        break;
      case 'login.html':
      case 'register.html':
        this.initAuthPages();
        break;
    }
  }

  // Initialize home page
  initHomePage() {
    // This will be handled by home.js
    console.log('Home page initialized');
  }

  // Initialize explore page
  initExplorePage() {
    // This will be handled by explore.js
    console.log('Explore page initialized');
  }

  // Initialize event page
  initEventPage() {
    // This will be handled by event.js
    console.log('Event page initialized');
  }

  // Initialize booking page
  initBookingPage() {
    // Redirect if no event ID
    const params = this.getUrlParams();
    if (!params.eventId) {
      this.showNotification('No event selected for booking', 'error');
      setTimeout(() => {
        window.location.href = 'index.html';
      }, 2000);
    }
  }

  // Initialize dashboard page
  initDashboardPage() {
    // Redirect if not logged in
    if (!this.getCurrentUser()) {
      this.showNotification('Please log in to access your dashboard', 'warning');
      setTimeout(() => {
        window.location.href = 'login.html?redirect=dashboard.html';
      }, 2000);
    }
  }

  // Initialize auth pages
  initAuthPages() {
    // Redirect if already logged in
    if (this.getCurrentUser()) {
      const params = this.getUrlParams();
      const redirect = params.redirect || 'dashboard.html';
      window.location.href = redirect;
    }
  }

  // Utility: Lazy load images
  setupLazyLoading() {
    const images = document.querySelectorAll('img[data-src]');
    
    const imageObserver = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          const img = entry.target;
          img.src = img.dataset.src;
          img.classList.remove('lazy');
          imageObserver.unobserve(img);
        }
      });
    });

    images.forEach(img => imageObserver.observe(img));
  }

  // API utility: Make authenticated API calls
  async apiCall(endpoint, options = {}) {
    const token = this.getToken();
    const baseUrl = 'https://localhost:7105';
    
    const defaultOptions = {
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
      }
    };

    const finalOptions = {
      ...defaultOptions,
      ...options,
      headers: {
        ...defaultOptions.headers,
        ...options.headers
      }
    };

    try {
      const response = await fetch(`${baseUrl}${endpoint}`, finalOptions);
      
      // Handle token expiration
      if (response.status === 401) {
        this.showNotification('Your session has expired. Please log in again.', 'warning');
        this.logout();
        return null;
      }
      
      return response;
    } catch (error) {
      console.error('API call failed:', error);
      throw error;
    }
  }
}

// Initialize app when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
  console.log('=== APP INITIALIZATION START ===');
  console.log('Initializing EventifyApp...');
  window.app = new EventifyApp();
  console.log('App initialized successfully:', window.app);
  console.log('=== APP INITIALIZATION COMPLETE ===');
});

// Handle browser back/forward buttons
window.addEventListener('popstate', () => {
  window.app.handlePageLoad();
});

// Export for use in other scripts
window.EventifyApp = EventifyApp;