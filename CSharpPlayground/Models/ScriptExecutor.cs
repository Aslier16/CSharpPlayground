using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;

namespace CSharpPlayground.Models;

public class ScriptExecutor
{
    public static async Task<string> ExecuteScript(string code)
    {
        var output = string.Empty;

        try
        {
            // 创建一个 StringWriter 来捕获控制台输出
            var outputBuilder = new StringBuilder();
            await using var consoleOutput = new StringWriter(outputBuilder);

            // 保存原始控制台输出流并替换为我们的 StringWriter
            var originalOutput = Console.Out;
            Console.SetOut(consoleOutput);

            try
            {
                var options = ScriptOptions.Default
                    .WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                        .Where(x => !x.IsDynamic && !string.IsNullOrEmpty(x.Location)))
                    .WithImports("System", "System.Linq", "System.Collections.Generic", "System.IO", "CSharpPlayground.Models");
                var script = CSharpScript.Create(code, options);
                var result = script.RunAsync().GetAwaiter().GetResult();

                // 收集控制台输出
                string consoleResult = outputBuilder.ToString();

                // 合并返回值和控制台输出
                if (!string.IsNullOrEmpty(consoleResult))
                {
                    output = consoleResult;
                }
                

                // 如果有返回值且没有控制台输出，则使用返回值
                if (result.ReturnValue != null && string.IsNullOrEmpty(output))
                {
                    output = result.ReturnValue.ToString();
                }
                else if (result.ReturnValue != null)
                {
                    // 如果同时有控制台输出和返回值，则两者都显示
                    output += $"\n返回值: {result.ReturnValue}";
                }

                // 确保 output 不为 null
                output ??= "脚本执行完毕，无输出";
            }
            finally
            {
                // 恢复原始控制台输出流
                Console.SetOut(originalOutput);
            }
        }
        catch (Exception ex)
        {
            output = $"执行错误: {ex.Message}";
        }

        return output;
    }
}