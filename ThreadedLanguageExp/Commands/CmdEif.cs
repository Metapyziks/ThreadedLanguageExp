
namespace ThreadedLanguage.Commands
{
    [CommandIdentifier( "eif" )]
    internal class CmdEif : CommandType
    {
        public CmdEif()
            : base( ParameterType.Expression, true, true )
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
            while ( thread.CurrentCommand != null && (
                thread.CurrentCommand.CommandType is CmdEls ||
                thread.CurrentCommand.CommandType is CmdEif ) );
        }
    }
}
