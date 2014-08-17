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
namespace MakeFileBuilder
{
    public sealed partial class MakeFileBuilder
    {
        public object
        Build(
            XmlUtilities.XmlModule moduleToBuild,
            out bool success)
        {
            var isPlist = moduleToBuild is XmlUtilities.OSXPlistModule;
            var locationMap = moduleToBuild.Locations;
            var outputDir = locationMap[XmlUtilities.OSXPlistModule.OutputDir];
            var outputDirPath = outputDir.GetSingleRawPath();

            if (!System.IO.Directory.Exists(outputDirPath))
            {
                System.IO.Directory.CreateDirectory(outputDirPath);
            }

            var xmlFileLoc = locationMap[XmlUtilities.XmlModule.OutputFile];
            var xmlFilePath = xmlFileLoc.GetSingleRawPath();

            // write a script that can be invoked by the MakeFile to generate the XML file
            var shellScriptLeafName = isPlist ? "writePList.py" : "writeXMLFile.py";
            var shellScriptLoc = Bam.Core.FileLocation.Get(outputDir, shellScriptLeafName, Bam.Core.Location.EExists.WillExist);
            var shellScriptPath = shellScriptLoc.GetSingleRawPath();
            XmlUtilities.XmlDocumentToPythonScript.Write(moduleToBuild.Document, shellScriptPath, xmlFilePath);

            var node = moduleToBuild.OwningNode;
            var makeFile = new MakeFile(node, this.topLevelMakeFilePath);

            var dirsToCreate = moduleToBuild.Locations.FilterByType(Bam.Core.ScaffoldLocation.ETypeHint.Directory, Bam.Core.Location.EExists.WillExist);

            var recipe = new Bam.Core.StringArray();
            recipe.Add(System.String.Format("$(shell python {0})", shellScriptPath));

            var rule = new MakeFileRule(
                moduleToBuild,
                XmlUtilities.XmlModule.OutputFile,
                node.UniqueModuleName,
                dirsToCreate,
                null,
                null,
                recipe);
            rule.OutputLocationKeys = new Bam.Core.Array<Bam.Core.LocationKey>(XmlUtilities.XmlModule.OutputFile);
            makeFile.RuleArray.Add(rule);

            var makeFilePath = MakeFileBuilder.GetMakeFilePathName(node);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(makeFilePath));

            using (var makeFileWriter = new System.IO.StreamWriter(makeFilePath))
            {
                makeFile.Write(makeFileWriter);
            }

            var exportedTargets = makeFile.ExportedTargets;
            var exportedVariables = makeFile.ExportedVariables;
            var returnData = new MakeFileData(makeFilePath, exportedTargets, exportedVariables, null);
            success = true;
            return returnData;
        }
    }
}
