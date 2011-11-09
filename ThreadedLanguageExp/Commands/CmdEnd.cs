
namespace ThreadedLanguage.Commands
{
    [CommandIdentifier( "end" )]
    internal class CmdEnd : CommandType
    {
        public CmdEnd()
            : base( ParameterType.None, false, true )
        {

        }
    }
}
