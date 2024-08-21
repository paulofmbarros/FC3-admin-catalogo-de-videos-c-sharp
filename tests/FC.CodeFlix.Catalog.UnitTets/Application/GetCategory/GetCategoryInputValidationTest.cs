// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.GetCategory;

using Fc.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;
using FluentAssertions;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryInputValidationTest
{
    private readonly GetCategoryTestFixture fixture;

    public GetCategoryInputValidationTest(GetCategoryTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(ValidationOk))]
    [Trait("Application ", "GetCategory - Use Cases")]
    public void ValidationOk()
    {
        // Arrange
        var input = new GetCategoryInput(Guid.NewGuid());
        var validator = new GetCategoryInputValidator();
        // Act
        var result = validator.Validate(input);
        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}