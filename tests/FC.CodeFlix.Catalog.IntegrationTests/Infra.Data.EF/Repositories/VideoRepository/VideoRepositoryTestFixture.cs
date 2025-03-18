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

}