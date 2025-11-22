// Eventify Mock Data
// This file contains sample events, organizers, and other data for the application

// Sample Organizers
const organizers = [
  {
    id: 1,
    name: "TechCorp Events",
    avatar: "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=150&h=150&fit=crop&crop=face",
    email: "contact@techcorp.com",
    bio: "Leading technology conference organizer with 10+ years of experience.",
    website: "https://techcorp.com",
    eventsCount: 25
  },
  {
    id: 2,
    name: "Creative Studios",
    avatar: "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=150&h=150&fit=crop&crop=face",
    email: "hello@creativestudios.com",
    bio: "Bringing artists and creatives together through innovative workshops and exhibitions.",
    website: "https://creativestudios.com",
    eventsCount: 18
  },
  {
    id: 3,
    name: "Wellness Collective",
    avatar: "https://images.unsplash.com/photo-1594736797933-d0f26d0b4dac?w=150&h=150&fit=crop&crop=face",
    email: "info@wellnesscollective.org",
    bio: "Promoting health and wellness through community events and educational programs.",
    website: "https://wellnesscollective.org",
    eventsCount: 32
  }
];

// Event Categories
const categories = [
  "Technology",
  "Business",
  "Arts & Culture",
  "Health & Wellness", 
  "Food & Drink",
  "Sports & Fitness",
  "Music",
  "Education",
  "Networking",
  "Entertainment"
];

