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
			MonoModder modder = MonoModRule.Modder;
			modder.PostProcessors = (PostProcessor)Delegate.Combine(modder.PostProcessors, new PostProcessor(MonoModRules.PostProcessor));
		}

		public static void PostProcessor(MonoModder modder)
		{
			foreach (TypeDefinition typeDefinition in modder.Module.GetTypes())
			{
				if (typeDefinition.FullName == "PartialityLauncher.MainForm")
				{
					foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
					{
						if (methodDefinition.IsConstructor)
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
				throw new Exception();
			bool second = false;
			bool after = false;
			foreach (Instruction instruction in method.Body.Instructions)
			{
				if (instruction.OpCode == OpCodes.Ldc_I4 && (int)instruction.Operand >= 9)
				{
					instruction.Operand = (int)instruction.Operand / 2;
					if (second)
					{
						instruction.Operand = (int)instruction.Operand + 100;
						second = false;
						after = true;
					}
					else if (!after)
					{
						second = true;
						instruction.Operand = (int)instruction.Operand + 160;
					}
				}
			}
		}
	}
}