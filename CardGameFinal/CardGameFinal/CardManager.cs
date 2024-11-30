using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Reflection.Metadata;


namespace CardGameFinal
{
    public class CardManager : DrawableGameComponent
    {
        List<int> deck = new List<int>();
        List<int> hand = new List<int>();
        List<int> graveyard = new List<int>();

    //variables for displaying hand
        protected List<Rectangle> drawRect = new List<Rectangle>();
        protected List<Rectangle> drawRectPicture = new List<Rectangle>();
        protected Rectangle enlargeRect;
        protected Rectangle enlargePic;
        protected int enlargeID = -1;
        Vector2 handCenterPos;
        Vector2 cardPos;
        float handWidth;
        Texture2D cardTexture;
        int cardWidth = 64;
        int cardHeight = 128;
        float gap = 1.1f;
        SpriteFont font;

    //Variables for showing card selected
        Boolean hasSelected = false;
        int cardSelected;
        Rectangle selectRect;
        Texture2D greenRectangle;

        //For draw new hand button
        Rectangle redrawRect;
        Texture2D redrawCost;
        Texture2D redrawFree;
        Boolean redrawCosts = true;
        float redrawDefaultTime = 20;
        float redrawCurrentTime;


        string testString = "Name of Card";

        string currentDeckName = "default";






        private static Random rng = new Random();
        protected Texture2D texture;
        protected SpriteBatch spriteBatch;
        protected float screenWidth;
        protected float screenHeight;
        protected Game1 game;

        string line;
        string[] temp;
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Decks\\DefaultDeck.txt");

        public CardManager(Game1 theGame) : base(theGame)
        { 
            game = theGame;
            redrawCost = game.Content.Load<Texture2D>("Images/redrawCost");
            redrawFree = game.Content.Load<Texture2D>("Images/redrawFree");

        }

        protected override void LoadContent()
        {
            spriteBatch = game.getSpriteBatch();
            screenWidth = game.getScreenWidth();
            screenHeight = game.getScreenHeight();
            handCenterPos.X = screenWidth / 2 - cardWidth/2;
            handCenterPos.Y = screenHeight - cardHeight-5;
            cardPos.Y = handCenterPos.Y;

            cardTexture = game.getCardBack();
            font = game.getFont();

            greenRectangle = new Texture2D(GraphicsDevice, 1, 1);
            greenRectangle.SetData(new[] { Color.LightGreen });
            redrawRect = new Rectangle(100, 600, 128, 64);
            redrawCurrentTime = redrawDefaultTime;

        }

        public void triggerLoadContent()
        {
            LoadContent();
        }

        public void triggerUpdate(GameTime gametime)
        {
            Update(gametime);
        }

        public void triggerDraw(GameTime gametime)
        {
            Draw(gametime);
        }
        public override void Update(GameTime gameTime)
        {
            float eTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Draw rectangles for cards in hand
            drawRect.Clear();
            drawRectPicture.Clear();
            for (int i = 0; i < hand.Count; i++)
            {
                handWidth = (hand.Count - 1 - i * 2) * cardWidth * gap;
                cardPos.X = handCenterPos.X - (handWidth / 2);
                if(hasSelected && cardSelected == i)
                {
                    selectRect = new Rectangle((int)cardPos.X-5, (int)cardPos.Y-5, cardWidth+10, cardHeight+10);
                }
                drawRect.Add(new Rectangle((int)cardPos.X, (int)cardPos.Y, cardWidth, cardHeight));
                drawRectPicture.Add(new Rectangle((int)cardPos.X + 2, (int)cardPos.Y+ 14, 60, 60));

            }
            //Count down till redrawing hand is free
            redrawCurrentTime -= eTime;
            if (redrawCurrentTime <= 0) { redrawCosts = false; }
        }



            public void setDeck(string deckname)
        {
            StreamReader inputreader = new StreamReader(path);
            currentDeckName = deckname;
            line = inputreader.ReadLine();
            temp = line.Split(',');
            for(int i = 0; i < temp.Length; i++)
            {
                deck.Add(int.Parse(temp[i]));
            }
        }

