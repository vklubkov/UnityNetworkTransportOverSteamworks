#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#if !HAS_NETCODE_PACKAGE

namespace NetworkTransportOverSteamworks {
    // NetDebug stub for when Netcode for Entities is not available.
    public struct NetDebug { }
}

#endif
#endif