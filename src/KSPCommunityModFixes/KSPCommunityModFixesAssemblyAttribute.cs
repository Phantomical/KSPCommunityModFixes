using System;

namespace KSPCommunityModFixes;

/// <summary>
/// This attribute is used to mark assemblies that contains KSPCMF fixes.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
internal sealed class KSPCommunityModFixesAssemblyAttribute : Attribute { }
