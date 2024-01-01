using Common.Services;
using Ganss.Xss;
using System.Web;
using IHtmlSanitizer = Common.Services.IHtmlSanitizer;

namespace Server.Services
{
    public class HtmlSanitizer : IHtmlSanitizer
    {
        private readonly string[] _allowedTags = new string[] { "#text", "#document", "p", "b", "i", "u", "strong", "em", "strike", "code", "pre", "br", "ul", "ol", "li", "img", "h1", "h2", "h3", "h4", "h5", "h6" };
        public string Sanitize(string html)
        {
            var sanitizer = new Ganss.Xss.HtmlSanitizer();
            var sanitized = sanitizer.Sanitize(html);
            return sanitized;
        }
    }
}
