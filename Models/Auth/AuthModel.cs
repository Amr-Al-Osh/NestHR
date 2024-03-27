namespace NestHR.Models.Auth
{
    public class AuthModel
    {

        public string Message { get; set; }
        public bool IsAuthuntecated { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public List<string> Roles { get; set; }
        public DateTime ExpierOn { get; set; }
    }
}
