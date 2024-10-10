// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Entity.Genre;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;

[Collection(nameof(GenreTestFixture))]
public class GenreTest
{
    private readonly GenreTestFixture genreTestFixture;

    public GenreTest(GenreTestFixture genreTestFixture)
    {
        this.genreTestFixture = genreTestFixture;
    }


    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain ", "Genre - Aggregates")]
    public void Instantiate()
    {
        var dateTimeBefore = DateTime.Now;
        var genre = this.genreTestFixture.GetExampleGenre();

        var dateTimeAfter = DateTime.Now;

        genre.Should().NotBeNull();
        genre.Name.Should().Be(genre.Name);
        genre.IsActive.Should().BeTrue();
        genre.Id.Should().NotBe(Guid.Empty);

        genre.CreatedAt.Should().BeOnOrAfter(dateTimeBefore).And.BeOnOrBefore(dateTimeAfter);


    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain ", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var dateTimeBefore = DateTime.Now;
        var genre = this.genreTestFixture.GetExampleGenre(isActive);

        var dateTimeAfter = DateTime.Now;

        genre.Should().NotBeNull();
        genre.IsActive.Should().Be(isActive);
        genre.Id.Should().NotBe(Guid.Empty);

        genre.CreatedAt.Should().BeOnOrAfter(dateTimeBefore).And.BeOnOrBefore(dateTimeAfter);


    }

    [Theory(DisplayName = nameof(Activate))]
    [Trait("Domain ", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Activate(bool isActive)
    {
        var genre = this.genreTestFixture.GetExampleGenre(isActive);

        genre.Activate();

        genre.Should().NotBeNull();
        genre.Id.Should().NotBe(Guid.Empty);
        genre.IsActive.Should().Be(true);

    }

    [Theory(DisplayName = nameof(Deactivate))]
    [Trait("Domain ", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Deactivate(bool isActive)
    {
        var genre = this.genreTestFixture.GetExampleGenre(isActive);

        genre.Deactivate();

        genre.Should().NotBeNull();
        genre.Id.Should().NotBe(Guid.Empty);
        genre.IsActive.Should().Be(false);

    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain ", "Genre - Aggregates")]
    public void Update()
    {
        var genre = this.genreTestFixture.GetExampleGenre();
        var newName = this.genreTestFixture.GetValidName();
        var oldIsActive = genre.IsActive;

        genre.Update(newName);


        genre.Should().NotBeNull();
        genre.Name.Should().Be(newName);
        genre.Id.Should().NotBe(Guid.Empty);
        genre.IsActive.Should().Be(oldIsActive);

    }

    [Theory(DisplayName = nameof(InstantiateThrowWhenNameIsEmpty))]
    [Trait("Domain ", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData( "   ")]
    public void InstantiateThrowWhenNameIsEmpty(string? name)
    {

        var action = new Action(() => new Genre(name));

        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be empty or null.");

    }

    [Theory(DisplayName = nameof(UpdateThrowWhenNameIsEmpty))]
    [Trait("Domain ", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData( "   ")]
    public void UpdateThrowWhenNameIsEmpty(string? name)
    {
      var genre = this.genreTestFixture.GetExampleGenre();

      var action = () => genre.Update(name);

      action.Should().Throw<EntityValidationException>()
          .WithMessage("Name should not be empty or null.");

    }

    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain ", "Genre - Aggregates")]
    public void AddCategory()
    {
        var genre = this.genreTestFixture.GetExampleGenre();
        var categoryGuid = Guid.NewGuid();

        genre.AddCategory(categoryGuid);

        genre.Categories.Should().NotBeNull();
        genre.Categories.Should().HaveCount(1);
        genre.Categories.Should().Contain(categoryGuid);

    }

    [Fact(DisplayName = nameof(AddTwoCategories))]
    [Trait("Domain ", "Genre - Aggregates")]
    public void AddTwoCategories()
    {
        var genre = this.genreTestFixture.GetExampleGenre();
        var categoryGuid = Guid.NewGuid();
        var categoryGuid2 = Guid.NewGuid();

        genre.AddCategory(categoryGuid);
        genre.AddCategory(categoryGuid2);

        genre.Categories.Should().NotBeNull();
        genre.Categories.Should().HaveCount(2);
        genre.Categories.Should().Contain(categoryGuid);
        genre.Categories.Should().Contain(categoryGuid2);

    }

    [Fact(DisplayName = nameof(RemoveCategories))]
    [Trait("Domain ", "Genre - Aggregates")]
    public void RemoveCategories()
    {

        var exampleGuid = Guid.NewGuid();

        var genre = this.genreTestFixture.GetExampleGenre(categoriesIds:new List<Guid>()
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            exampleGuid,
            Guid.NewGuid(),
        });

        genre.RemoveCategory(exampleGuid);

        genre.Categories.Should().HaveCount(4);
        genre.Categories.Should().NotContain(exampleGuid);

    }

    [Fact(DisplayName = nameof(RemoveAllCategories))]
    [Trait("Domain ", "Genre - Aggregates")]
    public void RemoveAllCategories()
    {


        var genre = this.genreTestFixture.GetExampleGenre(categoriesIds:new List<Guid>()
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
        });

        genre.RemoveAllCategories();

        genre.Categories.Should().HaveCount(0);

    }


    
}