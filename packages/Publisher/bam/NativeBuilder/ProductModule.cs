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
namespace Publisher
{
namespace V2
{
    public sealed class NativePackager :
        IPackagePolicy
    {
        static private void
        CopyFile(
            Package sender,
            Bam.Core.V2.ExecutionContext context,
            string sourcePath,
            string destinationDir)
        {
            // TODO: convert this to a command line tool as well
            // but it would require a module to have more than one tool
            if (!System.IO.Directory.Exists(destinationDir))
            {
                System.IO.Directory.CreateDirectory(destinationDir);
            }
            var destinationPath = System.IO.Path.Combine(destinationDir, System.IO.Path.GetFileName(sourcePath));

            var commandLine = new Bam.Core.StringArray();
            var interfaceType = Bam.Core.State.ScriptAssembly.GetType("CommandLineProcessor.V2.IConvertToCommandLine");
            if (interfaceType.IsAssignableFrom(sender.Settings.GetType()))
            {
                var map = sender.Settings.GetType().GetInterfaceMap(interfaceType);
                map.InterfaceMethods[0].Invoke(sender.Settings, new[] { sender, commandLine as object });
            }

            commandLine.Add(sourcePath);
            commandLine.Add(destinationPath);
            CommandLineProcessor.V2.Processor.Execute(context, sender.Tool as Bam.Core.V2.ICommandLineTool, commandLine);
        }

        void
        IPackagePolicy.Package(
            Package sender,
            Bam.Core.V2.ExecutionContext context,
            Bam.Core.V2.TokenizedString packageRoot,
            System.Collections.ObjectModel.ReadOnlyDictionary<Bam.Core.V2.Module, System.Collections.Generic.Dictionary<Bam.Core.V2.TokenizedString, PackageReference>> packageObjects)
        {
            var root = packageRoot.Parse();
            foreach (var module in packageObjects)
            {
                foreach (var path in module.Value)
                {
                    var sourcePath = path.Key.ToString();
                    if (path.Value.IsMarker)
                    {
                        var destinationDir = root;
                        if (null != path.Value.SubDirectory)
                        {
                            destinationDir = System.IO.Path.Combine(destinationDir, path.Value.SubDirectory);
                        }
                        CopyFile(sender, context, sourcePath, destinationDir);
                        path.Value.DestinationDir = destinationDir;
                    }
                    else
                    {
                        var subdir = path.Value.SubDirectory;
                        foreach (var reference in path.Value.References)
                        {
                            var destinationDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(reference.DestinationDir, path.Value.SubDirectory));
                            CopyFile(sender, context, sourcePath, destinationDir);
                            path.Value.DestinationDir = destinationDir;
                        }
                    }
                }
            }
        }
    }
}
namespace V2
{
    public sealed class NativeInnoSetup :
        IInnoSetupPolicy
    {
        void
        IInnoSetupPolicy.CreateInstaller(
            InnoSetupInstaller sender,
            Bam.Core.V2.ExecutionContext context,
            Bam.Core.V2.ICommandLineTool compiler,
            Bam.Core.V2.TokenizedString scriptPath)
        {
            var args = new Bam.Core.StringArray();
            args.Add(scriptPath.Parse());
            CommandLineProcessor.V2.Processor.Execute(context, compiler, args);
        }
    }
}
namespace V2
{
    public sealed class NativeNSIS :
        INSISPolicy
    {
        void
        INSISPolicy.CreateInstaller(
            NSISInstaller sender,
            Bam.Core.V2.ExecutionContext context,
            Bam.Core.V2.ICommandLineTool compiler,
            Bam.Core.V2.TokenizedString scriptPath)
        {
            var args = new Bam.Core.StringArray();
            args.Add(scriptPath.Parse());
            CommandLineProcessor.V2.Processor.Execute(context, compiler, args);
        }
    }
}
namespace V2
{
    public sealed class NativeTarBall :
        ITarPolicy
    {
        void
        ITarPolicy.CreateTarBall(
            TarBall sender,
            Bam.Core.V2.ExecutionContext context,
            Bam.Core.V2.ICommandLineTool compiler,
            Bam.Core.V2.TokenizedString scriptPath,
            Bam.Core.V2.TokenizedString outputPath)
        {
            var args = new Bam.Core.StringArray();
            args.Add("-c");
            args.Add("-v");
            args.Add("-T");
            args.Add(scriptPath.Parse());
            args.Add("-f");
            args.Add(outputPath.ToString());
            CommandLineProcessor.V2.Processor.Execute(context, compiler, args);
        }
    }
}
namespace V2
{
    public sealed class NativeDMG :
        IDiskImagePolicy
    {
        void
        IDiskImagePolicy.CreateDMG(
            DiskImage sender,
            Bam.Core.V2.ExecutionContext context,
            Bam.Core.V2.ICommandLineTool compiler,
            Bam.Core.V2.TokenizedString sourceFolderPath,
            Bam.Core.V2.TokenizedString outputPath)
        {
            var volumeName = "My Volume";
            var tempDiskImagePathName = System.IO.Path.GetTempPath() + System.Guid.NewGuid().ToString() + ".dmg"; // must have .dmg extension
            var diskImagePathName = outputPath.ToString();

            // create the disk image
            {
                var args = new Bam.Core.StringArray();
                args.Add("create");
                args.Add("-quiet");
                args.Add("-srcfolder");
                args.Add(System.String.Format("\"{0}\"", sourceFolderPath.ToString()));
                args.Add("-size");
                args.Add("32m");
                args.Add("-fs");
                args.Add("HFS+");
                args.Add("-volname");
                args.Add(System.String.Format("\"{0}\"", volumeName));
                args.Add(tempDiskImagePathName);
                CommandLineProcessor.V2.Processor.Execute(context, compiler, args);
            }

            // mount disk image
            {
                var args = new Bam.Core.StringArray();
                args.Add("attach");
                args.Add("-quiet");
                args.Add(tempDiskImagePathName);
                CommandLineProcessor.V2.Processor.Execute(context, compiler, args);
            }

            // TODO
            /// do a copy

            // unmount disk image
            {
                var args = new Bam.Core.StringArray();
                args.Add("detach");
                args.Add("-quiet");
                args.Add(System.String.Format("\"/Volumes/{0}\"", volumeName));
                CommandLineProcessor.V2.Processor.Execute(context, compiler, args);
            }

            // hdiutil convert myimg.dmg -format UDZO -o myoutputimg.dmg
            {
                var args = new Bam.Core.StringArray();
                args.Add("convert");
                args.Add("-quiet");
                args.Add(tempDiskImagePathName);
                args.Add("-format");
                args.Add("UDZO");
                args.Add("-o");
                args.Add(diskImagePathName);
                CommandLineProcessor.V2.Processor.Execute(context, compiler, args);
            }
        }
    }
}
}
namespace NativeBuilder
{
    public sealed partial class NativeBuilder
    {
        private void
        nativeCopyNodeLocation(
            Publisher.ProductModule moduleToBuild,
            Bam.Core.BaseModule primaryModule,
            Bam.Core.LocationArray directoriesToCreate,
            Publisher.ProductModuleUtilities.MetaData meta,
            Publisher.PublishDependency nodeInfo,
            string publishDirectoryPath,
            object context)
        {
            foreach (var dir in directoriesToCreate)
            {
                var dirPath = dir.GetSinglePath();
                NativeBuilder.MakeDirectory(dirPath);
            }

