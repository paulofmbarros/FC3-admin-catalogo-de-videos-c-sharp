﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Extensions;

using Enum;

public static class RatingExtensions
{
    public static Rating ToRating(this string ratingString)=> ratingString switch
    {
        "Er" => Rating.Er,
        "L" => Rating.L,
        "10" => Rating.Rate10,
        "12" => Rating.Rate12,
        "14" => Rating.Rate14,
        "16" => Rating.Rate16,
        "18" => Rating.Rate18,
        _ => throw new ArgumentOutOfRangeException(nameof(ratingString))
    };

    public static string ToStringSignal(this Rating rating) => rating switch
    {
        Rating.Er => "Er",
        Rating.L => "L",
        Rating.Rate10 => "10",
        Rating.Rate12 => "12",
        Rating.Rate14 => "14",
        Rating.Rate16 => "16",
        Rating.Rate18 => "18",
        _ => throw new ArgumentOutOfRangeException(nameof(rating))
    };
}