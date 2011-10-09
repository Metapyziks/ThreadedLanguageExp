using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ThreadedLanguageExp;

namespace CommandLine
{
    class Program
    {
        static void Main( string[] args )
        {
            while ( true )
            {
                ThreadLang.Run( Console.ReadLine() );
            }
        }
    }
}
