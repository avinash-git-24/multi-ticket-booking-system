# Acceptance Criteria and Verification Checklist

This document defines the numbered verification criteria (AC-01 through AC-25) mapped against global criteria AC-A through AC-L for the Movie and Event Ticket Booking System.

| ID | Category | Requirement Description | Target Phase | Verification Method | Status |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **AC-01** | Setup & Build | Solution builds cleanly with zero compiler warnings or errors on .NET 10 | Phase 2 | `dotnet build` | In Progress |
| **AC-02** | Database Setup | Database schema scripts execute without error on fresh SQL Server instance | Phase 2 | SQL script execution & schema query | Pending |
| **AC-03** | Design System | Red-and-white theme (#F84464) applied consistently across all views | Phase 2 | CSS token verification & UI inspection | Pending |
| **AC-04** | Responsive Layout | Layout adapts responsively to mobile (375px), tablet (768px), and desktop (1280px) | Phase 2/3 | Viewport layout testing | Pending |
| **AC-05** | User Registration | Registration with name, email, phone, and hashed password (PBKDF2) | Phase 3 | Register form submission & DB hash check | Pending |
| **AC-06** | User Authentication | Cookie-based login and logout with Secure, HttpOnly, and SameSite attributes | Phase 3 | Login session & cookie inspector | Pending |
| **AC-07** | Role Authorization | Admin role access restricted to [Authorize(Roles="Admin")]; users redirected | Phase 3/6 | Unauthorized HTTP 403/Redirect checks | Pending |
| **AC-08** | Movie Discovery | Movie catalogue with server-side search, genre, language, and format filters | Phase 4 | Search query execution with filter params | Pending |
| **AC-09** | Showtime Schedule | Theatre and showtime selection with date strip, hall format, and ticket pricing | Phase 4 | Showtime query and UI date strip | Pending |
| **AC-10** | Interactive Seat Map | Visual seat map displaying Silver, Gold, Platinum rows with real availability | Phase 4 | Seat map rendering & state toggles | Pending |
| **AC-11** | Seat Hold Concurrency | Server-enforced time-limited seat hold (8 minutes) with single SQL transaction | Phase 4 | Concurrency test with competing requests | Pending |
| **AC-12** | Event Discovery | Event catalogue featuring concert, comedy, workshop, and sports filters | Phase 5 | Event query execution & card rendering | Pending |
| **AC-13** | Ticket Tiers & Limit | Multi-tier event tickets with capacity check and max-per-order boundaries | Phase 5 | Tier selection and stepper limit validation | Pending |
| **AC-14** | Event Inventory Guard | Atomic decrement prevents overselling when last tickets are booked simultaneously | Phase 5 | Parallel ticket claim load test | Pending |
| **AC-15** | Server-Side Pricing | Calculation of base amount, convenience fee, and GST strictly on server | Phase 4/5 | Price tamper rejection test | Pending |
| **AC-16** | Simulated Payment | Test payment gateway supporting Success, Failure, and Timeout simulation | Phase 4/5 | Payment flow execution with demo notice | Pending |
| **AC-17** | Inventory Restoration | Failed payments, cancellations, or expired holds restore inventory automatically | Phase 4/5 | Failed payment test & seat release check | Pending |
| **AC-18** | Booking Confirmation | Printable confirmation with unique reference (e.g., SSP-2026-XXXX) | Phase 4/5 | Confirmation page render & reference format | Pending |
| **AC-19** | My Bookings Ledger | User booking history showing Upcoming, Past, and Cancelled statuses | Phase 4/5 | Booking list query filtered by user ID | Pending |
| **AC-20** | User Cancellation | User can cancel booking before start cutoff; seats restored immediately | Phase 4/5 | Cancel action execution & status change | Pending |
| **AC-21** | IDOR Protection | User cannot view, download, or cancel bookings belonging to other users | Phase 8 | Cross-user booking ID tamper test | Pending |
| **AC-22** | Admin Management | Admin CRUD for movies, events, theatres, showtimes, seats, and bookings | Phase 6 | Admin CRUD form execution & persistence | Pending |
| **AC-23** | Data Verification Tag | Data sources tracked with VerificationStatus (Verified, Partially Verified, Demo) | Phase 7 | Catalogue metadata query & UI badges | Pending |
| **AC-24** | Security Hardening | Anti-forgery tokens, parameterized SQL only, Razor encoding, no plain secrets | Phase 8 | Security audit & SQL injection test suite | Pending |
| **AC-25** | Git and Docs Delivery | 80+ atomic commits, milestone tags, complete README, and demo script | Phase 9 | `git rev-list --count main` & tag verification | Pending |
