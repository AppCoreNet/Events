// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace AppCore.EventModel.Metadata
{
    public class EventDescriptorFactoryTests
    {
        [Fact]
        public void CreatesEventDescriptor()
        {
            var factory = new EventDescriptorFactory(Enumerable.Empty<IEventMetadataProvider>());
            EventDescriptor descriptor = factory.CreateDescriptor(typeof(TestEvent));

            descriptor.EventType.Should()
                      .Be<TestEvent>();
        }

        [Fact]
        public void PopulatesEventDescriptorWithMetadata()
        {
            var provider1 = Substitute.For<IEventMetadataProvider>();
            provider1.When(p => p.GetMetadata(Arg.Any<Type>(), Arg.Any<IDictionary<string, object>>()))
                     .Do(
                         ci =>
                         {
                             var metadata = ci.ArgAt<IDictionary<string, object>>(1);
                             metadata.Add("1", 1);
                         });

            var provider2 = Substitute.For<IEventMetadataProvider>();
            provider2.When(p => p.GetMetadata(Arg.Any<Type>(), Arg.Any<IDictionary<string, object>>()))
                     .Do(
                         ci =>
                         {
                             var metadata = ci.ArgAt<IDictionary<string, object>>(1);
                             metadata.Add("2", 2);
                         });

            var factory = new EventDescriptorFactory(
                new[]
                {
                    provider1,
                    provider2
                });

            EventDescriptor descriptor = factory.CreateDescriptor(typeof(TestEvent));
            descriptor.Metadata.Should()
                      .BeEquivalentTo(
                          new KeyValuePair<string, object>("1", 1),
                          new KeyValuePair<string, object>("2", 2));
        }
    }
}
