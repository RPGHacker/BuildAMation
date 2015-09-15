#region License
// Copyright (c) 2010-2015, Mark Final
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of BuildAMation nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion // License
namespace C
{
    namespace DefaultSettings
    {
        public static partial class DefaultSettingsExtensions
        {
            public static void Defaults(this C.ICOnlyCompilerOptions settings, Bam.Core.Module module)
            {
            }
            public static void
            SharedSettings(
                this C.ICOnlyCompilerOptions shared,
                C.ICOnlyCompilerOptions lhs,
                C.ICOnlyCompilerOptions rhs)
            {
            }
            public static void
            Delta(
                this C.ICOnlyCompilerOptions delta,
                C.ICOnlyCompilerOptions lhs,
                C.ICOnlyCompilerOptions rhs)
            {
            }
            public static void
            Clone(
                this C.ICOnlyCompilerOptions settings,
                C.ICOnlyCompilerOptions other)
            {
            }
        }
    }

    public interface ICompilationPolicy
    {
        void
        Compile(
            ObjectFile sender,
            Bam.Core.ExecutionContext context,
            Bam.Core.TokenizedString objectFilePath,
            Bam.Core.Module source);
    }

    public interface ILibrarianPolicy
    {
        void
        Archive(
            StaticLibrary sender,
            Bam.Core.ExecutionContext context,
            Bam.Core.TokenizedString libraryPath,
            System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.Module> objectFiles,
            System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.Module> headers);
    }

    public interface ILinkerPolicy
    {
        void
        Link(
            ConsoleApplication sender,
            Bam.Core.ExecutionContext context,
            Bam.Core.TokenizedString executablePath,
            System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.Module> objectFiles,
            System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.Module> headers,
            System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.Module> libraries,
            System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.Module> frameworks);
    }

    public interface IHeaderLibraryPolicy
    {
        void
        HeadersOnly(
            HeaderLibrary sender,
            Bam.Core.ExecutionContext context,
            System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.Module> headers);
    }

    // TODO: register a tooltype, e.g. compiler, linker, archiver

    public abstract class CompilerTool :
        Bam.Core.PreBuiltTool
    {
        // TODO: is this needed?
        public virtual void
        CompileAsShared(
            Bam.Core.Settings settings)
        {}
    }

    public abstract class LibrarianTool :
        Bam.Core.PreBuiltTool
    { }

