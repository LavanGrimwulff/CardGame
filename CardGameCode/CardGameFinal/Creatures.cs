using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CardGameFinal
{
    public class Creatures : DrawableGameComponent
    {
        int health = 10;
        int[] statuses;
        float speed = 60f;
        int armor = 0;
        int damage = 1;
        double range = 1000;
        int target;
        string attackType;
        int creatureType;
        Boolean alive = true;
        Vector2 moveLocation;
        int dFrames = 0;


        Texture2D creatureTexture;


        protected Texture2D texture;
        protected Rectangle drawRect;
        protected SpriteBatch spriteBatch ;

        protected Vector2 Position;
        protected Vector2 Direction;

        protected Game1 game;
        protected Random rand;
        protected float screenWidth;
        protected float screenHeight;



        

        public Creatures(Game1 theGame, float ix, float iy, int type, int targetFaction, CreatureType cType) : base(theGame)
        {
            creatureType = type;
            target = targetFaction;
            game = theGame;
            Position.X = ix;
            Position.Y = iy;
            creatureTexture = theGame.getTexture(0);
            moveLocation.X = 900f;
            moveLocation.Y = 500f;
            
            health = cType.getHealth();

            speed = cType.getSpeed();
            armor = cType.getArmor();
            damage = cType.getDamage();
            range = cType.getRange();
            attackType = cType.getAttackType();

            spriteBatch = game.getSpriteBatch();
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        protected override void LoadContent()
        {

            screenWidth = game.getScreenWidth();
            screenHeight = game.getScreenHeight();

            creatureTexture = game.getTexture(creatureType);
            if(creatureType != 3)
                drawRect = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
            else drawRect = new Rectangle((int)Position.X, (int)Position.Y, 64, 64);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            float eTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            drawRect.X = (int)Position.X;
            drawRect.Y = (int)Position.Y;

            if (Direction.X != 0 || Direction.Y != 0)
            {
                Direction.Normalize();
            }
            Position = Position + Direction * speed * eTime;

            dFrames++;


            if (health <= 0)
            {
                alive = false;
            }
        }


        public int getCreatureType()
        {
            return creatureType;
        }

        public void attack(Creatures otherCreature)
        {
            
            if (dFrames >=60)
            {
                otherCreature.takeDamage(damage);
                dFrames = 0;
            }
            
        }

        public void takeDamage(int damage) 
        {
            damage = damage - armor;
            if (damage > 0)
            {
                health = health - damage;
            }
            
        }

        public void setMove(Vector2 move)
        {
         //   moveLocation = move;
            Direction = move;
        }

        public Vector2 getPosition() 
        {
            return Position;
        }

        public double getDistance(Creatures otherCreature)
        {
            Vector2 otherPosition = otherCreature.getPosition();
            double distance = (Position.X - otherPosition.X) * (Position.X - otherPosition.X) + (Position.Y - otherPosition.Y) * (Position.Y - otherPosition.Y);
            return distance;
        }

        public Boolean getAlive()
        {
            return alive;
        }

        public double getRange()
        {
            return range;
        }
        public int getTarget()
        {
            return target;
        }

        public override void Draw(GameTime gameTime)
        {

            if(!alive) { return; }
            else if (alive)
            {
                creatureTexture = game.getTexture(creatureType);
                spriteBatch.Draw(creatureTexture, drawRect, Color.White);
            }

            
        }
    }
}
