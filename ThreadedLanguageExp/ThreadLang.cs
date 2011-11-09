using System;
using System.Collections.Generic;

namespace ThreadedLanguage
{
    public class PrintEventArgs : EventArgs
    {
        public readonly String Message;

        public PrintEventArgs( String message )
        {
            Message = message;
        }
    }

    public delegate void PrintEventHandler( object sender, PrintEventArgs e );

    public static class ThreadLang
    {
        private static List<Thread> stThreads = new List<Thread>();

        public static int MaximumThreads = 8;
        public static int MaximumIterations = 4096;

        public static event PrintEventHandler Print;

        internal static void StartThread( Thread thread )
        {
            if ( stThreads.Count >= MaximumThreads )
                throw new Exception( "Cannot create a new thread - the limit of "
                    + MaximumThreads + " threads are already active." );

            stThreads.Add( thread );
        }

        internal static void PrintMessage( String message )
        {
            if ( Print != null )
                Print( null, new PrintEventArgs( message ) );
        }

        public static void Run( String script, bool newScope = false )
        {
            Thread thread;
#if DEBUG
#else
            try
            {
#endif
                thread = new Thread( new Block( Command.Parse( script ) ), null, !newScope );
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
                stThreads.Clear();
                throw new Exception( "An error occurred while parsing a script.", e );
            }

            try
            {
#endif
                int iters = 0;

                while ( stThreads.Count > 0 && iters++ < MaximumIterations )
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

                if ( iters >= MaximumIterations )
                    throw new Exception( "The script has been running for the maximum of " + MaximumIterations + " iterations and has been stopped." );
#if DEBUG
#else
            }
            catch ( Exception e )
            {
                stThreads.Clear();
                throw new Exception( "An error occurred while running a script.\n  at line: " + thread.CurrentCommand.LineNumber, e );
            }
#endif
        }
    }
}
