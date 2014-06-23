// <copyright file="ProductModule.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>Publisher package</summary>
// <author>Mark Final</author>
namespace QMakeBuilder
{
    public sealed partial class QMakeBuilder
    {
        private static void
        InstallFile(
            string destinationDirectory,
            QMakeData proData)
        {
            var customRules = new Opus.Core.StringArray();
            customRules.Add(System.String.Format("target.path={0}", destinationDirectory));
            customRules.Add(System.String.Format("INSTALLS+=target"));
            if (null == proData.CustomRules)
            {
                proData.CustomRules = customRules;
            }
            else
            {
                proData.CustomRules.AddRangeUnique(customRules);
            }
        }

        public object
        Build(
            Publisher.ProductModule moduleToBuild,
            out bool success)
        {
            var primaryNode = Publisher.ProductModuleUtilities.GetPrimaryNode(moduleToBuild);
            var locationMap = primaryNode.Module.Locations;
            var publishDirLoc = locationMap[C.Application.OutputDir];
            var publishDirPath = publishDirLoc.GetSingleRawPath();

            foreach (var dependency in primaryNode.ExternalDependents)
            {
                var proData = dependency.Data as QMakeData;
                var module = dependency.Module;
                var moduleType = module.GetType();
                var flags = System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic;
                var fields = moduleType.GetFields(flags);
                foreach (var field in fields)
                {
                    var candidates = field.GetCustomAttributes(typeof(Publisher.PublishModuleDependencyAttribute), false);
                    if (0 == candidates.Length)
                    {
                        continue;
                    }
                    if (candidates.Length > 1)
                    {
                        throw new Opus.Core.Exception("More than one publish module dependency found");
                    }
                    var candidateData = field.GetValue(module) as Opus.Core.Array<Opus.Core.LocationKey>;
                    foreach (var key in candidateData)
                    {
                        // TODO: the key needs to be on an options interface or similiar
                        var sourceLoc = dependency.Module.Locations[C.Application.OutputFile];
                        var sourcePath = sourceLoc.GetSingleRawPath();

                        var keyName = Publisher.ProductModuleUtilities.GetPublishedKeyName(primaryNode.Module, module, key);
                        var newKey = new Opus.Core.LocationKey(keyName, Opus.Core.ScaffoldLocation.ETypeHint.File);
                        Publisher.ProductModuleUtilities.GenerateDestinationPath(sourcePath, publishDirPath, moduleToBuild, newKey);

                        InstallFile(publishDirPath, proData);
                    }
                }
            }

            success = true;
            return null;
        }
    }
}
