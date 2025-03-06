// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Repository;

using Entity;
using SeedWork;
using SeedWork.SearchableRepository;

public interface IVideoRepository : IGenericRepository<Video>, ISearchableRepository<Video>
{
    
}