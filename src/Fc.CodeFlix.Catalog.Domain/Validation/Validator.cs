// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Validation;

public abstract class Validator
{
    protected readonly ValidationHandler Handler;
    protected Validator(ValidationHandler handler) => this.Handler = handler;

    public abstract void Validate();
}