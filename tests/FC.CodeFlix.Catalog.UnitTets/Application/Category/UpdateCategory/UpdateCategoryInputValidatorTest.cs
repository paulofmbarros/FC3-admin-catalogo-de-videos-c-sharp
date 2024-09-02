// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Category.UpdateCategory;

using Fc.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryInputValidatorTest
{
    private readonly UpdateCategoryTestFixture fixture;

    public UpdateCategoryInputValidatorTest(UpdateCategoryTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(DontValidateWhenEmptyGuid))]
    [Trait("Application", "UpdateCategoryInputValidator - Use Case")]
    public void DontValidateWhenEmptyGuid()
    {
        // Arrange
        var input = this.fixture.GenerateUpdateCategoryInput(Guid.Empty);
        var validator = new UpdateCategoryInputValidator();

        // Act
       var validationResult = validator.Validate(input);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Should().Contain(x => x.ErrorMessage == "Id is required");
    }

    [Fact(DisplayName = nameof(ValidateWhenValid))]
    [Trait("Application", "UpdateCategoryInputValidator - Use Case")]
    public void ValidateWhenValid()
    {
        // Arrange
        var input = this.fixture.GenerateUpdateCategoryInput();
        var validator = new UpdateCategoryInputValidator();

        // Act
        var validationResult = validator.Validate(input);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().HaveCount(0);
    }

}