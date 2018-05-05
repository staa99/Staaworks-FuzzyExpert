using Staaworks.BankExpert.FuzzyExpert.Language.DeclarationFactories;
using Staaworks.BankExpert.Shared.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StaaworksBankExpertCommandLineTests
{
    class Program
    {
        static void Main (string[] args)
        {
            SystemGeneratedSourceCache.Data = new Dictionary<string, object>
            {
                ["a"] = -2,
                ["b"] = 3
            };

            var statement = "multiply(sum(a,b), square(3))";
            var a = SourceParser.Evaluate(statement);
            var toAndBack = SourceParser.ToAndBack(statement);
            Console.WriteLine("{0} = {1}", statement, a);
            Console.WriteLine("{0} = {1}", statement, toAndBack);
        }


        //class TestLinqGroup
        //{
        //    static void Main ()
        //    {
        //        string instanceId1 = Guid.NewGuid().ToString(),
        //            instanceId2 = Guid.NewGuid().ToString(),
        //            instanceId3 = Guid.NewGuid().ToString();

        //        string farmer1Name = "Adetona",
        //            farmer2Name = "Adeola",
        //            farmer3Name = "Sunmonu";

        //        var list = new List<(string IDH_InstaceId, string InstanceId, string FarmerName, string uploadDate, string data)>
        //        {
        //            (instanceId1, Guid.NewGuid().ToString(), farmer1Name, DateTime.Now.Subtract(TimeSpan.FromDays(3)).ToString(), "Sample data 1"),
        //            (instanceId2, Guid.NewGuid().ToString(), farmer2Name, DateTime.Now.Subtract(TimeSpan.FromDays(3)).ToString(), "Sample data 2"),
        //            (instanceId2, Guid.NewGuid().ToString(), farmer2Name, DateTime.Now.Subtract(TimeSpan.FromDays(2)).ToString(), "Sample data 2 update 1"),
        //            (instanceId1, Guid.NewGuid().ToString(), farmer1Name, DateTime.Now.Subtract(TimeSpan.FromDays(2)).ToString(), "Sample data 1 update 1"),
        //            (instanceId3, Guid.NewGuid().ToString(), farmer3Name, DateTime.Now.Subtract(TimeSpan.FromDays(3)).ToString(), "Sample data 3"),
        //            (instanceId2, Guid.NewGuid().ToString(), farmer2Name, DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToString(), "Sample data 2 update 2"),
        //            (instanceId1, Guid.NewGuid().ToString(), farmer1Name, DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToString(), "Sample data 1 update 2"),
        //            (instanceId3, Guid.NewGuid().ToString(), farmer3Name, DateTime.Now.Subtract(TimeSpan.FromDays(2)).ToString(), "Sample data 3 update 1"),
        //            (instanceId3, Guid.NewGuid().ToString(), farmer3Name, DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToString(), "Sample data 3 update 2"),
        //            (instanceId3, Guid.NewGuid().ToString(), farmer3Name, DateTime.Now.ToString(), "Sample data 3 update 3"),
        //            (instanceId1, Guid.NewGuid().ToString(), farmer1Name, DateTime.Now.ToString(), "Sample data 1 update 3"),
        //            (instanceId2, Guid.NewGuid().ToString(), farmer2Name, DateTime.Now.ToString(), "Sample data 2 update 3"),
        //        };

        //        var latest = list.GroupBy(d => d.IDH_InstaceId).Select(g => g.OrderByDescending(ge => DateTime.Parse(ge.uploadDate)).ElementAt(0)).ToList();
        //        foreach(var entry in latest)
        //        {
        //            Console.WriteLine(entry);
        //        }
        //    }
        //}
    }
}
