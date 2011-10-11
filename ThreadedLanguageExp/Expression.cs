using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp
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
        Equals      = 0x12,
        Greater     = 0x11,
        Less        = 0x10
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
            TokenType type = FindNextTokenType( str, ref index );

            if ( index < str.Length )
            {
                Expression exp;

                if ( type == TokenType.Value )
                    exp = new ExpressionValue( ReadValue( str, ref index ) );
                else if ( type == TokenType.Variable )
                    exp = new ExpressionVar( ReadVariable( str, ref index ) );
                else if ( type == TokenType.OpenBracket )
                    exp = Parse( ReadBracketContents( str, ref index ), true );
                else
                    throw new Exception( "Unexpected symbol encountered at character "
                        + index + " (" + str[ index ] + ") in \"" + str + "\"." );

                type = FindNextTokenType( str, ref index );

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

        private static TokenType FindNextTokenType( String str, ref int index )
        {
            SkipWhitespace( str, ref index );

            if ( index == str.Length )
                return TokenType.None;

            if ( char.IsDigit( str[ index ] ) ||
                str[ index ] == '[' || str[ index ] == '"' )
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

        private static Operator ReadOperator( String str, ref int index )
        {
            SkipWhitespace( str, ref index );

            switch ( str[ index++ ] )
            {
                case '+':
                    return Operator.Add;
                case '-':
                    return Operator.Subtract;
                case '*':
                    return Operator.Multiply;
                case '/':
                    return Operator.Divide;
                case '&':
                    return Operator.And;
                case '|':
                    return Operator.Or;
                case '^':
                    return Operator.Xor;
                case '=':
                    return Operator.Equals;
                case '>':
                    return Operator.Greater;
                case '<':
                    return Operator.Less;
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
                        escape = !escape;
                    else
                        escape = false;

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
            return scope[ Identifier ];
        }

        public override string ToString()
        {
            return Identifier;
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
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
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

            switch ( Operator )
            {
                case Operator.Add:
                    return left.Add( Right.Evaluate( scope ) );
                case Operator.Subtract:
                    return left.Subtract( Right.Evaluate( scope ) );
                case Operator.Multiply:
                    return left.Multiply( Right.Evaluate( scope ) );
                case Operator.Divide:
                    return left.Divide( Right.Evaluate( scope ) );
                case Operator.And:
                    return left.And( Right.Evaluate( scope ) );
                case Operator.Or:
                    return left.Or( Right.Evaluate( scope ) );
                case Operator.Xor:
                    return left.Xor( Right.Evaluate( scope ) );
                case Operator.Equals:
                    return left.Equals( Right.Evaluate( scope ) );
                case Operator.Greater:
                    return left.Greater( Right.Evaluate( scope ) );
                case Operator.Less:
                    return left.Less( Right.Evaluate( scope ) );
            }

            return null;
        }

        public override string ToString()
        {
            switch ( Operator )
            {
                case Operator.Add:
                    return "(" + Left.ToString() + "+" + Right.ToString() + ")";
                case Operator.Subtract:
                    return "(" + Left.ToString() + "-" + Right.ToString() + ")";
                case Operator.Multiply:
                    return "(" + Left.ToString() + "*" + Right.ToString() + ")";
                case Operator.Divide:
                    return "(" + Left.ToString() + "/" + Right.ToString() + ")";
                case Operator.And:
                    return "(" + Left.ToString() + "&" + Right.ToString() + ")";
                case Operator.Or:
                    return "(" + Left.ToString() + "|" + Right.ToString() + ")";
                case Operator.Xor:
                    return "(" + Left.ToString() + "^" + Right.ToString() + ")";
                case Operator.Equals:
                    return "(" + Left.ToString() + "=" + Right.ToString() + ")";
                case Operator.Greater:
                    return "(" + Left.ToString() + ">" + Right.ToString() + ")";
                case Operator.Less:
                    return "(" + Left.ToString() + "<" + Right.ToString() + ")";
            }

            return base.ToString();
        }
    }
}
