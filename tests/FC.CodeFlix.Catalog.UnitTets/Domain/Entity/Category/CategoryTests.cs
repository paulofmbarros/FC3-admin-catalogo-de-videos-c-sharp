// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Entity.Category;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Exceptions;

public class CategoryTests
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        // Arrange
        var validData = new
        {
            Name = "Category Name",
            Description = "Category Description",
        };
        var dateTimeBefore = DateTime.Now;

        // Act
        var category = new Category(validData.Name, validData.Description);

        var dateTimeAfter = DateTime.Now;


        // Assert
        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt >= dateTimeBefore && category.CreatedAt <= dateTimeAfter);
        Assert.True(category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        // Arrange
        var validData = new
        {
            Name = "Category Name",
            Description = "Category Description",
        };
        var dateTimeBefore = DateTime.Now;

        // Act
        var category = new Category(validData.Name, validData.Description, isActive);

        var dateTimeAfter = DateTime.Now;


        // Assert
        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt >= dateTimeBefore && category.CreatedAt <= dateTimeAfter);
        Assert.Equal(isActive,category.IsActive);
    }

    [Theory(DisplayName = nameof(ErrorWhenNameIsEmpty))]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [Trait("Domain", "Category - Aggregates")]
    public void ErrorWhenNameIsEmpty(string? name)
    {
        // Arrange
        var invalidData = new
        {
            Name = name,
            Description = "Category Description",
        };

        // Act
        var exception = Assert.Throws<EntityValidationException>(() => new Category(invalidData.Name!, invalidData.Description));

        // Assert
        Assert.Equal("Name should not be empty or null", exception.Message);
    }

    [Theory(DisplayName = nameof(ErrorWhenDescriptionIsNull))]
    [InlineData(null)]
    [Trait("Domain", "Category - Aggregates")]
    public void ErrorWhenDescriptionIsNull(string? description)
    {
        // Arrange
        var invalidData = new
        {
            Name = "Category Name",
            Description = description,
        };

        // Act
        var exception = Assert.Throws<EntityValidationException>(() => new Category(invalidData.Name, invalidData.Description!));

        // Assert
        Assert.Equal("Description should not be empty or null", exception.Message);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
    [InlineData("a")]
    [InlineData("ab")]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsLessThan3Characters(string name)
    {
        var invalidData = new
        {
            Name = name,
            Description = "Category Description",
        };

        var exception = Assert.Throws<EntityValidationException>(() => new Category(invalidData.Name, invalidData.Description));

        Assert.Equal("Name should have at least 3 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThanCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThanCharacters()
    {
        var name = new string('a', 256);

        var invalidData = new
        {
            Name = name,
            Description = "Category Description",
        };

        var exception = Assert.Throws<EntityValidationException>(() => new Category(invalidData.Name, invalidData.Description));

        Assert.Equal("Name should have less than 255 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThanCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThanCharacters()
    {
        var description = new string('a', 10001);

        var invalidData = new
        {
            Name = "name",
            Description = description,
        };

        var exception = Assert.Throws<EntityValidationException>(() => new Category(invalidData.Name, invalidData.Description));

        Assert.Equal("Description should have less than 10.000 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var invalidData = new
        {
            Name = "name",
            Description = "Category Description",
        };

        var category = new Category(invalidData.Name, invalidData.Description, false);

        category.Activate();
        Assert.True(category.IsActive);
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var data = new
        {
            Name = "name",
            Description = "Category Description",
        };

        var category = new Category(data.Name, data.Description, true);

        category.Deactivate();
        Assert.False(category.IsActive);
    }
    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var data = new
        {
            Name = "name",
            Description = "Category Description",
        };

        var category = new Category(data.Name, data.Description, true);

        var newValues = new
        {
            Name = "new name",
            Description = "new description",
        };

        category.Update(newValues.Name, newValues.Description);

        Assert.Equal(newValues.Name, category.Name);
        Assert.Equal(newValues.Description, category.Description);

    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var data = new
        {
            Name = "name",
            Description = "Category Description",
        };

        var category = new Category(data.Name, data.Description, true);

        var newValues = new
        {
            Name = "new name",
        };

        category.Update(newValues.Name);

        Assert.Equal(newValues.Name, category.Name);
        Assert.Equal(data.Description, category.Description);
    }


    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        // Arrange
        var invalidData = new
        {
            Name = "name",
            Description = "Category Description",
        };

        var category = new Category(invalidData.Name, invalidData.Description, true);


        // Act
        var exception = Assert.Throws<EntityValidationException>(() => category.Update(name!) );

        // Assert
        Assert.Equal("Name should not be empty or null", exception.Message);
    }


    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characters))]
    [InlineData("a")]
    [InlineData("ab")]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsLessThan3Characters(string name)
    {
        var validData = new
        {
            Name = "name",
            Description = "Category Description",
        };

        var category = new Category(validData.Name, validData.Description, true);

        var exception = Assert.Throws<EntityValidationException>(() => category.Update(name));

        Assert.Equal("Name should have at least 3 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThanCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThanCharacters()
    {
        var name = new string('a', 256);

        var validData = new
        {
            Name = "name",
            Description = "Category Description",
        };

        var category = new Category(validData.Name, validData.Description, true);

        var exception = Assert.Throws<EntityValidationException>(() => category.Update(name));

        Assert.Equal("Name should have less than 255 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThanCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThanCharacters()
    {
        var description = new string('a', 10001);

        var validData = new
        {
            Name = "name",
            Description = "description",
        };

        var category = new Category(validData.Name, validData.Description, true);

        var exception = Assert.Throws<EntityValidationException>(() => category.Update(validData.Name , description));

        Assert.Equal("Description should have less than 10.000 characters", exception.Message);
    }
}