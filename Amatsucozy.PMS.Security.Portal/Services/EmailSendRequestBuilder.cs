using Amatsucozy.PMS.Email.Contracts;

namespace Amatsucozy.PMS.Security.Portal.Services;

public sealed class EmailSendRequestBuilder : IEmailSendRequestBuilder
{
    public EmailSendRequest GetConfirmEmailSendRequest(
        string toEmail,
        string username,
        string confirmationLink,
        string companyName)
    {
        return new EmailSendRequest
        {
            Key = "Account.Confirmation",
            PlaceholderMappings = new Dictionary<string, string>
            {
                { "Username", username },
                { "ConfirmationLink", confirmationLink },
                { "CompanyName", companyName }
            },
            To = new[] { toEmail },
            Cc = Enumerable.Empty<string>(),
            Bcc = Enumerable.Empty<string>()
        };
    }
}