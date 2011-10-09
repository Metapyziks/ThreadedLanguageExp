namespace ThreadedLanguageExp.Commands
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
            scope.Declare( command.Identifier,
                new TLBit( command.ParamExpression.Evaluate( scope ) ) );
        }
    }
}
