using Staaworks.BankExpert.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Staaworks.BankExpert.Shared.Cache;
using static Staaworks.BankExpert.FuzzyExpert.Language.StatementBuilder;

namespace Staaworks.BankExpert.FuzzyExpert.Language.DeclarationFactories
{
    public static class VariableDeclarationFactory
    {
        public static VariableDeclaration ToVariableDeclaration(this IEnumerable<Token> tokens, string identifier, ref Context context)
        {
            try
            {
                bool hasMin = false,
                    hasMax = false,
                    hasText = false,
                    hasLabels = false,
                    hasOutputDefinitionType = false,
                    hasDefinitions = false,
                    hasSource = false,
                    hasOptions = false,
                    isOverflowOutput = false;

                const string OVERFLOW_DEFINITION_TYPE = "overflow", FIXED_DEFINITION_TYPE = "fixed";

                float min = 0, max = 0;
                string text = "",
                        outputDefinitionType = "",
                        source = "";

                List<string> labels = null;
                List<OutputDefinition> definitions = null;
                List<Option> options = null;

                foreach (var token in tokens)
                {
                    if (token.Label == "min:")
                    {
                        if (token.Type != TokenType.text)
                        {
                            throw new ArgumentException("min must not be a list");
                        }

                        if (!float.TryParse(((string)token), out float minValue))
                        {
                            throw new ArgumentException("min must be a number");
                        }

                        hasMin = true;
                        min = minValue;
                    }

                    else if (token.Label == "max:")
                    {
                        if (token.Type != TokenType.text)
                        {
                            throw new ArgumentException("max must not be a list");
                        }

                        if (!float.TryParse(((string)token), out float maxValue))
                        {
                            throw new ArgumentException("max must be a number");
                        }

                        hasMax = true;
                        max = maxValue;
                    }

                    else if (token.Label == "labels:")
                    {
                        if (token.Type != TokenType.list)
                        {
                            throw new ArgumentException("labels must be a list");
                        }

                        if (!token.ListValue.TokenPairs.Any())
                        {
                            throw new ArgumentException("labels must not be empty");
                        }


                        hasLabels = true;
                        labels = token.ListValue.TokenPairs.Select(p => p.Value.StringValue).ToList();
                    }

                    else if (token.Label == "text:")
                    {
                        if (token.Type != TokenType.text)
                        {
                            throw new ArgumentException("text must not be a list");
                        }

                        if (string.IsNullOrWhiteSpace(((string)token)))
                        {
                            throw new ArgumentException("text must not be blank");
                        }

                        hasText = true;
                        text = ((string)token);
                    }

                    else if (token.Label == "source:")
                    {
                        if (token.Type != TokenType.text)
                        {
                            throw new ArgumentException("source must not be a list");
                        }

                        if (string.IsNullOrWhiteSpace(((string)token)))
                        {
                            throw new ArgumentException("source must not be blank");
                        }

                        hasText = true;
                        source = token;
                        hasSource = true;
                    }

                    else if (token.Label == "definitionsType:")
                    {
                        if (token.Type != TokenType.text)
                        {
                            throw new ArgumentException("definitionsType must not be a list");
                        }

                        if (((string)token).Trim() != OVERFLOW_DEFINITION_TYPE && token != FIXED_DEFINITION_TYPE)
                        {
                            throw new ArgumentException("Definitions type must be one of fixed and overflow");
                        }

                        isOverflowOutput = ((string)token).Trim() == OVERFLOW_DEFINITION_TYPE;
                        hasOutputDefinitionType = true;
                        outputDefinitionType = token;
                    }

                    else if (token.Label == "outputDefinitions:")
                    {
                        if (token.Type != TokenType.list)
                        {
                            throw new ArgumentException("outputDefinitions must be a list");
                        }

                        if (!token.ListValue.TokenPairs.Any())
                        {
                            throw new ArgumentException("outputDefinitions must not be empty");
                        }
                        definitions = new List<OutputDefinition>();
                        hasDefinitions = true;
                        foreach (var definition in token.ListValue.TokenPairs.Select(p => p.Value.ListValue))
                        {
                            if (!definition.HasKey("isContext:"))
                            {
                                definition["isContext:"] = new Token(TokenType.text, label: "isContext:", textValue: "false");
                            }

                            definitions.Add(new OutputDefinition
                            {
                                IsContext = bool.Parse(definition["isContext:"].StringValue),
                                Text = definition["text:"].StringValue,
                                Value = float.Parse(definition["value:"].StringValue)
                            });
                        }

                        definitions = definitions.OrderBy(d => d.Value).ToList();
                    }

                    else if (token.Label == "options:")
                    {
                        if (token.Type != TokenType.list)
                        {
                            throw new ArgumentException("options must be a list");
                        }

                        if (!token.ListValue.TokenPairs.Any())
                        {
                            throw new ArgumentException("options must not be empty");
                        }

                        options = new List<Option>();
                        hasOptions = true;

                        bool alreadyHasFreeText = false;
                        foreach (var (label, option) in token.ListValue.TokenPairs.Select(p => (p.Value.Label, p.Value.ListValue)))
                        {
                            if (!option.HasKey("text:"))
                            {
                                option["text:"] = new Token(TokenType.text, label: "text:", textValue: label);
                            }

                            if (!option.HasKey("type:"))
                            {
                                option["type:"] = new Token(TokenType.text, label: "type:", textValue: "fixed");
                            }

                            if (option["type:"].StringValue == "freetext")
                            {
                                if (alreadyHasFreeText)
                                {
                                    throw new ArgumentException("There can be only one free text option");
                                }
                                alreadyHasFreeText = true;
                            }

                            options.Add(new Option
                            {
                                Value = float.Parse(option["value:"].StringValue),
                                Type = option["type:"].StringValue == "fixed" ?
                                                    OptionType.constant :
                                                    option["type:"].StringValue == "freetext" ?
                                                                    OptionType.freetext :
                                                                    throw new ArgumentException("Option type has to be one of (fixed | freetext)"),
                                UserText = option["text:"].StringValue
                            });
                        }
                    }
                }


                if (!hasMin || !hasMax || !hasLabels)
                {
                    throw new ArgumentException("min, max and labels are compulsory");
                }

                if (hasOptions) // This is supposed to be a question variable
                {
                    if (!hasSource)
                    {
                        hasSource = true;
                        source = "user";
                    }

                    if (!hasText)
                    {
                        text = identifier;
                    }
                }


                if (hasDefinitions)
                {
                    if (!hasOutputDefinitionType)
                    {
                        hasOutputDefinitionType = true;
                        outputDefinitionType = "overflow";
                        isOverflowOutput = true;
                    }
                }

                var declaration = new VariableDeclaration
                {
                    Value = new VariableDeclaration.ValueType
                    {
                        Min = min,
                        Max = max,
                        FuzzySetNames = labels
                    },
                    Identifier = identifier
                };


                if (hasSource)
                {
                    context.Questions.Add(new Question
                    {
                        Name = identifier,
                        UserText = text,
                        Options = options,
                        Source = source
                    });
                }
                else if (hasDefinitions)
                {
                    context.Output = new Output
                    {
                        Name = identifier,
                        Type = isOverflowOutput ? Shared.Models.DefinitionType.overflow : Shared.Models.DefinitionType.constant,
                        Definitions = definitions
                    };
                }
                else
                {
                    throw new ArgumentException("A variable must represent either a question or an output");
                }

                return declaration;
            }
            catch (Exception ex)
            {
                var message = String.Format("Error occurred while creating the variable with id: {0}", identifier);
                throw new InvalidOperationException(message, ex);
            }
        }
    }


