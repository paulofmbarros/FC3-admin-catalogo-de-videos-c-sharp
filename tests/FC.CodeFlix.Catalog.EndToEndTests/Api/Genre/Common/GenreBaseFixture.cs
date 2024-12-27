// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Common;

using Base;
using Category.Common;
using Fc.CodeFlix.Catalog.Domain.Entity;

public class GenreBaseFixture : BaseFixture
{
    public GenrePersistence Persistence { get; set; }
    public CategoryPersistence CategoryPersistence { get; set; }

    public GenreBaseFixture() : base()
    {
        var dbContext = this.CreateDbContext();
        this.Persistence = new GenrePersistence(dbContext);
        this.CategoryPersistence = new CategoryPersistence(dbContext);
    }

    public Genre GetExampleGenre(string name = "")
    {
        var category = new Genre(string.IsNullOrEmpty(name) ? this.GetGenreName() : name,
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

    public bool GetRandomBoolean() => this.Faker.Random.Bool();

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

    public string GetValidCategoryDescription()
    {
        var categoryDescription = this.Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription.Substring(0, 10000);
        }

        return categoryDescription;
    }

    public List<Genre> GetExampleGenresList(int length = 10)
        => Enumerable.Range(0, length).Select(_ => this.GetExampleGenre()).ToList();
}