            var moduleToCopy = meta.Node.Module;
            var moduleLocations = moduleToCopy.Locations;

            var sourceKey = nodeInfo.Key;
            if (!moduleLocations.Contains(sourceKey))
            {
                return;
            }

            var sourceLoc = moduleLocations[sourceKey];
            if (!sourceLoc.IsValid)
            {
                return;
            }

            // take the common subdirectory by default, otherwise override on a per Location basis
            var attribute = meta.Attribute as Publisher.CopyFileLocationsAttribute;
            var subDirectory = attribute.CommonSubDirectory;
            var nodeSpecificSubdirectory = nodeInfo.SubDirectory;
            if (!string.IsNullOrEmpty(nodeSpecificSubdirectory))
            {
                subDirectory = nodeSpecificSubdirectory;
            }

            var publishedKeyName = Publisher.ProductModuleUtilities.GetPublishedKeyName(
                primaryModule,
                moduleToCopy,
                sourceKey);

            if (sourceKey.IsFileKey)
            {
                var publishedKey = new Bam.Core.LocationKey(publishedKeyName, Bam.Core.ScaffoldLocation.ETypeHint.File);
                Publisher.ProductModuleUtilities.CopyFileToLocation(
                    sourceLoc,
                    publishDirectoryPath,
                    subDirectory,
                    moduleToBuild,
                    publishedKey);
            }
            else if (sourceKey.IsSymlinkKey)
            {
                var publishedKey = new Bam.Core.LocationKey(publishedKeyName, Bam.Core.ScaffoldLocation.ETypeHint.Symlink);
                Publisher.ProductModuleUtilities.CopySymlinkToLocation(
                    sourceLoc,
                    publishDirectoryPath,
                    subDirectory,
                    moduleToBuild,
                    publishedKey);
            }
            else if (sourceKey.IsDirectoryKey)
            {
                throw new Bam.Core.Exception("Directories cannot be published yet");
            }
            else
            {
                throw new Bam.Core.Exception("Unsupported Location type");
            }
        }

