using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "set" )]
    internal class CmdSet : CommandType
    {
        public CmdSet()
            : base( ParameterType.Identifier | ParameterType.Expression )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            scope[ command.Identifier ] =
                TLObject.Convert( scope[ command.Identifier ].GetType(),
                command.ParamExpression.Evaluate( scope ) );

            thread.Advance();
        }
    }
}