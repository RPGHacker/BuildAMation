// <copyright file="CCompiler.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>Clang package</summary>
// <author>Mark Final</author>
namespace Clang
{
    public sealed class CCompiler : C.Compiler, C.ICompiler, Opus.Core.ITool
    {
        // TODO: this needs to be shared
        private static string InstallPath
        {
            get;
            set;
        }

        static CCompiler()
        {
            if (Opus.Core.OSUtilities.IsWindowsHosting)
            {
                InstallPath = @"D:\dev\Thirdparty\Clang\3.1\build\bin\Release";
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }

        #region ITool Members

        string Opus.Core.ITool.Executable(Opus.Core.Target target)
        {
            // TODO: can we have this extension somewhere central?
            return System.IO.Path.Combine(InstallPath, "clang.exe");
        }

        #endregion

        #region ICompiler Members

        Opus.Core.StringArray C.ICompiler.IncludeDirectoryPaths(Opus.Core.Target target)
        {
            return new Opus.Core.StringArray();
        }

        Opus.Core.StringArray C.ICompiler.IncludePathCompilerSwitches
        {
            get
            {
                Opus.Core.StringArray switches = new Opus.Core.StringArray();
                switches.Add("-I");
                return switches;
            }
        }

        #endregion
    }
}
