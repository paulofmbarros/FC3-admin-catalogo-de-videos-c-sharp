// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.CastMember.CreateCastMember;

using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Moq;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest
{
    private readonly CreateCastMemberTestFixture fixture;

    public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = (nameof(Create)))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task Create()
    {
        // Arrange
       var input = new CreateCastMemberInput { Name = fixture.GetValidName(), Type = fixture.GetRandomCastMemberType() };
       var repositoryMock = new Mock<ICastMemberRepository>();
       var unitOfWorkMock = new Mock<IUnitOfWork>();

       var useCase = new CreateCastMember(repositoryMock.Object, unitOfWorkMock.Object);


        // Act
        var output = await useCase.Handle(input);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().NotBe(default(DateTime));
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once());
        repositoryMock.Verify(x=>x.Insert(It.Is<CastMember>(x=>x.Name == input.Name && x.Type == input.Type),It.IsAny<CancellationToken>()), Times.Once());



    }


}