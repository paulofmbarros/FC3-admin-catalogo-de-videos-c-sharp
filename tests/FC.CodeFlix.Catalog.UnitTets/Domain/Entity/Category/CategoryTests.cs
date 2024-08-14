// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Entity.Category;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTests
{
    private readonly CategoryTestFixture fixture;

    public CategoryTests(CategoryTestFixture fixture) => this.fixture = fixture;


    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        // Arrange
        var validData = this.fixture.GetValidCategory();

        var dateTimeBefore = DateTime.Now;

        // Act
        var category = new Category(validData.Name, validData.Description);

        var dateTimeAfter = DateTime.Now;


        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.Id.Should().NotBe(Guid.Empty);
        category.CreatedAt.Should().BeOnOrAfter(dateTimeBefore).And.BeOnOrBefore(dateTimeAfter);
        category.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        // Arrange
        var validData = this.fixture.GetValidCategory();

        var dateTimeBefore = DateTime.Now;

        // Act
        var category = new Category(validData.Name, validData.Description, isActive);

        var dateTimeAfter = DateTime.Now;


        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.Id.Should().NotBe(Guid.Empty);
        category.CreatedAt.Should().BeOnOrAfter(dateTimeBefore).And.BeOnOrBefore(dateTimeAfter);
        category.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(ErrorWhenNameIsEmpty))]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [Trait("Domain", "Category - Aggregates")]
    public void ErrorWhenNameIsEmpty(string? name)
    {
        // Arrange
        var invalidData = this.fixture.GetValidCategory();

        // Act
        var exception = Assert.Throws<EntityValidationException>(() => new Category(name!, invalidData.Description));

        // Assert
        exception.Message.Should().Be("Name should not be empty or null.");
    }

    [Theory(DisplayName = nameof(ErrorWhenDescriptionIsNull))]
    [InlineData(null)]
    [Trait("Domain", "Category - Aggregates")]
    public void ErrorWhenDescriptionIsNull(string? description)
    {
        // Arrange
        var invalidData = this.fixture.GetValidCategory();

        // Act
        var exception = Assert.Throws<EntityValidationException>(() => new Category(invalidData.Name, description!));

        // Assert
        exception.Message.Should().Be("Description should not be null.");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
    [MemberData(nameof(GetNamesWithLessThan3Characters), 10)]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsLessThan3Characters(string name)
    {
        var invalidData = this.fixture.GetValidCategory();

        var exception = Assert.Throws<EntityValidationException>(() => new Category(name, invalidData.Description));


        exception.Message.Should().Be("Name should have at least 3 characters.");
    }

    public static IEnumerable<object[]> GetNamesWithLessThan3Characters(int numberOfTests = 6)
    {
        var fixture = new CategoryTestFixture();
        for (int i = 0; i < numberOfTests; i++)
        {
            var idOdd = i % 2 == 1;
            yield return new object[]
            {
                fixture.GetValidCategoryName()[..(idOdd ? 1 : 2)]
            };
        }
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThanCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThanCharacters()
    {
        var name = new string('a', 256);

        var invalidData = this.fixture.GetValidCategory();

        var exception = Assert.Throws<EntityValidationException>(() => new Category(name, invalidData.Description));

        exception.Message.Should().Be("Name should have less than 255 characters.");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThanCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThanCharacters()
    {
        var description = new string('a', 10001);

        var invalidData = this.fixture.GetValidCategory();

        var exception = Assert.Throws<EntityValidationException>(() => new Category(invalidData.Name, description));

        exception.Message.Should().Be("Description should have less than 10000 characters.");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var category = this.fixture.GetValidCategory();

        category.Activate();
        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var category = this.fixture.GetValidCategory();

        category.Deactivate();
        category.IsActive.Should().BeFalse();
    }
    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {

        var category = this.fixture.GetValidCategory();

        var categoryWithNewValues = this.fixture.GetValidCategory();

        category.Update(categoryWithNewValues.Name, categoryWithNewValues.Description);

        categoryWithNewValues.Name.Should().Be(category.Name);
        categoryWithNewValues.Description.Should().Be(category.Description);

    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var category = this.fixture.GetValidCategory();

        var newCategoryName = this.fixture.GetValidCategoryName();

        category.Update(newCategoryName);

        newCategoryName.Should().Be(category.Name);
    }


    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        // Arrange
        var category = this.fixture.GetValidCategory();


        // Act
        var exception = Assert.Throws<EntityValidationException>(() => category.Update(name!) );

        // Assert
        exception.Message.Should().Be("Name should not be empty or null.");
    }


    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characters))]
    [InlineData("a")]
    [InlineData("ab")]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsLessThan3Characters(string name)
    {
        var category = this.fixture.GetValidCategory();

        var exception = Assert.Throws<EntityValidationException>(() => category.Update(name));

        exception.Message.Should().Be("Name should have at least 3 characters.");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThanCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThanCharacters()
    {
        var name = this.fixture.Faker.Lorem.Sentence(256);

        var category = this.fixture.GetValidCategory();

        var exception = Assert.Throws<EntityValidationException>(() => category.Update(name));

        exception.Message.Should().Be("Name should have less than 255 characters.");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThanCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThanCharacters()
    {
        var description = this.fixture.Faker.Lorem.Sentence(10001);

        var category = this.fixture.GetValidCategory();

        var exception = Assert.Throws<EntityValidationException>(() => category.Update(category.Name , description));

        exception.Message.Should().Be("Description should have less than 10000 characters.");
    }
}