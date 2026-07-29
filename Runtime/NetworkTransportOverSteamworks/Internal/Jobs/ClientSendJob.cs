#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Networking.Transport;

namespace NetworkTransportOverSteamworks {
    [BurstCompile]
    internal unsafe struct ClientSendJob : IJob {
        public PacketsQueue SendQueue;
        [ReadOnly, NativeDisableUnsafePtrRestriction] public void* Sockets;
        [ReadOnly] public uint Connection;
        [ReadOnly] public SteamSendFlags SendFlags;

        public void Execute() {
            var count = SendQueue.Count;
            for (var i = 0; i < count; i++) {
                var packetProcessor = SendQueue[i];
                if (packetProcessor.Length == 0)
                    return;

                var dataPointer = (byte*)packetProcessor.GetUnsafePayloadPtr() + packetProcessor.Offset;

                long messageNumber;
                SteamAPI.ISteamNetworkingSockets.SendMessageToConnection(
                    @this: Sockets,
                    Connection,
                    dataPointer,
                    dataSize: (uint)packetProcessor.Length,
                    sendFlags: (int)SendFlags,
                    &messageNumber);
            }
        }
    }
}

#endif