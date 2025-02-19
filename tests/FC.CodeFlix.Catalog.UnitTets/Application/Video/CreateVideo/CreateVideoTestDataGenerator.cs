// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.CreateVideo;

using System.Collections;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;

public class CreateVideoTestDataGenerator : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var invalidInputsList = new List<object[]>();
        var fixture = new CreateVideoTestFixture();
        const int totalInvalidCases = 4;

        for (int i = 0; i < totalInvalidCases * 2; i++)
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
                case 2:
                    invalidInputsList.Add(new object[] { new CreateVideoInput(
                            fixture.GetTooLongTitle(),
                            fixture.GetValidDescription(),
                            fixture.GetValidYearLaunched(),
                            fixture.GetRandomBoolean(),
                            fixture.GetRandomBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetValidRating()),

                        "Title should be less or equal 255 characters long."
                    });
                    break;
                case 3:
                    invalidInputsList.Add(new object[] { new CreateVideoInput(
                            fixture.GetValidTitle(),
                            fixture.GetTooLongDescription(),
                            fixture.GetValidYearLaunched(),
                            fixture.GetRandomBoolean(),
                            fixture.GetRandomBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetValidRating()),

                        "Description should be less or equal 4000 characters long."
                    });
                    break;
                default:
                    break;

            }
        }


        return invalidInputsList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}