// Automatically generated by Opus v0.00
namespace MixedTest
{
    // Define module classes here
    class CSharpTest :
        CSharp.Executable
    {
        public
        CSharpTest()
        {
            var sourceDir = this.PackageLocation.SubDirectory("source");
            this.source = Opus.Core.FileLocation.Get(sourceDir, "main.cs");
        }

        [Opus.Core.SourceFiles]
        Opus.Core.Location source;
    }

    class CApp :
        C.Application
    {
        public
        CApp()
        {
            var sourceDir = this.PackageLocation.SubDirectory("source");
            this.source.Include(sourceDir, "main.c");
        }

        [Opus.Core.SourceFiles]
        C.ObjectFile source = new C.ObjectFile();

        [Opus.Core.RequiredModules]
        Opus.Core.TypeArray requiredModules = new Opus.Core.TypeArray(typeof(CSharpTest));

        [Opus.Core.DependentModules(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.TypeArray dependentModules = new Opus.Core.TypeArray(typeof(WindowsSDK.WindowsSDK));

        [C.RequiredLibraries(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.StringArray libraries = new Opus.Core.StringArray("KERNEL32.lib");
    }
}
