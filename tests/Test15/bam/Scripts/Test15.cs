#region License
// Copyright (c) 2010-2017, Mark Final
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
namespace Test15
{
    sealed class StaticLibrary2 :
        C.StaticLibrary
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.CreateHeaderContainer("$(packagedir)/include/staticlibrary2.h");
            var source = this.CreateCSourceContainer("$(packagedir)/source/staticlibrary2.c");
            this.PublicPatch((settings, appliedTo) =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;
                    if (null != compiler)
                    {
                        compiler.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)/include"));
                    }
                });

            this.CompileAgainstPublicly<Test14.StaticLibrary1>(source);
        }
    }

    sealed class DynamicLibrary2 :
        C.DynamicLibrary
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            var bamVersion = Bam.Core.Graph.Instance.ProcessState.Version;
            this.Macros["MajorVersion"] = Bam.Core.TokenizedString.CreateVerbatim(bamVersion.Major.ToString());
            this.Macros["MinorVersion"] = Bam.Core.TokenizedString.CreateVerbatim(bamVersion.Minor.ToString());
            this.Macros["PatchVersion"] = Bam.Core.TokenizedString.CreateVerbatim(bamVersion.Build.ToString());
            this.Macros["Description"] = Bam.Core.TokenizedString.CreateVerbatim("Test15: Example dynamic library");

            this.CreateHeaderContainer("$(packagedir)/include/dynamiclibrary2.h");
            var source = this.CreateCSourceContainer("$(packagedir)/source/dynamiclibrary2.c");
            this.PublicPatch((settings, appliedTo) =>
            {
                var compiler = settings as C.ICommonCompilerSettings;
                if (null != compiler)
                {
                    compiler.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)/include"));
                }
            });

            // because DynamicLibrary1 pokes out of the public API of DynamicLibrary2, the dependency has to be marked
            // as 'public' so that forwarding occurs
            this.CompilePubliclyAndLinkAgainst<Test14.DynamicLibrary1>(source);

            if (this.Linker is VisualCCommon.LinkerBase)
            {
                this.LinkAgainst<WindowsSDK.WindowsSDK>();
            }
        }
    }
}
