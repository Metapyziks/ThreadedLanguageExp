using System;
using System.Collections.Generic;

namespace ThreadedLanguage
{
    internal enum Operator
    {
        Add         = 0x50,
        Subtract    = 0x60,
        Multiply    = 0x70,
        Divide      = 0x80,
        And         = 0x40,
        Or          = 0x20,
        Xor         = 0x30,
        Equal       = 0x01,
        NotEqual    = 0x02,
        Greater     = 0x11,
        Less        = 0x10,

        GreaterOrEqual = 0x14,
        LessOrEqual    = 0x13
    }

    internal enum PrefixOperator
    {
        Not = 0x01,
        Plus = 0x02,
        Minus = 0x03
    }

    internal class Expression
    {
        private static readonly char[] stByteChars = new char[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
        };

        private enum TokenType
        {
            None,
            Value,
            Variable,
            Operator,
            PrefixOperator,
            OpenBracket,
            CloseBracket
        }

        public static Expression Parse( String str, bool bracketed = false )
        {
            int index = 0;
            return Parse( str, ref index, true );
        }

        private static Expression Parse( String str, ref int index, bool bracketed = false )
        {
            TokenType type = FindNextTokenType( str, ref index, true );

            if ( index < str.Length )
            {
                Expression exp;

                bool negative = false;
                bool not = false;

                while ( type == TokenType.PrefixOperator )
                {
                    PrefixOperator op = ReadPrefixOperator( str, ref index );
                    switch ( op )
                    {
                        case PrefixOperator.Not:
                            not = !not;
                            break;
                        case PrefixOperator.Minus:
                            negative = !negative;
                            break;
                    }

                    type = FindNextTokenType( str, ref index, true );

                    if ( index >= str.Length )
                        return null;
                }

                if ( type == TokenType.Value )
                    exp = new ExpressionValue( ReadValue( str, ref index ) );
                else if ( type == TokenType.Variable )
                    exp = new ExpressionVar( ReadVariable( str, ref index ) );
                else if ( type == TokenType.OpenBracket )
                    exp = Parse( ReadBracketContents( str, ref index ), true );
                else
                    throw new Exception( "Unexpected symbol encountered at character "
                        + index + " (" + str[ index ] + ") in \"" + str + "\"." );

                exp.Not = not ? !exp.Not : exp.Not;
                exp.Minus = negative ? !exp.Minus : exp.Minus;

                type = FindNextTokenType( str, ref index, false );

                if ( type != TokenType.Operator )
                    return exp;

                Operator oper = ReadOperator( str, ref index );

                ExpressionBranch branch =
                    new ExpressionBranch( exp, Parse( str, ref index ), oper, bracketed );
                
                ExpressionBranch toReturn = branch;
                ExpressionBranch parent = branch;
                ExpressionBranch right;

                while ( branch.Right is ExpressionBranch &&
                    (int) ( right = branch.Right as ExpressionBranch )
                    .Operator + 15 <= (int) branch.Operator &&
                    !right.Bracketed )
                {
                    branch.Right = right.Left;
                    right.Left = branch;

                    if ( branch.Bracketed )
                    {
                        branch.Bracketed = false;
                        right.Bracketed = true;
                    }

                    if( toReturn == branch )
                        toReturn = right;

                    if ( parent != branch )
                        parent.Left = right;

                    parent = right;
                }

                return toReturn;
            }
            else
                return null;
        }

        private static void SkipWhitespace( String str, ref int index )
        {
            while ( index < str.Length && char.IsWhiteSpace( str[ index ] ) )
                ++index;
        }

        private static TokenType FindNextTokenType( String str, ref int index, bool postOp )
        {
            SkipWhitespace( str, ref index );

            if ( index == str.Length )
                return TokenType.None;

            if ( char.IsDigit( str[ index ] ) ||
                str[ index ] == '[' || str[ index ] == '"' || str[ index ] == '\'' )
                return TokenType.Value;
            if ( char.IsLetter( str[ index ] ) || str[ index ] == '_' )
            {
                if ( ( str.Length >= index + 4 && str.Substring( index, 4 ) == "true" &&
                    ( str.Length == index + 4
                    || !char.IsLetterOrDigit( str[ index + 4 ] ) || str[ index + 4 ] != '_' ) ) ||
                    ( str.Length >= index + 5 && str.Substring( index, 5 ) == "false" &&
                    ( str.Length == index + 5
                    || !char.IsLetterOrDigit( str[ index + 5 ] ) || str[ index + 5 ] != '_' ) ) )
                    return TokenType.Value;

                return TokenType.Variable;
            }
            if ( str[ index ] == '(' )
                return TokenType.OpenBracket;
            if ( str[ index ] == ')' )
                return TokenType.CloseBracket;
            if ( postOp && ( str[ index ] == '!' || str[ index ] == '+' || str[ index ] == '-' ) )
                return TokenType.PrefixOperator;

            return TokenType.Operator;
        }

