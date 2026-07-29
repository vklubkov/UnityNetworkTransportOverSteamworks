#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_LOGGING
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING
#endif

#if NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING && NETWORK_TRANSPORT_OVER_STEAMWORKS_ENABLE_TRACING
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING
#endif

using System;
using NetworkTransportOverSteamworks.Debugging;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Networking.Transport;

#if HAS_NETCODE_PACKAGE
using Unity.NetCode;
#endif

namespace NetworkTransportOverSteamworks {
    [BurstCompile]
    public unsafe struct SteamNetworkInterface : INetworkInterface {
        readonly InternalState _state;

        NativeReference<NetworkEndpoint> _endpoint;
        NativeArray<IntPtr> _receivedMessages;

        public NetworkEndpoint LocalEndpoint {
            get {
                try {
                    Trace.NetworkInterface.LocalEndpointGetStart(in _state, in _endpoint);

                    if (_state.IsValid == 0) {
                        Log.NetworkInterface.Invalid(in _state);
                        return default;
                    }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    if (!_endpoint.IsCreated || !_receivedMessages.IsCreated) {
                        Log.NetworkInterface.NotInitialized(in _state);
                        return default;
                    }
#endif

                    return _endpoint.Value;
                }
                finally {
                    Trace.NetworkInterface.LocalEndpointGetEnd(in _state, in _endpoint);
                }
            }
        }

        public SteamNetworkInterface(
            bool isServer,
            IntPtr sockets,
            SteamNetworkInterfaceType interfaceType,
            SteamSendFlags sendFlags = Defaults.SendFlags,
            int messagesPerReceive = Defaults.MessagesPerReceive)
            : this(isServer ? (byte)1 : (byte)0,
                   sockets,
                   interfaceType,
                   sendFlags,
                   messagesPerReceive,
                   hasNetDebug: 0,
                   default) {
            Trace.NetworkInterface.ConstructorWithoutNetDebugStart();
            Trace.NetworkInterface.ConstructorWithoutNetDebugEnd(in _state, in _endpoint);
        }

        public SteamNetworkInterface(
            in NetDebug netDebug,
            bool isServer,
            IntPtr sockets,
            SteamNetworkInterfaceType interfaceType,
            SteamSendFlags sendFlags = Defaults.SendFlags,
            int messagesPerReceive = Defaults.MessagesPerReceive)
            : this(isServer ? (byte)1 : (byte)0,
                   sockets,
                   interfaceType,
                   sendFlags,
                   messagesPerReceive,
                   hasNetDebug: 1,
                   in netDebug) {
            Trace.NetworkInterface.ConstructorWithNetDebugStart(in netDebug);
            Trace.NetworkInterface.ConstructorWithNetDebugEnd(in _state, in _endpoint);
        }

        SteamNetworkInterface(
            byte isServer,
            IntPtr sockets,
            SteamNetworkInterfaceType interfaceType,
            SteamSendFlags sendFlags,
            int messagesPerReceive,
            byte hasNetDebug,
            in NetDebug netDebug) {
            Trace.NetworkInterface.ConstructorPrivateStart(hasNetDebug, in netDebug);

            var state = new InternalState(
                isServer, sockets, interfaceType, sendFlags, messagesPerReceive, hasNetDebug, in netDebug);

            if (sockets == IntPtr.Zero) {
                Log.NetworkInterface.InvalidArgumentSockets(sockets, in state);
                state.IsValid = 0;
            }

            if (!interfaceType.CheckInterfaceType()) {
                Log.NetworkInterface.InvalidArgumentInterfaceType(interfaceType, in state);
                state.IsValid = 0;
            }

            if (!sendFlags.CheckSendFlags()) {
                Log.NetworkInterface.InvalidArgumentSendFlags(sendFlags, in state);
                state.IsValid = 0;
            }

            if (messagesPerReceive <= 0)
                Log.NetworkInterface.InvalidArgumentMessagesPerReceive(messagesPerReceive, in state);

            _state = state;

            _endpoint = default;
            _receivedMessages = default;

            Trace.NetworkInterface.ConstructorPrivateEnd(in _state, in _endpoint);
        }

        public int Initialize(ref NetworkSettings settings, ref int packetPadding) {
            try {
                Trace.NetworkInterface.InitializeStart(in _state, in _endpoint);

                if (_state.IsValid == 0) {
                    Log.NetworkInterface.Invalid(in _state);
                    return -1;
                }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (_endpoint.IsCreated || _receivedMessages.IsCreated) {
                    Log.NetworkInterface.AlreadyInitialized(in _state);
                    return -1;
                }
#endif
                _endpoint = new NativeReference<NetworkEndpoint>(default, Allocator.Persistent);
                _receivedMessages = new NativeArray<IntPtr>(_state.MessagesPerReceive, Allocator.Persistent);

                Log.NetworkInterface.Initialized(in _state);

                return 0;
            }
            finally {
                Trace.NetworkInterface.InitializeEnd(in _state, in _endpoint);
            }
        }

