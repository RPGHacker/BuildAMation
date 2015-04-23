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
namespace C
{
namespace V2
{
    public interface ICompilationPolicy
    {
        void
        Compile(
            string objectFilePath,
            string sourceFilePath);
    }

    public interface ILibrarianPolicy
    {
        void
        Archive(
            string libraryPath,
            System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.V2.Module> inputs);
    }

    public interface ILinkerPolicy
    {
        void
        Link(
            string executablePath,
            System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.V2.Module> objectFiles,
            System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.V2.Module> libraries,
            System.Collections.ObjectModel.ReadOnlyCollection<Bam.Core.V2.Module> frameworks);
    }

    public class ObjectFile :
        Bam.Core.V2.Module,
        Bam.Core.V2.IChildModule,
        Bam.Core.V2.IInputPath
    {
        private Bam.Core.V2.TokenizedString Path = null;
        private Bam.Core.V2.Module Parent = null;
        private static ICompilationPolicy SharedPolicy = null;
        private ICompilationPolicy Policy = null;

        static public Bam.Core.V2.FileKey Key = Bam.Core.V2.FileKey.Generate("Compiled Object File");

        protected override void Init()
        {
            this.RegisterGeneratedFile(Key, new Bam.Core.V2.TokenizedString("$(buildroot)/$(config)/$basename($(inputpath)).obj", null));
        }

        public Bam.Core.V2.TokenizedString InputPath
        {
            get
            {
                return this.Path;
            }
            set
            {
                this.Macros.Add("inputpath", value);
                this.Path = value;
            }
        }

        Bam.Core.V2.Module Bam.Core.V2.IChildModule.Parent
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

        protected override void ExecuteInternal()
        {
            var sourceFile = this.InputPath.ToString();
            var objectFile = this.GeneratedPaths[Key].ToString();
            this.Policy.Compile(objectFile, sourceFile);
        }

        protected override void GetExecutionPolicy(string mode)
        {
            if (null == SharedPolicy)
            {
                System.Func<ICompilationPolicy> getPolicy = () =>
                {
                    var className = "C.V2." + mode + "Compilation";
                    var type = System.Type.GetType(className);
                    if (null == type)
                    {
                        throw new Bam.Core.Exception("Unable to locate class '{0}'", className);
                    }
                    var policy = System.Activator.CreateInstance(type) as ICompilationPolicy;
                    if (null == policy)
                    {
                        throw new Bam.Core.Exception("Unable to create instance of class '{0}'", className);
                    }
                    return policy;
                };
                SharedPolicy = getPolicy();
            }
            this.Policy = SharedPolicy;
        }
    }
}
    /// <summary>
    /// C object file
    /// </summary>
    [Bam.Core.ModuleToolAssignment(typeof(ICompilerTool))]
    public class ObjectFile :
        Bam.Core.BaseModule
    {
        private static readonly Bam.Core.LocationKey SourceFile = new Bam.Core.LocationKey("SourceFile", Bam.Core.ScaffoldLocation.ETypeHint.File);
        public static readonly Bam.Core.LocationKey OutputDir = new Bam.Core.LocationKey("ObjectFileDir", Bam.Core.ScaffoldLocation.ETypeHint.Directory);
        public static readonly Bam.Core.LocationKey OutputFile = new Bam.Core.LocationKey("ObjectFile", Bam.Core.ScaffoldLocation.ETypeHint.File);

        public Bam.Core.Location SourceFileLocation
        {
            get
            {
                return this.Locations[SourceFile];
            }

            set
            {
                this.Locations[SourceFile] = value;
            }
        }

        public void
        Include(
            Bam.Core.Location baseLocation,
            string pattern)
        {
            this.SourceFileLocation = new Bam.Core.ScaffoldLocation(baseLocation, pattern, Bam.Core.ScaffoldLocation.ETypeHint.File);
        }
    }
}
