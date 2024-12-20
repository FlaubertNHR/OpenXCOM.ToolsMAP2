using System;
using System.Reflection;
using System.Runtime.InteropServices;


// General Information about an assembly is controlled through the following
// attributes.
[assembly: AssemblyTitle("DSShared")]
[assembly: AssemblyDescription("Directory Service Shared library")]
#if DEBUG
[assembly: AssemblyConfiguration("debug")]
#else
[assembly: AssemblyConfiguration("release")]
#endif
[assembly: AssemblyCompany("the grays")]
[assembly: AssemblyProduct("DSShared")]
[assembly: AssemblyCopyright("2017-2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to
// COM
[assembly: Guid("cb6f00c2-bdf3-4432-a57a-1982dd251532")]

// The assembly version has following format:
//
// Major.Minor.Build.Revision
//
// You can specify all the values, or the defaults by using '*' for the Build
// and Revision.
[assembly: AssemblyVersion("4.2.3.0")]
[assembly: AssemblyFileVersion("4.2.3.0")]
[assembly: AssemblyInformationalVersion("4.2.3.0")]

// satisfy FxCop:
//[assembly: System.CLSCompliant(true)]

// satisfy FxCop:
[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
