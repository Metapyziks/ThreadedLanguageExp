using System;

namespace ThreadedLanguage.Commands
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
#if DEBUG
            ThreadLang.PrintMessage( command.ParamExpression.ToString() + " = "
                + command.ParamExpression.Evaluate( scope ).ToString() );
#else
            ThreadLang.PrintMessage( command.ParamExpression.Evaluate( scope ).ToString() );
#endif

            thread.Advance();
        }
    }

    [CommandIdentifier( "pln" )]
    internal class CmdPln : CommandType
    {
        public CmdPln()
            : base( ParameterType.Expression )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            if ( command.ParamExpression != null )
            {
#if DEBUG
                ThreadLang.PrintMessage( command.ParamExpression.ToString() + " = "
                    + command.ParamExpression.Evaluate( scope ).ToString() + "\n" );
#else
                ThreadLang.PrintMessage( command.ParamExpression.Evaluate( scope ).ToString() + "\n" );
#endif
            }
            else
                ThreadLang.PrintMessage( "\n" );

            thread.Advance();
        }
    }
}
