#region License
// Copyright 2010-2014 Mark Final
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
#endregion
namespace QMakeBuilder
{
    public sealed partial class QMakeBuilder :
        Bam.Core.IBuilderPreExecute
    {
        #region IBuilderPreExecute Members

        void
        Bam.Core.IBuilderPreExecute.PreExecute()
        {
            Bam.Core.Log.DebugMessage("PreExecute for QMakeBuilder");

            var mainPackage = Bam.Core.State.PackageInfo[0];
            var priFileName = "EmptyConfig.pri";
            var rootDirectory = mainPackage.BuildDirectory;
            var priFilePath = System.IO.Path.Combine(rootDirectory, priFileName);

            System.IO.Directory.CreateDirectory(rootDirectory);
            using (System.IO.TextWriter proFileWriter = new System.IO.StreamWriter(priFilePath))
            {
                proFileWriter.WriteLine("# -- Generated by Opus --");
                proFileWriter.WriteLine("TEMPLATE=");
                proFileWriter.WriteLine("# CONFIG cannot be completely emptied");
                proFileWriter.WriteLine("CONFIG-=qt");
                proFileWriter.WriteLine("CONFIG-=lex");
                proFileWriter.WriteLine("CONFIG-=yacc");
                proFileWriter.WriteLine("CONFIG-=warn_on");
                proFileWriter.WriteLine("CONFIG-=uic");
                proFileWriter.WriteLine("CONFIG-=resources");
                proFileWriter.WriteLine("CONFIG-=rtti_off");
                proFileWriter.WriteLine("CONFIG-=exceptions_off");
                proFileWriter.WriteLine("CONFIG-=stl_off");
                proFileWriter.WriteLine("CONFIG-=incremental_off");
                proFileWriter.WriteLine("CONFIG-=thread_off");
                proFileWriter.WriteLine("CONFIG-=windows");
                proFileWriter.WriteLine("CONFIG-=warn_on");
                proFileWriter.WriteLine("CONFIG-=incremental");
                proFileWriter.WriteLine("CONFIG-=flat");
                proFileWriter.WriteLine("CONFIG-=link_prl");
                proFileWriter.WriteLine("CONFIG-=precompile_header");
                proFileWriter.WriteLine("CONFIG-=autogen_precompile_source");
                proFileWriter.WriteLine("CONFIG-=copy_dir_files");
                proFileWriter.WriteLine("CONFIG-=embed_manifest_dll");
                proFileWriter.WriteLine("CONFIG-=embed_manifest_exe");
                proFileWriter.WriteLine("CONFIG-=shared");
                proFileWriter.WriteLine("CONFIG-=stl");
                proFileWriter.WriteLine("CONFIG-=exceptions");
                proFileWriter.WriteLine("CONFIG-=rtti");
                proFileWriter.WriteLine("CONFIG-=mmx");
                proFileWriter.WriteLine("CONFIG-=3dnow");
                proFileWriter.WriteLine("CONFIG-=sse");
                proFileWriter.WriteLine("CONFIG-=sse2");
                proFileWriter.WriteLine("CONFIG-=incredibuild_xge");
                proFileWriter.WriteLine("CONFIG-=console");
                proFileWriter.WriteLine("QT=");
                proFileWriter.WriteLine("DESTDIR=");
                proFileWriter.WriteLine("DEFINES=");
                proFileWriter.WriteLine("RC_FILE=");
                proFileWriter.WriteLine("QMAKE_COMPILER_DEFINES=");
                proFileWriter.WriteLine("QMAKE_CFLAGS=");
                proFileWriter.WriteLine("QMAKE_CFLAGS_DEBUG=");
                proFileWriter.WriteLine("QMAKE_CFLAGS_RELEASE=");
                proFileWriter.WriteLine("QMAKE_CXXFLAGS=");
                proFileWriter.WriteLine("QMAKE_CXXFLAGS_DEBUG=");
                proFileWriter.WriteLine("QMAKE_CXXFLAGS_RELEASE=");
                proFileWriter.WriteLine("QMAKE_LFLAGS=");
                proFileWriter.WriteLine("QMAKE_LFLAGS_DEBUG=");
                proFileWriter.WriteLine("QMAKE_LFLAGS_RELEASE=");
                proFileWriter.WriteLine("QMAKE_LN_SHLIB=");
                proFileWriter.WriteLine("QMAKE_MACOSX_DEPLOYMENT_TARGET=10.8");
            }

            this.EmptyConfigPriPath = priFilePath;
        }

        #endregion
    }
}
