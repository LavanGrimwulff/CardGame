using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;

namespace CardGameFinal
{
    public class LibraryComplier
    {
        int numberOfCards;
        CardClass[] cardList;
        string line;
        string path = Path.Combine(Directory.GetCurrentDirectory(), "CardDatabase.txt");

        public LibraryComplier() 
        {
            var lineCount = File.ReadLines(path).Count();
            numberOfCards = (lineCount - 8)/5;
        }

        public CardClass[] compileCards(Game1 theGame) 
        {
            int c = 0;
            string n= " ";
            string t = " ";
            Boolean sus = false;
            Boolean ats = false;
            Boolean sts = false;
            string tt = " ";
            int[,] uts = null;
            int[,] dtt = null;
            int[,] stt = null;
            string[] temp;


            try 
            {
                StreamReader inputreader = new StreamReader(path);

            cardList = new CardClass[numberOfCards];

    //First 8 lines of file are template info, iterate through them so we don't get garbage data
            for(int i = 0; i < 8; i++)
                {
                    line = inputreader.ReadLine();
                }

            for (int i = 0; i < numberOfCards; i++)
            {
                    for (int j = 0; j < 4; j++) 
                    {
                        line = inputreader.ReadLine();
                        if (j == 0) 
                        {
                            temp = line.Split(',');
                            c = Int32.Parse(temp[0]);
                            n = temp[1];
                            t = temp[2];
                            sus = Convert.ToBoolean(temp[3]);
                            ats = Convert.ToBoolean(temp[4]);
                            sts = Convert.ToBoolean(temp[5]);
                            tt = temp[6];
                        }
                        else if (j == 1) 
                        {
                            temp = line.Split(',');
                            uts = new int[temp.Length/2,2];
                            for (int k = 0; k < temp.Length; k++)
                            {

                                if (k % 2 == 0)
                                {
                                    uts[k / 2, 0] = Int32.Parse(temp[k]);
                                }
                                else
                                {
                                    uts[k / 2, 1] = Int32.Parse(temp[k]);
                                }
                            }
                        }
                        else if (j == 2) {
                            temp = line.Split(',');
                            dtt = new int[temp.Length/2,2];
                            for (int k = 0; k < temp.Length; k++)
                            {
                                if (k % 2 == 0)
                                {
                                    dtt[k / 2, 0] = Int32.Parse(temp[k]);
                                }else
                                dtt[k / 2, 1] = Int32.Parse(temp[k]);
                            }
 
                        }
                        else if (j == 3) 
                        {
                            temp = line.Split(',');
                            stt = new int[temp.Length/2,2];
                            for (int k = 0; k < temp.Length; k++)
                            {
                                if (k % 2 == 0)
                                {
                                    stt[k / 2, 0] = Int32.Parse(temp[k]);
                                }else
                                stt[k / 2,1] = Int32.Parse(temp[k]);
                            }
                        }

                    }
                    cardList[i] = new CardClass(theGame, c, n, t, sus, ats, sts, tt, uts, dtt, stt);
                    line = inputreader.ReadLine();
                    System.Diagnostics.Debug.WriteLine("We added card " + i);
                }
            }
            catch (Exception e) { Console.WriteLine("Exception: " + e.Message); }
            return cardList;
        }

        public int getNumberOfCards()
        {
            return numberOfCards;
        }
        public string getPath()
        {
            return path;
        }
    }
}
