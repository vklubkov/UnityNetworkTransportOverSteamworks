#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_LOGGING
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING
#endif

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Networking.Transport;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NetworkTransportOverSteamworks.Debugging {
    [BurstCompile]
    internal static class Common {
        static readonly FixedString32Bytes _packageNameTag = "[Steamworks Transport]";
        static readonly FixedString64Bytes _packageNameTagDark = $"<color=lightblue>{_packageNameTag}</color>";
        static readonly FixedString64Bytes _packageNameTagLight = $"<color=#1f3d61>{_packageNameTag}</color>";

        static readonly FixedString32Bytes _playTypeTag = "[CLIENT AND SERVER MODE]";
        static readonly FixedString64Bytes _playTypeTagDark = $"<color=white>{_playTypeTag}</color>";
        static readonly FixedString64Bytes _playTypeBothLight = $"<color=black>{_playTypeTag}</color>";

        static readonly FixedString32Bytes _clientTag = "[CLIENT]";
        static readonly FixedString64Bytes _clientTagDark = $"<color=orange>{_clientTag}</color>";
        static readonly FixedString64Bytes _clientTagLight = $"<color=brown>{_clientTag}</color>";

        static readonly FixedString32Bytes _serverTag = "[SERVER]";
        static readonly FixedString64Bytes _serverTagDark = $"<color=magenta>{_serverTag}</color>";
        static readonly FixedString64Bytes _serverTagLight = $"<color=purple>{_serverTag}</color>";

        static readonly FixedString32Bytes _p2PTag = "[P2P]";
        static readonly FixedString64Bytes _p2PTagDark = $"<color=#d4f8b4>{_p2PTag}</color>";
        static readonly FixedString64Bytes _p2PTagLight = $"<color=#59ad10>{_p2PTag}</color>";

        static readonly FixedString32Bytes _ipTag = "[IP]";
        static readonly FixedString64Bytes _ipTagDark = $"<color=#b4f8b6>{_ipTag}</color>";
        static readonly FixedString64Bytes _ipTagLight = $"<color=#10ad14>{_ipTag}</color>";

        static readonly FixedString32Bytes _fakeIPTag = "[FAKE IP]";
        static readonly FixedString64Bytes _fakeIPTagDark = $"<color=#b4f8d8>{_fakeIPTag}</color>";
        static readonly FixedString64Bytes _fakeIPTagLight = $"<color=#10ad64>{_fakeIPTag}</color>";

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeAppendPackageNameTag(this ref FixedString512Bytes message) {
#if !UNITY_EDITOR || NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_RICH_LOGS
            var packageName = _packageNameTag;
#else
            var packageName = EditorGUIUtility.isProSkin
                ? _packageNameTagDark
                : _packageNameTagLight;
#endif
            message.SafeAppend(in packageName);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeAppendPlayTypeTag(this ref FixedString512Bytes message, PlayType type) {
#if !UNITY_EDITOR || NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_RICH_LOGS
            if (type == PlayType.ClientAndServer)
                message.SafeAppend(in _playTypeBothTag);
#else
            if (type == PlayType.ClientAndServer) {
                var typeTag = EditorGUIUtility.isProSkin ? _playTypeTagDark : _playTypeBothLight;
                message.SafeAppend(in typeTag);
            }
#endif
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeAppendServerTag(this ref FixedString512Bytes message, byte isServer) {
#if !UNITY_EDITOR || NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_RICH_LOGS
            var serverTag = isServer != 0 ? _serverTag : _clientTag;
#else
            var serverTag =
                isServer != 0
                    ? EditorGUIUtility.isProSkin
                        ? _serverTagDark
                        : _serverTagLight
                    : EditorGUIUtility.isProSkin
                        ? _clientTagDark
                        : _clientTagLight;
#endif
            message.SafeAppend(in serverTag);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeAppendInterfaceTypeTag(this ref FixedString512Bytes message, SteamNetworkInterfaceType type) {
#if !UNITY_EDITOR || NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_RICH_LOGS
            var typeTag = type switch {
                NetworkInterfaceType.P2P => _p2PTag,
                NetworkInterfaceType.IP => _ipTag,
                NetworkInterfaceType.FakeIP => _fakeIPTag,
                _ => ""
            };
#else
            var typeTag = type switch {
                SteamNetworkInterfaceType.P2P => EditorGUIUtility.isProSkin ? _p2PTagDark : _p2PTagLight,
                SteamNetworkInterfaceType.IP => EditorGUIUtility.isProSkin ? _ipTagDark : _ipTagLight,
                SteamNetworkInterfaceType.FakeIP => EditorGUIUtility.isProSkin ? _fakeIPTagDark : _fakeIPTagLight,
                _ => ""
            };
#endif
            message.SafeAppend(in typeTag);
        }

        [BurstCompile]
        public static void SafeAppendNetworkEndpoint(
            this ref FixedString512Bytes message,
            byte isServer,
            in NativeReference<NetworkEndpoint> endpointRef) {
            if (!message.SafeAppendIsCreated(in endpointRef))
                return;

            var endpoint = endpointRef.Value;
            uint pollGroup;
            if (isServer == 0) {
                endpoint.GetConnectionWithPollGroup(out var connection, out pollGroup);
                if (!message.SafeAppendConnection(connection))
                    return;
            }
            else {
                endpoint.GetListenSocketWithPollGroup(out var listenSocket, out pollGroup);
                if (!message.SafeAppendListenSocket(listenSocket))
                    return;
            }

            message.SafeAppendPollGroup(pollGroup);
        }

        [BurstCompile]
        static bool SafeAppendIsCreated(
            this ref FixedString512Bytes message,
            in NativeReference<NetworkEndpoint> endpoint) {
            FixedString32Bytes initialized = endpoint.IsCreated ? "True" : "False"; // 5
            FixedString64Bytes endpointInitialized = $"Is endpoint initialized: {initialized}"; // 25 + 5 = 30
            var remainingCapacity = message.SafeAppend(endpointInitialized);
            return endpoint.IsCreated && remainingCapacity > 0;
        }

        [BurstCompile]
        static bool SafeAppendConnection(this ref FixedString512Bytes message, uint connection) {
            FixedString32Bytes connectionMessage = $"\nConnection: {connection}"; // 1 + 12 + 10 = 23
            var remainingCapacity = message.SafeAppend(connectionMessage);
            return remainingCapacity > 0;
        }

        [BurstCompile]
        static bool SafeAppendListenSocket(this ref FixedString512Bytes message, uint listenSocket) {
            FixedString32Bytes listenSocketMessage = $"\nListen Socket: {listenSocket}"; // 1 + 15 + 10 = 26
            var remainingCapacity = message.SafeAppend(listenSocketMessage);
            return remainingCapacity > 0;
        }

        [BurstCompile]
        // ReSharper disable once UnusedMethodReturnValue.Local
        static bool SafeAppendPollGroup(this ref FixedString512Bytes message, uint pollGroup) {
            FixedString32Bytes pollGroupMessage = $"\nPoll group: {pollGroup}"; // 1 + 12 + 10 = 23
            var remainingCapacity = message.SafeAppend(pollGroupMessage);
            return remainingCapacity > 0;
        }

        [BurstCompile]
        public static int SafeAppend(this ref FixedString512Bytes message, byte c) {
            var remainingLength = message.Capacity - message.Length - 4; // -4 because the minimal output is: "..."
            if (remainingLength <= 0)
                return remainingLength; // Can't output anything

            message.Append((char)c);
            return message.Capacity - message.Length - 4;
        }

        [BurstCompile]
        public static int SafeAppend(this ref FixedString512Bytes message, in FixedString32Bytes data) {
            var remainingLength = message.Capacity - message.Length - 4; // -4 because the minimal output is: "a..."
            if (remainingLength <= 0)
                return remainingLength; // Can't output anything

            if (remainingLength >= data.Length)
                message.Append(data);
            else {
                for (var i = 0; i < remainingLength + 1; i++)
                    message.Append(data[i]);

                message.Append('.');
                message.Append('.');
                message.Append('.');
            }

            return message.Capacity - message.Length - 4;
        }

        [BurstCompile]
        public static int SafeAppend(this ref FixedString512Bytes message, in FixedString64Bytes data) {
            var remainingLength = message.Capacity - message.Length - 4; // -4 because the minimal output is: "a..."
            if (remainingLength <= 0)
                return remainingLength; // Can't output anything

            if (remainingLength >= data.Length)
                message.Append(data);
            else {
                for (var i = 0; i < remainingLength + 1; i++)
                    message.Append(data[i]);

                message.Append('.');
                message.Append('.');
                message.Append('.');
            }

            return message.Capacity - message.Length - 4;
        }

        [BurstCompile]
        public static int SafeAppend(this ref FixedString512Bytes message, in FixedString128Bytes data) {
            var remainingLength = message.Capacity - message.Length - 4; // -4 because the minimal output is: "a..."
            if (remainingLength <= 0)
                return remainingLength; // Can't output anything

            if (remainingLength >= data.Length)
                message.Append(data);
            else {
                for (var i = 0; i < remainingLength + 1; i++)
                    message.Append(data[i]);

                message.Append('.');
                message.Append('.');
                message.Append('.');
            }

            return message.Capacity - message.Length - 4;
        }

        [BurstCompile]
        public static int SafeAppend(this ref FixedString512Bytes message, in FixedString512Bytes data) {
            var remainingLength = message.Capacity - message.Length - 4; // -4 because the minimal output is: "a..."
            if (remainingLength <= 0)
                return remainingLength; // Can't output anything

            if (remainingLength >= data.Length)
                message.Append(data);
            else {
                for (var i = 0; i < remainingLength + 1; i++)
                    message.Append(data[i]);

                message.Append('.');
                message.Append('.');
                message.Append('.');
            }

            return message.Capacity - message.Length - 4;
        }
    }
}

#endif