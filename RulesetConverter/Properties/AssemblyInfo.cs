using System;
using System.Reflection;
using System.Runtime.InteropServices;


// General Information about an assembly is controlled through the following
// attributes.
[assembly: AssemblyTitle("RulesetConverter")]
[assembly: AssemblyDescription("Converts OxC ruleset file to MapViewII YAML format.")]
#if DEBUG
[assembly: AssemblyConfiguration("debug")]
#else
[assembly: AssemblyConfiguration("release")]
#endif
[assembly: AssemblyCompany("the mutons")]
[assembly: AssemblyProduct("RulesetConverter")]
[assembly: AssemblyCopyright("2019-2023")]
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
[assembly: AssemblyVersion("1.3.0.1")]
[assembly: AssemblyFileVersion("1.3.0.1")]
[assembly: AssemblyInformationalVersion("1.3.0.1")]

// satisfy FxCop:
//[assembly: System.CLSCompliant(true)]

// satisfy FxCop:
[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
