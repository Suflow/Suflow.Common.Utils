using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suflow.Common.Utils
{
    public class CompilationAndExecutionHelper
    {
        static void Run(string[] args)
        {
            var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
            var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, "foo.exe", true);
            parameters.GenerateExecutable = true;
            CompilerResults results = csc.CompileAssemblyFromSource(parameters,
            @"using System.Linq;
            class Program {
              public static void Main(string[] args) {
                var q = from i in Enumerable.Range(1,100)
                          where i % 2 == 0
                          select i;
              }
            }");
            results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));
        }
    }
}
