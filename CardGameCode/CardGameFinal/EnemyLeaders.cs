using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameFinal
{
    public class EnemyLeaders : DrawableGameComponent
    {
        int health = 1000;
        double maxMana = 200;
        double mana = 0;
        int[,] summonableCreatures;
        string leaderName;
        Boolean summonLoop = true;
        Game1 game;
        CreatureType[] unitType;
        List<Creatures> creatureArray;

        Texture2D creatureTexture;
        protected Rectangle drawRect;
        protected SpriteBatch spriteBatch;
        Boolean alive = false;


        Random rnd = new Random();



        public EnemyLeaders(Game1 theGame, string name, int hp, double maxMp, double mp, int[,] summons, ref List<Creatures> CA) : base(theGame)
        {
            leaderName = name;
            health = hp;
            maxMana = maxMp;
            mana = mp;
            summonableCreatures = summons;
            game = theGame;
            unitType = theGame.getCreatureTypeArray();
            creatureArray = new List<Creatures>();
            creatureArray = CA;
        }

  /*      protected override void LoadContent()
        {
            spriteBatch = game.getSpriteBatch();
            creatureArray.Add(new Creatures(game, 1300 - (float)rnd.NextDouble(), 400 - (float)rnd.NextDouble(), 3, 1, unitType[2]));
            creatureTexture = game.getTexture(3);
            drawRect = new Rectangle(1300, 400, 64, 64);
            alive = true;
        }*/

        public override void Update(GameTime gameTime)
        {
            if (mana < maxMana)
            {
                mana += 0.025;
            }
            if (mana >= maxMana)
            {
                while (summonLoop)
                {
                    int creatureToSummon = rnd.Next(0, summonableCreatures.GetLength(0));
                    if (summonableCreatures[creatureToSummon, 1] < mana)
                    {
                        creatureArray.Add(new Creatures(game, 1300 - (float)rnd.NextDouble(), 400 - (float)rnd.NextDouble(), summonableCreatures[creatureToSummon, 0], 1, unitType[summonableCreatures[creatureToSummon, 0]]));
                        game.componentAdd();
                        mana -= summonableCreatures[creatureToSummon, 1];
                    }
                    else summonLoop = false;
                }
                summonLoop = true;
            }

        }

        public void triggerUpdate(GameTime gametime)
        {
            Update(gametime);
        }
        public void triggerLoadContent()
        {
            LoadContent();
        }

  /*      public void triggerDraw(GameTime gametime)
        {
            Draw(gametime);
        }
*/
        public double getMana()
        {
            return mana;
        }


 //       public override void Draw(GameTime gameTime)
   //     {
   //         if (alive)
   //             spriteBatch.Draw(creatureTexture, drawRect, Color.White);
   //         else return;
   //     }
    }
}
