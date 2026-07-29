#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

using System.Runtime.InteropServices;
using Unity.Burst;

namespace NetworkTransportOverSteamworks {
    [BurstCompile]
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal struct NetworkEndpointListenSocket {
        [FieldOffset(0)] public uint ListenSocket;
        [FieldOffset(4)] public uint PollGroup;
        [FieldOffset(8)] public byte InterfaceType;
    }
}

#endif