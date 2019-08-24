using System;
using System.Reflection;
using System.Runtime.InteropServices;


// General Information about an assembly is controlled through the following
// attributes.
[assembly: AssemblyTitle("McdView")]
[assembly: AssemblyDescription("McdView executable")]
#if DEBUG
[assembly: AssemblyConfiguration("debug")]
#else
[assembly: AssemblyConfiguration("release")]
#endif
[assembly: AssemblyCompany("the sectoids")]
[assembly: AssemblyProduct("McdView")]
[assembly: AssemblyCopyright("2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("4c71f729-dfeb-4c5c-a056-420f49a83949")]

// The assembly version has following format:
//
// Major.Minor.Build.Revision
//
// You can specify all the values, or the defaults by using '*' for the Build
// and Revision.
[assembly: AssemblyVersion("3.3.0.0")]
[assembly: AssemblyFileVersion("3.3.0.0")]
[assembly: AssemblyInformationalVersion("3.3.0.0")]

// satisfy FxCop:
//[assembly: System.CLSCompliant(true)]

// satisfy FxCop:
[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
