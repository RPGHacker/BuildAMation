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
#region BamOptionGenerator
// Automatically generated file from BamOptionGenerator.
// Command line arguments:
//     -i=ICCompilerOptions.cs&../../../C/dev/Scripts/ICCompilerOptionsOSX.cs
//     -n=LLVMGcc
//     -c=CCompilerOptionCollection
//     -p
//     -d
//     -dd=../../../CommandLineProcessor/dev/Scripts/CommandLineDelegate.cs&../../../XcodeProjectProcessor/dev/Scripts/Delegate.cs
//     -pv=GccCommon.PrivateData
//     -e
#endregion // BamOptionGenerator
namespace LLVMGcc
{
    public partial class CCompilerOptionCollection
    {
        #region ICCompilerOptions Option delegates
        private static void
        VisibilityCommandLineProcessor(
             object sender,
             Bam.Core.StringArray commandLineBuilder,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            var enumOption = option as Bam.Core.ValueTypeOption<EVisibility>;
            switch (enumOption.Value)
            {
                case EVisibility.Default:
                    commandLineBuilder.Add("-fvisibility=default");
                    break;
                case EVisibility.Hidden:
                    commandLineBuilder.Add("-fvisibility=hidden");
                    break;
                case EVisibility.Internal:
                    commandLineBuilder.Add("-fvisibility=internal");
                    break;
                case EVisibility.Protected:
                    commandLineBuilder.Add("-fvisibility=protected");
                    break;
                default:
                    throw new Bam.Core.Exception("Unrecognized visibility option");
            }
        }
        private static void
        VisibilityXcodeProjectProcessor(
             object sender,
             XcodeBuilder.PBXProject project,
             XcodeBuilder.XcodeNodeData currentObject,
             XcodeBuilder.XCBuildConfiguration configuration,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            var visibility = option as Bam.Core.ValueTypeOption<EVisibility>;
            var visibilityOption = configuration.Options["GCC_SYMBOLS_PRIVATE_EXTERN"];
            if (visibility.Value == EVisibility.Default)
            {
                visibilityOption.AddUnique("NO");
            }
            else
            {
                visibilityOption.AddUnique("YES");
            }
            if (visibilityOption.Count != 1)
            {
                throw new Bam.Core.Exception("More than one visibility option has been set");
            }
        }
        #endregion
        #region C.ICCompilerOptionsOSX Option delegates
        private static void
        FrameworkSearchDirectoriesCommandLineProcessor(
             object sender,
             Bam.Core.StringArray commandLineBuilder,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            var switchPrefix = "-F";
            var frameworkIncludePathsOption = option as Bam.Core.ReferenceTypeOption<Bam.Core.DirectoryCollection>;
            // TODO: convert to 'var'
            foreach (string includePath in frameworkIncludePathsOption.Value)
            {
                if (includePath.Contains(" "))
                {
                    commandLineBuilder.Add(System.String.Format("{0}\"{1}\"", switchPrefix, includePath));
                }
                else
                {
                    commandLineBuilder.Add(System.String.Format("{0}{1}", switchPrefix, includePath));
                }
            }
        }
        private static void
        FrameworkSearchDirectoriesXcodeProjectProcessor(
             object sender,
             XcodeBuilder.PBXProject project,
             XcodeBuilder.XcodeNodeData currentObject,
             XcodeBuilder.XCBuildConfiguration configuration,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            var frameworkPathsOption = option as Bam.Core.ReferenceTypeOption<Bam.Core.DirectoryCollection>;
            configuration.Options["FRAMEWORK_SEARCH_PATHS"].AddRangeUnique(frameworkPathsOption.Value.ToStringArray());
        }
        private static void
        SDKVersionCommandLineProcessor(
             object sender,
             Bam.Core.StringArray commandLineBuilder,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            var sdkVersionOption = option as Bam.Core.ReferenceTypeOption<string>;
            var sysroot = System.String.Format("-isysroot /Applications/Xcode.app/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX{0}.sdk", sdkVersionOption.Value);
            commandLineBuilder.Add(sysroot);
        }
        private static void
        SDKVersionXcodeProjectProcessor(
             object sender,
             XcodeBuilder.PBXProject project,
             XcodeBuilder.XcodeNodeData currentObject,
             XcodeBuilder.XCBuildConfiguration configuration,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            var sdkVersionOption = option as Bam.Core.ReferenceTypeOption<string>;
            configuration.Options["SDKROOT"].AddUnique(System.String.Format("macosx{0}", sdkVersionOption.Value));
        }
        private static void
        DeploymentTargetCommandLineProcessor(
             object sender,
             Bam.Core.StringArray commandLineBuilder,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            var deploymentTargetOption = option as Bam.Core.ReferenceTypeOption<string>;
            var deploymentTarget = System.String.Format("-mmacosx-version-min={0}", deploymentTargetOption.Value);
            commandLineBuilder.Add(deploymentTarget);
        }
        private static void
        DeploymentTargetXcodeProjectProcessor(
             object sender,
             XcodeBuilder.PBXProject project,
             XcodeBuilder.XcodeNodeData currentObject,
             XcodeBuilder.XCBuildConfiguration configuration,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            var deploymentTargetOption = option as Bam.Core.ReferenceTypeOption<string>;
            configuration.Options["MACOSX_DEPLOYMENT_TARGET"].AddUnique(deploymentTargetOption.Value);
        }
        private static void
        SupportedPlatformCommandLineProcessor(
             object sender,
             Bam.Core.StringArray commandLineBuilder,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            // don't think there is a command line for this
        }
        private static void
        SupportedPlatformXcodeProjectProcessor(
             object sender,
             XcodeBuilder.PBXProject project,
             XcodeBuilder.XcodeNodeData currentObject,
             XcodeBuilder.XCBuildConfiguration configuration,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            var supportedPlatformOption = option as Bam.Core.ValueTypeOption<C.EOSXPlatform>;
            switch (supportedPlatformOption.Value)
            {
            case C.EOSXPlatform.MacOSX:
                configuration.Options["SUPPORTED_PLATFORMS"].AddUnique("macosx");
                break;
            default:
                throw new Bam.Core.Exception("Unsupported OSX platform, '{0}'", supportedPlatformOption.Value.ToString());
            }
        }
        private static void
        CompilerNameCommandLineProcessor(
             object sender,
             Bam.Core.StringArray commandLineBuilder,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            // no action required
        }
        private static void
        CompilerNameXcodeProjectProcessor(
             object sender,
             XcodeBuilder.PBXProject project,
             XcodeBuilder.XcodeNodeData currentObject,
             XcodeBuilder.XCBuildConfiguration configuration,
             Bam.Core.Option option,
             Bam.Core.Target target)
        {
            var compilerNameOption = option as Bam.Core.ReferenceTypeOption<string>;
            if (!System.String.IsNullOrEmpty(compilerNameOption.Value))
            {
                configuration.Options["GCC_VERSION"].AddUnique(compilerNameOption.Value);
            }
        }
        #endregion
        protected override void
        SetDelegates(
            Bam.Core.DependencyNode node)
        {
            base.SetDelegates(node);
            this["Visibility"].PrivateData = new GccCommon.PrivateData(VisibilityCommandLineProcessor,VisibilityXcodeProjectProcessor);
            this["FrameworkSearchDirectories"].PrivateData = new GccCommon.PrivateData(FrameworkSearchDirectoriesCommandLineProcessor,FrameworkSearchDirectoriesXcodeProjectProcessor);
            this["SDKVersion"].PrivateData = new GccCommon.PrivateData(SDKVersionCommandLineProcessor,SDKVersionXcodeProjectProcessor);
            this["DeploymentTarget"].PrivateData = new GccCommon.PrivateData(DeploymentTargetCommandLineProcessor,DeploymentTargetXcodeProjectProcessor);
            this["SupportedPlatform"].PrivateData = new GccCommon.PrivateData(SupportedPlatformCommandLineProcessor,SupportedPlatformXcodeProjectProcessor);
            this["CompilerName"].PrivateData = new GccCommon.PrivateData(CompilerNameCommandLineProcessor,CompilerNameXcodeProjectProcessor);
        }
    }
}