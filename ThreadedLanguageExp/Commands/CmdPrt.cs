using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "prt" )]
    internal class CmdPrt : CommandType
    {
        public CmdPrt()
            : base( ParameterType.Expression )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            Console.WriteLine( command.ParamExpression.ToString() + " = "
                + command.ParamExpression.Evaluate( scope ).ToString() );
           
            thread.Advance();
        }
    }
}
