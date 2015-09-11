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

licenseHeaderFile = os.path.relpath(os.path.join(os.path.dirname(optionGeneratorExe), "licenseheader.txt"))

# C compiler options
cCompiler_options = [
    optionGeneratorExe,
    "-i=" + os.path.relpath(os.path.join(stdPackageDir, "C", "dev", "Scripts", "ICCompilerOptions.cs")) + os.pathsep + os.path.relpath(os.path.join(stdPackageDir, "MingwCommon", "dev", "Scripts", "ICCompilerOptions.cs")),
    "-n=MingwCommon",
    "-c=CCompilerOptionCollection",
    "-p", # generate properties
    "-d", # generate delegates
    "-dd=" + os.path.relpath(os.path.join(stdPackageDir, "CommandLineProcessor", "dev", "Scripts", "CommandLineDelegate.cs")),
    "-pv=PrivateData",
    "-l=" + licenseHeaderFile
]
cCompiler_options.extend(extra_args)
(stdout,stderr) = ExecuteProcess(cCompiler_options, True, True)
print stdout

# C++ compiler options
cxxCompiler_options = [
    optionGeneratorExe,
    "-i=" + os.path.relpath(os.path.join(stdPackageDir, "C", "dev", "Scripts", "ICxxCompilerOptions.cs")),
    "-n=MingwCommon",
    "-c=CxxCompilerOptionCollection",
    "-p", # generate properties
    "-d", # generate delegates
    "-dd=" + os.path.relpath(os.path.join(stdPackageDir, "CommandLineProcessor", "dev", "Scripts", "CommandLineDelegate.cs")),
    "-pv=PrivateData",
    "-e", # this option set derives from the C option set
    "-b", # used as a base class for shared code
    "-l=" + licenseHeaderFile
]
cxxCompiler_options.extend(extra_args)
(stdout,stderr) = ExecuteProcess(cxxCompiler_options, True, True)
print stdout

# Linker options
linker_options = [
    optionGeneratorExe,
    "-i=" + os.path.relpath(os.path.join(stdPackageDir, "C", "dev", "Scripts", "ILinkerOptions.cs")) + os.pathsep + os.path.relpath(os.path.join(stdPackageDir, "MingwCommon", "dev", "Scripts", "ILinkerOptions.cs")),
    "-n=MingwCommon",
    "-c=LinkerOptionCollection",
    "-p", # generate properties
    "-d", # generate delegates
    "-dd=" + os.path.relpath(os.path.join(stdPackageDir, "CommandLineProcessor", "dev", "Scripts", "CommandLineDelegate.cs")),
    "-pv=PrivateData",
    "-l=" + licenseHeaderFile
]
linker_options.extend(extra_args)
(stdout,stderr) = ExecuteProcess(linker_options, True, True)
print stdout

# Archiver options
archiver_options = [
    optionGeneratorExe,
    "-i=" + os.path.relpath(os.path.join(stdPackageDir, "C", "dev", "Scripts", "IArchiverOptions.cs")) + os.pathsep + os.path.relpath(os.path.join(stdPackageDir, "MingwCommon", "dev", "Scripts", "IArchiverOptions.cs")),
    "-n=MingwCommon",
    "-c=ArchiverOptionCollection",
    "-p", # generate properties
    "-d", # generate delegates
    "-dd=" + os.path.relpath(os.path.join(stdPackageDir, "CommandLineProcessor", "dev", "Scripts", "CommandLineDelegate.cs")),
    "-pv=PrivateData",
    "-l=" + licenseHeaderFile
]
archiver_options.extend(extra_args)
(stdout,stderr) = ExecuteProcess(archiver_options, True, True)
print stdout