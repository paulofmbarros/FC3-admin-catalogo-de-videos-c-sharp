// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Category.ListCategories;

using Fc.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;

public class ListCategoriesTestDataGenerator
{
    public static IEnumerable<object[]> GetInputsWithoutAllParameters(int times = 10)
    {
        var fixture = new ListCategoriesTestFixture();
        var inputExample = fixture.GetExampleInput();

        for (int i = 0; i < times; i++)
        {
            switch (i % 6)
            {
                case 0:
                    yield return new object[] { new ListCategoriesInput() };
                    break;
                case 1:
                    yield return new object[] { new ListCategoriesInput { Page = inputExample.Page } };
                    break;
                case 2:
                    yield return new object[] { new ListCategoriesInput { Page= inputExample.Page ,PerPage = inputExample.PerPage } };
                    break;
                case 3:
                    yield return new object[] { new ListCategoriesInput { Page= inputExample.Page ,PerPage = inputExample.PerPage, Search = inputExample.Search} };
                    break;
                case 4:
                    yield return new object[] { new ListCategoriesInput { Page= inputExample.Page ,PerPage = inputExample.PerPage, Search = inputExample.Search, Sort = inputExample.Sort} };
                    break;
                case 5:
                    yield return new object[] { inputExample };
                    break;

                default:
                    break;


            }
        }
    }
}