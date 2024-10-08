﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Category.CreateCategory;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using UseCases = Fc.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory - Use Case ")]
    public async void CreateCategory()
    {
        var repositoryMock = this.fixture.GetRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(unitOfWorkMock.Object, repositoryMock.Object);

        var input = this.fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive ?? true);
        output.CreatedAt.Should().NotBe(default);
        repositoryMock.Verify(x => x.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
    [Trait("Application", "CreateCategory - Use Case ")]
    public async void CreateCategoryWithOnlyName()
    {
        var repositoryMock = this.fixture.GetRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(unitOfWorkMock.Object, repositoryMock.Object);

        var input = new UseCases.CreateCategoryInput(this.fixture.GetValidCategoryName(), string.Empty, null);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(string.Empty);
        output.IsActive.Should().BeTrue();
        output.CreatedAt.Should().NotBe(default);
        repositoryMock.Verify(x => x.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
    [Trait("Application", "CreateCategory - Use Case ")]
    public async Task CreateCategoryWithOnlyNameAndDescription()
    {
        var repositoryMock = this.fixture.GetRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(unitOfWorkMock.Object, repositoryMock.Object);

        var input = new UseCases.CreateCategoryInput(this.fixture.GetValidCategoryName(), this.fixture.GetValidCategoryDescription(), true);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().BeTrue();
        output.CreatedAt.Should().NotBe(default);
        repositoryMock.Verify(x => x.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategory))]
    [Trait("Application", "CreateCategory - Use Case ")]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 24,
        MemberType = typeof(CreateCategoryTestDataGenerator))]
    public async void ThrowWhenCantInstantiateCategory(UseCases.CreateCategoryInput input, string exceptionMessage)
    {
        var repositoryMock = this.fixture.GetRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(unitOfWorkMock.Object, repositoryMock.Object);

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

       await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);

    }

}