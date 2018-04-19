using Staaworks.BankExpert.FuzzyExpert.Language.DeclarationFactories;
using Staaworks.BankExpert.Shared.Cache;
using System;
using System.Collections.Generic;

namespace StaaworksBankExpertCommandLineTests
{
    class Program
    {
        static void Main(string[] args)
        {
            SystemGeneratedSourceCache.Data = new Dictionary<string, object>
            {
                ["a"] = -2,
                ["b"] = 3
            };

            var statement = "multiply(sum(a,b), subtract(a,b))";
            var a = SourceParser.Evaluate(statement);
            Console.WriteLine("{0} = {1}", statement, a);
        }
    }
}
