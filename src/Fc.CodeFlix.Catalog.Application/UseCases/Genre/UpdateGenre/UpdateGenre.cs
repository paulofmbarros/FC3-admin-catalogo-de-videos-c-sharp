// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;

using Common;
using Domain.Repository;
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

        if(request.CategoriesIds is not null && request.CategoriesIds.Count != 0)
        {
            genre.RemoveAllCategories();

            foreach (var categoryId in request.CategoriesIds)
            {
               // var category = await this.categoryRepository.Get(categoryId, cancellationToken);
                genre.AddCategory(categoryId);
            }
        }

        await this.genreRepository.Update(genre, cancellationToken);
        await this.unitOfWork.Commit(cancellationToken);

        return GenreModelOutput.FromGenre(genre);
    }
}