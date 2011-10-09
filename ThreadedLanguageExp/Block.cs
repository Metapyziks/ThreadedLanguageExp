using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedLanguageExp
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
                            if ( depth == 0 )
                                break;
                        }
                        else if ( bcmd.CommandType.BlockOpen )
                            ++depth;
                        block.Add( bcmd );
                    }
                }
            }

            Commands = cmds.ToArray();
        }
    }
}
