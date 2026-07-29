#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport;

namespace NetworkTransportOverSteamworks {
    [BurstCompile]
    internal static class Extensions {
        public static bool CheckPlayType(this PlayType playType, bool client = false, bool server = false) {
            if (client && server)
                return playType is PlayType.ClientAndServer;

            if (client)
                return playType is PlayType.ClientAndServer or PlayType.Client;

            if (server)
                return playType is PlayType.ClientAndServer or PlayType.Server;

            return playType is PlayType.ClientAndServer or PlayType.Client or PlayType.Server;
        }

        public static bool CheckInterfaceType(this SteamNetworkInterfaceType clientInterface) =>
            clientInterface is SteamNetworkInterfaceType.P2P or
                SteamNetworkInterfaceType.IP or
                SteamNetworkInterfaceType.FakeIP;

        public static bool CheckInterfaceFlags(this SteamNetworkInterfaceTypeFlags serverInterfaces) =>
            serverInterfaces.HasFlag(SteamNetworkInterfaceTypeFlags.P2P) ||
            serverInterfaces.HasFlag(SteamNetworkInterfaceTypeFlags.IP) ||
            serverInterfaces.HasFlag(SteamNetworkInterfaceTypeFlags.FakeIP);

        public static bool CheckSendFlags(this SteamSendFlags sendFlags) =>
            sendFlags.HasFlag(SteamSendFlags.Unreliable) ||
            sendFlags.HasFlag(SteamSendFlags.NoNagle) ||
            sendFlags.HasFlag(SteamSendFlags.NoDelay) ||
            sendFlags.HasFlag(SteamSendFlags.UnreliableNoDelay) ||
            sendFlags.HasFlag(SteamSendFlags.Reliable) ||
            sendFlags.HasFlag(SteamSendFlags.ReliableNoNagle) ||
            sendFlags.HasFlag(SteamSendFlags.UseCurrentThread) ||
            sendFlags.HasFlag(SteamSendFlags.AutoRestartBrokenSession);

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void ApplyConnectionWithPollGroupAndInterfaceType(
            this ref NetworkEndpoint endpoint, byte interfaceType, uint connection, uint pollGroup = 0) {
            endpoint = default;
            endpoint.Family = NetworkFamily.Custom;
            var dataPtr = (NetworkEndpointConnection*)endpoint.GetRawDataPtrRef();
            dataPtr->Connection = connection;
            dataPtr->PollGroup = pollGroup;
            dataPtr->InterfaceType = interfaceType;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void GetConnection(this in NetworkEndpoint endpoint, out uint connection) {
            var dataPtr = (NetworkEndpointConnection*)endpoint.GetRawDataPtrIn();
            connection = dataPtr->Connection;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void GetConnectionWithPollGroup(
            this in NetworkEndpoint endpoint, out uint connection, out uint pollGroup) {
            var dataPtr = (NetworkEndpointConnection*)endpoint.GetRawDataPtrIn();
            connection = dataPtr->Connection;
            pollGroup = dataPtr->PollGroup;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void GetConnectionInterfaceType(
            this in NetworkEndpoint endpoint, out SteamNetworkInterfaceType interfaceType) {
            var dataPtr = (NetworkEndpointConnection*)endpoint.GetRawDataPtrIn();
            interfaceType = (SteamNetworkInterfaceType)dataPtr->InterfaceType;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void GetListenSocketsWithPollGroupsAndInterfaceTypes(
            this in NetworkEndpoint endpoint,
            out uint listenSocketP2P, out uint pollGroupP2P,
            out uint listenSocketIP, out uint pollGroupIP,
            out uint listenSocketFakeIP, out uint pollGroupFakeIP,
            out SteamNetworkInterfaceTypeFlags interfaceTypes) {
            var dataPtr = (NetworkEndpointListenSockets*)endpoint.GetRawDataPtrIn();
            listenSocketP2P = dataPtr->ListenSocketP2P;
            pollGroupP2P = dataPtr->PollGroupP2P;
            listenSocketIP = dataPtr->ListenSocketIP;
            pollGroupIP = dataPtr->PollGroupIP;
            listenSocketFakeIP = dataPtr->ListenSocketFakeIP;
            pollGroupFakeIP = dataPtr->PollGroupFakeIP;
            interfaceTypes = (SteamNetworkInterfaceTypeFlags)dataPtr->InterfaceTypes;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void CreateEndpointWithListenSocketPollGroupAndInterfaceType(
            this uint listenSocket, uint pollGroup,
            SteamNetworkInterfaceType interfaceType,
            out NetworkEndpoint endpoint) {
            endpoint = default;
            endpoint.Family = NetworkFamily.Custom;
            var dataPtr = (NetworkEndpointListenSocket*)endpoint.GetRawDataPtrRef();
            dataPtr->ListenSocket = listenSocket;
            dataPtr->PollGroup = pollGroup;
            dataPtr->InterfaceType = (byte)interfaceType;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void GetListenSocket(this in NetworkEndpoint endpoint, out uint listenSocket) {
            var dataPtr = (NetworkEndpointListenSocket*)endpoint.GetRawDataPtrIn();
            listenSocket = dataPtr->ListenSocket;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void GetListenSocketWithPollGroup(
            this in NetworkEndpoint endpoint, out uint listenSocket, out uint pollGroup) {
            var dataPtr = (NetworkEndpointListenSocket*)endpoint.GetRawDataPtrIn();
            listenSocket = dataPtr->ListenSocket;
            pollGroup = dataPtr->PollGroup;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void GetListenSocketWithPollGroupAndInterfaceType(
            this in NetworkEndpoint endpoint,
            out uint listenSocket, out uint pollGroup,
            out byte interfaceType) {
            var dataPtr = (NetworkEndpointListenSocket*)endpoint.GetRawDataPtrIn();
            listenSocket = dataPtr->ListenSocket;
            pollGroup = dataPtr->PollGroup;
            interfaceType = dataPtr->InterfaceType;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte* GetRawDataPtrRef(this ref NetworkEndpoint endpoint) =>
            (byte*)UnsafeUtility.AddressOf(ref endpoint.Transferrable);

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte* GetRawDataPtrIn(this in NetworkEndpoint endpoint) =>
            (byte*)UnsafeUtility.AddressOf(ref Unsafe.AsRef(in endpoint.Transferrable));

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr FindOrCreateGameServerInterface(
            this int userHandle, in FixedString64Bytes version) =>
                SteamAPI.Internal.FindOrCreateGameServerInterface(userHandle, version.GetUnsafePtr());

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr FindOrCreateUserInterface(this int userHandle, in FixedString64Bytes version) =>
            SteamAPI.Internal.FindOrCreateUserInterface(userHandle, version.GetUnsafePtr());
    }
}

#endif