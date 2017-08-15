using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

//Testkommentar

namespace DelaInIGrupper
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            //Add comment here
            log.Info("Program start");
            // Skrivs ut när programmet startas:
            PrintWhite("*** Create groups ***".ToUpper());
            Console.WriteLine();

            //resultList ska användas till att spara resultet.
            //den används om användaren vill spara resultatet till fil.
            List<string> printList = new List<string>();
            List<string>[] resultList;
            //names innehåller alla namn. Vi anropar SelectInput()
            //för att få dessa från användaren eller från fil.
            string[] names = SelectInput();
            
            int numberOfGroups = AskUserForGroups();
            
            ShuffleArray(names);
            
            resultList = DisplayResult(names, numberOfGroups, printList);
            AskUserWriteFile(printList);
            

            ReshuffleGroups(names, numberOfGroups, printList, resultList);
        }

        /*
         * AskUserWriteFile
         * 
         * Metoden frågar användaren om denna vill spara resultatet på fil
         * Om ja, så sparas resultatet i "result.txt".
         * 
         * Metoden tar en List<string> som innehåller resultatet som 
         * inparameter
         */
        private static void AskUserWriteFile(List<string> result)
        {
            Console.WriteLine("Save result to file? (y/n): ");
            string answer = Console.ReadLine();
            if (answer.ToLower() == "y")
            {
                File.WriteAllLines("result.txt", result);
                Console.WriteLine("Resultatet sparat till result.txt");
                log.Info("Saved to file");
            }
            Console.WriteLine();
        }
        /*
         * SelectInput
         * 
         * Metoden frågar användaren om denna vill skriva in namn eller
         * läsa in namnen från fil. Filen som används är "names.txt" och 
         * innehåller namnen på alla studenter i Hogia Academy.
         */
        private static string[] SelectInput()
        {
            Console.WriteLine("Load data from file?? (y/n): ");
            string[] names;
            string answer = Console.ReadLine();
            if (answer == "y")
            {
                names = ReadNamesFromFile();

            }
            else
            {
                names = AskUserForNames();
            }
            return names;
        }
        /*
         * ReadNamesFromFile
         * 
         * Metoden läser in alla namn från filen "names.txt".
         * 
         * Retunerar en string[] med namnen.
         */
        private static string[] ReadNamesFromFile()
        {
            string[] names = File.ReadAllLines("names.txt");
            log.Info("Read from file");
            return names;
        }

        /*
         * DisplayResult
         * 
         * Metoden skriver ut resultatet på konsollen och sparar även
         * resultatet i en List<string> för ev kunna spara på fil.
         * 
         * Metoden tar en string[] med namn, en int med antal grupper
         * samt en List<string> för att spara resultatet i som 
         * inparametrar.
         */
        private static List<string>[] DisplayResult(string[] names, int numberOfGroups, List<string> result)
        {
            Random rnd = new Random();
            
            List<string>[] groups = AssignAndSortGroups(names, numberOfGroups);

            Console.WriteLine();
            for (int i = 0; i < groups.Length; i++)
            {
                int groupLeader = rnd.Next(0,groups[i].Count);
                result.Add($"Group {(i + 1)}");
                Thread.Sleep(1000);
                PrintGreen($"Group {(i + 1)}");
                Speak($"Group {(i + 1)}");

                for (int j = 0; j < groups[i].Count; j++)
                {
                    if(j == groupLeader)
                    {
                        result.Add("Groupleader");
                        PrintWhite("Groupleader");
                        PrintWhite(groups[i][j]);
                        Speak($"Groupleader {groups[i][j]}");
                        continue;
                    }
                    result.Add(groups[i][j]);
                    Thread.Sleep(500);
                    Console.WriteLine(groups[i][j]);

                }
                Console.WriteLine();
                result.Add(Environment.NewLine);
            }
            return groups;
        }

        /*
         * AssignAndSortGroups
         * 
         * Metoden delar in personer i grupper och sorterar i 
         * bokstavordning.
         * 
         * Metoden tar en string[] med namn och en int med antal grupper
         * som inparametrar.
         * 
         * Metoden retunerar en List<string>[] med resultatet
         */
        private static List<string>[] AssignAndSortGroups(string[] names, int numberOfGroups)
        {
            List<string>[] groups = CreateArray(numberOfGroups);

            for (int i = 0; i < names.Length; i++)
            {
                int temp = i % numberOfGroups;
                groups[temp].Add(names[i]);
            }

            foreach (var list in groups)
            {
                list.Sort();
            }

            return groups;
        }

        /*
         * PrintGreen
         * 
         * Metoden skriver ut en text på konsollen i grön färg
         * 
         * Metoden tar en string som inparameter.
         */
        private static void PrintGreen(string v)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(v);
            Console.ResetColor();
        }
        /*
         * CreateArray
         * 
         * Metoden skapar en array som innehåller alla grupplistor.
         * 
         * Metoden tar en int med antal grupper som inparameter
         * 
         * Metoden retunerar en List<string>[] med resultatet.
         */
        private static void Speak(string str)
        {
            // TODO: Använd en häftigare röst!!
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.Speak(str);
        }
        private static List<string>[] CreateArray(int numberOfGroups)
        {
            List<string>[] groups = new List<string>[numberOfGroups];
            for (int i = 0; i < numberOfGroups; i++)
            {
                groups[i] = new List<string>();
                
            }
            return groups;
        }
        /*
         * AskUserForGroups
         * 
         * Metoden frågar användaren efter önskat antal grupper
         * 
         * Metoden retunerar en int med resultatet
         */
        private static int AskUserForGroups()
        {
            Console.Write("How many groups? ");
            int numberOfGroups = Int32.Parse(Console.ReadLine());
            if(numberOfGroups <= 0)
            {
                numberOfGroups = 1;
            }
            return numberOfGroups;
        }
        /*
         * AskUserForNames
         * 
         * Metoden ber användaren att mata in alla namn.
         * Därefter delas inmatningssträngen och läggs i en array
         * 
         * Metoden retunerar en string[] med namnen
         */
        private static string[] AskUserForNames()
        {
            string[] names;
            bool b = false;
            do
            {
                Console.Write("Input all names [separate with ,]: ");
                names = Console.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); //TODO: Rätta bugg som gör att man inte kan skriva mellanslag
                b = ValidateArray(names);

            } while (!b);

            return names;
        }
        private static bool ValidateArray(string[] array)
        {
            bool result = true;
            if(array.Length == 0)
            {
                log.Error("List empty");
                PrintRed("List empty");
                Speak("List empty");
                result = false; //alternativt sätt: "return false"
            }
            else
            {
                foreach (string name in array)
                {
                    if (name.Length < 2 || name.Length > 20)
                    {
                        log.Error($"{ name} is incorrect, a name can only contain 2 - 20 letters");
                        PrintRed($"{name} is incorrect, a name can only contain 2 - 20 letters");
                        Speak($"{name} is incorrect, a name can only contain 2 - 20 letters");
                        result = false;
                        //break;
                    }
                    else if (!name.All(char.IsLetter))
                    {
                        log.Error("Name may contain only letters.");
                        PrintRed($"Error with {name}.");
                        PrintRed("Name may contain only letters.");
                        Speak($"Error with {name}.");
                        Speak("Name may contain only letters.");
                        result = false;
                        //break;
                    }
                }
            }
            return result;

        }
        //TODO: Mer informativa variabelnamn
        private static void PrintRed(string v)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(v);
            Console.ResetColor();
        }

        private static void PrintWhite(string v)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(v);
            Console.ResetColor();
        }

        /*
* ShuffleArray
* 
* Metoden blandar värdena i en array.
* Den bygger på Fisher-Yates shuffle algoritm 
* 
* Metoden tar en string[] som inparameter 
*/
        static void ShuffleArray(string[] array)
        {
            Random random = new Random();
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + random.Next(n - i);
                string s = array[r];
                array[r] = array[i];
                array[i] = s;
            }
        }

        static void ReshuffleGroups(string[] names, int numberOfGroups, List<string> resultList, List<string>[] groups)
        {
            Console.Write("Do you want to reshuffle the groups? (y/n): ");
            if (Console.ReadLine().ToLower() == "y")
            {
                ShuffleArray(names);

                DisplayResult(names, numberOfGroups, resultList);
                AskUserWriteFile(resultList);
                ReshuffleGroups(names, numberOfGroups, resultList, groups);
            }
            else
            {
                Console.WriteLine("Exiting program.");
                log.Info("Exiting Program");
            }
        }
    }
}
