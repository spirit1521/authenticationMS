using System.Threading.Tasks;

namespace AuthMicroservice.Application.Interface
{
    public interface IEmailService
    {
        Task SendResetToken(string email, string token);
    }
}