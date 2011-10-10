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
            scope.Declare( command.Identifier,
                new TLByt( command.ParamExpression.Evaluate( scope ) ) );

            thread.Advance();
        }
    }
}
