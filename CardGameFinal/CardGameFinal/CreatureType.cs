using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameFinal
{
    public class CreatureType
    {
        int health = 10;
        int[] statuses;
        float speed = 60f;
        int armor = 0;
        int damage = 1;
        double range = 1000;
        string attackType;


        public CreatureType(int hp, float spd, int amr, int dmg, double rng, string at)
        {
            health = hp;
            speed = spd;
            armor = amr;
            damage = dmg;
            range = rng;
            attackType = at;
        }

        public int getHealth()
        {
            return health;
        }
        public int[] getStatuses()
        {
            return statuses;
        }
        public float getSpeed()
        {
            return speed;
        }
        public int getArmor()
        {
            return armor;
        }
        public int getDamage()
        {
            return damage;
        }
        public double getRange()
        {
            return range;
        }
        public string getAttackType() 
        { 
            return attackType;
        }



    }


}
