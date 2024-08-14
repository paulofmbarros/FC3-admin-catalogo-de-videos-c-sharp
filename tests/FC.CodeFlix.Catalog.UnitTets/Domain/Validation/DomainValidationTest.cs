// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Validation;

using System.ComponentModel;
using Bogus;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using Fc.CodeFlix.Catalog.Domain.Validation;
using FluentAssertions;

public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();


    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOk()
    {
        var value = this.Faker.Commerce.ProductName();
        Action action = () => DomainValidation.NotNull(value, "Value");

        action.Should().NotThrow();

    }

    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullThrowWhenNull()
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        var value = this.Faker.Commerce.ProductName();
        Action action = () => DomainValidation.NotNull(null, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null.");

    }

    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        var value = this.Faker.Commerce.ProductName();
        Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be empty or null.");

    }

    [Fact(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOk()
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        var value = this.Faker.Commerce.ProductName();
        Action action = () => DomainValidation.NotNullOrEmpty(value, fieldName);

        action.Should().NotThrow<EntityValidationException>();

    }

    [Theory(DisplayName = nameof(MinLengthThrowWhenLessThanMinLength))]
    [MemberData(nameof(GetValuesSmallerThanMinLength), parameters: 10)]
    [Trait("Domain", "DomainValidation - Validation")]
    public void MinLengthThrowWhenLessThanMinLength(string target, int minLength)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        Action action = () => DomainValidation.MinLength(target, minLength, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should have at least {minLength} characters.");

    }

    public static IEnumerable<object[]> GetValuesSmallerThanMinLength(int numberOfTests = 6)
    {
        var faker = new Faker();
        for (var i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length + faker.Random.Int(1, 20);
            yield return new object[]
            {
                example, minLength
            };
        }
    }

    [Theory(DisplayName = nameof(MinLengthOk))]
    [MemberData(nameof(GetValuesGreaterThanMinLength), parameters: 10)]
    [Trait("Domain", "DomainValidation - Validation")]
    public void MinLengthOk(string target, int minLength)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        Action action = () => DomainValidation.MinLength(target, minLength, fieldName);

        action.Should().NotThrow<EntityValidationException>();

    }

    [Theory(DisplayName = nameof(MaxLengthThrowWhenGreaterThanMaxLength))]
    [MemberData(nameof(GetValuesGreaterThanMax), parameters: 10)]
    [Trait("Domain", "DomainValidation - Validation")]
    public void MaxLengthThrowWhenGreaterThanMaxLength(string target, int maxLength)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        Action action = () => DomainValidation.MaxLength(target, maxLength, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should have less than {maxLength} characters.");

    }

    [Theory(DisplayName = nameof(MaxLengthOk))]
    [MemberData(nameof(GetValuesLessThanMax), parameters: 10)]
    [Trait("Domain", "DomainValidation - Validation")]
    public void MaxLengthOk(string target, int maxLength)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        Action action = () => DomainValidation.MaxLength(target, maxLength, fieldName);

        action.Should().NotThrow<EntityValidationException>();

    }

    public static IEnumerable<object[]> GetValuesGreaterThanMax(int numberOfTests = 6)
    {
        var faker = new Faker();
        for (var i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length - faker.Random.Int(1, 5);
            yield return new object[]
            {
                example, maxLength
            };
        }
    }

    public static IEnumerable<object[]> GetValuesLessThanMax(int numberOfTests = 6)
    {
        var faker = new Faker();
        for (var i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length + faker.Random.Int(0, 5);
            yield return new object[]
            {
                example, maxLength
            };
        }
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMinLength(int numberOfTests = 6)
    {
        var faker = new Faker();
        for (var i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length - faker.Random.Int(1, 5);
            yield return new object[]
            {
                example, minLength
            };
        }
    }

}