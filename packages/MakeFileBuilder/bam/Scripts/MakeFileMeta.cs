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
using System.Linq;
namespace MakeFileBuilder
{
    public sealed class MakeFileMeta
    {
        public MakeFileMeta(
            Bam.Core.Module module)
        {
            this.Module = module;
            module.MetaData = this;
            this.CommonMetaData = Bam.Core.Graph.Instance.MetaData as MakeFileCommonMetaData;
            this.Rules = new Bam.Core.Array<Rule>();
        }

        public MakeFileCommonMetaData CommonMetaData
        {
            get;
            private set;
        }

        public Rule
        AddRule()
        {
            var rule = new Rule(this.Module, this.Rules.Count);
            this.Rules.Add(rule);
            return rule;
        }

        public Bam.Core.Array<Rule> Rules
        {
            get;
            private set;
        }

        private Bam.Core.Module Module
        {
            get;
            set;
        }

        public static void PreExecution()
        {
            var graph = Bam.Core.Graph.Instance;
            graph.MetaData = new MakeFileCommonMetaData();
        }

        public static void PostExecution()
        {
            var graph = Bam.Core.Graph.Instance;
            var commonMeta = graph.MetaData as MakeFileCommonMetaData;

            var makeEnvironment = new System.Text.StringBuilder();
            var makeVariables = new System.Text.StringBuilder();
            var makeRules = new System.Text.StringBuilder();

            // delete suffix rules
            makeEnvironment.AppendLine(".SUFFIXES:");
            foreach (var env in commonMeta.Environment)
            {
                makeEnvironment.AppendFormat("{0}:={1}", env.Key, env.Value.ToString(System.IO.Path.PathSeparator));
                makeEnvironment.AppendLine();
            }

            if (commonMeta.Directories.Count > 0)
            {
                makeVariables.Append("DIRS:=");
                foreach (var dir in commonMeta.Directories)
                {
                    makeVariables.AppendFormat("{0} ", dir);
                }
                makeVariables.AppendLine();
            }

            // all rule
            makeRules.Append("all:");
            var allPrerequisites = new Bam.Core.StringArray();
            foreach (var module in graph.TopLevelModules)
            {
                var metadata = module.MetaData as MakeFileMeta;
                if (null == metadata)
                {
                    throw new Bam.Core.Exception("Top level module, {0}, did not have any Make metadata", module.ToString());
                }
                foreach (var rule in metadata.Rules)
                {
                    // TODO: could just exit from the loop after the first iteration
                    if (!rule.IsFirstRule)
                    {
                        continue;
                    }
                    rule.AppendTargetNames(allPrerequisites);
                }
            }
            makeRules.AppendLine(allPrerequisites.ToString(' '));

            // directory direction rule
            makeRules.AppendLine("$(DIRS):");
            if (Bam.Core.OSUtilities.IsWindowsHosting)
            {
                makeRules.AppendLine("\tmkdir $@");
            }
            else
            {
                makeRules.AppendLine("\tmkdir -pv $@");
            }

            // clean rule
            makeRules.AppendLine(".PHONY: clean");
            makeRules.AppendLine("clean:");
            makeRules.AppendLine("\t@rm -frv $(DIRS)");

            foreach (var rank in graph.Reverse())
            {
                foreach (var module in rank)
                {
                    var metadata = module.MetaData as MakeFileMeta;
                    if (null == metadata)
                    {
                        continue;
                    }

                    foreach (var rule in metadata.Rules)
                    {
                        rule.WriteVariables(makeVariables);
                        rule.WriteRules(makeRules);
                    }
                }
            }

            Bam.Core.Log.DebugMessage("MAKEFILE CONTENTS: BEGIN");
            Bam.Core.Log.DebugMessage(makeEnvironment.ToString());
            Bam.Core.Log.DebugMessage(makeVariables.ToString());
            Bam.Core.Log.DebugMessage(makeRules.ToString());
            Bam.Core.Log.DebugMessage("MAKEFILE CONTENTS: END");

            var makeFilePath = Bam.Core.TokenizedString.Create("$(buildroot)/Makefile", null);
            makeFilePath.Parse();

            using (var writer = new System.IO.StreamWriter(makeFilePath.ToString()))
            {
                writer.Write(makeEnvironment.ToString());
                writer.Write(makeVariables.ToString());
                writer.Write(makeRules.ToString());
            }

            Bam.Core.Log.Info("Successfully created MakeFile for package '{0}'\n\t{1}", graph.MasterPackage.Name, makeFilePath);
        }
    }
}
