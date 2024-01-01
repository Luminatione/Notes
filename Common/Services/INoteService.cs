using Common.DTO;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public interface INoteService
    {
        public Task<Response> Create(NoteDto note, int userId);
        public Task<Response<Note>> Get(int id);
        public Task<Response<List<Note>>> GetAll();
        public Task<Response> Update(Note note);
        public Task<Response<bool>> HasAccess(int userId, int noteId);
        public Task<Response<List<Note>>> GetByUser(int userId);
        public Task<Response<List<string>>> GetTitles();
        public Task<Response<string>> Decrypt(int id, string password);
    }
}
