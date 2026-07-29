#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

namespace NetworkTransportOverSteamworks {
    public enum SteamNetworkInterfaceType {
        None = 0,
        P2P = 1,
        IP = 2,
        FakeIP = 3
    }
}

#endif