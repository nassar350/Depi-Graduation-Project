// Create Event Page JavaScript - Backend API Integration
class CreateEventPage {
  constructor() {
    this.currentStep = 1;
    this.maxStep = 4;
    this.categories = [];
    this.apiBaseUrl = window.API_BASE_URL || 'https://localhost:7105';
    
    this.init();
  }

  init() {
    // Check authentication - require login
    const user = app.getCurrentUser();
    if (!user) {
      app.showNotification('Please log in to create events', 'warning');
      setTimeout(() => {
        window.location.href = 'login.html?redirect=create-event.html';
      }, 1000);
      return;
    }

    this.setupEventHandlers();
    this.setupFormValidation();
    this.setMinDateTime();
    this.initializeCategories();
  }

  setupEventHandlers() {
    const form = document.getElementById('createEventForm');
    if (form) {
      form.addEventListener('submit', (e) => this.handleFormSubmit(e));
    }

    // Character counter for description
    const descTextarea = document.getElementById('eventDescription');
    const charCount = document.getElementById('descCharCount');
    if (descTextarea && charCount) {
      descTextarea.addEventListener('input', () => {
        charCount.textContent = descTextarea.value.length;
      });
    }

    // Start/End date validation
    const startDateInput = document.getElementById('startDate');
    const endDateInput = document.getElementById('endDate');

    if (startDateInput && endDateInput) {
      startDateInput.addEventListener('change', () => this.validateDateRange());
      endDateInput.addEventListener('change', () => this.validateDateRange());
    }

    // Real-time preview updates
    this.setupPreviewUpdates();
  }

  setupFormValidation() {
    const inputs = document.querySelectorAll('.form-input, .form-select, .form-textarea');
    inputs.forEach(input => {
      input.addEventListener('blur', () => this.validateField(input));
      input.addEventListener('input', () => this.clearFieldError(input));
    });
  }

  setupPreviewUpdates() {
    const previewFields = ['eventTitle', 'startDate', 'endDate', 'eventLocation', 'eventDescription'];
    
    previewFields.forEach(fieldId => {
      const field = document.getElementById(fieldId);
      if (field) {
        field.addEventListener('input', () => {
          if (this.currentStep === 4) {
            this.updateEventPreview();
          }
        });
      }
    });
  }

  setMinDateTime() {
    const startDateInput = document.getElementById('startDate');
    const endDateInput = document.getElementById('endDate');
    
    // Set minimum datetime to current time
    const now = new Date();
    now.setMinutes(now.getMinutes() + 60); // At least 1 hour from now
    const minDateTime = now.toISOString().slice(0, 16);
    
    if (startDateInput) {
      startDateInput.min = minDateTime;
    }
    
    if (endDateInput) {
      endDateInput.min = minDateTime;
    }
  }

  validateDateRange() {
    const startDateInput = document.getElementById('startDate');
    const endDateInput = document.getElementById('endDate');
    
    if (startDateInput.value && endDateInput.value) {
      const startDate = new Date(startDateInput.value);
      const endDate = new Date(endDateInput.value);
      
      if (endDate <= startDate) {
        this.showFieldError(endDateInput, 'End date must be after start date');
        return false;
      } else {
        this.clearFieldError(endDateInput);
        return true;
      }
    }
    return true;
  }

  // Categories Management
  initializeCategories() {
    // Add a default category
    this.addCategory();
  }

  addCategory() {
    const container = document.getElementById('categoriesContainer');
    if (!container) return;

    const categoryId = `category-${Date.now()}`;
    const categoryHtml = `
      <div class="category-item" id="${categoryId}" style="
        border: 2px solid var(--border); 
        border-radius: var(--radius-md); 
        padding: var(--space-lg); 
        margin-bottom: var(--space-md);
        position: relative;
      ">
        <div class="form-row">
          <div class="form-group" style="flex: 2;">
            <label class="form-label">Category Title *</label>
            <input 
              type="text" 
              class="form-input category-title" 
              placeholder="e.g., VIP, Premium, Regular"
              required
            >
          </div>
          <div class="form-group" style="flex: 1;">
            <label class="form-label">Seats *</label>
            <input 
              type="number" 
              class="form-input category-seats" 
              placeholder="50"
              min="1"
              max="10000"
              required
            >
          </div>
          <div class="form-group" style="flex: 1;">
            <label class="form-label">Ticket Price *</label>
            <input 
              type="number" 
              class="form-input category-price" 
              placeholder="0.00"
              min="0"
              step="0.01"
              required
            >
          </div>
          <div class="form-group" style="flex: 0 0 auto;">
            <label class="form-label" style="visibility: hidden;">Remove</label>
            <button 
              type="button" 
              class="btn btn-outline" 
              style="background: var(--danger); color: white; border-color: var(--danger);"
              onclick="createEventPage.removeCategory('${categoryId}')"
            >
              ✕
            </button>
          </div>
        </div>
      </div>
    `;

    container.insertAdjacentHTML('beforeend', categoryHtml);
    this.updateAddButtonState();
  }

