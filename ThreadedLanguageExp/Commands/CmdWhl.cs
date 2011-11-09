
namespace ThreadedLanguage.Commands
{
    [CommandIdentifier( "whl" )]
    internal class CmdWhl : CommandType
    {
        public CmdWhl()
            : base( ParameterType.Expression, true, false )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            if ( new TLBit( command.ParamExpression.Evaluate( scope ) ).Value )
                thread.EnterBlock( command.InnerBlock );
        }

        public override void ExitInnerBlock( Command command, Thread thread, Scope scope )
        {
            if ( new TLBit( command.ParamExpression.Evaluate( scope ) ).Value )
                thread.EnterBlock( command.InnerBlock );
            else
                thread.Advance();
        }
    }
}
