// Authentication JavaScript - Real API Integration
// Handles login, register with https://localhost:7105/ API

class AuthPage {
  constructor() {
    this.currentPage = this.getCurrentPage();
    this.apiBaseUrl = window.API_BASE_URL || 'https://eventify.runasp.net';
    this.init();
  }

  init() {
    this.setupForms();
    this.setupPasswordToggles();
    this.setupValidation();
    this.handleRedirect();
  }

  getCurrentPage() {
    const path = window.location.pathname;
    if (path.includes('login')) return 'login';
    if (path.includes('register')) return 'register';
    return 'login';
  }

  setupForms() {
    // Login form
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
      loginForm.addEventListener('submit', (e) => this.handleLogin(e));
    }

    // Register form
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
      registerForm.addEventListener('submit', (e) => this.handleRegister(e));
    }

    // Demo credentials button (disabled for real API)
    const demoBtn = document.getElementById('demoBtn');
    if (demoBtn) {
      demoBtn.addEventListener('click', () => this.fillDemoCredentials());
    }

    // Password confirmation validation
    const confirmPasswordInput = document.getElementById('confirmPassword');
    if (confirmPasswordInput) {
      confirmPasswordInput.addEventListener('blur', () => this.validatePasswordConfirmation());
    }
  }

  setupPasswordToggles() {
    const toggles = document.querySelectorAll('.password-toggle');
    toggles.forEach(toggle => {
      toggle.addEventListener('click', (e) => {
        e.preventDefault();
        const input = toggle.previousElementSibling;
        const type = input.type === 'password' ? 'text' : 'password';
        input.type = type;
        toggle.textContent = type === 'password' ? '👁️' : '🙈';
      });
    });
  }

  setupValidation() {
    const inputs = document.querySelectorAll('.form-input');
    inputs.forEach(input => {
      input.addEventListener('blur', () => this.validateField(input));
      input.addEventListener('input', () => this.clearErrors(input));
    });

    // Password strength indicator
    const passwordInput = document.getElementById('password');
    if (passwordInput) {
      passwordInput.addEventListener('input', () => {
        this.displayPasswordStrength(passwordInput.value, 'passwordStrength');
      });
    }
  }

  validateField(field) {
    const value = field.value.trim();
    let isValid = true;
    let message = '';

    // Clear previous errors
    this.clearErrors(field);

    // Required validation
    if (field.hasAttribute('required') && !value) {
      isValid = false;
      message = 'This field is required';
    }

    // Email validation
    if (field.type === 'email' && value) {
      const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailPattern.test(value)) {
        isValid = false;
        message = 'Please enter a valid email address';
      }
    }

    // Password validation
    if (field.name === 'password' && value) {
      if (value.length < 6 || value.length > 50) {
        isValid = false;
        message = 'Password must be 6-50 characters long';
      } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(value)) {
        isValid = false;
        message = 'Password must contain at least one lowercase, uppercase, and digit';
      }
    }

    // Phone validation (Egyptian mobile format)
    if (field.name === 'phone' && value) {
      const phonePattern = /^(010|011|012|015)[0-9]{8}$/;
      if (!phonePattern.test(value)) {
        isValid = false;
        message = 'Phone must be 11 digits starting with 010, 011, 012, or 015';
      }
    }

    if (!isValid) {
      this.showFieldError(field, message);
    }

    return isValid;
  }

  validatePasswordConfirmation() {
    const password = document.getElementById('password')?.value;
    const confirmPassword = document.getElementById('confirmPassword')?.value;
    const confirmField = document.getElementById('confirmPassword');

    if (confirmField && password && confirmPassword) {
      if (password !== confirmPassword) {
        this.showFieldError(confirmField, 'Passwords do not match');
        return false;
      } else {
        this.clearErrors(confirmField);
        return true;
      }
    }
    return true;
  }

  showFieldError(field, message) {
    field.classList.add('error');
    
    // Remove existing error
    const existingError = field.parentNode.querySelector('.field-error');
    if (existingError) {
      existingError.remove();
    }

    // Add new error
    const errorDiv = document.createElement('div');
    errorDiv.className = 'field-error';
    errorDiv.textContent = message;
    field.parentNode.appendChild(errorDiv);
  }

  clearErrors(field) {
    field.classList.remove('error');
    const errorDiv = field.parentNode.querySelector('.field-error');
    if (errorDiv) {
      errorDiv.remove();
    }
  }

  // Handle login with real API
  async handleLogin(event) {
    event.preventDefault();
    
    const form = event.target;
    const submitBtn = document.getElementById('loginBtn');
    const originalText = submitBtn.textContent;

    // Validate form
    if (!app.validateForm(form)) {
      app.showNotification('Please fix the errors and try again', 'error');
      return;
    }

    try {
      // Show loading state
      submitBtn.textContent = 'Signing In...';
      submitBtn.disabled = true;

      // Get form data
      const formData = new FormData(form);
      const email = formData.get('email').toLowerCase().trim();
      const password = formData.get('password');

      // Call real API
      const response = await fetch(`${this.apiBaseUrl}/api/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email: email,
          password: password
        })
      });

      if (response.ok) {
        const data = await response.json();
        
        if (data.success && data.token && data.user) {
          const user = {
            id: data.user.id,
            name: data.user.name,
            email: data.user.email,
            phone: data.user.phoneNumber,
            role: data.user.role,
            token: data.token,
            expiresAt: data.expiresAt,
            loginTime: new Date().toISOString(),
            avatar: `https://ui-avatars.com/api/?name=${encodeURIComponent(data.user.name)}&background=7c5cff&color=ffffff&size=200`
          };
          
          // Store token and user info
          localStorage.setItem('eventify_token', data.token);
          app.setCurrentUser(user);
          
          app.showNotification(`Welcome back, ${user.name}!`, 'success');
          
          // Handle redirect
          this.handleSuccessfulAuth();
        } else {
          app.showNotification(data.message || 'Invalid response from server', 'error');
        }
      } else {
        const data = await response.json().catch(() => ({ message: 'Invalid email or password' }));
        app.showNotification(data.message || 'Invalid email or password', 'error');
      }

    } catch (error) {
      console.error('Login error:', error);
      app.showNotification('Login failed. Please check your connection and try again.', 'error');
    } finally {
      // Restore button state
      submitBtn.textContent = originalText;
      submitBtn.disabled = false;
    }
  }

  // Handle register with real API
  async handleRegister(event) {
    event.preventDefault();
    
    const form = event.target;
    const submitBtn = document.getElementById('registerBtn');
    const originalText = submitBtn.textContent;

    // Validate form
    if (!app.validateForm(form) || !this.validatePasswordConfirmation()) {
      app.showNotification('Please fix the errors and try again', 'error');
      return;
    }

    // Check terms agreement
    if (!document.getElementById('agreeTerms').checked) {
      app.showNotification('Please agree to the Terms of Service and Privacy Policy', 'error');
      return;
    }

    try {
      // Show loading state
      submitBtn.textContent = 'Creating Account...';
      submitBtn.disabled = true;

      // Get form data
      const formData = new FormData(form);
      const firstName = formData.get('firstName').trim();
      const lastName = formData.get('lastName').trim();
      const email = formData.get('email').toLowerCase().trim();
      const phone = formData.get('phone').trim();
      const password = formData.get('password');

      // Call real API
      const response = await fetch(`${this.apiBaseUrl}/api/auth/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          name: `${firstName} ${lastName}`,
          email: email,
          password: password,
          phone: phone
        })
      });

      if (response.ok) {
        const data = await response.json();
        
        if (data.success) {
          app.showNotification(data.message || 'Account created successfully! Please log in with your credentials.', 'success');
          
          // Switch to login form after successful registration
          setTimeout(() => {
            // If on register page, redirect to login
            if (this.currentPage === 'register') {
              window.location.href = 'login.html';
            }
            
            // Pre-fill email field if on same page
            const loginEmailField = document.getElementById('loginEmail');
            if (loginEmailField) {
              loginEmailField.value = email;
            }
          }, 1500);
        } else {
          const errorMessage = data.errors && data.errors.length > 0 ? data.errors[0] : data.message;
          app.showNotification(errorMessage, 'error');
        }
        
      } else {
        const data = await response.json().catch(() => ({ message: 'Registration failed. Please try again.' }));
        const errorMessage = data.errors && data.errors.length > 0 ? data.errors[0] : data.message;
        app.showNotification(errorMessage, 'error');
      }
      
    } catch (error) {
      console.error('Registration error:', error);
      app.showNotification('Registration failed. Please check your connection and try again.', 'error');
    } finally {
      // Restore button state
      submitBtn.textContent = originalText;
      submitBtn.disabled = false;
    }
  }

  // Utility method to decode JWT payload
  decodeJWTPayload(token) {
    try {
      const payload = token.split('.')[1];
      const decoded = atob(payload);
      return JSON.parse(decoded);
    } catch (error) {
      console.error('Error decoding JWT:', error);
      return null;
    }
  }

  handleSuccessfulAuth() {
    // Get redirect URL from query params or default to dashboard
    const params = new URLSearchParams(window.location.search);
    const redirect = params.get('redirect') || 'dashboard.html';
    
    // Delay redirect to show success message
    setTimeout(() => {
      window.location.href = redirect;
    }, 1000);
  }

  handleRedirect() {
    // If user is already logged in, redirect them
    const user = app.getCurrentUser();
    if (user) {
      const params = new URLSearchParams(window.location.search);
      const redirect = params.get('redirect') || 'dashboard.html';
      window.location.href = redirect;
    }
  }

  fillDemoCredentials() {
    app.showNotification('Demo accounts are disabled. Please register a new account or use your existing credentials.', 'info');
  }

  checkPasswordStrength(password) {
    let strength = 0;
    
    if (password.length >= 8) strength++;
    if (/[a-z]/.test(password)) strength++;
    if (/[A-Z]/.test(password)) strength++;
    if (/[0-9]/.test(password)) strength++;
    if (/[^A-Za-z0-9]/.test(password)) strength++;
    
    return strength;
  }

  displayPasswordStrength(password, targetElementId) {
    const targetElement = document.getElementById(targetElementId);
    if (!targetElement) return;
    
    const strength = this.checkPasswordStrength(password);
    const strengthLevels = ['Very Weak', 'Weak', 'Fair', 'Good', 'Strong'];
    const strengthColors = ['#ef4444', '#f59e0b', '#eab308', '#22c55e', '#10b981'];
    
    if (password.length === 0) {
      targetElement.innerHTML = '';
      return;
    }
    
    const strengthText = strengthLevels[Math.min(strength, 4)];
    const strengthColor = strengthColors[Math.min(strength, 4)];
    
    targetElement.innerHTML = `
      <div style="margin-top: 8px;">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 4px;">
          <span style="font-size: 0.875rem; color: var(--muted);">Password Strength:</span>
          <span style="font-size: 0.875rem; color: ${strengthColor}; font-weight: 600;">${strengthText}</span>
        </div>
        <div style="width: 100%; height: 4px; background: var(--border); border-radius: 2px; overflow: hidden;">
          <div style="width: ${(strength / 5) * 100}%; height: 100%; background: ${strengthColor}; transition: all 0.3s ease;"></div>
        </div>
      </div>
    `;
  }
}

// Initialize authentication page
document.addEventListener('DOMContentLoaded', () => {
  window.authPage = new AuthPage();
});