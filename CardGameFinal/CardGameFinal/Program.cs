
using System;

using var game = new CardGameFinal.Game1();
try
{
    game.Run();
}catch(Exception e)
{
    Console.WriteLine(e.ToString());
}