﻿// <copyright file="RemoveOpusAssemblyAction.cs" company="Mark Final">
//  Opus
// </copyright>
// <summary>Opus main application.</summary>
// <author>Mark Final</author>

[assembly: Bam.Core.RegisterAction(typeof(Bam.RemoveOpusAssemblyAction))]

namespace Bam
{
    [Core.TriggerAction]
    internal class RemoveOpusAssemblyAction :
        Core.IActionWithArguments
    {
        public string CommandLineSwitch
        {
            get
            {
                return "-removeopusassembly";
            }
        }

        public string Description
        {
            get
            {
                return "Removes an BuildAMation assembly from the package definition (separated by " + System.IO.Path.PathSeparator + ")";
            }
        }

        void
        Core.IActionWithArguments.AssignArguments(
            string arguments)
        {
            var assemblyNames = arguments.Split(System.IO.Path.PathSeparator);
            this.OpusAssemblyNameArray = new Core.StringArray(assemblyNames);
        }

        private Core.StringArray OpusAssemblyNameArray
        {
            get;
            set;
        }

        public bool
        Execute()
        {
            bool isWellDefined;
            var mainPackageId = Core.PackageUtilities.IsPackageDirectory(Core.State.WorkingDirectory, out isWellDefined);
            if (null == mainPackageId)
            {
                throw new Core.Exception("Working directory, '{0}', is not a package", Core.State.WorkingDirectory);
            }
            if (!isWellDefined)
            {
                throw new Core.Exception("Working directory, '{0}', is not a valid package", Core.State.WorkingDirectory);
            }

            var xmlFile = new Core.PackageDefinitionFile(mainPackageId.DefinitionPathName, true);
            if (isWellDefined)
            {
                xmlFile.Read(true);
            }

            var success = false;
            foreach (var opusAssemblyName in this.OpusAssemblyNameArray)
            {
                if (xmlFile.OpusAssemblies.Contains(opusAssemblyName))
                {
                    xmlFile.OpusAssemblies.Remove(opusAssemblyName);

                    Core.Log.MessageAll("Removed BuildAMation assembly '{0}' from package '{1}'", opusAssemblyName, mainPackageId.ToString());

                    success = true;
                }
                else
                {
                    Core.Log.MessageAll("BuildAMation assembly '{0}' was not used by package '{1}'", opusAssemblyName, mainPackageId.ToString());
                }
            }

            if (success)
            {
                xmlFile.Write();
                return true;
            }
            else
            {
                return false;
            }
        }

        #region ICloneable Members

        object
        System.ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}