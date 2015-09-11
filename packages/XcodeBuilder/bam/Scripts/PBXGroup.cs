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
namespace XcodeBuilder
{
    public sealed class PBXGroup :
        XcodeNodeData,
        IWriteableNode
    {
        public
        PBXGroup(
            string name) : base(name)
        {
            this.Children = new Bam.Core.Array<XcodeNodeData>();
        }

        public string Path
        {
            get;
            set;
        }

        public string SourceTree
        {
            get;
            set;
        }

        public Bam.Core.Array<XcodeNodeData> Children
        {
            get;
            private set;
        }

#region IWriteableNode implementation

        void
        IWriteableNode.Write(
            System.IO.TextWriter writer)
        {
            if (0 == this.Children.Count)
            {
                return;
            }
            if (string.IsNullOrEmpty(this.SourceTree))
            {
                throw new Bam.Core.Exception("Source tree not set");
            }

            if (string.IsNullOrEmpty(this.Name))
            {
                // this is the main group
                writer.WriteLine("\t\t{0} = {{", this.UUID);
            }
            else
            {
                writer.WriteLine("\t\t{0} /* {1} */ = {{", this.UUID, this.Name);
            }
            writer.WriteLine("\t\t\tisa = PBXGroup;");
            writer.WriteLine("\t\t\tchildren = (");
            foreach (var child in this.Children)
            {
                if (child is PBXFileReference)
                {
                    writer.WriteLine("\t\t\t\t{0} /* {1} */,", child.UUID, (child as PBXFileReference).ShortPath);
                }
                else
                {
                    writer.WriteLine("\t\t\t\t{0} /* {1} */,", child.UUID, child.Name);
                }
            }
            writer.WriteLine("\t\t\t);");
            if (string.IsNullOrEmpty(this.Path))
            {
                if (!string.IsNullOrEmpty(this.Name))
                {
                    writer.WriteLine("\t\t\tname = {0};", this.Name);
                }
            }
            else
            {
                writer.WriteLine("\t\t\tpath = {0};", this.Path);
            }
            writer.WriteLine("\t\t\tsourceTree = \"{0}\";", this.SourceTree);
            writer.WriteLine("\t\t};");
        }

#endregion
    }
}