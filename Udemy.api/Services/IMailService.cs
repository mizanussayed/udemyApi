using Udemy.api.Models.Dto;

namespace Udemy.api.Services;
public interface IMailService
{
    Task SendAsync(MailRequest request);
}
