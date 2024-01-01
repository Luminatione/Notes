using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Common.Model
{
    public class Note
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, RegularExpression("^[a-zA-Z0-9]*$"), MaxLength(32), MinLength(4)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000), AllowHtml]
        public string Text { get; set; } = string.Empty;

        [Required]
        public bool IsEncrypted { get; set; } = false;

        public byte[]? PasswordHash { get; set; }

        [Required]
        public User Owner { get; set; }

        [Required]
        public bool IsPublic { get; set; } = false;

        [Required]
        public List<User> UsersWithAccess { get; set; } = new List<User>();
    }
}
