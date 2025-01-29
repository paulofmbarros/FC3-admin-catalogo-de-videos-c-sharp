// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Entity;

using Exceptions;
using SeedWork;
using Validation;
using Validator;

public class Video : AggregateRoot
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Opened { get; set; }
    public bool Published { get; set; }
    public int YearLaunched { get; set; }
    public int Duration { get; set; }

    public DateTime CreatedAt { get; set; }

    public Video(string title, string description, bool opened, bool published, int yearLaunched, int duration)
    {
        this.Title = title;
        this.Description = description;
        this.Opened = opened;
        this.Published = published;
        this.YearLaunched = yearLaunched;
        this.Duration = duration;
        this.CreatedAt = DateTime.Now;
    }

    public void Validate(ValidationHandler validationHandler)
    {
        var videoValidator = new VideoValidator(this, validationHandler);
        videoValidator.Validate();
    }

    public void Update(string expectedTitle, string expectedDescription, bool expectedOpened, bool expectedPublished, int expectedYearLaunched, int expectedDuration)
    {
        this.Title = expectedTitle;
        this.Description = expectedDescription;
        this.Opened = expectedOpened;
        this.Published = expectedPublished;
        this.YearLaunched = expectedYearLaunched;
        this.Duration = expectedDuration;
    }
}