using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp
{
    public static class ThreadLang
    {
        private static List<Thread> stThreads = new List<Thread>();

        public static int MaximumThreads = 64;

        internal static void StartThread( Thread thread )
        {
            if ( stThreads.Count >= MaximumThreads )
                throw new Exception( "Cannot create a new thread - the limit of "
                    + MaximumThreads + " threads are already active." );

            stThreads.Add( thread );
        }

        public static void Run( String script )
        {
            Thread thread;
#if DEBUG
#else
            try
            {
#endif
                thread = new Thread( new Block( Command.Parse( script ) ), null, true );
                if ( !thread.Scope.IsDeclared( "main" ) )
                    thread.Scope.Declare( "main", new TLThr( thread ) );
                else
                    thread.Scope[ "main" ] = new TLThr( thread );
                StartThread( thread );
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
                while ( stThreads.Count > 0 )
                {
                    for ( int i = 0; i < stThreads.Count; ++i )
                    {
                        thread = stThreads[ i ];

                        if ( !thread.Exited )
                            thread.Step();

                        if ( thread.Exited )
                            stThreads.RemoveAt( i-- );
                    }
                }
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
