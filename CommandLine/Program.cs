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
                Console.Write( "> " );

#if DEBUG
#else
                try
                {
#endif
                    ThreadLang.Run( Console.ReadLine() );
#if DEBUG
#else
                }
                catch ( Exception e )
                {
                    Console.WriteLine( "Error while running script:\n"
                        + e.ToString() );
                }
#endif
            }
        }
    }
}
