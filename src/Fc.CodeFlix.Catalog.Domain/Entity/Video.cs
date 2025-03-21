// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Entity;

using Enum;
using Exceptions;
using SeedWork;
using Validation;
using Validator;
using ValueObject;

public class Video : AggregateRoot
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Opened { get; set; }
    public bool Published { get; set; }
    public int YearLaunched { get; set; }
    public int Duration { get; set; }
    public Rating Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public Image? Thumb { get; private set; }
    public Image? ThumbHalf { get; private set; }
    public Image? Banner { get; private set; }
    public Media? Media { get; private set; }

    public Media? Trailer { get; private set; }

    private List<Guid> categories;

    public IReadOnlyList<Guid> Categories => this.categories.AsReadOnly();

    private List<Guid> genres;
    public IReadOnlyList<Guid> Genres => this.genres.AsReadOnly();

    private List<Guid> castMembers;

    public IReadOnlyList<Guid> CastMembers => this.castMembers.AsReadOnly();


    public Video(string title, string description, bool opened, bool published, int yearLaunched, int duration, Rating rating)
    {
        this.Title = title;
        this.Description = description;
        this.Opened = opened;
        this.Published = published;
        this.YearLaunched = yearLaunched;
        this.Duration = duration;
        this.CreatedAt = DateTime.Now;
        this.Rating = rating;
        this.categories = new List<Guid>();
        this.genres = new List<Guid>();
        this.castMembers = new List<Guid>();
    }

    public void Validate(ValidationHandler validationHandler)
    {
        var videoValidator = new VideoValidator(this, validationHandler);
        videoValidator.Validate();
    }

    public void Update(string expectedTitle, string expectedDescription, bool expectedOpened, bool expectedPublished, int expectedYearLaunched, int expectedDuration, Rating? rating = null)
    {
        this.Title = expectedTitle;
        this.Description = expectedDescription;
        this.Opened = expectedOpened;
        this.Published = expectedPublished;
        this.YearLaunched = expectedYearLaunched;
        this.Duration = expectedDuration;
        if (rating != null)
        {
            this.Rating = rating.Value;
        }
    }

    public void UpdateThumb(string imagePath)
        => this.Thumb = new Image(imagePath);

    public void UpdateThumbHalf(string imagePath) =>
        this.ThumbHalf = new Image(imagePath);

    public void UpdateBanner(string imagePath) =>
        this.Banner = new Image(imagePath);

    public void UpdateMedia(string path)
        => this.Media = new Media(path);

    public void UpdateTrailer(string path)
        => this.Trailer = new Media(path);

    public void UpdateAsSentToEncode()
    {
        if (this.Media is null)
            throw new EntityValidationException("There is no media");

        this.Media.UpdateAsSentToEncode();
    }

    public void UpdateAsEncoded(string validEncodedPath)
    {
        if (this.Media is null)
            throw new EntityValidationException("There is no media");

        this.Media.UpdateAsEncoded(validEncodedPath);
    }

    public void AddCategory(Guid categoryId) => this.categories.Add(categoryId);

    public void RemoveCategory(Guid categoryId) =>
        this.categories.Remove(categoryId);

    public void RemoveAllCategories() => this.categories = new List<Guid>();

    public void AddGenre(Guid genreId) => this.genres.Add(genreId);


    public void RemoveGenre(Guid genreId) => this.genres.Remove(genreId);

    public void RemoveAllGenres() => this.genres = new List<Guid>();

    public void AddCastMember(Guid castMemberId) => this.castMembers.Add(castMemberId);

    public void RemoveCastMember(Guid castMemberId) => this.castMembers.Remove(castMemberId);

    public void RemoveAllCastMembers() => this.castMembers = new List<Guid>();

}