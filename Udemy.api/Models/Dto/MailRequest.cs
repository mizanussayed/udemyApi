namespace Udemy.api.Models.Dto;

public class MailRequest
{
    public required string To { get; set; }
    public required string Subject { get; set; }
    public string? Body { get; set; }
    public string? From { get; set; }
}
