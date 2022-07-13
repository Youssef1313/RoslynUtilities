using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MS.CA.Utilities.Tests
{
    internal static class AssertEx
    {
        public static void NotNull([NotNull] object? @object)
        {
            Assert.NotNull(@object);
            AssertCondition(@object is not null);
        }

        private static void AssertCondition([DoesNotReturnIf(false)] bool condition)
        {
            Debug.Assert(condition);
        }
    }
}