    public abstract class LinkerTool :
        Bam.Core.PreBuiltTool
    {
        public abstract void ProcessLibraryDependency(
            CModule executable,
            CModule library);
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple=true)]
    public abstract class ToolRegistrationAttribute :
        System.Attribute
    {
        protected ToolRegistrationAttribute(
            string toolsetName,
            Bam.Core.EPlatform platform,
            EBit bitDepth)
        {
            this.ToolsetName = toolsetName;
            this.Platform = platform;
            this.BitDepth = bitDepth;
        }

        public string ToolsetName
        {
            get;
            private set;
        }

        public Bam.Core.EPlatform Platform
        {
            get;
            private set;
        }

        public EBit BitDepth
        {
            get;
            private set;
        }
    }

    public sealed class RegisterCCompilerAttribute :
        ToolRegistrationAttribute
    {
        public RegisterCCompilerAttribute(
            string toolsetName,
            Bam.Core.EPlatform platform,
            EBit bitDepth)
            :
            base(toolsetName, platform, bitDepth)
        {
        }
    }

    public sealed class RegisterCxxCompilerAttribute :
        ToolRegistrationAttribute
    {
        public RegisterCxxCompilerAttribute(
            string toolsetName,
            Bam.Core.EPlatform platform,
            EBit bitDepth)
            :
            base(toolsetName, platform, bitDepth)
        {
        }
    }

    public sealed class RegisterArchiverAttribute :
        ToolRegistrationAttribute
    {
        public RegisterArchiverAttribute(
            string toolsetName,
            Bam.Core.EPlatform platform,
            EBit bitDepth)
            :
            base(toolsetName, platform, bitDepth)
        {
        }
    }

    public sealed class RegisterCLinkerAttribute :
        ToolRegistrationAttribute
    {
        public RegisterCLinkerAttribute(
            string toolsetName,
            Bam.Core.EPlatform platform,
            EBit bitDepth)
            :
            base(toolsetName, platform, bitDepth)
        {
        }
    }

    public sealed class RegisterCxxLinkerAttribute :
        ToolRegistrationAttribute
    {
        public RegisterCxxLinkerAttribute(
            string toolsetName,
            Bam.Core.EPlatform platform,
            EBit bitDepth)
            :
            base(toolsetName, platform, bitDepth)
        {
        }
    }

    public sealed class RegisterObjectiveCCompilerAttribute :
        ToolRegistrationAttribute
    {
        public RegisterObjectiveCCompilerAttribute(
            string toolsetName,
            Bam.Core.EPlatform platform,
            EBit bitDepth)
            :
            base(toolsetName, platform, bitDepth)
        {
        }
    }

    public sealed class RegisterObjectiveCxxCompilerAttribute :
        ToolRegistrationAttribute
    {
        public RegisterObjectiveCxxCompilerAttribute(
            string toolsetName,
            Bam.Core.EPlatform platform,
            EBit bitDepth)
            :
            base(toolsetName, platform, bitDepth)
        {
        }
    }

    public static class DefaultToolchain
    {
        public class DefaultToolchainCommand :
            Bam.Core.IStringCommandLineArgument
        {
            string Bam.Core.ICommandLineArgument.LongName
            {
                get
                {
                    return "--C.toolchain";
                }
            }

            string Bam.Core.ICommandLineArgument.ShortName
            {
                get
                {
                    return null;
                }
            }

            string Bam.Core.ICommandLineArgument.ContextHelp
            {
                get
                {
                    return "Define the default C toolchain, used as resolution when multiple toolchains are present";
                }
            }
        }

        private static System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<CompilerTool>>  C_Compilers   = new System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<CompilerTool>>();
        private static System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<CompilerTool>>  Cxx_Compilers = new System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<CompilerTool>>();
        private static System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<LibrarianTool>> Archivers     = new System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<LibrarianTool>>();
        private static System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<LinkerTool>>    C_Linkers     = new System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<LinkerTool>>();
        private static System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<LinkerTool>>    Cxx_Linkers   = new System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<LinkerTool>>();
        private static System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<CompilerTool>>  ObjectiveC_Compilers   = new System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<CompilerTool>>();
        private static System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<CompilerTool>>  ObjectiveCxx_Compilers   = new System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<CompilerTool>>();
        // TODO: slightly confusing there is a class and a variable almost identically named
        private static string DefaultToolChain = null;

        private static System.Collections.Generic.IEnumerable<System.Tuple<System.Type,T>>
        GetToolsFromMetaData<T>()
            where T : ToolRegistrationAttribute
        {
            var allTypes = Bam.Core.State.ScriptAssembly.GetTypes();
            foreach (var type in allTypes)
            {
                var tools = type.GetCustomAttributes(typeof(T), false) as T[];
                if (0 == tools.Length)
                {
                    continue;
                }
                foreach (var tool in tools)
                {
                    if (Bam.Core.OSUtilities.CurrentOS != tool.Platform)
                    {
                        continue;
                    }
                    yield return new System.Tuple<System.Type, T>(type, tool);
                }
            }
        }

        private static void
        FindTools<AttributeType, ToolType>(
            System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<ToolType>> collection)
            where AttributeType : ToolRegistrationAttribute
            where ToolType : Bam.Core.PreBuiltTool
        {
            var graph = Bam.Core.Graph.Instance;
            foreach (var toolData in GetToolsFromMetaData<AttributeType>())
            {
                var tool = graph.MakeModuleOfType<ToolType>(toolData.Item1);
                var bits = toolData.Item2.BitDepth;
                if (!collection.ContainsKey(bits))
                {
                    collection[bits] = new Bam.Core.Array<ToolType>(tool);
                }
                else
                {
                    collection[bits].AddUnique(tool);
                }
            }
        }

        static DefaultToolchain()
        {
            DefaultToolChain = Bam.Core.CommandLineProcessor.Evaluate(new DefaultToolchainCommand());
            FindTools<RegisterCCompilerAttribute, CompilerTool>(C_Compilers);
            FindTools<RegisterCxxCompilerAttribute, CompilerTool>(Cxx_Compilers);
            FindTools<RegisterArchiverAttribute, LibrarianTool>(Archivers);
            FindTools<RegisterCLinkerAttribute, LinkerTool>(C_Linkers);
            FindTools<RegisterCxxLinkerAttribute, LinkerTool>(Cxx_Linkers);
            FindTools<RegisterObjectiveCCompilerAttribute, CompilerTool>(ObjectiveC_Compilers);
            FindTools<RegisterObjectiveCxxCompilerAttribute, CompilerTool>(ObjectiveCxx_Compilers);
        }

        private static ToolType
        GetTool<ToolType>(
            System.Collections.Generic.Dictionary<EBit, Bam.Core.Array<ToolType>> collection,
            EBit bitDepth,
            string toolDescription)
            where ToolType : Bam.Core.PreBuiltTool
        {
            if (!collection.ContainsKey(bitDepth) || 0 == collection[bitDepth].Count)
            {
                throw new Bam.Core.Exception("No default {0}s for this platform in {1}-bits", toolDescription, (int)bitDepth);
            }
            var candidates = collection[bitDepth];
            if (candidates.Count > 1)
            {
                if (null != DefaultToolChain)
                {
                    foreach (var tool in candidates)
                    {
                        var attr = tool.GetType().GetCustomAttributes(false);
                        if ((attr[0] as ToolRegistrationAttribute).ToolsetName == DefaultToolChain)
                        {
                            return tool;
                        }
                    }
                }

                var tooManyInstance = new System.Text.StringBuilder();
                tooManyInstance.AppendFormat("There are {0} {1}s available for this platform in {2}-bits:", candidates.Count, toolDescription, (int)bitDepth);
                tooManyInstance.AppendLine();
                foreach (var tool in candidates)
                {
                    tooManyInstance.AppendFormat("\t{0}", tool.GetType().ToString());
                    tooManyInstance.AppendLine();
                }
                throw new Bam.Core.Exception(tooManyInstance.ToString());
            }
            var toolToUse = candidates[0];
            var toolToolSet = (toolToUse.GetType().GetCustomAttributes(false)[0] as ToolRegistrationAttribute).ToolsetName;
            if ((null != DefaultToolChain) && (toolToolSet != DefaultToolChain))
            {
                throw new Bam.Core.Exception("{0}-bit {1} identified is from toolchain {2}, not from {3} as requested", (int)bitDepth, toolDescription, toolToolSet, DefaultToolChain);
            }
            return toolToUse;
        }

        public static CompilerTool C_Compiler(EBit bitDepth)
        {
            return GetTool<CompilerTool>(C_Compilers, bitDepth, "C compiler");
        }

        public static CompilerTool Cxx_Compiler(EBit bitDepth)
        {
            return GetTool<CompilerTool>(Cxx_Compilers, bitDepth, "C++ compiler");
        }

        public static LibrarianTool Librarian(EBit bitDepth)
        {
            return GetTool<LibrarianTool>(Archivers, bitDepth, "librarian");
        }

        public static LinkerTool C_Linker(EBit bitDepth)
        {
            return GetTool<LinkerTool>(C_Linkers, bitDepth, "C linker");
        }

        public static LinkerTool Cxx_Linker(EBit bitDepth)
        {
            return GetTool<LinkerTool>(Cxx_Linkers, bitDepth, "C++ linker");
        }

        public static CompilerTool ObjectiveC_Compiler(EBit bitDepth)
        {
            return GetTool<CompilerTool>(ObjectiveC_Compilers, bitDepth, "Objective C compiler");
        }

        public static CompilerTool ObjectiveCxx_Compiler(EBit bitDepth)
        {
            return GetTool<CompilerTool>(ObjectiveCxx_Compilers, bitDepth, "Objective C++ compiler");
        }
    }

    public class SourceFile :
        Bam.Core.Module,
        Bam.Core.IInputPath
    {
        static public Bam.Core.FileKey Key = Bam.Core.FileKey.Generate("Source File");

        public override void
        Evaluate()
        {
            this.ReasonToExecute = null;
            // TODO: could do a hash check of the contents?
        }

        protected override void
        ExecuteInternal(
            Bam.Core.ExecutionContext context)
        {
            // TODO: exception to this is generated source, but there ought to be an override for that
        }

        protected override void GetExecutionPolicy(string mode)
        {
            // there is no execution policy
        }

        public virtual Bam.Core.TokenizedString InputPath
        {
            get
            {
                return this.GeneratedPaths[Key];
            }
            set
            {
                this.GeneratedPaths[Key] = value;
            }
        }
    }

    public sealed class HeaderFile :
        Bam.Core.Module,
        Bam.Core.IInputPath,
        Bam.Core.IChildModule
    {
        static public Bam.Core.FileKey Key = Bam.Core.FileKey.Generate("Header File");

        public override void
        Evaluate()
        {
            this.ReasonToExecute = null;
            // TODO: could do a hash check of the contents?
        }

        protected override void
        ExecuteInternal(
            Bam.Core.ExecutionContext context)
        {
            // TODO: exception to this is generated source, but there ought to be an override for that
        }

        protected override void GetExecutionPolicy(string mode)
        {
            // there is no execution policy
        }

        public Bam.Core.TokenizedString InputPath
        {
            get
            {
                return this.GeneratedPaths[Key];
            }
            set
            {
                this.GeneratedPaths[Key] = value;
            }
        }

        Bam.Core.Module Bam.Core.IChildModule.Parent
        {
            get;
            set;
        }
    }

    public class ObjectFile :
        CModule,
        Bam.Core.IChildModule,
        Bam.Core.IInputPath
    {
        private Bam.Core.Module Parent = null;
        private ICompilationPolicy Policy = null;
        public SourceFile Source = null;

        static public Bam.Core.FileKey Key = Bam.Core.FileKey.Generate("Compiled Object File");

        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);
            this.Compiler = DefaultToolchain.C_Compiler(this.BitDepth);
            this.RegisterGeneratedFile(Key, Bam.Core.TokenizedString.Create("$(packagebuilddir)/$(moduleoutputdir)/@basename($(inputpath))$(objext)", this));
        }

        public Bam.Core.TokenizedString InputPath
        {
            get
            {
                return this.Source.InputPath;
            }
            set
            {
                if (null == this.Source)
                {
                    // this cannot be a referenced module, since there will be more than one object
                    // of this type (generally)
                    // but this does mean there may be many instances of this 'type' of module
                    // and for multi-configuration builds there may be many instances of the same path
                    this.Source = Bam.Core.Module.Create<SourceFile>();
                    this.DependsOn(this.Source);
                }
                this.Source.InputPath = value;
                this.Macros.Add("inputpath", value);
            }
        }

        Bam.Core.Module Bam.Core.IChildModule.Parent
        {
            get
            {
                return this.Parent;
            }
            set
            {
                this.Parent = value;
            }
        }

        public CompilerTool Compiler
        {
            get
            {
                return this.Tool as CompilerTool;
            }
            set
            {
                this.Tool = value;
            }
        }

        protected override void
        ExecuteInternal(
            Bam.Core.ExecutionContext context)
        {
            var sourceFile = this.Source;
            var objectFile = this.GeneratedPaths[Key];
            this.Policy.Compile(this, context, objectFile, sourceFile);
        }

        protected override void GetExecutionPolicy(string mode)
        {
            var className = "C." + mode + "Compilation";
            this.Policy = Bam.Core.ExecutionPolicyUtilities<ICompilationPolicy>.Create(className);
        }

        public override void
        Evaluate()
        {
            this.ReasonToExecute = null;
            var graph = Bam.Core.Graph.Instance;
            var factory = graph.MetaData as System.Threading.Tasks.TaskFactory;
            this.EvaluationTask = factory.StartNew(() =>
            {
                var objectFilePath = this.GeneratedPaths[Key].Parse();
                if (!System.IO.File.Exists(objectFilePath))
                {
                    this.ReasonToExecute = Bam.Core.ExecuteReasoning.FileDoesNotExist(this.GeneratedPaths[Key]);
                    return;
                }
                var objectFileWriteTime = System.IO.File.GetLastWriteTime(objectFilePath);

                var sourcePath = this.InputPath.Parse();
                var sourceWriteTime = System.IO.File.GetLastWriteTime(sourcePath);
                if (sourceWriteTime > objectFileWriteTime)
                {
                    this.ReasonToExecute = Bam.Core.ExecuteReasoning.InputFileNewer(this.GeneratedPaths[Key], this.InputPath);
                    return;
                }

                var includeSearchPaths = (this.Settings as C.ICommonCompilerOptions).IncludePaths;

                var filesToSearch = new System.Collections.Generic.Queue<string>();
                filesToSearch.Enqueue(sourcePath);

                var headerPathsFound = new Bam.Core.StringArray();
                while (filesToSearch.Count > 0)
                {
                    var fileToSearch = filesToSearch.Dequeue();

                    string fileContents = null;
                    using (System.IO.TextReader reader = new System.IO.StreamReader(fileToSearch))
                    {
                        fileContents = reader.ReadToEnd();
                    }

                    var matches = System.Text.RegularExpressions.Regex.Matches(
                        fileContents,
                        "^\\s*#include \"(.*)\"",
                        System.Text.RegularExpressions.RegexOptions.Multiline);
                    if (0 == matches.Count)
                    {
                        // no #includes
                        return;
                    }

                    foreach (System.Text.RegularExpressions.Match match in matches)
                    {
                        bool exists = false;
                        // search for the file on the include paths the compiler uses
                        foreach (var includePath in includeSearchPaths)
                        {
                            try
                            {
                                var potentialPath = System.IO.Path.Combine(includePath.Parse(), match.Groups[1].Value);
                                if (!System.IO.File.Exists(potentialPath))
                                {
                                    continue;
                                }
                                potentialPath = System.IO.Path.GetFullPath(potentialPath);
                                var headerWriteTime = System.IO.File.GetLastWriteTime(potentialPath);

                                // early out - header is newer than generated object file
                                if (headerWriteTime > objectFileWriteTime)
                                {
                                    this.ReasonToExecute = Bam.Core.ExecuteReasoning.InputFileNewer(this.GeneratedPaths[Key], Bam.Core.TokenizedString.Create(potentialPath, null, verbatim:true));
                                    return;
                                }

                                if (!headerPathsFound.Contains(potentialPath))
                                {
                                    headerPathsFound.Add(potentialPath);
                                    filesToSearch.Enqueue(potentialPath);
                                }

                                exists = true;
                                break;
                            }
                            catch (System.Exception ex)
                            {
                                Bam.Core.Log.MessageAll("IncludeDependency Exception: Cannot locate '{0}' on '{1}' due to {2}", match.Groups[1].Value, includePath, ex.Message);
                            }
                        }

                        if (!exists)
                        {
#if false
                                Bam.Core.Log.DebugMessage("***** Could not locate '{0}' on any include search path, included from {1}:\n{2}",
                                                          match.Groups[1],
                                                          fileToSearch,
                                                          entry.includePaths.ToString('\n'));
#endif
                        }
                    }
                }

                return;
            });
        }
    }
}
