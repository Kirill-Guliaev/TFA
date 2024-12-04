using System.ComponentModel.DataAnnotations;

namespace TFA.Storage;

public class Forum
{
    [Key]
    public Guid ForumId { get; set; }   

    public string Title { get; set; }
}