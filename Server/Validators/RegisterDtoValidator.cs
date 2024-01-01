using Common;
using Common.DTO;
using Server.DTO;

namespace Server.Validators
{
    public class RegisterDtoValidator : IValidator<RegisterDto>
    {
        public Response Validate(RegisterDto user)
        {
            if (user.Username == null || user.Password == null || user.ConfirmPassword == null)
            {
                return new Response { IsSuccess = false, Message = "All fields are required." };
            }
            if(user.Password != user.ConfirmPassword)
            {
                return new Response { IsSuccess = false, Message = "Passwords do not match." };
            }
            if(user.Password.Length < 8)
            {
                return new Response { IsSuccess = false, Message = "Password must be at least 8 characters long." };
            }
            if (user.Password.Length > 20)
            {
                return new Response { IsSuccess = false, Message = "Password must be at at most 20 characters long." };
            }
            if (!user.Password.Any(char.IsUpper))
            {
                return new Response { IsSuccess = false, Message = "Password must contain at least one uppercase letter." };
            }
            if (user.Username.Length < 6)
            {
                return new Response { IsSuccess = false, Message = "Username must be at least 3 characters long." };
            }
            if (user.Username.Length > 20)
            {
                return new Response { IsSuccess = false, Message = "Username must be at most 20 characters long." };
            }
            return new Response { IsSuccess = true, Message = "Data is valid" };
            
        }
    }
}
