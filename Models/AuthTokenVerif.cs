namespace AuthVerif.Models
{
    public class AuthTokenVerif
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
        public User user { get; set; }
    }
}
