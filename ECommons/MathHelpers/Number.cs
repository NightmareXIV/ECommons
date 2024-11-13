using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.MathHelpers;
public readonly record struct Number
{
    public readonly int Value;

    public Number(int value)
    {
        Value = value;
    }

    public static implicit operator uint(Number n) => (uint)n.Value;
    public static implicit operator int(Number n) => n.Value;
    public static implicit operator Number(int n) => new(n);
    public static implicit operator Number(uint n) => new((int)n);

    public readonly override string ToString()
    {
        return $"{Value}";
    }
}
