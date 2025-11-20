// Create Event Page JavaScript
class CreateEventPage {
  constructor() {
    this.currentStep = 1;
    this.maxStep = 4;
    this.eventData = {};
    
    this.init();
  }

  init() {
    // Check authentication - require organizer access
    const user = app.getCurrentUser();
    if (!user) {
      app.showNotification('Please log in to create events', 'warning');
      setTimeout(() => {
        window.location.href = 'login.html?redirect=create-event.html';
      }, 1000);
      return;
    }

    // Check if user can create events (organizers only)
    if (user.userType === 'attendee') {
      app.showNotification('Only event organizers can create events. Please contact us to upgrade your account.', 'warning');
      setTimeout(() => {
        window.location.href = 'about.html';
      }, 2000);
      return;
    }

    this.setupEventHandlers();
    this.setupFormValidation();
    this.setMinDate();
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

    // Location format handling
    const formatSelect = document.getElementById('eventFormat');
    if (formatSelect) {
      formatSelect.addEventListener('change', (e) => this.handleFormatChange(e.target.value));
    }

    // Date/time validation
    const dateInput = document.getElementById('eventDate');
    const timeInput = document.getElementById('eventTime');
    const deadlineInput = document.getElementById('registrationDeadline');

    if (dateInput && timeInput && deadlineInput) {
      [dateInput, timeInput].forEach(input => {
        input.addEventListener('change', () => this.updateRegistrationDeadline());
      });
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
    const previewFields = ['eventTitle', 'eventCategory', 'eventDate', 'eventTime', 'eventLocation', 'eventDescription'];
    
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

  setMinDate() {
    const dateInput = document.getElementById('eventDate');
    const deadlineInput = document.getElementById('registrationDeadline');
    
    if (dateInput) {
      // Set minimum date to tomorrow
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);
      dateInput.min = tomorrow.toISOString().split('T')[0];
    }

    if (deadlineInput) {
      // Set minimum datetime to now
      const now = new Date();
      deadlineInput.min = now.toISOString().slice(0, 16);
    }
  }

  handleFormatChange(format) {
    const locationInput = document.getElementById('eventLocation');
    const locationHint = document.getElementById('locationHint');
    
    if (!locationInput || !locationHint) return;

    switch (format) {
      case 'virtual':
        locationInput.placeholder = 'Platform name (e.g., Zoom, Teams, YouTube Live)';
        locationHint.textContent = 'Specify the virtual platform or provide a link';
        break;
      case 'hybrid':
        locationInput.placeholder = 'Venue name + Virtual platform';
        locationHint.textContent = 'Include both physical venue and virtual platform details';
        break;
      default:
        locationInput.placeholder = 'Enter venue name or address';
        locationHint.textContent = 'Provide the full address or venue name';
    }
  }

  updateRegistrationDeadline() {
    const dateInput = document.getElementById('eventDate');
    const timeInput = document.getElementById('eventTime');
    const deadlineInput = document.getElementById('registrationDeadline');
    
    if (dateInput.value && timeInput.value && deadlineInput) {
      // Set default deadline to 24 hours before event
      const eventDateTime = new Date(`${dateInput.value}T${timeInput.value}`);
      eventDateTime.setHours(eventDateTime.getHours() - 24);
      
      // Don't set deadline in the past
      const now = new Date();
      if (eventDateTime > now) {
        deadlineInput.value = eventDateTime.toISOString().slice(0, 16);
      }
      
      // Set max deadline to event start time
      const maxDeadline = new Date(`${dateInput.value}T${timeInput.value}`);
      deadlineInput.max = maxDeadline.toISOString().slice(0, 16);
    }
  }

  togglePricing() {
    const pricingSection = document.getElementById('pricingSection');
    const ticketType = document.querySelector('input[name="ticketType"]:checked').value;
    const priceInput = document.getElementById('ticketPrice');
    
    if (pricingSection) {
      pricingSection.style.display = ticketType === 'paid' ? 'block' : 'none';
      
      if (priceInput) {
        priceInput.required = ticketType === 'paid';
        if (ticketType === 'free') {
          priceInput.value = '0';
        }
      }
    }
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

    const requiredFields = currentStepContent.querySelectorAll('[required]');
    let isValid = true;

    requiredFields.forEach(field => {
      if (!this.validateField(field)) {
        isValid = false;
      }
    });

    return isValid;
  }

  validateField(field) {
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
      switch (field.id) {
        case 'eventTitle':
          if (value.length < 5) {
            errorMessage = 'Event title must be at least 5 characters';
            isValid = false;
          }
          break;
          
        case 'eventDate':
          const eventDate = new Date(value);
          const tomorrow = new Date();
          tomorrow.setDate(tomorrow.getDate() + 1);
          tomorrow.setHours(0, 0, 0, 0);
          
          if (eventDate < tomorrow) {
            errorMessage = 'Event date must be at least tomorrow';
            isValid = false;
          }
          break;
          
        case 'eventDescription':
          if (value.length < 50) {
            errorMessage = 'Description should be at least 50 characters';
            isValid = false;
          }
          break;
          
        case 'eventCapacity':
          const capacity = parseInt(value);
          if (capacity < 1 || capacity > 10000) {
            errorMessage = 'Capacity must be between 1 and 10,000';
            isValid = false;
          }
          break;
          
        case 'ticketPrice':
          const price = parseFloat(value);
          if (price < 0) {
            errorMessage = 'Price cannot be negative';
            isValid = false;
          }
          break;
          
        case 'eventImage':
          if (value && !this.isValidUrl(value)) {
            errorMessage = 'Please enter a valid URL';
            isValid = false;
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
    const isVirtual = formData.format === 'virtual';
    const isFree = formData.ticketType === 'free';

    previewContainer.innerHTML = `
      <div style="display: flex; gap: var(--space-lg); margin-bottom: var(--space-lg); flex-wrap: wrap;">
        <img 
          src="${formData.image || 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=300&h=200&fit=crop'}" 
          alt="Event preview"
          style="width: 200px; height: 120px; object-fit: cover; border-radius: var(--radius-md);"
          onerror="this.src='https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=300&h=200&fit=crop'"
        >
        
        <div style="flex: 1; min-width: 250px;">
          <h4 style="margin-bottom: var(--space-sm);">${formData.title || 'Event Title'}</h4>
          <p style="margin-bottom: var(--space-md); color: var(--muted);">
            📅 ${formData.date ? EventifyData.formatDate(formData.date) : 'Date'} 
            ${formData.time ? 'at ' + EventifyData.formatTime(formData.time) : ''}<br>
            ${isVirtual ? '🌐' : '📍'} ${formData.location || 'Location'}<br>
            👥 ${formData.capacity || 'Capacity'} attendees
          </p>
          
          <div style="display: flex; gap: var(--space-sm); flex-wrap: wrap;">
            <span class="event-tag">${formData.category || 'Category'}</span>
            <span class="event-tag">${formData.format || 'Format'}</span>
            <span class="event-tag" style="background: var(--accent); color: white;">
              ${isFree ? 'Free' : (formData.price ? app.formatCurrency(formData.price) : 'Paid')}
            </span>
          </div>
        </div>
      </div>
      
      <div style="margin-bottom: var(--space-lg);">
        <h5 style="margin-bottom: var(--space-sm);">Description</h5>
        <p style="line-height: 1.6; color: var(--muted);">
          ${formData.description || 'Event description will appear here...'}
        </p>
      </div>
      
      ${formData.agenda ? `
        <div style="margin-bottom: var(--space-lg);">
          <h5 style="margin-bottom: var(--space-sm);">Agenda</h5>
          <p style="line-height: 1.6; color: var(--muted); white-space: pre-line;">
            ${formData.agenda}
          </p>
        </div>
      ` : ''}
      
      ${formData.requirements ? `
        <div style="margin-bottom: var(--space-lg);">
          <h5 style="margin-bottom: var(--space-sm);">Requirements</h5>
          <p style="line-height: 1.6; color: var(--muted); white-space: pre-line;">
            ${formData.requirements}
          </p>
        </div>
      ` : ''}
    `;
  }

  getFormData() {
    const formData = new FormData(document.getElementById('createEventForm'));
    const data = {};
    
    for (let [key, value] of formData.entries()) {
      data[key] = value;
    }
    
    // Get radio button values
    const ticketType = document.querySelector('input[name="ticketType"]:checked');
    if (ticketType) {
      data.ticketType = ticketType.value;
    }
    
    const publishStatus = document.querySelector('input[name="publishStatus"]:checked');
    if (publishStatus) {
      data.publishStatus = publishStatus.value;
    }

    return data;
  }

  async handleFormSubmit(event) {
    event.preventDefault();
    
    if (!this.validateCurrentStep()) {
      app.showNotification('Please fix the errors before submitting', 'error');
      return;
    }

    const formData = this.getFormData();
    
    // Create event object
    const eventObject = this.createEventObject(formData);
    
    try {
      // In a real app, this would be an API call
      await this.saveEvent(eventObject);
      
      // Show success
      this.showSuccessModal(eventObject);
      
    } catch (error) {
      console.error('Error creating event:', error);
      app.showNotification('Error creating event. Please try again.', 'error');
    }
  }

  createEventObject(formData) {
    const user = app.getCurrentUser();
    const now = new Date();
    
    // Generate a unique ID (in a real app, this would be done server-side)
    const id = Date.now() + Math.random().toString(36).substr(2, 9);
    
    const eventObject = {
      id: id,
      title: formData.title,
      description: formData.description,
      category: formData.category,
      date: formData.date,
      time: formData.time,
      duration: formData.duration,
      location: formData.location,
      format: formData.format,
      image: formData.image || 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&h=600&fit=crop',
      price: formData.ticketType === 'free' ? 0 : parseFloat(formData.price || 0),
      currency: formData.currency || 'USD',
      capacity: parseInt(formData.capacity),
      ticketsAvailable: parseInt(formData.capacity),
      registrationDeadline: formData.registrationDeadline,
      requireApproval: formData.requireApproval === 'true',
      agenda: formData.agenda,
      requirements: formData.requirements,
      tags: formData.tags ? formData.tags.split(',').map(tag => tag.trim()) : [],
      status: formData.publishStatus || 'published',
      
      // Organizer info
      organizerId: user.id || user.email,
      organizerName: user.name,
      organizerEmail: user.email,
      organizerAvatar: user.avatar,
      
      // Metadata
      createdAt: now.toISOString(),
      updatedAt: now.toISOString()
    };

    return eventObject;
  }

  async saveEvent(eventObject) {
    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    // Save to localStorage (simulating database)
    const customEvents = JSON.parse(localStorage.getItem('customEvents') || '[]');
    customEvents.push(eventObject);
    localStorage.setItem('customEvents', JSON.stringify(customEvents));
    
    // Also add to main events data if published
    if (eventObject.status === 'published') {
      // In a real app, this would be handled server-side
      const mainEvents = JSON.parse(localStorage.getItem('eventify_events') || '[]');
      mainEvents.push(eventObject);
      localStorage.setItem('eventify_events', JSON.stringify(mainEvents));
      
      // Update EventifyData
      if (window.EventifyData && EventifyData.customEvents) {
        EventifyData.customEvents.push(eventObject);
      }
    }
    
    return eventObject;
  }

  showSuccessModal(eventObject) {
    const modal = document.getElementById('successModal');
    const actionsContainer = document.getElementById('eventActions');
    
    if (actionsContainer) {
      actionsContainer.innerHTML = `
        <a href="event.html?id=${eventObject.id}" class="btn btn-primary">
          👁️ View Event
        </a>
        <a href="dashboard.html" class="btn btn-outline">
          📊 Go to Dashboard
        </a>
        <button 
          class="btn btn-outline" 
          onclick="createEventPage.shareEvent('${eventObject.id}')"
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

  isValidUrl(string) {
    try {
      new URL(string);
      return true;
    } catch (_) {
      return false;
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