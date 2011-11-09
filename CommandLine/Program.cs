using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ThreadedLanguage;
using System.IO;

namespace CommandLine
{
    class Program
    {
        static void Main( string[] args )
        {
            ThreadLang.Print += delegate( object sender, PrintEventArgs e )
            {
                Console.Write( e.Message );
            };

            if ( args.Length != 0 )
            {
                ThreadLang.Run( File.ReadAllText( args[ 0 ] ) );
                return;
            }

            Console.Write( "> " );

            List<String> prevLines = new List<string>();
            String lastLine = "";

            while ( true )
            {
                ConsoleKeyInfo key = Console.ReadKey();

                switch ( key.Key )
                {
                    case ConsoleKey.Enter:
                        if ( ( key.Modifiers & ConsoleModifiers.Control ) != 0 )
                        {
                            String code = String.Join( "\n", prevLines ) + "\n " + lastLine;
#if DEBUG
#else
                            try
                            {
#endif
                                ThreadLang.Run( code );
#if DEBUG
#else
                            }
                            catch ( Exception e )
                            {
                                Console.WriteLine( e.Message );

                                if ( e.InnerException != null )
                                    Console.WriteLine( e.InnerException.Message );
                            }
#endif
                            lastLine = "";
                            prevLines.Clear();
                            Console.WriteLine();
                            Console.Write( "> " );
                        }
                        else
                        {
                            prevLines.Add( lastLine );
                            lastLine = "";
                            Console.WriteLine();
                            Console.Write( "| " );
                        }
                        break;
                    case ConsoleKey.Backspace:
                        if ( lastLine.Length > 0 )
                        {
                            lastLine = lastLine.Substring( 0, lastLine.Length - 1 );
                            Console.Write( " " );
                            --Console.CursorLeft;
                        }
                        else
                            ++Console.CursorLeft;
                        break;
                    default:
                        lastLine += key.KeyChar;
                        break;
                }
            }
        }
    }
}
