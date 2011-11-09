namespace ThreadedLanguage.Commands
{
    [CommandIdentifier( "dec" )]
    internal class CmdDefDec : CommandType
    {
        public CmdDefDec()
            : base( ParameterType.Identifier | ParameterType.Expression )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            if ( command.ParamExpression != null )
            {
                scope.Declare( command.Identifier,
                    new TLDec( command.ParamExpression.Evaluate( scope ) ) );
            }
            else
                scope.Declare( command.Identifier, new TLDec( 0.0 ) );

            thread.Advance();
        }
    }
}
