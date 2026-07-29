#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_DISABLE_LOGGING
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING
#endif

#if NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_LOGGING && NETWORK_TRANSPORT_OVER_STEAMWORKS_ENABLE_TRACING
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_ENABLE_TRACING
#endif

#if HAS_NETCODE_PACKAGE && HAS_ENTITIES_PACKAGE

using System;
using System.Collections.Generic;
using NetworkTransportOverSteamworks.Debugging;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace NetworkTransportOverSteamworks {
    [BurstCompile]
    public class SteamNetworkDriverConstructor : INetworkStreamDriverConstructor {
        static PlayType _playType;
        static IntPtr _sockets;
        static SteamNetworkInterfaceTypeFlags _serverInterfaces;
        static SteamNetworkInterfaceType _clientInterface;
        static SteamSendFlags _sendFlags;
        static int _messagesPerReceive;

        /// Returns the pointer to the Steamworks sockets.
        /// Use this one where you want to get the sockets
        /// for a Steam user (tied to the running Steam app).
        [BurstCompile]
        public static IntPtr GetSteamUserSockets() {
            try {
                Trace.NetworkDriverConstructor.GetSteamUserSocketsStart();

                var userHandle = SteamAPI.GetHSteamUser();
                if (userHandle <= 0) {
                    Log.NetworkDriverConstructor.GetSteamUserSocketsHUserFailure();
                    return IntPtr.Zero;
                }

                var sockets = userHandle.FindOrCreateUserInterface(SteamAPI.SocketsVersion);
                if (sockets == IntPtr.Zero) {
                    Log.NetworkDriverConstructor.GetSteamUserSocketsFailure(userHandle, SteamAPI.SocketsVersion);
                    return IntPtr.Zero;
                }

                return sockets;
            }
            finally {
                Trace.NetworkDriverConstructor.GetSteamUserSocketsEnd();
            }
        }

        /// Returns the pointer to the Steamworks sockets.
        /// Use this one where you want to get the sockets for
        /// Steamworks game server (not tied to the Steam app).
        [BurstCompile]
        public static IntPtr GetSteamGameServerSockets() {
            try {
                Trace.NetworkDriverConstructor.GetSteamGameServerSocketsStart();

                var userHandle = SteamAPI.SteamGameServer.GetHSteamUser();
                if (userHandle <= 0) {
                    Log.NetworkDriverConstructor.GetSteamGameServerSocketsHUserFailure();
                    return IntPtr.Zero;
                }

                var sockets = userHandle.FindOrCreateGameServerInterface(SteamAPI.SocketsVersion);
                if (sockets == IntPtr.Zero) {
                    Log.NetworkDriverConstructor.GetSteamGameServerSocketsFailure(userHandle, SteamAPI.SocketsVersion);
                    return IntPtr.Zero;
                }

                return sockets;
            }
            finally {
                Trace.NetworkDriverConstructor.GetSteamGameServerSocketsEnd();
            }
        }

        /// Sets <see cref="SteamNetworkDriverConstructor"/> as the driver constructor in
        /// Netcode for Entities, so any new client drivers created after this call will
        /// use Steamworks, and creation of any new server drivers will fail.
        /// Also updates the drivers of the existing worlds, but only
        /// if they are specified in the <c>worlds</c> parameter.
        /// The client interface type is set via <c>clientInterface</c>.
        /// <c>sockets</c> require the sockets returned from
        /// <see cref="GetSteamUserSockets"/> or <see cref="GetSteamGameServerSockets"/>.
        /// You can also specify the Steamworks send flags via <c>sendFlags</c> and
        /// how many Steamworks messages are processed per frame via <c>messagesPerReceive</c>.
        /// Returns false in case of errors, and you can assume
        /// that in this case the driver constructor was not set.
        /// Errors while updating the drivers of the existing worlds are
        /// reported to the console but don't cause this method to return false.
        public static bool InitializeClient(
            IntPtr sockets,
            SteamNetworkInterfaceType clientInterface,
            SteamSendFlags sendFlags = Defaults.SendFlags,
            int messagesPerReceive = Defaults.MessagesPerReceive,
            List<World> worlds = null) {
            var succeeded = true;
            try {
                Trace.NetworkDriverConstructor.InitializeClientStart();

                var playType = (PlayType)ClientServerBootstrap.RequestedPlayType;
                if (!playType.CheckPlayType(client: true)) {
                    Log.NetworkDriverConstructor.NotInitializedClient(playType);
                    return false;
                }

                if (!clientInterface.CheckInterfaceType()) {
                    Log.NetworkDriverConstructor.NotInitializedClient(clientInterface);
                    return false;
                }

                return Initialize(
                    playType,
                    sockets,
                    clientInterface,
                    serverInterfaces: SteamNetworkInterfaceTypeFlags.None,
                    sendFlags,
                    messagesPerReceive,
                    worlds);
            }
            catch {
                succeeded = false;
                throw;
            }
            finally {
                if (succeeded) { // Match Burst behavior where exceptions prevent the execution of `finally` blocks.
                    Trace.NetworkDriverConstructor.InitializeClientEnd();
                }
            }
        }

        /// Sets <see cref="SteamNetworkDriverConstructor"/> as the driver constructor in
        /// Netcode for Entities, so any new server drivers created after this call will
        /// use Steamworks, and creation of any new client drivers will fail.
        /// Also updates the drivers of the existing worlds, but only
        /// if they are specified in the <c>worlds</c> parameter.
        /// The server interface types are set via <c>serverInterfaces</c>.
        /// <c>sockets</c> require the sockets returned from
        /// <see cref="GetSteamUserSockets"/> or <see cref="GetSteamGameServerSockets"/>.
        /// You can also specify the Steamworks send flags via <c>sendFlags</c> and
        /// how many Steamworks messages are processed per frame via <c>messagesPerReceive</c>.
        /// Returns false in case of errors, and you can assume
        /// that in this case the driver constructor was not set.
        /// Errors while updating the drivers of the existing worlds are
        /// reported to the console but don't cause this method to return false.
        public static bool InitializeServer(
            IntPtr sockets,
            SteamNetworkInterfaceTypeFlags serverInterfaces,
            SteamSendFlags sendFlags = Defaults.SendFlags,
            int messagesPerReceive = Defaults.MessagesPerReceive,
            List<World> worlds = null) {
            var succeeded = true;
            try {
                Trace.NetworkDriverConstructor.InitializeServerStart();

                var playType = (PlayType)ClientServerBootstrap.RequestedPlayType;
                if (!playType.CheckPlayType(server: true)) {
                    Log.NetworkDriverConstructor.NotInitializedServer(playType);
                    return false;
                }

                if (!serverInterfaces.CheckInterfaceFlags()) {
                    Log.NetworkDriverConstructor.NotInitializedServer(serverInterfaces);
                    return false;
                }

                return Initialize(
                    playType,
                    sockets,
                    clientInterface: SteamNetworkInterfaceType.None,
                    serverInterfaces,
                    sendFlags,
                    messagesPerReceive,
                    worlds);
            }
            catch {
                succeeded = false;
                throw;
            }
            finally {
                if (succeeded) { // Match Burst behavior where exceptions prevent the execution of `finally` blocks.
                    Trace.NetworkDriverConstructor.InitializeServerEnd();
                }
            }
        }

        /// Sets <see cref="SteamNetworkDriverConstructor"/> as the driver constructor in
        /// Netcode for Entities, so any new drivers created after this call will use Steamworks.
        /// Also updates the drivers of the existing worlds, but only
        /// if they are specified in the <c>worlds</c> parameter.
        /// The client interface type is set via <c>clientInterface</c>.
        /// The server interface types are set via <c>serverInterfaces</c>.
        /// <c>sockets</c> require the sockets returned from
        /// <see cref="GetSteamUserSockets"/> or <see cref="GetSteamGameServerSockets"/>.
        /// You can also specify the Steamworks send flags via <c>sendFlags</c> and
        /// how many Steamworks messages are processed per frame via <c>messagesPerReceive</c>.
        /// Returns false in case of errors, and you can assume
        /// that in this case the driver constructor was not set.
        /// Errors while updating the drivers of the existing worlds are
        /// reported to the console but don't cause this method to return false.
        public static bool InitializeClientAndServer(
            IntPtr sockets,
            SteamNetworkInterfaceType clientInterface,
            SteamNetworkInterfaceTypeFlags serverInterfaces,
            SteamSendFlags sendFlags = Defaults.SendFlags,
            int messagesPerReceive = Defaults.MessagesPerReceive,
            List<World> worlds = null) {
            var succeeded = true;
            try {
                Trace.NetworkDriverConstructor.InitializeClientAndServerStart();

                var playType = (PlayType)ClientServerBootstrap.RequestedPlayType;
                if (!playType.CheckPlayType(client: true, server: true)) {
                    Log.NetworkDriverConstructor.NotInitializedClientAndServer(playType);
                    return false;
                }

                if (!clientInterface.CheckInterfaceType()) {
                    Log.NetworkDriverConstructor.NotInitializedClient(_clientInterface);
                    return false;
                }

                if (!serverInterfaces.CheckInterfaceFlags()) {
                    Log.NetworkDriverConstructor.NotInitializedServer(serverInterfaces);
                    return false;
                }

                return Initialize(
                    playType,
                    sockets,
                    clientInterface,
                    serverInterfaces,
                    sendFlags,
                    messagesPerReceive,
                    worlds);
            }
            catch {
                succeeded = false;
                throw;
            }
            finally {
                if (succeeded) { // Match Burst behavior where exceptions prevent the execution of `finally` blocks.
                    Trace.NetworkDriverConstructor.InitializeClientAndServerEnd();
                }
            }
        }

        static bool Initialize(
            PlayType playType,
            IntPtr sockets,
            SteamNetworkInterfaceType clientInterface,
            SteamNetworkInterfaceTypeFlags serverInterfaces,
            SteamSendFlags sendFlags = Defaults.SendFlags,
            int messagesPerReceive = Defaults.MessagesPerReceive,
            List<World> worlds = null) {
            var succeeded = true;
            try {
                Trace.NetworkDriverConstructor.InitializeStart();

                if (sockets == IntPtr.Zero) {
                    Log.NetworkDriverConstructor.NotInitializedSockets();
                    return false;
                }

                if (!sendFlags.CheckSendFlags()) {
                    Log.NetworkDriverConstructor.NotInitializedSendFlags(sendFlags);
                    return false;
                }

                if (messagesPerReceive <= 0) {
                    Log.NetworkDriverConstructor.MessagesPerReceive(messagesPerReceive);
                    messagesPerReceive = Defaults.MessagesPerReceive;
                }

                _playType = playType;
                _sockets = sockets;
                _clientInterface = clientInterface;
                _serverInterfaces = serverInterfaces;
                _sendFlags = sendFlags;
                _messagesPerReceive = messagesPerReceive;

                // Set custom drivers
                NetworkStreamReceiveSystem.DriverConstructor = new SteamNetworkDriverConstructor();

                if (worlds is { Count: > 0 }) {
                    var initialized = false;
                    foreach (var world in worlds) {
                        if (!world.IsCreated)
                            continue;

                        var netDebug = world.EntityManager
                            .CreateEntityQuery(typeof(NetDebug))
                            .GetSingleton<NetDebug>();

                        var driverStore = new NetworkDriverStore();

                        var isServer = world.IsServer() && _playType != PlayType.Client;
                        if (isServer) {
                            NetworkStreamReceiveSystem.DriverConstructor.CreateServerDriver(
                                world, ref driverStore, netDebug);
                        }

                        if (world.IsClient() && _playType != PlayType.Server) {
                            NetworkStreamReceiveSystem.DriverConstructor.CreateClientDriver(
                                world, ref driverStore, netDebug);
                        }

                        var networkStreamDriver = world.EntityManager
                            .CreateEntityQuery(typeof(NetworkStreamDriver))
                            .GetSingleton<NetworkStreamDriver>();

                        networkStreamDriver.ResetDriverStore(world.Unmanaged, ref driverStore);

                        initialized |= driverStore.IsCreated;

                        Log.NetworkDriverConstructor.UpdateWorld(
                            _playType, isServer, world.Name, in driverStore, in netDebug);
                    }

                    if (!initialized)
                        Log.NetworkDriverConstructor.NoCreatedDrivers();
                }

                Log.NetworkDriverConstructor.Initialized();
                return true;
            }
            catch {
                succeeded = false;
                throw;
            }
            finally {
                if (succeeded) { // Match Burst behavior where exceptions prevent the execution of `finally` blocks.
                    Trace.NetworkDriverConstructor.InitializeEnd();
                }
            }
        }

        /// Resets the driver constructor in Netcode for Entities, so any
        /// new drivers created after this call use the default Netcode connections.
        /// Also updates the drivers of the existing worlds, but only if they
        /// are specified in the <c>worlds</c> parameter. Errors while updating
        /// the drivers of the existing worlds are reported to the console.
        public static void Deinitialize(List<World> worlds = null) {
            var succeeded = true;
            try {
                Trace.NetworkDriverConstructor.DeinitializeStart();

                _playType = default;
                _sockets = default;
                _clientInterface = default;
                _serverInterfaces = default;
                _sendFlags = default;
                _messagesPerReceive = 0;

                // Reset the drivers
                NetworkStreamReceiveSystem.DriverConstructor = DefaultDriverBuilder.DefaultDriverConstructor;

                if (worlds is { Count: > 0 }) {
                    var initialized = false;
                    foreach (var world in worlds) {
                        if (!world.IsCreated)
                            continue;

                        var netDebug = world.EntityManager
                            .CreateEntityQuery(typeof(NetDebug))
                            .GetSingleton<NetDebug>();

                        var driverStore = new NetworkDriverStore();

                        var isServer = world.IsServer() && _playType != PlayType.Client;
                        if (isServer) {
                            NetworkStreamReceiveSystem.DriverConstructor.CreateServerDriver(
                                world, ref driverStore, netDebug);
                        }

                        if (world.IsClient() && _playType != PlayType.Server) {
                            NetworkStreamReceiveSystem.DriverConstructor.CreateClientDriver(
                                world, ref driverStore, netDebug);
                        }

                        var networkStreamDriver = world.EntityManager
                            .CreateEntityQuery(typeof(NetworkStreamDriver))
                            .GetSingleton<NetworkStreamDriver>();

                        networkStreamDriver.ResetDriverStore(world.Unmanaged, ref driverStore);

                        initialized |= driverStore.IsCreated;

                        Log.NetworkDriverConstructor.UpdateWorld(
                            _playType, isServer, world.Name, in driverStore, in netDebug);
                    }

                    if (!initialized)
                        Log.NetworkDriverConstructor.NoCreatedDrivers();
                }

                Log.NetworkDriverConstructor.Deinitialized();
            }
            catch {
                succeeded = false;
                throw;
            }
            finally {
                if (succeeded) { // Match Burst behavior where exceptions prevent the execution of `finally` blocks.
                    Trace.NetworkDriverConstructor.DeinitializeEnd();
                }
            }
        }

        public void CreateClientDriver(World world, ref NetworkDriverStore driver, NetDebug netDebug) {
            var succeeded = true;
            try {
                Trace.NetworkDriverConstructor.CreateClientDriverStart(
                    _playType, world.Name, in driver, in netDebug);

                if (!_playType.CheckPlayType(client: true)) {
                    Log.NetworkDriverConstructor.NotInitializedClient(_playType);
                    return;
                }

                if (_sockets == IntPtr.Zero) {
                    Log.NetworkDriverConstructor.NotInitializedSockets();
                    return;
                }

                if (!_clientInterface.CheckInterfaceType()) {
                    Log.NetworkDriverConstructor.NotInitializedClient(_clientInterface);
                    return;
                }

                if (!_sendFlags.CheckSendFlags()) {
                    Log.NetworkDriverConstructor.NotInitializedSendFlags(_sendFlags);
                    return;
                }

                if (_messagesPerReceive <= 0) {
                    Log.NetworkDriverConstructor.MessagesPerReceive(_messagesPerReceive);
                    _messagesPerReceive = Defaults.MessagesPerReceive;
                }

                var settings = DefaultDriverBuilder.GetNetworkClientSettings();

                var clientDriverInstance = DefaultDriverBuilder.CreateClientNetworkDriver(
                    new SteamNetworkInterface(
                        in netDebug,
                        isServer: false,
                        _sockets,
                        _clientInterface,
                        _sendFlags,
                        _messagesPerReceive),
                    settings);

                driver.RegisterDriver(TransportType.Socket, clientDriverInstance);

                Log.NetworkDriverConstructor.CreateDriverResult(
                    _playType, isServer: false, world.Name, in driver, netDebug);
            }
            catch {
                succeeded = false;
                throw;
            }
            finally {
                if (succeeded) { // Match Burst behavior where exceptions prevent the execution of `finally` blocks.
                    Trace.NetworkDriverConstructor.CreateClientDriverEnd(
                        _playType, world.Name, in driver, in netDebug);
                }
            }
        }

        public void CreateServerDriver(World world, ref NetworkDriverStore driver, NetDebug netDebug) {
            var succeeded = true;
            try {
                Trace.NetworkDriverConstructor.CreateServerDriverStart(
                    _playType, world.Name, in driver, in netDebug);

                if (!_playType.CheckPlayType(server: true)) {
                    Log.NetworkDriverConstructor.NotInitializedClient(_playType);
                    return;
                }

                if (_sockets == IntPtr.Zero) {
                    Log.NetworkDriverConstructor.NotInitializedSockets();
                    return;
                }

                if (!_serverInterfaces.CheckInterfaceFlags()) {
                    Log.NetworkDriverConstructor.NotInitializedServer(_serverInterfaces);
                    return;
                }

                if (_messagesPerReceive <= 0) {
                    Log.NetworkDriverConstructor.MessagesPerReceive(_messagesPerReceive);
                    _messagesPerReceive = Defaults.MessagesPerReceive;
                }

                var settings = DefaultDriverBuilder.GetNetworkServerSettings();

                if (_serverInterfaces.HasFlag(SteamNetworkInterfaceTypeFlags.P2P)) {
                    var p2PDriverInstance = DefaultDriverBuilder.CreateServerNetworkDriver(
                        new SteamNetworkInterface(
                            in netDebug,
                            isServer: true,
                            _sockets,
                            SteamNetworkInterfaceType.P2P,
                            _sendFlags,
                            _messagesPerReceive),
                        settings);

                    driver.RegisterDriver(TransportType.Socket, p2PDriverInstance);
                }

                if (_serverInterfaces.HasFlag(SteamNetworkInterfaceTypeFlags.IP)) {
                    var ipDriverInstance = DefaultDriverBuilder.CreateServerNetworkDriver(
                        new SteamNetworkInterface(
                            in netDebug,
                            isServer: true,
                            _sockets,
                            SteamNetworkInterfaceType.IP,
                            _sendFlags,
                            _messagesPerReceive),
                        settings);

                    driver.RegisterDriver(TransportType.Socket, ipDriverInstance);
                }

                if (_serverInterfaces.HasFlag(SteamNetworkInterfaceTypeFlags.FakeIP)) {
                    var fakeIPDriverInstance = DefaultDriverBuilder.CreateServerNetworkDriver(
                        new SteamNetworkInterface(
                            in netDebug,
                            isServer: true,
                            _sockets,
                            SteamNetworkInterfaceType.FakeIP,
                            _sendFlags,
                            _messagesPerReceive),
                        settings);

                    driver.RegisterDriver(TransportType.Socket, fakeIPDriverInstance);
                }

                Log.NetworkDriverConstructor.CreateDriverResult(
                    _playType, isServer: true, world.Name, in driver, netDebug);
            }
            catch {
                succeeded = false;
                throw;
            }
            finally {
                if (succeeded) { // Match Burst behavior where exceptions prevent the execution of `finally` blocks.
                    Trace.NetworkDriverConstructor.CreateServerDriverEnd(
                        _playType, world.Name, in driver, in netDebug);
                }
            }
        }
    }
}

#endif
#endif