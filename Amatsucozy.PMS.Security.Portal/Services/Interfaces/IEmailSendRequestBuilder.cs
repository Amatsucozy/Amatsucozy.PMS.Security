using Amatsucozy.PMS.Email.Contracts;

namespace Amatsucozy.PMS.Security.Portal.Services;

public interface IEmailSendRequestBuilder
{
    EmailSendRequest GetConfirmEmailSendRequest(
        string toEmail,
        string username,
        string confirmationLink,
        string companyName);
}
