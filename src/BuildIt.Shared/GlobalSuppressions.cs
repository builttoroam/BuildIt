// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

// NB: DO NOT add this file to the BuildIt.Shared project itself - this doesn't work with Stylecop.
// Use "Add as Link" for each project you want to use this file
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "Nothing wrong with regions")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1200:Using directives must be placed correctly", Justification = "Moving using inside namespace is dumb")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:Prefix local calls with this", Justification = "this is redundant, don't do this")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File must have header", Justification = "We don't require headers on files, that's what comments on commit are for")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:Single-line comment must be preceded by blank line", Justification = "Disabled because it intefers with resharper/warning disable/restore comments")]