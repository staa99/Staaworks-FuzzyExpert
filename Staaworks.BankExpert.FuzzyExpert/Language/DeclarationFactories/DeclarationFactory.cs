using Staaworks.BankExpert.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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


                if (hasOptions)
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
}
