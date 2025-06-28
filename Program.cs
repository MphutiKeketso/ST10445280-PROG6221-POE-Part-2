// See https://aka.ms/new-console-template for more information
using POE_Part_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POE_Part_2
{
    class Program
    {
        public static string name = "";

        static void Main(string[] args)
        {
            //GUI.Play();
            Console.ForegroundColor = ConsoleColor.Red;
            
            while (name == "")
            {
                Frontend.StartDisplayGUI();
                Console.WriteLine("Welcome to NEOHAC. I'm Neo, your chatbot assistant");
                Console.WriteLine("Please enter your name: ");
                name = Console.ReadLine();

                if (!string.IsNullOrEmpty(name))
                {
                    try
                    {                       
                        Frontend gui2 = new Frontend(name);
                        gui2.GUIManiuplation();
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        Console.WriteLine("Press enter to try again");
                        string input = Console.ReadLine();
                        Console.Clear();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                    }
                }
                else 
                {
                    Console.WriteLine("Name cannot be empty. Please enter a valid name.");
                    name = "";
                }

                 
            }

            Console.ReadKey();
        }
    }
}