public class RequestContext
{
    public int? UserId { get; set; }
    public List<string> Roles { get; set; } = new();
    public string Email { get; set; }
}
