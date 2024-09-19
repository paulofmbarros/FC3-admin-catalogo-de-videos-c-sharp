// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Base;

using Bogus;
using Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

public class BaseFixture
{
    protected Faker Faker { get; set; }
    public ApiClient ApiClient { get; set; }
    public CustomWebApplicationFactory<Program> WebFactory { get; set; }
    public HttpClient HttpClient { get; set; }

    public BaseFixture()
    {
        this.Faker = new Faker("pt_BR");
        this.WebFactory = new CustomWebApplicationFactory<Program>();
        this.HttpClient = this.WebFactory.CreateClient();
        this.ApiClient = new ApiClient(this.HttpClient);
    }

    public CodeflixCatalogDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseInMemoryDatabase("end2end-tests-db")
            .Options;

        var dbContext = new CodeflixCatalogDbContext(options);

        return dbContext;
    }


}