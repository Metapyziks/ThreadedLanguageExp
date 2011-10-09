using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp
{
    internal class Thread
    {
        private struct BlockEntry
        {
            public readonly int EntryPoint;
            public readonly Block Block;

            public BlockEntry( int entryPoint, Block block )
            {
                EntryPoint = entryPoint;
                Block = block;
            }
        }

        private Stack<BlockEntry> myStack;
        private int myCurrentCommandNum;

        public Block CurrentBlock
        {
            get
            {
                if ( myStack.Count == 0 )
                    return null;

                return myStack.Peek().Block;
            }
        }

        public Command CurrentCommand
        {
            get
            {
                if ( myStack.Count == 0 )
                    return null;

                return CurrentBlock.Commands[ myCurrentCommandNum ];
            }
        }

        public bool Exited
        {
            get
            {
                return myStack.Count == 0;
            }
        }

        public Scope Scope;

        public Thread( Block startBlock, Scope startScope = null, bool global = false )
        {
            myStack = new Stack<BlockEntry>();

            if( !global )
                Scope = startScope;
            
            EnterBlock( startBlock );

            if( global )
                Scope = startScope;
        }

        public void EnterBlock( Block block )
        {
            myStack.Push( new BlockEntry( myCurrentCommandNum, block ) );

            myCurrentCommandNum = 0;
            Scope = new Scope( Scope );
        }

        public void ExitBlock()
        {
            myStack.Pop();
            Scope = Scope.Parent;

            if( myStack.Count > 0 )
                myCurrentCommandNum = myStack.Peek().EntryPoint;
        }

        public void Step()
        {
            CurrentCommand.Execute( this, Scope );

            myCurrentCommandNum++;

            if ( myCurrentCommandNum >= CurrentBlock.Commands.Length )
                ExitBlock();
        }
    }
}
