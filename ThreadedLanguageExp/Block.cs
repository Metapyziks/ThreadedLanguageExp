﻿using System.Collections.Generic;

namespace ThreadedLanguage
{
    internal class Block
    {
        public readonly Command[] Commands;

        public Block( Command[] commands )
        {
            List<Command> cmds = new List<Command>();

            int i = 0;
            while ( i < commands.Length )
            {
                Command cmd = commands[ i++ ];
                cmds.Add( cmd );

                if ( cmd.CommandType.BlockOpen )
                {
                    List<Command> block = new List<Command>();
                    int depth = 0;
                    while ( i < commands.Length )
                    {
                        Command bcmd = commands[ i++ ];
                        if ( bcmd.CommandType.BlockClose )
                        {
                            --depth;

                            if ( depth == -1 )
                            {
                                if ( bcmd.CommandType.BlockOpen )
                                    --i;

                                break;
                            }
                        }
                        
                        if ( bcmd.CommandType.BlockOpen )
                            ++depth;
                        
                        block.Add( bcmd );
                    }

                    cmd.InnerBlock = new Block( block.ToArray() );
                }
            }

            Commands = cmds.ToArray();
        }
    }
}
