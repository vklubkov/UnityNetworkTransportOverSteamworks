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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Networking.Transport;
using Debug = UnityEngine.Debug;

#if HAS_NETCODE_PACKAGE
using Unity.NetCode;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NetworkTransportOverSteamworks.Debugging {
    [BurstCompile]
    internal static class Trace {
        static readonly FixedString32Bytes _traceTag = "[TRACE]";
        static readonly FixedString64Bytes _traceTagDark = $"<color=cyan>{_traceTag}</color>";
        static readonly FixedString64Bytes _traceTagLight = $"<color=#008b8b>{_traceTag}</color>";

        static readonly FixedString32Bytes _start = "START";
        static readonly FixedString32Bytes _end = "END";

#if HAS_NETCODE_PACKAGE

        [BurstCompile]
        internal static class NetworkDriverConstructor {
            static readonly FixedString64Bytes _getSteamUserSockets =
                "NetworkDriverConstructor.GetSteamUserSockets()"; // 46

            static readonly FixedString64Bytes _getSteamGameServerSockets =
                "NetworkDriverConstructor.GetSteamGameServerSockets()"; // 52

            static readonly FixedString128Bytes _initializeClient =
                "NetworkDriverConstructor.InitializeClient(IntPtr, SteamNetworkInterfaceType, int, SteamSendFlags, List<World>)"; // 110

            static readonly FixedString128Bytes _initializeServer =
                "NetworkDriverConstructor.InitializeServer(IntPtr, SteamNetworkInterfaceTypeFlags, int, SteamSendFlags, List<World>)"; // 115

            static readonly FixedString512Bytes _initializeClientAndServer =
                "NetworkDriverConstructor.InitializeClientAndServer(IntPtr, SteamNetworkInterfaceType, SteamNetworkInterfaceTypeFlags, int, SteamSendFlags, List<World>)"; // 151

            static readonly FixedString128Bytes _initialize =
                "NetworkDriverConstructor.Initialize(NetworkDriversConfig, List<World>)"; // 70

            static readonly FixedString64Bytes _deinitialize =
                "NetworkDriverConstructor.Deinitialize(List<World>)"; // 50

            static readonly FixedString128Bytes _createClientDriver =
                "NetworkDriverConstructor.CreateClientDriver(World, ref NetworkDriverStore, NetDebug)"; // 84

            static readonly FixedString128Bytes _createServerDriver =
                "NetworkDriverConstructor.CreateServerDriver(World, ref NetworkDriverStore, NetDebug)"; // 84


            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void GetSteamUserSocketsStart() =>
                TraceInternal(in _getSteamUserSockets, in _start);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void GetSteamUserSocketsEnd() =>
                TraceInternal(in _getSteamUserSockets, in _end);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void GetSteamGameServerSocketsStart() =>
                TraceInternal(in _getSteamGameServerSockets, in _start);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void GetSteamGameServerSocketsEnd() =>
                TraceInternal(in _getSteamGameServerSockets, in _end);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void InitializeClientStart() =>
                TraceInternal(in _initializeClient, in _start);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void InitializeClientEnd() =>
                TraceInternal(in _initializeClient, in _end);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void InitializeServerStart() =>
                TraceInternal(in _initializeServer, in _start);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void InitializeServerEnd() =>
                TraceInternal(in _initializeServer, in _end);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void InitializeClientAndServerStart() =>
                TraceInternal(in _initializeClientAndServer, in _start);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void InitializeClientAndServerEnd() =>
                TraceInternal(in _initializeClientAndServer, in _end);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void InitializeStart() =>
                TraceInternal(in _initialize, in _start);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void InitializeEnd() =>
                TraceInternal(in _initialize, in _end);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void DeinitializeStart() =>
                TraceInternal(in _deinitialize, in _start);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void DeinitializeEnd() =>
                TraceInternal(in _deinitialize, in _end);

            [BurstCompile]
            static void TraceInternal(
                in FixedString64Bytes method,
                in FixedString32Bytes position) {
                var message = new FixedString512Bytes();
                message.SafeAppendPackageNameTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendTraceTag();
                message.SafeAppend((byte)' ');
                message.SafeAppend(method);
                message.SafeAppend((byte)' ');
                message.SafeAppend(position);
                message.SafeAppend((byte)'\n');
                Debug.Log(message);
            }

            [BurstCompile]
            static void TraceInternal(
                in FixedString128Bytes method,
                in FixedString32Bytes position) {
                var message = new FixedString512Bytes();
                message.SafeAppendPackageNameTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendTraceTag();
                message.SafeAppend((byte)' ');
                message.SafeAppend(method);
                message.SafeAppend((byte)' ');
                message.SafeAppend(position);
                message.SafeAppend((byte)'\n');
                Debug.Log(message);
            }

            [BurstCompile]
            static void TraceInternal(
                in FixedString512Bytes method,
                in FixedString32Bytes position) {
                var message = new FixedString512Bytes();
                message.SafeAppendPackageNameTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendTraceTag();
                message.SafeAppend((byte)' ');
                message.SafeAppend(method);
                message.SafeAppend((byte)' ');
                message.SafeAppend(position);
                message.SafeAppend((byte)'\n');
                Debug.Log(message);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void CreateClientDriverStart(
                PlayType playType,
                in FixedString512Bytes worldName,
                in NetworkDriverStore networkDriverStore,
                in NetDebug netDebug) =>
                TraceInternal(
                    in _createClientDriver,
                    in _start,
                    playType,
                    isServer: 0,
                    in worldName,
                    in networkDriverStore,
                    in netDebug);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void CreateClientDriverEnd(
                PlayType playType,
                in FixedString512Bytes worldName,
                in NetworkDriverStore networkDriverStore,
                in NetDebug netDebug) =>
                TraceInternal(
                    in _createClientDriver,
                    in _end,
                    playType,
                    isServer: 0,
                    in worldName,
                    in networkDriverStore,
                    in netDebug);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void CreateServerDriverStart(
                PlayType playType,
                in FixedString512Bytes worldName,
                in NetworkDriverStore networkDriverStore,
                in NetDebug netDebug) =>
                TraceInternal(
                    in _createServerDriver,
                    in _start,
                    playType,
                    isServer: 1,
                    in worldName,
                    in networkDriverStore,
                    in netDebug);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void CreateServerDriverEnd(
                PlayType playType,
                in FixedString512Bytes worldName,
                in NetworkDriverStore networkDriverStore,
                in NetDebug netDebug) =>
                TraceInternal(
                    in _createServerDriver,
                    in _end,
                    playType,
                    isServer: 1,
                    in worldName,
                    in networkDriverStore,
                    in netDebug);

            [BurstCompile]
            static void TraceInternal(
                in FixedString128Bytes method,
                in FixedString32Bytes position,
                PlayType playType,
                byte isServer,
                in FixedString512Bytes worldName,
                in NetworkDriverStore networkDriverStore,
                in NetDebug netDebug) {
                var message = new FixedString512Bytes();
                message.SafeAppendPackageNameTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendTraceTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendPlayTypeTag(playType);
                message.SafeAppend((byte)' ');
                message.SafeAppendServerTag(isServer);
                message.SafeAppend((byte)' ');
                message.SafeAppend(method);
                message.SafeAppend((byte)' ');
                message.SafeAppend(position);
                message.SafeAppend((byte)'\n');
                message.SafeAppendWorldName(in worldName);
                message.SafeAppend((byte)'\n');
                message.SafeAppendDriverState(in networkDriverStore);
                message.LogAsTrace(hasNetDebug: 1, in netDebug);
            }
        }

#endif

        [BurstCompile]
        internal static class NetworkInterface {
            static readonly FixedString64Bytes _localEndpoint =
                "NetworkInterface.LocalEndpoint { get; }"; // 39

            static readonly FixedString128Bytes _constructorWithoutNetDebug =
                "NetworkInterface.NetworkInterface(PlayType, bool, IntPtr, SteamNetworkInterfaceType, SteamSendFlags, int)"; // 105

            static readonly FixedString128Bytes _constructorWithNetDebug =
                "NetworkInterface.NetworkInterface(PlayType, bool, IntPtr, SteamNetworkInterfaceType, SteamSendFlags, int, in NetDebug)"; // 118

            static readonly FixedString128Bytes _constructorPrivate =
                "NetworkInterface.NetworkInterface(PlayType, byte, IntPtr, SteamNetworkInterfaceType, SteamSendFlags, int, byte, in NetDebug)"; // 124

            static readonly FixedString64Bytes _initialize =
                "NetworkInterface.Initialize(ref NetworkSettings, ref int)"; // 57

            static readonly FixedString32Bytes _dispose =
                "NetworkInterface.Dispose()"; // 26

            static readonly FixedString64Bytes _bind =
                "NetworkInterface.Bind(NetworkEndpoint)"; // 38

            static readonly FixedString32Bytes _listen =
                "NetworkInterface.Listen()"; // 25

            static readonly FixedString128Bytes _scheduleReceive =
                "NetworkInterface.ScheduleReceive(ref ReceiveJobArguments, JobHandle)"; // 68

            static readonly FixedString128Bytes _scheduleSend =
                "NetworkInterface.ScheduleSend(ref SendJobArguments, JobHandle)"; // 62

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void LocalEndpointGetStart(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _localEndpoint, in _start, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void LocalEndpointGetEnd(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _localEndpoint, in _end, in state, in endpoint);

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ConstructorWithoutNetDebugStart() {
                var message = new FixedString512Bytes();
                message.SafeAppendPackageNameTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendTraceTag();
                message.SafeAppend((byte)' ');
                message.SafeAppend(_constructorWithoutNetDebug);
                message.SafeAppend((byte)' ');
                message.SafeAppend(_start);
                Debug.Log(message);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ConstructorWithoutNetDebugEnd(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _constructorWithoutNetDebug, in _end, in state, in endpoint);

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ConstructorWithNetDebugStart(in NetDebug netDebug) {
                var message = new FixedString512Bytes();
                message.SafeAppendPackageNameTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendTraceTag();
                message.SafeAppend((byte)' ');
                message.SafeAppend(_constructorWithNetDebug);
                message.SafeAppend((byte)' ');
                message.SafeAppend(_start);
                message.LogAsTrace(hasNetDebug: 1, in netDebug);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ConstructorWithNetDebugEnd(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _constructorWithNetDebug, in _end, in state, in endpoint);

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ConstructorPrivateStart(byte hasNetDebug, in NetDebug netDebug) {
                var message = new FixedString512Bytes();
                message.SafeAppendPackageNameTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendTraceTag();
                message.SafeAppend((byte)' ');
                message.SafeAppend(_constructorPrivate);
                message.SafeAppend((byte)' ');
                message.SafeAppend(_start);
                message.LogAsTrace(hasNetDebug, in netDebug);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ConstructorPrivateEnd(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _constructorPrivate, in _end, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void InitializeStart(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _initialize, in _start, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void InitializeEnd(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _initialize, in _end, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void DisposeStart(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _dispose, in _start, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void DisposeEnd(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _dispose, in _end, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void BindStart(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _bind, in _start, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void BindEnd(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _bind, in _end, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ListenStart(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _listen, in _start, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ListenEnd(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _listen, in _end, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ScheduleReceiveStart(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _scheduleReceive, in _start, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ScheduleReceiveEnd(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _scheduleReceive, in _end, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ScheduleSendStart(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _scheduleSend, in _start, in state, in endpoint);

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING")]
            public static void ScheduleSendEnd(
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) =>
                TraceInternal(in _scheduleSend, in _end, in state, in endpoint);

            [BurstCompile]
            static void TraceInternal(
                in FixedString32Bytes method,
                in FixedString32Bytes position,
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) {
                var message = new FixedString512Bytes();
                message.SafeAppendPackageNameTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendTraceTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendServerTag(state.IsServer);
                message.SafeAppend((byte)' ');
                message.SafeAppendInterfaceTypeTag(state.InterfaceType);
                message.SafeAppend((byte)' ');
                message.SafeAppend(method);
                message.SafeAppend((byte)' ');
                message.SafeAppend(position);
                message.SafeAppend((byte)'\n');
                message.SafeAppendNetworkInterfaceState(in state);
                message.SafeAppend((byte)'\n');
                message.SafeAppendNetworkEndpoint(state.IsServer, in endpoint);
                message.LogAsTrace(state.HasNetDebug, in state.NetDebug);
            }

            [BurstCompile]
            static void TraceInternal(
                in FixedString64Bytes method,
                in FixedString32Bytes position,
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) {
                var message = new FixedString512Bytes();
                message.SafeAppendPackageNameTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendTraceTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendServerTag(state.IsServer);
                message.SafeAppend((byte)' ');
                message.SafeAppendInterfaceTypeTag(state.InterfaceType);
                message.SafeAppend((byte)' ');
                message.SafeAppend(method);
                message.SafeAppend((byte)' ');
                message.SafeAppend(position);
                message.SafeAppend((byte)'\n');
                message.SafeAppendNetworkInterfaceState(in state);
                message.SafeAppend((byte)'\n');
                message.SafeAppendNetworkEndpoint(state.IsServer, in endpoint);
                message.LogAsTrace(state.HasNetDebug, in state.NetDebug);
            }

            [BurstCompile]
            static void TraceInternal(
                in FixedString128Bytes method,
                in FixedString32Bytes position,
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) {
                var message = new FixedString512Bytes();
                message.SafeAppendPackageNameTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendTraceTag();
                message.SafeAppend((byte)' ');
                message.SafeAppendServerTag(state.IsServer);
                message.SafeAppend((byte)' ');
                message.SafeAppendInterfaceTypeTag(state.InterfaceType);
                message.SafeAppend((byte)' ');
                message.SafeAppend(method);
                message.SafeAppend((byte)' ');
                message.SafeAppend(position);
                message.SafeAppend((byte)'\n');
                message.SafeAppendNetworkInterfaceState(in state);
                message.SafeAppend((byte)'\n');
                message.SafeAppendNetworkEndpoint(state.IsServer, in endpoint);
                message.LogAsTrace(state.HasNetDebug, in state.NetDebug);
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SafeAppendTraceTag(this ref FixedString512Bytes message) {
#if !UNITY_EDITOR || NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_RICH_LOGS
            var traceTag = _traceTag;
#else
            var traceTag = EditorGUIUtility.isProSkin
                ? _traceTagDark
                : _traceTagLight;
#endif
            message.SafeAppend(in traceTag);
        }

#if HAS_NETCODE_PACKAGE

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SafeAppendDriverState(
            this ref FixedString512Bytes message,
            in NetworkDriverStore networkDriverStore) {
            var isInitialized = networkDriverStore.IsCreated;
            FixedString32Bytes initializedState = isInitialized ? "initialized" : "not initialized"; // 15
            FixedString32Bytes initialized = $"Driver: {initializedState}"; // 8 + 15 = 23
            message.SafeAppend(initialized);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SafeAppendWorldName(this ref FixedString512Bytes message, in FixedString512Bytes worldName) {
            var world = new FixedString512Bytes();
            FixedString32Bytes prefix = "World: "; // 7
            world.SafeAppend(prefix);
            world.SafeAppend(worldName);
            message.SafeAppend(world);
        }

#endif

        [BurstCompile]
        static void SafeAppendNetworkInterfaceState(
            this ref FixedString512Bytes message,
            in InternalState state) {
            if (!message.SafeAppendIsValid(state.IsValid))
                return;

            if (!message.SafeAppendSocketsInterface(state.Sockets))
                return;

            if (!message.SafeAppendMessagesPerReceive(state.MessagesPerReceive))
                return;

            message.SafeAppendSendFlags(state.SendFlags);
        }

        [BurstCompile]
        static bool SafeAppendIsValid(this ref FixedString512Bytes message, byte isValid) {
            FixedString32Bytes valid = isValid == 0 ? "False" : "True"; // 5
            FixedString32Bytes listenSocketMessage = $"Is valid: {valid}"; // 10 + 5 = 15
            var remainingCapacity = message.SafeAppend(listenSocketMessage);
            return isValid != 0 && remainingCapacity > 0;
        }

        [BurstCompile]
        static bool SafeAppendSocketsInterface(this ref FixedString512Bytes message, IntPtr sockets) {
            FixedString32Bytes socketsState = sockets == IntPtr.Zero ? "invalid" : "valid"; // 7
            FixedString32Bytes listenSocketMessage = $"\nSockets interface: {socketsState}"; // 1 + 19 + 7 = 27
            var remainingCapacity = message.SafeAppend(listenSocketMessage);
            return remainingCapacity > 0;
        }

        [BurstCompile]
        static bool SafeAppendMessagesPerReceive(this ref FixedString512Bytes message, int count) {
            FixedString64Bytes pollGroupMessage = $"\nMessages per receive: {count}"; // 1 + 22 + 10 = 33
            var remainingCapacity = message.SafeAppend(pollGroupMessage);
            return remainingCapacity > 0;
        }

        [BurstCompile]
        // ReSharper disable once UnusedMethodReturnValue.Local
        static bool SafeAppendSendFlags(this ref FixedString512Bytes message, SteamSendFlags sendFlags) {
            FixedString32Bytes flags = $"\nSend flags: {(int)sendFlags}"; // 1 + 12 + 11 = 24
            var remainingCapacity = message.SafeAppend(flags);
            return remainingCapacity > 0;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void LogAsTrace(this in FixedString512Bytes message, byte hasNetDebug, in NetDebug netDebug) {
#if HAS_NETCODE_PACKAGE
            if (hasNetDebug == 0)
                Debug.Log(message);
            else
                netDebug.DebugLog(message);
#else
            Debug.Log(message);
#endif
        }
    }
}

#endif