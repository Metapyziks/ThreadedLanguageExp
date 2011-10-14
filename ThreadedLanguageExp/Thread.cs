using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp
{
    internal class Thread
    {
        private static Scope stGlobalScope = new Scope( null, true );

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

        public static int MaximumStackSize = 256;

        private Stack<BlockEntry> myStack;
        private Stack<Scope> myPastScopes;
        private int myCurrentCommandNum;

        private Thread mySyncThread;
        private bool myGrouped;

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

        public bool IsSyncing
        {
            get
            {
                return mySyncThread != null;
            }
        }

        public Thread SyncThread
        {
            get
            {
                return mySyncThread;
            }
        }

        public bool Exited
        {
            get
            {
                return myStack.Count == 0;
            }
        }

        public bool Grouped
        {
            get
            {
                return myGrouped;
            }
        }

        public Scope Scope;

        public Thread( Block startBlock, Scope startScope = null, bool keepScope = false )
        {
            myStack = new Stack<BlockEntry>();
            myPastScopes = new Stack<Scope>();

            if( !keepScope )
                Scope = startScope ?? stGlobalScope;
            
            EnterBlock( startBlock );

            if( keepScope )
                Scope = startScope ?? stGlobalScope;
        }

        public void EnterBlock( Block block, bool newScope = false, Scope scope = null )
        {
            if ( myStack.Count >= MaximumStackSize )
                throw new Exception( "Stack overflow when attempting to enter block." );

            myStack.Push( new BlockEntry( myCurrentCommandNum, block ) );

            myCurrentCommandNum = 0;

            if ( !newScope )
                Scope = new Scope( Scope );
            else
            {
                myPastScopes.Push( Scope );
                Scope = scope ?? stGlobalScope;
            }

            if ( myCurrentCommandNum >= CurrentBlock.Commands.Length )
                ExitBlock();
        }

        public void ExitBlock()
        {
            BlockEntry top = myStack.Pop();
            Scope = Scope.Parent;

            if ( Scope == null && myPastScopes.Count > 0 )
                Scope = myPastScopes.Pop();

            if ( myStack.Count > 0 )
            {
                myCurrentCommandNum = top.EntryPoint;
                CurrentCommand.ExitInnerBlock( this, Scope );
            }
        }

        public void Step()
        {
            if ( IsSyncing )
            {
                if ( SyncThread.SyncThread != this )
                    return;

                SyncThread.Advance();
                Advance();

                SyncThread.mySyncThread = null;
                mySyncThread = null;
            }

            do
                CurrentCommand.Execute( this, Scope );
            while ( Grouped && CurrentCommand != null );
        }

        public void Advance()
        {
            ++myCurrentCommandNum;

            if ( myCurrentCommandNum >= CurrentBlock.Commands.Length )
                ExitBlock();
        }

        public void Sync( Thread thread )
        {
            mySyncThread = thread;
        }

        public void BeginGroup()
        {
            myGrouped = true;
        }

        public void EndGroup()
        {
            myGrouped = false;
        }
    }
}
