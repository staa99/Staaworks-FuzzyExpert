using AForge.Fuzzy;
using Staaworks.BankExpert.FuzzyExpert.Language;
using Staaworks.BankExpert.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using static AForge.Fuzzy.TrapezoidalFunction;

namespace Staaworks.BankExpert.FuzzyExpert.Engine
{
    public static  class Loader
    {
        private static  readonly string BaseConfigDir = Environment.CurrentDirectory + "\\Config\\Engine\\";
        public static  Context CurrentContext { get; set; }
        private static string Context { get; set; }

        public static void SetCurrentContextName (string name) =>
            Context = name;



        public static InferenceSystem Load(string source = null)
        {
            if (source == null)
            {
                source = File.ReadAllText(BaseConfigDir + Context);
            }

            var parser = new Parser();
            var context = new Context { Name = Context, Declarations = source };

            var declarations = parser.LoadContext(source, ref context);

            CurrentContext = context;
            return LoadFromDeclarations(declarations);
        }



        private static (List<VariableDeclaration> vars, List<FuzzySetDeclaration> fsets, List<RuleDeclaration> rules) SplitDeclarations(IEnumerable<Declaration> declarations)
        {
            var vars = new List<VariableDeclaration>();
            var fsets = new List<FuzzySetDeclaration>();
            var rules = new List<RuleDeclaration>();

            foreach (var declaration in declarations)
            {
                switch (declaration.Definition)
                {
                    case Language.DefinitionType.FuzzySet:
                        fsets.Add(declaration as FuzzySetDeclaration);
                        break;

                    case Language.DefinitionType.Rule:
                        rules.Add(declaration as RuleDeclaration);
                        break;

                    case Language.DefinitionType.Variable:
                        vars.Add(declaration as VariableDeclaration);
                        break;
                }
            }

            return (vars, fsets, rules);
        }



        private static InferenceSystem LoadFromDeclarations((List<VariableDeclaration> vars, List<FuzzySetDeclaration> fsets, List<RuleDeclaration> rules) declarations)
        {
            var fuzzySets = new Dictionary<string, FuzzySet>();

            var db = new Database();


            foreach (var fset in declarations.fsets)
            {
                TrapezoidalFunction membership;
                float max = 1, min = 0;
                var map = fset.RealValue;
                if (map.ContainsKey("max"))
                {
                    max = float.Parse(map["max"]);
                }

                if (map.ContainsKey("min"))
                {
                    min = float.Parse(map["min"]);
                }



                // function must contain 0 and 1.
                if (map.ContainsKey("3")) // 4 points (trapezoid)
                {
                    membership = new TrapezoidalFunction(float.Parse(map["0"]), float.Parse(map["1"]), float.Parse(map["2"]), float.Parse(map["3"]), max, min);
                }
                else if (map.ContainsKey("2")) // 3 points (triangle)
                {
                    membership = new TrapezoidalFunction(float.Parse(map["0"]), float.Parse(map["1"]), float.Parse(map["2"]), max, min);
                }
                else // 2 points, must be with edge
                {
                    var edgeType = MapToEdge(map["edge"]);
                    membership = new TrapezoidalFunction(float.Parse(map["0"]), float.Parse(map["1"]), max, min, edgeType);
                }

                fuzzySets[fset.Identifier] = new FuzzySet(fset.Identifier, membership);
            }


            foreach (var variable in declarations.vars)
            {
                var lvar = new LinguisticVariable(variable.Identifier, variable.RealValue.Min, variable.RealValue.Max);
                foreach (var id in variable.RealValue.FuzzySetNames)
                {
                    lvar.AddLabel(fuzzySets[id]);
                }
                db.AddVariable(lvar);
            }


            var system = new InferenceSystem(db, new CentroidDefuzzifier(1000));

            foreach (var rule in declarations.rules)
            {
                system.NewRule(rule.Identifier, rule.RealValue);
            }

            return system;
        }



        private static EdgeType MapToEdge(string mapValue)
        {
            switch (mapValue)
            {
                case "right":
                    return EdgeType.Right;
                case "left":
                    return EdgeType.Left;
                default: return EdgeType.Left;
            }
        }


        
    }
}
