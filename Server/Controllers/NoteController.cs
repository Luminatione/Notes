using Common.DTO;
using Common.Model;
using Common.Services;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IHtmlSanitizer = Ganss.Xss.IHtmlSanitizer;

namespace Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class NoteController : Controller
    {
        private readonly INoteService _noteService;
        private readonly IAuthService _authService;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public NoteController(INoteService noteService, IAuthService authService, IHtmlSanitizer htmlSanitizer)
        {
            _noteService = noteService;
            _authService = authService;
            _htmlSanitizer = htmlSanitizer;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var response = await _noteService.GetByUser(GetUserId());
            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            ViewBag.Notes = response.Value;
            return View(response.Value);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] NoteDto note)
        {
            if (ModelState.IsValid)
            {
                note.Text = _htmlSanitizer.Sanitize(note.Text);
                var response = await _noteService.Create(note, GetUserId());
                if (!response.IsSuccess)
                {
                    return BadRequest(response.Message);
                }
                return RedirectToAction("Index");
            }
            return View(note);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (!(await _noteService.HasAccess(GetUserId(), id)).Value)
            {
                return BadRequest("You don't have access to this note");
            }
            var response = await _noteService.Get(id);
            if (response.IsSuccess)
            {
                return View(response.Value);
            }
            return Problem(response.Message);
        }

        [HttpGet("Decrypt/{id}")]
        public async Task<IActionResult> Decrypt(int id, string password)
        {
            var response = await _noteService.Get(id);
            if ((await _noteService.HasAccess(GetUserId(), id)).Value == false)
            {
                return BadRequest("You don't have access to this note");
            }
            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            if (response.Value.IsEncrypted)
            {
                return Ok((await _noteService.Decrypt(response.Value.Id, password)).Value);
            }
            return Ok(response.Value.Text);
        }

        private int GetUserId()
        {
            return int.Parse(User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}