        public void Dispose() {
            try {
                Trace.NetworkInterface.DisposeStart(in _state, in _endpoint);

                if (_state.IsValid == 0) {
                    Log.NetworkInterface.Invalid(in _state);
                    return;
                }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                var isEndpointInitialized = _endpoint.IsCreated;
                if (isEndpointInitialized)
                    _endpoint.Dispose();

                var isReceivedMessagesInitialized = _receivedMessages.IsCreated;
                if (isReceivedMessagesInitialized)
                    _receivedMessages.Dispose();

                if (!isEndpointInitialized || !isReceivedMessagesInitialized)
                    Log.NetworkInterface.NotInitialized(in _state);
#else
                _endpoint.Dispose();
                _receivedMessages.Dispose();
#endif

                Log.NetworkInterface.Disposed(in _state);
            }
            finally {
                Trace.NetworkInterface.DisposeEnd(in _state, in _endpoint);
            }
        }

        public int Bind(NetworkEndpoint endpoint) {
            try {
                Trace.NetworkInterface.BindStart(in _state, in _endpoint);

                if (_state.IsValid == 0) {
                    Log.NetworkInterface.Invalid(in _state);
                    return -1;
                }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (!_endpoint.IsCreated || !_receivedMessages.IsCreated) {
                    Log.NetworkInterface.NotInitialized(in _state);
                    return -1;
                }
#endif

                if (_state.IsServer == 0) {
                    endpoint.GetConnectionInterfaceType(out var interfaceType);

                    if (interfaceType != _state.InterfaceType) {
                        Log.NetworkInterface.NotBound(in _state, in _endpoint);
                        return -1;
                    }

                    _endpoint.Value = endpoint;
                }
                else {
                    endpoint.GetListenSocketsWithPollGroupsAndInterfaceTypes(
                        out var listenSocketP2P,
                        out var pollGroupP2P,
                        out var listenSocketIP,
                        out var pollGroupIP ,
                        out var listenSocketFakeIP,
                        out var pollGroupFakeIP,
                        out var interfaceTypes);

                    NetworkEndpoint listenSocketEndpoint;
                    if ((interfaceTypes & SteamNetworkInterfaceTypeFlags.P2P) ==
                        SteamNetworkInterfaceTypeFlags.P2P &&
                        _state.InterfaceType == SteamNetworkInterfaceType.P2P) {
                        listenSocketP2P.CreateEndpointWithListenSocketPollGroupAndInterfaceType(
                            pollGroupP2P, SteamNetworkInterfaceType.P2P, out listenSocketEndpoint);
                    }
                    else if ((interfaceTypes & SteamNetworkInterfaceTypeFlags.IP) ==
                             SteamNetworkInterfaceTypeFlags.IP &&
                             _state.InterfaceType == SteamNetworkInterfaceType.IP) {
                        listenSocketIP.CreateEndpointWithListenSocketPollGroupAndInterfaceType(
                            pollGroupIP, SteamNetworkInterfaceType.IP, out listenSocketEndpoint);
                    }
                    else if ((interfaceTypes & SteamNetworkInterfaceTypeFlags.FakeIP) ==
                             SteamNetworkInterfaceTypeFlags.FakeIP &&
                             _state.InterfaceType == SteamNetworkInterfaceType.FakeIP) {
                        listenSocketFakeIP.CreateEndpointWithListenSocketPollGroupAndInterfaceType(
                            pollGroupFakeIP, SteamNetworkInterfaceType.FakeIP, out listenSocketEndpoint);
                    }
                    else {
                        Log.NetworkInterface.NotBound(in _state, in _endpoint);
                        return -1;
                    }

                    _endpoint.Value = listenSocketEndpoint;
                }

                Log.NetworkInterface.Bound(in _state, in _endpoint);

                return 0;
            }
            finally {
                Trace.NetworkInterface.BindEnd(in _state, in _endpoint);
            }
        }

        public int Listen() {
            try {
                Trace.NetworkInterface.ListenStart(in _state, in _endpoint);

                if (_state.IsValid == 0) {
                    Log.NetworkInterface.Invalid(in _state);
                    return -1;
                }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (!_endpoint.IsCreated || !_receivedMessages.IsCreated) {
                    Log.NetworkInterface.NotInitialized(in _state);
                    return -1;
                }
#endif

                if (_state.IsServer == 0) {
                    Log.NetworkInterface.NotServer(in _state, in _endpoint);
                    return -1;
                }

                Log.NetworkInterface.Listening(in _state, in _endpoint);

                return 0;
            }
            finally {
                Trace.NetworkInterface.ListenEnd(in _state, in _endpoint);
            }
        }

