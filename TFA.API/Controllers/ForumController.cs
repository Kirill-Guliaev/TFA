using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFA.Storage;

namespace TFA.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ForumController : ControllerBase
{
    /// <summary>
    /// Get lost on every forums
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(string[]))]
    public async Task<IActionResult> GetForums(
        [FromServices] ForumDbContext dbContext,
        CancellationToken cancellationToken)
    {
        await dbContext.Forums.AddAsync(new Forum() { ForumId =  Guid.NewGuid(), Title = "test title" });
        await dbContext.SaveChangesAsync();
        var forumTitles = await dbContext.Forums.Select(f => f.Title).ToArrayAsync(cancellationToken);
        return Ok(forumTitles);
    }
}
