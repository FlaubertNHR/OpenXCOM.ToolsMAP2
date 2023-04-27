using System;
using System.Reflection;
using System.Runtime.InteropServices;


// General Information about an assembly is controlled through the following
// attributes.
[assembly: AssemblyTitle("ConfigConverter")]
[assembly: AssemblyDescription("Converts MapViewI configuration files to MapViewII YAML format.")]
#if DEBUG
[assembly: AssemblyConfiguration("debug")]
#else
[assembly: AssemblyConfiguration("release")]
#endif
[assembly: AssemblyCompany("the mutons")]
[assembly: AssemblyProduct("ConfigConverter")]
[assembly: AssemblyCopyright("2017-2023")]
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
[assembly: AssemblyVersion("2.2.2.1")]
[assembly: AssemblyFileVersion("2.2.2.1")]
[assembly: AssemblyInformationalVersion("2.2.2.1")]

// satisfy FxCop:
//[assembly: System.CLSCompliant(true)]

// satisfy FxCop:
[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
