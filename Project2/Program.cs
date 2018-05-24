using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.Write("Please enter a path to the source file: ");
            string userInput = Console.ReadLine();
            if(userInput.Length < 7)
            {
                Console.WriteLine("File name isnt big enough to possibly be a .TM file made from a .txt file");
                Console.WriteLine("Press any key to exit program and try again.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            if (userInput[userInput.Length - 7] != '.' && userInput[userInput.Length - 6] != 'T' && userInput[userInput.Length - 5] != 'M')
            {
                Console.WriteLine("File was not a .TM file");
                Console.WriteLine("Press any key to exit program and try again.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            TuringMachine testMachine;
            try { testMachine = new TuringMachine(userInput); }
            catch(FormatException x)
            {
                Console.WriteLine(x.Message);
                Console.WriteLine("Press any key to exit program and try again.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("File could not be accessed due to permission level.");
                Console.WriteLine("Press any key to exit program and try again.");
                Console.ReadKey();
                Environment.Exit(0);
                
            }
            catch(System.IO.FileNotFoundException)
            {
                Console.WriteLine("File does not exist");
                Console.WriteLine("Press any key to exit program and try again.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            testMachine = new TuringMachine(userInput);
            try { testMachine.run(testMachine); }
            catch(FormatException x)
            {
                Console.WriteLine(x.Message);
                Console.WriteLine("Press any key to exit program and try again.");
                Console.ReadKey();
                Environment.Exit(0);

            }






            Console.ReadKey();
        }
    }
}