  removeCategory(categoryId) {
    const categoryElement = document.getElementById(categoryId);
    if (categoryElement) {
      categoryElement.remove();
      this.updateAddButtonState();
    }
  }

  updateAddButtonState() {
    const container = document.getElementById('categoriesContainer');
    const addBtn = document.getElementById('addCategoryBtn');
    if (container && addBtn) {
      const categoryCount = container.children.length;
      addBtn.style.display = categoryCount >= 5 ? 'none' : 'block';
    }
  }

  getCategoriesData() {
    const categories = [];
    const categoryItems = document.querySelectorAll('.category-item');
    
    categoryItems.forEach(item => {
      const titleInput = item.querySelector('.category-title');
      const seatsInput = item.querySelector('.category-seats');
      const priceInput = item.querySelector('.category-price');
      
      if (titleInput && seatsInput && priceInput && titleInput.value.trim() && seatsInput.value && priceInput.value) {
        categories.push({
          title: titleInput.value.trim(),
          seats: parseInt(seatsInput.value),
          TicketPrice: parseFloat(priceInput.value)
        });
      }
    });
    
    return categories;
  }

  // Step navigation
  nextStep() {
    if (!this.validateCurrentStep()) {
      app.showNotification('Please fix the errors before continuing', 'error');
      return;
    }

    if (this.currentStep < this.maxStep) {
      this.currentStep++;
      this.updateStepDisplay();
      
      if (this.currentStep === 4) {
        this.updateEventPreview();
      }
      
      // Scroll to top
      window.scrollTo(0, 0);
    }
  }

  prevStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
      this.updateStepDisplay();
      window.scrollTo(0, 0);
    }
  }

  updateStepDisplay() {
    // Update step indicators
    document.querySelectorAll('.step').forEach((step, index) => {
      step.classList.toggle('active', index + 1 <= this.currentStep);
    });

    // Update step content
    document.querySelectorAll('.step-content').forEach((content, index) => {
      content.style.display = index + 1 === this.currentStep ? 'block' : 'none';
    });
  }

  validateCurrentStep() {
    const currentStepContent = document.querySelector(`.step-content[data-step="${this.currentStep}"]`);
    if (!currentStepContent) return true;

    let isValid = true;

    // Step 1: Basic details
    if (this.currentStep === 1) {
      const requiredFields = currentStepContent.querySelectorAll('[required]');
      requiredFields.forEach(field => {
        if (!this.validateField(field)) {
          isValid = false;
        }
      });

      // Validate date range
      if (!this.validateDateRange()) {
        isValid = false;
      }
    }

    // Step 2: Description
    if (this.currentStep === 2) {
      const descField = document.getElementById('eventDescription');
      if (!this.validateField(descField)) {
        isValid = false;
      }
    }

    // Step 3: Categories
    if (this.currentStep === 3) {
      const categories = this.getCategoriesData();
      if (categories.length === 0) {
        app.showNotification('Please add at least one category', 'error');
        isValid = false;
      }
    }

    // Step 4: Terms agreement
    if (this.currentStep === 4) {
      const termsCheckbox = document.querySelector('input[name="agreeToTerms"]');
      if (termsCheckbox && !termsCheckbox.checked) {
        app.showNotification('Please agree to the Terms of Service', 'error');
        isValid = false;
      }
    }

    return isValid;
  }

  validateField(field) {
    if (!field) return true;
    
    const value = field.value.trim();
    let isValid = true;
    let errorMessage = '';

    // Clear previous errors
    this.clearFieldError(field);

    // Required field validation
    if (field.hasAttribute('required') && !value) {
      errorMessage = 'This field is required';
      isValid = false;
    }

    // Specific field validations
    if (isValid && value) {
      switch (field.name || field.id) {
        case 'name':
        case 'eventTitle':
          if (value.length < 3 || value.length > 100) {
            errorMessage = 'Event name must be 3-100 characters';
            isValid = false;
          }
          break;
          
        case 'description':
        case 'eventDescription':
          if (value.length < 10 || value.length > 1000) {
            errorMessage = 'Description must be 10-1000 characters';
            isValid = false;
          }
          break;
          
        case 'address':
        case 'eventLocation':
          if (value.length < 5 || value.length > 200) {
            errorMessage = 'Address must be 5-200 characters';
            isValid = false;
          }
          break;
          
        case 'startDate':
          const startDate = new Date(value);
          const now = new Date();
          if (startDate <= now) {
            errorMessage = 'Start date must be in the future';
            isValid = false;
          }
          break;
          
        case 'endDate':
          const endDate = new Date(value);
          const startValue = document.getElementById('startDate')?.value;
          if (startValue) {
            const startDateTime = new Date(startValue);
            if (endDate <= startDateTime) {
              errorMessage = 'End date must be after start date';
              isValid = false;
            }
          }
          break;
      }
    }

    if (!isValid) {
      this.showFieldError(field, errorMessage);
    }

    return isValid;
  }

  showFieldError(field, message) {
    field.classList.add('error');
    
    // Remove existing error message
    const existingError = field.parentNode.querySelector('.field-error');
    if (existingError) {
      existingError.remove();
    }

    // Add new error message
    const errorElement = document.createElement('div');
    errorElement.className = 'field-error';
    errorElement.textContent = message;
    field.parentNode.appendChild(errorElement);
  }

  clearFieldError(field) {
    field.classList.remove('error');
    const errorElement = field.parentNode.querySelector('.field-error');
    if (errorElement) {
      errorElement.remove();
    }
  }

  updateEventPreview() {
    const previewContainer = document.getElementById('eventPreview');
    if (!previewContainer) return;

    const formData = this.getFormData();
    const categories = this.getCategoriesData();

    previewContainer.innerHTML = `
      <div style="display: flex; gap: var(--space-lg); margin-bottom: var(--space-lg); flex-wrap: wrap;">
        <div style="width: 200px; height: 120px; background: var(--accent); border-radius: var(--radius-md); display: flex; align-items: center; justify-content: center; color: white; font-size: 2rem;">
          🎉
        </div>
        
        <div style="flex: 1; min-width: 250px;">
          <h4 style="margin-bottom: var(--space-sm);">${formData.get('name') || 'Event Name'}</h4>
          <p style="margin-bottom: var(--space-md); color: var(--muted);">
            📅 ${formData.get('startDate') ? new Date(formData.get('startDate')).toLocaleString() : 'Start Date'}<br>
            📅 ${formData.get('endDate') ? new Date(formData.get('endDate')).toLocaleString() : 'End Date'}<br>
            📍 ${formData.get('address') || 'Address'}<br>
            👥 ${categories.reduce((total, cat) => total + cat.seats, 0)} total seats
          </p>
          
          <div style="display: flex; gap: var(--space-sm); flex-wrap: wrap;">
            ${categories.map(cat => `<span class="event-tag">${cat.title}: ${cat.seats} seats - $${cat.TicketPrice}</span>`).join('')}
          </div>
        </div>
      </div>
      
      <div style="margin-bottom: var(--space-lg);">
        <h5 style="margin-bottom: var(--space-sm);">Description</h5>
        <p style="line-height: 1.6; color: var(--muted);">
          ${formData.get('description') || 'Event description will appear here...'}
        </p>
      </div>
    `;
  }

  getFormData() {
    return new FormData(document.getElementById('createEventForm'));
  }

  async handleFormSubmit(event) {
    event.preventDefault();
    
    if (!this.validateCurrentStep()) {
      app.showNotification('Please fix the errors before submitting', 'error');
      return;
    }

    const formData = this.getFormData();
    const categories = this.getCategoriesData();
    
    if (categories.length === 0) {
      app.showNotification('Please add at least one category', 'error');
      return;
    }

    try {
      await this.createEvent(formData, categories);
    } catch (error) {
      console.error('Error creating event:', error);
      app.showNotification('Error creating event. Please try again.', 'error');
    }
  }

  async createEvent(formData, categories) {
    const user = app.getCurrentUser();
    const token = localStorage.getItem('eventify_token');

    if (!token) {
      app.showNotification('Authentication required. Please log in again.', 'error');
      window.location.href = 'login.html?redirect=create-event.html';
      return;
    }

    // Show loading state
    const submitBtn = document.querySelector('button[type="submit"]');
    const originalText = submitBtn.textContent;
    submitBtn.textContent = 'Creating Event...';
    submitBtn.disabled = true;

    try {
      // Prepare form data for API
      const apiFormData = new FormData();
      
      // Required fields
      apiFormData.append('name', formData.get('name'));
      apiFormData.append('description', formData.get('description'));
      apiFormData.append('address', formData.get('address'));
      
      // Convert datetime-local to ISO 8601 format
      const startDate = new Date(formData.get('startDate')).toISOString();
      const endDate = new Date(formData.get('endDate')).toISOString();
      apiFormData.append('startDate', startDate);
      apiFormData.append('endDate', endDate);
      
      // Categories as JSON string
      apiFormData.append('categoriesJson', JSON.stringify(categories));
      
      // Photo (if provided)
      const photoFile = document.getElementById('eventImage').files[0];
      if (photoFile) {
        apiFormData.append('photo', photoFile);
      }

      // Call API
      const response = await fetch(`${this.apiBaseUrl}/api/Events`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        },
        body: apiFormData
      });

      if (response.ok) {
        const result = await response.json();
        app.showNotification('Event created successfully!', 'success');
        this.showSuccessModal(result);
      } else {
        const errorData = await response.json().catch(() => null);
        let errorMessage = 'Failed to create event';
        
        if (errorData) {
          if (errorData.errors && Array.isArray(errorData.errors)) {
            errorMessage = errorData.errors[0];
          } else if (errorData.message) {
            errorMessage = errorData.message;
          } else if (typeof errorData === 'string') {
            errorMessage = errorData;
          }
        }
        
        app.showNotification(errorMessage, 'error');
      }

    } catch (error) {
      console.error('API call failed:', error);
      if (error.name === 'TypeError' && error.message.includes('fetch')) {
        app.showNotification('Unable to connect to server. Please check your connection and try again.', 'error');
      } else {
        app.showNotification('An unexpected error occurred. Please try again.', 'error');
      }
    } finally {
      // Restore button state
      submitBtn.textContent = originalText;
      submitBtn.disabled = false;
    }
  }

  showSuccessModal(eventData) {
    const modal = document.getElementById('successModal');
    const actionsContainer = document.getElementById('eventActions');
    
    if (actionsContainer && eventData) {
      actionsContainer.innerHTML = `
        <a href="dashboard.html" class="btn btn-primary">
          📊 Go to Dashboard
        </a>
        <button 
          class="btn btn-outline" 
          onclick="createEventPage.shareEvent('${eventData.id || eventData.eventId}')"
        >
          📤 Share Event
        </button>
      `;
    }
    
    app.openModal('successModal');
  }

  closeSuccessModal() {
    app.closeModal(document.getElementById('successModal'));
    // Redirect to dashboard
    window.location.href = 'dashboard.html';
  }

  shareEvent(eventId) {
    const shareUrl = `${window.location.origin}/event.html?id=${eventId}`;
    
    if (navigator.share) {
      navigator.share({
        title: 'Check out this event on Eventify!',
        url: shareUrl
      });
    } else {
      // Fallback: copy to clipboard
      navigator.clipboard.writeText(shareUrl).then(() => {
        app.showNotification('Event link copied to clipboard!', 'success');
      }).catch(() => {
        // Final fallback: show URL in alert
        alert(`Share this event: ${shareUrl}`);
      });
    }
  }
}

