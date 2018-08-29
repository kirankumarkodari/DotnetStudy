using System;


namespace First_console_app
{
    class Program
    {
        static void Main(string[] args)
        {
            string name,name2; //Variable for storing string value

            //Displaying message for entring value
            Console.WriteLine("Please Enter Your Good Name");

            //Accepting and holding values in name variable
            name = Console.ReadLine();
            name2= Console.ReadLine();

            //Displaying Output
            Console.WriteLine("Welcome {1} in your first csharp program", name, name2);// {1} means which argument it need to print 0-> first 1-> second 

            //Holding console screen
            Console.ReadLine();
        }
    }
}