        [BurstCompile]
        public JobHandle ScheduleReceive(ref ReceiveJobArguments arguments, JobHandle dep) {
            try {
                Trace.NetworkInterface.ScheduleReceiveStart(in _state, in _endpoint);

                if (_state.IsValid == 0) {
                    Log.NetworkInterface.Invalid(in _state);
                    return dep;
                }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (!_endpoint.IsCreated || !_receivedMessages.IsCreated) {
                    Log.NetworkInterface.NotInitialized(in _state);
                    return dep;
                }
#endif
                var sockets = (void*)_state.Sockets;

                var receivedMessagesLength = _receivedMessages.Length;
                var receivedMessagesPtr = (SteamAPI.SteamNetworkingMessage**)_receivedMessages.GetUnsafePtr();

                if (_state.IsServer != 0) {
                    _endpoint.Value.GetListenSocketWithPollGroupAndInterfaceType(
                        out var listenSocket, out var pollGroup, out var interfaceType);

                    if (listenSocket == 0) {
                        Log.NetworkInterface.NoListenSocket(in _state);
                        return dep;
                    }

                    // On server, we can only read messages from the poll group
                    if (pollGroup == 0) {
                        Log.NetworkInterface.NoPollGroup(in _state);
                        return dep;
                    }

                    var receivedMessagesCount = SteamAPI.ISteamNetworkingSockets.ReceiveMessagesOnPollGroup(
                        sockets, pollGroup, receivedMessagesPtr, receivedMessagesLength);

                    if (receivedMessagesCount <= 0)
                        return dep;

                    var job = new ServerReceiveJob {
                        ReceiveQueue = arguments.ReceiveQueue,
                        Messages = _receivedMessages,
                        Count = receivedMessagesCount,
                        PollGroup = pollGroup,
                        InterfaceType = interfaceType,
                    };

                    return job.Schedule(dep);
                }
                else {
                    _endpoint.Value.GetConnectionWithPollGroup(out var connection, out var pollGroup);

                    int receivedMessagesCount;

                    // On client, we can read messages either from the poll group or from the connection
                    if (pollGroup == 0) {
                        if (connection == 0) {
                            Log.NetworkInterface.NoConnectionAndNoPollGroup(in _state);
                            return dep;
                        }

                        receivedMessagesCount = SteamAPI.ISteamNetworkingSockets.ReceiveMessagesOnConnection(
                            sockets, connection, receivedMessagesPtr, receivedMessagesLength);
                    }
                    else {
                        receivedMessagesCount = SteamAPI.ISteamNetworkingSockets.ReceiveMessagesOnPollGroup(
                            sockets, pollGroup, receivedMessagesPtr, receivedMessagesLength);
                    }

                    if (receivedMessagesCount <= 0)
                        return dep;

                    var job = new ClientReceiveJob {
                        ReceiveQueue = arguments.ReceiveQueue,
                        Endpoint = _endpoint.Value,
                        Messages = _receivedMessages,
                        Count = receivedMessagesCount
                    };

                    return job.Schedule(dep);
                }
            }
            finally {
                Trace.NetworkInterface.ScheduleReceiveEnd(in _state, in _endpoint);
            }
        }

        [BurstCompile]
        public JobHandle ScheduleSend(ref SendJobArguments arguments, JobHandle dep) {
            try {
                Trace.NetworkInterface.ScheduleSendStart(in _state, in _endpoint);

                if (_state.IsValid == 0) {
                    Log.NetworkInterface.Invalid(in _state);
                    return dep;
                }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (!_endpoint.IsCreated || !_receivedMessages.IsCreated) {
                    Log.NetworkInterface.NotInitialized(in _state);
                    return dep;
                }
#endif
                if (_state.IsServer != 0) {
                    _endpoint.Value.GetListenSocket(out var listenSocket);
                    if (listenSocket == 0) {
                        Log.NetworkInterface.NoListenSocket(in _state);
                        return dep;
                    }

                    var job = new ServerSendJob {
                        SendQueue = arguments.SendQueue,
                        Sockets = (void*)_state.Sockets,
                        SendFlags = _state.SendFlags,
                    };

                    return job.Schedule(dep);
                }
                else {
                    _endpoint.Value.GetConnection(out var connection);
                    if (connection == 0) {
                        Log.NetworkInterface.NoConnection(in _state);
                        return dep;
                    }

                    var job = new ClientSendJob {
                        SendQueue = arguments.SendQueue,
                        Sockets = (void*)_state.Sockets,
                        Connection = connection,
                        SendFlags = _state.SendFlags
                    };

                    return job.Schedule(dep);
                }
            }
            finally {
                Trace.NetworkInterface.ScheduleSendEnd(in _state, in _endpoint);
            }
        }
    }
}

#endif