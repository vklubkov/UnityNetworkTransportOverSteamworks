#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE
#endif

#if !NETWORK_TRANSPORT_OVER_STEAMWORKS_INTERNAL_DISABLE

using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;

namespace NetworkTransportOverSteamworks {
    [BurstCompile]
    internal unsafe struct ServerReceiveJob : IJob {
        public PacketsQueue ReceiveQueue;
        [ReadOnly] public NativeArray<IntPtr> Messages;
        [ReadOnly] public int Count;
        [ReadOnly] public uint PollGroup;
        [ReadOnly] public byte InterfaceType;

        public void Execute() {
            for (var i = 0; i < Count; i++) {
                var messagePtr = Messages[i];
                var messageRawPtr = (SteamAPI.SteamNetworkingMessage*)messagePtr;
                if (messageRawPtr == null)
                    return;

                if (messageRawPtr->Size > 0 && ReceiveQueue.EnqueuePacket(out var packetProcessor)) {
                    packetProcessor.AppendToPayload(messageRawPtr->Data, messageRawPtr->Size);

                    ref var packetProcessorEndpoint = ref packetProcessor.EndpointRef;

                    packetProcessorEndpoint.ApplyConnectionWithPollGroupAndInterfaceType(
                        InterfaceType, messageRawPtr->Connection, PollGroup);
                }

                SteamAPI.SteamNetworkingMessage.Release(messagePtr);
            }
        }
    }
}

#endif