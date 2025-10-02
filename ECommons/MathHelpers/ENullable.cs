using InteropGenerator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.MathHelpers;
[Serializable]
public partial struct ENullable<T> where T : struct
{
    private readonly bool hasValue; // Do not rename (binary serialization)
    internal T value; // Do not rename (binary serialization) or make readonly (can be mutated in ToString, etc.)

    public ENullable(T value)
    {
        this.value = value;
        hasValue = true;
    }

    public readonly bool HasValue
    {
        
        get => hasValue;
    }

    public readonly T Value
    {
        get
        {
            if(!hasValue)
            {
                throw new InvalidOperationException("Nullable object has no value");
            }
            return value;
        }
    }

    
    public readonly T GetValueOrDefault() => value;

    
    public readonly T GetValueOrDefault(T defaultValue) =>
        hasValue ? value : defaultValue;

    public override bool Equals(object? other)
    {
        if(!hasValue) return other == null;
        if(other == null) return false;
        return value.Equals(other);
    }

    public override int GetHashCode() => hasValue ? value.GetHashCode() : 0;

    public override string? ToString() => hasValue ? value.ToString() : "";

    
    public static implicit operator ENullable<T>(T value)
    {
        return new ENullable<T>(value);
    }

    
    public static explicit operator T(ENullable<T> value)
    {
        return value!.Value;
    }

    
    public static implicit operator ENullable<T>(T? value)
    {
        if(!value.HasValue) return new ENullable<T>();
        return new ENullable<T>(value.Value);
    }

    
    public static implicit operator T?(ENullable<T> value)
    {
        if(!value.HasValue) return new T?();
        return value!.Value;
    }

    public static bool operator ==(ENullable<T> left, ENullable<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ENullable<T> left, ENullable<T> right)
    {
        return !(left == right);
    }
}

public static class ENullable
{
    public static int Compare<T>(ENullable<T> n1, ENullable<T> n2) where T : struct
    {
        if(n1.HasValue)
        {
            if(n2.HasValue) return Comparer<T>.Default.Compare(n1.value, n2.value);
            return 1;
        }
        if(n2.HasValue) return -1;
        return 0;
    }

    public static bool Equals<T>(ENullable<T> n1, ENullable<T> n2) where T : struct
    {
        if(n1.HasValue)
        {
            if(n2.HasValue) return EqualityComparer<T>.Default.Equals(n1.value, n2.value);
            return false;
        }
        if(n2.HasValue) return false;
        return true;
    }

    // If the type provided is not a Nullable Type, return null.
    // Otherwise, return the underlying type of the Nullable type
    public static Type? GetUnderlyingType(Type nullableType)
    {
        ArgumentNullException.ThrowIfNull(nullableType);

        if(nullableType.IsGenericType && !nullableType.IsGenericTypeDefinition)
        {
            // Instantiated generic type only
            var genericType = nullableType.GetGenericTypeDefinition();
            if(ReferenceEquals(genericType, typeof(ENullable<>)))
            {
                return nullableType.GetGenericArguments()[0];
            }
        }
        return null;
    }

    /// <summary>
    /// Retrieves a readonly reference to the location in the <see cref="ENullable{T}"/> instance where the value is stored.
    /// </summary>
    /// <typeparam name="T">The underlying value type of the <see cref="ENullable{T}"/> generic type.</typeparam>
    /// <param name="nullable">The readonly reference to the input <see cref="ENullable{T}"/> value.</param>
    /// <returns>A readonly reference to the location where the instance's <typeparamref name="T"/> value is stored. If the instance's <see cref="ENullable{T}.HasValue"/> is false, the current value at that location may be the default value.</returns>
    /// <remarks>
    /// As the returned readonly reference refers to data that is stored in the input <paramref name="nullable"/> value, this method should only ever be
    /// called when the input reference points to a value with an actual location and not an "rvalue" (an expression that may appear on the right side but not left side of an assignment). That is, if this API is called and the input reference
    /// points to a value that is produced by the compiler as a defensive copy or a temporary copy, the behavior might not match the desired one.
    /// </remarks>
    public static ref readonly T GetValueRefOrDefaultRef<T>(ref readonly ENullable<T> nullable)
        where T : struct
    {
        return ref nullable.value;
    }
}