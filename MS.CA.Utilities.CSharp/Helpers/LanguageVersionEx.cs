using Microsoft.CodeAnalysis.CSharp;

namespace MS.CA.Utilities.CSharp.Generators
{
    internal class LanguageVersionEx
    {
        /// <summary>
        /// C# language version 9.0
        /// </summary>
        /// <remarks>
        /// <para>Features:</para>
        /// <list type="bullet">
        /// <item><description>Records</description></item>
        /// <item><description>Init only setters</description></item>
        /// <item><description>Top-level statements</description></item>
        /// <item><description>Pattern matching enhancements</description></item>
        /// <item><description>Native sized integers</description></item>
        /// <item><description>Function pointers</description></item>
        /// <item><description>Suppress emitting localsinit flag</description></item>
        /// <item><description>Target-typed new expressions</description></item>
        /// <item><description>Static anonymous functions</description></item>
        /// <item><description>Target-typed conditional expressions</description></item>
        /// <item><description>Covariant return types</description></item>
        /// <item><description>Extension GetEnumerator support for foreach loops</description></item>
        /// <item><description>Lambda discard parameters</description></item>
        /// <item><description>Attributes on local functions</description></item>
        /// <item><description>Module initializers</description></item>
        /// <item><description>New features for partial methods</description></item>
        /// </list>
        /// </remarks>
        public static LanguageVersion CSharp9 = (LanguageVersion)900;

        /// <summary>
        /// C# language version 10.0
        /// </summary>
        /// <remarks>
        /// <para>Features:</para>
        /// <list type="bullet">
        /// <item><description>Record structs</description></item>
        /// <item><description>Global using directives</description></item>
        /// <item><description>Lambda improvements</description></item>
        /// <item><description>Improved definite assignment</description></item>
        /// <item><description>Constant interpolated strings</description></item>
        /// <item><description>Mix declarations and variables in deconstruction</description></item>
        /// <item><description>Extended property patterns</description></item>
        /// <item><description>Sealed record ToString</description></item>
        /// <item><description>Source Generator v2 APIs</description></item>
        /// <item><description>Method-level AsyncMethodBuilder</description></item>
        /// </list>
        /// </remarks>
        public static LanguageVersion CSharp10 = (LanguageVersion)1000;
    }
}
