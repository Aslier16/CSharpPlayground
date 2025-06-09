using System;

namespace CSharpPlayground.Models;

public static class PlaygroundExtension
{
    public static T Dump<T>(this T input)
    {
        Console.WriteLine(input);
        return input;
    }
}