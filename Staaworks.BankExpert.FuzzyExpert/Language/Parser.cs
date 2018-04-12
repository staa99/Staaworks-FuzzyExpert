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
            int index = 1;
            while (node != null)
            {
                var line = node.Value;
                var declaration = StatementBuilder.Parse(node, ref context, lineNumber: index);


                switch (declaration.type)
                {
                    case DefinitionType.Variable:
                        if (vars.Any(d => d.Identifier == declaration.var.Identifier))
                        {
                            throw new InterpretationException("Duplicate identifiers found", index);
                        }
                        vars.Add(declaration.var);
                        break;

                    case DefinitionType.FuzzySet:
                        if (fsets.Any(d => d.Identifier == declaration.fs.Identifier))
                        {
                            throw new InterpretationException("Duplicate identifiers found", index);
                        }
                        fsets.Add(declaration.fs);
                        break;

                    case DefinitionType.Rule:
                        if (rules.Any(d => d.Identifier == declaration.rule.Identifier))
                        {
                            throw new InterpretationException("Duplicate identifiers found", index);
                        }
                        rules.Add(declaration.rule);
                        break;
                }
                node = node.Next;
            }

            return (vars, fsets, rules);
        }
    }
}