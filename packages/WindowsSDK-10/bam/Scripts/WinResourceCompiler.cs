#region License
// Copyright (c) 2010-2016, Mark Final
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
namespace WindowsSDK
{
    [C.RegisterWinResourceCompiler("VisualC", Bam.Core.EPlatform.Windows, C.EBit.ThirtyTwo)]
    [C.RegisterWinResourceCompiler("VisualC", Bam.Core.EPlatform.Windows, C.EBit.SixtyFour)]
    public sealed class WinResourceCompiler :
        C.WinResourceCompilerTool
    {
        public WinResourceCompiler()
        {
            var architecture = Bam.Core.TokenizedString.CreateVerbatim(Bam.Core.OSUtilities.Is64BitHosting ? "x64" : "x86");
            this.Macros.Add("CompilerPath", Bam.Core.TokenizedString.Create("$(0)/Windows Kits/8.1/bin/$(1)/rc.exe", null, new Bam.Core.TokenizedStringArray(Bam.Core.OSUtilities.WindowsProgramFilesx86Path, architecture)));
            this.Macros.AddVerbatim("objext", ".res");
        }

        public override Bam.Core.TokenizedString Executable
        {
            get
            {
                return this.Macros["CompilerPath"];
            }
        }

        public override Bam.Core.Settings
        CreateDefaultSettings<T>(
            T module)
        {
            var settings = new WinResourceCompilerSettings(module);
            return settings;
        }
    }
}
