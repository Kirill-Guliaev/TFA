using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFA.Domain.Models;

public class Topic
{
    public  Guid Id { get; set; }

    public string Title { get; set; }

    public DateTimeOffset CreatedAd { get; set; }

    public string Author { get; set; }
}
