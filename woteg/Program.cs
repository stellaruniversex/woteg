using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime;
using System.Threading;
/*    1     2    3    4    5    6    7    8    9   10   11   12
  -------------------------------------------------------------
O |300 |360 |450 |600 |800 |1100|1400|1800|2500|4000|8000|9999| - 30 M-sun (12 age O-class star is 1,600,000,000 km in diameter, and more of a M-class supergiant)
B |180 |220 |270 |350 |480 |650 |900 |1200|1800|3000|6500|8000| - 10 M-sun
A |80  |100 |125 |160 |190 |225 |280 |400 |600 |850 |1500|3000| - 2.5 M-sun
F |20  |22  |25  |28  |33  |40  |60  |85  |120 |180 |500 |1800| - 1.5 M-sun
G |10  |11  |12  |13  |14  |16  |19  |25  |40  |100 |300 |1200| - 1 M-sun
K |2   |2   |3   |4   |5   |7   |10  |12  |15  |19  |25  |35  | - 0.5 M-sun
M |2   |1   |1   |1   |1   |1   |1   |1   |1   |1   |1   |1   | - 0.25 M-sun
  -------------------------------------------------------------*/
/*Habitability modifier:
 if the temperature is between 1 above and 1 below optimal, then habitability is +1, if 2 above or 2 below, +0, then decreases by 1 for each 1 temp level below or above optimal
 if the pressure is between 0 and 1 above, then habitability is +1, if between 2 above and 1 below, +0, then decreases by 1 for each 1 press level below or above optimal*/
 /*Resource dependent population modifier:
  Equal to the fourth root of total resources, with a flat +0.5 for each type of resources available, as more types of local resources allow easier production. If total resources are equal to 52, and there are 5 types of resources available, the population modifier is approx 5.19, this is by far the biggest modifier*/
