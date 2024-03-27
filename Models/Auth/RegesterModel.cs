using System.ComponentModel.DataAnnotations;

namespace NestHR.Models.Auth
{
    public class RegesterModel
    {
        public required int UserNum { get; set; }

        public required string UserName { get; set; }

        public required string password { get; set;}
    }
}
