// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Validation;

public class NotificationValidationHandler : ValidationHandler
{
    private readonly List<ValidationError> errors;

    public IReadOnlyCollection<ValidationError> Errors => this.errors.AsReadOnly();

    public bool HasErrors() => this.errors.Count > 0;

    public NotificationValidationHandler() => this.errors = [];

    public override void HandleError(ValidationError error) => this.errors.Add(error);
}