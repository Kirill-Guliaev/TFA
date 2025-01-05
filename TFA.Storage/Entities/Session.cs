using System.ComponentModel.DataAnnotations.Schema;

namespace TFA.Storage.Entities;

public class Session
{
    public Guid SessionId { get; set; }

    public Guid UserId { get; set; }    

    public DateTimeOffset ExpireAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}
