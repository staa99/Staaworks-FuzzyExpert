using Staaworks.BankExpert.FuzzyExpert.Language.Formatting;
using System;
using System.Collections.Generic;

namespace Staaworks.BankExpert.FuzzyExpert.Language
{
    public abstract class Declaration
    {
        protected Declaration(DefinitionType definition)
        {
            Definition = definition;
        }

        public DefinitionType Definition { get; protected set; }
        public string Identifier { get; set; }
        public abstract object Value { get; set; }


        public abstract string ToDeclarationString();
    }



    public class VariableDeclaration: Declaration
    {
        public VariableDeclaration() : base (DefinitionType.Variable)
        {

        }

        public ValueType RealValue { get; private set; }

        public override object Value
        {
            get => RealValue;

            set
            {
                if (value is ValueType)
                {
                    RealValue = value as ValueType;
                }
            }
        }


        public class ValueType
        {
            public float Min { get; set; }
            public float Max { get; set; }
            public List<string> FuzzySetNames { get; set; }
        }
        public enum Type
        {
            question, output
        }


        public override string ToDeclarationString()
        {
            return String.Format("var {0} => min: {1}, max: {2}, labels: {3}", Identifier, RealValue.Min, RealValue.Max, RealValue.FuzzySetNames.ToDString());
        }
    }




    public class FuzzySetDeclaration : Declaration
    {
        public FuzzySetDeclaration() : base(DefinitionType.FuzzySet) {}

        public Dictionary<string, string> RealValue { get; private set; }
        
        public override object Value
        {
            get => RealValue;

            set
            {
                if (value is Dictionary<string, string>)
                {
                    RealValue = value as Dictionary<string, string>;
                }
            }
        }


        public override string ToDeclarationString()
        {
            return String.Format("set {0} => {1}", Identifier, RealValue.ToDString());
        }
    }



    public class RuleDeclaration : Declaration
    {
        public RuleDeclaration() : base(DefinitionType.Rule)
        {

        }

        public string RealValue { get; private set; }

        public override object Value
        {
            get => RealValue;

            set
            {
                if (value is string)
                {
                    RealValue = value as string;
                }
            }
        }


        public override string ToDeclarationString()
        {
            return String.Format("rule {0} => {1}", Identifier, RealValue);
        }
    }
}
