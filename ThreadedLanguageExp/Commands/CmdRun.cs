using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "run" )]
    internal class CmdRun : CommandType
    {
        public CmdRun()
            : base( ParameterType.Identifier )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            command.InnerBlock = new Block( Command.Parse( File.ReadAllText( command.Identifier + ".tl" ) ) );
            thread.EnterBlock( command.InnerBlock );
        }

        public override void ExitInnerBlock( Command command, Thread thread, Scope scope )
        {
            thread.Advance();
        }
    }
}
