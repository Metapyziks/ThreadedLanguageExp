
namespace ThreadedLanguage.Commands
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
            if ( new TLBit( command.ParamExpression.Evaluate( scope ) ).Value )
                thread.EnterBlock( command.InnerBlock );
            else
                thread.Advance();
        }

        public override void ExitInnerBlock( Command command, Thread thread, Scope scope )
        {
            do
                thread.Advance();
            while ( thread.CurrentCommand != null &&
                thread.CurrentCommand.CommandType is CmdEls ||
                thread.CurrentCommand.CommandType is CmdEif );
        }
    }
}