        private static String ReadBracketContents( String str, ref int index )
        {
            ++index;

            int depth = 0;
            String contents = "";

            while ( index < str.Length )
            {
                if ( str[ index ] == '(' )
                    ++depth;
                else if ( str[ index ] == ')' )
                {
                    --depth;
                    if ( depth == -1 )
                    {
                        ++index;
                        break;
                    }
                }

                contents += str[ index++ ];
            }

            return contents;
        }

        private static PrefixOperator ReadPrefixOperator( String str, ref int index )
        {
            SkipWhitespace( str, ref index );

            switch ( str[ index++ ] )
            {
                case '!':
                    return PrefixOperator.Not;
                case '+':
                    return PrefixOperator.Plus;
                case '-':
                    return PrefixOperator.Minus;
            }

            throw new Exception( "This should never actually happen" );
        }

        private static readonly Dictionary<String, Operator> stOpers = new Dictionary<string, Operator>()
        {
            { ">=", Operator.GreaterOrEqual },
            { "<=", Operator.LessOrEqual },
            { "==", Operator.Equal },
            { "!=", Operator.NotEqual },
            { "+", Operator.Add },
            { "-", Operator.Subtract },
            { "*", Operator.Multiply },
            { "/", Operator.Divide },
            { "&", Operator.And },
            { "|", Operator.Or },
            { "^", Operator.Xor },
            { ">", Operator.Greater },
            { "<", Operator.Less }
        };

        private static Operator ReadOperator( String str, ref int index )
        {
            SkipWhitespace( str, ref index );

            foreach ( KeyValuePair<String, Operator> keyVal in stOpers )
            {
                bool match = true;

                for ( int i = 0; i < keyVal.Key.Length; ++i )
                {
                    if ( str.Length < index + i
                        || keyVal.Key[ i ] != str[ index + i ] )
                    {
                        match = false;
                        break;
                    }
                }

                if ( match )
                {
                    index += keyVal.Key.Length;
                    return keyVal.Value;
                }
            }

            throw new Exception( "Unknown token encountered: \""
                + str[ index - 1 ] + "\"" );
        }

        private static TLObject ReadValue( String str, ref int index )
        {
            SkipWhitespace( str, ref index );

            string digits;

            if ( str[ index ] == '[' )
            {
                digits = str.Substring( index + 1, 2 ).ToLower();
                index += 4;
                return new TLByt( (byte) (
                    ( Array.IndexOf( stByteChars, digits[ 0 ] ) * 0x10 )
                    | Array.IndexOf( stByteChars, digits[ 1 ] ) ) );
            }

            if ( str[ index ] == '\'' )
            {
                char val = (char) 0;
                if ( str[ ++index ] == '\\' )
                {
                    switch ( str[ ++index ] )
                    {
                        case 'n':
                            val = '\n'; break;
                        case 't':
                            val = '\t'; break;
                        case 'r':
                            val = '\r'; break;
                        case '\\':
                            val = '\\'; break;
                        case '\'':
                            val = '\''; break;
                    }
                }
                else
                    val = str[ index ];

                index += 2;

                return new TLByt( (byte) val );
            }

            if ( str[ index ] == '"' )
            {
                String val = "";
                bool escape = false;
                while ( ++index < str.Length )
                {
                    if ( !escape && str[ index ] == '"' )
                    {
                        ++index;
                        break;
                    }

                    if ( str[ index ] == '\\' )
                    {
                        escape = !escape;

                        if ( escape )
                            continue;
                    }
                    else if ( escape )
                    {
                        switch ( str[ index ] )
                        {
                            case 'n':
                                val += '\n'; break;
                            case 't':
                                val += '\t'; break;
                            case 'r':
                                val += '\r'; break;
                            case '\\':
                                val += '\\'; break;
                            case '"':
                                val += '"'; break;
                        }

                        escape = false;
                    }
                    else
                        val += str[ index ];
                }

                return new TLStr( val );
            }

            if ( str[ index ] == 't' )
            {
                index += 4;
                return new TLBit( true );
            }

            if ( str[ index ] == 'f' )
            {
                index += 5;
                return new TLBit( false );
            }

            digits = "";
            bool dec = false;

            while ( index < str.Length )
            {
                if ( str[ index ] == '.' )
                    dec = true;
                else if ( !char.IsNumber( str[ index ] ) )
                    break;

                digits += str[ index++ ];
            }

            if ( dec )
                return new TLDec( double.Parse( digits ) );
            
            return new TLInt( int.Parse( digits ) );
        }

        private static String ReadVariable( String str, ref int index )
        {
            SkipWhitespace( str, ref index );

            string ident = "";

            while ( index < str.Length )
            {
                if ( !char.IsLetterOrDigit( str[ index ] )
                    && str[ index ] != '_' )
                    break;

                ident += str[ index++ ];
            }

            return ident;
        }