// Sample Events (at least 8 with future dates)
const events = [
  {
    id: 1,
    title: "Future of AI Conference 2025",
    description: "Join industry leaders as we explore the cutting-edge developments in artificial intelligence, machine learning, and their impact on society. This full-day conference features keynotes from top researchers, hands-on workshops, and networking opportunities with AI professionals from around the world.",
    image: "https://images.unsplash.com/photo-1485827404703-89b55fcc595e?w=800&h=400&fit=crop",
    date: "2025-12-15",
    time: "09:00",
    endTime: "17:00",
    location: "San Francisco Convention Center, CA",
    price: 299,
    capacity: 500,
    ticketsAvailable: 342,
    category: "Technology",
    tags: ["AI", "Machine Learning", "Technology", "Innovation"],
    organizerId: 1,
    featured: true,
    createdAt: "2025-10-01T10:00:00Z"
  },
  {
    id: 2,
    title: "Creative Design Workshop",
    description: "Unleash your creativity in this intensive 2-day workshop covering modern design principles, user experience design, and digital art techniques. Perfect for designers at all levels looking to expand their skillset and creative vision.",
    image: "https://images.unsplash.com/photo-1561070791-2526d30994b5?w=800&h=400&fit=crop",
    date: "2025-11-28",
    time: "10:00",
    endTime: "16:00",
    location: "Downtown Art Studio, NYC",
    price: 150,
    capacity: 30,
    ticketsAvailable: 8,
    category: "Arts & Culture",
    tags: ["Design", "Workshop", "Creative", "UX"],
    organizerId: 2,
    featured: false,
    createdAt: "2025-09-15T14:30:00Z"
  },
  {
    id: 3,
    title: "Startup Pitch Competition",
    description: "Watch promising startups pitch their innovative ideas to a panel of veteran investors and industry experts. Network with entrepreneurs, investors, and business leaders while witnessing the next generation of game-changing companies.",
    image: "https://images.unsplash.com/photo-1559136555-9303baea8ebd?w=800&h=400&fit=crop",
    date: "2025-12-03",
    time: "18:00",
    endTime: "21:00",
    location: "Innovation Hub, Austin TX",
    price: 75,
    capacity: 200,
    ticketsAvailable: 156,
    category: "Business",
    tags: ["Startup", "Investment", "Networking", "Business"],
    organizerId: 1,
    featured: true,
    createdAt: "2025-10-10T09:15:00Z"
  },
  {
    id: 4,
    title: "Mindfulness & Meditation Retreat",
    description: "Escape the hustle and bustle of daily life with this peaceful weekend retreat. Learn meditation techniques, practice mindful breathing, and connect with nature in a serene mountain setting. All skill levels welcome.",
    image: "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&h=400&fit=crop",
    date: "2025-11-22",
    time: "08:00",
    endTime: "17:00",
    location: "Mountain View Retreat, Colorado",
    price: 225,
    capacity: 50,
    ticketsAvailable: 23,
    category: "Health & Wellness",
    tags: ["Meditation", "Wellness", "Retreat", "Mindfulness"],
    organizerId: 3,
    featured: false,
    createdAt: "2025-09-20T11:45:00Z"
  },
  {
    id: 5,
    title: "Gourmet Food Festival",
    description: "Indulge in a culinary adventure featuring renowned chefs, local food vendors, wine tastings, and cooking demonstrations. Experience diverse cuisines from around the world in this delicious celebration of food and culture.",
    image: "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800&h=400&fit=crop",
    date: "2025-12-07",
    time: "11:00",
    endTime: "19:00",
    location: "Riverside Park, Portland OR",
    price: 45,
    capacity: 1000,
    ticketsAvailable: 743,
    category: "Food & Drink",
    tags: ["Food", "Festival", "Culinary", "Wine"],
    organizerId: 2,
    featured: true,
    createdAt: "2025-08-30T16:20:00Z"
  },
  {
    id: 6,
    title: "Digital Marketing Masterclass",
    description: "Master the latest digital marketing strategies and tools in this comprehensive one-day intensive. Learn about social media marketing, SEO, content creation, and analytics from industry veterans.",
    image: "https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=800&h=400&fit=crop",
    date: "2025-11-30",
    time: "09:30",
    endTime: "17:30",
    location: "Business Center, Chicago IL",
    price: 199,
    capacity: 80,
    ticketsAvailable: 34,
    category: "Business",
    tags: ["Marketing", "Digital", "SEO", "Social Media"],
    organizerId: 1,
    featured: false,
    createdAt: "2025-10-05T13:10:00Z"
  },
  {
    id: 7,
    title: "Jazz Under the Stars",
    description: "An enchanting evening of smooth jazz under the open sky. Feature performances by acclaimed jazz musicians, food trucks, and a relaxed atmosphere perfect for music lovers and casual listeners alike.",
    image: "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=800&h=400&fit=crop",
    date: "2025-12-14",
    time: "19:30",
    endTime: "23:00",
    location: "Central Park Amphitheater, NYC",
    price: 55,
    capacity: 300,
    ticketsAvailable: 189,
    category: "Music",
    tags: ["Jazz", "Live Music", "Outdoor", "Evening"],
    organizerId: 2,
    featured: false,
    createdAt: "2025-09-25T08:45:00Z"
  },
  {
    id: 8,
    title: "Blockchain & Cryptocurrency Summit",
    description: "Dive deep into the world of blockchain technology and cryptocurrency. Learn about DeFi, NFTs, smart contracts, and the future of digital finance from industry pioneers and thought leaders.",
    image: "https://images.unsplash.com/photo-1639762681485-074b7f938ba0?w=800&h=400&fit=crop",
    date: "2025-12-20",
    time: "08:00",
    endTime: "18:00",
    location: "Miami Convention Center, FL",
    price: 349,
    capacity: 400,
    ticketsAvailable: 267,
    category: "Technology",
    tags: ["Blockchain", "Cryptocurrency", "DeFi", "FinTech"],
    organizerId: 1,
    featured: true,
    createdAt: "2025-10-12T15:30:00Z"
  },
  {
    id: 9,
    title: "Yoga & Wellness Workshop",
    description: "Start your journey to better health with this beginner-friendly yoga workshop. Learn basic poses, breathing techniques, and relaxation methods in a supportive and peaceful environment.",
    image: "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=800&h=400&fit=crop",
    date: "2025-11-25",
    time: "07:00",
    endTime: "10:00",
    location: "Serenity Studio, Los Angeles CA",
    price: 35,
    capacity: 25,
    ticketsAvailable: 12,
    category: "Health & Wellness",
    tags: ["Yoga", "Wellness", "Beginner", "Health"],
    organizerId: 3,
    featured: false,
    createdAt: "2025-10-08T12:00:00Z"
  },
  {
    id: 10,
    title: "Photography Exhibition Opening",
    description: "Celebrate the opening of 'Urban Landscapes' - a stunning photography exhibition featuring works from emerging and established photographers capturing the beauty of modern city life.",
    image: "https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800&h=400&fit=crop",
    date: "2025-12-05",
    time: "18:00",
    endTime: "21:00",
    location: "Modern Art Gallery, Seattle WA",
    price: 25,
    capacity: 150,
    ticketsAvailable: 98,
    category: "Arts & Culture",
    tags: ["Photography", "Art", "Exhibition", "Culture"],
    organizerId: 2,
    featured: false,
    createdAt: "2025-09-30T17:15:00Z"
  }
];

