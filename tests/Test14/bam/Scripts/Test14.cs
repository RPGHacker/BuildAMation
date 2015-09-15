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
using Bam.Core;
namespace Test14
{
    public sealed class DynamicLibraryA :
        C.DynamicLibrary
    {
        private Bam.Core.Module.PublicPatchDelegate includePaths = (settings, appliedTo) =>
            {
                var compiler = settings as C.ICommonCompilerOptions;
                if (null != compiler)
                {
                    compiler.IncludePaths.Add(Bam.Core.TokenizedString.Create("$(packagedir)/include", appliedTo));
                }
            };

        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.CreateHeaderContainer("$(packagedir)/include/dynamicLibraryA.h");

            var source = this.CreateCSourceContainer("$(packagedir)/source/dynamicLibraryA.c");
            source.PrivatePatch(settings => this.includePaths(settings, this));

            this.PublicPatch((settings, appliedTo) => this.includePaths(settings, appliedTo));

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows) &&
                this.Linker is VisualC.LinkerBase)
            {
                this.LinkAgainst<WindowsSDK.WindowsSDK>();
            }
        }
    }

    public sealed class DynamicLibraryB :
        C.DynamicLibrary
    {
        private Bam.Core.Module.PublicPatchDelegate includePaths = (settings, appliedTo) =>
        {
            var compiler = settings as C.ICommonCompilerOptions;
            if (null != compiler)
            {
                compiler.IncludePaths.Add(Bam.Core.TokenizedString.Create("$(packagedir)/include", appliedTo));
            }
        };

        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.CreateHeaderContainer("$(packagedir)/include/dynamicLibraryB.h");

            var source = this.CreateCSourceContainer("$(packagedir)/source/dynamicLibraryB.c");
            source.PrivatePatch(settings => this.includePaths(settings, this));

            this.PublicPatch((settings, appliedTo) => this.includePaths(settings, appliedTo));

            this.LinkAgainst<DynamicLibraryA>();

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows) &&
                this.Linker is VisualC.LinkerBase)
            {
                this.LinkAgainst<WindowsSDK.WindowsSDK>();
            }
        }
    }

    public sealed class Application :
        C.ConsoleApplication
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            var source = this.CreateCSourceContainer("$(packagedir)/source/main.c");

            this.PrivatePatch(settings =>
                {
                    var gccLinker = settings as GccCommon.ICommonLinkerOptions;
                    if (null != gccLinker)
                    {
                        gccLinker.CanUseOrigin = true;
                        gccLinker.RPath.Add("$ORIGIN");
                    }
                });

            this.CompileAndLinkAgainst<DynamicLibraryA>(source);
            this.CompileAndLinkAgainst<DynamicLibraryB>(source);

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows) &&
                this.Linker is VisualC.LinkerBase)
            {
                this.LinkAgainst<WindowsSDK.WindowsSDK>();
            }
        }
    }

    public sealed class RuntimePackage :
        Publisher.Package
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            var app = this.Include<Application>(C.ConsoleApplication.Key, EPublishingType.ConsoleApplication);
            this.Include<DynamicLibraryA>(C.DynamicLibrary.Key, ".", app);
            this.Include<DynamicLibraryB>(C.DynamicLibrary.Key, ".", app);
        }
    }
}
