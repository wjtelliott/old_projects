using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GearedUpEngine.Assets.Util
{
    public sealed class DedicatedServerDisplay
    {
        string[] lines;
        public DedicatedServerDisplay()
        {
            this.lines = new string[0];
        }
        public void Append(string message, Color color)
        {
            string appended = Tokenize(message, color);
            Append(ref this.lines, appended);
        }
        public void Append(string message)
        {
            Append(ref this.lines, message);
        }
        void Append(ref string[] arrayToAppend, string addition)
        {
            Array.Resize<string>(ref arrayToAppend, arrayToAppend.Length + 1);
            arrayToAppend[arrayToAppend.Length - 1] = addition;
        }
        string Tokenize(string text, Color c)
        {
            return
                (c == Color.White) ? string.Format("[0]{0}", text) :
                (c == Color.Green) ? string.Format("[1]{0}", text) :
                (c == Color.Red) ? string.Format("[2]{0}", text) :
                string.Format("[0]{0}", text);
        }
        Color GetColorFromCode(string code)
        {
            switch (code)
            {
                default:
                case "[0]":
                    return Color.White;
                case "[1]":
                    return Color.Green;
                case "[2]":
                    return Color.Red;
            }
        }
        public void Draw(SpriteBatch spriteBatch, FontManager fontManager)
        {

            int position = 460,
                offset = 20,
                offsetIndexer = 0;

            for (int i = this.lines.Length - 1; i >= 0; i--)
            {
                Vector2 stringPosition = new Vector2(10, position - (offset * offsetIndexer));
                
                Color stringColor = GetColorFromCode(this.lines[i].Substring(0, 3));

                spriteBatch.DrawString(fontManager.GetFontByName("console"), this.lines[i].Substring(3), stringPosition, stringColor);

                offsetIndexer++;
            }


            spriteBatch.DrawString(fontManager.GetFontByName("console"), "Version: dev_01", new Vector2(670, 460), Color.Aqua);
        }
    }
}
