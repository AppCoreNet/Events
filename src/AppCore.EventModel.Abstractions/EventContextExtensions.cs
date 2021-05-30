// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using AppCore.Diagnostics;

namespace AppCore.EventModel
{
    /// <summary>
    /// Provides extension methods for the <see cref="IEventContext"/>.
    /// </summary>
    public static class EventContextExtensions
    {
        /// <summary>
        /// Adds a feature to the <see cref="IEventContext"/>.
        /// </summary>
        /// <typeparam name="T">The type of the feature.</typeparam>
        /// <param name="context">The <see cref="IEventContext"/>.</param>
        /// <param name="feature">The feature that should be added.</param>
        /// <exception cref="InvalidOperationException">The event context feature is already registered.</exception>
        public static void AddFeature<T>(this IEventContext context, T feature)
        {
            Ensure.Arg.NotNull(context, nameof(context));
            Ensure.Arg.NotNull(feature, nameof(feature));

            try
            {
                context.Features.Add(typeof(T), feature);
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException($"Event context feature {typeof(T).GetDisplayName()} already registered.");
            }
        }

        /// <summary>
        /// Gets the feature with the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the feature.</typeparam>
        /// <param name="context">The <see cref="IEventContext"/>.</param>
        /// <param name="feature">The feature.</param>
        /// <returns><c>true</c> if the feature was found; <c>false</c> otherwise.</returns>
        public static bool TryGetFeature<T>(this IEventContext context, out T feature)
        {
            Ensure.Arg.NotNull(context, nameof(context));

            if (context.Features.TryGetValue(typeof(T), out object tmp))
            {
                feature = (T) tmp;
                return true;
            }

            feature = default;
            return false;
        }

        /// <summary>
        /// Gets the feature with the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the feature.</typeparam>
        /// <param name="context">The <see cref="IEventContext"/>.</param>
        /// <returns>The feature.</returns>
        /// <exception cref="InvalidOperationException">The event context feature is not available.</exception>
        public static T GetFeature<T>(this IEventContext context)
        {
            if (!TryGetFeature(context, out T feature))
                throw new InvalidOperationException($"Event context feature {typeof(T).GetDisplayName()} is not available.");

            return feature;
        }

        /// <summary>
        /// Gets a value indicating whether a feature is available.
        /// </summary>
        /// <typeparam name="T">The type of the feature.</typeparam>
        /// <param name="context">The <see cref="IEventContext"/>.</param>
        /// <returns><c>true</c> if the feature is available; <c>false</c> otherwise.</returns>
        public static bool HasFeature<T>(this IEventContext context)
        {
            Ensure.Arg.NotNull(context, nameof(context));
            return context.Features.ContainsKey(typeof(T));
        }
    }
}
