#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;

namespace NetworkTransportOverSteamworks {
    [BurstCompile]
    internal static class SteamAPI {
        // - C/C++ int is 32 bits on most modern systems
        //   (and on all systems supported by Steam),
        //   so a C# int is used to handle ints.

        // - An enum in C/C++ can be any integral type,
        //   but by default it is 32 bits or smaller
        //   if the larger member fits into the data type,
        //   so a C# int is used to handle enums.


#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN) && UNITY_64
        const string _nativeLib = "steam_api64";
#else
        const string _nativeLib = "steam_api";
#endif

        // STEAMNETWORKINGSOCKETS_INTERFACE_VERSION
        public static readonly FixedString64Bytes SocketsVersion = "SteamNetworkingSockets012";

        [DllImport(_nativeLib,
            EntryPoint = "SteamAPI_GetHSteamUser",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int // HSteamUser (int32)
            GetHSteamUser();


        [BurstCompile]
        [StructLayout(LayoutKind.Sequential)] // Default packing (should work fine unless Linux x86 is supported)
        internal unsafe struct SteamNetworkingMessage {
            public void* Data; // void *m_pData
            public int Size; // int m_cbSize
            public uint Connection; // HSteamNetConnection m_conn (uint32)

            // Fields below are not used by Steam Transport:
            public SteamNetworkingIdentity IdentityPeer; // SteamNetworkingIdentity m_identityPeer
            public long ConnectionUserData; // int64 m_nConnUserData
            public long TimeReceivedMicroseconds; // SteamNetworkingMicroseconds m_usecTimeReceived (int64)
            public long MessageNumber; // int64 m_nMessageNumber
            public IntPtr OnFreeData; // void (*m_pfnFreeData)( SteamNetworkingMessage_t *pMsg )
            public IntPtr OnRelease; // void (*m_pfnRelease)( SteamNetworkingMessage_t *pMsg )
            public int Channel; // int m_nChannel
            public int Flags; // int m_nFlags
            public long UserData; // int64 m_nUserData
            public ushort LaneIndex; // uint16 m_idxLane
            public ushort Padding; // uint16 _pad1__

            [DllImport(_nativeLib,
                EntryPoint = "SteamAPI_SteamNetworkingMessage_t_Release",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern void Release(IntPtr @this); // SteamNetworkingMessage*
        }


        [BurstCompile]
        [StructLayout(LayoutKind.Sequential, Pack = 1)] // #pragma pack(push,1)
        internal unsafe struct SteamNetworkingIdentity {
            public int Type; // ESteamNetworkingIdentityType m_eType (enum)
            public int Size; // int m_cbSize
            public fixed byte Data[128]; // a union with max size of 128 chars
        }


        [BurstCompile]
        // ReSharper disable once InconsistentNaming
        internal static unsafe class ISteamNetworkingSockets {
            [DllImport(_nativeLib,
                EntryPoint = "SteamAPI_ISteamNetworkingSockets_SendMessageToConnection",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern int // EResult (enum)
                SendMessageToConnection(
                    void* @this, // ISteamNetworkingSockets*
                    uint connection, // HSteamNetConnection hConn (uint32)
                    byte* data, // const void *pData
                    uint dataSize, // uint32 cbData
                    int sendFlags, // int nSendFlags
                    long* messageNumber); // int64 *pOutMessageNumber

            [DllImport(_nativeLib,
                EntryPoint = "SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern int // int
                ReceiveMessagesOnConnection(
                    void* @this, // ISteamNetworkingSockets*
                    uint connection, // HSteamNetConnection hConn (uint32)
                    SteamNetworkingMessage** messages, // SteamNetworkingMessage_t **ppOutMessages
                    int maxMessages); // int nMaxMessages

            [DllImport(_nativeLib,
                EntryPoint = "SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnPollGroup",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern int // int
                ReceiveMessagesOnPollGroup(
                    void* @this, // ISteamNetworkingSockets*
                    uint pollGroup, // HSteamNetPollGroup hPollGroup (uint32)
                    SteamNetworkingMessage** messages, // SteamNetworkingMessage_t **ppOutMessages
                    int maxMessages); // int nMaxMessages
        }


        [BurstCompile]
        internal static class SteamGameServer {
            [DllImport(_nativeLib,
                EntryPoint = "SteamGameServer_GetHSteamUser",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern int // HSteamUser (int)
                // ReSharper disable once MemberHidesStaticFromOuterClass
                GetHSteamUser();
        }


        [BurstCompile]
        internal static unsafe class Internal {
            [DllImport(_nativeLib,
                EntryPoint = "SteamInternal_FindOrCreateUserInterface",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr // void*
                FindOrCreateUserInterface(
                    int user, // HSteamUser hSteamUser (int)
                    void* version); // const char *pszVersion

            [DllImport(_nativeLib,
                EntryPoint = "SteamInternal_FindOrCreateGameServerInterface",
                CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr // void*
                FindOrCreateGameServerInterface(
                    int steamUser, // HSteamUser hSteamUser (int)
                    void* version); // const char *pszVersion
        }
    }
}

#endif