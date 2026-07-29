#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_LOGGING
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING
#endif

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Networking.Transport;
using Debug = UnityEngine.Debug;

#if HAS_NETCODE_PACKAGE
using Unity.NetCode;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NetworkTransportOverSteamworks.Debugging {
    [BurstCompile]
    internal static class Log {
        enum MessageType {
            // ReSharper disable once UnusedMember.Local
            None,
            Success,
            Warning,
            Failure
        }

        static readonly FixedString32Bytes _successTag = "[SUCCESS]";
        static readonly FixedString64Bytes _successTagDark = $"<color=green>{_successTag}</color>";
        static readonly FixedString64Bytes _successTagLight = $"<color=#006400>{_successTag}</color>";

        static readonly FixedString32Bytes _warningTag = "[WARNING]";
        static readonly FixedString64Bytes _warningTagDark = $"<color=yellow>{_warningTag}</color>";
        static readonly FixedString64Bytes _warningTagLight = $"<color=olive>{_warningTag}</color>";

        static readonly FixedString32Bytes _failureTag = "[FAILURE]";
        static readonly FixedString64Bytes _failureTagDark = $"<color=red>{_failureTag}</color>";
        static readonly FixedString64Bytes _failureTagLight = $"<color=maroon>{_failureTag}</color>";

#if HAS_NETCODE_PACKAGE

        [BurstCompile]
        internal static class NetworkDriverConstructor {
            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void GetSteamUserSocketsHUserFailure() {
                FixedString64Bytes message = "Failed to get the user handle from Steamworks SDK (client)."; // 59
                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void GetSteamUserSocketsFailure(int userHandle, in FixedString64Bytes socketsVersion) {
                FixedString512Bytes message =
                    $"Failed to get {socketsVersion} from Steamworks SDK (client) via user handle: {userHandle}."; // 14 + 61 + 47 + 11 + 1 = 134

                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void GetSteamGameServerSocketsHUserFailure() {
                FixedString128Bytes message = "Failed to get the user handle from Steamworks SDK (game server)."; // 64
                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void GetSteamGameServerSocketsFailure(int userHandle, in FixedString64Bytes socketsVersion) {
                FixedString512Bytes message =
                    $"Failed to get {socketsVersion} from Steamworks SDK (game server) via user handle: {userHandle}."; // 14 + 61 + 52 + 11 + 1 = 139

                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NotInitializedClient(PlayType playType) {
                FixedString32Bytes playTypeString = playType switch {
                    PlayType.Client => "Client",
                    PlayType.Server => "Server",
                    PlayType.ClientAndServer => "ClientAndServer", // 15
                    _ => "Unknown"
                };

                FixedString64Bytes message = $"Invalid client Play Type: {playTypeString}."; // 26 + 15 + 1 = 42
                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NotInitializedClient(SteamNetworkInterfaceType clientInterface) {
                FixedString64Bytes message = $"Invalid client interface requested: {(int)clientInterface}."; // 36 + 11 + 1 = 48
                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NotInitializedServer(PlayType playType) {
                FixedString32Bytes playTypeString = playType switch {
                    PlayType.Client => "Client",
                    PlayType.Server => "Server",
                    PlayType.ClientAndServer => "ClientAndServer", // 15
                    _ => "Unknown"
                };

                FixedString64Bytes message = $"Invalid server Play Type: {playTypeString}."; // 26 + 15 + 1 = 42
                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NotInitializedServer(SteamNetworkInterfaceTypeFlags serverInterfaces) {
                FixedString64Bytes message = $"Invalid server interfaces requested: {(int)serverInterfaces}."; // 37 + 11 + 1 = 49
                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NotInitializedClientAndServer(PlayType playType) {
                FixedString32Bytes playTypeString = playType switch {
                    PlayType.Client => "Client",
                    PlayType.Server => "Server",
                    PlayType.ClientAndServer => "ClientAndServer", // 15
                    _ => "Unknown"
                };

                FixedString64Bytes message = $"Invalid client and server Play Type: {playTypeString}."; // 37 + 15 + 1 = 53
                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NotInitializedSockets() {
                FixedString64Bytes message = "Invalid sockets: null."; // 22
                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NotInitializedSendFlags(SteamSendFlags sendFlags) {
                FixedString64Bytes message = $"Invalid Steams send flags: {(int)sendFlags}."; // 27 + 11 + 1 = 39
                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void MessagesPerReceive(int messagesPerReceive) {
                FixedString128Bytes message = $"MessagesPerReceive value is invalid: {messagesPerReceive}. Using the default value: {Defaults.MessagesPerReceive}."; // 38 + 11 + 27 + 11 + 1 = 88
                LogInternal(MessageType.Warning, message);
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void UpdateWorld(
                PlayType playType,
                bool isServer,
                in FixedString512Bytes worldName,
                in NetworkDriverStore driver,
                in NetDebug netDebug) {
                var messageType = driver.IsCreated ? MessageType.Success : MessageType.Failure;
                var message = new FixedString512Bytes();
                FixedString32Bytes prefix = "Updated drivers in world "; // 25
                message.SafeAppend(prefix);
                message.SafeAppend(worldName); // Any

                if (driver.IsCreated)
                    message.SafeAppend((byte)'.');
                else {
                    FixedString64Bytes postfix = ", but its NetworkDriverStore reports it is not created."; // 55
                    message.SafeAppend(postfix);
                }

                LogInternal(playType, isServer ? (byte)1 : (byte)0, messageType, in message, in netDebug);
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NoCreatedDrivers() {
                FixedString32Bytes message = "No drivers were created."; // 24
                LogInternal(MessageType.Failure, message);
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void Initialized() {
                FixedString64Bytes message = "Initialization succeeded."; // 25
                LogInternal(MessageType.Success, message);
            }

            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void Exception(Exception e) => Debug.LogException(e);

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void Deinitialized() {
                FixedString64Bytes message = "Deinitialization succeeded."; // 27
                LogInternal(MessageType.Success, message);
            }

            [BurstCompile]
            static void LogInternal(MessageType messageType, in FixedString64Bytes message) {
                var fullMessage = new FixedString512Bytes();
                fullMessage.SafeAppendPackageNameTag();
                fullMessage.SafeAppend((byte)' ');

                switch (messageType) {
                    case MessageType.Success:
                        fullMessage.SafeAppendSuccessTag();
                        break;
                    case MessageType.Warning:
                        fullMessage.SafeAppendWarningTag();
                        break;
                    case MessageType.Failure:
                        fullMessage.SafeAppendFailureTag();
                        break;
                }

                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppend(message);

                switch (messageType) {
                    case MessageType.Warning:
                        Debug.LogWarning(fullMessage);
                        break;
                    case MessageType.Failure:
                        Debug.LogError(fullMessage);
                        break;
                    default:
                        Debug.Log(fullMessage);
                        break;
                }
            }

            [BurstCompile]
            static void LogInternal(MessageType messageType, in FixedString128Bytes message) {
                var fullMessage = new FixedString512Bytes();
                fullMessage.SafeAppendPackageNameTag();
                fullMessage.SafeAppend((byte)' ');

                switch (messageType) {
                    case MessageType.Success:
                        fullMessage.SafeAppendSuccessTag();
                        break;
                    case MessageType.Warning:
                        fullMessage.SafeAppendWarningTag();
                        break;
                    case MessageType.Failure:
                        fullMessage.SafeAppendFailureTag();
                        break;
                }

                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppend(message);

                switch (messageType) {
                    case MessageType.Warning:
                        Debug.LogWarning(fullMessage);
                        break;
                    case MessageType.Failure:
                        Debug.LogError(fullMessage);
                        break;
                    default:
                        Debug.Log(fullMessage);
                        break;
                }
            }

            [BurstCompile]
            static void LogInternal(MessageType messageType, in FixedString512Bytes message) {
                var fullMessage = new FixedString512Bytes();
                fullMessage.SafeAppendPackageNameTag();
                fullMessage.SafeAppend((byte)' ');

                switch (messageType) {
                    case MessageType.Success:
                        fullMessage.SafeAppendSuccessTag();
                        break;
                    case MessageType.Warning:
                        fullMessage.SafeAppendWarningTag();
                        break;
                    case MessageType.Failure:
                        fullMessage.SafeAppendFailureTag();
                        break;
                }

                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppend(message);

                switch (messageType) {
                    case MessageType.Warning:
                        Debug.LogWarning(fullMessage);
                        break;
                    case MessageType.Failure:
                        Debug.LogError(fullMessage);
                        break;
                    default:
                        Debug.Log(fullMessage);
                        break;
                }
            }

            [BurstCompile]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void CreateDriverResult(
                PlayType playType,
                bool isServer,
                in FixedString512Bytes worldName,
                in NetworkDriverStore driver,
                in NetDebug netDebug) {
                var messageType = driver.IsCreated ? MessageType.Success : MessageType.Failure;
                var message = new FixedString512Bytes();
                FixedString32Bytes prefix = "Registered drivers in world "; // 28
                message.SafeAppend(prefix);
                message.SafeAppend(worldName); // Any

                if (driver.IsCreated)
                    message.SafeAppend((byte)'.');
                else {
                    FixedString64Bytes postfix = ", but its NetworkDriverStore reports it is not created."; // 55
                    message.SafeAppend(postfix);
                }

                LogInternal(playType, isServer ? (byte)1 : (byte)0, messageType, in message, in netDebug);
            }

            static void LogInternal(
                PlayType playType,
                byte isServer,
                MessageType messageType,
                in FixedString512Bytes message,
                in NetDebug netDebug) {
                var fullMessage = new FixedString512Bytes();
                fullMessage.SafeAppendPackageNameTag();
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendPlayTypeTag(playType);
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendServerTag(isServer);
                fullMessage.SafeAppend((byte)' ');

                switch (messageType) {
                    case MessageType.Success:
                        fullMessage.SafeAppendSuccessTag();
                        break;
                    case MessageType.Warning:
                        fullMessage.SafeAppendWarningTag();
                        break;
                    case MessageType.Failure:
                        fullMessage.SafeAppendFailureTag();
                        break;
                }

                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppend(message);

                switch (messageType) {
                    case MessageType.Warning:
                        fullMessage.LogAsWarning(hasNetDebug: 1, in netDebug);
                        break;
                    case MessageType.Failure:
                        fullMessage.LogAsError(hasNetDebug: 1, in netDebug);
                        break;
                    default:
                        fullMessage.LogAsInfo(hasNetDebug: 1, in netDebug);
                        break;
                }
            }
        }

#endif

        [BurstCompile]
        internal static class NetworkInterface {
            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void Invalid(in InternalState state) {
                FixedString64Bytes message = "NetworkInterface state is invalid."; // 34
                LogInternal(MessageType.Failure, message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NotInitialized(in InternalState state) {
                FixedString64Bytes message = "NetworkInterface is not initialized."; // 36
                LogInternal(MessageType.Failure, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void InvalidArgumentPlayType(PlayType playType, in InternalState state) {
                FixedString32Bytes value = playType switch {
                    PlayType.ClientAndServer => "ClientAndServer", // 15
                    PlayType.Client => "Client",
                    PlayType.Server => "Server",
                    _ => "invalid"
                };

                FixedString64Bytes message = $"Invalid argument: playType = {value}."; // 29 + 15 + 1 = 45
                LogInternal(MessageType.Failure, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void InvalidArgumentInterfaceType(
                SteamNetworkInterfaceType interfaceType,
                in InternalState state) {
                FixedString32Bytes value = interfaceType switch {
                    SteamNetworkInterfaceType.P2P => "P2P",
                    SteamNetworkInterfaceType.IP => "IP",
                    SteamNetworkInterfaceType.FakeIP => "FakeIP",
                    _ => "invalid"
                };

                FixedString64Bytes message = $"Invalid argument: interfaceType = {value}."; // 34 + 7 + 1 = 42
                LogInternal(MessageType.Failure, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void InvalidArgumentSockets(IntPtr value, in InternalState state) {
                FixedString64Bytes message = $"Invalid argument: sockets = {value}."; // 28 + 20 + 1 = 49
                LogInternal(MessageType.Failure, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void InvalidArgumentMessagesPerReceive(int value, in InternalState state) {
                FixedString128Bytes message = $"Invalid argument: messagesPerReceive = {value}. Using the default value: {state.MessagesPerReceive}."; // 40 + 11 + 27 + 11 + 1 = 90
                LogInternal(MessageType.Warning, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void InvalidArgumentSendFlags(
                SteamSendFlags sendFlagsFlags,
                in InternalState state) {
                FixedString64Bytes message = $"Invalid argument: sendFlags = {(int)sendFlagsFlags}."; // 30 + 11 + 1 = 42
                LogInternal(MessageType.Failure, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void AlreadyInitialized(in InternalState state) {
                FixedString64Bytes message = "NetworkInterface is already initialized."; // 40
                LogInternal(MessageType.Failure, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void Initialized(in InternalState state) {
                FixedString64Bytes message = "NetworkInterface is initialized."; // 32
                LogInternal(MessageType.Success, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void Disposed(in InternalState state) {
                FixedString32Bytes message = "NetworkInterface is disposed."; // 29
                LogInternal(MessageType.Success, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NotBound(in InternalState state, in NativeReference<NetworkEndpoint> endpoint) {
                FixedString64Bytes message = "Failed to bind the NetworkInterface."; // 36
                LogInternal(MessageType.Failure, in message, in state, in endpoint);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void Bound(in InternalState state, in NativeReference<NetworkEndpoint> endpoint) {
                FixedString32Bytes message = "NetworkInterface is bound."; // 26
                LogInternal(MessageType.Success, in message, in state, in endpoint);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NotServer(in InternalState state, in NativeReference<NetworkEndpoint> endpoint) {
                FixedString64Bytes message = "NetworkInterface is trying to listen while not a server."; // 56
                LogInternal(MessageType.Failure, in message, in state, in endpoint);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void Listening(in InternalState state, in NativeReference<NetworkEndpoint> endpoint) {
                FixedString64Bytes message = "NetworkInterface is listening."; // 30
                LogInternal(MessageType.Success, in message, in state, in endpoint);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NoPollGroup(in InternalState state) {
                FixedString64Bytes message = "NetworkInterface has no poll group."; // 35
                LogInternal(MessageType.Failure, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NoListenSocket(in InternalState state) {
                FixedString64Bytes message = "NetworkInterface has no listen socket."; // 38
                LogInternal(MessageType.Failure, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NoConnectionAndNoPollGroup(in InternalState state) {
                FixedString64Bytes message = "NetworkInterface has no connection and no poll group."; // 53
                LogInternal(MessageType.Failure, in message, in state);
            }

            [BurstCompile]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Conditional("NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING")]
            public static void NoConnection(in InternalState state) {
                FixedString64Bytes message = "NetworkInterface has no connection."; // 35
                LogInternal(MessageType.Failure, in message, in state);
            }

            [BurstCompile]
            static void LogInternal(
                MessageType messageType,
                in FixedString32Bytes message,
                in InternalState state) {
                var fullMessage = new FixedString512Bytes();
                fullMessage.SafeAppendPackageNameTag();
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendServerTag(state.IsServer);
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendInterfaceTypeTag(state.InterfaceType);
                fullMessage.SafeAppend((byte)' ');

                switch (messageType) {
                    case MessageType.Success:
                        fullMessage.SafeAppendSuccessTag();
                        break;
                    case MessageType.Warning:
                        fullMessage.SafeAppendWarningTag();
                        break;
                    case MessageType.Failure:
                        fullMessage.SafeAppendFailureTag();
                        break;
                }

                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppend(message);

                switch (messageType) {
                    case MessageType.Warning:
                        fullMessage.LogAsWarning(state.HasNetDebug, in state.NetDebug);
                        break;
                    case MessageType.Failure:
                        fullMessage.LogAsError(state.HasNetDebug, in state.NetDebug);
                        break;
                    default:
                        fullMessage.LogAsInfo(state.HasNetDebug, in state.NetDebug);
                        break;
                }
            }

            [BurstCompile]
            static void LogInternal(
                MessageType messageType,
                in FixedString64Bytes message,
                in InternalState state) {
                var fullMessage = new FixedString512Bytes();
                fullMessage.SafeAppendPackageNameTag();
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendServerTag(state.IsServer);
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendInterfaceTypeTag(state.InterfaceType);
                fullMessage.SafeAppend((byte)' ');

                switch (messageType) {
                    case MessageType.Success:
                        fullMessage.SafeAppendSuccessTag();
                        break;
                    case MessageType.Warning:
                        fullMessage.SafeAppendWarningTag();
                        break;
                    case MessageType.Failure:
                        fullMessage.SafeAppendFailureTag();
                        break;
                }

                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppend(message);

                switch (messageType) {
                    case MessageType.Warning:
                        fullMessage.LogAsWarning(state.HasNetDebug, in state.NetDebug);
                        break;
                    case MessageType.Failure:
                        fullMessage.LogAsError(state.HasNetDebug, in state.NetDebug);
                        break;
                    default:
                        fullMessage.LogAsInfo(state.HasNetDebug, in state.NetDebug);
                        break;
                }
            }

            [BurstCompile]
            static void LogInternal(
                MessageType messageType,
                in FixedString128Bytes message,
                in InternalState state) {
                var fullMessage = new FixedString512Bytes();
                fullMessage.SafeAppendPackageNameTag();
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendServerTag(state.IsServer);
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendInterfaceTypeTag(state.InterfaceType);
                fullMessage.SafeAppend((byte)' ');

                switch (messageType) {
                    case MessageType.Success:
                        fullMessage.SafeAppendSuccessTag();
                        break;
                    case MessageType.Warning:
                        fullMessage.SafeAppendWarningTag();
                        break;
                    case MessageType.Failure:
                        fullMessage.SafeAppendFailureTag();
                        break;
                }

                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppend(message);

                switch (messageType) {
                    case MessageType.Warning:
                        fullMessage.LogAsWarning(state.HasNetDebug, in state.NetDebug);
                        break;
                    case MessageType.Failure:
                        fullMessage.LogAsError(state.HasNetDebug, in state.NetDebug);
                        break;
                    default:
                        fullMessage.LogAsInfo(state.HasNetDebug, in state.NetDebug);
                        break;
                }
            }

            [BurstCompile]
            // ReSharper disable once UnusedMember.Local
            static void LogInternal(
                MessageType messageType,
                in FixedString512Bytes message,
                in InternalState state) {
                var fullMessage = new FixedString512Bytes();
                fullMessage.SafeAppendPackageNameTag();
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendServerTag(state.IsServer);
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendInterfaceTypeTag(state.InterfaceType);
                fullMessage.SafeAppend((byte)' ');

                switch (messageType) {
                    case MessageType.Success:
                        fullMessage.SafeAppendSuccessTag();
                        break;
                    case MessageType.Warning:
                        fullMessage.SafeAppendWarningTag();
                        break;
                    case MessageType.Failure:
                        fullMessage.SafeAppendFailureTag();
                        break;
                }

                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppend(message);

                switch (messageType) {
                    case MessageType.Warning:
                        fullMessage.LogAsWarning(state.HasNetDebug, in state.NetDebug);
                        break;
                    case MessageType.Failure:
                        fullMessage.LogAsError(state.HasNetDebug, in state.NetDebug);
                        break;
                    default:
                        fullMessage.LogAsInfo(state.HasNetDebug, in state.NetDebug);
                        break;
                }
            }

            [BurstCompile]
            static void LogInternal(
                MessageType messageType,
                in FixedString32Bytes message,
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) {
                var fullMessage = new FixedString512Bytes();
                fullMessage.SafeAppendPackageNameTag();
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendServerTag(state.IsServer);
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendInterfaceTypeTag(state.InterfaceType);
                fullMessage.SafeAppend((byte)' ');

                switch (messageType) {
                    case MessageType.Success:
                        fullMessage.SafeAppendSuccessTag();
                        break;
                    case MessageType.Warning:
                        fullMessage.SafeAppendWarningTag();
                        break;
                    case MessageType.Failure:
                        fullMessage.SafeAppendFailureTag();
                        break;
                }

                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppend(message);
                fullMessage.SafeAppend((byte)'\n');
                fullMessage.SafeAppendNetworkEndpoint(state.IsServer, in endpoint);

                switch (messageType) {
                    case MessageType.Warning:
                        fullMessage.LogAsWarning(state.HasNetDebug, in state.NetDebug);
                        break;
                    case MessageType.Failure:
                        fullMessage.LogAsError(state.HasNetDebug, in state.NetDebug);
                        break;
                    default:
                        fullMessage.LogAsInfo(state.HasNetDebug, in state.NetDebug);
                        break;
                }
            }

            [BurstCompile]
            static void LogInternal(
                MessageType messageType,
                in FixedString64Bytes message,
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) {
                var fullMessage = new FixedString512Bytes();
                fullMessage.SafeAppendPackageNameTag();
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendServerTag(state.IsServer);
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendInterfaceTypeTag(state.InterfaceType);
                fullMessage.SafeAppend((byte)' ');

                switch (messageType) {
                    case MessageType.Success:
                        fullMessage.SafeAppendSuccessTag();
                        break;
                    case MessageType.Warning:
                        fullMessage.SafeAppendWarningTag();
                        break;
                    case MessageType.Failure:
                        fullMessage.SafeAppendFailureTag();
                        break;
                }

                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppend(message);
                fullMessage.SafeAppend((byte)'\n');
                fullMessage.SafeAppendNetworkEndpoint(state.IsServer, in endpoint);

                switch (messageType) {
                    case MessageType.Warning:
                        fullMessage.LogAsWarning(state.HasNetDebug, in state.NetDebug);
                        break;
                    case MessageType.Failure:
                        fullMessage.LogAsError(state.HasNetDebug, in state.NetDebug);
                        break;
                    default:
                        fullMessage.LogAsInfo(state.HasNetDebug, in state.NetDebug);
                        break;
                }
            }

            [BurstCompile]
            // ReSharper disable once UnusedMember.Local
            static void LogInternal(
                MessageType messageType,
                in FixedString512Bytes message,
                in InternalState state,
                in NativeReference<NetworkEndpoint> endpoint) {
                var fullMessage = new FixedString512Bytes();
                fullMessage.SafeAppendPackageNameTag();
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendServerTag(state.IsServer);
                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppendInterfaceTypeTag(state.InterfaceType);
                fullMessage.SafeAppend((byte)' ');

                switch (messageType) {
                    case MessageType.Success:
                        fullMessage.SafeAppendSuccessTag();
                        break;
                    case MessageType.Warning:
                        fullMessage.SafeAppendWarningTag();
                        break;
                    case MessageType.Failure:
                        fullMessage.SafeAppendFailureTag();
                        break;
                }

                fullMessage.SafeAppend((byte)' ');
                fullMessage.SafeAppend(message);
                fullMessage.SafeAppend((byte)'\n');
                fullMessage.SafeAppendNetworkEndpoint(state.IsServer, in endpoint);

                switch (messageType) {
                    case MessageType.Warning:
                        fullMessage.LogAsWarning(state.HasNetDebug, in state.NetDebug);
                        break;
                    case MessageType.Failure:
                        fullMessage.LogAsError(state.HasNetDebug, in state.NetDebug);
                        break;
                    default:
                        fullMessage.LogAsInfo(state.HasNetDebug, in state.NetDebug);
                        break;
                }
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SafeAppendSuccessTag(this ref FixedString512Bytes message) {
#if !UNITY_EDITOR || NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_RICH_LOGS
            var successTag = _successTag;
#else
            var successTag = EditorGUIUtility.isProSkin
                ? _successTagDark
                : _successTagLight;
#endif
            message.SafeAppend(in successTag);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SafeAppendWarningTag(this ref FixedString512Bytes message) {
#if !UNITY_EDITOR || NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_RICH_LOGS
            var warningTag = _warningTag;
#else
            var warningTag = EditorGUIUtility.isProSkin
                ? _warningTagDark
                : _warningTagLight;
#endif
            message.SafeAppend(in warningTag);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SafeAppendFailureTag(this ref FixedString512Bytes message) {
#if !UNITY_EDITOR || NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_RICH_LOGS
            var failureTag = _failureTag;`
#else
            var failureTag = EditorGUIUtility.isProSkin
                ? _failureTagDark
                : _failureTagLight;
#endif
            message.SafeAppend(in failureTag);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void LogAsInfo(this in FixedString512Bytes message, byte hasNetDebug, in NetDebug netDebug) {
#if HAS_NETCODE_PACKAGE
            if (hasNetDebug == 0)
                Debug.Log(message);
            else
                netDebug.Log(message);
#else
            Debug.Log(message);
#endif
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void LogAsWarning(this in FixedString512Bytes message, byte hasNetDebug, in NetDebug netDebug) {
#if HAS_NETCODE_PACKAGE
            if (hasNetDebug == 0)
                Debug.LogWarning(message);
            else
               netDebug.LogWarning(message);
#else
            Debug.LogWarning(message);
#endif
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void LogAsError(this in FixedString512Bytes message, byte hasNetDebug, in NetDebug netDebug) {
#if HAS_NETCODE_PACKAGE
            if (hasNetDebug == 0)
                Debug.LogError(message);
            else
                netDebug.LogError(message);
#else
            Debug.LogError(message);
#endif
        }
    }
}

#endif