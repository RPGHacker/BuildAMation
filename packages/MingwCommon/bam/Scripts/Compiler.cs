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
namespace MingwCommon
{
    public abstract class CompilerBase :
        C.CompilerTool
    {
        public CompilerBase()
        {
            this.InheritedEnvironmentVariables.Add("TEMP");

            this.Macros.Add("InstallPath", Configure.InstallPath);
            this.Macros.Add("BinPath", Bam.Core.TokenizedString.Create(@"$(InstallPath)\bin", this));
            this.Macros.Add("CompilerPath", Bam.Core.TokenizedString.Create(@"$(BinPath)\mingw32-gcc-4.8.1.exe", this));
            this.Macros.Add("objext", ".o");

            this.EnvironmentVariables.Add("PATH", new Bam.Core.TokenizedStringArray(this.Macros["BinPath"]));
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
            if (typeof(C.Cxx.ObjectFile).IsInstanceOfType(module) ||
                typeof(C.Cxx.ObjectFileCollection).IsInstanceOfType(module))
            {
                var settings = new Mingw.CxxCompilerSettings(module);
                this.OverrideDefaultSettings(settings);
                return settings;
            }
            else if (typeof(C.ObjectFile).IsInstanceOfType(module) ||
                     typeof(C.CObjectFileCollection).IsInstanceOfType(module))
            {
                var settings = new Mingw.CompilerSettings(module);
                this.OverrideDefaultSettings(settings);
                return settings;
            }
            else
            {
                throw new Bam.Core.Exception("Could not determine type of module {0}", typeof(T).ToString());
            }
        }

        protected abstract void
        OverrideDefaultSettings(
            Bam.Core.Settings settings);
    }

    [C.RegisterCCompiler("Mingw", Bam.Core.EPlatform.Windows, C.EBit.ThirtyTwo)]
    public class Compiler32 :
        CompilerBase
    {
        protected override void
        OverrideDefaultSettings(
            Bam.Core.Settings settings)
        {
            var cSettings = settings as C.ICommonCompilerSettings;
            cSettings.Bits = C.EBit.ThirtyTwo;
        }
    }

    [C.RegisterCxxCompiler("Mingw", Bam.Core.EPlatform.Windows, C.EBit.ThirtyTwo)]
    public sealed class Compiler32Cxx :
        Compiler32
    {
        public Compiler32Cxx()
        {
            this.Macros.Add("CompilerPath", Bam.Core.TokenizedString.Create(@"$(BinPath)\mingw32-g++.exe", this));
        }

        protected override void
        OverrideDefaultSettings(
            Bam.Core.Settings settings)
        {
            base.OverrideDefaultSettings(settings);
            var cSettings = settings as C.ICommonCompilerSettings;
            cSettings.TargetLanguage = C.ETargetLanguage.Cxx;
        }
    }
}