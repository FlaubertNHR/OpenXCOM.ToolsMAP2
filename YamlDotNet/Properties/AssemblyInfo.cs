// This file is part of YamlDotNet - A .NET library for YAML.
// Copyright (c) Antoine Aubry and contributors
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#if !UNITY
using System.Reflection;
using System.Runtime.InteropServices;
using System;


// General Information about an assembly is controlled through the following
// attributes.
[assembly: AssemblyTitle("YamlDotNet")]
[assembly: AssemblyDescription("The YamlDotNet library.")]
#if DEBUG
[assembly: AssemblyConfiguration("debug")]
#else
[assembly: AssemblyConfiguration("release")]
#endif
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("YamlDotNet")]
[assembly: AssemblyCopyright("\u00A9 Antoine Aubry and contributors 2008, 2009, 2010, 2011, 2012, 2013, 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// The assembly version has following format:
//
// Major.Minor.Build.Revision
//
// You can specify all the values, or the defaults by using '*' for the Build
// and Revision.
[assembly: AssemblyVersion("0.0.1.0")]
[assembly: AssemblyFileVersion("0.0.1.0")]
[assembly: AssemblyInformationalVersion("0.0.1.0")]

[assembly: CLSCompliant(true)]

#if !SIGNED
	#if PORTABLE
//kL_edit[assembly: InternalsVisibleTo("YamlDotNet.Test.Portable")]
	#else
//kL_edit[assembly: InternalsVisibleTo("YamlDotNet.Test")]
	#endif
#endif

#endif
