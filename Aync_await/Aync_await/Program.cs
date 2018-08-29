using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aync_await
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<int> task=Method1();
            Method2();
            
            Method3(3);
            Console.ReadKey();
        }
        public static async Task<int> Method1()
        {
            int count = 0;
            await Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine(" Method 1");
                    count = count + 1;
                }
            });
            return count;
        }
        public static void Method2()
        {
            for (int i = 0; i < 25; i++)
            {
                Console.WriteLine(" Method 2");
            }
        }
        public static void Method3(int count)
        {
            
                Console.WriteLine(" Method 3 & count:"+count.ToString());
           
        }
    }
}
