using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "grp" )]
    internal class CmdGrp : CommandType
    {
        public CmdGrp()
            : base( ParameterType.None, true )
        {
            
        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            thread.BeginGroup();
            thread.EnterBlock( command.InnerBlock );
        }

        public override void ExitInnerBlock( Command command, Thread thread, Scope scope )
        {
            thread.EndGroup();
            thread.Advance();
        }
    }
}
