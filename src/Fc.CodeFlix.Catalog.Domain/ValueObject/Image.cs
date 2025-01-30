// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.ValueObject;

using SeedWork;

public class Image : ValueObject
{
    public string Path { get; }

    public Image(string path)
    {
        Path = path;
    }

    public override bool Equals(ValueObject? other) => other is Image && this.Path == ((Image)other).Path;

    protected override int GetCustomHashCode() => HashCode.Combine(this.Path);
}