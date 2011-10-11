using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp
{
    public static class ThreadLang
    {
        public static void Run( String script )
        {
            Thread thread;
#if DEBUG
#else
            try
            {
#endif
                thread = new Thread( new Block( Command.Parse( script ) ), null, true );
#if DEBUG
#else
            }
            catch ( Exception e )
            {
                throw new Exception( "An error occurred while parsing a script.", e );
            }

            try
            {
#endif
                while ( !thread.Exited )
                    thread.Step();
#if DEBUG
#else
            }
            catch ( Exception e )
            {
                throw new Exception( "An error occurred while running a script.\n  at line: " + thread.CurrentCommand.LineNumber, e );
            }
#endif
        }
    }
}
