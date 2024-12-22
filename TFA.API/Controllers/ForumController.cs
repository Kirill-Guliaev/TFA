using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TFA.API.Models;
using TFA.Domain.UseCases.CreateForum;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Domain.UseCases.GetTopics;

namespace TFA.API.Controllers;

[ApiController]
[Route("forums")]
public class ForumController : ControllerBase
{
    /// <summary>
    /// Get list on every forums
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(Name = nameof(GetForums))]
    [ProducesResponseType(200, Type = typeof(ForumResponse[]))]
    public async Task<IActionResult> GetForums(
        [FromServices] IGetForumsUseCase useCase,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var forums = await useCase.ExecuteAsync(cancellationToken);
        return Ok(forums.Select(mapper.Map<Forum>));
    }

    [HttpPost]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    [ProducesResponseType(201, Type = typeof(Forum))]
    public async Task<IActionResult> CreateForum(
    [FromBody] CreateForum request,
    [FromServices] ICreateForumUseCase useCase,
     [FromServices] IMapper mapper,
    CancellationToken cancellationToken)
    {
        var command = new CreateForumCommand(request.Title);
        var forum = await useCase.Execute(command, cancellationToken);

        return CreatedAtRoute(nameof(GetForums), mapper.Map<Forum>(forum));
    }

    [HttpPost("{forumId:guid}/topics")]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(201, Type = typeof(Topic))]
    public async Task<IActionResult> CreateTopic(
        Guid forumId,
        [FromBody] CreateTopic request,
        [FromServices] ICreateTopicUseCase useCase,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var topic = await useCase.ExecuteAsync(new(forumId, request.Title), cancellationToken);
        return CreatedAtRoute(nameof(GetForums), mapper.Map<Topic>(topic));
    }

    [HttpGet("{forumId:guid}/topics")]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetTopics(
        [FromRoute] Guid forumId,
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromServices] IGetTopicsUseCase useCase,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new(forumId, skip, take), cancellationToken);
        var topics = result.resources.Select(mapper.Map<Topic>);
        return Ok(new { topics, result.totalCount });
    }


}
