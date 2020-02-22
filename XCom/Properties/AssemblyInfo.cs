using System;
using System.Reflection;
using System.Runtime.InteropServices;


// General Information about an assembly is controlled through the following
// attributes.
[assembly: AssemblyTitle("XCom")]
[assembly: AssemblyDescription("XCom resource interpreter library")]
#if DEBUG
[assembly: AssemblyConfiguration("debug")]
#else
[assembly: AssemblyConfiguration("release")]
#endif
[assembly: AssemblyCompany("the grays")]
[assembly: AssemblyProduct("XCom")]
[assembly: AssemblyCopyright("2017-2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a1790457-d476-4b72-9e2e-95d387a97667")]

// The assembly version has following format:
//
// Major.Minor.Build.Revision
//
// You can specify all the values, or the defaults by using '*' for the Build
// and Revision.
[assembly: AssemblyVersion("3.5.1.0")]
[assembly: AssemblyFileVersion("3.5.1.0")]
[assembly: AssemblyInformationalVersion("3.5.1.0")]

// satisfy FxCop:
//[assembly: System.CLSCompliant(true)]

// satisfy FxCop:
[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
