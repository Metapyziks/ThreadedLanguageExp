using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ThreadedLanguageExp.Commands
{
    [CommandIdentifier( "end" )]
    internal class CmdEnd : CommandType
    {
        public CmdEnd()
            : base( ParameterType.None, false, true )
        {

        }
    }
}
