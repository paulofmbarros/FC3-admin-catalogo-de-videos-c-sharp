// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre;

using Domain.Repository;
using Exceptions;
using Interfaces;
using MediatR;

public class DeleteGenre : IDeleteGenre
{
    private readonly IGenreRepository genreRepository;
    private readonly IUnitOfWork unitOfWork;

    public DeleteGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork)
    {
        this.genreRepository = genreRepository;
        this.unitOfWork = unitOfWork;
    }


    public async Task<Unit> Handle(DeleteGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await this.genreRepository.Get(request.Id, cancellationToken);

        await this.genreRepository.Delete(genre, cancellationToken);

        await this.unitOfWork.Commit(cancellationToken);

        return Unit.Value;
    }
}