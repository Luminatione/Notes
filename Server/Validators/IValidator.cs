using Common;

namespace Server.Validators
{
    public interface IValidator<T>
    {
        Response Validate(T obj);
    }
}
