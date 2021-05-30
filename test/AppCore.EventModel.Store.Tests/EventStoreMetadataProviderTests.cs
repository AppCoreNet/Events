// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace AppCore.Events.Storage
{
    public class EventStoreMetadataProviderTests
    {
        [Fact]
        public void GetMetadataAddsPersistentMetadataKey()
        {
            var provider = new EventStoreMetadataProvider();
            var metadata = new Dictionary<string, object>();
            provider.GetMetadata(typeof(TestEvent), metadata);

            metadata.Should()
                    .Contain(EventStoreMetadataKeys.PersistentMetadataKey, true);

            metadata.Should()
                    .Contain(EventStoreMetadataKeys.StreamNameMetadataKey, "test");
        }
    }
}
