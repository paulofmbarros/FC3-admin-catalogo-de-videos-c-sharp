// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Extensions;

using Fc.CodeFlix.Catalog.Domain.Enum;
using Fc.CodeFlix.Catalog.Domain.Extensions;
using FluentAssertions;

public class RatingExtensionsTest
{
    [Theory(DisplayName = nameof(StringToRating))]
    [Trait("Domain", "Rating Extensions")]
    [InlineData("Er", Rating.Er)]
    [InlineData("L", Rating.L)]
    [InlineData("10", Rating.Rate10)]
    [InlineData("12", Rating.Rate12)]
    [InlineData("14", Rating.Rate14)]
    [InlineData("16", Rating.Rate16)]
    [InlineData("18", Rating.Rate18)]
    public void StringToRating(string ratingString, Rating expectedRating)
    {
        var rating = ratingString.ToRating();
        rating.Should().Be(expectedRating);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionWhenInvalidRating))]
    [Trait("Domain", "Rating Extensions")]
    public void ThrowsExceptionWhenInvalidRating()
    {
        var act = () => "InvalidRating".ToRating();
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory(DisplayName = nameof(RatingToString))]
    [Trait("Domain", "Rating Extensions")]
    [InlineData(Rating.Er, "Er" )]
    [InlineData(Rating.L, "L")]
    [InlineData(Rating.Rate10,"10")]
    [InlineData(Rating.Rate12,"12")]
    [InlineData(Rating.Rate14,"14")]
    [InlineData(Rating.Rate16,"16")]
    [InlineData(Rating.Rate18,"18")]
    public void RatingToString(Rating ratingEnum, string ratingString)
    {
        var rating = ratingEnum.ToStringSignal();
        rating.Should().Be(ratingString);
    }
}