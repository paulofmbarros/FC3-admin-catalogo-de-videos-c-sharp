// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.Controllers;

using ApiModels.Genre;
using ApiModels.Response;
using Application.UseCases.Genre.Common;
using Application.UseCases.Genre.CreateGenre;
using Application.UseCases.Genre.DeleteGenre;
using Application.UseCases.Genre.GetGenre;
using Application.UseCases.Genre.UpdateGenre;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class GenresController : ControllerBase
{
    private readonly IMediator mediator;

    public GenresController(ILogger<GenresController> logger, IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var output = await this.mediator.Send(new GetGenreInput(id), cancellationToken);

        return this.Ok(new ApiResponse<GenreModelOutput>(output));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteGenreInput(id), cancellationToken);

        return this.NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreInput input, CancellationToken cancellationToken)
    {
       var output = await this.mediator.Send(input, cancellationToken);

        return this.CreatedAtAction(nameof(this.GetById), new { id = output.Id }, new ApiResponse<GenreModelOutput>(output));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateGenre(
        [FromRoute] Guid id,
        [FromBody] UpdateGenreApiInput input,
        CancellationToken cancellationToken)
    {
        var output = await this.mediator.Send(new UpdateGenreInput(id, input.Name, input.IsActive, input.CategoriesIds), cancellationToken);

        return this.CreatedAtAction(nameof(this.GetById), new { id = output.Id }, new ApiResponse<GenreModelOutput>(output));
    }
}