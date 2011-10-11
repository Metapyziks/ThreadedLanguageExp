﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "cal" )]
    internal class CmdCal : CommandType
    {
        public CmdCal()
            : base( ParameterType.Identifier )
        {
            
        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            TLSub sub = scope[ command.Identifier ] as TLSub;

            command.InnerBlock = sub.Block;
            thread.EnterBlock( command.InnerBlock, true, new Scope( sub.Scope ) );
        }

        public override void ExitInnerBlock( Command command, Thread thread, Scope scope )
        {
            thread.Advance();
        }
    }
}