using Staaworks.BankExpert.FuzzyExpert.Language.Exceptions;
using Staaworks.BankExpert.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Staaworks.BankExpert.FuzzyExpert.Language
{
    public class Parser
    {
        public (List<VariableDeclaration> vars, List<FuzzySetDeclaration> fsets, List<RuleDeclaration> rules) LoadContext(string source, ref Context context)
        {
            source = string.Join("", source.Split('\n').Where(val => !val.Trim().Equals("") && !val.Trim().StartsWith("//") && !val.Trim().StartsWith("#")).ToArray());
            var lines = new LinkedList<string>(source.Split('}').Select(s => Regex.Replace(s, "[\\t|\\r|\\n|\\{]", " ")).Where(val => !val.Trim().Equals("") && !val.Trim().StartsWith("//") && !val.Trim().StartsWith("#")).Select(s => s.Trim()).ToList());
            
            var vars = new List<VariableDeclaration>();
            var fsets = new List<FuzzySetDeclaration>();
            var rules = new List<RuleDeclaration>();


            var node = lines.First;
            var index = 1;
            while (node != null)
            {
                var line = node.Value;
                var (fs, var, rule, type) = StatementBuilder.Parse(node, ref context, lineNumber: index);


                switch (type)
                {
                    case DefinitionType.Variable:
                        if (vars.Any(d => d.Identifier == var.Identifier))
                        {
                            throw new InterpretationException("Duplicate identifiers found", index);
                        }
                        vars.Add(var);
                        break;

                    case DefinitionType.FuzzySet:
                        if (fsets.Any(d => d.Identifier == fs.Identifier))
                        {
                            throw new InterpretationException("Duplicate identifiers found", index);
                        }
                        fsets.Add(fs);
                        break;

                    case DefinitionType.Rule:
                        if (rules.Any(d => d.Identifier == rule.Identifier))
                        {
                            throw new InterpretationException("Duplicate identifiers found", index);
                        }
                        rules.Add(rule);
                        break;
                }

                index++;
                node = node.Next;
            }

            return (vars, fsets, rules);
        }
    }
}