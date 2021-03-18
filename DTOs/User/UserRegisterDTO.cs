using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WalletQ.DTOs.User
{
    public class UserRegisterDTO
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        [Required]
        [MaxLength(25)]
        public string Surname { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [StringLength(50,MinimumLength =7)]
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }

    }
}
