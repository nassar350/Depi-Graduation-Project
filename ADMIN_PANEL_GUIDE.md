# Admin Panel Integration Guide

## Overview
A comprehensive admin panel has been implemented for Eventify, allowing administrators to manage users, events, organizers, and categories through a centralized interface.

## Features

### 1. **User Management**
- View all registered users with their details
- Edit user information (name, email)
- Change user roles (Admin/User)
- Delete user accounts
- Search users by name or email
- View number of events created by each user

### 2. **Event Management**
- View all events across the platform
- See event details (title, organizer, date, location, bookings, revenue)
- Delete events
- View events by clicking on them
- Search events by title, location, or organizer
- Track event performance metrics

### 3. **Organizer Management**
- View all users who have created events (organizers)
- See organizer statistics:
  - Total events created
  - Total revenue generated
- View all events by a specific organizer
- Search organizers by name or email

### 4. **Category Management**
- Add new event categories
- Edit existing categories
- Delete categories
- View number of events in each category
- Quick category creation from admin panel

### 5. **Dashboard Statistics**
- Total users count
- Total events count
- Total organizers count
- Total categories count

## Access Control

### Role-Based Authentication
- **Admin Role**: `role === 0`
- **User Role**: `role === 1`

### Security Features
- Admin panel only accessible to users with admin role
- Automatic redirect to home page for non-admin users
- Token-based authentication for all API calls
- Admin Panel button visible only for admin users in navigation

## Files Created/Modified

### New Files
1. **`EventifyFrontEnd/admin-panel.html`**
   - Main admin panel interface
   - Tabbed layout for different management sections
   - Modal dialogs for editing/deleting items
   - Responsive design with search functionality

2. **`EventifyFrontEnd/js/admin.js`**
   - Admin panel logic and API integrations
   - CRUD operations for all entities
   - Real-time search and filtering
   - Statistics calculation
   - Toast notifications for user feedback

### Modified Files
1. **`EventifyFrontEnd/js/app.js`**
   - Added `adminLinks` handling in `checkAuthState()`
   - Show/hide admin panel link based on user role
   - Added Admin Panel button to user info section

2. **`EventifyFrontEnd/css/styles.css`**
   - Added `.btn-warning` style for admin panel button

3. **Navigation Updates** (index.html, explore.html, dashboard.html)
   - Added admin panel link with `data-admin-only` attribute

## API Endpoints Used

### User Management
```
GET    /api/User                 - Get all users
GET    /api/User/{id}            - Get user by ID
PUT    /api/User/{id}            - Update user
DELETE /api/User/{id}            - Delete user
```

### Event Management
```
GET    /api/Events               - Get all events
GET    /api/Events/{id}          - Get event by ID
DELETE /api/Events/{id}          - Delete event
```

### Category Management
```
GET    /api/Categories           - Get all categories
POST   /api/Categories           - Create category
PUT    /api/Categories/{id}      - Update category
DELETE /api/Categories/{id}      - Delete category
```

## User Interface Components

### Admin Tabs
1. **Users Tab** - User management interface
2. **Events Tab** - Event oversight and management
3. **Organizers Tab** - Organizer statistics and management
4. **Categories Tab** - Category CRUD operations

### Modal Dialogs
1. **Edit User Modal** - Update user details and role
2. **Category Modal** - Add/edit categories
3. **Delete Confirmation Modal** - Confirm deletion operations

### Data Tables
- Sortable columns
- Search functionality
- Action buttons (Edit, Delete, View)
- Role/Status badges with color coding
- Responsive design with horizontal scroll on mobile

## Styling Features

### Color Coding
- **Admin Badge**: Yellow background (`#fef3c7`)
- **User Badge**: Blue background (`#dbeafe`)
- **Admin Panel Button**: Warning yellow (`#ffb800`)

### Responsive Design
- Mobile-friendly tables with horizontal scroll
- Flexible grid layouts for statistics
- Collapsible navigation for small screens

## Security Considerations

### Frontend Protection
1. Role check on page load (redirects non-admins)
2. Admin-only links hidden from non-admin users
3. Token verification before API calls

### Backend Requirements (To Implement)
1. Add `[Authorize(Roles = "Admin")]` attribute to sensitive endpoints
2. Implement role-based authorization in UserController
3. Add admin-only endpoints for user role management
4. Secure delete operations to prevent unauthorized access

## Backend Integration Needed

### 1. Update UserController
Add role-based authorization:
```csharp
[HttpPut("{id:int}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userDto)
{
    // Update user including role changes
}

[HttpDelete("{id:int}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteUser(int id)
{
    // Delete user
}
```

### 2. Update EventsController
Add admin endpoints:
```csharp
[HttpDelete("{id:int}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteEvent(int id)
{
    // Delete event
}
```

### 3. Update CategoriesController
Add CRUD operations:
```csharp
[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
{
    // Create category
}

[HttpPut("{id:int}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
{
    // Update category
}

[HttpDelete("{id:int}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteCategory(int id)
{
    // Delete category
}
```

## Usage Instructions

### For Admins
1. Log in with an admin account (role = 0)
2. Click "Admin Panel" button in the navigation bar
3. Use tabs to navigate between management sections
4. Use search boxes to filter data
5. Click action buttons to edit or delete items
6. View statistics at the top of the page

### For Development
1. Ensure backend endpoints support DELETE operations
2. Add `[Authorize(Roles = "Admin")]` to sensitive endpoints
3. Test role-based access control
4. Implement proper error handling for failed operations

## Testing Checklist

- [ ] Admin can view all users
- [ ] Admin can edit user roles
- [ ] Admin can delete users
- [ ] Admin can view all events
- [ ] Admin can delete events
- [ ] Admin can view organizers and their events
- [ ] Admin can create/edit/delete categories
- [ ] Non-admin users cannot access admin panel
- [ ] Search functionality works across all tabs
- [ ] Statistics update correctly
- [ ] Delete confirmations work properly
- [ ] Toast notifications display correctly
- [ ] Mobile responsive layout works

## Future Enhancements

1. **Analytics Dashboard**
   - Revenue charts
   - User growth metrics
   - Event popularity trends

2. **Bulk Operations**
   - Bulk user deletion
   - Bulk event approval/rejection
   - Export data to CSV

3. **Activity Logs**
   - Track admin actions
   - User activity monitoring
   - Audit trail for changes

4. **Advanced Filtering**
   - Date range filters for events
   - Role-based user filtering
   - Event status filtering (upcoming/past/cancelled)

5. **Email Notifications**
   - Send notifications to users
   - Bulk email to organizers
   - System announcements

## Support
For issues or questions regarding the admin panel, refer to the main documentation or contact the development team.
