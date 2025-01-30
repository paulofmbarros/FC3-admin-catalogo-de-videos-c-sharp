// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.SeedWork;


// Fazemos isto porque o que define um value object é o facto de todas as propriedades serem iguais. Dai não precisar de um identificador.
// Se tivermos um identificador, então é uma entidade.
// Se todas as propriedades forem iguais então é o mesmo value Object, ao contrario do que acontece nas entidades, que podem ter as propriedades iguais mas serem entidades diferentes devido ao ID.
public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract bool Equals(ValueObject? other);
    protected abstract int GetCustomHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return this.Equals((ValueObject)obj);
    }


    public override int GetHashCode() => this.GetCustomHashCode();

    public static bool operator ==(ValueObject a, ValueObject b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(ValueObject a, ValueObject b) => !(a == b);
}