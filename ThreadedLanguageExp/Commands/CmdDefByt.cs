namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "byt" )]
    internal class CmdDefByt : CommandType
    {
        public CmdDefByt()
            : base( ParameterType.Identifier | ParameterType.Expression )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            if ( command.ParamExpression != null )
            {
                scope.Declare( command.Identifier,
                    new TLByt( command.ParamExpression.Evaluate( scope ) ) );
            }
            else
                scope.Declare( command.Identifier, new TLByt( 0 ) );

            thread.Advance();
        }
    }
}
