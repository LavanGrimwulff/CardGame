using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Color = Microsoft.Xna.Framework.Color;


namespace CardGameFinal
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        SpriteFont Font1;
        Random rand;
        double eTime;
        double elapsedTime;
        float screenWidth, screenHeight;


        double distanceDefault = 40000000;
        double distance;
        Boolean foundTarget;
        double collisionDefault = 100;
        double collisionRange = 100;
        double currentCollision;
        int collisionTarget;
        Boolean foundCollision = false;
        Vector2 nullMove = Vector2.Zero;



        MouseState mouseState;
        MouseState prevMouseState;
        Vector2 mousePosition;
        KeyboardState prevKeyboardState;
        double mana = 30;
        double maxMana = 100;

        SoundEffect explosion;
        Texture2D spelltexture;


        protected Texture2D cardback;
        protected Texture2D[] unitTextures;
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Textures.txt");
        string line;
        string[] temp;
        string tempword;
        int[,] incrementAmount = new int[1, 2];

        Boolean Space = false;
        Boolean Shift = false;
        Boolean CTRL = false;
        Boolean Tab = false;
        Boolean LeftClick = false;
        Boolean RightClick = false;
        Boolean enlarge = false;

        EnemyLeaders enemyLeader;

        int currentHandSize;
        int currentUnits;
        int cardsInGame;
        int cardAtPos;

        double currentDistance;
        int currentTarget;
        string datapath;
        public List<Creatures> creatureArray;
        CreatureType[] creatureTypeArray;
        CardManager cardManager;
        CardClass[] cardList;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1600;
            IsFixedTimeStep = false;

            // create new random number generator
            rand = new Random((int)DateTime.Now.Ticks);
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            nullMove.X = 0;
            nullMove.Y = 0;



            Font1 = Content.Load<SpriteFont>("NewSpriteFont");
            cardback = Content.Load<Texture2D>("Images/CardBack");

            creatureArray = new List<Creatures>();

            //Load all cards
            LibraryComplier libraryCompiler = new LibraryComplier();
            cardsInGame = libraryCompiler.getNumberOfCards();
            cardList = new CardClass[cardsInGame];
            cardList = libraryCompiler.compileCards(this);
            datapath = libraryCompiler.getPath();

            //Start the card manager, manages deck/hand/graveyard
            cardManager = new CardManager(this);
            cardManager.setDeck("Default");

            //set up array that holds the name of textures
            StreamReader inputreader = new StreamReader(path);
            line = inputreader.ReadLine();
            temp = line.Split(',');
            unitTextures = new Texture2D[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                tempword = String.Concat("Images/" + temp[i]);
                unitTextures[i] = Content.Load<Texture2D>(tempword);
            }

            //Load creature type data into array, reading first two lines to remove template data
            path = Path.Combine(Directory.GetCurrentDirectory(), "CreatureTypes.txt");
            inputreader = new StreamReader(path);
            var lineCount = File.ReadLines(path).Count();
            creatureTypeArray = new CreatureType[lineCount];
            line = inputreader.ReadLine();
            line = inputreader.ReadLine();
            for (int i = 0; i < lineCount - 2; i++)
            {
                line = inputreader.ReadLine();
                temp = line.Split(',');
                int hp = Int32.Parse(temp[0]);
                float spd = float.Parse(temp[1]);
                int amr = Int32.Parse(temp[2]);
                int dmg = Int32.Parse(temp[3]);
                double rng = double.Parse(temp[4]);
                string at = temp[5];
                creatureTypeArray[i] = new CreatureType(hp, spd, amr, dmg, rng, at);
            }

            //Load texture for spells
            spelltexture = Content.Load<Texture2D>("Images/explosionsheet");

            //Load enemy leader summon data
            path = Path.Combine(Directory.GetCurrentDirectory(), "EnemyLeader.txt");
            inputreader = new StreamReader(path);
            lineCount = File.ReadLines(path).Count();
            int[,] enemySummons = new int[2,2];
            line = inputreader.ReadLine();
            temp = line.Split(',');
            enemySummons[0, 0] = Int32.Parse(temp[0]);
            enemySummons[1, 0] = Int32.Parse(temp[1]);
            line = inputreader.ReadLine();
            temp = line.Split(',');
            enemySummons[0, 1] = Int32.Parse(temp[0]);
            enemySummons[1, 1] = Int32.Parse(temp[1]);

        //Summon enemy leader
            enemyLeader = new EnemyLeaders(this, "test", 1000, 100, 40, enemySummons, ref creatureArray);

          //  spriteBatch.Begin();
            creatureArray.Add(new Creatures(this, 1300, 400, 3, 1, creatureTypeArray[2]));
            Components.Add(creatureArray[0]);
         //   spriteBatch.End();


            base.Initialize();
        }

        protected override void LoadContent()
        {
   //         spriteBatch = new SpriteBatch(GraphicsDevice);
            IsMouseVisible = true;
            cardManager.triggerLoadContent();
            enemyLeader.triggerLoadContent();


            screenWidth = Window.ClientBounds.Width;
            screenHeight = (float)(Window.ClientBounds.Height);

            //Load sound effect
            explosion = Content.Load<SoundEffect>("Audio/Explosion");
            

            //Get starting hand of 7 cards
            cardManager.redrawHand();



            // TODO: use this.Content to load your game content here
        }

        private void UpdateInput()
        {
            KeyboardState newState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            // Check to see whether the Spacebar is down.
            if (newState.IsKeyDown(Keys.Space) && newState != prevKeyboardState)
            {
                Space = true;
            }
            // Otherwise, check to see whether it was down before.
            else if (prevKeyboardState.IsKeyDown(Keys.Space))
            {
                Space = false;
            }

            // Check to see whether the Shift is down.
            if (newState.IsKeyDown(Keys.LeftShift) && newState != prevKeyboardState)
            {
                Shift = true;
            }
            // Otherwise, check to see whether it was down before.
            else if (prevKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                Shift = false;
            }

            // Check to see whether the Control is down.
            if (newState.IsKeyDown(Keys.LeftControl) && newState != prevKeyboardState)
            {
                CTRL = true;
            }
            // Otherwise, check to see whether it was down before.
            else if (prevKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                CTRL = false;
            }

            // Check to see whether the Tab is down.
            if (newState.IsKeyDown(Keys.Tab) && newState != prevKeyboardState)
            {
                Tab = true;
            }
            // Otherwise, check to see whether it was down before.
            else if (prevKeyboardState.IsKeyDown(Keys.Tab))
            {
                Tab = false;
            }

            // Check to see whether the LeftClick is down.
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                LeftClick = true;
            }
            // Otherwise, check to see whether it was down before.
            else if (prevMouseState.LeftButton == ButtonState.Pressed)
            {
                LeftClick = false;
            }

            // Check to see whether the RightClick is down.
            if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
            {
                RightClick = true;
            }
            // Otherwise, check to see whether it was down before.
            else if (prevMouseState.RightButton == ButtonState.Pressed)
            {
                RightClick = false;
            }

            prevKeyboardState = newState;
            prevMouseState = mouseState;
        }

        protected override void Update(GameTime gameTime)
        {
            cardManager.triggerUpdate(gameTime);
            enemyLeader.triggerUpdate(gameTime);
            for (int i = 0; i < cardList.Length; i++)
            {
                cardList[i].Update(gameTime);
            }


            UpdateInput();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            elapsedTime = gameTime.ElapsedGameTime.Ticks;
            eTime = (double)elapsedTime / (double)TimeSpan.TicksPerSecond;
            mouseState = Mouse.GetState();
            mousePosition.X = mouseState.X;
            mousePosition.Y = mouseState.Y;
            //Test code that spawns a unit, spacebar spawns friendly, shift spawns enemy
            if (Space)
            {
                mousePosition.X = mouseState.X;
                mousePosition.Y = mouseState.Y;
                incrementAmount = cardList[0].execute(mousePosition, ref creatureArray, this);
                while (incrementAmount[0, 1] > 0)
                {
                    Components.Add(creatureArray[incrementAmount[0, 0]]);
                    incrementAmount[0, 0]++;
                    incrementAmount[0, 1]--;
                }
            }
            if (Shift)
            {
                mousePosition.X = mouseState.X;
                mousePosition.Y = mouseState.Y;
                creatureArray.Add(new Creatures(this, mousePosition.X, mousePosition.Y, 1, 1, creatureTypeArray[1]));
                Components.Add(creatureArray[creatureArray.Count - 1]);
            }

            if (CTRL)
            {
                cardManager.draw();
            }
            if (Tab)
            {
                cardManager.removeFromHand(0);
            }
            if (LeftClick && cardManager.isButton(mousePosition))
            {
                if (!cardManager.getRedrawCosts())
                {
                    cardManager.clickRedraw();
                }else if(cardManager.getRedrawCosts() && mana >= 20)
                {
                    cardManager.clickRedraw();
                    mana -= 20;
                }
            }
            if (LeftClick && !enlarge)
            {
                if (!cardManager.isThereSelected())
                {
                    cardAtPos = cardManager.getCardAtPos(mousePosition);
                    if (cardAtPos != -1)
                    {
                        cardManager.selectCard(cardAtPos);
                    }
                }
                else
                {
                    if (mana >= cardList[cardManager.getCardInHandID(cardAtPos)].getCost())
                    {
                        incrementAmount = cardList[cardManager.getCardInHandID(cardAtPos)].execute(mousePosition, ref creatureArray, this);
                        //  incrementAmount = cardList[0].execute(mousePosition, ref creatureArray, this);
                        while (incrementAmount[0, 1] > 0)
                        {
                            Components.Add(creatureArray[incrementAmount[0, 0]]);
                            incrementAmount[0, 0]++;
                            incrementAmount[0, 1]--;
                        }
                        mana -= cardList[cardManager.getCardInHandID(cardAtPos)].getCost();
                        cardManager.deselect();
                        cardManager.removeFromHand(cardAtPos);
                    }
                }
            }
            if (RightClick && cardManager.isThereSelected())
            {
                cardManager.deselect();
            }else if (RightClick && !enlarge)
            {
                cardAtPos = cardManager.getCardAtPos(mousePosition);
                if (cardAtPos != -1)
                {
                    cardManager.enlargeCard(cardAtPos);
                    enlarge = true;
                }
            }else if (RightClick && enlarge)
            {
                cardManager.enlargeStop();
                enlarge =false;
            }

            //remove creatures that aren't alive
            for (int i = 0; i < creatureArray.Count; i++)
            {
                if (creatureArray[i].getAlive() == false)
                {
                    creatureArray.Remove(creatureArray[i]);
                }
            }

            //Basic logic for units to find a target
            for (int i = 0; i < creatureArray.Count; i++)
            {
                distance = distanceDefault;
                foundTarget = false;
                collisionRange = collisionDefault;
                foundCollision = false;
                for (int j = 0; j < creatureArray.Count; j++)
                {
                    if (creatureArray[i].getTarget() != creatureArray[j].getTarget())
                    {
                        if (i != j)
                        {
                            currentDistance = creatureArray[i].getDistance(creatureArray[j]);

                            if (currentDistance < distance)
                            {
                                distance = currentDistance;
                                currentTarget = j;
                                foundTarget = true;
                            }
                        }
                    }
                    else
                    {
                        if (i != j)
                        {
                            currentCollision = creatureArray[i].getDistance(creatureArray[j]);

                            if (currentCollision < collisionRange)
                            {
                                collisionRange = currentCollision;
                                collisionTarget = j;
                                foundCollision = true;
                            }
                        }
                    }
                }
                if (distance <= creatureArray[i].getRange() && foundTarget)
                {
                    creatureArray[i].attack(creatureArray[currentTarget]);
                    creatureArray[i].setMove(nullMove);

                }
                else if (foundCollision)
                {
                    creatureArray[i].setMove(creatureArray[i].getPosition() - creatureArray[collisionTarget].getPosition());
                }

                else if (distance > creatureArray[i].getRange() && foundTarget)
                {
                    creatureArray[i].setMove(creatureArray[currentTarget].getPosition() - creatureArray[i].getPosition());
                }
                else
                {
                    creatureArray[i].setMove(nullMove);
                }




                currentHandSize = cardManager.getHandSize();
                base.Update(gameTime);
            }
            if (mana < maxMana)
            {
                mana += 0.3;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            base.Draw(gameTime);  // draw everything else in the game
            cardManager.Draw(gameTime);
            enemyLeader.Draw(gameTime);
            for(int i = 0;i<cardList.Length;i++)
            {
                cardList[i].Draw(gameTime,spelltexture);
            }


            double fps = 1.0 / eTime;  // calculate and display frame rate and count of alive/total units.
            spriteBatch.DrawString(Font1, "fps " + fps.ToString("f6"), new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(Font1, "Creatures in play: " + creatureArray.Count, new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(Font1, "Cards in hand: " + currentHandSize, new Vector2(10, 50), Color.White);
            spriteBatch.DrawString(Font1, "Screen Width: " + screenWidth, new Vector2(10, 70), Color.White);
            spriteBatch.DrawString(Font1, "Screen Height: " + screenHeight, new Vector2(10, 90), Color.White);
            spriteBatch.DrawString(Font1, "Mouse Location: " + mousePosition, new Vector2(10, 110), Color.White);
            spriteBatch.DrawString(Font1, "Current mana: " + mana, new Vector2(10, 130), Color.White);
            spriteBatch.DrawString(Font1, "Enemy mana: " + enemyLeader.getMana(), new Vector2(10, 150), Color.White);
            spriteBatch.DrawString(Font1, "Card List : " + cardList.Length, new Vector2(10, 170), Color.White);



            spriteBatch.End();
        }
        public SpriteBatch getSpriteBatch()
        {
            return spriteBatch;
        }
        public float getScreenWidth()
        {
            return screenWidth;
        }

        public float getScreenHeight()
        {
            return screenHeight;
        }

        public Texture2D getTexture(int a)
        {
            return unitTextures[a];
        }

        public Texture2D getCardBack()
        {
            return cardback;
        }

        public SpriteFont getFont()
        {
            return Font1;
        }

        public CreatureType[] getCreatureTypeArray()
        {
            return creatureTypeArray;
        }

        public void componentAdd()
        {
            Components.Add(creatureArray[creatureArray.Count - 1]);
        }

        public void playExplosion()
        {
            explosion.Play();
        }

        public Texture2D getSpellTexture()
        {
            return spelltexture;
        }
    }
}