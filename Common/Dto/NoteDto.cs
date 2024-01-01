using Common.Model;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Common.DTO
{
    public class NoteDto
    {
        public string Title { get; set; } = string.Empty;

        [AllowHtml]
        public string Text { get; set; } = string.Empty;

        public bool IsEncrypted { get; set; } = false;

        public string? Password { get; set; }

        public bool IsPublic { get; set; } = false;

        public string? UsersWithAccess { get; set; } = string.Empty;
    }
}
