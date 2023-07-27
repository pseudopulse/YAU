using System;
using RoR2.Networking;

namespace YAU.Networking {
    public class NetworkingHelper {
        internal static short TakenMessagesIndex = 100;

        public static short GetMessageIndex() {
            if (TakenMessagesIndex == short.MaxValue) {
                YAU.ModLogger.LogError("Mod attempted to request a message index with the maximum amount has been claimed.");
                throw new OverflowException();
            }

            TakenMessagesIndex++;
            return TakenMessagesIndex;
        }

        public static NetworkWriter CreateMessage(short messageType, Action<NetworkWriter> setup) {
            NetworkWriter writer = new();
            writer.StartMessage(messageType);
            setup(writer);
            writer.FinishMessage();
            return writer;
        }

        public static void ServerSendToAll(NetworkWriter writer) {
            if (NetworkServer.active) {
                foreach (NetworkConnection connection in NetworkServer.connections) {
                    if (connection == null) {
                        continue;
                    }

                    connection?.SendWriter(writer, QosChannelIndex.defaultReliable.intVal);
                }
            }
        }

        public static void SendToServer(NetworkWriter writer) {
            if (ClientScene.readyConnection == null) {
                return;
            }

            ClientScene.readyConnection?.SendWriter(writer, QosChannelIndex.defaultReliable.intVal);
        }
    }
}