public class RefreshToken
{
    public string? Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public string? UserId { get; set; } // Assuming you're associating the refresh token with a user
}
