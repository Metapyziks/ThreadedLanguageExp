namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "str" )]
    internal class CmdDefStr : CommandType
    {
        public CmdDefStr()
            : base( ParameterType.Identifier | ParameterType.Expression )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            if ( command.ParamExpression != null )
            {
                scope.Declare( command.Identifier,
                    new TLStr( command.ParamExpression.Evaluate( scope ) ) );
            }
            else
                scope.Declare( command.Identifier, new TLStr() );

            thread.Advance();
        }
    }
}
