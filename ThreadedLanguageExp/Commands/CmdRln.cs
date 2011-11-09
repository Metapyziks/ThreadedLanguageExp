using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadedLanguage.Commands
{
    [CommandIdentifier( "rln" )]
    internal class CmdRln : CommandType
    {
        public CmdRln()
            : base( ParameterType.Identifier )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            String input = Console.ReadLine();

            if ( command.Identifier != "" )
                scope[ command.Identifier ] = TLObject.Convert( scope[ command.Identifier ].GetType(), new TLStr( input ) );

            thread.Advance();
        }
    }

    [CommandIdentifier( "rky" )]
    internal class CmdRky : CommandType
    {
        public CmdRky()
            : base( ParameterType.Identifier )
        {

        }

        public override void Execute( Command command, Thread thread, Scope scope )
        {
            char input = Console.ReadKey().KeyChar;

            if ( command.Identifier != "" )
                scope[ command.Identifier ] = TLObject.Convert( scope[ command.Identifier ].GetType(), new TLByt( (byte) input ) );

            thread.Advance();
        }
    }
}
