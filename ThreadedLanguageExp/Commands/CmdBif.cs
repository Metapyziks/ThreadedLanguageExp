using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "bif" )]
    internal class CmdBif : CommandType
    {
        public CmdBif()
            : base( ParameterType.Expression, true, false )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            if( new TLBit( command.ParamExpression.Evaluate( scope ) ).Value )
                thread.EnterBlock( command.InnerBlock );
        }

        public override void ExitInnerBlock( Command command, Thread thread, Scope scope )
        {
            thread.Advance();
        }
    }
}
