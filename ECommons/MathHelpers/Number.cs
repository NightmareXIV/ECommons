using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.MathHelpers;
public record struct Number
{
    public int Value;

    public static implicit operator uint(Number n) => (uint)n.Value;
    public static implicit operator int(Number n) => n.Value;
    public static implicit operator Number(int n) => new() { Value = n };
    public static implicit operator Number(uint n) => new() { Value = (int)n };

    public readonly override string ToString()
    {
        return $"{Value}";
    }
}
