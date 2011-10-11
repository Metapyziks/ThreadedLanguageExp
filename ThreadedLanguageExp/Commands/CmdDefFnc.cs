namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "fnc" )]
    internal class CmdDefFnc : CommandType
    {
        public CmdDefFnc()
            : base( ParameterType.Identifier, true )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            scope.Declare( command.Identifier,
                new TLFnc( command.InnerBlock, scope ) );

            thread.Advance();
        }
    }
}
