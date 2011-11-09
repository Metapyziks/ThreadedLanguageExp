
namespace ThreadedLanguage.Commands
{
    [CommandIdentifier( "out" )]
    internal class CmdOut : CommandType
    {
        public CmdOut()
            : base( ParameterType.Identifier | ParameterType.Expression )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            TLObject val = command.ParamExpression.Evaluate( scope );
            TLStr str = scope[ command.Identifier ] as TLStr;

            if ( val is TLByt )
                str.Write( val as TLByt );
            else
            {
                TLStr data = ( val as TLStr );
                while ( data.DataWaiting )
                    str.Write( data.Read() );
            }

            thread.Advance();
        }
    }
}
