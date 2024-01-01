using Common;
using Common.DTO;
using Common.Model;
using Common.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Server.Services
{
    public class NoteService : INoteService
    {
        private readonly DataBaseContext _dataBaseContext;
        private readonly IAuthService _authService;

        public NoteService(DataBaseContext dataBaseContext, IAuthService authService)
        {
            _dataBaseContext = dataBaseContext;
            _authService = authService;
        }

        public async Task<Response> Create(NoteDto note, int userId)
        {
            try
            {
                List<string> users = note.UsersWithAccess?.Split(',').ToList() ?? new List<string>();
                if (note.IsPublic && note.IsEncrypted)
                {
                    return new Response { Message = "Public note can not be encrypted", IsSuccess = false };
                }
                Note newNote = new Note
                {
                    Owner = await _dataBaseContext.Users.FirstOrDefaultAsync(e => e.Id == userId),
                    Title = note.Title,
                    Text = note.Text,
                    IsEncrypted = note.IsEncrypted,
                    IsPublic = note.IsPublic,
                    PasswordHash = note.Password != null ? HashPassword(note.Password) : new byte[0],
                    UsersWithAccess = await _dataBaseContext.Users.Where(e => users.Contains(e.Username)).ToListAsync()
                };

                if (note.IsEncrypted)
                {
                    newNote.Text = Encrypt(note.Text, note.Password);
                }

                await _dataBaseContext.Notes.AddAsync(newNote);
                await _dataBaseContext.SaveChangesAsync();
                return new Response { IsSuccess = true, Message = "Note Added" };
            }
            catch (Exception e)
            {
                return new Response { Message = e.Message, IsSuccess = false };
            }
        }

        public async Task<Response<Note>> Get(int id)
        {
            try
            {
                var note = await _dataBaseContext.Notes.Include("Owner").Include("UsersWithAccess").FirstOrDefaultAsync(e => e.Id == id);
                if (note == null)
                {
                    return new Response<Note> { IsSuccess = false, Message = "No such note found" };
                }
                return new Response<Note> { Value = note, IsSuccess = true, Message = "Note Found" };
            }
            catch (Exception e)
            {
                return new Response<Note> { Message = e.Message, IsSuccess = false };
            }
        }

        public async Task<Response<List<Note>>> GetAll()
        {
            try
            {
                return new Response<List<Note>> { Value = await _dataBaseContext.Notes.ToListAsync(), IsSuccess = true, Message = "Success" };
            }
            catch (Exception e)
            {
                return new Response<List<Note>> { Message = e.Message, IsSuccess = false };
            }
        }

        public async Task<Response<List<Note>>> GetByUser(int userId)
        {
            var user = await _dataBaseContext.Users.FirstOrDefaultAsync(e => e.Id == userId);
            if (user == null)
            {
                return new Response<List<Note>> { IsSuccess = false, Message = "No such user found" };
            }
            try
            {
                return new Response<List<Note>> { Value = await _dataBaseContext.Notes.Include("Owner").Where(e => e.Owner.Id == userId).ToListAsync(), IsSuccess = true, Message = "Success" };
            }
            catch (Exception e)
            {
                return new Response<List<Note>> { Message = e.Message, IsSuccess = false };
            }
        }

        public async Task<Response<List<string>>> GetTitles()
        {
            try
            {
                return new Response<List<string>> { Value = (await _dataBaseContext.Notes.ToListAsync()).Select(e => e.Title).ToList(), IsSuccess = true, Message = "Success" };
            }
            catch (Exception e)
            {
                return new Response<List<string>> { Message = e.Message, IsSuccess = false };
            }
        }

        public async Task<Response<bool>> HasAccess(int userId, int noteId)
        {
            var note = await Get(noteId);
            try
            {
                if (!note.IsSuccess)
                {
                    return new Response<bool> { IsSuccess = false, Value = false, Message = "No such user" };
                }

                bool hasAccess = note.Value.UsersWithAccess.Any(e => e.Id == userId) || note.Value.IsPublic || note.Value.Owner.Id == userId;

                return new Response<bool> { IsSuccess = true, Value = hasAccess, Message = "Checked access" };
            }
            catch (Exception e)
            {
                return new Response<bool> { Message = e.Message, IsSuccess = false };
            }
        }

        public async Task<Response> Update(Note note)
        {
            var currentNote = await _dataBaseContext.Notes.FirstOrDefaultAsync(e => e.Id == note.Id);
            if (currentNote == null)
            {
                return new Response { IsSuccess = false, Message = "No such note found" };
            }
            currentNote.Owner = note.Owner;
            currentNote.Title = note.Title;
            currentNote.Text = note.Text;
            currentNote.IsEncrypted = note.IsEncrypted;
            currentNote.UsersWithAccess = note.UsersWithAccess;
            currentNote.IsPublic = note.IsPublic;
            currentNote.PasswordHash = note.PasswordHash;
            _dataBaseContext.Notes.Update(currentNote);
            return new Response { IsSuccess = true, Message = "Note Updated" };
        }

        private byte[] HashPassword(string password)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, new byte[16], 10000, HashAlgorithmName.SHA256))
            {
                byte[] hashedBytes = pbkdf2.GetBytes(32);
                return hashedBytes;
            }
        }

        private string Encrypt(string text, string password)
        {
            byte[] encrypted;
            byte[] IV;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = HashPassword(password);
                aesAlg.GenerateIV();
                IV = aesAlg.IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(IV, 0, IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            swEncrypt.Write(text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        public async Task<Response<string>> Decrypt(int id, string password)
        {
            Note note = (await Get(id)).Value;
            byte[] passwordHash = HashPassword(password);
            if(!Enumerable.SequenceEqual(note.PasswordHash, passwordHash))
            {
                return new Response<string>() { IsSuccess = false, Message = "Wrong password" };
            }

            byte[] encryptedBytes = Convert.FromBase64String(note.Text);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = passwordHash;
                byte[] IV = new byte[aesAlg.BlockSize / 8];
                Array.Copy(encryptedBytes, IV, IV.Length);
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes, IV.Length, encryptedBytes.Length - IV.Length))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return new Response<string>() { Value = srDecrypt.ReadToEnd(), IsSuccess = true, Message = "Decrypted" };
                        }
                    }
                }
            }
        }
    }
}
