﻿namespace ThreadedLanguageExp.Commands
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
            scope.Declare( command.Identifier,
                new TLDec( command.ParamExpression.Evaluate( scope ) ) );
        }
    }
}
