// <copyright file="VCXBuildProject.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>VSSolutionBuilder package</summary>
// <author>Mark Final</author>
namespace VSSolutionBuilder
{
    public class VCXBuildProject : ICProject
    {
        private string ProjectName = null;
        private string PathName = null;
        private System.Uri PackageUri = null;
        private System.Guid ProjectGuid = System.Guid.NewGuid();
        private System.Collections.Generic.List<string> PlatformList = new System.Collections.Generic.List<string>();
        private ProjectConfigurationCollection ProjectConfigurations = new ProjectConfigurationCollection();
        private ProjectFileCollection SourceFileCollection = new ProjectFileCollection();
        private ProjectFileCollection HeaderFileCollection = new ProjectFileCollection();
        private System.Collections.Generic.List<IProject> DependentProjectList = new System.Collections.Generic.List<IProject>();
        private System.Collections.Generic.List<string> ReferencesList = new System.Collections.Generic.List<string>();

        public VCXBuildProject(string moduleName, string projectPathName, Opus.Core.PackageIdentifier packageId, Opus.Core.ProxyModulePath proxyPath)
        {
            this.ProjectName = moduleName;
            this.PathName = projectPathName;
            this.PackageDirectory = packageId.Path;
            if (null != proxyPath)
            {
                this.PackageDirectory = proxyPath.Combine(packageId);
            }

            bool isPackageDirAbsolute = Opus.Core.RelativePathUtilities.IsPathAbsolute(this.PackageDirectory);
            System.UriKind kind = isPackageDirAbsolute ? System.UriKind.Absolute : System.UriKind.Relative;

            if (this.PackageDirectory[this.PackageDirectory.Length - 1] == System.IO.Path.DirectorySeparatorChar)
            {
                this.PackageUri = new System.Uri(this.PackageDirectory, kind);
            }
            else
            {
                this.PackageUri = new System.Uri(this.PackageDirectory + System.IO.Path.DirectorySeparatorChar, kind);
            }
        }

        string IProject.Name
        {
            get
            {
                return this.ProjectName;
            }
        }

        string IProject.PathName
        {
            get
            {
                return this.PathName;
            }
        }

        System.Guid IProject.Guid
        {
            get
            {
                return this.ProjectGuid;
            }
        }

        public string PackageDirectory
        {
            get;
            private set;
        }

        System.Collections.Generic.List<string> IProject.Platforms
        {
            get
            {
                return this.PlatformList;
            }
        }

        ProjectConfigurationCollection IProject.Configurations
        {
            get
            {
                return this.ProjectConfigurations;
            }
        }

        ProjectFileCollection IProject.SourceFiles
        {
            get
            {
                return this.SourceFileCollection;
            }
        }

        ProjectFileCollection ICProject.HeaderFiles
        {
            get
            {
                return this.HeaderFileCollection;
            }
        }

        System.Collections.Generic.List<IProject> IProject.DependentProjects
        {
            get
            {
                return this.DependentProjectList;
            }
        }

        System.Collections.Generic.List<string> IProject.References
        {
            get
            {
                return this.ReferencesList;
            }
        }

