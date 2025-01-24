// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.Controllers;

using ApiModels.Response;
using Application.UseCases.CastMember.Common;
using Application.UseCases.CastMember.DeleteCastMember;
using Application.UseCases.CastMember.GetCastMember;
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
}