/*Population is 10 to the power of the resource-dependent pop modifier + habitability + proximity to a 9+ population planet (+1 max, -0.25 for each 15 days by fastest warp travel) + (log10 of age)-1 + random modifier between +0 and +2 + tech level * 0.1 for each tech level above 12
 A planet with res_dep modifier of 5.19, hab mod of +2, 60 days distance from nearest 9+ planet (+0), 100 years old, +0.75 random modifier and tech 16 would have a population of 10^8.34 or 218,776,162
 (notes inside a note: 4th root is just a square root done twice
 before 100 years, planets need time to have infrastructure constructed, to attract people, etc etc...)*/
 /*Resources
Resources available determine the maximum size of industry the planet can sustain without importing any materials, in current versions planets will import from at most 15 pc (1/2 sector) away, considering for travel times and availability
Iron - basic construction material, found on all iron (+2), iron-silicate and silicate (-2) planets, min 1
Aluminum - advanced construction material, found on all non-icy planets
Silicon - used in semiconductors, found on all non-icy planets (+2 silicate, +0 iron-silicate, -1 iron), min 1
Uranium - basic fission fuel, found on all non-icy planets but most abundant in young ones (+3 for age 1, +1 for age 2, +0 for 3-4, then decreasing by 1 for each 2 billion years of age, min 1
Thorium - stable fission fuel, found on all non-icy planets but most abundant in young ones (+3 for age 1-2, +2 for age 3-5, +1 for age 6-9, +0 for age 10-12), min 1
Plutonium - powerful fission fuel, found on very young non-icy planets
Lithium - used in ionic form for batteries, found on all non-icy planets, mostly hot ones
Helium-3 - non-radioactive fusion fuel, found mostly in gas giants (+8), also on moons (+2)
Iridium - advanced material for fusion reactors and plasma weapons, rare and found mostly on cold (t < 8) and young (age < 4) non-icy planets (+4, -1 for each billion years of age, up to +0 for 5 billion years)
you'll notice a pattern... you won't find any of these on icy planets!*/
/*Each sector will have certain factions controlling the area, randomly selected from a list of 16 factions that don't necessarily border in lore*/
namespace woteg
{
    class StarSystem
    {
        string name;
        int starclass; //0 - M, 1 - K, 2 - G, 3 - F, 4 - A, 5 - B, 6 - O
        int number; // number of stars, later each distant multiple will have their own solar system
        int startype; // if !O class then 0, if =O class then 1 - normal O-class, 2 - lum variable, 3 - Wolf-Rayet star, 4 - helium-carbon star, 5 - pulsating star, 6 - carbon star
        int age; // in billions of years
        int diameter; // result of star class and age
        int heatmodifier; // result of star class and type
        int mindia; // minimum diameter
        int maxdis; // maximum distance
        public void setName (string nm) { name = nm; }
        public void setStarClass(int s) { starclass = s; }
        public void setNumber(int n) { number = n; }
        public void setStarType(int st) { startype = st; }
        public void setAge(int ag) { age = ag; }
        public void setDiameter(int dia) { diameter = dia; }
        public void setHeatModifier(int hm) { heatmodifier = hm; }
        public void setMinDia(int md) { mindia = md; }
        public void setMaxDis(int max) { maxdis = max; }
        public string getName() { return name; }
        public int getStarClass() { return starclass; }
        public int getNumber() { return number; }
        public int getStarType() { return startype; }
        public int getAge() { return age; }
        public int getDiameter() { return diameter; }
        public int getHeatModifier() { return heatmodifier; }
        public int getMinDia() { return mindia; }
        public int getMaxDis() { return maxdis; }
    }
    class Planet
    {
        string name;
        int seed; // planet seed
        int diameter; // diameter
        int planettype; // planet type (0 - iron-silicate, 1 - iron, 2 - silicate, 3 - icy, 4 - gas giant)
        int mass; // mass
        int temperature; // temperature
        int gravity;
        int atmosphere; // atmosphere level
        int atmotype; // atmosphere type (0 - none, 1 - breathable, 2 - toxic)
        int life; // life level (0-20)
        int moons; // number of moons (0-6)
        long population; // can handle populations of up to 9.2 quintillion
        int[] resources = new int[20];
    }
    class Program
    {
        static void Main(string[] args)
        {
            int tmod = 0;
            int stargen = 0;
            int starclass = 0;
            int planetnum = 0;
            char input = ' ';
            Random rand = new Random();
            StarSystem star = new StarSystem();
            StarSystem[] starsystem = new StarSystem[16384];
            Planet[,] planets = new Planet[16384,20]; // 16384 star systems, 20 planets
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine("WAR OF THE EIGHT GALAXIES");
            Console.WriteLine("STAR SYSTEM GENERATOR");
            Console.WriteLine("Note: this is a test! Generates one star system only!!!");
            Console.WriteLine("In the final WotEG, these systems will be generated procedurally in chunks, to prevent system overload.");
            Console.WriteLine("The whole galaxy would be over 100 gigabytes in size if it had to be rendered in full!");
            tmod = rand.Next(256);
            Console.WriteLine("{0}", tmod);
            Console.WriteLine("Type 1 to begin generation.");
            char.TryParse(Console.ReadLine(), out input);
            switch (input)
            {
                case '1':
                    starGenerator();
                    break;
                default:
                    Console.WriteLine("Invalid input, try again");
                    break;
            }
            //Console.WriteLine("Here's a random sector while I make the generator...");
            //for (int i = 0; i < 30; i++) // used to be 100, I'm trying to reduce processing power needed!
            //{
            //    for (int j = 0; j < 30; j++)
            //    {
            //        stargen = rand.Next(1, 101);
            //        if (stargen <= 15)
            //        {
            //            starclass = rand.Next(1, 101);
            //            if (starclass == 1) Console.ForegroundColor = ConsoleColor.Magenta;
            //            else if (starclass >= 2 && starclass <= 3) Console.ForegroundColor = ConsoleColor.Blue;
            //            else if (starclass >= 4 && starclass <= 6) Console.ForegroundColor = ConsoleColor.White;
            //            else if (starclass >= 7 && starclass <= 10) Console.ForegroundColor = ConsoleColor.Yellow;
            //            else if (starclass >= 11 && starclass <= 20) Console.ForegroundColor = ConsoleColor.DarkYellow;
            //            else if (starclass >= 21 && starclass <= 40) Console.ForegroundColor = ConsoleColor.Red;
            //            else Console.ForegroundColor = ConsoleColor.DarkRed;
            //            Console.Write("●");
            //        }
            //        else Console.Write(" ");
            //    }
            //    Console.Write("\n");
            //}
        }
        public static void starGenerator() // it begins...
        {

        }
    }
}
