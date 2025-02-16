// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.CreateVideo;

using Fc.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;

public class CreateVideoTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var invalidInputsList = new List<object[]>();
        var fixture = new CreateVideoTestFixture();
        const int totalInvalidCases = 2;

        for (int i = 0; i < times; i++)
        {
            switch (i % totalInvalidCases)
            {
                case 0:
                    invalidInputsList.Add(new object[] { new CreateVideoInput(
                        string.Empty,
                        fixture.GetValidDescription(),
                        fixture.GetValidYearLaunched(),
                        fixture.GetRandomBoolean(),
                        fixture.GetRandomBoolean(),
                        fixture.GetValidDuration(),
                        fixture.GetValidRating()),

                        "Title should not be null or empty."
                    });
                    break;
                case 1:
                    invalidInputsList.Add(new object[] { new CreateVideoInput(
                            fixture.GetValidTitle(),
                            string.Empty,
                            fixture.GetValidYearLaunched(),
                            fixture.GetRandomBoolean(),
                            fixture.GetRandomBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetValidRating()),

                        "Description should not be empty."
                    });
                    break;
                default:
                    break;

            }
        }


        return invalidInputsList;
    }
}