        public bool Not;
        public bool Minus;

        public virtual TLObject Evaluate( Scope scope )
        {
            return null;
        }
    }

    internal class ExpressionLeaf : Expression
    {
    }

    internal class ExpressionVar : ExpressionLeaf
    {
        public readonly string Identifier;

        public ExpressionVar( String identifier )
        {
            Identifier = identifier;
        }

        public override TLObject Evaluate( Scope scope )
        {
            if ( Not )
                return scope[ Identifier ].Not();

            if ( Minus )
                return scope[ Identifier ].Minus();

            return scope[ Identifier ];
        }

        public override string ToString()
        {
            return ( Not ? "!" : Minus ? "-" : "" ) + Identifier;
        }
    }

    internal class ExpressionValue : ExpressionLeaf
    {
        public readonly TLObject Value;

        public ExpressionValue( TLObject value )
        {
            Value = value;
        }

        public override TLObject Evaluate( Scope scope )
        {
            if ( Not )
                return Value.Not();

            if ( Minus )
                return Value.Minus();

            return Value;
        }

        public override string ToString()
        {
            return ( Not ? "!" : Minus ? "-" : "" ) + Value.ToString();
        }
    }

    internal class ExpressionBranch : Expression
    {
        public bool Bracketed;

        public Expression Left;
        public Expression Right;

        public readonly Operator Operator;

        public ExpressionBranch( Expression left, Expression right, Operator oper, bool bracketed = false )
        {
            Left = left;
            Right = right;
            Operator = oper;

            Bracketed = bracketed;
        }

        public override TLObject Evaluate( Scope scope )
        {
            TLObject left = Left.Evaluate( scope );

            TLObject result;

            switch ( Operator )
            {
                case Operator.Add:
                    result = left.Add( Right.Evaluate( scope ) ); break;
                case Operator.Subtract:
                    result = left.Subtract( Right.Evaluate( scope ) ); break;
                case Operator.Multiply:
                    result = left.Multiply( Right.Evaluate( scope ) ); break;
                case Operator.Divide:
                    result = left.Divide( Right.Evaluate( scope ) ); break;
                case Operator.And:
                    result = left.And( Right.Evaluate( scope ) ); break;
                case Operator.Or:
                    result = left.Or( Right.Evaluate( scope ) ); break;
                case Operator.Xor:
                    result = left.Xor( Right.Evaluate( scope ) ); break;
                case Operator.Equal:
                    result = left.Equal( Right.Evaluate( scope ) ); break;
                case Operator.NotEqual:
                    result = left.NotEqual( Right.Evaluate( scope ) ); break;
                case Operator.Greater:
                    result = left.Greater( Right.Evaluate( scope ) ); break;
                case Operator.Less:
                    result = left.Less( Right.Evaluate( scope ) ); break;
                case Operator.GreaterOrEqual:
                    result = left.GreaterOrEqual( Right.Evaluate( scope ) ); break;
                case Operator.LessOrEqual:
                    result = left.LessOrEqual( Right.Evaluate( scope ) ); break;
                default:
                    return null;
            }

            if ( Not )
                return result.Not();

            if ( Minus )
                return result.Minus();

            return result;
        }

        public override string ToString()
        {
            String str = ( Not ? "!" : Minus ? "-" : "" );

            switch ( Operator )
            {
                case Operator.Add:
                    str += "(" + Left.ToString() + "+" + Right.ToString() + ")"; break;
                case Operator.Subtract:
                    str += "(" + Left.ToString() + "-" + Right.ToString() + ")"; break;
                case Operator.Multiply:
                    str += "(" + Left.ToString() + "*" + Right.ToString() + ")"; break;
                case Operator.Divide:
                    str += "(" + Left.ToString() + "/" + Right.ToString() + ")"; break;
                case Operator.And:
                    str += "(" + Left.ToString() + "&" + Right.ToString() + ")"; break;
                case Operator.Or:
                    str += "(" + Left.ToString() + "|" + Right.ToString() + ")"; break;
                case Operator.Xor:
                    str += "(" + Left.ToString() + "^" + Right.ToString() + ")"; break;
                case Operator.Equal:
                    str += "(" + Left.ToString() + "==" + Right.ToString() + ")"; break;
                case Operator.NotEqual:
                    str += "(" + Left.ToString() + "!=" + Right.ToString() + ")"; break;
                case Operator.Greater:
                    str += "(" + Left.ToString() + ">" + Right.ToString() + ")"; break;
                case Operator.Less:
                    str += "(" + Left.ToString() + "<" + Right.ToString() + ")"; break;
                case Operator.GreaterOrEqual:
                    str += "(" + Left.ToString() + ">=" + Right.ToString() + ")"; break;
                case Operator.LessOrEqual:
                    str += "(" + Left.ToString() + "<=" + Right.ToString() + ")"; break;
                default:
                    return base.ToString();
            }

            return str;
        }
    }
}
