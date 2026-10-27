#:property TargetFramework=net11.0
#:package Microsoft.Extensions.DependencyInjection@11.0.0-rc.2.26504.105
#:property PublishAot=false

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.Json;


var previousAverage = (CHF)100000;

IEnumerable<CHF> currentValue = [(CHF)89000, (CHF)100200, (CHF)111000];

var currentAverage = currentValue.Average();
Console.WriteLine($"Current average: {currentAverage}");
Publisher.Publish(new AverageCalculated());

if (currentAverage > previousAverage)
{
    Publisher.Publish(new ValueIncreased(previousAverage, currentAverage));
}
else if (currentAverage < previousAverage)
{
    Publisher.Publish(new ValueDecreased(previousAverage, currentAverage));
}

interface IEvent
{

}

/// <summary>
/// Should go to usage topic that is used to track usage of functionality and be named "average-calculated"
/// </summary>
record class AverageCalculated : IEvent
{

};

/// <summary>
/// Should go to the value-changed topic and be named "value-increased"
/// </summary>
record class ValueIncreased(CHF PreviousAverage, CHF CurrentAverage) : IEvent
{

};

/// <summary>
/// Should go to the value-changed topic and be named "value-decreased"
/// </summary>
record class ValueDecreased(CHF PreviousAverage, CHF CurrentAverage) : IEvent
{

};


static class Publisher
{

    public static void Publish<TEvent>(TEvent domainEvent) where TEvent : notnull, IEvent
    {
        var payload = JsonSerializer.Serialize(domainEvent) ?? throw new InvalidOperationException("Failed to serialize domain event.");
        Publish("value-changed", typeof(TEvent).Name, payload);
    }

    // Do not change this method signature or its behavior
    private static void Publish(string topic, string metadataEventName, string payload)
    {
        bool isTopicValid = topic is "value-changed" or "usage";
        bool isEventNameValid = metadataEventName is "value-increased" or "value-decreased" or "average-calculated";

        if (!isTopicValid || !isEventNameValid)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid topic '{topic}' or event name '{metadataEventName}'.");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"Publishing event '{metadataEventName}' to topic '{topic}' with payload: {payload}");
        }
    }
}


static class ExtensionsMMethods
{
    public static T Average<T>(this IEnumerable<T> values)
    {
        // Implement average calculation here
        return default;
    }
}

record struct CHF(decimal Value)
    : INumberBase<CHF>
{
    public static CHF One => new(decimal.One);
    public static int Radix => 10;
    public static CHF Zero => new(decimal.Zero);
    public static CHF AdditiveIdentity => Zero;
    public static CHF MultiplicativeIdentity => One;

    public static CHF Abs(CHF value) => new(decimal.Abs(value.Value));
    public static bool IsCanonical(CHF value) => true;
    public static bool IsComplexNumber(CHF value) => false;
    public static bool IsEvenInteger(CHF value) => decimal.IsEvenInteger(value.Value);
    public static bool IsFinite(CHF value) => true;
    public static bool IsImaginaryNumber(CHF value) => false;
    public static bool IsInfinity(CHF value) => false;
    public static bool IsInteger(CHF value) => decimal.IsInteger(value.Value);
    public static bool IsNaN(CHF value) => false;
    public static bool IsNegative(CHF value) => decimal.IsNegative(value.Value);
    public static bool IsNegativeInfinity(CHF value) => false;
    public static bool IsNormal(CHF value) => !IsZero(value);
    public static bool IsOddInteger(CHF value) => decimal.IsOddInteger(value.Value);
    public static bool IsPositive(CHF value) => decimal.IsPositive(value.Value);
    public static bool IsPositiveInfinity(CHF value) => false;
    public static bool IsRealNumber(CHF value) => true;
    public static bool IsSubnormal(CHF value) => false;
    public static bool IsZero(CHF value) => value.Value == decimal.Zero;

    public static CHF MaxMagnitude(CHF x, CHF y) => new(decimal.MaxMagnitude(x.Value, y.Value));
    public static CHF MaxMagnitudeNumber(CHF x, CHF y) => MaxMagnitude(x, y);
    public static CHF MinMagnitude(CHF x, CHF y) => new(decimal.MinMagnitude(x.Value, y.Value));
    public static CHF MinMagnitudeNumber(CHF x, CHF y) => MinMagnitude(x, y);

    public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out CHF result)
        where TOther : INumberBase<TOther>
    {
        try
        {
            result = value switch
            {
                CHF chfValue => chfValue,
                _ => new(decimal.CreateChecked(value))
            };
        }
        catch { result = default; }

        return result != default;
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out CHF result)
        where TOther : INumberBase<TOther>
    {
        try
        {
            result = value switch
            {
                CHF chfValue => chfValue,
                _ => new(decimal.CreateSaturating(value))
            };
        }
        catch { result = default; }

        return result != default;
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out CHF result)
        where TOther : INumberBase<TOther>
    {
        try
        {
            result = value switch
            {
                CHF chfValue => chfValue,
                _ => new(decimal.CreateTruncating(value))
            };
        }
        catch { result = default; }

        return result != default;
    }

    public static bool TryConvertToChecked<TOther>(CHF value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(CHF))
        {
            result = (TOther)(object)value;
            return true;
        }

        try
        {
            result = TOther.CreateSaturating(value.Value);
            return true;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }

    public static bool TryConvertToSaturating<TOther>(CHF value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(CHF))
        {
            result = (TOther)(object)value;
            return true;
        }

        try
        {
            result = TOther.CreateSaturating(value.Value);
            return true;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }

    public static bool TryConvertToTruncating<TOther>(CHF value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(CHF))
        {
            result = (TOther)(object)value;
            return true;
        }

        try
        {
            result = TOther.CreateTruncating(value.Value);
            return true;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }

    public static CHF operator +(CHF value) => value;
    public static CHF operator +(CHF left, CHF right) => new(left.Value + right.Value);
    public static CHF operator -(CHF value) => new(-value.Value);
    public static CHF operator -(CHF left, CHF right) => new(left.Value - right.Value);
    public static CHF operator ++(CHF value) => new(value.Value + decimal.One);
    public static CHF operator --(CHF value) => new(value.Value - decimal.One);
    public static CHF operator *(CHF left, CHF right) => new(left.Value * right.Value);
    public static CHF operator /(CHF left, CHF right) => new(left.Value / right.Value);
    public static CHF operator /(CHF value, int divisor) => new(value.Value / divisor);

    public static bool operator <(CHF left, CHF right) => left.Value < right.Value;
    public static bool operator >(CHF left, CHF right) => left.Value > right.Value;
    public static bool operator <=(CHF left, CHF right) => left.Value <= right.Value;
    public static bool operator >=(CHF left, CHF right) => left.Value >= right.Value;

    public static implicit operator CHF(decimal value) => new(value);
    public static explicit operator decimal(CHF value) => value.Value;

    public int CompareTo(CHF other) => Value.CompareTo(other.Value);
    public override string ToString() => Value.ToString(CultureInfo.CurrentCulture);
    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => Value.TryFormat(destination, out charsWritten, format, provider);

    public static CHF Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
        => throw new NotSupportedException();

    public static CHF Parse(string s, NumberStyles style, IFormatProvider? provider)
        => throw new NotSupportedException();

    public static CHF Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => throw new NotSupportedException();

    public static CHF Parse(string s, IFormatProvider? provider)
        => throw new NotSupportedException();

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out CHF result)
        => throw new NotSupportedException();

    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out CHF result)
        => throw new NotSupportedException();

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out CHF result)
        => throw new NotSupportedException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out CHF result)
        => throw new NotSupportedException();
}