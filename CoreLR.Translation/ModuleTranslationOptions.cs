using System.IO;

namespace CoreLR.Translation
{
	public sealed class ModuleTranslationOptions
	{
		public ModuleTranslationOptions(string inputPath)
		{
			InputPath = inputPath;
			OutputPath = Path.ChangeExtension(inputPath, ".elf");
		}

		public string InputPath { get; set; }
		public string OutputPath { get; set; }

		public string ModuleName
		{
			get { return Path.GetFileNameWithoutExtension(OutputPath); }
		}
	}
}
