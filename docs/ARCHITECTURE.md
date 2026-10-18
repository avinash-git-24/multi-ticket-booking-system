# System Architecture and Technical Design
**Application Name:** ShowSphere (Movie and Event Ticket Booking System)  
**Target Organization:** College Technical Demonstration  
**Author:** Avinash Kumar  
**Date:** October 2026  

---

## 1. Environment and Runtime Audit

| Component | Detected Specification | Verification Command / Status |
| :--- | :--- | :--- |
| Operating System | Windows 11 Pro (Build 10.0.26300, win-x64) | `dotnet --info` (Verified) |
| .NET SDK | 10.0.401 (Commit e34a38d2ae) | `dotnet --list-sdks` (Verified) |
| ASP.NET Core Runtime | Microsoft.AspNetCore.App 10.0.12 | `dotnet --list-runtimes` (Verified) |
| .NET Core Runtime | Microsoft.NETCore.App 10.0.12, 8.0.28 | `dotnet --list-runtimes` (Verified) |
| SQL Server Instance | Microsoft SQL Server 2022 LocalDB (v17.0.4025.3) | `sqllocaldb info MSSQLLocalDB` (Running) |
| Default Connection String | `Server=(localdb)\\MSSQLLocalDB;Database=ShowSphereDb;Trusted_Connection=True;TrustServerCertificate=True;` | LocalDB Verified |
| Version Control | Git 2.55.0.windows.3 | `git --version` (Verified) |
| Remote Repository | https://github.com/avinash-git-24/multi-ticket-booking-system.git | `git remote -v`, `git ls-remote origin` (Verified Empty) |

---

## 2. Technology Justification and Stack Selection

### Why ASP.NET Core MVC (And Not ASP.NET Web Forms)
- **ASP.NET Web Forms Limitations:** ASP.NET Web Forms is tied to the legacy .NET Framework (up to 4.8.x) and Windows IIS. It relies on heavy ViewState, simulated event loops, page lifecycles, and tight coupling of HTML to server controls. It cannot run on modern cross-platform .NET (Core / .NET 8 / .NET 10).
- **ASP.NET Core MVC Advantages:** Provides a clean separation of concerns (Model-View-Controller), native dependency injection, testability, high-performance asynchronous request handling, modern Razor syntax, and full control over emitted HTML, CSS, and JavaScript.
- **Architectural Requirement:** This project uses ASP.NET Core MVC on the current .NET LTS/Current SDK (`net10.0`).