        private void SerializeVCXProj()
        {
            System.Xml.XmlDocument xmlDocument = null;
            try
            {
                System.Uri projectLocationUri = new System.Uri(this.PathName, System.UriKind.RelativeOrAbsolute);

                xmlDocument = new System.Xml.XmlDocument();

                xmlDocument.AppendChild(xmlDocument.CreateComment("Automatically generated by Opus v" + Opus.Core.State.OpusVersionString));

                MSBuildProject project = new MSBuildProject(xmlDocument, "4.0", "Build");

                // project globals (guid, etc)
                {
                    MSBuildPropertyGroup globalPropertyGroup = project.CreatePropertyGroup();
                    globalPropertyGroup.Label = "Globals";
                    globalPropertyGroup.CreateProperty("ProjectGuid", this.ProjectGuid.ToString("B").ToUpper());
                }

                // import default project props
                project.CreateImport(@"$(VCTargetsPath)\Microsoft.Cpp.Default.props");

                // configurations
                this.ProjectConfigurations.SerializeMSBuild(project, projectLocationUri);

                // source files
                if (this.SourceFileCollection.Count > 0)
                {
                    this.SourceFileCollection.SerializeMSBuild(project, "ClCompile", projectLocationUri, this.PackageUri);
                }
                if (this.HeaderFileCollection.Count > 0)
                {
                    this.HeaderFileCollection.SerializeMSBuild(project, "ClInclude", projectLocationUri, this.PackageUri);
                }

                // project dependencies
                // these were in the .sln file pre MSBuild
                if (this.DependentProjectList.Count > 0)
                {
                    MSBuildItemGroup dependencyItemGroup = project.CreateItemGroup();
                    foreach (IProject dependentProject in this.DependentProjectList)
                    {
                        string relativePath = Opus.Core.RelativePathUtilities.GetPath(dependentProject.PathName, this.PathName);
                        MSBuildItem projectReference = dependencyItemGroup.CreateItem("ProjectReference", relativePath);
                        projectReference.CreateMetaData("Project", dependentProject.Guid.ToString("B").ToUpper());
                        projectReference.CreateMetaData("ReferenceOutputAssembly", "false");
                    }
                }

                // import targets
                project.CreateImport(@"$(VCTargetsPath)\Microsoft.Cpp.targets");
            }
            catch (Opus.Core.Exception exception)
            {
                string message = System.String.Format("Xml construction error from project '{0}'", this.PathName);
                throw new Opus.Core.Exception(message, exception);
            }

            // write XML to disk
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(this.PathName));

            System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.CloseOutput = true;
            xmlWriterSettings.OmitXmlDeclaration = false;
            xmlWriterSettings.NewLineOnAttributes = false;

