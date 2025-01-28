// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Entity;

using SeedWork;

public class Video : AggregateRoot
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Opened { get; set; }
    public bool Published { get; set; }
    public int YearLaunched { get; set; }
    public int Duration { get; set; }

    public Video(string title, string description, bool opened, bool published, int yearLaunched, int duration)
    {
        Title = title;
        Description = description;
        Opened = opened;
        Published = published;
        YearLaunched = yearLaunched;
        Duration = duration;
    }
}