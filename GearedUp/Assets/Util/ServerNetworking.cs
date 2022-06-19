using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GearedUpEngine.Assets.Network.GearedUp;
using GearedUpEngine.Assets.Entities;
using System.Xml.Serialization;
using System.IO;
using GearedUpEngine.Assets.Map;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 4/5/2017 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Util
{

    /// <summary>
    /// Our server's shorthand networking utility
    /// </summary>
    public class ServerNetworking
    {

        /// <summary>
        /// Read our client message for which direction our client is attempting to move
        /// </summary>
        /// <param name="message">Our net incoming message from the client</param>
        public void ClientMovement(NetIncomingMessage message, ServerPlayer player)
        {

            if (player == null)
                return;

            // Read byte for how many inputs we have
            byte[] clientMovementInputs = new byte[message.ReadByte()];

            // Read our client inputs as bytes
            for (int i = 0; i < clientMovementInputs.Length; i++)
                clientMovementInputs[i] = message.ReadByte();

            // Tell our player to add acceleration from our tokenized movement
            player.AddVelocityFromClientKeys(clientMovementInputs);
        }

        public void UpdateClientGameState_OtherClients(NetServer server, NetConnection user, ServerPlayer[] players)
        {
            foreach (ServerPlayer player in players)
                if (user.RemoteUniqueIdentifier != player.UserID)
                {
                    NetOutgoingMessage netNewPlayer = server.CreateMessage();
                    netNewPlayer.Write((byte)NetworkDataType.NewClientPlayer);
                    netNewPlayer.Write(player.UserID);
                    server.SendMessage(netNewPlayer, user, NetDeliveryMethod.ReliableOrdered);
                }
        }

        public void CreateNewPlayerForAll(NetServer server, long uid)
        {
            NetOutgoingMessage netNewPlayer = server.CreateMessage();
            netNewPlayer.Write((byte)NetworkDataType.NewClientPlayer);
            netNewPlayer.Write(uid);
            server.SendToAll(netNewPlayer, NetDeliveryMethod.ReliableOrdered);
        }

        public void RequestLoginCredentials(NetServer server, NetConnection user)
        {
            NetOutgoingMessage netPasswordRequest = server.CreateMessage();
            netPasswordRequest.Write((byte)NetworkDataType.ClientAccountLogin);
            server.SendMessage(netPasswordRequest, user, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendChatToAll(NetServer server, string message)
        {
            NetOutgoingMessage netOut = server.CreateMessage();
            netOut.Write((byte)NetworkDataType.GameChatMessage);
            netOut.Write(message);
            server.SendToAll(netOut, NetDeliveryMethod.ReliableOrdered);
        }

        public bool CheckPlayerLogin(NetIncomingMessage message, out string accountName)
        {
            string username = message.ReadString(),
                password = message.ReadString();
            accountName = username;
            switch (username)
            {
                case "billy":
                    return (password == "password") ? true : false;
                case "cody":
                    return (password == "baraboo") ? true : false;
            }
            return false;
        }

        public ServerPlayer AssignPlayerData(string accountName, long uid)
        {
            // For now all we assign is account name to player name
            return new ServerPlayer(accountName, uid);
        }

        /// <summary>
        /// Send a message to all clients that this player has moved to a new position
        /// </summary>
        /// <param name="server">The server object</param>
        /// <param name="connections">The connections to the server object</param>
        /// <param name="player">The player that has moved</param>
        public void PlayerMovementUpdateRaw(NetServer server, List<NetConnection> connections, ServerPlayer player)
        {

            // If our player is null, do nothing
            if (player == null)
                return;

            // Create our message
            NetOutgoingMessage netOut = server.CreateMessage();

            // Write our network type
            netOut.Write((byte)NetworkDataType.PlayerMovementUpdate);

            // Write the player uid in question that's moving
            netOut.Write((long)player.UserID);

            // Write the position of the player
            netOut.Write((float)player.Position.X);
            netOut.Write((float)player.Position.Y);
            
            // Send the message to all clients, including the client that controls that player
            server.SendToAll(netOut, NetDeliveryMethod.Unreliable);

        }
        
    }
}
