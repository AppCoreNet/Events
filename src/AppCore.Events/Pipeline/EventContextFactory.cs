// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AppCore.Diagnostics;
using AppCore.Events.Metadata;

namespace AppCore.Events.Pipeline
{
    using EventContextFactoryDelegate = Func<EventDescriptor, IEvent, IEventContext>;

    /// <inheritdoc />
    public class EventContextFactory : IEventContextFactory
    {
        private static readonly Type _eventContextType = typeof(EventContext<>);

        private static readonly ConcurrentDictionary<Type, EventContextFactoryDelegate> _eventContextFactories =
            new ConcurrentDictionary<Type, EventContextFactoryDelegate>();

        private readonly IEventDescriptorFactory _descriptorFactory;

        private static Delegate GetFactoryDelegate(Type type, Type[] argTypes, Type[] passedArgTypes)
        {
            Ensure.Arg.NotNull(type, nameof(type));

            ConstructorInfo constructor = type.GetTypeInfo()
                                              .DeclaredConstructors.FirstOrDefault(
                                                  ci => ci.IsPublic
                                                        && ci.GetParameters()
                                                             .Select(p => p.ParameterType)
                                                             .SequenceEqual(argTypes));

            if (constructor == null)
            {
                throw new NotImplementedException(
                    $"Type '{type.GetDisplayName()}' does not have a constructor with arguments of type "
                    + $"'{string.Join(",", argTypes.Select(t => t.GetDisplayName()))}'.");
            }

            ParameterExpression[] parameters =
                passedArgTypes.Select((t, i) => Expression.Parameter(t, "arg" + i))
                              .ToArray();

            NewExpression body = Expression.New(
                constructor,
                parameters.Select((t, i) => Expression.Convert(parameters[i], argTypes[i])));

            return Expression.Lambda(body, parameters)
                             .Compile();
        }

        private static EventContextFactoryDelegate GetEventContextFactory(Type eventType)
        {
            return _eventContextFactories.GetOrAdd(
                eventType,
                t =>
                {
                    Type eventContextType = _eventContextType.MakeGenericType(t);

                    Delegate factory = GetFactoryDelegate(
                        eventContextType,
                        new[]
                        {
                            typeof(EventDescriptor),
                            eventType
                        },
                        new[]
                        {
                            typeof(EventDescriptor),
                            typeof(IEvent)
                        });

                    return (EventContextFactoryDelegate) factory;
                });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventContextFactory"/> class.
        /// </summary>
        /// <param name="descriptorFactory">The <see cref="IEventDescriptorFactory"/>.</param>
        public EventContextFactory(IEventDescriptorFactory descriptorFactory)
        {
            Ensure.Arg.NotNull(descriptorFactory, nameof(descriptorFactory));
            _descriptorFactory = descriptorFactory;
        }

        /// <inheritdoc />
        public IEventContext CreateContext(IEvent @event)
        {
            Ensure.Arg.NotNull(@event, nameof(@event));

            Type eventType = @event.GetType();
            EventDescriptor descriptor = _descriptorFactory.CreateDescriptor(eventType);
            EventContextFactoryDelegate factory = GetEventContextFactory(eventType);
            return factory(descriptor, @event);
        }
    }
}
