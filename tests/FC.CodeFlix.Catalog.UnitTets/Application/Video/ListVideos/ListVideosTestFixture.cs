// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.ListVideos;

using Domain.Entity.Video;
using Fc.CodeFlix.Catalog.Domain.Entity;

[CollectionDefinition(nameof(ListVideosTestFixture))]
public class ListVideosTestFixtureCollection : ICollectionFixture<ListVideosTestFixture>
{

}


public class ListVideosTestFixture : VideoTestFixture
{
    public List<Video> CreateExampleVideosList()
       => Enumerable.Range(1, Random.Shared.Next(2,10)).Select(_ => this.GetValidVideoWithAllProperties()).ToList();

    public (List<Video> Videos, List<Category> categories, List<Genre> genres  ) CreateExampleVideosListWithRelations()
    {
        var itemsToCreated = Random.Shared.Next(2, 10);
        var categories = new List<Category>();
        var genres = new List<Genre>();
        var videos = Enumerable.Range(1, itemsToCreated).Select(_ => this.GetValidVideoWithAllProperties()).ToList();

        videos.ForEach(video =>
        {
            video.RemoveAllCategories();
            var qtdCategories =  Random.Shared.Next(2, 5);
            for (var i = 0; i < qtdCategories; i++)
            {
                var category = this.GetExampleCategory();
                categories.Add(category);
                video.AddCategory(category.Id);
            }

            video.RemoveAllGenres();
            var qtdGenres =  Random.Shared.Next(2, 5);
            for (var i = 0; i < qtdGenres; i++)
            {
                var genre = this.GetExampleGenre();
                genres.Add(genre);
                video.AddGenre(genre.Id);
            }
        });

        return (videos, categories, genres);
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

    public string GetValidCategoryDescription()
    {
        var categoryDescription = this.Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription.Substring(0, 10000);
        }

        return categoryDescription;
    }


    private Category GetExampleCategory()
    {
        var category = new Category(this.GetValidCategoryName(), this.GetValidCategoryDescription(), this.GetRandomBoolean());
        return category;
    }


    private Genre GetExampleGenre()
    {
        var category = new Genre(this.GetGenreName(),
            this.GetRandomBoolean());
        return category;
    }

    private string GetGenreName()
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

    public List<Video> CreateExampleVideosListWithoutRelations() =>
        Enumerable.Range(1, Random.Shared.Next(2,10)).Select(_ => this.GetValidVideo()).ToList();
}