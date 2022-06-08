using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace woteg
{
    class mapgenerator
    {
        public static void mapgen(int[,] points, int[,] land, int[,] world, int[] population, int cities)
        {
            int provinces = 0;
            int seed = 0;
            int lastchanged = 0;
            int TopCont = 0;
            int BotCont = 0;
            int LefCont = 0;
            int RigCont = 0;
            int Conflicts = 0;
            int Solve = 0;
            int[,] newareas = new int[100, 100];
            bool IsConflicted = false;
            //default world empty area = 88;
            //doing some clearing for that 0 province
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    points[i, j] = 88;
                }
            }
            //map settings
            Console.Clear();
            Console.WriteLine("Number of provinces:");
            provinces = int.Parse(Console.ReadLine());
            if (provinces < 4 || provinces > 12)
            {
                Console.WriteLine("Invalid number of provinces, allowed number: 4-12");
            }
            else
            {
                Console.WriteLine("Number of provinces: {0}", provinces);
                Console.WriteLine("Set seed:");
                seed = int.Parse(Console.ReadLine());
                Random rand = new Random(seed);
                points[9, 9] = 0;
                points[4, 49] = 0;
                points[9, 89] = 1;
                points[49, 94] = 1;
                points[49, 4] = 2;
                points[89, 9] = 2;
                points[94, 49] = 3;
                points[89, 89] = 3;
                for (int i = 0; i < provinces; i++)
                {
                    points[rand.Next(18, 80), rand.Next(18, 80)] = i + 4;
                }
                // show points
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        //Console.Write("{0:D2}|", points[i,j]);
                        //if (points[i, j] != 88) Console.Write("█");
                        //else Console.Write("▒");
                        world[i, j] = points[i, j]; // initial world points
                        if (points[i, j] != 88) newareas[i, j] = 1;
                    }
                    Console.Write("\n");
                }
                // province generation, repeat expansion 500! times to make sure
                // medium/sea - ▒ 
                // fullblock/land - █
                for (int times = 0; times < 500; times++)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            //if (world[i, j] != 88 && world[i,j] != lastchanged)
                            if (world[i, j] != 88 && newareas[i, j] == 1)
                            {
                                //if (i - 1 >= 0 && world[i - 1, j] == 88)
                                //----------------------------------------------------------------- do the left!
                                if (i - 1 >= 0)
                                {
                                    TopCont = 88;
                                    BotCont = 88;
                                    LefCont = 88;
                                    RigCont = 88;
                                    Conflicts = 0;
                                    IsConflicted = false;
                                    if (j - 1 >= 0) TopCont = world[i - 1, j - 1];
                                    if (j + 1 <= 99) BotCont = world[i - 1, j + 1];
                                    if (i - 2 >= 0) LefCont = world[i - 2, j];
                                    RigCont = world[i, j];
                                    if (TopCont != world[i, j] && TopCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (BotCont != world[i, j] && BotCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (LefCont != world[i, j] && LefCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (IsConflicted == false)
                                    {
                                        world[i - 1, j] = world[i, j];
                                    }
                                    else
                                    {
                                        //Solve = (i + j) % 4;
                                        Solve = rand.Next(0, 4);
                                        switch (Solve)
                                        {
                                            case 0:
                                                if (RigCont != 88) world[i - 1, j] = RigCont;
                                                //else world[i-1, j] = RigCont;
                                                break;
                                            case 1:
                                                if (BotCont != 88) world[i - 1, j] = BotCont;
                                                else world[i - 1, j] = RigCont;
                                                break;
                                            case 2:
                                                if (LefCont != 88) world[i - 1, j] = LefCont;
                                                else world[i - 1, j] = RigCont;
                                                break;
                                            case 3:
                                                if (TopCont != 88) world[i - 1, j] = TopCont;
                                                else world[i - 1, j] = RigCont;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    //newareas[i - 1, j] = 1; // sets as newly changed area
                                }
                                //----------------------------------------------------------------- now the top!
                                if (j - 1 >= 0)
                                {
                                    TopCont = 88;
                                    BotCont = 88;
                                    LefCont = 88;
                                    RigCont = 88;
                                    Conflicts = 0;
                                    IsConflicted = false;
                                    if (j - 2 >= 0) TopCont = world[i, j - 2];
                                    BotCont = world[i, j];
                                    if (i - 1 >= 0) LefCont = world[i - 1, j - 1];
                                    if (i + 1 <= 99) RigCont = world[i + 1, j - 1];
                                    if (TopCont != world[i, j] && TopCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (LefCont != world[i, j] && LefCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (RigCont != world[i, j] && RigCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (IsConflicted == false)
                                    {
                                        world[i, j - 1] = world[i, j];
                                    }
                                    else
                                    {
                                        //Solve = (i + j) % 4;
                                        Solve = rand.Next(0, 4);
                                        switch (Solve)
                                        {
                                            case 0:
                                                if (TopCont != 88) world[i, j - 1] = TopCont;
                                                else world[i, j - 1] = BotCont;
                                                break;
                                            case 1:
                                                if (RigCont != 88) world[i, j - 1] = RigCont;
                                                else world[i, j - 1] = BotCont;
                                                break;
                                            case 2:
                                                if (BotCont != 88) world[i, j - 1] = BotCont;
                                                //else world[i, j + 1] = BotCont;
                                                break;
                                            case 3:
                                                if (LefCont != 88) world[i, j - 1] = LefCont;
                                                else world[i, j - 1] = BotCont;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    //newareas[i, j-1] = 1; // sets as newly changed area
                                }
                                //----------------------------------------------------------------- right!
                                if (i + 1 <= 99)
                                {
                                    TopCont = 88;
                                    BotCont = 88;
                                    LefCont = 88;
                                    RigCont = 88;
                                    Conflicts = 0;
                                    IsConflicted = false;
                                    if (j - 1 >= 0) TopCont = world[i + 1, j - 1];
                                    if (j + 1 <= 99) BotCont = world[i + 1, j + 1];
                                    LefCont = world[i, j];
                                    if (i + 2 <= 99) RigCont = world[i + 2, j];
                                    if (TopCont != world[i, j] && TopCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (BotCont != world[i, j] && BotCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (RigCont != world[i, j] && RigCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (IsConflicted == false)
                                    {
                                        world[i + 1, j] = world[i, j];
                                    }
                                    else
                                    {
                                        //Solve = (i + j) % 4;
                                        Solve = rand.Next(0, 4);
                                        switch (Solve)
                                        {
                                            case 0:
                                                if (LefCont != 88) world[i + 1, j] = LefCont;
                                                //else world[i, j + 1] = LefCont;
                                                break;
                                            case 1:
                                                if (TopCont != 88) world[i + 1, j] = TopCont;
                                                else world[i + 1, j] = LefCont;
                                                break;
                                            case 2:
                                                if (RigCont != 88) world[i + 1, j] = RigCont;
                                                else world[i + 1, j] = LefCont;
                                                break;
                                            case 3:
                                                if (BotCont != 88) world[i + 1, j] = BotCont;
                                                else world[i + 1, j] = LefCont;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    //newareas[i+1, j] = 1; // sets as newly changed area
                                }
                                //----------------------------------------------------------------- finally, BOTTOM!
                                if (j + 1 <= 99)
                                {
                                    TopCont = 88;
                                    BotCont = 88;
                                    LefCont = 88;
                                    RigCont = 88;
                                    Conflicts = 0;
                                    IsConflicted = false;
                                    TopCont = world[i, j];
                                    if (j + 2 <= 99) BotCont = world[i, j + 2];
                                    if (i - 1 >= 0) LefCont = world[i - 1, j + 1];
                                    if (i + 1 <= 99) RigCont = world[i + 1, j + 1];
                                    if (BotCont != world[i, j] && BotCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (LefCont != world[i, j] && LefCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (RigCont != world[i, j] && RigCont != 88)
                                    {
                                        IsConflicted = true;
                                        Conflicts++;
                                    }
                                    if (IsConflicted == false)
                                    {
                                        world[i, j + 1] = world[i, j];
                                    }
                                    else
                                    {
                                        //Solve = (i + j) % 4;
                                        Solve = rand.Next(0, 4);
                                        switch (Solve)
                                        {
                                            case 0:
                                                if (BotCont != 88) world[i, j + 1] = BotCont;
                                                else world[i, j + 1] = TopCont;
                                                break;
                                            case 1:
                                                if (LefCont != 88) world[i, j + 1] = LefCont;
                                                else world[i, j + 1] = TopCont;
                                                break;
                                            case 2:
                                                if (TopCont != 88) world[i, j + 1] = TopCont;
                                                //else world[i, j + 1] = TopCont;
                                                break;
                                            case 3:
                                                if (RigCont != 88) world[i, j + 1] = RigCont;
                                                else world[i, j + 1] = TopCont;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    //newareas[i, j+1] = 1; // sets as newly changed area
                                }
                                //if (j - 1 >= 0 && world[i, j-1] == 88) world[i, j-1] = world[i, j];
                                //if (i + 1 <= 99 && world[i+1, j] == 88) world[i+1, j] = world[i, j];
                                //if (j + 1 <= 99 && world[i, j + 1] == 88) world[i, j+1] = world[i, j];
                                newareas[i, j] = 2; // sets as old area, will not be changed anymore
                                //i++;
                                //lastchanged = world[i, j];
                            }
                        }
                    }
                    //for (int i = 0; i < 100; i++)
                    //{
                    //    for (int j = 0; j < 100; j++) // show off that beautiful world
                    //    {
                    //        //Console.Write("{0:D2}|", points[i,j]);
                    //        if (world[i, j] != 88) Console.Write("█");
                    //        else Console.Write("▒");
                    //    }
                    //    Console.Write("\n");
                    //}
                    // here, the new land being expanded!!!
                    //for (int i = 0; i < 100; i++)
                    //{
                    //    for (int j = 0; j < 100; j++) // show off that beautiful world
                    //    {
                    //        //Console.Write("{0:D2}|", points[i,j]);
                    //        if (world[i, j] == 88 || world[i, j] > 3) Console.Write("█");
                    //        else Console.Write("▒");
                    //    }
                    //    Console.Write("\n");
                    //}
                    //Thread.Sleep(1000);
                    for (int i = 0; i < 100; i++)
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            if (newareas[i, j] == 2)
                            {
                                if (i - 1 >= 0 && newareas[i - 1, j] < 1) newareas[i - 1, j] = 1;
                                if (i + 1 <= 99 && newareas[i + 1, j] < 1) newareas[i + 1, j] = 1;
                                if (j - 1 >= 0 && newareas[i, j - 1] < 1) newareas[i, j - 1] = 1;
                                if (j + 1 <= 99 && newareas[i, j + 1] < 1) newareas[i, j + 1] = 1;
                            }
                        }
                    }
                    //for (int i = 0; i < 100; i++)
                    //{
                    //    for (int j = 0; j < 100; j++) // show off that beautiful world
                    //    {
                    //        //Console.Write("{0:D2}|", points[i,j]);
                    //        if (newareas[i,j] == 1) Console.Write("█");
                    //        else Console.Write("▒");
                    //    }
                    //    Console.Write("\n");
                    //}
                    //for (int i = 0; i < 100; i++)
                    //{
                    //    for (int j = 0; j < 100; j++) // show off that beautiful world
                    //    {
                    //        //Console.Write("{0:D2}|", points[i,j]);
                    //        if (world[i, j] > 3) Console.Write("█");
                    //        else Console.Write("▒");
                    //    }
                    //    Console.Write("\n");
                    //}
                    //Console.Clear();
                }
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++) // show off that beautiful world
                    {
                        //Console.Write("{0:D2}|", points[i,j]);
                        if (world[i, j] != 88 && world[i, j] > 3) Console.Write("█");
                        else Console.Write("▒");
                    }
                    Console.Write("\n");
                }
                //Thread.Sleep(2000);
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++) // now in different colors
                    {
                        //Console.Write("{0:D2}|", points[i,j]);
                        if (world[i, j] != 88)
                        {
                            switch (world[i, j])
                            {
                                case 4:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case 5:
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    break;
                                case 6:
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    break;
                                case 7:
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    break;
                                case 8:
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    break;
                                case 9:
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    break;
                                case 10:
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    break;
                                case 11:
                                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                    break;
                                case 12:
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    break;
                                case 13:
                                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                                    break;
                                case 14:
                                    Console.ForegroundColor = ConsoleColor.White;
                                    break;
                                case 15:
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    break;
                                default:
                                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                                    break;
                            }
                            Console.Write("█");
                        }
                        else
                        {
                            Console.Write("▒");
                        }
                    }
                    Console.Write("\n");
                }
            }
            // landarea calculation and density calculation
            int[] area = new int[12];
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (world[i, j] > 3)
                    {
                        area[world[i, j] - 4]++;
                    }
                }
            }
            for (int i = 0; i < provinces; i++)
            {
                switch (i + 4)
                {
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case 5:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    case 6:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case 7:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case 8:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case 9:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case 10:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case 11:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        break;
                    case 12:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case 13:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case 14:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case 15:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        break;
                }
                Console.WriteLine("Area of province {0}: {1} km^2", i + 1, area[i] * 25);
            }
            // base density: 300/km^2 at 7,000 km^2, squareroot density factor (basearea/area) if above base area, square it if below
            int basedensity = 300;
            double basearea = 7000;
            double currdensity = 0;
            int totpopulation = 0;
            for (int i = 0; i < provinces; i++)
            {
                if (area[i] * 25 >= basearea)
                {
                    currdensity = Math.Sqrt(Math.Pow((basearea / (area[i] * 25) / 1.0), 3)) * basedensity;
                }
                else currdensity = Math.Pow((basearea * 1.0) / (area[i] * 25) / 1.0, 2) * basedensity;
                population[i] = Convert.ToInt32(area[i] * 25 * currdensity);
                switch (i + 4)
                {
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case 5:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    case 6:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case 7:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case 8:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case 9:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case 10:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case 11:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        break;
                    case 12:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case 13:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case 14:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case 15:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        break;
                }
                Console.WriteLine("Population of province {0}: {1}", i + 1, Convert.ToInt32(area[i] * 25 * currdensity));
                Console.WriteLine("Pop. density of province {0}: {1:F2}", i + 1, currdensity);
                totpopulation += Convert.ToInt32(area[i] * 25 * currdensity);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Country population: {0}", totpopulation);
            // todo: choose which province is the capital, with a 4:2 weight for largest population being capital
            // seed: 12794
            // city generation
            for (int i = 0; i < provinces; i++)
            {

            }
            // population generation
            //if (world[0,0] > 3)
            //{
            //
            //}
        }
    }
}
