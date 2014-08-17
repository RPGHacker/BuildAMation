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
    public partial class MakeFileBuilder
    {
        public object
        Build(
            CSharp.Assembly moduleToBuild,
            out System.Boolean success)
        {
            var assemblyModule = moduleToBuild as Bam.Core.BaseModule;
            var node = assemblyModule.OwningNode;
            var target = node.Target;
            var assemblyOptions = assemblyModule.Options;
            var options = assemblyOptions as CSharp.OptionCollection;

            var inputVariables = new MakeFileVariableDictionary();
            if (node.ExternalDependents != null)
            {
                foreach (var dependentNode in node.ExternalDependents)
                {
                    if (null != dependentNode.Data)
                    {
                        continue;
                    }

                    var keyFilters = new Bam.Core.Array<Bam.Core.LocationKey>(
                        CSharp.Assembly.OutputFile
                        );
                    var assemblyLocations = new Bam.Core.LocationArray();
                    dependentNode.FilterOutputLocations(keyFilters, assemblyLocations);
                    var data = dependentNode.Data as MakeFileData;
                    var csharpOptions = options as CSharp.IOptions;
                    foreach (var loc in assemblyLocations)
                    {
                        csharpOptions.References.Add(loc.GetSinglePath());
                        inputVariables.Add(CSharp.Assembly.OutputFile, data.VariableDictionary[CSharp.Assembly.OutputDir]);
                    }
                }
            }

            var sourceFiles = new Bam.Core.StringArray();
            var fields = moduleToBuild.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                // C# files
                {
                    var sourceFileAttributes = field.GetCustomAttributes(typeof(Bam.Core.SourceFilesAttribute), false);
                    if (null != sourceFileAttributes && sourceFileAttributes.Length > 0)
                    {
                        var sourceField = field.GetValue(moduleToBuild);
                        if (sourceField is Bam.Core.Location)
                        {
                            var file = sourceField as Bam.Core.Location;
                            var absolutePath = file.GetSinglePath();
                            if (!System.IO.File.Exists(absolutePath))
                            {
                                throw new Bam.Core.Exception("Source file '{0}' does not exist", absolutePath);
                            }

                            sourceFiles.Add(absolutePath);
                        }
                        else if (sourceField is Bam.Core.FileCollection)
                        {
                            var sourceCollection = sourceField as Bam.Core.FileCollection;
                            // TODO: convert to var
                            foreach (Bam.Core.Location location in sourceCollection)
                            {
                                var absolutePath = location.GetSinglePath();
                                if (!System.IO.File.Exists(absolutePath))
                                {
                                    throw new Bam.Core.Exception("Source file '{0}' does not exist", absolutePath);
                                }

                                sourceFiles.Add(absolutePath);
                            }
                        }
                        else
                        {
                            throw new Bam.Core.Exception("Field '{0}' of '{1}' should be of type Bam.Core.File or Bam.Core.FileCollection, not '{2}'", field.Name, node.ModuleName, sourceField.GetType().ToString());
                        }
                    }
                }

                // WPF application definition .xaml file
                {
                    var xamlFileAttributes = field.GetCustomAttributes(typeof(CSharp.ApplicationDefinitionAttribute), false);
                    if (null != xamlFileAttributes && xamlFileAttributes.Length > 0)
                    {
                        var sourceField = field.GetValue(moduleToBuild);
                        if (sourceField is Bam.Core.Location)
                        {
                            var file = sourceField as Bam.Core.Location;
                            var absolutePath = file.GetSinglePath();
                            if (!System.IO.File.Exists(absolutePath))
                            {
                                throw new Bam.Core.Exception("Application definition file '{0}' does not exist", absolutePath);
                            }

                            var csPath = absolutePath + ".cs";
                            if (!System.IO.File.Exists(csPath))
                            {
                                throw new Bam.Core.Exception("Associated source file '{0}' to application definition file '{1}' does not exist", csPath, absolutePath);
                            }

                            sourceFiles.Add(csPath);
                        }
                        else if (sourceField is Bam.Core.FileCollection)
                        {
                            var sourceCollection = sourceField as Bam.Core.FileCollection;
                            if (sourceCollection.Count != 1)
                            {
                                throw new Bam.Core.Exception("There can be only one application definition");
                            }

                            // TODO: convert to var
                            foreach (string absolutePath in sourceCollection)
                            {
                                if (!System.IO.File.Exists(absolutePath))
                                {
                                    throw new Bam.Core.Exception("Application definition file '{0}' does not exist", absolutePath);
                                }

                                var csPath = absolutePath + ".cs";
                                if (!System.IO.File.Exists(csPath))
                                {
                                    throw new Bam.Core.Exception("Associated source file '{0}' to application definition file '{1}' does not exist", csPath, absolutePath);
                                }

                                sourceFiles.Add(csPath);
                            }
                        }
                        else
                        {
                            throw new Bam.Core.Exception("Field '{0}' of '{1}' should be of type Bam.Core.File or Bam.Core.FileCollection, not '{2}'", field.Name, node.ModuleName, sourceField.GetType().ToString());
                        }
                    }
                }

                // WPF page .xaml files
                {
                    var xamlFileAttributes = field.GetCustomAttributes(typeof(CSharp.PagesAttribute), false);
                    if (null != xamlFileAttributes && xamlFileAttributes.Length > 0)
                    {
                        var sourceField = field.GetValue(moduleToBuild);
                        if (sourceField is Bam.Core.Location)
                        {
                            var file = sourceField as Bam.Core.Location;
                            var absolutePath = file.GetSinglePath();
                            if (!System.IO.File.Exists(absolutePath))
                            {
                                throw new Bam.Core.Exception("Page file '{0}' does not exist", absolutePath);
                            }

                            var csPath = absolutePath + ".cs";
                            if (!System.IO.File.Exists(csPath))
                            {
                                throw new Bam.Core.Exception("Associated source file '{0}' to page file '{1}' does not exist", csPath, absolutePath);
                            }

                            sourceFiles.Add(csPath);
                        }
                        else if (sourceField is Bam.Core.FileCollection)
                        {
                            var sourceCollection = sourceField as Bam.Core.FileCollection;
                            if (sourceCollection.Count != 1)
                            {
                                throw new Bam.Core.Exception("There can be only one page file");
                            }

                            // TODO: convert to var
                            foreach (string absolutePath in sourceCollection)
                            {
                                if (!System.IO.File.Exists(absolutePath))
                                {
                                    throw new Bam.Core.Exception("Page file '{0}' does not exist", absolutePath);
                                }

                                var csPath = absolutePath + ".cs";
                                if (!System.IO.File.Exists(csPath))
                                {
                                    throw new Bam.Core.Exception("Associated source file '{0}' to page file '{1}' does not exist", csPath, absolutePath);
                                }

                                sourceFiles.Add(csPath);
                            }
                        }
                        else
                        {
                            throw new Bam.Core.Exception("Field '{0}' of '{1}' should be of type Bam.Core.File or Bam.Core.FileCollection, not '{2}'", field.Name, node.ModuleName, sourceField.GetType().ToString());
                        }
                    }
                }
            }

            if (0 == sourceFiles.Count)
            {
                throw new Bam.Core.Exception("There were no source files specified for the module '{0}'", node.ModuleName);
            }

            // at this point, we know the node outputs need building

            // create all directories required
            var dirsToCreate = moduleToBuild.Locations.FilterByType(Bam.Core.ScaffoldLocation.ETypeHint.Directory, Bam.Core.Location.EExists.WillExist);

            var commandLineBuilder = new Bam.Core.StringArray();
            if (options is CommandLineProcessor.ICommandLineSupport)
            {
                var commandLineOption = options as CommandLineProcessor.ICommandLineSupport;
                commandLineOption.ToCommandLineArguments(commandLineBuilder, target, null);
            }
            else
            {
                throw new Bam.Core.Exception("Compiler options does not support command line translation");
            }

            foreach (var source in sourceFiles)
            {
                if (source.Contains(" "))
                {
                    commandLineBuilder.Add(System.String.Format("\"{0}\"", source));
                }
                else
                {
                    commandLineBuilder.Add(source);
                }
            }

            var compilerInstance = target.Toolset.Tool(typeof(CSharp.ICSharpCompilerTool));
            var executablePath = compilerInstance.Executable((Bam.Core.BaseTarget)target);

            var recipes = new Bam.Core.StringArray();
            if (executablePath.Contains(" "))
            {
                recipes.Add(System.String.Format("\"{0}\" {1}", executablePath, commandLineBuilder.ToString(' ')));
            }
            else
            {
                recipes.Add(System.String.Format("{0} {1}", executablePath, commandLineBuilder.ToString(' ')));
            }

            var makeFile = new MakeFile(node, this.topLevelMakeFilePath);

            var rule = new MakeFileRule(
                moduleToBuild,
                CSharp.Assembly.OutputFile,
                node.UniqueModuleName,
                dirsToCreate,
                inputVariables,
                null,
                recipes);

            var toolOutputLocKeys = compilerInstance.OutputLocationKeys(moduleToBuild);
            var outputFileLocations = moduleToBuild.Locations.Keys(Bam.Core.ScaffoldLocation.ETypeHint.File, Bam.Core.Location.EExists.WillExist);
            var outputFileLocationsOfInterest = outputFileLocations.Intersect(toolOutputLocKeys);
            rule.OutputLocationKeys = outputFileLocationsOfInterest;
            makeFile.RuleArray.Add(rule);

            var makeFilePath = MakeFileBuilder.GetMakeFilePathName(node);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(makeFilePath));
            Bam.Core.Log.DebugMessage("Makefile : '{0}'", makeFilePath);

            using (System.IO.TextWriter makeFileWriter = new System.IO.StreamWriter(makeFilePath))
            {
                makeFile.Write(makeFileWriter);
            }

            success = true;
            var compilerTool = compilerInstance as Bam.Core.ITool;
            System.Collections.Generic.Dictionary<string, Bam.Core.StringArray> environment = null;
            if (compilerTool is Bam.Core.IToolEnvironmentVariables)
            {
                environment = (compilerTool as Bam.Core.IToolEnvironmentVariables).Variables((Bam.Core.BaseTarget)target);
            }
            var returnData = new MakeFileData(makeFilePath, makeFile.ExportedTargets, makeFile.ExportedVariables, environment);
            return returnData;
        }
    }
}
