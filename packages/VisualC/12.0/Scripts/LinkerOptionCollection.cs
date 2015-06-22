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
using C.V2.DefaultSettings;
namespace VisualC
{
    public static partial class NativeImplementation
    {
        public static void
        Convert(
            this C.V2.ICommonLinkerOptions options,
            Bam.Core.V2.Module module,
            Bam.Core.StringArray commandLine)
        {
            //var applicationFile = module as C.V2.ConsoleApplication;
            switch (options.OutputType)
            {
                case C.ELinkerOutput.Executable:
                    commandLine.Add(System.String.Format("-OUT:{0}", module.GeneratedPaths[C.V2.ConsoleApplication.Key].ToString()));
                    break;

                case C.ELinkerOutput.DynamicLibrary:
                    commandLine.Add("-DLL");
                    commandLine.Add(System.String.Format("-OUT:{0}", module.GeneratedPaths[C.V2.ConsoleApplication.Key].ToString()));
                    break;
            }
            foreach (var path in options.LibraryPaths)
            {
                var format = path.ContainsSpace ? "-LIBPATH:\"{0}\"" : "-LIBPATH:{0}";
                commandLine.Add(System.String.Format(format, path.ToString()));
            }
        }

        public static void
        Convert(
            this V2.ICommonLinkerOptions options,
            Bam.Core.V2.Module module,
            Bam.Core.StringArray commandLine)
        {
        }
    }

namespace V2
{
    public interface ICommonLinkerOptions
    {
    }

    public class LinkerSettings :
        Bam.Core.V2.Settings,
        C.V2.ICommonLinkerOptions,
        ICommonLinkerOptions,
        CommandLineProcessor.V2.IConvertToCommandLine
    {
        public LinkerSettings(Bam.Core.V2.Module module)
        {
            (this as C.V2.ICommonLinkerOptions).Defaults(module);
        }

        C.ELinkerOutput C.V2.ICommonLinkerOptions.OutputType
        {
            get;
            set;
        }

        Bam.Core.Array<Bam.Core.V2.TokenizedString> C.V2.ICommonLinkerOptions.LibraryPaths
        {
            get;
            set;
        }

        void
        CommandLineProcessor.V2.IConvertToCommandLine.Convert(
            Bam.Core.V2.Module module,
            Bam.Core.StringArray commandLine)
        {
            (this as C.V2.ICommonLinkerOptions).Convert(module, commandLine);
            (this as ICommonLinkerOptions).Convert(module, commandLine);
        }
    }

    [C.V2.RegisterLinker("VisualC", Bam.Core.EPlatform.Windows)]
    public sealed class Linker :
        C.V2.LinkerTool
    {
        public Linker()
        {
            this.Macros.Add("InstallPath", Configure.InstallPath);
            this.Macros.Add("LinkerPath", Bam.Core.V2.TokenizedString.Create(@"$(InstallPath)\VC\bin\link.exe", this));
            this.Macros.Add("exeext", ".exe");
            this.Macros.Add("dynamicprefix", string.Empty);
            this.Macros.Add("dynamicext", ".dll");
            this.Macros.Add("libprefix", string.Empty);
            this.Macros.Add("libext", ".lib");

            this.PublicPatch(settings =>
            {
                var linking = settings as C.V2.ICommonLinkerOptions;
                linking.LibraryPaths.Add(Bam.Core.V2.TokenizedString.Create(@"$(InstallPath)\VC\lib\amd64", this));
            });
        }

        public override Bam.Core.V2.Settings CreateDefaultSettings<T>(T module)
        {
            var settings = new LinkerSettings(module);
            return settings;
        }

        public override Bam.Core.V2.TokenizedString Executable
        {
            get
            {
                return this.Macros["LinkerPath"];
            }
        }

        public override bool UseLPrefixLibraryPaths
        {
            get
            {
                return false;
            }
        }
    }
}

    public sealed partial class LinkerOptionCollection :
        VisualCCommon.LinkerOptionCollection
    {
        public
        LinkerOptionCollection(
            Bam.Core.DependencyNode node) : base(node)
        {}
    }
}
