# Handoff Note: Agent 2 (UI/UX) & Agent 5 (Database) to Agent 3 (Frontend) & Agent 4 (Backend)

**Date:** October 2026  
**From:** Agent 2 (UI/UX Designer) & Agent 5 (Database Engineer)  
**To:** Agent 3 (Frontend Developer) & Agent 4 (Backend Developer)  
**Status:** Phase 2 Complete and Verified  

---

## 1. What Was Delivered
1. **Physical SQL Server Database (`ShowSphereDb`):**
   - Executed `01_schema.sql`, `02_indexes.sql`, and `03_seed_reference.sql` on real Microsoft SQL Server 2022 LocalDB.
   - 19 relational tables created with primary/foreign keys and check constraints.
   - Filtered index on `ShowtimeSeats(Status, HoldExpiresAtUtc)` and `Bookings(Status, ExpiresAtUtc)` for high-velocity hold cleanup.
   - 240 physical seats (Silver, Gold, Platinum rows) populated across 4 screens in Rajkot.
2. **Project Solution & Data Access Foundation:**
   - Solution `TicketBooking.slnx` with `TicketBooking.Web` (MVC net10.0) and `TicketBooking.Tests` (xUnit net10.0).
   - Pure ADO.NET connection factory (`IDbConnectionFactory`, `DbConnectionFactory`) with `Microsoft.Data.SqlClient`.
   - `DatabaseInitializer` configured in `Program.cs` for startup schema checks and PBKDF2 Admin user seeding.
3. **ShowSphere UI/UX System:**
   - Full CSS design tokens in `site.css` (primary `#F84464`, hover `#D82245`, dark `#1F2533`).
   - Sticky navbar with Rajkot city selector, search pill, and category navigation.
   - Persistent college demonstration notice on all layouts.
   - Component styles: Movie cards, Event cards with date chips, Seat map grid with screen arch, Steppers, Checkout cards.
4. **Automated Test Harness:**
   - 4 unit tests passing cleanly in `tests/TicketBooking.Tests/FoundationTests.cs`.

---

## 2. Agreed Contracts and Rules
- **Backend Team (Agent 4):**
  - Use ADO.NET parameterized queries exclusively (`@Param` with `cmd.Parameters.AddWithValue` or explicit types).
  - All seat holds and event ticket deductions must execute within explicit database transactions.
  - Password hashing must use ASP.NET Core `PasswordHasher<User>`.
  - Cookie authentication scheme with `HttpOnly`, `SameSite = Lax`, and `SecurePolicy = Always`.
- **Frontend Team (Agent 3):**
  - Use CSS classes from `site.css`: `.btn-primary-showsphere`, `.btn-outline-showsphere`, `.movie-card`, `.event-card`, `.seat-btn`, `.seat-selected`, `.ticket-stepper`.
  - Every form must include `@Html.AntiForgeryToken()`.
  - Seat selections must post array of seat IDs to `/booking/hold-seats`.
  - Event selections must post category ID and quantity to `/booking/hold-tickets`.

---

## 3. Verification Evidence
- `sqlcmd` execution of `INFORMATION_SCHEMA.TABLES`: 19 tables verified.
- `sqlcmd` counts: Cities = 5, Theatres = 3, Screens = 4, Seats = 240.
- `dotnet build`: Succeeded with 0 errors.
- `dotnet test`: 4 passed, 0 failed.
- Defect Log: DEF-001 documented and resolved.
