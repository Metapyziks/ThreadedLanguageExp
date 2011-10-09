using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp
{
    internal enum Operator
    {
        Add,
        Subtract,
        Multiply,
        Divide
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

        public static Expression Parse( String str )
        {
            int index = 0;
            return Parse( str, ref index );
        }

        private static Expression Parse( String str, ref int index )
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
                    exp = Parse( ReadBracketContents( str, ref index ) );
                else
                    throw new Exception( "Unexpected symbol encountered at character "
                        + index + " (" + str[ index ] + ") in \"" + str + "\"." );

                type = FindNextTokenType( str, ref index );

                if ( type != TokenType.Operator )
                    return exp;

                Operator oper = ReadOperator( str, ref index );

                return new ExpressionBranch( exp, Parse( str, ref index ), oper );
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

            if ( char.IsDigit( str[ index ] ) || str[ index ] == '[' )
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
        private readonly Expression Left;
        private readonly Expression Right;

        public readonly Operator Operator;

        public ExpressionBranch( Expression left, Expression right, Operator oper )
        {
            Left = left;
            Right = right;
            Operator = oper;
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
            }

            return null;
        }

        public override string ToString()
        {
            switch ( Operator )
            {
                case Operator.Add:
                    return "( " + Left.ToString() + " + " + Right.ToString() + " )";
                case Operator.Subtract:
                    return "( " + Left.ToString() + " - " + Right.ToString() + " )";
                case Operator.Multiply:
                    return "( " + Left.ToString() + " * " + Right.ToString() + " )";
                case Operator.Divide:
                    return "( " + Left.ToString() + " / " + Right.ToString() + " )";
            }

            return base.ToString();
        }
    }
}
