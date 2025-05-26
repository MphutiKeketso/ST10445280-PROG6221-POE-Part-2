using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Media;


namespace POE_Part_2
{
    internal class Frontend : Backend
    {
        public String Name { get; set; }

        public Frontend(string name) : base("")
        {
            Name = name;
        }

        public static void StartDisplayGUI()
        {
            // Console.SetWindowSize(1980, WindowHeight);
            Console.SetWindowSize(180, 50);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("              NNNN             NNNNNNNN   EEEEEEEEEEEEEEEE   OOOOOOOO  HHHHH         HHHHH   AAAAAA                              CCCCCCCCCCC");
            Console.WriteLine("            NNNNNNNN         NNNNNNNN   EEEEEEEEEEEEEEEE   OOOOOOOOOOOO  HHHHH         HHHHH   AAAAAAAAAAAA                    CCCCCCC   CCCCCCC");
            Console.WriteLine("          NNNNNNNNNNNN     NNNNNNNN   EEEEEEEE           OOOOOO    OOOOOO  HHHHH         HHHHH   AAAAAA   AAAAAA                 CCCCCCC      CCCCCCC");
            Console.WriteLine("        NNNNNNN NNNNNNN NNNNNNNN   EEEEEEEEEEEEEEEE   OOOOOO    OOOOOO       HHHHHHHHHHHHHHHHHHH   AAAAAA      AAAAAA              CCCCCCC");
            Console.WriteLine("      NNNNNNN     NNNNNNNNNNNN   EEEEEEEEEEEEEEEE   OOOOOO    OOOOOO           HHHHHHHHHHHHHHHHHHH   AAAAAAAAAAAAAAAAAAAAA           CCCCCCC     ");
            Console.WriteLine("    NNNNNNN         NNNNNNNN   EEEEEEEE           OOOOOO     OOOOOO              HHHHH         HHHHH   AAAAAA            AAAAAA        CCCCCCC     CCCCCCC");
            Console.WriteLine("  NNNNNNN            NNNNN   EEEEEEEEEEEEEEEE      OOOOOOOOOOOOOO                  HHHHH         HHHHH   AAAAAA               AAAAAA     CCCCCCC   CCCCCCC");
            Console.WriteLine("NNNNNNN               NN   EEEEEEEEEEEEEEEE         OOOOOOOOOOO                      HHHHH         HHHHH   AAAAAA                  AAAAAA    CCCCCCCCCCC");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------");

        }

        public void PersonalizedGreeting()
        {
            Console.WriteLine("Hi " + Name);
            Console.WriteLine("Is there aything I can help you with today?");
        }


        public void GUIManiuplation()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Frontend gui2 = new Frontend(Name);
            StartDisplayGUI();
            gui2.PersonalizedGreeting();
            gui2.GetUserInput();

        }

        public void GetUserInput()
        {
            string userInput= "";
            bool askAgain = false;
            while (string.IsNullOrEmpty(userInput))
            {
                if (askAgain)
                {
                    Console.WriteLine("\nDo you have anymore questions I can answer?\n If so please enter your question below:");
                }
                else
                {
                    Console.WriteLine("\nPlease enter your question:");
                    askAgain = true;
                }
                
                userInput = Console.ReadLine();
                Console.WriteLine("Thinking...");
                if (!string.IsNullOrEmpty(userInput))
                {
                    Backend backend = new Backend(userInput);
                    backend.Question(); // Call the Question method and wait for it to complete
                    userInput = ""; // Reset userInput for the next iteration
                }
                else
                {
                    Console.WriteLine("No input provided. Please try again.");
                }
            }
        }

        public static void Play()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("Resources/greeting.wav");
                player.PlaySync();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("[Error: The audio file was not found. Please ensure it exists at the specified location.]");
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error playing audio: " + e.Message + "]");
            }


        }




    }
}
