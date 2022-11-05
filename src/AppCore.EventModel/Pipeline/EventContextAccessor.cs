// Licensed under the MIT License.
// Copyright (c) 2018-2020 the AppCore .NET project.

#if !NETSTANDARD2_0
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#else
using System.Threading;
#endif

namespace AppCore.EventModel.Pipeline
{
    /// <summary>
    /// Default implementation of the <see cref="IEventContextAccessor"/> interface.
    /// </summary>
    public class EventContextAccessor : IEventContextAccessor
    {
#if !NETSTANDARD2_0
        private static readonly string _logicalDataKey = "__EventContext__" + AppDomain.CurrentDomain.Id;

        /// <inheritdoc />
        public IEventContext? EventContext
        {
            get
            {
                var handle = CallContext.LogicalGetData(_logicalDataKey) as ObjectHandle;
                return handle?.Unwrap() as IEventContext;
            }
            set
            {
                CallContext.LogicalSetData(_logicalDataKey, new ObjectHandle(value));
            }
        }
#endif

#if NETSTANDARD2_0
        private readonly AsyncLocal<IEventContext?> _eventContext = new AsyncLocal<IEventContext?>();

        /// <inheritdoc />
        public IEventContext? EventContext
        {
            get => _eventContext.Value;
            set => _eventContext.Value = value;
        }
#endif
    }
}