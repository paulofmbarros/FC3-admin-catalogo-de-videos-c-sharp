﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Base;

using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Codeflix.Catalog.EndToEndTests.Base;
using Infra.Data.EF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class BaseFixture
{
    protected Faker Faker { get; set; }

    public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }
    public HttpClient HttpClient { get; set; }
    public ApiClient ApiClient { get; set; }

    private readonly string dbConnectionString;

    public BaseFixture()
    {
        this.Faker = new Faker("pt_BR");
        this.WebAppFactory = new CustomWebApplicationFactory<Program>();
        this.HttpClient = WebAppFactory.CreateClient();
        this.ApiClient = new ApiClient(HttpClient);
        var configuration = WebAppFactory.Services.GetService<IConfiguration>();
        this.dbConnectionString = configuration.GetConnectionString("CatalogDb");
    }

    public CodeflixCatalogDbContext CreateDbContext()
    {
        var context = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseMySql(this.dbConnectionString, ServerVersion.AutoDetect(this.dbConnectionString))
                .Options
        );
        return context;
    }

    public void ClearPersistence()
    {
        var context = this.CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
    public void Dispose()
    {
        WebAppFactory.Dispose();
    }


}