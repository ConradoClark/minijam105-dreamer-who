
using System;

public class FrameVariableDefinition<T>
{
    public string Variable { get; set; }
    public Func<T> Getter { get; set; }

    public FrameVariableDefinition(string variable, Func<T> getter)
    {
        Variable = variable;
        Getter = getter;
    }
}

