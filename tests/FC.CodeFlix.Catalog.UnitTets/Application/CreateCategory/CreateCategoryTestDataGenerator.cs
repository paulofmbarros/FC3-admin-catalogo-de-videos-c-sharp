// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.CreateCategory;

using Fc.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;

public class CreateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var invalidInputsList = new List<object[]>();
        var fixture = new CreateCategoryTestFixture();
        var totalInvalidCases = 4;

        for (int i = 0; i < times; i++)
        {
            switch (i % totalInvalidCases)
            {
                case 0:
                    var invalidInputShortName = fixture.GetInvalidInputShortName();
                    invalidInputsList.Add(new object[] { invalidInputShortName, "Name should have at least 3 characters." });
                    break;
                case 1:
                    var invalidInputLongName = fixture.GetInvalidInputLongName();
                    invalidInputsList.Add(new object[] { invalidInputLongName, "Name should have less than 255 characters." });
                    break;
                case 2:
                    // description nao pode ser null
                    var invalidInputNullDescription = fixture.GetInvalidInputNullDescription();
                    invalidInputsList.Add(new object[] { invalidInputNullDescription, "Description should not be null." });
                    break;
                case 3:
                    var invalidInputLongDescription = fixture.GetInvalidInputLongDescription();
                    invalidInputsList.Add(new object[] { invalidInputLongDescription, "Description should have less than 10000 characters." });
                    break;
                default:
                    break;

            }
        }


        return invalidInputsList;
    }


}