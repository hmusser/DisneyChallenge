using System.ComponentModel.DataAnnotations;

namespace DisneyChallenge.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
