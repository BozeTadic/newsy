namespace Newsy.Api.Domain;

public class Author
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public ICollection<Article>? Articles { get; set; }
}