### Complete Technology Stack
- **Backend Framework:** C# 13 / ASP.NET Core MVC (`net10.0`)
- **Data Access:** Pure ADO.NET (`Microsoft.Data.SqlClient`) using parameterized commands exclusively. No Entity Framework and no Dapper.
- **Database Engine:** Microsoft SQL Server 2022 (LocalDB / Express / Docker compatible)
- **Authentication:** Cookie Authentication (`CookieAuthenticationDefaults.AuthenticationScheme`) with ASP.NET Core `PasswordHasher<TUser>` (PBKDF2 with SHA-256 and HMAC). No plain-text passwords or custom unverified hashes.
- **Frontend Presentation:** Razor Views (.cshtml), HTML5, CSS3, JavaScript (ES6+), customized Bootstrap 5 with custom CSS design tokens (BookMyShow inspired red/white theme, #F84464 primary).
- **Asset Integrity:** Original SVG vector icons, CSS artwork, and legitimate metadata with strict attribution.
- **Testing:** xUnit, Microsoft.AspNetCore.Mvc.Testing, and custom concurrency test harness.

---

## 3. Layered Architecture and Module Boundaries

The solution strictly enforces a directional 4-layer dependency model:

```
[ Presentation Layer: Razor Views, TagHelpers, Custom CSS/JS ]
                     │
                     ▼
[ Controller Layer: Route Handlers, Model Binding, Anti-Forgery ]
                     │
                     ▼
[ Service / Business Layer: Validation, Pricing Engine, Hold Timer, Concurrency Control ]
                     │
                     ▼
[ Repository / Data Layer: Pure ADO.NET, Parameterized SQL, Explicit Transactions ]
                     │
                     ▼
[ Database: SQL Server 2022 (Tables, Constraints, Indexes, Triggers/Locks) ]
```

### Architectural Principles
1. **Server Authority:** Browser inputs (seat IDs, ticket counts) are untrusted. All prices, totals, tax rates, seat validity, and booking eligibility are computed and enforced strictly by server-side domain services.
2. **Strict Separation of Models:** Domain entities represent database records. ViewModels represent UI inputs and display needs. Controllers map between them via service methods; domain entities are never exposed raw in view contracts.
3. **Connection Lifecycle:** Every database interaction acquires a new connection through `IDbConnectionFactory`, opens it asynchronously, executes parameterized SQL within `using` scopes, and disposes resources cleanly.
4. **Transaction Integrity:** Seat reservation and event ticket deduction occur inside serializable/repeatable read transactions with locking hints to eliminate race conditions.
5. **No Em-Dashes Rule:** All code, comments, log messages, and documentation avoid Unicode em-dashes entirely.

---

## 4. Route Map

### Public and Consumer Routes
- `GET /` : Home landing page with city selector, carousel, recommended movies, and popular events
- `GET /movies` : Movie catalogue with search, genre, language, and format filters
- `GET /movies/{id}` : Movie details, synopsis, cast, certificate, language, and active showtimes
- `GET /movies/{id}/showtimes` : Theatre and showtime selector with date tabs
- `GET /booking/seats/{showtimeId}` : Interactive seat selection map with real-time availability
- `POST /booking/hold-seats` : Acquire temporary time-limited hold on selected seats
- `GET /events` : Event catalogue with category, date, and price filters
- `GET /events/{id}` : Event details, venue details, and ticket tier selector
- `POST /booking/hold-tickets` : Reserve event tickets with quantity boundary checks
- `GET /booking/checkout/{bookingReference}` : Order summary, server-verified price breakdown, simulated payment form
- `POST /booking/process-payment` : Simulated payment gateway submission (Success / Failure / Timeout simulation)
- `GET /booking/confirmation/{bookingReference}` : Printable confirmation with QR placeholder and verification badge
- `GET /account/login` : Login form (ReturnUrl support)
- `POST /account/login` : Authenticate user credentials and issue secure authentication cookie
- `GET /account/register` : User registration form
- `POST /account/register` : Validate registration, hash password, and create user account
- `POST /account/logout` : Sign out user and revoke cookie
- `GET /account/profile` : User profile information
- `GET /account/my-bookings` : User booking history (Upcoming, Past, Cancelled)
- `POST /booking/cancel/{bookingReference}` : Cancel booking (ownership and cutoff time validated, restores seats)

### Admin Routes (`[Authorize(Roles = "Admin")]`)
- `GET /admin` : Dashboard with real-time statistics (sales, bookings, inventory counts)
- `GET /admin/movies` : Movie inventory management
- `GET /admin/movies/create`, `POST /admin/movies/create` : Add new movie
- `GET /admin/movies/edit/{id}`, `POST /admin/movies/edit/{id}` : Update movie details
- `POST /admin/movies/delete/{id}` : Soft-delete or archive movie
- `GET /admin/events` : Event inventory management
- `GET /admin/events/create`, `POST /admin/events/create` : Add new event and ticket tiers
- `GET /admin/theatres` : Manage theatres and screens
- `GET /admin/showtimes` : Schedule movie showtimes with hall assignment and pricing
- `POST /admin/showtimes/create` : Create showtime with seat matrix generation
- `GET /admin/bookings` : System-wide booking ledger with search and status filters
- `POST /admin/bookings/cancel/{reference}` : Admin booking override cancellation
- `GET /admin/catalogue-sync` : Catalogue synchronization and verification metadata review
- `POST /admin/catalogue-sync/trigger` : Trigger metadata refresh from legitimate sources

---

## 5. Security Architecture
1. **Password Security:** ASP.NET Core `PasswordHasher<User>` utilizing PBKDF2 with HMAC-SHA256, 100,000 iterations, and unique cryptographic salt per user.
2. **Cookie Flags:** `HttpOnly = true`, `SecurePolicy = Always`, `SameSite = SameSiteMode.Lax`, sliding expiration enabled.
3. **Anti-Forgery:** Every state-changing form incorporates an `[ValidateAntiForgeryToken]` token validated by ASP.NET Core middleware.
4. **SQL Injection Defense:** 100% parameterized queries via `SqlCommand.Parameters.AddWithValue` or explicit `SqlParameter` types. Zero string concatenation.
5. **Cross-Site Scripting (XSS):** Razor automatic HTML encoding by default. No `Html.Raw` usage on user-provided inputs.
6. **Insecure Direct Object Reference (IDOR):** Booking retrieval and cancellation enforce user ownership matching `User.FindFirstValue(ClaimTypes.NameIdentifier)`.
7. **Simulated Payment Notice:** Prominent notice displayed on checkout and confirmation pages: "College Demonstration - No Real Payment Processed".
