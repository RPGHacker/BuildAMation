#region License
// Copyright 2010-2015 Mark Final
//
// This file is part of BuildAMation.
//
// BuildAMation is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// BuildAMation is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with BuildAMation.  If not, see <http://www.gnu.org/licenses/>.
#endregion // License
using Bam.Core.V2; // for EPlatform.PlatformExtensions
namespace C
{
namespace V2
{
    public class DynamicLibrary :
        ConsoleApplication
    {
        static public Bam.Core.V2.FileKey ImportLibraryKey = Bam.Core.V2.FileKey.Generate("Import Library File");

        protected override void
        Init(
            Bam.Core.V2.Module parent)
        {
            base.Init(parent);
            this.GeneratedPaths[Key] = Bam.Core.V2.TokenizedString.Create("$(pkgbuilddir)/$(moduleoutputdir)/$(dynamicprefix)$(OutputName)$(dynamicext)", this);

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                this.RegisterGeneratedFile(ImportLibraryKey, Bam.Core.V2.TokenizedString.Create("$(pkgbuilddir)/$(moduleoutputdir)/$(libprefix)$(OutputName)$(libext)", this));
            }

            this.PrivatePatch(settings =>
            {
                var linker = settings as C.V2.ICommonLinkerOptions;
                if (null != linker)
                {
                    linker.OutputType = ELinkerOutput.DynamicLibrary;
                }
            });
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.V2.Module> Source
        {
            get
            {
                return new System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.V2.Module>(this.sourceModules.ToArray());
            }
        }

        public override CObjectFileCollection
        CreateCSourceContainer()
        {
            var collection = base.CreateCSourceContainer();
            collection.PrivatePatch(settings =>
            {
                var compiler = settings as C.V2.ICommonCompilerOptions;
                compiler.PreprocessorDefines.Add("D_BAM_DYNAMICLIBRARY_BUILD");
                (collection.Tool as C.V2.CompilerTool).CompileAsShared(settings);
            });
            return collection;
        }

        public override Cxx.V2.ObjectFileCollection
        CreateCxxSourceContainer(string wildcardPath = null)
        {
            var collection = base.CreateCxxSourceContainer(wildcardPath);
            collection.PrivatePatch(settings =>
            {
                var compiler = settings as C.V2.ICommonCompilerOptions;
                compiler.PreprocessorDefines.Add("D_BAM_DYNAMICLIBRARY_BUILD");
                (collection.Tool as C.V2.CompilerTool).CompileAsShared(settings);
            });
            return collection;
        }
    }
}
namespace Cxx
{
namespace V2
{
    public class DynamicLibrary :
        C.V2.DynamicLibrary
    {
        protected override void
        Init(
            Bam.Core.V2.Module parent)
        {
            base.Init(parent);
            this.Linker = C.V2.DefaultToolchain.Cxx_Linker(this.BitDepth);
        }
    }
}
}
    /// <summary>
    /// C/C++ dynamic library
    /// </summary>
    public partial class DynamicLibrary :
        Application,
        Bam.Core.IPostActionModules
    {
        public static readonly Bam.Core.LocationKey ImportLibraryFile = new Bam.Core.LocationKey("ImportLibraryFile", Bam.Core.ScaffoldLocation.ETypeHint.File);
        public static readonly Bam.Core.LocationKey ImportLibraryDir = new Bam.Core.LocationKey("ImportLibraryDirectory", Bam.Core.ScaffoldLocation.ETypeHint.Directory);

        protected
        DynamicLibrary()
        {
            this.PostActionModuleTypes = new Bam.Core.TypeArray();
            if (Bam.Core.OSUtilities.IsUnixHosting)
            {
                this.PostActionModuleTypes.Add(typeof(PosixSharedLibrarySymlinks));
            }
        }

        [LocalCompilerOptionsDelegate]
        protected static void
        DynamicLibrarySetDLLBuildPreprocessorDefine(
            Bam.Core.IModule module,
            Bam.Core.Target target)
        {
            var compilerOptions = module.Options as ICCompilerOptions;
            compilerOptions.Defines.Add("D_BAM_DYNAMICLIBRARY_BUILD");
        }

        [LocalLinkerOptionsDelegate]
        protected static void
        DynamicLibraryEnableDLL(
            Bam.Core.IModule module,
            Bam.Core.Target target)
        {
            var linkerOptions = module.Options as ILinkerOptions;
            linkerOptions.OutputType = ELinkerOutput.DynamicLibrary;

            if (module.Options is ILinkerOptionsOSX && target.HasPlatform(Bam.Core.EPlatform.OSX32))
            {
                // only required for 32-bit builds
                (module.Options as ILinkerOptionsOSX).SuppressReadOnlyRelocations = true;
            }
        }

        public Bam.Core.TypeArray PostActionModuleTypes
        {
            get;
            private set;
        }

        #region IPostActionModules Members

        Bam.Core.TypeArray
        Bam.Core.IPostActionModules.GetPostActionModuleTypes(
            Bam.Core.BaseTarget target)
        {
            return this.PostActionModuleTypes;
        }

        #endregion
    }
}
