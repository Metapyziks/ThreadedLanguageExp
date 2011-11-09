namespace ThreadedLanguage.Commands
{
    [CommandIdentifier( "sub" )]
    internal class CmdDefSub : CommandType
    {
        public CmdDefSub()
            : base( ParameterType.Identifier, true )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            scope.Declare( command.Identifier,
                new TLSub( command.InnerBlock, scope ) );

            thread.Advance();
        }
    }
}
