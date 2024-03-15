namespace NestHR.Models
{
    public class Users
    {
        public int UserId { get; set; }

        public int UserNum { get; set; }

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int? Lang { get; set; }

        public int? Theam { get; set; }
    }
}
