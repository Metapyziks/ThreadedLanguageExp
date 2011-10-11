﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "inp" )]
    internal class CmdInp : CommandType
    {
        public CmdInp()
            : base( ParameterType.Identifier | ParameterType.Expression )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            scope[ command.Identifier ] =
                ( command.ParamExpression.Evaluate( scope ) as TLStr ).Read();

            thread.Advance();
        }
    }
}
