// Automatically generated by Opus v0.00
namespace CodeGenTest
{
    // Define module classes here
    class TestAppGeneratedSource :
        CodeGenModule
    {
        public
        TestAppGeneratedSource()
        {
            this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(TestAppGeneratedSource_UpdateOptions);
        }

        void
        TestAppGeneratedSource_UpdateOptions(
            Opus.Core.IModule module,
            Opus.Core.Target target)
        {
            CodeGenOptionCollection options = module.Options as CodeGenOptionCollection;
        }
    }

    class TestApp :
        C.Application
    {
        public
        TestApp()
        {
            this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(TestApp_UpdateOptions);
        }

        void
        TestApp_UpdateOptions(
            Opus.Core.IModule module,
            Opus.Core.Target target)
        {
            C.ILinkerOptions options = module.Options as C.ILinkerOptions;
            options.DoNotAutoIncludeStandardLibraries = false;
        }

        class SourceFiles :
            C.ObjectFileCollection
        {
            public
            SourceFiles()
            {
                var sourceDir = this.PackageLocation.SubDirectory("source");
                var testAppDir = sourceDir.SubDirectory("testapp");
                this.Include(testAppDir, "main.c");
            }

            [Opus.Core.DependentModules]
            Opus.Core.TypeArray vcDependencies = new Opus.Core.TypeArray(typeof(TestAppGeneratedSource));
        }

        [Opus.Core.SourceFiles]
        SourceFiles source = new SourceFiles();

        [Opus.Core.DependentModules(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.TypeArray vcDependents = new Opus.Core.TypeArray(typeof(WindowsSDK.WindowsSDK));
    }
}
