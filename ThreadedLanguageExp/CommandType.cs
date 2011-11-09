using System;
using System.Collections.Generic;
using System.Reflection;

namespace ThreadedLanguage
{
    internal class CommandIdentifierAttribute : Attribute
    {
        public readonly String Identifier;

        public CommandIdentifierAttribute( String identifier )
        {
            Identifier = identifier;
        }
    }

    public class UnknownCommandTypeException : Exception
    {
        public readonly string Identifier;

        public UnknownCommandTypeException( String identifier )
            : base( "Attempted to use an unknown command with identifier \""
                + identifier + "\"." )
        {
            Identifier = identifier;
        }
    }

    internal enum ParameterType
    {
        None            = 0,
        Identifier      = 1,
        Expression      = 2,
        IdentifierList  = 4
    }

    internal class CommandType
    {
        private static Dictionary<String, CommandType> stKnownTypes;

        private static void FindCommandTypes()
        {
            stKnownTypes = new Dictionary<string, CommandType>();

            Assembly asm = Assembly.GetExecutingAssembly();
            Type[] types = asm.GetTypes();

            foreach ( Type t in types )
            {
                if ( t.BaseType == typeof( CommandType ) )
                {
                    object[] attribs =
                        t.GetCustomAttributes( typeof( CommandIdentifierAttribute ), false );

                    if ( attribs.Length > 0 )
                    {
                        CommandIdentifierAttribute identAttrib =
                            attribs[ 0 ] as CommandIdentifierAttribute;

                        stKnownTypes.Add( identAttrib.Identifier,
                            asm.CreateInstance( t.FullName ) as CommandType );
                    }
                }
            }
        }

        public static CommandType Get( String identifier )
        {
            if ( stKnownTypes == null )
                FindCommandTypes();

            if ( stKnownTypes.ContainsKey( identifier ) )
                return stKnownTypes[ identifier ];

            throw new UnknownCommandTypeException( identifier );
        }

        private string myIdentifier;

        public readonly ParameterType ParamType;
        public readonly bool BlockOpen;
        public readonly bool BlockClose;

        public string Identifier
        {
            get
            {
                if ( myIdentifier == null )
                {
                    object[] attribs = GetType()
                        .GetCustomAttributes( typeof( CommandIdentifierAttribute ), false );

                    if ( attribs.Length > 0 )
                    {
                        CommandIdentifierAttribute identAttrib =
                                attribs[ 0 ] as CommandIdentifierAttribute;

                        myIdentifier = identAttrib.Identifier;
                    }
                    else
                        myIdentifier = "nil";
                }

                return myIdentifier;
            }
        }

        public CommandType( ParameterType paramType, bool blockOpen = false, bool blockClose = false )
        {
            ParamType = paramType;
            BlockOpen = blockOpen;
            BlockClose = blockClose;
        }

        public virtual void Execute( Command command, Thread thread, Scope scope )
        {

        }

        public virtual void ExitInnerBlock( Command command, Thread thread, Scope scope )
        {

        }
    }
}
