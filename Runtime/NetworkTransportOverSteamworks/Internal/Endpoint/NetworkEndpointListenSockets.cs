#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

using System.Runtime.InteropServices;
using Unity.Burst;

namespace NetworkTransportOverSteamworks {
    [BurstCompile]
    [StructLayout(LayoutKind.Explicit)]
    internal struct NetworkEndpointListenSockets {
        [FieldOffset(0)] public uint ListenSocketP2P;
        [FieldOffset(4)] public uint PollGroupP2P;
        [FieldOffset(8)] public uint ListenSocketIP;
        [FieldOffset(12)] public uint PollGroupIP;
        [FieldOffset(16)] public uint ListenSocketFakeIP;
        [FieldOffset(20)] public uint PollGroupFakeIP;
        [FieldOffset(24)] public byte InterfaceTypes;
    }
}

#endif