// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Validation;

public abstract class ValidationHandler
{
    public abstract void HandleError(ValidationError error);

    public void HandleError(string message) => this.HandleError(new ValidationError(message));
}