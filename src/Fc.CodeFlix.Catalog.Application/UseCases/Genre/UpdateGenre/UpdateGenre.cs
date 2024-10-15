// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;

using Common;
using Domain.Repository;
using Exceptions;
using Interfaces;

public class UpdateGenre : IUpdateGenre
{
    private readonly IGenreRepository genreRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICategoryRepository categoryRepository;

    public UpdateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
    {
        this.genreRepository = genreRepository;
        this.unitOfWork = unitOfWork;
        this.categoryRepository = categoryRepository;
    }

    public async Task<GenreModelOutput> Handle(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await this.genreRepository.Get(request.Id, cancellationToken);

        genre.Update(request.Name);

        if(request.IsActive is not null)
        {
            if((bool)request.IsActive)
            {
                genre.Activate();
            }
            else
            {
                genre.Deactivate();
            }
        }

        if(request.CategoriesIds is not null)
        {
            genre.RemoveAllCategories();

            if (request.CategoriesIds.Count > 0)
            {
                await this.ValidateCategoriesIds(request, cancellationToken);

                foreach (var categoryId in request.CategoriesIds)
                {
                    genre.AddCategory(categoryId);
                }
            }
        }

        await this.genreRepository.Update(genre, cancellationToken);
        await this.unitOfWork.Commit(cancellationToken);

        return GenreModelOutput.FromGenre(genre);
    }

    private async Task ValidateCategoriesIds(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var idsInPersistence = await this.categoryRepository.GetIdsByIds(request.CategoriesIds, cancellationToken);

        if (idsInPersistence.Count < request.CategoriesIds.Count)
        {
            var notFoundIds = request.CategoriesIds.Except(idsInPersistence).ToList();
            throw new RelatedAggregateException($"Related category Id (or ids) not found: {string.Join(", ", notFoundIds)}");
        }
    }
}