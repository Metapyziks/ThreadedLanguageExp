using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp
{
    public static class ThreadLang
    {
        private static Scope stGlobalScope;

        public static void Run( String script )
        {
            if ( stGlobalScope == null )
                stGlobalScope = new Scope();

            Thread thread = new Thread( new Block( Command.Parse( script ) ), stGlobalScope, true );

            while ( !thread.Exited )
                thread.Step();
        }
    }
}
