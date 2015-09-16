#!/usr/bin/python

import fileinput
import fnmatch
from optparse import OptionParser
import os
import platform
import re
import shutil
import subprocess
import sys
import tarfile
import tempfile
import time

filesToDelete=[\
".gitignore"
]

dirsToDelete=[\
"codingtools",
"packages/Clang-3.1",
"packages/Clang-3.3",
"packages/Clang-Apple503",
"packages/Clang-Apple600",
"packages/ComposerXE-12",
"packages/ComposerXECommon",
"packages/Gcc-4.0",
"packages/Gcc-4.1",
"packages/Gcc-4.4",
"packages/Gcc-4.5",
"packages/Gcc-4.6",
"packages/Mingw-3.4.5",
"packages/Mingw-4.5.0",
"packages/QMakeBuilder",
"packages/VisualC-8.0",
"packages/VisualC-9.0",
"packages/VisualC-10.0",
"packages/VisualC-11.0",
"packages/WindowsSDK-6.0A",
"packages/WindowsSDK-7.1",
"packages/XmlUtilities"
]
# TODO remove .git


def CloneBuildAMation(dir):
    subprocess.check_call(["git", "clone", "https://github.com/markfinal/BuildAMation", dir])
    print >>sys.stdout, "Cloning complete"
    sys.stdout.flush()


def CleanClone():
    print >>sys.stdout, "Waited before cleaning"
    sys.stdout.flush()
    time.sleep(5)
    for file in filesToDelete:
        print >>sys.stdout, "Deleting file %s" % file
        sys.stdout.flush()
        os.remove(file)
    for directory in dirsToDelete:
        print >>sys.stdout, "Deleting directory %s" % directory
        sys.stdout.flush()
        shutil.rmtree(directory)


def UpdateVersionNumbers(version):
    for root, dirnames, filenames in os.walk(os.getcwd()):
        for filename in fnmatch.filter(filenames, "AssemblyInfo.cs"):
            assemblyInfoPath = os.path.join(root, filename)
            print >>sys.stdout, assemblyInfoPath
            sys.stdout.flush()
            for line in fileinput.input(os.path.join(root, filename), inplace=1):#, backup='.bk'):
                line = re.sub('AssemblyInformationalVersion\("[0-9.]+"\)', 'AssemblyInformationalVersion("%s")'%version, line.rstrip())
                print line


def Build():
    print >>sys.stdout, "Starting build in %s" % os.getcwd()
    sys.stdout.flush()
    if platform.system() == "Windows":
        print >>sys.stdout, "WARNING: build disabled on Windows"
        sys.stdout.flush()
    elif platform.system() == "Darwin":
        subprocess.check_call([r"/Applications/Xamarin Studio.app/Contents/MacOS/mdtool", "build", "--target:Build", "--configuration:Release", "BuildAMation.sln"])
    elif platform.system() == "Linux":
        subprocess.check_call(["mdtool", "build", "--target:Build", "--configuration:Release", "BuildAMation.sln"])
    else:
        raise RuntimeError("Unrecognized platform, %s" % platform.system())
    print >>sys.stdout, "Finished build"
    sys.stdout.flush()


def MakeDistribution(version):
    coDir, bamDir = os.path.split(os.getcwd())
    tarPath = os.path.join(coDir, "BuildAMation-%s-binary.tgz"%version)
    print >>sys.stdout, "Writing tar file %s" % tarPath
    sys.stdout.flush()
    os.chdir(coDir)
    with tarfile.open(tarPath, "w:gz") as tar:
        if os.path.isdir(os.path.join(bamDir, "bin")):
          tar.add(os.path.join(bamDir, "bin"))
        tar.add(os.path.join(bamDir, "Changelog.txt"))
        tar.add(os.path.join(bamDir, "env.bat"))
        tar.add(os.path.join(bamDir, "env.sh"))
        tar.add(os.path.join(bamDir, "License.md"))
        tar.add(os.path.join(bamDir, "packages"))
        tar.add(os.path.join(bamDir, "tests"))
    print >>sys.stdout, "Finished writing tar file %s" % tarPath
    sys.stdout.flush()


def Main(dir, version):
    print >>sys.stdout, "Creating BuildAMation version %s" % version
    sys.stdout.flush()
    CloneBuildAMation(dir)
    cwd = os.getcwd()
    try:
        os.chdir(dir)
        CleanClone()
        UpdateVersionNumbers(version)
        Build()
        MakeDistribution(version)
    finally:
        os.chdir(cwd)


if __name__ == "__main__":
    parser = OptionParser()
    parser.add_option("-v", "--version", dest="version", help="Version to create")
    (options, args) = parser.parse_args()
    if not options.version:
        parser.error("Must supply a version")
        
    tempDir = tempfile.mkdtemp()
    cloningDir = os.path.join(tempDir, "BuildAMation-%s" % options.version)
    os.makedirs(cloningDir)
    #cloningDir = r"c:\users\mark\appdata\local\temp\tmpg4tul0"
    try:
        Main(cloningDir, options.version)
    except Exception, e:
        print >>sys.stdout, "Failed because %s" % str(e)
        sys.stdout.flush()
    finally:
        print >>sys.stdout, "Sleeping"
        sys.stdout.flush()
        time.sleep(5)
        print >>sys.stdout, "Deleting clone"
        sys.stdout.flush()
        #shutil.rmtree(cloningDir)
    print >>sys.stdout, "Done"
    sys.stdout.flush()
