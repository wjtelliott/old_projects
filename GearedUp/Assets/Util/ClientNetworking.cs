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
    /// Our client's shorthand network helper
    /// </summary>
    public class ClientNetworking
    {
        /// <summary>
        /// Connect to a server with a given IP
        /// </summary>
        /// <param name="client">Game client object</param>
        /// <param name="ip">IP to connect to</param>
        public void ConnectTo(NetClient client, System.Net.IPEndPoint ip)
        {
            client.Connect(ip);
        }

        /// <summary>
        /// Download a full Xml serialized object from the server.
        /// You will still have to cast the object as its needed type after
        /// downloading from the function.
        /// </summary>
        /// <param name="message">The NetIncoming message from the server containing the object</param>
        /// <param name="textManager">Game's texture manager</param>
        /// <returns>Serialized Object</returns>
        public object DownloadObject(NetIncomingMessage message, TextureManager textManager)
        {
            // At the moment, only one object is implemented. Static Entities

            // Check if this is a static or dynamic entity
            if (message.ReadBoolean())
            {
                // Static

                // Read the string to grab the Xml
                string objXml = message.ReadString();

                // Serialize it into a static object Xml
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(StaticEntity));

                // Use a string reader to feed into the Xml Serializer
                using (StringReader textReader = new StringReader(objXml))
                {
                    // Create a static entity and initialize it with the given Xml
                    StaticEntity obj = (StaticEntity)xmlSerializer.Deserialize(textReader);

                    // Re-serialize the texture (We can't do this in Xml)
                    obj.Texture = textManager.GetTextureByName(message.ReadString());

                    // Return our downloaded object
                    return obj;
                }
            }

            // Error
            return null;
        }

        /// <summary>
        /// Download a tilemap in Xml from the server to the client
        /// </summary>
        /// <param name="message">NetIncoming message from the server containing the tilemap</param>
        /// <param name="textManager">Game's texture manager</param>
        /// <returns>The deserialized tilemap object</returns>
        public object DownloadTileMap(NetIncomingMessage message, TextureManager textManager)
        {
            // Create a tilemap object
            Tilemap map;

            // Read our Xml string from the buffer
            string objXml = message.ReadString();

            // Init our Xml serializer
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Tilemap));

            // Init a string reader to feed into our Xml serializer
            using (StringReader textReader = new StringReader(objXml))
            {
                // Deserialize the map from Xml
                map = (Tilemap)xmlSerializer.Deserialize(textReader);

                // Re-serialize the tile textures ( we can't do this from Xml )
                foreach (Tile t in map.Tiles)
                    if (t.Texture == null)
                        t.Texture = textManager.GetTextureByName(t.TextureSerialization);
            }

            // Return the downloaded map
            return map;
        }

        /// <summary>
        /// Send a text chat to the server for all users
        /// </summary>
        /// <param name="client">Our game client object</param>
        /// <param name="message">The message we wish to send</param>
        public void SendChat(NetClient client, string message)
        {
            // Create an outgoing message
            NetOutgoingMessage netOut = client.CreateMessage();

            // Append our data type, so the server knows what to do with
            // this sent data.
            netOut.Write((byte)NetworkDataType.GameChatMessage);

            // Write our message to the buffer
            netOut.Write(message);

            // Send our message buffer to the server
            client.SendMessage(netOut, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Downloads a new client player entity, reading the UID as a long
        /// </summary>
        /// <param name="message">The NetIncoming message</param>
        /// <returns></returns>
        public ClientPlayer DownloadClientPlayer(NetIncomingMessage message)
        {
            
            // Read the UID and return the player
            return new ClientPlayer(message.ReadInt64());
        }

        /// <summary>
        /// Download our client UID from the server and return it as a string.
        /// </summary>
        /// <param name="message">Our net message</param>
        /// <returns>Our client's UID</returns>
        public UInt64 DownloadClientUID(NetIncomingMessage message)
        {

            // Read the UID
            return message.ReadUInt64();
        }

        /// <summary>
        /// Request our UID from the server
        /// </summary>
        /// <param name="client">The game's net client</param>
        public void RequestClientUID(NetClient client)
        {

            //Create our message
            NetOutgoingMessage netOut = client.CreateMessage();

            // Write the data header
            netOut.Write((byte)NetworkDataType.ClientRequestUID);

            // Send the message, they'll know the reason and respond automatically
            client.SendMessage(netOut, NetDeliveryMethod.ReliableOrdered);
        }


        /// <summary>
        /// We use this to test anything related to client/server interaction
        /// </summary>
        /// <param name="client"></param>
        public void SendTestingCommand(NetClient client)
        {
            //obsolete
            NetOutgoingMessage netout = client.CreateMessage();
            netout.Write((byte)NetworkDataType.TEST);
            client.SendMessage(netout, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Send to the server that we are trying to move our character in a direction
        /// (We send our keys, and the server will determine velocity)
        /// </summary>
        /// <param name="client"></param>
        public void SendCharacterMovement(NetClient client, KeyboardState kState)
        {

            //TODO: Add new key bindings for certain movements
            NetOutgoingMessage netOut = client.CreateMessage();

            // Write our header, we want to move a direction
            netOut.Write((byte)NetworkDataType.ClientMovementRequest);

            // Get our movement keys
            Dictionary<Keys, byte> pressedMovementKeys = new Dictionary<Keys, byte>();
            if (kState.IsKeyDown(Keys.A)) pressedMovementKeys.Add(Keys.A, 0);
            if (kState.IsKeyDown(Keys.W)) pressedMovementKeys.Add(Keys.W, 1);
            if (kState.IsKeyDown(Keys.S)) pressedMovementKeys.Add(Keys.S, 2);
            if (kState.IsKeyDown(Keys.D)) pressedMovementKeys.Add(Keys.D, 3);

            // Write how many keys are in the buffer so we know how far we read on the server side
            netOut.Write((byte)pressedMovementKeys.Count);

            // Write our keys to the buffer
            foreach (byte movementByte in pressedMovementKeys.Values)
                netOut.Write(movementByte);

            // Send our buffer to the server
            client.SendMessage(netOut, NetDeliveryMethod.Unreliable);
        }

        public void SendPlayerLogin(NetClient client, string username, string password)
        {
            NetOutgoingMessage netOut = client.CreateMessage();
            netOut.Write((byte)NetworkDataType.ClientAccountLogin);
            netOut.Write(username);
            netOut.Write(password);
            client.SendMessage(netOut, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Download the raw player's position, without velocity. This should
        /// only be used when you're trying to connect for the first time or
        /// debugging. The client should normally download the velocity and
        /// interpolate from there itself.
        /// </summary>
        /// <param name="netMessage">The net message from the server</param>
        /// <param name="players">The player list on the client machine</param>
        public void DownloadPlayerMovement(NetIncomingMessage netMessage, ClientPlayer[] players)
        {

            // Read the player in question from the message
            long uid = netMessage.ReadInt64();

            // Loop through each player in our list
            foreach (ClientPlayer p in players)
            {

                // Find the player for the movement update
                if (p.UniqueUserID == uid)
                {

                    // Update their position from the message
                    p.RawPosition.X = (int)netMessage.ReadFloat();
                    p.RawPosition.Y = (int)netMessage.ReadFloat();

                    // Break from our list, we don't want to loop through items we don't care about
                    break;
                }
            }

        }
    }
}
