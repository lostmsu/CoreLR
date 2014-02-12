using System;
using LLVM;
using Mono.Cecil;

namespace CoreLR.Translation
{
	public sealed class ModuleTranslator
	{
		public ModuleTranslator(ModuleTranslationOptions options)
		{
			resolver = new DefaultAssemblyResolver();
			context = Context.Global;
			module = new Module(options.ModuleName, context);
			assembly = resolver.Resolve(options.InputPath);
			this.options = options;
		}

		readonly ModuleTranslationOptions options;
		readonly BaseAssemblyResolver resolver;
		readonly Context context;
		readonly Module module;
		readonly AssemblyDefinition assembly;

		public void Translate()
		{
			foreach (var moduleDefinition in assembly.Modules)
			{
				foreach (var typeDefinition in moduleDefinition.Types)
				{
					Translate(typeDefinition);
				}
			}
		}

		void Translate(TypeDefinition typeDefinition)
		{
			TranslateStaticFields(typeDefinition);

			foreach (var methodDefinition in typeDefinition.Methods)
			{
				Translate(methodDefinition);
			}
		}

		void Translate(MethodDefinition methodDefinition)
		{
			throw new NotImplementedException();
		}

		void TranslateStaticFields(TypeDefinition typeDefinition)
		{
			throw new NotImplementedException();
		}
	}
}
