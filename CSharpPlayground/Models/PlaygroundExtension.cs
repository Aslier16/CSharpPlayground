using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tmds.DBus.Protocol;

namespace CSharpPlayground.Models;

public static class PlaygroundExtension
{
    // public static T Dump<T>(this T input) where T : struct
    // {
    //     Console.WriteLine(input);
    //     return input;
    // }

    public static T Dump<T>(this T input)
    {
        if (input is null)
        {
            Console.WriteLine("null");
            return input;
        }

        var type = input.GetType();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        var sb = new StringBuilder();
        try
        {
            sb.AppendLine($"Type: {type.Name}");

            sb.AppendLine("Fields\tValue");
            if (fields.Length > 0)
            {
                foreach (var field in fields)
                {
                    var value = field.GetValue(input);
                    sb.AppendLine($"{field.Name}\t{value}");
                }
            }
            else
            {
                sb.AppendLine("No fields available.");
            }

            sb.AppendLine("Properties\tValue");
            if (properties.Length > 0)
            {
                foreach (var property in properties)
                {
                    if (property.CanRead && property.GetIndexParameters().Length == 0)
                    {
                        var value = property.GetValue(input);
                        sb.AppendLine($"{property.Name}: {value}");
                    }
                }
            }
            else
            {
                sb.AppendLine("无属性或属性不可见.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.WriteLine(sb.ToString());
        }

        return input;
    }

    public static IEnumerable<T> Dump<T>(this IEnumerable<T> input)
    {
        var sb = new StringBuilder();
        using (var enumerator = input.GetEnumerator())
        {
            sb.Append(enumerator.Current + " ");
            while (enumerator.MoveNext())
            {
                sb.Append(" " + enumerator.Current);
            }
        }

        Console.WriteLine(sb.ToString());
        return input;
    }
    
}