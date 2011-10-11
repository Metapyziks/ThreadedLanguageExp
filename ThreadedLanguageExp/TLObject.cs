using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ThreadedLanguageExp
{
    public class BadOperatorException : Exception
    {
        internal BadOperatorException( TLObject obj, Operator oper )
            : base( "The operator \"" + Enum.GetName( typeof( Operator ), oper )
                + "\" cannot be used with a value of type \"" + obj.GetType().Name + "." )
        {

        }
    }

    internal class TLObject
    {
        public static T Convert<T>( TLObject obj )
            where T : TLObject
        {
            return Convert( typeof( T ), obj ) as T;
        }

        public static TLObject Convert( Type type, TLObject obj )
        {
            return type.GetConstructor( new Type[] { typeof( TLObject ) } )
                .Invoke( new object[] { obj } ) as TLObject;
        }

        public TLObject()
        {

        }

        public TLObject( TLObject convert )
        {

        }

        public virtual TLObject Add( TLObject obj )
        {
            throw new BadOperatorException( this, Operator.Add );
        }

        public virtual TLObject Subtract( TLObject obj )
        {
            throw new BadOperatorException( this, Operator.Subtract );
        }

        public virtual TLObject Multiply( TLObject obj )
        {
            throw new BadOperatorException( this, Operator.Multiply );
        }

        public virtual TLObject Divide( TLObject obj )
        {
            throw new BadOperatorException( this, Operator.Divide );
        }

        public virtual TLObject And( TLObject obj )
        {
            throw new BadOperatorException( this, Operator.And );
        }

        public virtual TLObject Or( TLObject obj )
        {
            throw new BadOperatorException( this, Operator.Or );
        }

        public virtual TLObject Xor( TLObject obj )
        {
            throw new BadOperatorException( this, Operator.Xor );
        }

        public virtual TLObject Equals( TLObject obj )
        {
            throw new BadOperatorException( this, Operator.Equals );
        }

        public virtual TLObject Greater( TLObject obj )
        {
            throw new BadOperatorException( this, Operator.Xor );
        }

        public virtual TLObject Less( TLObject obj )
        {
            throw new BadOperatorException( this, Operator.Equals );
        }
    }

    internal class TLInt : TLObject
    {
        public int Value;

        public TLInt( int value )
        {
            Value = value;
        }

        public TLInt( TLObject convert )
            : base( convert )
        {
            if ( convert is TLInt )
                Value = ( convert as TLInt ).Value;
            else if ( convert is TLBit )
                Value = ( convert as TLBit ).Value ? 1 : 0;
            else if ( convert is TLByt )
                Value = ( convert as TLByt ).Value;
            else if ( convert is TLDec )
                Value = (int) ( convert as TLDec ).Value;
            else
                Value = 0;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override TLObject Add( TLObject obj )
        {
            return new TLInt( Value + new TLInt( obj ).Value );
        }

        public override TLObject Subtract( TLObject obj )
        {
            return new TLInt( Value - new TLInt( obj ).Value );
        }

        public override TLObject Multiply( TLObject obj )
        {
            return new TLInt( Value * new TLInt( obj ).Value );
        }

        public override TLObject Divide( TLObject obj )
        {
            return new TLInt( Value / new TLInt( obj ).Value );
        }

        public override TLObject And( TLObject obj )
        {
            return new TLInt( Value & new TLInt( obj ).Value );
        }

        public override TLObject Or( TLObject obj )
        {
            return new TLInt( Value | new TLInt( obj ).Value );
        }

        public override TLObject Xor( TLObject obj )
        {
            return new TLInt( Value ^ new TLInt( obj ).Value );
        }

        public override TLObject Equals( TLObject obj )
        {
            return new TLBit( Value == new TLInt( obj ).Value );
        }

        public override TLObject Greater( TLObject obj )
        {
            return new TLBit( Value > new TLInt( obj ).Value );
        }

        public override TLObject Less( TLObject obj )
        {
            return new TLBit( Value < new TLInt( obj ).Value );
        }
    }

    internal class TLBit : TLObject
    {
        public bool Value;

        public TLBit( bool value )
        {
            Value = value;
        }

        public TLBit( TLObject convert )
            : base( convert )
        {
            if ( convert is TLInt )
                Value = ( convert as TLInt ).Value != 0;
            else if ( convert is TLBit )
                Value = ( convert as TLBit ).Value;
            else if ( convert is TLByt )
                Value = ( convert as TLByt ).Value != 0;
            else if ( convert is TLDec )
                Value = ( convert as TLDec ).Value != 0.0;
            else if ( convert is TLStr )
                Value = ( convert as TLStr ).DataWaiting;
            else
                Value = false;
        }

        public override TLObject And( TLObject obj )
        {
            return new TLBit( Value && new TLBit( obj ).Value );
        }

        public override TLObject Or( TLObject obj )
        {
            return new TLBit( Value || new TLBit( obj ).Value );
        }

        public override TLObject Xor( TLObject obj )
        {
            return new TLBit( Value ^ new TLBit( obj ).Value );
        }

        public override TLObject Equals( TLObject obj )
        {
            return new TLBit( Value == new TLBit( obj ).Value );
        }

        public override TLObject Greater( TLObject obj )
        {
            return new TLBit( Value && !new TLBit( obj ).Value );
        }

        public override TLObject Less( TLObject obj )
        {
            return new TLBit( !Value && new TLBit( obj ).Value );
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    internal class TLByt : TLObject
    {
        public byte Value;

        public TLByt( byte value )
        {
            Value = value;
        }

        public TLByt( TLObject convert )
            : base( convert )
        {
            if ( convert is TLInt )
                Value = (byte) ( convert as TLInt ).Value;
            else if ( convert is TLBit )
                Value = (byte) ( ( convert as TLBit ).Value ? 1 : 0 );
            else if ( convert is TLByt )
                Value = ( convert as TLByt ).Value;
            else if ( convert is TLDec )
                Value = (byte) ( convert as TLDec ).Value;
            else
                Value = 0;
        }

        public override string ToString()
        {
            return ( (char) Value ).ToString();
        }

        public override TLObject Add( TLObject obj )
        {
            return new TLByt( (byte)( Value + new TLInt( obj ).Value ) );
        }

        public override TLObject Subtract( TLObject obj )
        {
            return new TLByt( (byte) ( Value - new TLInt( obj ).Value ) );
        }

        public override TLObject Multiply( TLObject obj )
        {
            return new TLByt( (byte) ( Value * new TLInt( obj ).Value ) );
        }

        public override TLObject Divide( TLObject obj )
        {
            return new TLByt( (byte) ( Value / new TLInt( obj ).Value ) );
        }

        public override TLObject And( TLObject obj )
        {
            return new TLByt( (byte) ( Value & new TLByt( obj ).Value ) );
        }

        public override TLObject Or( TLObject obj )
        {
            return new TLByt( (byte) ( Value | new TLByt( obj ).Value ) );
        }

        public override TLObject Xor( TLObject obj )
        {
            return new TLByt( (byte) ( Value ^ new TLByt( obj ).Value ) );
        }

        public override TLObject Equals( TLObject obj )
        {
            return new TLBit( Value == new TLByt( obj ).Value );
        }

        public override TLObject Greater( TLObject obj )
        {
            return new TLBit( Value > new TLByt( obj ).Value );
        }

        public override TLObject Less( TLObject obj )
        {
            return new TLBit( Value < new TLByt( obj ).Value );
        }
    }

    internal class TLDec : TLObject
    {
        public double Value;

        public TLDec( double value )
        {
            Value = value;
        }

        public TLDec( TLObject convert )
            : base( convert )
        {
            if ( convert is TLInt )
                Value = ( convert as TLInt ).Value;
            else if ( convert is TLBit )
                Value = ( convert as TLBit ).Value ? 1 : 0;
            else if ( convert is TLByt )
                Value = ( convert as TLByt ).Value;
            else if ( convert is TLDec )
                Value = ( convert as TLDec ).Value;
            else
                Value = 0.0;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override TLObject Add( TLObject obj )
        {
            return new TLDec( Value + new TLDec( obj ).Value );
        }

        public override TLObject Subtract( TLObject obj )
        {
            return new TLDec( Value - new TLDec( obj ).Value );
        }

        public override TLObject Multiply( TLObject obj )
        {
            return new TLDec( Value * new TLDec( obj ).Value );
        }

        public override TLObject Divide( TLObject obj )
        {
            return new TLDec( Value / new TLDec( obj ).Value );
        }

        public override TLObject Equals( TLObject obj )
        {
            return new TLBit( Value == new TLDec( obj ).Value );
        }

        public override TLObject Greater( TLObject obj )
        {
            return new TLBit( Value > new TLDec( obj ).Value );
        }

        public override TLObject Less( TLObject obj )
        {
            return new TLBit( Value < new TLDec( obj ).Value );
        }
    }

    internal class TLStr : TLObject
    {
        private long myReadPos;
        private long myWritePos;

        public MemoryStream Stream;

        public bool DataWaiting
        {
            get
            {
                return myReadPos < myWritePos;
            }
        }

        public TLStr()
        {
            Stream = new MemoryStream();

            myReadPos = 0;
            myWritePos = 0;
        }

        public TLStr( String str )
        {
            FromString( str );
        }

        public TLStr( TLObject convert )
            : base( convert )
        {
            if ( convert is TLStr )
            {
                Stream = ( convert as TLStr ).Stream;
                myReadPos = ( convert as TLStr ).myReadPos;
                myWritePos = ( convert as TLStr ).myWritePos;
            }
            else
                FromString( convert.ToString() );
        }

        private void FromString( String str )
        {
            Stream = new MemoryStream();

            for ( int i = 0; i < str.Length; ++i )
                Stream.WriteByte( (byte) str[ i ] );

            myReadPos = 0;
            myWritePos = str.Length;
        }

        public override TLObject Add( TLObject obj )
        {
            return new TLStr( ToString() + obj.ToString() );
        }

        public TLByt Read()
        {
            Stream.Position = myReadPos++;
            return new TLByt( (byte) Stream.ReadByte() );
        }

        public void Write( TLByt value )
        {
            Stream.Position = myWritePos++;
            Stream.WriteByte( value.Value );
        }

        public override string ToString()
        {
            String str = "";

            byte[] buffer = Stream.GetBuffer();
            for ( int i = 0; i < myWritePos; ++i )
                str += (char) buffer[ i ];

            return str;
        }
    }

    internal class TLSub : TLObject
    {
        public Block Block;
        public Scope Scope;

        public TLSub( Block block, Scope scope )
        {
            Block = block;
            Scope = scope;
        }

        public TLSub( TLObject convert )
            : base( convert )
        {
            if ( convert is TLSub )
            {
                Block = ( convert as TLSub ).Block;
                Scope = ( convert as TLSub ).Scope;
            }
            else
            {
                Block = null;
                Scope = null;
            }
        }

        public override string ToString()
        {
            return "Subroutine";
        }
    }

    internal class TLThr : TLObject
    {
        public Thread Thread;

        public TLThr( Thread thread )
        {
            Thread = thread;
        }

        public TLThr( TLObject convert )
            : base( convert )
        {
            if ( convert is TLThr )
                Thread = ( convert as TLThr ).Thread;
            else
                Thread = null;
        }

        public override string ToString()
        {
            return "Thread";
        }
    }
}