// User bookings (stored in localStorage)
// This structure will be used to store user bookings
const sampleBookings = [
  {
    id: "BOOK001",
    eventId: 1,
    userId: 1,
    tickets: 2,
    totalAmount: 598,
    bookingDate: "2025-11-20T10:30:00Z",
    attendeeInfo: {
      name: "John Doe",
      email: "john.doe@example.com"
    },
    paymentStatus: "paid",
    promoCode: null
  }
];

// User data structure (stored in localStorage after auth)
const sampleUser = {
  id: 1,
  name: "John Doe",
  email: "john.doe@example.com",
  avatar: "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&h=150&fit=crop&crop=face",
  joinDate: "2025-01-15T09:00:00Z",
  role: "attendee", // or "organizer"
  organizerId: null // only if role is organizer
};

// Utility functions for data manipulation

/**
 * Get all events
 * @returns {Array} Array of all events
 */
function getAllEvents() {
  // Get any events stored in localStorage (from create-event)
  const storedEvents = JSON.parse(localStorage.getItem('customEvents') || '[]');
  return [...events, ...storedEvents];
}

/**
 * Get event by ID
 * @param {number} id - Event ID
 * @returns {Object|null} Event object or null if not found
 */
function getEventById(id) {
  const allEvents = getAllEvents();
  return allEvents.find(event => event.id === parseInt(id)) || null;
}

/**
 * Get organizer by ID
 * @param {number} id - Organizer ID
 * @returns {Object|null} Organizer object or null if not found
 */
function getOrganizerById(id) {
  return organizers.find(organizer => organizer.id === parseInt(id)) || null;
}

/**
 * Get upcoming events (events with future dates)
 * @param {number} limit - Maximum number of events to return
 * @returns {Array} Array of upcoming events
 */
function getUpcomingEvents(limit = null) {
  const allEvents = getAllEvents();
  const now = new Date();
  const upcoming = allEvents
    .filter(event => new Date(event.date + 'T' + event.time) > now)
    .sort((a, b) => new Date(a.date + 'T' + a.time) - new Date(b.date + 'T' + b.time));
  
  return limit ? upcoming.slice(0, limit) : upcoming;
}

/**
 * Get featured events
 * @returns {Array} Array of featured events
 */
function getFeaturedEvents() {
  return getAllEvents().filter(event => event.featured);
}

/**
 * Search events by title, description, or tags
 * @param {string} query - Search query
 * @returns {Array} Array of matching events
 */
function searchEvents(query) {
  if (!query) return getAllEvents();
  
  const allEvents = getAllEvents();
  const searchTerm = query.toLowerCase();
  
  return allEvents.filter(event => 
    event.title.toLowerCase().includes(searchTerm) ||
    event.description.toLowerCase().includes(searchTerm) ||
    event.tags.some(tag => tag.toLowerCase().includes(searchTerm)) ||
    event.location.toLowerCase().includes(searchTerm)
  );
}

