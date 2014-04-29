// <copyright file="LinkerOptionCollection.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>GccCommon package</summary>
// <author>Mark Final</author>
namespace GccCommon
{
    public partial class LinkerOptionCollection : C.LinkerOptionCollection, C.ILinkerOptions, C.ILinkerOptionsOSX, GccCommon.ILinkerOptions
    {
        protected override void SetDefaultOptionValues(Opus.Core.DependencyNode node)
        {
            base.SetDefaultOptionValues(node);

            Opus.Core.Target target = node.Target;

            ILinkerOptions linkerOptions = this as ILinkerOptions;
            linkerOptions.SixtyFourBit = Opus.Core.OSUtilities.Is64Bit(target);

            (this as C.ILinkerOptions).DoNotAutoIncludeStandardLibraries = false; // TODO: fix this - requires a bunch of stuff to be added to the command line

            linkerOptions.CanUseOrigin = false;
            linkerOptions.AllowUndefinedSymbols = (node.Module is C.DynamicLibrary);
            linkerOptions.RPath = new Opus.Core.StringArray();

            if (null != node.Children)
            {
                // we use gcc as the linker - if there is C++ code included, link against libstdc++
                foreach (Opus.Core.DependencyNode child in node.Children)
                {
                    if (child.Module is C.Cxx.ObjectFile || child.Module is C.Cxx.ObjectFileCollection |
                        child.Module is C.ObjCxx.ObjectFile || child.Module is C.ObjCxx.ObjectFileCollection)
                    {
                        (this as C.ILinkerOptions).Libraries.Add("-lstdc++");
                        break;
                    }
                }

                // we use gcc as the link - if there is ObjectiveC code included, link against -lobjc
                foreach (Opus.Core.DependencyNode child in node.Children)
                {
                    if (child.Module is C.ObjC.ObjectFile || child.Module is C.ObjC.ObjectFileCollection |
                        child.Module is C.ObjCxx.ObjectFile || child.Module is C.ObjCxx.ObjectFileCollection)
                    {
                        (this as C.ILinkerOptions).Libraries.Add("-lobjc");
                        break;
                    }
                }
            }

            /*
             This is an example link line using gcc with -v

Linker Error: ' C:/MinGW/bin/../libexec/gcc/mingw32/3.4.5/collect2.exe -Bdynamic -o d:\build\Test2-dev\Application\win32-debug-mingw\Application.exe C:/MinGW/bin/../lib/gcc/mingw32/3.4.5/../../../crt2.o C:/MinGW/bin/../lib/gcc/mingw32/3.4.5/crtbegin.o -LC:/MinGW/bin/../lib/gcc/mingw32/3.4.5 -LC:/MinGW/bin/../lib/gcc -LC:/MinGW/bin/../lib/gcc/mingw32/3.4.5/../../../../mingw32/lib -LC:/MinGW/bin/../lib/gcc/mingw32/3.4.5/../../.. --subsystem console d:\build\Test2-dev\Application\win32-debug-mingw\application.o d:\build\Test2-dev\Library\win32-debug-mingw\libLibrary.a d:\build\Test3-dev\Library2\win32-debug-mingw\libLibrary2.a -lmingw32 -lgcc -lmoldname -lmingwex -lmsvcrt -luser32 -lkernel32 -ladvapi32 -lshell32 -lmingw32 -lgcc -lmoldname -lmingwex -lmsvcrt C:/MinGW/bin/../lib/gcc/mingw32/3.4.5/crtend.o'
             */
        }

        public LinkerOptionCollection(Opus.Core.DependencyNode node)
            : base(node)
        {
        }

        public override Opus.Core.DirectoryCollection DirectoriesToCreate()
        {
            Opus.Core.DirectoryCollection directoriesToCreate = new Opus.Core.DirectoryCollection();

            string outputPathName = this.OutputFilePath;
            if (null != outputPathName)
            {
                directoriesToCreate.Add(System.IO.Path.GetDirectoryName(outputPathName));
            }

            string libraryPathName = this.StaticImportLibraryFilePath;
            if (null != libraryPathName)
            {
                directoriesToCreate.Add(System.IO.Path.GetDirectoryName(libraryPathName));
            }

            return directoriesToCreate;
        }
    }
}
