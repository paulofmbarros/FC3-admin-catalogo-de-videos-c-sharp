// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Base;

using Bogus;

public class BaseFixture
{
    protected Faker Faker { get; set; }

    public BaseFixture() => this.Faker = new Faker("pt_BR");
}