/**
 * Filter events by criteria
 * @param {Object} filters - Filter criteria
 * @returns {Array} Array of filtered events
 */
function filterEvents(filters = {}) {
  let filteredEvents = getAllEvents();
  
  // Filter by category
  if (filters.category && filters.category !== 'all') {
    filteredEvents = filteredEvents.filter(event => 
      event.category.toLowerCase() === filters.category.toLowerCase()
    );
  }
  
  // Filter by date range
  if (filters.startDate) {
    filteredEvents = filteredEvents.filter(event => 
      new Date(event.date) >= new Date(filters.startDate)
    );
  }
  
  if (filters.endDate) {
    filteredEvents = filteredEvents.filter(event => 
      new Date(event.date) <= new Date(filters.endDate)
    );
  }
  
  // Filter by price range
  if (filters.minPrice !== undefined) {
    filteredEvents = filteredEvents.filter(event => 
      event.price >= filters.minPrice
    );
  }
  
  if (filters.maxPrice !== undefined) {
    filteredEvents = filteredEvents.filter(event => 
      event.price <= filters.maxPrice
    );
  }
  
  return filteredEvents;
}

/**
 * Get events by category
 * @param {string} category - Event category
 * @returns {Array} Array of events in the category
 */
function getEventsByCategory(category) {
  return getAllEvents().filter(event => 
    event.category.toLowerCase() === category.toLowerCase()
  );
}

/**
 * Get related events (same category, excluding current event)
 * @param {number} eventId - Current event ID
 * @param {number} limit - Maximum number of related events
 * @returns {Array} Array of related events
 */
function getRelatedEvents(eventId, limit = 4) {
  const currentEvent = getEventById(eventId);
  if (!currentEvent) return [];
  
  return getAllEvents()
    .filter(event => 
      event.id !== eventId && 
      event.category === currentEvent.category
    )
    .sort(() => Math.random() - 0.5) // Random order
    .slice(0, limit);
}

/**
 * Add a new event (used by create-event functionality)
 * @param {Object} eventData - New event data
 * @returns {Object} Created event with generated ID
 */
function addEvent(eventData) {
  const allEvents = getAllEvents();
  const maxId = Math.max(...allEvents.map(e => e.id), 0);
  
  const newEvent = {
    ...eventData,
    id: maxId + 1,
    ticketsAvailable: eventData.capacity,
    featured: false,
    createdAt: new Date().toISOString()
  };
  
  // Store in localStorage
  const customEvents = JSON.parse(localStorage.getItem('customEvents') || '[]');
  customEvents.push(newEvent);
  localStorage.setItem('customEvents', JSON.stringify(customEvents));
  
  return newEvent;
}

/**
 * Format date for display
 * @param {string} date - Date string
 * @returns {string} Formatted date
 */
function formatDate(date) {
  return new Date(date).toLocaleDateString('en-US', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
}

/**
 * Format time for display
 * @param {string} time - Time string (HH:MM format)
 * @returns {string} Formatted time
 */
function formatTime(time) {
  const [hours, minutes] = time.split(':');
  const hour = parseInt(hours);
  const ampm = hour >= 12 ? 'PM' : 'AM';
  const displayHour = hour % 12 || 12;
  return `${displayHour}:${minutes} ${ampm}`;
}

/**
 * Generate a random booking ID
 * @returns {string} Random booking ID
 */
function generateBookingId() {
  return 'BOOK' + Math.random().toString(36).substr(2, 9).toUpperCase();
}

// Export data and functions for use in other scripts
window.EventifyData = {
  events,
  organizers,
  categories,
  sampleBookings,
  sampleUser,
  
  // Functions
  getAllEvents,
  getEventById,
  getOrganizerById,
  getUpcomingEvents,
  getFeaturedEvents,
  searchEvents,
  filterEvents,
  getEventsByCategory,
  getRelatedEvents,
  addEvent,
  formatDate,
  formatTime,
  generateBookingId
};

// For debugging in console
console.log('Eventify Data loaded:', window.EventifyData);