    public static class FuzzySetDeclarationFactory
    {
        public static FuzzySetDeclaration ToFuzzySetDeclaration(this IEnumerable<Token> tokens, string identifier)
        {
            try
            {
                if (tokens.Count() != 1)
                {
                    throw new ArgumentException("The token list for the creation of a fuzzy set declaration must have exactly one element:" +
                        " (The fuzzy set)");
                }

                var token = tokens.ElementAt(0);

                if (token.Type != TokenType.list)
                {
                    throw new ArgumentException("The token to be converted into a fuzzy set must be a token with 'Type = TokenType.list'");
                }


                if (token.ListValue.TokenPairs.Count() < 3)
                {
                    throw new ArgumentException("The token to be converted to a fuzzy set must have at least three elements");
                }


                var unnamedEntries = token.ListValue.
                    TokenPairs.Where(p => p.Key.StartsWith("param") && char.IsDigit(p.Key[5]))
                                .OrderBy(p => Convert.ToInt32(p.Key.Substring(5).TrimEnd(':')))
                                .Select(p => p.Value.StringValue)
                                .ToArray();

                var namedEntries = token.ListValue.
                    TokenPairs.Where(p => !p.Key.StartsWith("param") || !char.IsDigit(p.Key[5]))
                    .Select(p => p.Value)
                    .ToDictionary(t => t.Label, t => ((string)t));

                if (unnamedEntries.Length < 2 ||
                    unnamedEntries.Length > 4 ||
                    (unnamedEntries.Length == 2 && !namedEntries.ContainsKey("edge:")) ||
                    (unnamedEntries.Length == 4 && namedEntries.ContainsKey("edge:")))
                {
                    throw new ArgumentException("The token to be converted to a fuzzy set must have at least two and at most four unnamed elements in the order that describes the shape. If there are two elements, an edge must be given.\n If there are three elements with an edge, a trapezoid from top along that edge.\nIf there are three elements without an edge, a triangle is formed\nIf there are four elements, there must be no edge given and a trapezoid is formed");
                }


                var result = new Dictionary<string, string>();

                for (int i = 0; i < unnamedEntries.Length; i++)
                {
                    result[i.ToString()] = unnamedEntries[i];
                }


                if (namedEntries.ContainsKey("edge:"))
                {
                    result["edge"] = namedEntries["edge:"];
                }

                if (namedEntries.ContainsKey("max:"))
                {
                    result["max"] = namedEntries["max:"];
                }

                if (namedEntries.ContainsKey("min:"))
                {
                    result["min"] = namedEntries["min:"];
                }

                var declaration = new FuzzySetDeclaration
                {
                    Identifier = identifier,
                    Value = result
                };
                return declaration;
            }
            catch (Exception ex)
            {
                var message = String.Format("Error occurred while creating the rule from the tokens extracted from the declaration with id: {0}", identifier);
                throw new InvalidOperationException(message, ex);
            }
        }
    }




