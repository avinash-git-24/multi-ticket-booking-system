# Design System Specification: ShowSphere
**Brand Identity:** ShowSphere (Movie & Live Event Booking)  
**Primary Visual Archetype:** High-contrast Red-and-White Cinema & Event Portal  
**Visual Influences:** BookMyShow (Red-and-white accents, date strip, tiered seat map) and District.in (Contemporary event discovery cards, rounded chips, category pills)  

---

## 1. Design Tokens

### Color Palette
- **Brand Primary:** `#F84464` (ShowSphere Vibrant Red)
- **Brand Primary Hover / Active:** `#D82245` (Deep Crimson)
- **Brand Primary Dark:** `#A71431` (Dark Ruby)
- **Brand Primary Light / Tint:** `#FFF0F3` (Soft Rose Tint)
- **Secondary Accent:** `#222539` (Deep Midnight Charcoal)
- **Surface & Backgrounds:**
  - Base Background: `#FFFFFF` (Pure White)
  - Secondary Section Background: `#F5F5F7` (Cool Light Grey)
  - Card & Container Surface: `#FFFFFF` (Elevated White)
  - Modal / Overlay Backdrop: `rgba(15, 23, 42, 0.65)`
- **Typography & Text:**
  - Headings / Heavy Contrast: `#1A1D24` (Jet Dark)
  - Body Text: `#333545` (Charcoal Neutral)
  - Muted Text / Meta: `#666D7E` (Slate Grey)
  - Inverted / Light Text: `#FFFFFF`
- **Functional & Status Colors:**
  - Success / Confirmed: `#10B981` (Emerald)
  - Warning / Expiring Hold: `#F59E0B` (Amber Gold)
  - Danger / Sold Out / Failed: `#EF4444` (Coral Red)
  - Info / Demo Notice: `#3B82F6` (Electric Blue)
- **Borders & Dividers:**
  - Subtle Border: `#E5E7EB` (Light Neutral Border)
  - Focus Ring: `rgba(248, 68, 100, 0.4)` (ShowSphere Red Glow)

### Typography
- **Primary Font Family:** `'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif`
- **Scale:**
  - Display / Hero Heading: 32px / 2rem, Weight 800
  - Page Heading (h1): 26px / 1.625rem, Weight 700
  - Section Heading (h2): 20px / 1.25rem, Weight 700
  - Card Title (h3): 16px / 1rem, Weight 600
  - Body Text: 14px / 0.875rem, Weight 400
  - Small / Meta / Badge: 12px / 0.75rem, Weight 500
  - Micro / Seat Label: 10px / 0.625rem, Weight 600

### Spacing Scale
- 4px (xs), 8px (sm), 12px (md), 16px (lg), 24px (xl), 32px (2xl), 48px (3xl)

---

## 2. Core UI Component Specifications

### 1. Navigation Bar
- High-contrast white navbar with subtle bottom border (`#E5E7EB`) and sticky top position.
- Logo: ShowSphere with branded red play/ticket icon.
- City selector dropdown: Location pin icon with active city (Default: Rajkot).
- Search bar: Pill-shaped input with search icon, filtering movies and events.
- Category navigation links: Movies, Events.
- User menu: Login/Register buttons or User Avatar dropdown with My Bookings, Profile, Admin Dashboard, and Sign Out.

### 2. Movie Cards (BookMyShow Style)
- Poster aspect ratio 2:3 with rounded corners (8px) and smooth hover elevation.
- Rating & votes ribbon pinned to poster bottom.
- Language and format badges (e.g., "Hindi, Gujarati", "2D, 3D, IMAX").
- Clear action button: "Book Tickets" in primary red.

### 3. Event Cards (District.in Style)
- 16:9 banner preview with rounded corners (12px).
- Date badge: Month and Day tag floating on top left.
- Price indicator: "₹299 onwards".
- Category pills: Music, Comedy, Workshop, Sports.
- Venue and City subtitle.

### 4. Interactive Seat Map (Movie Booking)
- Screen display: Curved silver projection line indicating "All eyes this way please".
- Tier division:
  - **Platinum Tier (₹300):** Top rows, plush spacing.
  - **Gold Tier (₹220):** Middle rows, optimal sightlines.
  - **Silver Tier (₹150):** Front rows, value seating.
- Seat states:
  - **Available:** White background with subtle border `#D1D5DB`.
  - **Selected:** Solid ShowSphere Red `#F84464` with white text.
  - **Held / In Progress:** Amber `#F59E0B` outline.
  - **Booked / Sold Out:** Light grey `#E5E7EB` with muted cross, unclickable (`pointer-events: none`).
- Real-time bottom bar: Displays count of selected seats, tier names, total calculation, and "Proceed to Pay" button.

### 5. Event Ticket Selector
- Tier card breakdown (VIP, Gold, General Admission).
- Perks description for each tier.
- Numerical stepper counter (- / +) with validation boundaries (min: 0, max: 10 per order).
- Dynamic subtotal update on stepper change.

### 6. Checkout and Payment Modal
- Prominent persistent warning banner:
  `NOTICE: College Demonstration Project. No real financial transaction will occur. No real card details collected.`
- Breakdown: Base Ticket Price, Convenience Fee (10%), Integrated GST (18% on fee), Total Amount.
- Simulation modes: Success (generates booking), Insufficient Funds (releases hold), Network Timeout (releases hold).

---

## 3. Responsive Breakpoints

| Viewport | Min Width | Max Width | Target Layout Adaptations |
| :--- | :--- | :--- | :--- |
| Mobile | 0px | 575px | Single column cards, collapsible hamburger menu, horizontal scrolling date strip, full-width seat map with pinch/pan hint |
| Tablet | 576px | 991px | 2-column movie/event grid, condensed checkout sidebar |
| Desktop | 992px | 1279px | 4-column movie grid, 3-column event grid, sticky checkout panel |
| Wide Desktop | 1280px | - | Max container width 1200px, 4-column grid with generous whitespace |
