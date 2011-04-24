namespace NativeBuilder
{
    public sealed partial class NativeBuilder
    {
        public object Build(CodeGenTest.CodeGenModule codeGenModule, out bool success)
        {
            Opus.Core.DependencyNode node = codeGenModule.OwningNode;
            Opus.Core.Target target = node.Target;
            CodeGenTest.CodeGenTool tool = new CodeGenTest.CodeGenTool();
            CodeGenTest.CodeGenOptions toolOptions = codeGenModule.Options as CodeGenTest.CodeGenOptions;

            Opus.Core.StringArray inputFiles = new Opus.Core.StringArray();
            inputFiles.Add(tool.Executable(target));
            Opus.Core.StringArray outputFiles = codeGenModule.Options.OutputPaths.Paths;
            if (!RequiresBuilding(outputFiles, inputFiles))
            {
                Opus.Core.Log.DebugMessage("'{0}' is up-to-date", node.UniqueModuleName);
                success = true;
                return null;
            }

            Opus.Core.StringArray commandLineBuilder = new Opus.Core.StringArray();
            if (toolOptions is CommandLineProcessor.ICommandLineSupport)
            {
                CommandLineProcessor.ICommandLineSupport commandLineOption = toolOptions as CommandLineProcessor.ICommandLineSupport;
                commandLineOption.ToCommandLineArguments(commandLineBuilder, target);

                Opus.Core.DirectoryCollection directoriesToCreate = commandLineOption.DirectoriesToCreate();
                foreach (string directoryPath in directoriesToCreate)
                {
                    NativeBuilder.MakeDirectory(directoryPath);
                }
            }
            else
            {
                throw new Opus.Core.Exception("CodeGen options does not support command line translation");
            }

            int exitCode = CommandLineProcessor.Processor.Execute(node, tool, tool.Executable(target), commandLineBuilder);
            success = (0 == exitCode);

            return null;
        }
    }
}