    public static class RuleDeclarationFactory
    {
        public static RuleDeclaration ToRuleDeclaration(this IEnumerable<Token> tokens, string identifier)
        {
            try
            {
                string clause = tokens.Single(t => t.Label == "clause:");
                Token descriptionToken = tokens.Where(t => t.Label == "description:").SingleOrDefault();
                string description;
                if (descriptionToken != null)
                {
                    description = descriptionToken;
                }

                return new RuleDeclaration
                {
                    Identifier = identifier,
                    Value = clause
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("The rule with id: " + identifier + " was not correctly declared", ex);
            }
        }
    }





    public static class SourceParser
    {
        public static object Evaluate(string source)
        {
            OpTree function = source;
            return function.Evaluate();
        }

        public static string ToAndBack(string source)
        {
            OpTree tree = source;
            return tree;
        }


        private class OpTree
        {
            public string Operator { get; set; }
            public OpType Type { get; set; }
            public OpTree Operand1 { get; set; }
            public OpTree Operand2 { get; set; }
            

            public object Evaluate()
            {
                Operator = Operator.Trim();

                if (Type == OpType.value)
                {
                    return SystemGeneratedSourceCache.Data[Operator];
                }
                else if (Type == OpType.binary)
                {
                    var a = Convert.ToDouble(Operand1.Evaluate());
                    var b = Convert.ToDouble(Operand2.Evaluate());

                    switch (Operator)
                    {
                        case "sum": return a + b;
                        case "subtract": return a - b;
                        case "multiply": return a * b;
                        case "divide": return a / b;
                        case "mod": return a % b;
                        case "pow": return Math.Pow(a, b);
                        case "log": return Math.Log(a, b);
                        case "max": return Math.Max(a, b);
                        case "min": return Math.Min(a, b);
                    }
                }
                else 
                {
                    var a = Convert.ToDouble(Operand1.Evaluate());

                    switch (Operator)
                    {
                        case "abs": return Math.Abs(a);
                        case "sin": return Math.Sin(a);
                        case "arcsin": return Math.Asin(a);
                        case "cos": return Math.Cos(a);
                        case "arccos": return Math.Acos(a);
                        case "tan": return Math.Tan(a);
                        case "arctan": return Math.Atan(a);
                        case "sinh": return Math.Sinh(a);
                        case "cosh": return Math.Cosh(a);
                        case "tanh": return Math.Tanh(a);
                        case "round": return Math.Round(a);
                        case "sqrt": return Math.Sqrt(a);
                        case "square": return Math.Pow(a, 2);
                        case "cbrt": return Math.Pow(a, 1 / (3 + 0.0));
                        case "cube": return Math.Pow(a, 3);
                        case "pi": return Math.PI * a;
                        case "e": return Math.E * a;
                        case "ln": return Math.Log(a);
                        case "log": return Math.Log10(a);
                        case "exp": return Math.Exp(a);
                    }
                }

