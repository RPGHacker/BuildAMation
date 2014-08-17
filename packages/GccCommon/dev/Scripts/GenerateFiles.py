#!/usr/bin/python

from distutils.spawn import find_executable
import os
import sys
from optparse import OptionParser

bam_path = find_executable('bam')
if not bam_path:
  raise RuntimeError('Unable to locate bam')
bam_dir = os.path.dirname(bam_path)
package_tools_dir = os.path.join(bam_dir, 'packagetools')
sys.path.append(package_tools_dir)

from executeprocess import ExecuteProcess
from getpaths import GetBuildAMationPaths
from standardoptions import StandardOptions

parser = OptionParser()
(options, args, extra_args) = StandardOptions(parser)

stdPackageDir, testPackageDir, optionGeneratorExe = GetBuildAMationPaths(bam_dir)

# C compiler options
cCompiler_options = [
    optionGeneratorExe,
    "-i=" + os.path.relpath(os.path.join(stdPackageDir, "C", "dev", "Scripts", "ICCompilerOptions.cs")) + os.pathsep + \
            os.path.relpath(os.path.join(stdPackageDir, "GccCommon", "dev", "Scripts", "ICCompilerOptions.cs")),
    "-n=GccCommon",
    "-c=CCompilerOptionCollection",
    "-p", # generate properties
    "-d", # generate delegates
    "-dd=" + os.path.relpath(os.path.join(stdPackageDir, "CommandLineProcessor", "dev", "Scripts", "CommandLineDelegate.cs")) + os.pathsep + \
             os.path.relpath(os.path.join(stdPackageDir, "XcodeProjectProcessor", "dev", "Scripts", "Delegate.cs")),
    "-pv=PrivateData"
]
cCompiler_options.extend(extra_args)
(stdout,stderr) = ExecuteProcess(cCompiler_options, True, True)
print stdout

# C++ compiler options
cxxCompiler_options = [
    optionGeneratorExe,
    "-i=" + os.path.relpath(os.path.join(stdPackageDir, "C", "dev", "Scripts", "ICxxCompilerOptions.cs")),
    "-n=GccCommon",
    "-c=CxxCompilerOptionCollection",
    "-p", # generate properties
    "-d", # generate delegates
    "-dd=" + os.path.relpath(os.path.join(stdPackageDir, "CommandLineProcessor", "dev", "Scripts", "CommandLineDelegate.cs")) + os.pathsep + \
             os.path.relpath(os.path.join(stdPackageDir, "XcodeProjectProcessor", "dev", "Scripts", "Delegate.cs")),
    "-pv=PrivateData",
    "-e", # this option set derives from the C option set
    "-b" # used as a base class
]
cxxCompiler_options.extend(extra_args)
(stdout,stderr) = ExecuteProcess(cxxCompiler_options, True, True)
print stdout

# ObjectiveC compiler options
# there are no options at present

# ObjectiveC++ compiler options
objCxxCompiler_options = [
    optionGeneratorExe,
    "-i=" + os.path.relpath(os.path.join(stdPackageDir, "C", "dev", "Scripts", "ICxxCompilerOptions.cs")),
    "-n=GccCommon",
    "-c=ObjCxxCompilerOptionCollection",
    "-p", # generate properties
    "-d", # generate delegates
    "-dd=" + os.path.relpath(os.path.join(stdPackageDir, "CommandLineProcessor", "dev", "Scripts", "CommandLineDelegate.cs")) + os.pathsep + \
             os.path.relpath(os.path.join(stdPackageDir, "XcodeProjectProcessor", "dev", "Scripts", "Delegate.cs")),
    "-pv=PrivateData",
    "-e", # this option set derives from the C option set
    "-b" # used as a base class
]
objCxxCompiler_options.extend(extra_args)
(stdout,stderr) = ExecuteProcess(objCxxCompiler_options, True, True)
print stdout

# Linker options
linker_options = [
    optionGeneratorExe,
    "-i=" + os.path.relpath(os.path.join(stdPackageDir, "C", "dev", "Scripts", "ILinkerOptions.cs")) + os.pathsep + \
            os.path.relpath(os.path.join(stdPackageDir, "C", "dev", "Scripts", "ILinkerOptionsOSX.cs")) + os.pathsep + \
            os.path.relpath(os.path.join(stdPackageDir, "GccCommon", "dev", "Scripts", "ILinkerOptions.cs")),
    "-n=GccCommon",
    "-c=LinkerOptionCollection",
    "-p", # generate properties
    "-d", # generate delegates
    "-dd=" + os.path.relpath(os.path.join(stdPackageDir, "CommandLineProcessor", "dev", "Scripts", "CommandLineDelegate.cs")) + os.pathsep + \
             os.path.relpath(os.path.join(stdPackageDir, "XcodeProjectProcessor", "dev", "Scripts", "Delegate.cs")),
    "-pv=PrivateData"
]
linker_options.extend(extra_args)
(stdout,stderr) = ExecuteProcess(linker_options, True, True)
print stdout

# Archiver options
archiver_options = [
    optionGeneratorExe,
    "-i=" + os.path.relpath(os.path.join(stdPackageDir, "C", "dev", "Scripts", "IArchiverOptions.cs")) + os.pathsep + \
            os.path.relpath(os.path.join(stdPackageDir, "GccCommon", "dev", "Scripts", "IArchiverOptions.cs")),
    "-n=GccCommon",
    "-c=ArchiverOptionCollection",
    "-p", # generate properties
    "-d", # generate delegates
    "-dd=" + os.path.relpath(os.path.join(stdPackageDir, "CommandLineProcessor", "dev", "Scripts", "CommandLineDelegate.cs")) + os.pathsep + \
             os.path.relpath(os.path.join(stdPackageDir, "XcodeProjectProcessor", "dev", "Scripts", "Delegate.cs")),
    "-pv=PrivateData"
]
archiver_options.extend(extra_args)
(stdout,stderr) = ExecuteProcess(archiver_options, True, True)
print stdout
