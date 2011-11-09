
namespace ThreadedLanguage.Commands
{
    [CommandIdentifier( "els" )]
    internal class CmdEls : CommandType
    {
        public CmdEls()
            : base( ParameterType.None, true, true )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            thread.EnterBlock( command.InnerBlock );
        }

        public override void ExitInnerBlock( Command command, Thread thread, Scope scope )
        {
            thread.Advance();
        }
    }
}