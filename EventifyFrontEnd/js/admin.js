// Admin Panel JavaScript
const API_BASE_URL = 'https://eventify.runasp.net/api';

const adminPanel = {
  currentTab: 'users',
  allUsers: [],
  allEvents: [],
  allCategories: [],
  allBookings: [],
  allPayments: [],

  // Handle auth errors globally
  handleAuthError(response) {
    if (response.status === 401) {
      showToast('Session expired. Please login again.', 'error');
      localStorage.removeItem('eventify_token');
      localStorage.removeItem('eventifyUser');
      setTimeout(() => {
        window.location.href = 'login.html?redirect=admin-panel.html';
      }, 1500);
      return true;
    }
    if (response.status === 403) {
      showToast('Access denied. Admin privileges required.', 'error');
      setTimeout(() => {
        window.location.href = 'index.html';
      }, 1500);
      return true;
    }
    return false;
  },

  // Initialize Admin Panel
  async init() {
    console.log('Admin panel initializing...');
    
    // Check if user is admin
    const userJson = localStorage.getItem('eventifyUser');
    const token = localStorage.getItem('eventify_token');
    
    console.log('User JSON from localStorage:', userJson);
    console.log('Token exists:', !!token);
    
    if (!userJson || !token) {
      console.log('No user or token found, redirecting to index...');
      alert('Please login first. You will be redirected to the home page.');
      window.location.href = 'index.html';
      return;
    }
    
    const user = JSON.parse(userJson);
    console.log('Parsed user:', user);
    console.log('User role:', user.role, 'Type:', typeof user.role);
    
    // Check if user is admin (role can be 0 or 'Admin')
    const isAdmin = user.role === 0 || user.role === '0' || user.role === 'Admin' || user.role === 'admin';
    
    if (!user || !isAdmin) {
      console.log('User is not admin (role should be 0 or "Admin", got:', user.role, ')');
      alert('Access denied. Admin privileges required. You will be redirected to the home page.');
      window.location.href = 'index.html';
      return;
    }

    console.log('Admin user verified successfully!');
    this.setupEventListeners();
    await this.loadAllData();
  },

  // Setup Event Listeners
  setupEventListeners() {
    // Tab switching
    document.querySelectorAll('.admin-tab').forEach(tab => {
      tab.addEventListener('click', (e) => {
        this.switchTab(e.target.dataset.tab);
      });
    });

    // Search functionality
    document.getElementById('userSearch')?.addEventListener('input', (e) => {
      this.filterUsers(e.target.value);
    });

    document.getElementById('eventSearch')?.addEventListener('input', (e) => {
      this.filterEvents(e.target.value);
    });

    document.getElementById('bookingSearch')?.addEventListener('input', (e) => {
      this.filterBookings(e.target.value);
    });

    document.getElementById('bookingStatusFilter')?.addEventListener('change', (e) => {
      this.filterBookings(document.getElementById('bookingSearch').value);
    });

    document.getElementById('paymentSearch')?.addEventListener('input', (e) => {
      this.filterPayments(e.target.value);
    });

    document.getElementById('paymentStatusFilter')?.addEventListener('change', (e) => {
      this.filterPayments(document.getElementById('paymentSearch').value);
    });
  },

  // Switch between tabs
  switchTab(tabName) {
    // Update active tab
    document.querySelectorAll('.admin-tab').forEach(tab => {
      tab.classList.toggle('active', tab.dataset.tab === tabName);
    });

    // Update active content
    document.querySelectorAll('.admin-content').forEach(content => {
      content.classList.remove('active');
    });
    document.getElementById(`${tabName}-content`).classList.add('active');

    this.currentTab = tabName;
  },

  // Load all data
  async loadAllData() {
    try {
      await Promise.all([
        this.loadUsers(),
        this.loadEvents(),
        this.loadCategories(),
        this.loadBookings(),
        this.loadPayments(),
        this.loadStatistics()
      ]);
    } catch (error) {
      console.error('Error loading data:', error);
      showToast('Error loading data. Please refresh the page.', 'error');
    }
  },

  // Load Statistics from API
  async loadStatistics() {
    try {
      const token = localStorage.getItem('eventify_token');
      const response = await fetch(`${API_BASE_URL}/admin/statistics`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (this.handleAuthError(response)) return;
      if (!response.ok) throw new Error('Failed to fetch statistics');

      const result = await response.json();
      const stats = result.success ? result.data : result;
      
      console.log('Statistics from API:', stats);
      console.log('Total Categories from API:', stats.totalCategories);
      
      document.getElementById('totalUsers').textContent = stats.totalUsers || 0;
      document.getElementById('totalEvents').textContent = stats.totalEvents || 0;
      document.getElementById('totalCategories').textContent = stats.totalCategories || 0;
      document.getElementById('totalBookings').textContent = stats.totalBookings || 0;
      document.getElementById('platformRevenue').textContent = `${(stats.totalRevenue || 0).toFixed(2)} EGP`;
      
      // Additional stats if elements exist
      if (document.getElementById('pendingPayments')) {
        document.getElementById('pendingPayments').textContent = stats.pendingPayments || 0;
      }
      if (document.getElementById('activeEvents')) {
        document.getElementById('activeEvents').textContent = stats.activeEvents || 0;
      }
    } catch (error) {
      console.error('Error loading statistics:', error);
      // Fallback to client-side calculation
      document.getElementById('totalUsers').textContent = this.allUsers.length;
      document.getElementById('totalEvents').textContent = this.allEvents.length;
      document.getElementById('totalCategories').textContent = this.allCategories.length; // EventCategory enum has 11 values
      document.getElementById('totalBookings').textContent = this.allBookings.length;
      const platformRevenue = this.allPayments
        .filter(p => p.status === 0)
        .reduce((sum, p) => sum + (p.totalPrice || p.amount || 0), 0);
      document.getElementById('platformRevenue').textContent = `${platformRevenue.toFixed(2)} EGP`;
    }
  },

  // Load Users
  async loadUsers(searchTerm = '') {
    try {
      const token = localStorage.getItem('eventify_token');
      const url = searchTerm 
        ? `${API_BASE_URL}/admin/users?searchTerm=${encodeURIComponent(searchTerm)}`
        : `${API_BASE_URL}/admin/users`;
      
      const response = await fetch(url, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch users: ${response.status} ${errorText}`);
      }

      const result = await response.json();
      
      if (result.success === false) {
        throw new Error(result.errors?.[0]?.message || 'Failed to fetch users');
      }
      
      this.allUsers = result.success ? result.data : result;
      this.renderUsers(this.allUsers);
    } catch (error) {
      console.error('Error loading users:', error);
      showToast(error.message || 'Error loading users', 'error');
    }
  },

  // Render Users
  renderUsers(users) {
    const tbody = document.getElementById('usersTableBody');
    if (!users || users.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="6" class="empty-state">
            <div class="empty-state-icon">👥</div>
            <p>No users found</p>
          </td>
        </tr>
      `;
      return;
    }

    tbody.innerHTML = users.map(user => `
      <tr>
        <td>${user.id}</td>
        <td>${user.name || 'N/A'}</td>
        <td>${user.email}</td>
        <td>
          <span class="role-badge ${user.role === 0 ? 'role-admin' : 'role-user'}">
            ${user.role === 0 ? 'Admin' : 'User'}
          </span>
        </td>
        <td>${user.eventsCreated || 0}</td>
        <td>
          <div class="action-buttons">
            <button class="btn-icon btn-edit" onclick="adminPanel.editUser(${user.id})" title="Edit">
              ✏️
            </button>
            <button class="btn-icon btn-delete" onclick="adminPanel.confirmDelete('user', ${user.id}, '${user.name || user.email}')" title="Delete">
              🗑️
            </button>
          </div>
        </td>
      </tr>
    `).join('');
  },

  // Filter Users with server-side search
  async filterUsers(searchTerm) {
    await this.loadUsers(searchTerm);
  },

  // Edit User
  editUser(userId) {
    const user = this.allUsers.find(u => u.id === userId);
    if (!user) return;

    document.getElementById('editUserId').value = user.id;
    document.getElementById('editUserName').value = user.name || '';
    document.getElementById('editUserEmail').value = user.email;
    document.getElementById('editUserRole').value = user.role;

    this.openModal('editUserModal');
  },

  // Save User
  async saveUser() {
    const userId = document.getElementById('editUserId').value;
    const name = document.getElementById('editUserName').value;
    const email = document.getElementById('editUserEmail').value;
    const role = parseInt(document.getElementById('editUserRole').value);
    const phoneNumber = document.getElementById('editUserPhone')?.value || '';

    try {
      const token = localStorage.getItem('eventify_token');
      const response = await fetch(`${API_BASE_URL}/admin/users/${userId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ 
          id: parseInt(userId),
          name, 
          email, 
          phoneNumber,
          role,
          createdDate: new Date().toISOString()
        })
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to update user: ${response.status}`);
      }

      const result = await response.json();
      
      if (result.success === false) {
        throw new Error(result.errors?.[0]?.message || 'Failed to update user');
      }

      showToast('User updated successfully', 'success');
      this.closeModal('editUserModal');
      await this.loadUsers();
      await this.loadStatistics();
    } catch (error) {
      console.error('Error updating user:', error);
      showToast(error.message || 'Error updating user', 'error');
    }
  },

  // Load Events
  async loadEvents(searchTerm = '') {
    try {
      const token = localStorage.getItem('eventify_token');
      const url = searchTerm 
        ? `${API_BASE_URL}/admin/events?searchTerm=${encodeURIComponent(searchTerm)}`
        : `${API_BASE_URL}/admin/events`;
      
      const response = await fetch(url, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (this.handleAuthError(response)) return;

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch events: ${response.status}`);
      }

      const result = await response.json();
      
      if (result.success === false) {
        throw new Error(result.errors?.[0]?.message || 'Failed to fetch events');
      }
      
      this.allEvents = result.success ? result.data : result;
      this.renderEvents(this.allEvents);
    } catch (error) {
      console.error('Error loading events:', error);
      showToast(error.message || 'Error loading events', 'error');
    }
  },

  // Render Events
  renderEvents(events) {
    const tbody = document.getElementById('eventsTableBody');
    if (!events || events.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="8" class="empty-state">
            <div class="empty-state-icon">📅</div>
            <p>No events found</p>
          </td>
        </tr>
      `;
      return;
    }

    tbody.innerHTML = events.map(event => `
      <tr>
        <td>${event.id}</td>
        <td>${event.name || event.title || 'N/A'}</td>
        <td>${event.organizerName || 'N/A'}</td>
        <td>${new Date(event.startDate || event.date).toLocaleDateString()}</td>
        <td>${event.address || event.location || 'N/A'}</td>
        <td>${event.bookedTickets || 0}</td>
        <td>${event.revenue?.toFixed(2) || '0.00'} EGP</td>
        <td>
          <div class="action-buttons">
            <button class="btn-icon btn-edit" onclick="window.location.href='event.html?id=${event.id}'" title="View">
              👁️
            </button>
            <button class="btn-icon btn-delete" onclick="adminPanel.confirmDelete('event', ${event.id}, '${(event.name || event.title || '').replace(/'/g, "\\'")}')"
title="Delete">
              🗑️
            </button>
          </div>
        </td>
      </tr>
    `).join('');
  },

  // Filter Events with server-side search
  async filterEvents(searchTerm) {
    await this.loadEvents(searchTerm);
  },



  // Load Categories
  async loadCategories() {
    try {
      const token = localStorage.getItem('eventify_token');
      const response = await fetch(`${API_BASE_URL}/admin/eventcategories`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch event categories: ${response.status}`);
      }

      const result = await response.json();
      
      if (result.success === false) {
        throw new Error(result.errors?.[0]?.message || 'Failed to fetch event categories');
      }
      
      this.allCategories = result.success ? result.data : result;
      this.renderCategories(this.allCategories);
    } catch (error) {
      console.error('Error loading event categories:', error);
      showToast(error.message || 'Error loading event categories', 'error');
    }
  },

  // Render Event Categories
  renderCategories(categories) {
    const tbody = document.getElementById('categoriesTableBody');
    if (!categories || categories.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="3" class="empty-state">
            <div class="empty-state-icon">🏷️</div>
            <p>No event categories found</p>
          </td>
        </tr>
      `;
      return;
    }

    tbody.innerHTML = categories.map(category => {
      return `
        <tr>
          <td>${category.value}</td>
          <td>${category.name}</td>
          <td>${category.eventCount}</td>
        </tr>
      `;
    }).join('');
  },

  // Event categories are enum-based and cannot be edited or deleted
  // Category management removed - EventCategory is an immutable enum

  // Confirm Delete
  confirmDelete(type, id, name) {
    const typeLabel = type.charAt(0).toUpperCase() + type.slice(1);
    document.getElementById('deleteMessage').textContent = 
      `Are you sure you want to delete ${typeLabel} "${name}"? This action cannot be undone.`;

    const confirmBtn = document.getElementById('confirmDeleteBtn');
    confirmBtn.onclick = () => this.deleteItem(type, id);

    this.openModal('deleteModal');
  },

  // Delete Item
  async deleteItem(type, id) {
    try {
      const token = localStorage.getItem('eventify_token');
      
      // Special handling for booking cancellation (POST instead of DELETE)
      if (type === 'booking') {
        const response = await fetch(`${API_BASE_URL}/admin/bookings/${id}/cancel`, {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (!response.ok) {
          const errorText = await response.text();
          throw new Error(`Failed to cancel booking: ${response.status}`);
        }

        const result = await response.json();
        
        if (result.success === false) {
          throw new Error(result.errors?.[0]?.message || 'Failed to cancel booking');
        }

        showToast('Booking cancelled successfully', 'success');
        this.closeModal('deleteModal');
        await this.loadBookings();
        await this.loadPayments();
        await this.loadStatistics();
        return;
      }
      
      // Regular DELETE for other types
      const endpoints = {
        user: `admin/users/${id}`,
        event: `admin/events/${id}`,
        category: `admin/categories/${id}`
      };

      const response = await fetch(`${API_BASE_URL}/${endpoints[type]}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to delete ${type}: ${response.status}`);
      }

      const result = await response.json();
      
      if (result.success === false) {
        throw new Error(result.errors?.[0]?.message || `Failed to delete ${type}`);
      }

      showToast(`${type.charAt(0).toUpperCase() + type.slice(1)} deleted successfully`, 'success');
      this.closeModal('deleteModal');

      // Reload appropriate data
      switch(type) {
        case 'user':
          await this.loadUsers();
          break;
        case 'event':
          await this.loadEvents();
          break;
        case 'category':
          await this.loadCategories();
          break;
      }
      await this.loadStatistics();
    } catch (error) {
      console.error(`Error deleting ${type}:`, error);
      showToast(error.message || `Error deleting ${type}`, 'error');
    }
  },

  // Load Bookings
  async loadBookings(searchTerm = '') {
    try {
      const token = localStorage.getItem('eventify_token');
      const url = searchTerm 
        ? `${API_BASE_URL}/admin/bookings?searchTerm=${encodeURIComponent(searchTerm)}`
        : `${API_BASE_URL}/admin/bookings`;
      
      const response = await fetch(url, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (this.handleAuthError(response)) return;

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch bookings: ${response.status}`);
      }

      const result = await response.json();
      
      if (result.success === false) {
        throw new Error(result.errors?.[0]?.message || 'Failed to fetch bookings');
      }
      
      this.allBookings = result.success ? result.data : (result.data || result);
      this.renderBookings(this.allBookings);
    } catch (error) {
      console.error('Error loading bookings:', error);
      showToast(error.message || 'Error loading bookings', 'error');
    }
  },

  // Render Bookings
  renderBookings(bookings) {
    const tbody = document.getElementById('bookingsTableBody');
    if (!bookings || bookings.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="8" class="empty-state">
            <div class="empty-state-icon">🎫</div>
            <p>No bookings found</p>
          </td>
        </tr>
      `;
      return;
    }

    const statusMap = {
      0: { label: 'Booked', color: '#10b981' },
      1: { label: 'Pending', color: '#f59e0b' },
      2: { label: 'Cancelled', color: '#ef4444' }
    };

    tbody.innerHTML = bookings.map(booking => {
      const status = statusMap[booking.status] || { label: 'Unknown', color: '#6b7280' };
      return `
        <tr>
          <td>${booking.id}</td>
          <td>${booking.userName || 'N/A'}</td>
          <td>${booking.eventName || booking.eventTitle || 'N/A'}</td>
          <td>${booking.ticketsNum || booking.numberOfTickets || 0}</td>
          <td>${booking.totalPrice?.toFixed(2) || '0.00'} EGP</td>
          <td>
            <span class="status-badge" style="background: ${status.color}20; color: ${status.color};">
              ${status.label}
            </span>
          </td>
          <td>${booking.createdDate || booking.bookingDate ? new Date(booking.createdDate || booking.bookingDate).toLocaleDateString() : 'N/A'}</td>
          <td>
            <div class="action-buttons">
              <button class="btn-icon" onclick="adminPanel.viewBookingDetails(${booking.id})" title="View Details">
                👁️
              </button>
              <button class="btn-icon btn-delete" onclick="adminPanel.confirmDelete('booking', ${booking.id}, 'Booking #${booking.id}')" title="Cancel">
                🗑️
              </button>
            </div>
          </td>
        </tr>
      `;
    }).join('');
  },

  // Filter Bookings with server-side search
  async filterBookings(searchTerm) {
    await this.loadBookings(searchTerm);
    
    // Apply status filter client-side
    const statusFilter = document.getElementById('bookingStatusFilter')?.value;
    if (statusFilter !== '') {
      const filtered = this.allBookings.filter(b => b.status === parseInt(statusFilter));
      this.renderBookings(filtered);
    }
  },

  // View Booking Details
  viewBookingDetails(bookingId) {
    window.open(`booking-details.html?id=${bookingId}&from=booking`, '_blank');
  },

  // Load Payments
  async loadPayments(searchTerm = '') {
    try {
      const token = localStorage.getItem('eventify_token');
      const url = searchTerm 
        ? `${API_BASE_URL}/admin/payments?searchTerm=${encodeURIComponent(searchTerm)}`
        : `${API_BASE_URL}/admin/payments`;
      
      const response = await fetch(url, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (this.handleAuthError(response)) return;

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch payments: ${response.status}`);
      }

      const result = await response.json();
      
      if (result.success === false) {
        throw new Error(result.errors?.[0]?.message || 'Failed to fetch payments');
      }
      
      this.allPayments = result.success ? result.data : (result.data || result);
      this.renderPayments(this.allPayments);
    } catch (error) {
      console.error('Error loading payments:', error);
      showToast(error.message || 'Error loading payments', 'error');
    }
  },

  // Render Payments
  renderPayments(payments) {
    const tbody = document.getElementById('paymentsTableBody');
    if (!payments || payments.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="8" class="empty-state">
            <div class="empty-state-icon">💳</div>
            <p>No payments found</p>
          </td>
        </tr>
      `;
      return;
    }

    const statusMap = {
      0: { label: 'Paid', color: '#10b981' },
      1: { label: 'Pending', color: '#f59e0b' },
      2: { label: 'Cancelled', color: '#ef4444' },
      3: { label: 'Rejected', color: '#dc2626' },
      4: { label: 'Refunded', color: '#8b5cf6' }
    };

    tbody.innerHTML = payments.map(payment => {
      const status = statusMap[payment.status] || { label: 'Unknown', color: '#6b7280' };
      const paymentDate = payment.dateTime || payment.paymentDate;
      return `
        <tr>
          <td>${payment.bookingId}</td>
          <td>${payment.userName || 'N/A'}</td>
          <td>${payment.eventName || payment.eventTitle || 'N/A'}</td>
          <td>${payment.totalPrice?.toFixed(2) || payment.amount?.toFixed(2) || '0.00'} EGP</td>
          <td>${payment.paymentMethod || payment.method || 'N/A'}</td>
          <td>
            <span class="status-badge" style="background: ${status.color}20; color: ${status.color};">
              ${status.label}
            </span>
          </td>
          <td>${paymentDate ? new Date(paymentDate).toLocaleDateString() : 'N/A'}</td>
          <td>
            <div class="action-buttons">
              <button class="btn-icon" onclick="adminPanel.viewPaymentDetails(${payment.bookingId})" title="View Details">
                👁️
              </button>
            </div>
          </td>
        </tr>
      `;
    }).join('');
  },

  // Filter Payments with server-side search
  async filterPayments(searchTerm) {
    await this.loadPayments(searchTerm);
    
    // Apply status filter client-side
    const statusFilter = document.getElementById('paymentStatusFilter')?.value;
    if (statusFilter !== '') {
      const filtered = this.allPayments.filter(p => p.status === parseInt(statusFilter));
      this.renderPayments(filtered);
    }
  },

  // View Payment Details
  viewPaymentDetails(bookingId) {
    window.open(`booking-details.html?id=${bookingId}&from=payment`, '_blank');
  },

  // Open Modal
  openModal(modalId) {
    document.getElementById(modalId).classList.add('active');
  },

  // Close Modal
  closeModal(modalId) {
    document.getElementById(modalId).classList.remove('active');
  }
};

// Toast notification function
function showToast(message, type = 'info') {
  const toast = document.createElement('div');
  toast.className = `toast toast-${type}`;
  toast.textContent = message;
  toast.style.cssText = `
    position: fixed;
    top: 20px;
    right: 20px;
    background: ${type === 'success' ? '#10b981' : type === 'error' ? '#ef4444' : '#3b82f6'};
    color: white;
    padding: 1rem 1.5rem;
    border-radius: 8px;
    box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    z-index: 10000;
    animation: slideIn 0.3s ease;
  `;

  document.body.appendChild(toast);
  setTimeout(() => {
    toast.style.animation = 'slideOut 0.3s ease';
    setTimeout(() => toast.remove(), 300);
  }, 3000);
}

// Add animation styles
const style = document.createElement('style');
style.textContent = `
  @keyframes slideIn {
    from {
      transform: translateX(100%);
      opacity: 0;
    }
    to {
      transform: translateX(0);
      opacity: 1;
    }
  }
  @keyframes slideOut {
    from {
      transform: translateX(0);
      opacity: 1;
    }
    to {
      transform: translateX(100%);
      opacity: 0;
    }
  }
  .btn-danger {
    background: #ef4444;
    color: white;
    padding: 0.75rem 1.5rem;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-weight: 500;
  }
  .btn-danger:hover {
    background: #dc2626;
  }
`;
document.head.appendChild(style);

// Initialize when page loads
document.addEventListener('DOMContentLoaded', () => {
  adminPanel.init();
});
