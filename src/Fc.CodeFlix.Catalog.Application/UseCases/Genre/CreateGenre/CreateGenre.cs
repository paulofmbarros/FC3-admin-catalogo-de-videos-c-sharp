// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;

using Common;
using Domain.Entity;
using Domain.Repository;
using Exceptions;
using Interfaces;

public class CreateGenre : ICreateGenre
{
    private readonly IGenreRepository genreRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICategoryRepository categoryRepository;

    public CreateGenre(IGenreRepository genreRepository,
        IUnitOfWork unitOfWork,
        ICategoryRepository categoryRepository)
    {
        this.genreRepository = genreRepository;
        this.unitOfWork = unitOfWork;
        this.categoryRepository = categoryRepository;
    }


    public async Task<GenreModelOutput> Handle(CreateGenreInput request, CancellationToken cancellationToken)
    {
        var genre = new Genre(request.Name, request.IsActive);

        if (request.Categories is not null && request.Categories.Count > 0)
        {

            await this.ValidateCategoriesIds(request, cancellationToken);

            foreach (var categoryId in request.Categories)
            {
                genre.AddCategory(categoryId);
            }
        }

        await this.genreRepository.Insert(genre, cancellationToken);
        await this.unitOfWork.Commit(cancellationToken);

        return GenreModelOutput.FromGenre(genre);
    }

    private async Task ValidateCategoriesIds(CreateGenreInput request, CancellationToken cancellationToken)
    {
        var idsInPersistence = await this.categoryRepository.GetIdsByIds(request.Categories, cancellationToken);

        if (idsInPersistence.Count < request.Categories.Count)
        {
            var notFoundIds = request.Categories.Except(idsInPersistence).ToList();
            throw new RelatedAggregateException($"Related category Id (or ids) not found: {string.Join(",", notFoundIds)}");
        }
    }
}