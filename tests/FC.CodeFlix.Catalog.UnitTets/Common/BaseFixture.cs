﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Common;

using Bogus;

public abstract class BaseFixture
{

    public Faker Faker { get; set; }


    protected BaseFixture()
    {
        this.Faker = new Faker("pt_BR");
    }

    public bool GetRandomBoolean() => this.Faker.Random.Bool();
}