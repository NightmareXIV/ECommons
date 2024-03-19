namespace ECommons.ImGuiMethods;

public class Box<T>
{
    public T Value;

    public Box(T value)
    {
        Value = value;
    }

    public Box<T> Clone()
    {
        return new Box<T>(Value);
    }
}
