using System;
using System.Collections.Generic;

namespace ThreadedLanguage
{
    public class ParameterParseException : Exception
    {
        internal ParameterParseException( Command command )
            : base( "Error when parsing parameters for \"" + command.Identifier
                + "\" at line " + command.LineNumber + "." )
        {

        }

        internal ParameterParseException( Command command, String message )
            : base( "Error when parsing parameters for \"" + command.Identifier
                + "\" at line " + command.LineNumber + ":\n" + message )
        {

        }
    }

    internal class Command
    {
        public static Command[] Parse( String str )
        {
            String[] split = str.Split( '\n' );

            List<Command> commands = new List<Command>();

            for ( int i = 0; i < split.Length; ++i )
            {
                String line = split[ i ].Trim();

                if ( line.Length == 0 )
                    continue;

                String cmdIdent = line.Substring( 0, 3 );

                commands.Add( new Command( i + 1, CommandType.Get( cmdIdent ),
                    line.Substring( 3 ).Trim() ) );
            }

            return commands.ToArray();
        }

        public readonly int LineNumber;
        public readonly CommandType CommandType;

        public readonly String Identifier;
        public readonly String[] ParamIdentifiers;
        public readonly Expression ParamExpression;

        public Block InnerBlock;

        public String CommandIdentifier
        {
            get
            {
                return CommandType.Identifier;
            }
        }

        public Command( int lineNumber, CommandType commandType, String parameters )
        {
            LineNumber  = lineNumber;
            CommandType = commandType;

            int colon = parameters.IndexOf( ':' );

            if ( ( CommandType.ParamType & ParameterType.Identifier ) != 0 )
            {
                Identifier = parameters.Substring( 0,
                    colon != -1 ? colon : parameters.Length ).Trim();

                if ( colon != -1 )
                    parameters = parameters.Substring( colon + 1 );
                else
                    parameters = "";
            }
            
            if ( ( CommandType.ParamType & ParameterType.Expression ) != 0 )
                ParamExpression = Expression.Parse( parameters );
            else if ( ( CommandType.ParamType & ParameterType.IdentifierList ) != 0 )
                ParamIdentifiers = parameters.Split( new char[] { ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries );
        }

        public void Execute( Thread thread, Scope scope )
        {
            CommandType.Execute( this, thread, scope );
        }

        public void ExitInnerBlock( Thread thread, Scope scope )
        {
            CommandType.ExitInnerBlock( this, thread, scope );
        }
    }
}
