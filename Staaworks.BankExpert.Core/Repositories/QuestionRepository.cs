using AForge.Fuzzy;
using Staaworks.BankExpert.FuzzyExpert.Engine;
using Staaworks.BankExpert.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Context = Staaworks.BankExpert.Shared.Models.Context;
namespace Staaworks.BankExpert.Core.Repositories
{
    public static class QuestionRepository
    {
        private static InferenceSystem system;


        public static Context LoadContext(string context)
        {
            Loader.SetCurrentContextName(context);
            system = Loader.Load();
            return Loader.CurrentContext;
        }





        public static (string text, Context context) TreatUserInput(Dictionary<Question, Option> input)
        {
            if (system == null)
            {
                throw new Exception("Inference system not yet loaded, first load the inference system for the current context");
            }

            foreach (var pair in input)
            {
                var inputVariableName = pair.Key.Name;
                var value = pair.Value.Value;

                system.SetInput(inputVariableName, value);
            }


            var outputVariableName = Loader.CurrentContext.Output.Name;
            var result = system.Evaluate(outputVariableName);
            OutputDefinition chosenDefinition = null;

            if (Loader.CurrentContext.Output.Type == DefinitionType.constant)
            {
                chosenDefinition = Loader.CurrentContext.Output.Definitions.FirstOrDefault(d => d.Value == result);
            }
            else
            {
                var definitions = new LinkedList<OutputDefinition>(Loader.CurrentContext.Output.Definitions);

                var definition = definitions.First;
            

                while (definition != null && definition.Next != null)
                {
                    if (definition.Value.Value <= result && result <= definition.Next.Value.Value)
                    {
                        chosenDefinition = definition.Value;
                        break;
                    }
                    definition = definition.Next;
                }
            }

            if (chosenDefinition.IsContext) return (null, LoadContext(chosenDefinition.Text));
            else return (chosenDefinition.Text, null);
        }
    }
}
