
namespace ThreadedLanguage.Commands
{
    [CommandIdentifier( "inp" )]
    internal class CmdInp : CommandType
    {
        public CmdInp()
            : base( ParameterType.Identifier | ParameterType.Expression )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            TLStr stream = ( command.ParamExpression.Evaluate( scope ) as TLStr );

            if ( stream.DataWaiting )
            {
                scope[ command.Identifier ] =
                    ( command.ParamExpression.Evaluate( scope ) as TLStr ).Read();

                thread.Advance();
            }
        }
    }
}
