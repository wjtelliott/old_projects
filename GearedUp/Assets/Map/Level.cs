using GearedUpEngine.Assets.Entities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GearedUpEngine.Assets.Map
{
    public class Level
    {
        public string LevelName { get; set; }

        public Tilemap Tilemap { get; set; }
        public ClientPlayer[] Client_Players { get; set; }
        public ServerPlayer[] Server_Players { get; set; }
        public bool IsServerLevel { get; private set; }

        public Level(bool isServer, string levelName)
        {
            this.LevelName = levelName;
            this.IsServerLevel = isServer;
            if (this.IsServerLevel)
                this.Server_Players = new ServerPlayer[0];
            else this.Client_Players = new ClientPlayer[0];
        }

        public void Update()
        {
            if (this.IsServerLevel)
                foreach (ServerPlayer player in this.Server_Players)
                    player.Update();
            else foreach (ClientPlayer player in this.Client_Players)
                    player.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Tilemap.Draw(spriteBatch);
            foreach (ClientPlayer player in this.Client_Players)
                player.Draw(spriteBatch);
        }
    }
}
