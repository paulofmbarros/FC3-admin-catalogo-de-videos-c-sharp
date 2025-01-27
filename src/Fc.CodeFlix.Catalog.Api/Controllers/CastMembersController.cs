// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.Controllers;

using ApiModels.Response;
using Application.UseCases.CastMember.Common;
using Application.UseCases.CastMember.CreateCastMember;
using Application.UseCases.CastMember.DeleteCastMember;
using Application.UseCases.CastMember.GetCastMember;
using Application.UseCases.CastMember.ListCastMembers;
using Application.UseCases.CastMember.UpdateCastMember;
using Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CastMembersController : ControllerBase
{
    private readonly IMediator mediator;

    public CastMembersController(IMediator mediator) => this.mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var output = await this.mediator.Send(new GetCastMemberInput(id), cancellationToken);

        return this.Ok(new ApiResponse<CastMemberModelOutput>(output));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteCastMemberInput(id), cancellationToken);
        return this.NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateCastMemberInput input, CancellationToken cancellationToken)
    {
        var output = await this.mediator.Send(input, cancellationToken);

        return this.CreatedAtAction(nameof(Create), new { output.Id }, new ApiResponse<CastMemberModelOutput>(output));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCastMemberInput apiInput,
        CancellationToken cancellationToken)
    {
        var input = new UpdateCastMemberInput(id, apiInput.Name, apiInput.Type);

        var output = await this.mediator.Send(input, cancellationToken);

        return this.Ok(new ApiResponse<CastMemberModelOutput>(output));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseList<CastMemberModelOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] SearchOrder? direction,
        [FromQuery] int? page,
        [FromQuery(Name = "per_page")] int? perPage,
        [FromQuery] string? search,
        [FromQuery] string? sort,
        CancellationToken cancellationToken)
    {
        var input = new ListCastMembersInput()
        {
            Page = page ?? 1,
            PerPage = perPage ?? 10,
            Search = search ?? string.Empty,
            Direction = direction ?? SearchOrder.Asc,
            Sort = sort ?? string.Empty,
        };

        var output = await this.mediator.Send(input, cancellationToken);

        return this.Ok(new ApiResponseList<CastMemberModelOutput>(output));
    }
}