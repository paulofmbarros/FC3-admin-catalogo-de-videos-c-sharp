// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Category.UpdateCategory;

public class UpdateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();
        for (int i = 0; i < times; i++)
        {
            var exampleCategory = fixture.GetExampleCategory();
            var updateCategoryInput = fixture.GenerateUpdateCategoryInput(exampleCategory.Id);

            yield return new object[] { exampleCategory, updateCategoryInput };
        }
    }

    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var invalidInputsList = new List<object[]>();
        var fixture = new UpdateCategoryTestFixture();
        var totalInvalidCases = 3;

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