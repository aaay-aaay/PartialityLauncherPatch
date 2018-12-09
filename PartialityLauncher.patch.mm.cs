using System;
using MonoMod;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.InlineRT;

namespace MonoMod
{
    internal static class MonoModRules
    {
        static MonoModRules()
        {
            MonoModRule.Modder.PostProcessors += PostProcessor;
        }

        public static void PostProcessor(MonoModder modder)
        {
            foreach (TypeDefinition typeDefinition in modder.Module.GetTypes()) // every type MonoMod is dealing with right now
            {
                if (typeDefinition.FullName == "PartialityLauncher.MainForm") // get the MainForm
                {
                    foreach (MethodDefinition methodDefinition in typeDefinition.Methods) // every method in MainForm, including constructors + properties + etc.
                    {
                        if (methodDefinition.IsConstructor) // if it's the constructor
                        {
                            MonoModRules.ReplaceSize(methodDefinition);
                        }
                    }
                }
            }
        }

        public static void ReplaceSize(MethodDefinition method)
        {
            if (!method.HasBody)
                throw new Exception(); // the method must have an body or we can't change numbers
            bool first = true;
            bool second = false;
            foreach (Instruction instruction in method.Body.Instructions) // IL is a stack-based "language" that uses individual instructions that manipulate the stack and create the equivalent of complex expressions.
            {
                if (instruction.OpCode == OpCodes.Ldc_I4 && (int)instruction.Operand >= 9) // ldc.i4 "loads" a 4-byte integer, the operand. To make sure we only get sizes, we check it is >= 9.
                {
                    instruction.Operand = (int)instruction.Operand / 2; // half the operand
                    if (first) // if it is the first number, the width of the entire form
                    {
                        first = false; // the next one is not the first number
                        second = true; // the next one is the second number
                        instruction.Operand = (int)instruction.Operand + 160; // increase it
                    }
                    else if (second) // if it is the second number, the height of the entire form
                    {
                        instruction.Operand = (int)instruction.Operand + 100; // increase it
                        second = false; // the next one is not the second number
                    }
                }
            }
        }
    }
