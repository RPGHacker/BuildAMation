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
namespace ClangCommon
{
    public static partial class CommandLineLinkerImplementation
    {
        public static void
        Convert(
            this C.ICommonLinkerSettings settings,
            Bam.Core.Module module,
            Bam.Core.StringArray commandLine)
        {
            switch (settings.OutputType)
            {
                case C.ELinkerOutput.Executable:
                    commandLine.Add(System.String.Format("-o {0}", module.GeneratedPaths[C.ConsoleApplication.Key].ToString()));
                    break;

            case C.ELinkerOutput.DynamicLibrary:
                {
                    commandLine.Add("-dynamiclib");
                    commandLine.Add(System.String.Format("-o {0}", module.GeneratedPaths[C.ConsoleApplication.Key].ToString()));
                    var osxOpts = settings as C.ILinkerSettingsOSX;
                    if (null != osxOpts.InstallName)
                    {
                        commandLine.Add(System.String.Format("-Wl,-dylib_install_name,{0}", osxOpts.InstallName.Parse()));
                    }

                    var version = System.String.Format("{0}.{1}", module.Macros["MajorVersion"].Parse(), module.Macros["MinorVersion"].Parse());
                    commandLine.Add(System.String.Format("-current_version {0}", version));
                    // TODO: offer an option of setting the compatibility version differently
                    commandLine.Add(System.String.Format("-compatibility_version {0}", version));
                }
                break;
            }
            foreach (var path in settings.LibraryPaths)
            {
                commandLine.Add(System.String.Format("-L{0}", path.ParseAndQuoteIfNecessary()));
            }
            foreach (var path in settings.Libraries)
            {
                commandLine.Add(path);
            }
            if (settings.DebugSymbols.GetValueOrDefault())
            {
                commandLine.Add("-g");
            }
        }
    }
}