        public void draw()
        {

            if (deck.Count > 0)
            {
                hand.Add(deck[0]);
                deck.RemoveAt(0);
            }
            else
            {
                addGraveyardIntoDeck();

                if (deck.Count > 0)
                {
                    hand.Add(deck[0]);
                    deck.RemoveAt(0);
                }
            } 
            LoadContent();
        }

        public void selectCard(int slot)
        {
            hasSelected = true;
            cardSelected = slot;
        }

        public Boolean isThereSelected()
        {
            return hasSelected;
        }

        public void deselect()
        {
            hasSelected = false;
        }

        public int getCardAtPos(Vector2 Pos) 
        {
            int card = -1;
            for (int i = 0; i < drawRect.Count; i++)
            {
                if (drawRect[i].X < Pos.X && drawRect[i].X + cardWidth > Pos.X && drawRect[i].Y < Pos.Y && drawRect[i].Y + cardHeight > Pos.Y)
                {
                    card = i;
                }
            }
            return card;
        }

        public void shuffle()
        {
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }

        public void addToHand(int a)
        {
            hand.Add(a);
        }

        public void enlargeCard(int a)
        {
            enlargeID = getCardInHandID(a);
            int X = (int)(screenWidth/2 - cardWidth*3/2);
            int Y = 100;
            enlargeRect = new Rectangle(X, Y, cardWidth*3, cardHeight*3);
            enlargePic = new Rectangle(X + 6, Y+42, 180, 180);
        }

        public void enlargeStop()
        {
            enlargeID = -1;
        }

        public void removeFromHand(int a) 
        {
            if (hand.Count > a)
            {
                addToGraveyard(getCardInHandID(a));
                hand.RemoveAt(a);
            }
            drawRect.Clear();

        }

        public int getHandSize()
        {
            return hand.Count;
        }

        public int getCardInHandID(int pos)
        {
            int cardID = hand[pos];
            return cardID;
        }

        public void addToGraveyard(int a)
        {

            graveyard.Add(a);
        }

        public string getDeckName()
        {
            return currentDeckName;
        }

        public void addGraveyardIntoDeck()
        {
            while(graveyard.Count > 0)
            {
                deck.Add(graveyard[0]);
                graveyard.RemoveAt(0);
            }
            shuffle();
        }

        public void redrawHand()
        {
            while(getHandSize() > 0)
            {
                removeFromHand(0);
            }  
            
            for(int i = 0; i < 7; i++)
            {
                draw();
            }
        }
        public void clickRedraw()
        {
            redrawHand();
            redrawCosts = true;
            redrawCurrentTime = redrawDefaultTime;
        }

        public Boolean getRedrawCosts()
        {
            return redrawCosts;
        }

        public Boolean isButton(Vector2 pos)
        {
            if(redrawRect.X < pos.X && redrawRect.X + 128 > pos.X && redrawRect.Y < pos.Y && redrawRect.Y + 64 > pos.Y)
                return true;
            else return false;
        }

        public override void Draw(GameTime gameTime)
        {
            if (hasSelected)
            {
                spriteBatch.Draw(greenRectangle, selectRect, Color.Green);
            }
            for (int i = 0; i < drawRect.Count; i++)
            {
                spriteBatch.Draw(cardTexture, drawRect[i], Color.White);
                texture = game.getTexture(getCardInHandID (i));
                spriteBatch.Draw(texture, drawRectPicture[i], Color.White);
                spriteBatch.DrawString(font, testString, new Vector2(drawRect[i].X + 5, drawRect[i].Y + 5), Color.Black);
            }

            if (redrawCosts)
            {
                spriteBatch.Draw(redrawCost, redrawRect, Color.White);
            }
            else
            {
                spriteBatch.Draw(redrawFree, redrawRect, Color.White);

            }


            if (enlargeID != -1)
            {
                spriteBatch.Draw(cardTexture, enlargeRect, Color.White);
                texture = game.getTexture(enlargeID);
                spriteBatch.Draw(texture, enlargePic, Color.White);
            }

        }

    }
}
