namespace TicketBooking.Tests;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using TicketBooking.Web.Data.Repositories;
using TicketBooking.Web.Models;
using TicketBooking.Web.Models.DTOs;
using TicketBooking.Web.Services;

public class AuthTests
{
    private class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();
        private readonly Dictionary<int, List<string>> _userRoles = new();
        private int _nextId = 1;

        public Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = _users.FirstOrDefault(u => u.UserId == userId);
            if (user != null && _userRoles.TryGetValue(user.UserId, out var roles))
            {
                user.Roles = roles.ToList();
            }
            return Task.FromResult(user);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = _users.FirstOrDefault(u => string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));
            if (user != null && _userRoles.TryGetValue(user.UserId, out var roles))
            {
                user.Roles = roles.ToList();
            }
            return Task.FromResult(user);
        }

        public Task<int> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            user.UserId = _nextId++;
            _users.Add(user);
            _userRoles[user.UserId] = new List<string>();
            return Task.FromResult(user.UserId);
        }

        public Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            var existing = _users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existing == null) return Task.FromResult(false);

            existing.FullName = user.FullName;
            existing.PhoneNumber = user.PhoneNumber;
            existing.PreferredCityId = user.PreferredCityId;
            return Task.FromResult(true);
        }

        public Task<IReadOnlyList<string>> GetRolesByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (_userRoles.TryGetValue(userId, out var roles))
            {
                return Task.FromResult<IReadOnlyList<string>>(roles.ToList());
            }
            return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        }

        public Task<bool> AssignRoleAsync(int userId, string roleName, CancellationToken cancellationToken = default)
        {
            if (!_userRoles.TryGetValue(userId, out var roles))
            {
                roles = new List<string>();
                _userRoles[userId] = roles;
            }

            if (!roles.Contains(roleName))
            {
                roles.Add(roleName);
            }

            return Task.FromResult(true);
        }
    }

    [Fact]
    public async Task RegisterAsync_ValidRequest_CreatesUserWithHashedPasswordAndUserRole()
    {
        var repo = new FakeUserRepository();
        var hasher = new PasswordHasher<User>();
        var service = new UserService(repo, hasher, NullLogger<UserService>.Instance);

        var request = new RegisterRequestDto
        {
            FullName = "Aarav Sharma",
            Email = "aarav.sharma@example.com",
            PhoneNumber = "+919876543210",
            Password = "SecurePassword123!",
            PreferredCityId = 1
        };

        var (success, error, user) = await service.RegisterAsync(request);

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(user);
        Assert.Equal("aarav.sharma@example.com", user.Email);
        Assert.Contains("User", user.Roles);

        var stored = await repo.GetByEmailAsync("aarav.sharma@example.com");
        Assert.NotNull(stored);
        Assert.NotEqual("SecurePassword123!", stored.PasswordHash);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ReturnsError()
    {
        var repo = new FakeUserRepository();
        var hasher = new PasswordHasher<User>();
        var service = new UserService(repo, hasher, NullLogger<UserService>.Instance);

        var request1 = new RegisterRequestDto
        {
            FullName = "First User",
            Email = "duplicate@example.com",
            Password = "Password123!"
        };
        await service.RegisterAsync(request1);

        var request2 = new RegisterRequestDto
        {
            FullName = "Second User",
            Email = "duplicate@example.com",
            Password = "AnotherPassword123!"
        };
        var (success, error, user) = await service.RegisterAsync(request2);

        Assert.False(success);
        Assert.Equal("An account with this email address already exists.", error);
        Assert.Null(user);
    }

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsSuccessAndRoles()
    {
        var repo = new FakeUserRepository();
        var hasher = new PasswordHasher<User>();
        var service = new UserService(repo, hasher, NullLogger<UserService>.Instance);

        var registerReq = new RegisterRequestDto
        {
            FullName = "Bhavik Dave",
            Email = "bhavik.dave@example.com",
            Password = "CorrectPassword123!"
        };
        await service.RegisterAsync(registerReq);

        var (success, error, user) = await service.AuthenticateAsync("bhavik.dave@example.com", "CorrectPassword123!");

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(user);
        Assert.Equal("Bhavik Dave", user.FullName);
        Assert.Contains("User", user.Roles);
    }

    [Fact]
    public async Task AuthenticateAsync_WrongPassword_ReturnsFailure()
    {
        var repo = new FakeUserRepository();
        var hasher = new PasswordHasher<User>();
        var service = new UserService(repo, hasher, NullLogger<UserService>.Instance);

        var registerReq = new RegisterRequestDto
        {
            FullName = "Bhavik Dave",
            Email = "bhavik.dave@example.com",
            Password = "CorrectPassword123!"
        };
        await service.RegisterAsync(registerReq);

        var (success, error, user) = await service.AuthenticateAsync("bhavik.dave@example.com", "WrongPassword999!");

        Assert.False(success);
        Assert.Equal("Invalid email address or password.", error);
        Assert.Null(user);
    }

    [Fact]
    public async Task AuthenticateAsync_NonExistentEmail_ReturnsFailure()
    {
        var repo = new FakeUserRepository();
        var hasher = new PasswordHasher<User>();
        var service = new UserService(repo, hasher, NullLogger<UserService>.Instance);

        var (success, error, user) = await service.AuthenticateAsync("doesnotexist@example.com", "SomePassword!");

        Assert.False(success);
        Assert.Equal("Invalid email address or password.", error);
        Assert.Null(user);
    }
}
