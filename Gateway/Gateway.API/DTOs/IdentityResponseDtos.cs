namespace Gateway.API.DTOs
{   
    public class JwtResponsetDto 
    {
        public string? jwtToken {  get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsActive { get; set; }
        public RoleType Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalPages { get; init; }
    }
}
