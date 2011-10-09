namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "int" )]
    internal class CmdDefInt : CommandType
    {
        public CmdDefInt()
            : base( ParameterType.Identifier | ParameterType.Expression )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            scope.Declare( command.Identifier,
                new TLInt( command.ParamExpression.Evaluate( scope ) ) );
        }
    }
}
