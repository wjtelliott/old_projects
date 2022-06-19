using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 3/31/2017 by: William E
/// Do not modify.
/// </summary>
namespace GearedUpEngine.Assets.Network.GearedUp
{
    /// <summary>
    /// Our client proxys main class. We will use this to store 
    /// our NetClient and NetConfig for our client thread.
    /// </summary>
    public sealed class GearedClientProxy
    {
        // Our net client and net config we will use private fields
        readonly NetClient _client;
        readonly NetPeerConfiguration config;

        // Our other objects and classes can access these fields, but
        // may not alter or write to them, except at init
        public NetClient Client { get { return this._client; } }
        public NetPeerConfiguration Config { get { return this.config; } }

        /// <summary>
        /// Initialize our Client Proxy
        /// </summary>
        /// <param name="cfg">The Net Config of the Server/Client Application</param>
        public GearedClientProxy(string cfg)
        {
            // Init our config with the string version given,
            // then init our net client with the config and start listening
            // on our network for server interaction.
            this.config = new NetPeerConfiguration(cfg);
            this._client = new NetClient(this.config);
            this._client.Start();
        }
    }
}
