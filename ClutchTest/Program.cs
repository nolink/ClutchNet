using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClutchTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var hello = ClutchNet.Configuration.Get("hello", (key, value) => {

                Console.WriteLine(key + ":" + value);

            });
            Console.WriteLine(hello);

            Console.Read();

        }
    }
}