        private void
        nativeCopyAdditionalDirectory(
            Publisher.ProductModule moduleToBuild,
            Bam.Core.BaseModule primaryModule,
            Bam.Core.LocationArray directoriesToCreate,
            Publisher.ProductModuleUtilities.MetaData meta,
            Publisher.PublishDirectory directoryInfo,
            string publishDirectoryPath,
            object context)
        {
            foreach (var dir in directoriesToCreate)
            {
                var dirPath = dir.GetSinglePath();
                NativeBuilder.MakeDirectory(dirPath);
            }

            var publishedKeyName = Publisher.ProductModuleUtilities.GetPublishedAdditionalDirectoryKeyName(
                primaryModule,
                directoryInfo.Directory);
            var publishedKey = new Bam.Core.LocationKey(publishedKeyName, Bam.Core.ScaffoldLocation.ETypeHint.Directory);
            var sourceLoc = directoryInfo.DirectoryLocation;
            var attribute = meta.Attribute as Publisher.AdditionalDirectoriesAttribute;
            var subdirectory = attribute.CommonSubDirectory;
            Publisher.ProductModuleUtilities.CopyDirectoryToLocation(
                sourceLoc,
                publishDirectoryPath,
                subdirectory,
                directoryInfo.RenamedLeaf,
                moduleToBuild,
                publishedKey);
        }

        private void
        nativeCopyInfoPList(
            Publisher.ProductModule moduleToBuild,
            Bam.Core.BaseModule primaryModule,
            Bam.Core.LocationArray directoriesToCreate,
            Publisher.ProductModuleUtilities.MetaData meta,
            Publisher.PublishDependency nodeInfo,
            string publishDirectoryPath,
            object context)
        {
            foreach (var dir in directoriesToCreate)
            {
                var dirPath = dir.GetSinglePath();
                NativeBuilder.MakeDirectory(dirPath);
            }

            var plistNode = meta.Node;

            var moduleToCopy = plistNode.Module;
            var keyToCopy = nodeInfo.Key;

            var publishedKeyName = Publisher.ProductModuleUtilities.GetPublishedKeyName(
                primaryModule,
                moduleToCopy,
                keyToCopy);
            var publishedKey = new Bam.Core.LocationKey(publishedKeyName, Bam.Core.ScaffoldLocation.ETypeHint.File);
            var contentsLoc = moduleToBuild.Locations[Publisher.ProductModule.OSXAppBundleContents].GetSingleRawPath();
            var plistSourceLoc = moduleToCopy.Locations[keyToCopy];
            Publisher.ProductModuleUtilities.CopyFileToLocation(
                plistSourceLoc,
                contentsLoc,
                string.Empty,
                moduleToBuild,
                publishedKey);
        }

        public object
        Build(
            Publisher.ProductModule moduleToBuild,
            out bool success)
        {
            Publisher.DelegateProcessing.Process(
                moduleToBuild,
                nativeCopyNodeLocation,
                nativeCopyAdditionalDirectory,
                nativeCopyInfoPList,
                null,
                false);

            success = true;
            return null;
        }
    }
}