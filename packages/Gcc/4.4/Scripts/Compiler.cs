// <copyright file="Compiler.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>Gcc package</summary>
// <author>Mark Final</author>
namespace Gcc
{
    // Not sealed since the C++ compiler inherits from it
    public class CCompiler : GccCommon.CCompiler
    {
        private Opus.Core.StringArray requiredEnvironmentVariables = new Opus.Core.StringArray();
        private Opus.Core.StringArray includeFolders = new Opus.Core.StringArray();
        private string binPath;

        public CCompiler(Opus.Core.Target target)
        {
            if (!Opus.Core.OSUtilities.IsUnix(target.Platform))
            {
                throw new Opus.Core.Exception("Gcc compiler is only supported under unix32 and unix64 platforms", false);
            }

            Toolchain toolChainInstance = C.ToolchainFactory.GetTargetInstance(target) as Toolchain;
            this.binPath = toolChainInstance.BinPath(target);

            this.includeFolders.Add("/usr/include");
            string gccIncludeFolder;
            string gccIncludeFixedFolder;
            if (Opus.Core.OSUtilities.Is64BitHosting)
            {
                gccIncludeFolder = "/usr/lib/gcc/x86_64-linux-gnu/4.4/include";
                gccIncludeFixedFolder = "/usr/lib/gcc/x86_64-linux-gnu/4.4/include-fixed";
            }
            else
            {
                gccIncludeFolder = "/usr/lib/gcc/i486-linux-gnu/4.4/include";
                gccIncludeFixedFolder = "/usr/lib/gcc/i486-linux-gnu/4.4/include-fixed";
            }
            
            if (!System.IO.Directory.Exists(gccIncludeFolder))
            {
                throw new Opus.Core.Exception(System.String.Format("Gcc include folder '{0}' does not exist", gccIncludeFolder), false);
            }
            this.includeFolders.Add(gccIncludeFolder);
            
            if (!System.IO.Directory.Exists(gccIncludeFolder))
            {
                throw new Opus.Core.Exception(System.String.Format("Gcc include folder '{0}' does not exist", gccIncludeFixedFolder), false);
            }
            this.includeFolders.Add(gccIncludeFixedFolder);
        }

        public override string Executable(Opus.Core.Target target)
        {
            return System.IO.Path.Combine(this.binPath, "gcc-4.4");
        }

        public override string ExecutableCPlusPlus(Opus.Core.Target target)
        {
            return System.IO.Path.Combine(this.binPath, "g++-4.4");
        }

        public override Opus.Core.StringArray RequiredEnvironmentVariables
        {
            get
            {
                return this.requiredEnvironmentVariables;
            }
        }

        public override Opus.Core.StringArray EnvironmentPaths(Opus.Core.Target target)
        {
            Toolchain toolChainInstance = C.ToolchainFactory.GetTargetInstance(target) as Toolchain;
            return toolChainInstance.Environment;
        }

        public override Opus.Core.StringArray IncludeDirectoryPaths(Opus.Core.Target target)
        {
            return this.includeFolders;
        }
    }
}