#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

using System;

namespace NetworkTransportOverSteamworks {
    // These values can be found in steamnetworkingtypes.h
    // k_nSteamNetworkingSend_... flags
    [Flags]
    public enum SteamSendFlags {
        Unreliable = 0,
        NoNagle = 1,
        UnreliableNoNagle = Unreliable | NoNagle,
        NoDelay = 4,
        UnreliableNoDelay = Unreliable | NoDelay | NoNagle,
        Reliable = 8,
        ReliableNoNagle = Reliable | NoNagle,
        UseCurrentThread = 16,
        AutoRestartBrokenSession = 32
    }
}

#endif