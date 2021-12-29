// -----------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeNameK.Core.Utilities
{
    /// <summary>
    /// Extension class for IEnumerable<T>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Converts null to Enumerable.Empty{T}. The intention for this method is to simplify the caller by avoid null check for IEnumerable{T}.
        /// </summary>
        /// <typeparam name="T">The type to assign to the type parameter of the returned generic System.Collections.Generic.IEnumerable{T}.</typeparam>
        /// <param name="original">The original enumerable object. This could be null.</param>
        /// <returns>Returns the original IEnumerable{T} when it is not null. Or Enumerable.Empty{T} if it is null.</returns>
        /// <remarks>
        /// It is still preferred if the callee can return Enumerable.Empty{T}() than null.
        /// </remarks>
        public static IEnumerable<T> NullAsEmpty<T>(this IEnumerable<T>? original)
            => original ?? Enumerable.Empty<T>();

        /// <summary>
        ///  Executes an async version of foreach on a System.Collections.IEnumerable in which iterations may run in parallel.
        ///  The body delegate passed in will be executed on each iteration.
        /// </summary>
        /// <typeparam name="T"> The type of the elements in source.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="degreeOfParallelism">How many parallel tasks to execute.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <returns></returns>
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int degreeOfParallelism, Func<T, Task> body)
        {
            return Task.WhenAll(Partitioner.Create(source).GetPartitions(degreeOfParallelism).Select(
                partition =>
                {
                    return Task.Run(async () =>
                    {
                        using (partition)
                        {
                            while (partition.MoveNext())
                            {
                                await body(partition.Current).ConfigureAwait(false);
                            }
                        }
                    });
                }
            ));
        }
    }
}
