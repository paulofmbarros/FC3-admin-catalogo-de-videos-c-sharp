// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Common.Fixtures;

using System.Text;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.Common;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;

public abstract class VideoTestFixtureBase : BaseFixture
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

    public FileInput GetImageValidFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("test"));
        var fileInput = new FileInput("jpg", exampleStream);

        return fileInput;
    }

    public FileInput GetMediaValidFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("test"));
        var fileInput = new FileInput("mp4", exampleStream);

        return fileInput;
    }
}