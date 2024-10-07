﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

public class UpdateCategoryApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var invalidInputsList = new List<object[]>();
        var fixture = new UpdateCategoryApiTestFixture();
        var totalInvalidCases = 3;

        for (int i = 0; i < totalInvalidCases; i++)
        {
            switch (i % totalInvalidCases)
            {
                case 0:
                    var input1 = fixture.GetExampleInput();
                    input1.Name = fixture.GetInvalidNameTooShort();
                    invalidInputsList.Add(new object[] { input1, "Name should have at least 3 characters." });
                    break;
                case 1:
                    var input2 = fixture.GetExampleInput();
                    var invalidInputLongName = fixture.GetInvalidNameTooLong();
                    input2.Name = invalidInputLongName;
                    invalidInputsList.Add(new object[] { input2, "Name should have less than 255 characters." });
                    break;
                case 2:
                    var input3 = fixture.GetExampleInput();
                    input3.Description = fixture.GetInvalidDescriptionTooLong();
                    invalidInputsList.Add(new object[] { input3, "Description should have less than 10000 characters." });
                    break;
                default:
                    break;

            }
        }


        return invalidInputsList;
    }
}