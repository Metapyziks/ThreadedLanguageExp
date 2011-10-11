using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "syn" )]
    internal class CmdSyn : CommandType
    {
        public CmdSyn()
            : base( ParameterType.Identifier )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            Thread syncThread = ( scope[ command.Identifier ] as TLThr ).Thread;
            thread.Sync( syncThread );
        }

        public override void ExitInnerBlock( Command command, Thread thread, Scope scope )
        {
            thread.Advance();
        }
    }
}