            try
            {
                using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(this.PathName, xmlWriterSettings))
                {
                    xmlDocument.Save(xmlWriter);
                }
            }
            catch (Opus.Core.Exception exception)
            {
                string message = System.String.Format("Serialization error from project '{0}'", this.PathName);
                throw new Opus.Core.Exception(message, exception);
            }
        }

        private void SerializeVCXProjFilters()
        {
            string filtersPath = this.PathName + ".filters";

            System.Xml.XmlDocument xmlDocument = null;
            try
            {
                System.Uri projectLocationUri = new System.Uri(this.PathName, System.UriKind.RelativeOrAbsolute);

                xmlDocument = new System.Xml.XmlDocument();

                xmlDocument.AppendChild(xmlDocument.CreateComment("Automatically generated by Opus v" + Opus.Core.State.OpusVersionString));

                MSBuildProject project = new MSBuildProject(xmlDocument, "4.0", null);

                // create new filters
                MSBuildItemGroup filtersGroup = project.CreateItemGroup();
                Opus.Core.StringArray sourceSubDirectories = new Opus.Core.StringArray();
                Opus.Core.StringArray headerSubDirectories = new Opus.Core.StringArray();
                if (this.SourceFileCollection.Count > 0)
                {
                    {
                        MSBuildItem sourceFilesItem = filtersGroup.CreateItem("Filter", "Source Files");
                        sourceFilesItem.CreateMetaData("UniqueIdentifier", System.Guid.NewGuid().ToString("B").ToUpper());
                    }

                    foreach (ProjectFile file in this.SourceFileCollection)
                    {
                        string subdir = System.IO.Path.GetDirectoryName(file.RelativePath);
                        string relativeSubDirFull = Opus.Core.RelativePathUtilities.GetPath(subdir, this.PackageUri);
                        string[] relativeSubDirs = relativeSubDirFull.Split(System.IO.Path.DirectorySeparatorChar);
                        string currentBase = null;
                        foreach (string subd in relativeSubDirs)
                        {
                            if (null != currentBase)
                            {
                                currentBase = System.IO.Path.Combine(currentBase, subd);
                            }
                            else
                            {
                                currentBase = subd;
                            }

                            if (!sourceSubDirectories.Contains(currentBase))
                            {
                                sourceSubDirectories.Add(currentBase);
                            }
                        }
                    }

                    foreach (string sourceSubDir in sourceSubDirectories)
                    {
                        MSBuildItem sourceFilesItem = filtersGroup.CreateItem("Filter", System.IO.Path.Combine("Source Files", sourceSubDir));
                        sourceFilesItem.CreateMetaData("UniqueIdentifier", System.Guid.NewGuid().ToString("B").ToUpper());
                    }
                }
                if (this.HeaderFileCollection.Count > 0)
                {
                    {
                        MSBuildItem sourceFilesItem = filtersGroup.CreateItem("Filter", "Header Files");
                        sourceFilesItem.CreateMetaData("UniqueIdentifier", System.Guid.NewGuid().ToString("B").ToUpper());
                    }

                    foreach (ProjectFile file in this.HeaderFileCollection)
                    {
                        string subdir = System.IO.Path.GetDirectoryName(file.RelativePath);
                        string relativeSubDirFull = Opus.Core.RelativePathUtilities.GetPath(subdir, this.PackageUri);
                        string[] relativeSubDirs = relativeSubDirFull.Split(System.IO.Path.DirectorySeparatorChar);
                        string currentBase = null;
                        foreach (string subd in relativeSubDirs)
                        {
                            if (null != currentBase)
                            {
                                currentBase = System.IO.Path.Combine(currentBase, subd);
                            }
                            else
                            {
                                currentBase = subd;
                            }

                            if (!headerSubDirectories.Contains(currentBase))
                            {
                                headerSubDirectories.Add(currentBase);
                            }
                        }
                    }

                    foreach (string sourceSubDir in headerSubDirectories)
                    {
                        MSBuildItem sourceFilesItem = filtersGroup.CreateItem("Filter", System.IO.Path.Combine("Header Files", sourceSubDir));
                        sourceFilesItem.CreateMetaData("UniqueIdentifier", System.Guid.NewGuid().ToString("B").ToUpper());
                    }
                }

                // use the filters
                if (this.SourceFileCollection.Count > 0)
                {
                    MSBuildItemGroup sourceFilesGroup = project.CreateItemGroup();
                    foreach (ProjectFile file in this.SourceFileCollection)
                    {
                        string subdir = System.IO.Path.GetDirectoryName(file.RelativePath);
                        string relativeSubDir = Opus.Core.RelativePathUtilities.GetPath(subdir, this.PackageUri);

                        MSBuildItem item = sourceFilesGroup.CreateItem("ClCompile", Opus.Core.RelativePathUtilities.GetPath(file.RelativePath, projectLocationUri));
                        item.CreateMetaData("Filter", System.IO.Path.Combine("Source Files", relativeSubDir));
                    }
                }
                if (this.HeaderFileCollection.Count > 0)
                {
                    MSBuildItemGroup sourceFilesGroup = project.CreateItemGroup();
                    foreach (ProjectFile file in this.HeaderFileCollection)
                    {
                        string subdir = System.IO.Path.GetDirectoryName(file.RelativePath);
                        string relativeSubDir = Opus.Core.RelativePathUtilities.GetPath(subdir, this.PackageUri);

                        string elementName;
                        if ((null == file.FileConfigurations) ||
                            (0 == file.FileConfigurations.Count))
                        {
                            // no configuration - the header is just included
                            elementName = "ClInclude";
                        }
                        else
                        {
                            // a header file has a configuration setting, so must be a custom build step
                            elementName = "CustomBuild";
                        }

                        MSBuildItem item = sourceFilesGroup.CreateItem(elementName, Opus.Core.RelativePathUtilities.GetPath(file.RelativePath, projectLocationUri));
                        item.CreateMetaData("Filter", System.IO.Path.Combine("Header Files", relativeSubDir));
                    }
                }
            }
            catch (Opus.Core.Exception exception)
            {
                string message = System.String.Format("Xml construction error from project '{0}'", filtersPath);
                throw new Opus.Core.Exception(message, exception);
            }

            // write XML to disk
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filtersPath));

            System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.CloseOutput = true;
            xmlWriterSettings.OmitXmlDeclaration = false;
            xmlWriterSettings.NewLineOnAttributes = false;

            try
            {
                using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(filtersPath, xmlWriterSettings))
                {
                    xmlDocument.Save(xmlWriter);
                }
            }
            catch (Opus.Core.Exception exception)
            {
                string message = System.String.Format("Serialization error from project '{0}'", filtersPath);
                throw new Opus.Core.Exception(message, exception);
            }
        }

        void IProject.Serialize()
        {
            this.SerializeVCXProj();
            this.SerializeVCXProjFilters();
        }

        VisualStudioProcessor.EVisualStudioTarget IProject.VSTarget
        {
            get
            {
                return VisualStudioProcessor.EVisualStudioTarget.MSBUILD;
            }
        }
    }
}
