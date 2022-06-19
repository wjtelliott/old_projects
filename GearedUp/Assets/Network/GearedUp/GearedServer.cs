using System;
using System.IO;
using System.Xml.Serialization;
using GearedUpEngine.Assets.Entities;
using GearedUpEngine.Assets.Map;
using GearedUpEngine.Assets.Util;
using Lidgren.Network;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 3/31/2017 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Network.GearedUp
{

    public enum NetworkDataType
    {
        TileMapDownload = 0,
        ObjectDownload = 1,
        TEST = 2,
        GameChatMessage = 3,
        NewClientPlayer = 4,
        ClientRequestUID = 5,
        ClientMovementRequest = 6,
        PlayerMovementUpdate = 7,
        ClientAccountLogin = 8,
    }

    sealed class GearedServer
    {
        readonly GearedServerProxy _server;
        public bool ShutdownServer { get; set; }
        readonly ServerNetworking serverNetworkHelper;
        ServerPlayer[] players;
        public bool IsDedicated { get; set; }
        public GearedServer(int port, string appId)
        {
            this._server = new GearedServerProxy(port, appId);
            this.serverNetworkHelper = new ServerNetworking();
            this.ShutdownServer = false;
            this.players = new ServerPlayer[0];
        }

        public void StartServer()
        {
            this.ShutdownServer = false;

            if (this.IsDedicated) DeveloperDetails.Details = "[0]Launching server! Inside thread";

            try
            {
                this._server.Start();
                double nextSendMessages = NetTime.Now;
                
                while (!this.ShutdownServer)
                {
                    NetIncomingMessage netMsg;
                    while ((netMsg = this._server.ReadMessage()) != null)
                    {
                        //message receieved
                        switch (netMsg.MessageType)
                        {
                            case NetIncomingMessageType.DiscoveryRequest:
                                this._server.server.SendDiscoveryResponse(null, netMsg.SenderEndPoint);
                                
                                break;
                            case NetIncomingMessageType.StatusChanged:
                                NetConnectionStatus status = (NetConnectionStatus)netMsg.ReadByte();
                                if (status == NetConnectionStatus.Connected)
                                {
                                    //new connection

                                    {
                                        this.serverNetworkHelper.SendChatToAll(this._server.server, "A new player is connecting");

                                        this.serverNetworkHelper.CreateNewPlayerForAll(this._server.server, netMsg.SenderConnection.RemoteUniqueIdentifier);
                                        

                                        AppendPlayer(new ServerPlayer("unnamed", netMsg.SenderConnection.RemoteUniqueIdentifier));

                                        this.serverNetworkHelper.RequestLoginCredentials(this._server.server, netMsg.SenderConnection);

                                        if (this.IsDedicated)
                                            DeveloperDetails.Details += "[0]New player entered game";
                                    }

                                    ////test send obj
                                    {
                                        NetOutgoingMessage outMsg = this._server.server.CreateMessage();
                                        outMsg.Write((byte)NetworkDataType.TileMapDownload);

                                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Tilemap));
                                        using (StringWriter textWriter = new StringWriter())
                                        {
                                            xmlSerializer.Serialize(textWriter, this._server.tmp);
                                            outMsg.Write(textWriter.ToString());
                                        }

                                        this._server.server.SendMessage(outMsg, netMsg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                    }
                                    //{
                                    //    NetOutgoingMessage outMsg = this._server.server.CreateMessage();
                                    //    outMsg.Write((byte)NetworkDataType.ObjectDownload);
                                    //    outMsg.Write(true);
                                    //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(StaticEntity));
                                    //    using (StringWriter textWriter = new StringWriter())
                                    //    {
                                    //        xmlSerializer.Serialize(textWriter, this._server.statictest);
                                    //        outMsg.Write(textWriter.ToString());
                                    //        outMsg.Write("grass_left");
                                    //    }

                                    //    this._server.server.SendMessage(outMsg, this._server.server.Connections[0], NetDeliveryMethod.ReliableOrdered);
                                    //}
                                }
                                else if (status == NetConnectionStatus.Disconnected || status == NetConnectionStatus.Disconnecting)
                                {
                                    //lost player
                                    RemovePlayer(netMsg.SenderConnection.RemoteUniqueIdentifier);
                                }
                                break;
                            case NetIncomingMessageType.Data:

                                switch (netMsg.ReadByte())
                                {
                                    case (byte)NetworkDataType.GameChatMessage:
                                        string message = netMsg.ReadString();
                                        NetOutgoingMessage netOut = this._server.server.CreateMessage();
                                        netOut.Write((byte)NetworkDataType.GameChatMessage);
                                        netOut.Write(GetPlayerFromUID(netMsg.SenderConnection.RemoteUniqueIdentifier).Name + ": " + message);
                                        this._server.server.SendToAll(netOut, NetDeliveryMethod.ReliableOrdered);
                                        break;
                                    case (byte)NetworkDataType.ClientRequestUID:
                                        NetOutgoingMessage netOut2 = this._server.server.CreateMessage();
                                        netOut2.Write((byte)NetworkDataType.ClientRequestUID);
                                        netOut2.Write(netMsg.SenderConnection.RemoteUniqueIdentifier);
                                        this._server.server.SendMessage(netOut2, netMsg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                        break;
                                    case (byte)NetworkDataType.TEST:
                                        NetOutgoingMessage netNewPlayer = this._server.server.CreateMessage();
                                        netNewPlayer.Write((byte)NetworkDataType.NewClientPlayer);
                                        netNewPlayer.Write(4294967296);
                                        this._server.server.SendMessage(netNewPlayer, netMsg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                        break;
                                    case (byte)NetworkDataType.ClientMovementRequest:
                                        this.serverNetworkHelper.ClientMovement(netMsg, GetPlayerFromUID(netMsg.SenderConnection.RemoteUniqueIdentifier));
                                        break;
                                    case (byte)NetworkDataType.ClientAccountLogin:
                                        {

                                            // Check our login data and see if we need to disconnect the user
                                            if (!this.serverNetworkHelper.CheckPlayerLogin(netMsg, out string accountName))
                                            {

                                                if (this.IsDedicated) DeveloperDetails.Details += "[2]Player tried accessing account " + accountName;

                                                // The user did not send corret login information. Give them an error message
                                                netMsg.SenderConnection.Disconnect("NErr01");

                                                // Remove the old obsolete player
                                                this.RemovePlayer(netMsg.SenderConnection.RemoteUniqueIdentifier);
                                            }
                                            else
                                            {

                                                if (this.IsDedicated) DeveloperDetails.Details += "[1]Player " + accountName + " has logged in.";

                                                // The user logged in correctly, assign their player data
                                                this.ReplacePlayerByUID(netMsg.SenderConnection.RemoteUniqueIdentifier, this.serverNetworkHelper.AssignPlayerData(accountName, netMsg.SenderConnection.RemoteUniqueIdentifier));

                                                // Inform them of the other clients on this server
                                                this.serverNetworkHelper.UpdateClientGameState_OtherClients(this._server.server, netMsg.SenderConnection, this.players);
                                            }
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                    //Update some game logic
                    UpdateGameLogic();
                    
                    //Sleep
                    System.Threading.Thread.Sleep(1);
                }

            }
            catch (Exception e)
            {
                if (this.IsDedicated) DeveloperDetails.Details = "[2]" + e.ToString();

                EndServer();
            }
        }

        void AppendPlayer(ServerPlayer newPlayer)
        {
            Array.Resize<ServerPlayer>(ref this.players, this.players.Length + 1);
            this.players[this.players.Length - 1] = newPlayer;
        }

        void RemovePlayer(long uid)
        {
            bool error = true;
            foreach (ServerPlayer sp in this.players)
                if (sp.UserID == uid)
                {
                    error = false;
                    break;
                }
            if (error) return;

            ServerPlayer[] temp = new ServerPlayer[this.players.Length - 1];
            int index = 0;
            for (int i = 0; i < this.players.Length; i++)
                if (this.players[i].UserID != uid)
                {
                    temp[index] = this.players[i];
                    index++;
                }
            this.players = temp;
        }

        void ReplacePlayerByUID(long uid, ServerPlayer newPlayerData)
        {
            for (int i = 0; i < this.players.Length; i++)
                if (this.players[i] != null)
                    if (this.players[i].UserID == uid)
                    {
                        this.players[i] = newPlayerData;
                        return;
                    }
        }

        ServerPlayer GetPlayerFromUID(long uid)
        {
            foreach (ServerPlayer p in this.players)
                if (p != null)
                    if (p.UserID == uid)
                        return p;
            return null;
        }

        void UpdateGameLogic()
        {
            {


                foreach (ServerPlayer p in this.players)
                {
                    p.Update();
                    this.serverNetworkHelper.PlayerMovementUpdateRaw(this._server.server, this._server.server.Connections, p);
                }
            }
        }

        public void EndServer()
        {
            this.ShutdownServer = true;
        }
    }

    sealed class GearedServerProxy
    {
        public NetServer server;

        public Tilemap tmp;
        public StaticEntity statictest;

        readonly NetPeerConfiguration config;
        double nextSendUpdates;
        const string exitMessage = "Server exiting...";
        public GearedServerProxy(int port, string appId)
        {
            this.config = new NetPeerConfiguration(appId);

            //this.config.SimulatedMinimumLatency = 0.060f;
            

            this.config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            this.config.Port = port;
            this.server = new NetServer(this.config);

            

#pragma warning disable IDE0017 // Simplify object initialization
            statictest = new StaticEntity();
#pragma warning restore IDE0017 // Simplify object initialization
            statictest.DrawRectangle = new Microsoft.Xna.Framework.Rectangle(50, 50, 16, 16);
            statictest.IsVisible = true;

            tmp = new Tilemap();
            tmp.ChangeMap_Flatgrass(11, 11);
        }

        public void Start()
        {
            this.server.Start();
            nextSendUpdates = NetTime.Now;
        }

        public void Stop()
        {
            this.server.Shutdown(exitMessage);
        }

        public NetIncomingMessage ReadMessage()
        {
            return this.server.ReadMessage();
        }
    }
}
