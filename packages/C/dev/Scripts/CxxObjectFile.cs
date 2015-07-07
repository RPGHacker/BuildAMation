#region License
// Copyright 2010-2015 Mark Final
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
#endregion // License
namespace C.Cxx
{
namespace V2
{
namespace DefaultSettings
{
    public static partial class DefaultSettingsExtensions
    {
        public static void Defaults(this C.V2.ICxxOnlyCompilerOptions settings, Bam.Core.V2.Module module)
        {
            settings.ExceptionHandler = C.Cxx.EExceptionHandler.Disabled;
            // TODO: separate C language standards from C++
            (settings as C.V2.ICommonCompilerOptions).LanguageStandard = ELanguageStandard.Cxx98;
        }
        public static void Empty(this C.V2.ICxxOnlyCompilerOptions settings)
        {
            settings.ExceptionHandler = null;
        }
    }
}
    public class ObjectFile :
        C.V2.ObjectFile
    {
        public ObjectFile()
        {
            this.Compiler = C.V2.DefaultToolchain.Cxx_Compiler;
        }
    }
}
    /// <summary>
    /// C++ object file
    /// </summary>
    [Bam.Core.ModuleToolAssignment(typeof(ICxxCompilerTool))]
    public class ObjectFile :
        C.ObjectFile
    {}
}
