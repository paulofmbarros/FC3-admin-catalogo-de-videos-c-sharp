// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.CastMember.ListCastMembers;

using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;

[Collection(nameof(ListCastMembersTestFixture))]
public class ListCastMembersTest(ListCastMembersTestFixture fixture)
{
    [Fact(DisplayName = nameof(ListCastMembers))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task ListCastMembers()
    {
        // Arrange
        var castMembers = fixture.GetExampleCastMembersList(3);
        var searchOutput = new SearchOutput<CastMember>(1, 10, castMembers.Count, castMembers);
        var repositoryMock = new Mock<ICastMemberRepository>();
        repositoryMock.Setup(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchOutput);
        var input = new ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);

        var useCase = new ListCastMembers(repositoryMock.Object);

        // Act
        var result = await useCase.Handle(input,CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PerPage.Should().Be(input.PerPage);
        result.Page.Should().Be(input.Page);
        result.Total.Should().Be(searchOutput.Total);
        result.Items.ToList().ForEach(item =>
        {
            var castMember = castMembers.FirstOrDefault(x => x.Id == item.Id);
            castMember.Should().NotBeNull();
            item.Name.Should().Be(castMember.Name);
            item.Type.Should().Be(castMember.Type);
        });

        repositoryMock.Verify(x=>x.Search(It.Is<SearchInput>(x=> x.Page == input.Page
                                                                 && x.PerPage == input.PerPage
                                                                 && x.Order == input.Direction
                                                                 && x.OrderBy == input.Sort
                                                                 && x.Search == input.Search
                                                                 ), It.IsAny<CancellationToken>()),
            Times.Once);



    }

}