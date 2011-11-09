namespace ThreadedLanguage.Commands
{
    [CommandIdentifier( "bit" )]
    internal class CmdDefBit : CommandType
    {
        public CmdDefBit()
            : base( ParameterType.Identifier | ParameterType.Expression )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            if ( command.ParamExpression != null )
            {
                scope.Declare( command.Identifier,
                    new TLBit( command.ParamExpression.Evaluate( scope ) ) );
            }
            else
                scope.Declare( command.Identifier, new TLBit( false ) );

            thread.Advance();
        }
    }
}
