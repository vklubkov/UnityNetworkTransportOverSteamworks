#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

using System;
using Unity.Burst;

#if HAS_NETCODE_PACKAGE
using Unity.NetCode;
#endif

namespace NetworkTransportOverSteamworks {
    [BurstCompile]
    internal struct InternalState {
        public readonly IntPtr Sockets;
        public readonly NetDebug NetDebug;
        public readonly SteamNetworkInterfaceType InterfaceType;
        public readonly SteamSendFlags SendFlags;
        public readonly int MessagesPerReceive;
        public readonly byte IsServer;
        public readonly byte HasNetDebug;
        public byte IsValid;

        public InternalState(
            byte isServer,
            IntPtr sockets,
            SteamNetworkInterfaceType interfaceType,
            SteamSendFlags sendFlags,
            int messagesPerReceive,
            byte hasNetDebug,
            in NetDebug netDebug) {
            Sockets = sockets;
            NetDebug = netDebug;
            InterfaceType = interfaceType;
            SendFlags = sendFlags;
            MessagesPerReceive = messagesPerReceive <= 0 ? Defaults.MessagesPerReceive : messagesPerReceive;
            IsServer = isServer;
            HasNetDebug = hasNetDebug;
            IsValid = 1;
        }
    }
}

#endif