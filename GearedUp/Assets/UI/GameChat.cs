using GearedUpEngine.Assets.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GearedUpEngine.Assets.UI
{
    public class GameChat

    {
        SpriteFont font;
        float fadeTimer;
        string[] chat;
        Color drawColor;
        int charMax;
        KeyboardState kStateOld;
        public bool IsChatting { get; set; }
        public string UserInput { get; set; }
        public bool DoAction { get; set; }
        public GameChat(SpriteFont newFont)
        {
            this.font = newFont;
            this.fadeTimer = 0f;
            this.chat = new string[0];
            this.drawColor = new Color(255, 255, 255, 255);
            this.IsChatting = false;
            this.charMax = 24;
        }
        public void Update(KeyboardState kState)
        {
            if (this.kStateOld == null) this.kStateOld = kState;

            if (kState.IsKeyDown(Keys.Escape) && this.IsChatting)
                this.IsChatting = false;

            if (this.IsChatting)
            {
                
                foreach (char c in TypeManager.GetKeys(kState, this.kStateOld))
                {
                    if (this.UserInput.Length < this.charMax)
                        this.UserInput = (c == char.MinValue) ?
                            this.UserInput
                            :
                            (c == char.MaxValue) ?
                            (this.UserInput.Length > 0) ?
                            this.UserInput.Substring(0, this.UserInput.Length - 1)
                            :
                            ""
                            :
                            this.UserInput += c.ToString();
                    else if (this.UserInput.Length >= this.charMax)
                        this.UserInput = this.UserInput.Substring(0, this.charMax - 1);
                    else break;
                }

                this.drawColor = Color.White;
                this.fadeTimer = 255;

                if (kState.IsKeyDown(Keys.Enter))
                {
                    this.DoAction = true;
                    this.IsChatting = false;
                }
            }
            else
            {
                this.UserInput = "";
                this.fadeTimer = (this.fadeTimer > 0) ? this.fadeTimer - 0.5f : 0;
                this.drawColor = new Color((int)this.fadeTimer, (int)this.fadeTimer, (int)this.fadeTimer, (int)this.fadeTimer);
            }

            if (kState.IsKeyDown(Keys.T))
                this.IsChatting = true;

            this.kStateOld = kState;
        }
        public void AppendChat(string newMessage)
        {
            Array.Resize<string>(ref this.chat, this.chat.Length + 1);
            this.chat[this.chat.Length - 1] = newMessage;
            if (this.chat.Length > 10)
            {
                string[] temp = new string[10];
                for (int i = 0; i < temp.Length; i++)
                    temp[i] = this.chat[i + 1];
                this.chat = temp;
            }
            this.fadeTimer = 255;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            {
                Vector2 drawPosition = new Vector2(10, 320);
                if (this.IsChatting)
                {
                    spriteBatch.DrawString(this.font, "You: " + this.UserInput, drawPosition, this.drawColor);
                }
            }

            //draw prior chats
            {
                Vector2 drawPosition = new Vector2(10, 10);
                for (int i = this.chat.Length - 1; i >= 0; i--)
                {
                    spriteBatch.DrawString(this.font, this.chat[i], drawPosition, this.drawColor);
                    drawPosition.Y += 15;
                }
            }
        }
    }
}
