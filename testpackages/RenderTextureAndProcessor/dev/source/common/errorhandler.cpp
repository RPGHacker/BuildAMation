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
#include "errorhandler.h"
#include <Windows.h>
#include <cstdio>
#include <cstdarg>

#if defined(_MSC_VER)
#define SPRINTF sprintf_s
#define VSPRINTF vsprintf_s
#else
#define SPRINTF sprintf
#define VSPRINTF vsprintf
#endif

void ErrorHandler::Report(const char *file, int line, const char *message, ...)
{
    char buffer[1024];
    SPRINTF(buffer, "%s(%d) : %s\n", file, line, message);

    va_list list;
    va_start(list, message);
    if (::IsDebuggerPresent())
    {
        char buffer2[1024];
        VSPRINTF(buffer2, buffer, list);
        ::OutputDebugString(buffer2);
    }
    else
    {
        vfprintf(stderr, buffer, list);
    }
    va_end(list);
}

void ErrorHandler::ReportWin32Error(const char *file, int line, const void *hModule, const char *message, int errorCode)
{
    //translate the error code into a message
    ::DWORD luFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_MAX_WIDTH_MASK;
    if (0 != hModule)
    {
        luFlags |= FORMAT_MESSAGE_FROM_HMODULE;
    }

    void *lpText;
    DWORD count = ::FormatMessage(
        luFlags,
        hModule,
        errorCode,
        MAKELANGID (LANG_NEUTRAL, SUBLANG_DEFAULT),
        (LPTSTR) &lpText,
        0,
        NULL);
    if (0 == count)
    {
        REPORTERROR("Format message failed reporting a Win32 error");
        return;
    }

    Report(file, line, message, errorCode, lpText);

    //free buffer
    ::LocalFree(lpText);
}
