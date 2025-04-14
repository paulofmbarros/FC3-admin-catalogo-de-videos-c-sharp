// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Infra.Storage;

using Common;

[CollectionDefinition(nameof(StorageServiceTestFixture))]
public class StorageServiceTestFixtureCollection : ICollectionFixture<StorageServiceTestFixture>
{

}


public class StorageServiceTestFixture : BaseFixture
{
    public string GetBucketName() => "fc3-catalog-medias";

    public string GetFileName() => Faker.System.CommonFileName();

    public string GetContentFile() =>Faker.Lorem.Paragraph();

    public string GetContentType() =>Faker.System.MimeType();
}