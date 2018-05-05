using Staaworks.BankExpert.FuzzyExpert.Language.DeclarationFactories;
using Staaworks.BankExpert.FuzzyExpert.Language.Exceptions;
using Staaworks.BankExpert.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Staaworks.BankExpert.FuzzyExpert.Language
{
    public static class StatementBuilder
    {
        private const string LV = "var";
        private const string FS = "set";
        private const string RULE = "rule";
        private const string OPS = "=";
        private const string separator = "$$$";
        private static int tokenLabelIndex = 0;

        public static (DefinitionType defType, Declaration declaration) Build(int index, string line)
        {
            Declaration declaration = null;
            DefinitionType? definitionType = null;
            string error = null;
            var tokens = Regex.Split(line, "\\s+");

            try
            {
                var type = tokens[0].ToLower(); // Must be one of LV, FS, RULE
                if (type != RULE && type != LV && type != FS)
                {
                    error = string.Format("Unknown declaration type: must be one of ({0}, {1}, {2})", LV, FS, RULE);
                }

                var identifier = tokens[1];
                var op = tokens[2];

                if (OPS != op)
                {
                    error = string.Format("Unknown value separator: must be '{0}'. Remember to put a space between the separator and the identifier", OPS);
                }


                if (error == null)
                {
                    var value = "";
                    var rest = tokens.Skip(3);
                    foreach (var t in rest)
                    {
                        value += t + " ";
                    }

                    value.TrimEnd();


                    switch (type)
                    {
                        case LV:
                            definitionType = DefinitionType.Variable;
                            declaration = new VariableDeclaration
                            {
                                Identifier = identifier,
                                Value = ParseVariableValue(value)
                            };
                            break;

                        case FS:
                            definitionType = DefinitionType.FuzzySet;
                            declaration = new FuzzySetDeclaration
                            {
                                Identifier = identifier,
                                Value = ParseFuzzySetValue(value.Trim())
                            };
                            break;

                        case RULE:
                            definitionType = DefinitionType.Rule;
                            declaration = new RuleDeclaration
                            {
                                Identifier = identifier,
                                Value = value
                            };
                            break;
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                error = "Malformed declaration";
            }


            if (error != null)
            {
                throw new InterpretationException(error, index);
            }

            if (declaration == null || definitionType == null || !definitionType.HasValue)
            {
                throw new InterpretationException("Could not interpret statement", index);
            }


            return (definitionType.Value, declaration);
        }


        // min: MIN, max: MAX, labels: [L1; L2; ...]
        private static VariableDeclaration.ValueType ParseVariableValue(string raw, int lineNumber = -1)
        {
            float min = float.NaN, max = float.NaN;
            List<string> fuzzySetNames = new List<string>();

            var tokens = raw.Split(',').Select(v => v.Trim());

            foreach (var token in tokens)
            {
                if (token.StartsWith("min:"))
                {
                    var minString = token.Substring(4).Trim();
                    bool minTaken = float.TryParse(minString, out min);

                    if (!minTaken)
                    {
                        throw new InterpretationException("The value passed as min could not be parsed", lineNumber);
                    }
                }
                else if (token.StartsWith("max:"))
                {
                    var maxString = token.Substring(4).Trim();
                    bool maxTaken = float.TryParse(maxString, out max);

                    if (!maxTaken)
                    {
                        throw new InterpretationException("The value passed as max could not be parsed", lineNumber);
                    }
                }
                else if (token.StartsWith("labels:"))
                {
                    var list = token.Substring(7).Trim();
                    if (!list.StartsWith("[") || !list.EndsWith("]"))
                    {
                        throw new InterpretationException("Expecting a list", lineNumber);
                    }


                    var labelString = list.Substring(1, list.Length - 2);
                    fuzzySetNames = labelString.Split(';').Select(v => v.Trim()).ToList();

                    if (!fuzzySetNames.Any())
                    {
                        throw new InterpretationException("The variable must be a member of at least one fuzzy set", lineNumber);
                    }
                }
                else
                {
                    throw new InterpretationException("Unexpected token: " + token, lineNumber);
                }
            }

            if (min == double.NaN || max == double.NaN || !fuzzySetNames.Any())
            {
                throw new InterpretationException("Incomplete value", lineNumber);
            }


            return new VariableDeclaration.ValueType
            {
                Min = min,
                Max = max,
                FuzzySetNames = fuzzySetNames
            };
        }


        // [1; 2; edge: left; max: 1]
        private static Dictionary<string, string> ParseFuzzySetValue(string raw, int lineNumber = -1)
        {
            if (!raw.StartsWith("[") || !raw.EndsWith("]"))
            {
                throw new InterpretationException("Invalid fuzzy set value: list must be wrapped in brackets [ ]", lineNumber);
            }

            var tokens = raw.Substring(1, raw.Length - 2).Split(';').Select(val => val.Trim());
            Dictionary<string, string> values = new Dictionary<string, string>();

            int index = 0;
            foreach (var token in tokens)
            {
                if (token.StartsWith("edge:"))
                {
                    var edge = token.Substring(5).Trim();
                    if (!(new string[] { "right", "left" }).Contains(edge))
                    {
                        throw new InterpretationException("Unexpected value for edge. Expected right or left but found " + edge, lineNumber);
                    }

                    values["edge"] = edge;
                }
                else if (token.StartsWith("max:") || token.StartsWith("min:"))
                {
                    var minMax = double.Parse(token.Substring(4).Trim());
                    if (minMax > 1 || minMax < 0)
                    {
                        throw new InterpretationException("Max value is not within range", lineNumber);
                    }

                    if (token[2] == 'x')
                    {
                        values["max"] = minMax.ToString();
                    }
                    else
                    {
                        values["min"] = minMax.ToString();
                    }
                }
                else if (double.TryParse(token, out double value))
                {
                    values[index.ToString()] = value.ToString();
                    index++;
                }
                else
                {
                    throw new InterpretationException("Unexpected token: " + token, lineNumber);
                }
            }

            if (!values.ContainsKey("0") || !values.ContainsKey("1") || (!values.ContainsKey("2") && !values.ContainsKey("edge")))
            {
                throw new InterpretationException("Incomplete values passed for fuzzy set", lineNumber);
            }

            return values;
        }




        public static (FuzzySetDeclaration fs, VariableDeclaration var, RuleDeclaration rule, DefinitionType type) Parse(LinkedListNode<string> line, ref Context context, int lineNumber = -1)
        {
            var nodes = new LinkedList<string>(SpaceLine(line.Value).Trim().Split(' ').Where(s => s.Any()));

            var stack = new Stack<string>();

            int index = 0;

            var node = nodes.First;
            var type = node.Value.ToLower(); // Must be one of LV, FS, RULE
            if (type != RULE && type != LV && type != FS)
            {
                var error = string.Format("Unknown declaration type: must be one of ({0}, {1}, {2})", LV, FS, RULE);
                throw new InterpretationException(error, lineNumber);
            }

            stack.Push(type);


            var identifier = node.Next;

            stack.Push(identifier.Value);

            var op = identifier.Next;

            stack.Push(op.Value);

            var tokens = GetValue(stack, op.Next, index, lineNumber);

            (FuzzySetDeclaration fs, VariableDeclaration var, RuleDeclaration rule, DefinitionType type) declaration;

            switch (type)
            {
                case RULE:
                    declaration = (null, null, tokens.ToRuleDeclaration(identifier.Value), DefinitionType.Rule);
                    break;
                case LV:
                    declaration = (null, tokens.ToVariableDeclaration(identifier.Value, ref context), null, DefinitionType.Variable);
                    break;
                case FS:
                    declaration = (tokens.ToFuzzySetDeclaration(identifier.Value), null, null, DefinitionType.FuzzySet);
                    break;
                default:
                    // We know that it will never reach here but c# will not let us compile, so 
                    declaration = (null, null, null, default(DefinitionType));
                    break;
            }

            return declaration;
        }



        private static IEnumerable<Token> GetValue(Stack<string> stack, LinkedListNode<string> node, int index, int lineNumber)
        {
            var dic = new Dictionary<string, object>();
            var codeStack = new Stack<Token>();

            int bracketsLevel = 0;
            bool newListElement = false;
            bool freeTextStarted = false;
            Token currentRootToken = null;
            string path = "";

            // start value
            while (node != null)
            {
                string previousNodeValue = node.Previous.Value;
                string nodeValue = node.Value;
                string nextNodeValue = node.Next?.Value;

                try
                {
                    if (nodeValue == "\"")
                    {
                        freeTextStarted = !freeTextStarted;
                    }
                    else if (freeTextStarted)
                    {
                        if (codeStack.Count == 0)
                        {
                            throw new InterpretationException("Cannot use free text without using a label", lineNumber);
                        }


                        Token newToken;
                        if (previousNodeValue == "\"" &&
                            (
                                (
                                    path != "" &&
                                    currentRootToken[path].Type != TokenType.text
                                ) ||
                                (
                                    path == "" &&
                                    (currentRootToken.Type != TokenType.text ||
                                    currentRootToken != "")
                                ))
                            )
                        {
                            newToken = new Token(TokenType.text);

                            if (bracketsLevel == 0)
                            {
                                currentRootToken = newToken;

                                if (codeStack.Peek() != currentRootToken)
                                    codeStack.Push(currentRootToken);
                            }
                            else
                            {
                                if (path != "")
                                {
                                    path += "$$$";
                                }

                                path += newToken.Label;

                                currentRootToken[path] = newToken;
                            }
                        }
                        else
                        {
                            newToken = currentRootToken;
                        }
                                                
                        if (path == "")
                        {
                            if (newToken.StringValue != "")
                            {
                                newToken.Value = newToken.StringValue + " ";
                            }

                            newToken.Value = newToken.StringValue + nodeValue;
                        }
                        else if (path != "")
                        {
                            if (newToken[path].StringValue != "")
                            {
                                newToken[path].Value = newToken[path].StringValue + " ";
                            }

                            newToken[path].Value = newToken[path].StringValue + nodeValue;
                        }
                    }



                    else if (nodeValue == "[")
                    {
                        if (codeStack.Count == 0 || codeStack.Peek().Type != TokenType.list)
                        {
                            var newToken = new Token(TokenType.list);

                            if (bracketsLevel == 0)
                            {
                                currentRootToken = newToken;
                                codeStack.Push(currentRootToken);
                            }
                            else
                            {

                                if (path != "")
                                {
                                    path += "$$$";
                                }

                                path += newToken.Label;

                                currentRootToken[path] = newToken;
                            }
                        }

                        newListElement = true;
                        bracketsLevel++;
                    }

                    else if (nodeValue == "]")
                    {
                        // remove last path
                        path = string.Join(separator, path.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries).Reverse().Skip(1).Reverse().ToArray());
                        bracketsLevel--;
                    }

                    else if (nodeValue == ",")
                    {
                        // remove last path
                        path = string.Join(separator, path.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries).Reverse().Skip(1).Reverse().ToArray());
                        newListElement = true;
                    }

                    else if (nodeValue == ";")
                    {
                        stack.Push("nitem");

                        // remove last path
                        path = string.Join(separator, path.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries).Reverse().Skip(1).Reverse().ToArray());
                    }

                    else if (nodeValue.EndsWith(":") || stack.Count == 0 || stack.Peek() == "nitem" || newListElement )
                    {
                        Token nextToken;
                        if (nodeValue.EndsWith(":"))
                        {
                            nextToken = nextNodeValue == "[" ?
                                new Token(TokenType.list, label: nodeValue) :
                                new Token(TokenType.text, label: nodeValue);
                            stack.Push("item");
                            newListElement = false;
                        }
                        else // it has to be a text token
                        {
                            nextToken = new Token(TokenType.text, textValue: nodeValue);
                        }

                        if (bracketsLevel == 0)
                        {
                            currentRootToken = nextToken;
                            codeStack.Push(currentRootToken);
                        }
                        else
                        {
                            if (path != "")
                            {
                                path += "$$$";
                            }

                            path += nextToken.Label;

                            currentRootToken[path] = nextToken;
                        }
                    }

                    else if (stack.Peek() == "item")
                    {
                        if (currentRootToken != null)
                        {
                            if (currentRootToken.Type == TokenType.text)
                            {
                                currentRootToken.Value = ((string)currentRootToken) + " " + nodeValue;
                            }
                            else
                            {
                                if (path != "")
                                {
                                    if (currentRootToken[path].Type == TokenType.text)
                                    {
                                        if (currentRootToken[path].StringValue != "")
                                        {
                                            currentRootToken[path].Value = currentRootToken[path].StringValue + " ";
                                        }
                                        currentRootToken[path].Value = currentRootToken[path].StringValue + nodeValue;
                                    }
                                    else
                                    {
                                        path += "$$$";

                                        Token t;
                                        if (nextNodeValue == "[")
                                        {
                                            t = new Token(TokenType.list, label: nodeValue);
                                        }
                                        else
                                        {
                                            t = new Token(TokenType.text, textValue: nodeValue);
                                        }
                                        path += t.Label;

                                        currentRootToken[path] = t;
                                    }
                                }
                                else
                                {
                                    Token t;
                                    if (nextNodeValue == "[")
                                    {
                                        t = new Token(TokenType.list, label: nodeValue);
                                    }
                                    else
                                    {
                                        t = new Token(TokenType.text, textValue: nodeValue);
                                    }
                                    path += t.Label;

                                    currentRootToken[path] = t;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InterpretationException("Unexpected token: " + nodeValue, lineNumber, ex);
                }

                node = node.Next;
            }

            return codeStack;
        }


        private static string SpaceLine(string line)
        {
            char[] special = { '(', ')', '[', ']', '=', ',', '{', '}', ';', '"' };

            foreach (char c in special)
            {
                line = line.Replace(c + "", " " + c + " ");
            }

            return line;
        }



        public class Token
        {
            public Token(TokenType type, string label = null, string textValue = "")
            {
                Type = type;

                switch (type)
                {
                    case TokenType.text:
                        Value = textValue;
                        Label = label ?? string.Format("param{0}:", tokenLabelIndex + 1);
                        break;
                    case TokenType.list:
                        Label = label ?? string.Format("list{0}:", ++tokenLabelIndex);
                        Value = new TokenList();
                        break;
                }
                if (label == null) tokenLabelIndex++;
            }



            public string Label { get; set; }
            public object Value { private get; set; }




            public static implicit operator string(Token token)
            {
                if (token.Type == TokenType.text)
                {
                    return token.StringValue;
                }
                else
                {
                    throw new InvalidCastException("This token cannot be a string");
                }
            }
            

            public static implicit operator TokenList(Token token)
            {
                if (token.Type == TokenType.list)
                {
                    return token.ListValue;
                }
                else
                {
                    throw new InvalidOperationException("This token cannot be a list");
                }
            }


            public Token this[int index]
            {
                get => ((TokenList)this)[index];
            }


            public Token this[string key]
            {
                get => ((TokenList)this)[key];
                set => ((TokenList)this)[key] = value;
            }


            public TokenType Type { get; }
            public string StringValue => Type == TokenType.text && Value is string ? Value.ToString() : null;
            public TokenList ListValue => Type == TokenType.list ? Value as TokenList : null;

            public override string ToString()
            {
                switch (Type)
                {
                    case TokenType.text:
                        return Label + " " + StringValue;

                    case TokenType.list:
                        return "\n" + string.Join("\r\n", ListValue.TokenPairs.Select(x => x.Value));
                    default:
                        return "Token not evaluated";
                }
            }
        }


        public class TokenList
        {
            private string lastListToken;
            public TokenList()
            {
                Dic = new Dictionary<string, Token>();
            }
            private int index = 0;
            private Dictionary<string, Token> Dic { get; set; }


            public void Add(Token token)
            {
                Dic[index.ToString()] = token;
                index++;
            }


            public void Add(string key, Token token)
            {
                this[key] = token;
            }

            public Token Last => Dic[lastListToken];




            public bool HasKey(int key)
            {
                return key >= 0 && key <= index;
            }

            public bool HasKey (string key) => Dic.ContainsKey (key);
            public IEnumerable<KeyValuePair<string, Token>> TokenPairs => Dic;


            public Token this[string key]
            {
                get
                {
                    if (!key.Contains(separator))
                    {
                        return Dic[key];
                    }
                    else
                    {
                        var labelQueue = new Queue<string>(key.Trim(separator.ToCharArray()).Split(new string[]{ separator }, StringSplitOptions.RemoveEmptyEntries));
                        Token token = null;
                        var dic = Dic;

                        while (labelQueue.Any())
                        {
                            var k = labelQueue.Dequeue();
                            if (dic.ContainsKey(k))
                            {
                                token = dic[k];
                                dic = token.ListValue?.Dic;
                            }
                            else
                            {
                                token = null;
                                break;
                            }
                        }

                        return token;
                    }
                }

                set
                {
                    if (key != "")
                    {
                        if (!key.Contains(separator))
                        {
                            Dic[key] = value;
                            lastListToken = key;
                        }
                        else
                        {
                            var labelQueue = new Queue<string>(key.Trim(separator.ToCharArray()).Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries));
                            Token token = null;
                            var dic = Dic;

                            while (labelQueue.Any())
                            {
                                var k = labelQueue.Dequeue();
                                if (dic.ContainsKey(k))
                                {
                                    token = dic[k];
                                }
                                else
                                {
                                    if (labelQueue.Count == 0)
                                    {
                                        dic[k] = value;
                                    }
                                    else
                                    {
                                        dic[k] = new Token(TokenType.list, label: k);
                                        token = dic[k];
                                    }
                                }
                                dic = token.ListValue.Dic;
                            }
                        }
                    }
                }
            }

            public Token this[int index]
            {
                get
                {
                    if (index < 0 || index > this.index)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    return this[index.ToString()];
                }
            }
        }


        public enum TokenType
        {
            text, list
        }
    }
}