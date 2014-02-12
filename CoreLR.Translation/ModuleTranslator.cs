using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LLVM;
using Mono.Cecil;

namespace CoreLR.Translation
{
	public sealed class ModuleTranslator
	{
		public ModuleTranslator(ModuleTranslationOptions options)
		{
			Contract.Requires(options != null);

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

		readonly HashSet<IMemberDefinition> translated = new HashSet<IMemberDefinition>();

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
			Contract.Requires(typeDefinition != null);

			TranslateStaticFields(typeDefinition);

			foreach (var methodDefinition in typeDefinition.Methods)
			{
				Translate(methodDefinition);
			}
		}

		void Translate(MethodDefinition methodDefinition)
		{
			Contract.Requires(methodDefinition != null);
			Contract.Requires<InvalidOperationException>(!translated.Contains(methodDefinition));
			Contract.Ensures(translated.Contains(methodDefinition));

			translated.Add(methodDefinition);

			var function = DeclareFunction(methodDefinition);
			var prologue = new Block("prologue", context, function);

			throw new NotImplementedException();
		}

		Function DeclareFunction(MethodDefinition methodDefinition)
		{
			throw new NotImplementedException();
		}

		void TranslateStaticFields(TypeDefinition typeDefinition)
		{
			Contract.Requires(typeDefinition != null);
			Contract.Requires<InvalidOperationException>(!translated.Contains(typeDefinition));
			Contract.Ensures(translated.Contains(typeDefinition));

			translated.Add(typeDefinition);

			throw new NotImplementedException();
		}
	}
}
