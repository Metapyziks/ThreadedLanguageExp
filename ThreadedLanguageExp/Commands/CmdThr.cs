using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "thr" )]
    internal class CmdThr : CommandType
    {
        public CmdThr()
            : base( ParameterType.Identifier | ParameterType.IdentifierList )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            TLSub sub = scope[ command.ParamIdentifiers[ 0 ] ] as TLSub;

            Thread newThread = new Thread( sub.Block, sub.Scope );
            scope.Declare( command.Identifier, new TLThr( newThread ) );
            ThreadLang.StartThread( newThread );

            thread.Advance();
        }

        public override void ExitInnerBlock( Command command, Thread thread, Scope scope )
        {
            thread.Advance();
        }
    }
}