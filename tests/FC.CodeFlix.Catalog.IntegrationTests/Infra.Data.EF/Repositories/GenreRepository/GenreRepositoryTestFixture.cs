// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

using Base;
using Fc.CodeFlix.Catalog.Domain.Entity;

[CollectionDefinition(nameof(GenreRepositoryTestFixture))]
public class GenreRepostoryTestFixtureCollection : ICollectionFixture<GenreRepositoryTestFixture>
{

}

public class GenreRepositoryTestFixture : BaseFixture
{
    
    public bool GetRandomBoolean() => this.Faker.Random.Bool();
    public Genre GetExampleGenre()
    {
        var category = new Genre(this.GetGenreName(),
            this.GetRandomBoolean());
        return category;
    }
    
    public string GetGenreName()
    {
        var genreName = "";
        while (genreName.Length < 3)
        {
            genreName = this.Faker.Commerce.Categories(1)[0];
        }

        if (genreName.Length > 255)
        {
            genreName = genreName.Substring(0, 255);
        }

        return genreName;
    }

    public List<Category> GetExampleCategoriesList(int length = 10)
        => Enumerable.Range(0, length).Select(_ => this.GetExampleCategory()).ToList();
    public Category GetExampleCategory()
    {
        var category = new Category(this.GetValidCategoryName(), this.GetValidCategoryDescription(),
            this.GetRandomBoolean());
        return category;
    }

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
        {
            categoryName = this.Faker.Commerce.Categories(1)[0];
        }

        if (categoryName.Length > 255)
        {
            categoryName = categoryName.Substring(0, 255);
        }

        return categoryName;
    }

    public List<Genre> GetExampleGenresList(int length = 10)
        => Enumerable.Range(0, length).Select(_ => this.GetExampleGenre()).ToList();

    public string GetValidCategoryDescription()
    {
        var categoryDescription = this.Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription.Substring(0, 10000);
        }

        return categoryDescription;
    }

    public Genre GetExampleGenre(bool? isActive = null, List<Guid> categoryIds = null, string? name = null)
    {
        var genre = new Genre(name ?? this.GetGenreName(), isActive ?? this.GetRandomBoolean());
        if (categoryIds != null)
        {
            foreach (var categoryId in categoryIds)
            {
                genre.AddCategory(categoryId);
            }
        }

        return genre;
    }

    public List<Genre> GetExampleGenreListByNames(List<string> names)
        => names.Select(name => this.GetExampleGenre(name: name)).ToList();

}