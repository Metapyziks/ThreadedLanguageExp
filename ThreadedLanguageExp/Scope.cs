using System;
using System.Collections.Generic;

namespace ThreadedLanguage
{
    public class UndeclaredVariableException : Exception
    {
        public UndeclaredVariableException( String identifier )
            : base( "A variable with the identifier \"" + identifier
                + "\" was referenced but has not been declared." )
        {

        }
    }

    internal class Scope
    {
        private Dictionary<String, TLObject> myVariables;

        private readonly Scope myBase;

        public readonly Scope Parent;

        public Scope( Scope parent = null, bool global = false )
        {
            Parent = parent;

            if ( parent != null )
                myBase = parent.myBase ?? parent;

            myVariables = new Dictionary<string, TLObject>();
        }

        public TLObject this[ String identifier ]
        {
            get
            {
                if ( myVariables.ContainsKey( identifier ) )
                    return myVariables[ identifier ];
                else if ( Parent != null && Parent.IsDeclared( identifier ) )
                    return Parent[ identifier ];
                else
                    throw new UndeclaredVariableException( identifier );
            }
            set
            {
                if ( myVariables.ContainsKey( identifier ) )
                    myVariables[ identifier ] =
                        TLObject.Convert( myVariables[ identifier ].GetType(), value );
                else if ( Parent != null && Parent.IsDeclared( identifier ) )
                    Parent[ identifier ] = value;
                else
                    throw new UndeclaredVariableException( identifier );
            }
        }

        public bool IsDeclared( String identifier )
        {
            return myVariables.ContainsKey( identifier )
                || ( Parent != null && Parent.IsDeclared( identifier ) );
        }

        public void Declare( String identifier, TLObject value )
        {
            myVariables.Add( identifier, value );
        }
    }
}
