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
namespace C
{
    public abstract class CModule :
        Bam.Core.Module
    {
        protected Bam.Core.Array<Bam.Core.Module> headerModules = new Bam.Core.Array<Bam.Core.Module>();

        public CModule()
        {
            this.Macros.Add("OutputName", this.Macros["modulename"]);
            this.Macros.Add("MajorVersion", Bam.Core.TokenizedString.CreateVerbatim("1"));
            this.Macros.Add("MinorVersion", Bam.Core.TokenizedString.CreateVerbatim("0"));
            // default bit depth
            this.BitDepth = (EBit)Bam.Core.CommandLineProcessor.Evaluate(new DefaultBitDepth());
        }

        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            // if there is a parent from which this module is created, inherit bitdepth
            if (null != parent)
            {
                this.BitDepth = (parent as CModule).BitDepth;
            }
        }

        public EBit BitDepth
        {
            get;
            set;
        }

        protected static Bam.Core.Array<Bam.Core.Module>
        FlattenHierarchicalFileList(
            Bam.Core.Array<Bam.Core.Module> files)
        {
            var list = new Bam.Core.Array<Bam.Core.Module>();
            foreach (var input in files)
            {
                if (input is Bam.Core.IModuleGroup)
                {
                    foreach (var child in input.Children)
                    {
                        list.Add(child);
                    }
                }
                else
                {
                    list.Add(input);
                }
            }
            return list;
        }

        protected T
        InternalCreateContainer<T>(
            bool requires,
            string wildcardPath = null,
            Bam.Core.Module macroModuleOverride = null,
            System.Text.RegularExpressions.Regex filter = null,
            Bam.Core.Module.PrivatePatchDelegate privatePatch = null)
            where T : CModule, IAddFiles, new()
        {
            var source = Bam.Core.Module.Create<T>(this);
            if (null != privatePatch)
            {
                source.PrivatePatch(privatePatch);
            }

            if (requires)
            {
                this.Requires(source);
            }
            else
            {
                this.DependsOn(source);
            }

            if (null != wildcardPath)
            {
                (source as IAddFiles).AddFiles(wildcardPath, macroModuleOverride: macroModuleOverride, filter: filter);
            }

            return source;
        }

        public HeaderFileCollection
        CreateHeaderContainer(
            string wildcardPath = null,
            Bam.Core.Module macroModuleOverride = null,
            System.Text.RegularExpressions.Regex filter = null)
        {
            var headers = this.InternalCreateContainer<HeaderFileCollection>(true, wildcardPath, macroModuleOverride, filter);
            this.headerModules.Add(headers);
            return headers;
        }
    }
}
