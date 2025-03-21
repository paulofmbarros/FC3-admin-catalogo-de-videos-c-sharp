// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

using Base;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;

[CollectionDefinition(nameof(VideoRepositoryTestFixture))]
public class VideoRespositoryTestFixtureCollection : ICollectionFixture<VideoRepositoryTestFixture>
{

}

public class VideoRepositoryTestFixture : BaseFixture
{
    public Video GetExampleVideo() => new Video(
        this.GetValidTitle(),
        this.GetValidDescription(),
        this.GetRandomBoolean(),
        this.GetRandomBoolean(),
        this.GetValidYearLaunched(),
        this.GetValidDuration(),
        this.GetValidRating()
    );
    public string GetValidTitle() => this.Faker.Lorem.Letter(100);
    public string GetValidDescription() => this.Faker.Commerce.ProductDescription();
    public string GetTooLongDescription() => this.Faker.Lorem.Letter(4_001);

    public int GetValidYearLaunched()=> this.Faker.Date.BetweenDateOnly(new DateOnly(1960,1,1), new DateOnly(2022,1,1)).Year;

    public int GetValidDuration() => new Random().Next(100, 200);

    public string GetTooLongTitle()=> this.Faker.Lorem.Letter(400);

    public Rating GetValidRating()
    {
        var possibleValues = Enum.GetValues<Rating>();
        return possibleValues[new Random().Next(possibleValues.Length)];
    }

    public string GetValidImagePath() => this.Faker.Image.PlaceImgUrl();

    public string GetValidMediaPath()
    {
        var exampleMedias = new string[]
        {
            "https://www.googlestorage.com/file-example.mp4",
            "https://www.storage.com/example.mp4",
            "https://www.s3.com.br/file-example.mp4",
            "https://www.glg.io/file.mp4"
        };

        var random = new Random();
        return exampleMedias[random.Next(exampleMedias.Length)];
    }

    public Media GetValidMedia() => new Media(this.GetValidMediaPath());

    public bool GetRandomBoolean() => this.Faker.Random.Bool();

    public string GetValidName() => this.Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType() => (CastMemberType)new Random().Next(1, 2);

    public CastMember GetExampleCastMember() => new(this.GetValidName(), this.GetRandomCastMemberType());
    public List<CastMember> GetRandomCastMemberList() => Enumerable.Range(0, Random.Shared.Next(1,5)).Select(i => new CastMember(this.GetValidName(), this.GetRandomCastMemberType())).ToList();

    public List<CastMember> GetExampleCastMembersList(int quantity)
    {
        var castMembers = new List<CastMember>();
        for (var i = 0; i < quantity; i++)
        {
            castMembers.Add(new CastMember($"CastMember {i}", this.GetRandomCastMemberType()));
        }

        return castMembers;
    }

    public Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            this.GetRandomBoolean()
        );

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

    public List<Category> GetRandomCategoryList()=> Enumerable.Range(0, Random.Shared.Next(1,5)).Select(_ => new Category(GetValidCategoryName(), this.GetValidCategoryDescription())).ToList();

    public string GetValidGenreName() => Faker.Commerce.Categories(1)[0];

    public Genre GetExampleGenre() => new Genre(this.GetValidGenreName(),true);

    public List<Genre> GetRandomGenresList() =>
        Enumerable.Range(0, Random.Shared.Next(1,5)).Select(i => new Genre(this.GetValidGenreName(), true)).ToList();

    public Video GetValidVideoWithAllProperties()
    {
        var video =  new Video(
            this.GetValidTitle(),
            this.GetValidDescription(),
            this.GetRandomBoolean(),
            this.GetRandomBoolean(),
            this.GetValidYearLaunched(),
            this.GetValidDuration(),
            this.GetValidRating()
        );

        video.UpdateBanner(this.GetValidImagePath());
        video.UpdateThumb(this.GetValidImagePath());
        video.UpdateThumbHalf(this.GetValidImagePath());
        video.UpdateMedia(this.GetValidMediaPath());
        video.UpdateTrailer(this.GetValidMediaPath());

        video.UpdateAsEncoded(GetValidImagePath());

        return video;
    }

}