                throw new InvalidOperationException("The function: " + Operator + " is not defined");
            }

            public static implicit operator OpTree (string raw)
            {
                raw = raw.Trim();

                var parenthesisStartIndex = raw.IndexOf('(');
                var parenthesisEndIndex = raw.LastIndexOf(')');

                var commaIndex = -1;
                var level = 0;
                for (var i = parenthesisStartIndex + 2; i < parenthesisEndIndex; i++)
                {
                    if (level == 0 && raw[i] == ',')
                    {
                        commaIndex = i;
                        break;
                    }
                    else if (raw[i] == '(')
                    {
                        level++;
                    }
                    else if (raw[i] == ')')
                    {
                        level--;
                    }
                }

                if (parenthesisStartIndex > 0 && parenthesisEndIndex > parenthesisStartIndex)
                {
                    var type = commaIndex > parenthesisStartIndex ? OpType.binary : OpType.unary;
                    string op;
                    OpTree op1, op2;


                    op = string.Join("", raw.Take(parenthesisStartIndex));

                    int op1StartIndex, op1EndIndex, op2StartIndex, op2EndIndex;

                    op1StartIndex = parenthesisStartIndex + 1;

                    if (type == OpType.unary)
                    {
                        op1EndIndex = parenthesisEndIndex - 1;
                        op1 = raw.Substring(op1StartIndex, op1EndIndex - op1StartIndex + 1);
                        op2 = null;
                    }
                    else
                    {
                        op1EndIndex = commaIndex - 1;
                        op2StartIndex = commaIndex + 1;
                        op2EndIndex = parenthesisEndIndex - 1;

                        op1 = raw.Substring(op1StartIndex, op1EndIndex - op1StartIndex + 1);
                        op2 = raw.Substring(op2StartIndex, op2EndIndex - op2StartIndex + 1);
                    }

                    return new OpTree
                    {
                        Operand1 = op1,
                        Operand2 = op2,
                        Operator = op,
                        Type = type
                    };
                }
                else
                {
                    if (double.TryParse(raw, out var value))
                    {
                        SystemGeneratedSourceCache.Data[raw] = value;
                    }
                    else if (!SystemGeneratedSourceCache.Data.ContainsKey(raw))
                    {
                        throw new InvalidOperationException("You can't use the value: " + raw + " in an expression because it has not been cached");
                    }

                    return new OpTree
                    {
                        Operand1 = null,
                        Operand2 = null,
                        Operator = raw,
                        Type = OpType.value
                    };
                }
            }
            public static implicit operator string (OpTree tree)
            {
                if (tree.Type != OpType.value)
                {
                    var output = "";

                    output += tree.Operator;
                    output += "(";
                    output += tree.Operand1;

                    if (tree.Type == OpType.binary)
                    {
                        output += ", ";
                        output += tree.Operand2;
                    }

                    output += ")";

                    return output;
                }
                else
                {
                    return tree.Operator;
                }
            }
        }

        private enum OpType
        {
            unary, binary, value
        }
    }
}