// Form styling for steps
const style = document.createElement('style');
style.textContent = `
  .create-steps {
    margin-bottom: var(--space-2xl);
  }
  
  .step {
    display: flex;
    flex-direction: column;
    align-items: center;
    opacity: 0.5;
    transition: opacity var(--anim-speed) ease;
  }
  
  .step.active {
    opacity: 1;
  }
  
  .step-number {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background: var(--border);
    color: var(--muted);
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 600;
    margin-bottom: var(--space-xs);
    transition: all var(--anim-speed) ease;
  }
  
  .step.active .step-number {
    background: var(--accent);
    color: white;
  }
  
  .step-label {
    font-size: var(--font-size-sm);
    color: var(--muted);
    text-align: center;
  }
  
  .step-divider {
    width: 60px;
    height: 2px;
    background: var(--border);
    margin: 20px 0;
  }
  
  .step-content {
    display: none;
  }
  
  .step-content.active {
    display: block;
  }
  
  .form-actions {
    padding-top: var(--space-xl);
    border-top: 1px solid var(--border);
    display: flex;
    gap: var(--space-md);
    justify-content: flex-end;
  }
  
  .radio-group,
  .checkbox-group {
    display: flex;
    flex-direction: column;
    gap: var(--space-sm);
  }
  
  .radio-option,
  .checkbox-option {
    display: flex;
    align-items: flex-start;
    gap: var(--space-sm);
    padding: var(--space-sm);
    border: 2px solid var(--border);
    border-radius: var(--radius-md);
    cursor: pointer;
    transition: border-color var(--anim-speed) ease;
  }
  
  .radio-option:hover,
  .checkbox-option:hover {
    border-color: var(--accent);
  }
  
  .radio-option input,
  .checkbox-option input {
    margin: 0;
    margin-top: 2px;
  }
  
  .field-error {
    color: var(--danger);
    font-size: var(--font-size-sm);
    margin-top: var(--space-xs);
  }
  
  .form-input.error,
  .form-select.error,
  .form-textarea.error {
    border-color: var(--danger);
    box-shadow: 0 0 0 2px rgba(239, 68, 68, 0.1);
  }
  
  /* Category management styles */
  .category-item {
    position: relative;
    background: var(--card-bg);
    transition: all var(--anim-speed) ease;
  }
  
  .category-item:hover {
    border-color: var(--accent) !important;
  }
  
  .category-title,
  .category-seats {
    background: var(--bg);
  }
  
  /* File input styling */
  input[type="file"] {
    padding: var(--space-sm) !important;
    border: 2px dashed var(--border) !important;
    background: var(--bg) !important;
    border-radius: var(--radius-md);
    cursor: pointer;
    transition: all var(--anim-speed) ease;
  }
  
  input[type="file"]:hover {
    border-color: var(--accent) !important;
    background: var(--card-bg) !important;
  }
  
  input[type="file"]:focus {
    outline: none;
    border-color: var(--accent) !important;
    box-shadow: 0 0 0 2px rgba(124, 92, 255, 0.1) !important;
  }
  
  /* Event tag styles for preview */
  .event-tag {
    display: inline-block;
    padding: 4px 8px;
    background: var(--border);
    color: var(--text);
    font-size: 0.75rem;
    border-radius: 12px;
    font-weight: 500;
  }
  
  @media (max-width: 900px) {
    .create-steps {
      display: grid;
      grid-template-columns: 1fr auto 1fr auto 1fr auto 1fr;
      gap: var(--space-sm);
    }
    
    .step-divider {
      width: 30px;
      margin: 20px 0;
    }
    
    .form-actions {
      flex-direction: column;
    }
    
    .form-row {
      flex-direction: column;
    }
    
    .category-item .form-row {
      flex-direction: row;
      align-items: end;
    }
    
    .category-item .form-group:last-child {
      margin-top: 0;
    }
  }
  
  @media (max-width: 600px) {
    .create-steps {
      display: flex;
      flex-wrap: wrap;
      justify-content: center;
      gap: var(--space-md);
    }
    
    .step-divider {
      display: none;
    }
    
    .category-item .form-row {
      flex-direction: column;
    }
    
    .category-item .form-group {
      flex: 1 !important;
    }
  }
`;
document.head.appendChild(style);

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
  if (window.EventifyData) {
    window.createEventPage = new CreateEventPage();
  } else {
    console.error('EventifyData not loaded');
  }
});