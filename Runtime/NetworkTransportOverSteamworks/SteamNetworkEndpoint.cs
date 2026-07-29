#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Networking.Transport;

namespace NetworkTransportOverSteamworks {
    [BurstCompile]
    public static unsafe class SteamNetworkEndpoint {
        /// Creates a new <see cref="NetworkEndpoint"/> with the provided interface
        /// type, connection, poll group, and <see cref="NetworkEndpoint.Family"/>
        /// set to <see cref="NetworkFamily.Custom"/>. The caller must ensure the parameters
        /// hold the appropriate connection data  before calling this method.
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateEndpointForSteamConnection(
            out NetworkEndpoint endpoint,
            SteamNetworkInterfaceType type = SteamNetworkInterfaceType.None,
            uint connection = 0, uint pollGroup = 0) {
            endpoint = default;
            endpoint.Family = NetworkFamily.Custom;
            var dataPtr = (NetworkEndpointConnection*)endpoint.GetRawDataPtrRef();
            dataPtr->Connection = connection;
            dataPtr->PollGroup = pollGroup;
            dataPtr->InterfaceType = (byte)type;
        }

        /// Creates a new <see cref="NetworkEndpoint"/> with the provided interface
        /// types, listen sockets, poll groups, and <see cref="NetworkEndpoint.Family"/>
        /// set to <see cref="NetworkFamily.Custom"/>. The caller must ensure the parameters
        /// hold the appropriate listen sockets data  before calling this method.
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateEndpointForSteamListenSockets(
            out NetworkEndpoint endpoint,
            SteamNetworkInterfaceTypeFlags types = SteamNetworkInterfaceTypeFlags.None,
            uint listenSocketP2P = 0, uint pollGroupP2P = 0,
            uint listenSocketIP = 0, uint pollGroupIP = 0,
            uint listenSocketFakeIP = 0, uint pollGroupFakeIP = 0) {
            endpoint = default;
            endpoint.Family = NetworkFamily.Custom;
            var dataPtr = (NetworkEndpointListenSockets*)endpoint.GetRawDataPtrRef();
            dataPtr->ListenSocketP2P = listenSocketP2P;
            dataPtr->PollGroupP2P = pollGroupP2P;
            dataPtr->ListenSocketIP = listenSocketIP;
            dataPtr->PollGroupIP = pollGroupIP;
            dataPtr->ListenSocketFakeIP = listenSocketFakeIP;
            dataPtr->PollGroupFakeIP = pollGroupFakeIP;
            dataPtr->InterfaceTypes = (byte)types;
        }

        /// Fills the specified <see cref="NetworkEndpoint"/> with the provided interface
        /// type, connection, poll group, and <see cref="NetworkEndpoint.Family"/>
        /// set to <see cref="NetworkFamily.Custom"/>. The caller must ensure the parameters
        /// hold the appropriate connection data  before calling this method.
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSteamConnection(
            this ref NetworkEndpoint endpoint,
            SteamNetworkInterfaceType type = SteamNetworkInterfaceType.None,
            uint connection = 0, uint pollGroup = 0) {
            endpoint = default;
            endpoint.Family = NetworkFamily.Custom;
            var dataPtr = (NetworkEndpointConnection*)endpoint.GetRawDataPtrRef();
            dataPtr->Connection = connection;
            dataPtr->PollGroup = pollGroup;
            dataPtr->InterfaceType = (byte)type;
        }

        /// Fills the specified <see cref="NetworkEndpoint"/> with the provided interface
        /// types, listen sockets, poll groups, and <see cref="NetworkEndpoint.Family"/>
        /// set to <see cref="NetworkFamily.Custom"/>. The caller must ensure the parameters
        /// hold the appropriate listen sockets data  before calling this method.
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSteamListenSockets(
            this ref NetworkEndpoint endpoint,
            SteamNetworkInterfaceTypeFlags types = SteamNetworkInterfaceTypeFlags.None,
            uint listenSocketP2P = 0, uint pollGroupP2P = 0,
            uint listenSocketIP = 0, uint pollGroupIP = 0,
            uint listenSocketFakeIP = 0, uint pollGroupFakeIP = 0) {
            endpoint = default;
            endpoint.Family = NetworkFamily.Custom;
            var dataPtr = (NetworkEndpointListenSockets*)endpoint.GetRawDataPtrRef();
            dataPtr->ListenSocketP2P = listenSocketP2P;
            dataPtr->PollGroupP2P = pollGroupP2P;
            dataPtr->ListenSocketIP = listenSocketIP;
            dataPtr->PollGroupIP = pollGroupIP;
            dataPtr->ListenSocketFakeIP = listenSocketFakeIP;
            dataPtr->PollGroupFakeIP = pollGroupFakeIP;
            dataPtr->InterfaceTypes = (byte)types;
        }

        /// Gets the interface type, connection, and poll group
        /// from the specified <see cref="NetworkEndpoint"/>.
        /// The caller must ensure the <see cref="NetworkEndpoint"/> holds
        /// the appropriate connection data before calling this method.
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetSteamConnection(
            this ref NetworkEndpoint endpoint,
            out SteamNetworkInterfaceType type,
            out uint connection, out uint pollGroup) {
            var dataPtr = (NetworkEndpointConnection*)endpoint.GetRawDataPtrRef();
            connection = dataPtr->Connection;
            pollGroup = dataPtr->PollGroup;
            type = (SteamNetworkInterfaceType)dataPtr->InterfaceType;
        }

        /// Gets the interface types, listen sockets, and poll groups
        /// from the specified <see cref="NetworkEndpoint"/>.
        /// The caller must ensure the <see cref="NetworkEndpoint"/> holds
        /// the appropriate listen sockets data before calling this method.
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetSteamListenSockets(
            this in NetworkEndpoint endpoint,
            out SteamNetworkInterfaceTypeFlags types,
            out uint listenSocketP2P, out uint pollGroupP2P,
            out uint listenSocketIP, out uint pollGroupIP,
            out uint listenSocketFakeIP, out uint pollGroupFakeIP) {
            var dataPtr = (NetworkEndpointListenSockets*)endpoint.GetRawDataPtrIn();
            listenSocketP2P = dataPtr->ListenSocketP2P;
            pollGroupP2P = dataPtr->PollGroupP2P;
            listenSocketIP = dataPtr->ListenSocketIP;
            pollGroupIP = dataPtr->PollGroupIP;
            listenSocketFakeIP = dataPtr->ListenSocketFakeIP;
            pollGroupFakeIP = dataPtr->PollGroupFakeIP;
            types = (SteamNetworkInterfaceTypeFlags)dataPtr->InterfaceTypes;
        }
    }
}

#endif