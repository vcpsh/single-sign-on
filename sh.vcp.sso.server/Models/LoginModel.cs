using System.ComponentModel.DataAnnotations;

namespace sh.vcp.sso.server.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public bool Remember { get; set; }
        
        public string ReturnUrl { get; set; }
    }
}