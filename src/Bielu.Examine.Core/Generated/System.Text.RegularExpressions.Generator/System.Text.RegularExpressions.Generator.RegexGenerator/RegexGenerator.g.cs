﻿// <auto-generated/>
#nullable enable
#pragma warning disable CS0162 // Unreachable code
#pragma warning disable CS0164 // Unreferenced label
#pragma warning disable CS0219 // Variable assigned but never used

namespace Bielu.Examine.Core.Regex
{
    partial class QueryRegex
    {
        /// <remarks>
        /// Pattern:<br/>
        /// <code>([:,])-</code><br/>
        /// Explanation:<br/>
        /// <code>
        /// ○ 1st capture group.<br/>
        ///     ○ Match a character in the set [,:].<br/>
        /// ○ Match '-'.<br/>
        /// </code>
        /// </remarks>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Text.RegularExpressions.Generator", "8.0.9.3103")]
        public static partial global::System.Text.RegularExpressions.Regex PathRegex() => global::System.Text.RegularExpressions.Generated.PathRegex_0.Instance;
    }
}

namespace System.Text.RegularExpressions.Generated
{
    using System;
    using System.Buffers;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>Custom <see cref="Regex"/>-derived type for the PathRegex method.</summary>
    [GeneratedCodeAttribute("System.Text.RegularExpressions.Generator", "8.0.9.3103")]
    file sealed class PathRegex_0 : Regex
    {
        /// <summary>Cached, thread-safe singleton instance.</summary>
        internal static readonly PathRegex_0 Instance = new();
    
        /// <summary>Initializes the instance.</summary>
        private PathRegex_0()
        {
            base.pattern = "([:,])-";
            base.roptions = RegexOptions.None;
            ValidateMatchTimeout(Utilities.s_defaultTimeout);
            base.internalMatchTimeout = Utilities.s_defaultTimeout;
            base.factory = new RunnerFactory();
            base.capsize = 2;
        }
    
        /// <summary>Provides a factory for creating <see cref="RegexRunner"/> instances to be used by methods on <see cref="Regex"/>.</summary>
        private sealed class RunnerFactory : RegexRunnerFactory
        {
            /// <summary>Creates an instance of a <see cref="RegexRunner"/> used by methods on <see cref="Regex"/>.</summary>
            protected override RegexRunner CreateInstance() => new Runner();
        
            /// <summary>Provides the runner that contains the custom logic implementing the specified regular expression.</summary>
            private sealed class Runner : RegexRunner
            {
                /// <summary>Scan the <paramref name="inputSpan"/> starting from base.runtextstart for the next match.</summary>
                /// <param name="inputSpan">The text being scanned by the regular expression.</param>
                protected override void Scan(ReadOnlySpan<char> inputSpan)
                {
                    // Search until we can't find a valid starting position, we find a match, or we reach the end of the input.
                    while (TryFindNextPossibleStartingPosition(inputSpan) &&
                           !TryMatchAtCurrentPosition(inputSpan) &&
                           base.runtextpos != inputSpan.Length)
                    {
                        base.runtextpos++;
                        if (Utilities.s_hasTimeout)
                        {
                            base.CheckTimeout();
                        }
                    }
                }
        
                /// <summary>Search <paramref name="inputSpan"/> starting from base.runtextpos for the next location a match could possibly start.</summary>
                /// <param name="inputSpan">The text being scanned by the regular expression.</param>
                /// <returns>true if a possible match was found; false if no more matches are possible.</returns>
                private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
                {
                    int pos = base.runtextpos;
                    char ch;
                    
                    // Any possible match is at least 2 characters.
                    if (pos <= inputSpan.Length - 2)
                    {
                        // The pattern matches a character in the set - at index 1.
                        // Find the next occurrence. If it can't be found, there's no match.
                        ReadOnlySpan<char> span = inputSpan.Slice(pos);
                        for (int i = 0; i < span.Length - 1; i++)
                        {
                            int indexOfPos = span.Slice(i + 1).IndexOf('-');
                            if (indexOfPos < 0)
                            {
                                goto NoMatchFound;
                            }
                            i += indexOfPos;
                            
                            if ((((ch = span[i]) == ',') | (ch == ':')))
                            {
                                base.runtextpos = pos + i;
                                return true;
                            }
                        }
                    }
                    
                    // No match found.
                    NoMatchFound:
                    base.runtextpos = inputSpan.Length;
                    return false;
                }
        
                /// <summary>Determine whether <paramref name="inputSpan"/> at base.runtextpos is a match for the regular expression.</summary>
                /// <param name="inputSpan">The text being scanned by the regular expression.</param>
                /// <returns>true if the regular expression matches at the current position; otherwise, false.</returns>
                private bool TryMatchAtCurrentPosition(ReadOnlySpan<char> inputSpan)
                {
                    int pos = base.runtextpos;
                    int matchStart = pos;
                    char ch;
                    int capture_starting_pos = 0;
                    ReadOnlySpan<char> slice = inputSpan.Slice(pos);
                    
                    // 1st capture group.
                    {
                        capture_starting_pos = pos;
                        
                        // Match a character in the set [,:].
                        if (slice.IsEmpty || (((ch = slice[0]) != ',') & (ch != ':')))
                        {
                            UncaptureUntil(0);
                            return false; // The input didn't match.
                        }
                        
                        pos++;
                        slice = inputSpan.Slice(pos);
                        base.Capture(1, capture_starting_pos, pos);
                    }
                    
                    // Match '-'.
                    if (slice.IsEmpty || slice[0] != '-')
                    {
                        UncaptureUntil(0);
                        return false; // The input didn't match.
                    }
                    
                    // The input matched.
                    pos++;
                    base.runtextpos = pos;
                    base.Capture(0, matchStart, pos);
                    return true;
                    
                    // <summary>Undo captures until it reaches the specified capture position.</summary>
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    void UncaptureUntil(int capturePosition)
                    {
                        while (base.Crawlpos() > capturePosition)
                        {
                            base.Uncapture();
                        }
                    }
                }
            }
        }

    }
    
    /// <summary>Helper methods used by generated <see cref="Regex"/>-derived implementations.</summary>
    [GeneratedCodeAttribute("System.Text.RegularExpressions.Generator", "8.0.9.3103")]
    file static class Utilities
    {
        /// <summary>Default timeout value set in <see cref="AppContext"/>, or <see cref="Regex.InfiniteMatchTimeout"/> if none was set.</summary>
        internal static readonly TimeSpan s_defaultTimeout = AppContext.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") is TimeSpan timeout ? timeout : Regex.InfiniteMatchTimeout;
        
        /// <summary>Whether <see cref="s_defaultTimeout"/> is non-infinite.</summary>
        internal static readonly bool s_hasTimeout = s_defaultTimeout != Regex.InfiniteMatchTimeout;
    }
}