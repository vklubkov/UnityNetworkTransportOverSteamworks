#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

using Unity.Burst;

namespace NetworkTransportOverSteamworks {
    [BurstCompile]
    internal static class Defaults {
        public const int MessagesPerReceive = 128;
        public const SteamSendFlags SendFlags = SteamSendFlags.Unreliable;
    }
}

#endif