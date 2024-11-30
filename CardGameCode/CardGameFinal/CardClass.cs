using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System.Reflection.Metadata;

namespace CardGameFinal
{
    public class CardClass : DrawableGameComponent
    {
        protected int cost;
        protected string name;
        protected string text;

        protected Boolean summonSpell;
        protected Boolean attackSpell;
        protected Boolean statusSpell;
        protected int[,] unitsToSummon;
        protected int[,] damageToTargets;
        protected int[,] statusToApply;
        protected string targetType; //Declares friendly or enemy for status type spells
        protected Game1 game;

        Texture2D spelltexture;
        protected Rectangle[] spellvisuals;
        protected Rectangle[] sourceRect = new Rectangle[25];
        Vector2 visualSize = new Vector2(320,320);
        Vector2 origin = new Vector2(0,0);
        protected SpriteBatch spriteBatch;

        Random rnd = new Random();
        int b = 0;
        float timer = 0;
        float timerThreshold = 0.1f;


        public CardClass(Game1 theGame,int c,string n, string t, Boolean sus, Boolean ats, Boolean sts, string tt, int[,] utt, int[,] dtt, int[,] stt) : base(theGame)
        {
            cost = c; name = n; text = t; summonSpell = sus; attackSpell = ats; statusSpell = sts; targetType = tt;
            unitsToSummon = new int[utt.GetLength(0),utt.GetLength(1)];
            unitsToSummon = utt;
            damageToTargets = new int[dtt.GetLength(0), dtt.GetLength(1)];
            damageToTargets = dtt;
            spellvisuals = new Rectangle[3];
            statusToApply = new int[stt.GetLength(0), stt.GetLength(1)];
            statusToApply = stt;
            game = theGame;
            spelltexture = game.getSpellTexture();
            spriteBatch = game.getSpriteBatch();

            for(int i = 0;i<25;i++)
            {
                sourceRect[i] = new Rectangle(i % 5 * 64, i / 5 * 64, 64, 64);
            }

        }



        public override void Update(GameTime gameTime)
        {
            float eTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += eTime;
            if ((timer >timerThreshold)&& b<24)
            {
                b++;
                timer = 0;
            }

        }

            //Have the card perform all its effects at the mouse location
            public int[,] execute( Vector2 pos,ref List<Creatures> CA, Game1 theGame)
        {
            List<Creatures> creatureArray = new List<Creatures>();
            creatureArray = CA;
            int currentNumber = creatureArray.Count;
            int unitsAdded = 0;
            int[,] incrementAmount = new int[1,2];
            if (summonSpell)
            {
                CreatureType[] unitType = theGame.getCreatureTypeArray();
                for (int i = 0; i < unitsToSummon.GetLength(0); i++)
                {
                    pos.X -= (float)rnd.NextDouble();
                    pos.Y -= (float)rnd.NextDouble();
                    //summon unitsToSummon[i][1] of unit ID unitsToSummon[i][0]

                    for (int j = 0; j < unitsToSummon[i, 1]; j++)
                    {
                        creatureArray.Add(new Creatures(theGame, pos.X, pos.Y, unitsToSummon[i,0], 0, unitType[i]));
                        unitsAdded++;
                    }

                }
            }
            if (attackSpell)
            {
                //Create an array to store targets in, size of the array equals how many targets you can affect
                int[,] targets = new int[damageToTargets[0,0],2];
                int numberOfTargets = targets.GetLength(0);


                //Initialize the array
                for (int i = 0; i < targets.Length/2; i++)
                {
                    targets[i,0] = -1;
                    targets[i,1] = 999999999;
                }
                //get closest targets to cast location
                for(int i = 0; i<creatureArray.Count; i++)
                {
                    //faction check
                    if (creatureArray[i].getTarget() == 1)
                    {
                        //Check distance to each creature
                        Vector2 creaturePos = creatureArray[i].getPosition();
                        double distance = (pos.X - creaturePos.X) * (pos.X - creaturePos.X) + (pos.Y - creaturePos.Y) * (pos.Y - creaturePos.Y);
                        Boolean placed = false;
                        for (int j = 0; j < numberOfTargets; j++)
                        {
                            //Compare the distance to whats already in the array
                            if (distance < targets[j, 1] && !placed)
                            {
                                //Move everyone one slot over to make room
                                for (int k = 0; k < numberOfTargets - j - 1; k++)
                                {
                                    targets[targets.GetLength(0) - 1 - k, 0] = targets[targets.GetLength(0) - 2 - k, 0];
                                    targets[targets.GetLength(0) - 1 - k, 1] = targets[targets.GetLength(0) - 2 - k, 1];
                                }
                                targets[j, 0] = i;
                                targets[j, 1] = (int)distance;
                                placed = true;
                            }
                        }
                    }
                }
                // deal damageToTargets[i][1] to the closest array size targets
                for (int i = 0; i < numberOfTargets; i++)
                {
                    if (targets[i, 0] != -1)
                    {
                        creatureArray[targets[i,0]].takeDamage(damageToTargets[0, 1]);
                        spellvisuals[i] = new Rectangle((int)creatureArray[targets[i, 0]].getPosition().X, (int)creatureArray[targets[i, 0]].getPosition().Y, 32,32);
                    }
                }
                if (targets[0, 0] != -1)
                    b = 0;
                    game.playExplosion();
            }
            if (statusSpell)
            {
                for (int i = 0; i < statusToApply.Length; i++)
                {
                    if (targetType == "Friendly")
                    {
                        //get closest targets to cast location
                    }
                    if (targetType == "Enemy")
                    {
                        //get closest targets to cast location
                    }
                    // apply status StatusToApply[i][0] to the closest StatusToApply[i][1] targets
                }
            }
            incrementAmount[0, 0] = currentNumber;
            incrementAmount[0, 1] = unitsAdded;
            return incrementAmount;
        }

     //All the get/set functionality 
        public void setCost(int a)
        {
            cost = a;
        }

        public int getCost()
        {
            return cost;
        }

        public void setName(string a)
        {
            name = a;
        }

        public string getName()
        {
            return name;
        }

        public void setText(string a)
        {
            text = a;
        }

        public string getText()
        {
            return text;
        }

        public void setSpellType(Boolean summon, Boolean attack, Boolean status)
        {
            summonSpell = summon;
            attackSpell = attack;
            statusSpell = status;
        }

        public void setUnits(int[,] a)
        {
            unitsToSummon = a;
        }
        public void setDamage(int[,] a)
        {
            damageToTargets = a;
        }

        public void setStatus(int[,] a)
        {
            statusToApply = a;
        }

        public void Draw(GameTime gameTime, Texture2D a)
        {

                for (int i = 0; i < spellvisuals.Length; i++)
                {
                Vector2 spellPosition;
                spellPosition.X = spellvisuals[i].X;
                spellPosition.Y = spellvisuals[i].Y;
               //  spriteBatch.Draw(a, spellvisuals[i], sourceRect, Color.White);
                spriteBatch.Draw(a, spellvisuals[i], sourceRect[b], Color.White);
                 }

        }

    }
}
