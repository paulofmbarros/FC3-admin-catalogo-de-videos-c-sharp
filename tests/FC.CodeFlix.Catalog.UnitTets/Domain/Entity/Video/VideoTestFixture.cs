// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Entity.Video;

using Common;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;

[CollectionDefinition(nameof(VideoTestFixture))]
public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture>
{

}

public class VideoTestFixture : BaseFixture
{
    public Video GetValidVideo() => new Video(
        this.GetValidTitle(),
        this.GetValidDescription(),
        this.GetRandomBoolean(),
        this.GetRandomBoolean(),
        this.GetValidYearLaunched(),
        this.GetValidDuration(),
        this.GetValidRating()
    );

    public Video GetInvalidVideo() => new Video(
        this.GetTooLongTitle(),
        this.GetTooLongDescription(),
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
}