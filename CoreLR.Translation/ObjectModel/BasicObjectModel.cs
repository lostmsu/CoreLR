namespace CoreLR.Translation.ObjectModel
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Mono.Cecil;
	using Mono.Cecil.Cil;
	using TypeID = System.String;

	class BasicObjectModel
	{
		readonly ConcurrentDictionary<TypeID, TypeDefinition> pendingTypes =
			new ConcurrentDictionary<string, TypeDefinition>();

		public async Task<AssemblyDefinition> Translate(AssemblyDefinition assemblyDefinition)
		{
			Contract.Requires(assemblyDefinition != null);

			var targetAssembly = AssemblyDefinition.CreateAssembly(assemblyDefinition.Name,
				assemblyDefinition.MainModule.Name,assemblyDefinition.MainModule.Kind);

			var translationTasks =
				assemblyDefinition.Modules.Select(module => Translate(module, targetAssembly.MainModule));

			await Task.WhenAll(translationTasks.ToArray());
			return targetAssembly;
		}

		private async Task Translate(ModuleDefinition source, ModuleDefinition target)
		{
			var translationTasks =
				source.Types.Select(type => Translate(type));

			var types = await Task.WhenAll(translationTasks.ToArray());
			foreach (var type in types) target.Types.Add(type);
		}

		private Task<TypeDefinition> Translate(TypeDefinition type)
		{
			TypeDefinition translated;
			return this.pendingTypes.TryGetValue(GetTypeID(type), out translated)
				? Task.FromResult(translated)
				: Task.Run(() => TranslateImpl(type));
		}

		private TypeID GetTypeID(TypeDefinition type)
		{
			return type.FullName;
		}

		private Task<TypeReference> TranslateTypeRerefence(TypeReference type)
		{
			return Task.Run(() => TranslateTypeReferenceImpl(type));
		}

		private TypeReference TranslateTypeReferenceImpl(TypeReference type)
		{
			var attributes = Translate(type.Attributes);
			var target = new TypeDefinition(type.Namespace, type.Name, attributes);

			var existing = this.pendingTypes.GetOrAdd(GetTypeID(type), target);
			return existing;
		}

		private TypeDefinition TranslateImpl(TypeDefinition type)
		{
			var attributes = Translate(type.Attributes);
			var target = new TypeDefinition(type.Namespace, type.Name, attributes);

			var existing = this.pendingTypes.GetOrAdd(GetTypeID(type), target);
			if (existing != target)
				return existing;

			foreach (var field in type.Fields)
			{
				Translate(field, target);
			}

			return target;
		}

		private async void Translate(FieldDefinition field, TypeDefinition target)
		{
			var reference = new TypeReference()
			var targetType = await TranslateTypeRerefence(field.FieldType);
			var target = new FieldDefinition(field.Name, field.Attributes, targetType);
		}

		private TypeAttributes Translate(TypeAttributes typeAttributes)
		{
			// TODO: support beforefieldinit
			return (typeAttributes | TypeAttributes.Sealed) & ~TypeAttributes.Abstract;
		}

		private void Translate(MethodDefinition methodDefinition)
		{
			Contract.Requires(methodDefinition != null);
			Contract.Requires(methodDefinition.HasBody);
			Contract.Requires<NotImplementedException>(!methodDefinition.Body.HasExceptionHandlers,
				"Exceptions handlers are not implemented");
			// TODO: fix labels up

			var gen = methodDefinition.Body.GetILProcessor();
			Translate(methodDefinition, gen, 0, methodDefinition.Body.Instructions.Count - 1);
		}

		int Translate(MethodDefinition methodDefinition, ILProcessor gen, int start, int end)
		{
			int i = start;
			while(i <= end)
			{
				var instruction = methodDefinition.Body.Instructions[i];
				int replacementSize = Translate(instruction, gen);
				end = checked (end - 1 + replacementSize);
				i = checked (i + replacementSize);
			}

			return checked (end - start + 1);
		}

		private int Translate(Instruction instruction, ILProcessor gen)
		{
			throw new NotImplementedException();
		}
	}
}
