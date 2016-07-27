﻿//---------------------------------------------------------------------
// <copyright file="EdmExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    internal static class EdmExtensionMethods
    {
        /// <summary>
        /// Find the navigation target which is <paramref name="navigationProperty"/> of current <paramref name="navigationSource"/> targets.
        /// </summary>
        /// <param name="navigationSource">The navigation source to find.</param>
        /// <param name="navigationProperty">The navigation property</param>
        /// <param name="matchBindingPath">The function used to determine if the binding path matches.</param>
        /// <returns>The navigation target which matches the binding path.</returns>
        public static IEdmNavigationSource FindNavigationTarget(this IEdmNavigationSource navigationSource, IEdmNavigationProperty navigationProperty, Func<IEdmPathExpression, bool> matchBindingPath)
        {
            Debug.Assert(navigationSource != null);
            Debug.Assert(navigationProperty != null);
            Debug.Assert(matchBindingPath != null);

            if (navigationProperty.ContainsTarget)
            {
                return navigationSource.FindNavigationTarget(navigationProperty);
            }

            IEnumerable<IEdmNavigationPropertyBinding> bindings = navigationSource.FindNavigationPropertyBindings(navigationProperty);

            if (bindings != null)
            {
                foreach (var binding in bindings)
                {
                    if (matchBindingPath(binding.Path))
                    {
                        return binding.Target;
                    }
                }
            }

            return new UnknownEntitySet(navigationSource, navigationProperty);
        }

        /// <summary>
        /// Find the navigation target which is <paramref name="navigationProperty"/> of current <paramref name="navigationSource"/> targets.
        /// The function is specifically used in Uri parser.
        /// </summary>
        /// <typeparam name="T">The element type of parsedSegments to match with binding path.</typeparam>
        /// <param name="navigationSource">The navigation source to find.</param>
        /// <param name="navigationProperty">The navigation property</param>
        /// <param name="matchBindingPath">The function used to determine if the binding path matches.</param>
        /// <param name="parsedSegments">The parsed segments in path, which is used to match binding path.</param>
        /// <param name="bindingPath">The output binding path of the navigation property which matches the <paramref name="parsedSegments"/></param>
        /// <returns>The navigation target which matches the binding path.</returns>
        public static IEdmNavigationSource FindNavigationTarget<T>(this IEdmNavigationSource navigationSource, IEdmNavigationProperty navigationProperty, Func<IEdmPathExpression, List<T>, bool> matchBindingPath, List<T> parsedSegments, out IEdmPathExpression bindingPath)
        {
            Debug.Assert(navigationSource != null);
            Debug.Assert(navigationProperty != null);
            Debug.Assert(matchBindingPath != null);
            Debug.Assert(parsedSegments != null);

            bindingPath = null;

            if (navigationProperty.ContainsTarget)
            {
                return navigationSource.FindNavigationTarget(navigationProperty);
            }

            IEnumerable<IEdmNavigationPropertyBinding> bindings = navigationSource.FindNavigationPropertyBindings(navigationProperty);

            if (bindings != null)
            {
                foreach (var binding in bindings)
                {
                    if (matchBindingPath(binding.Path, parsedSegments))
                    {
                        bindingPath = binding.Path;
                        return binding.Target;
                    }
                }
            }

            return new UnknownEntitySet(navigationSource, navigationProperty);
        }
    }
}
