// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;

namespace YellowFlavor.Serialization.Embedded.CodeDom.ms.Compiler
{
    public class CodeGeneratorOptions
    {
        private readonly IDictionary _options = new ListDictionary();

        public string IndentString
        {
            get
            {
                object o = _options[nameof(IndentString)];
                return o != null ? (string)o : "    ";
            }
            set => _options[nameof(IndentString)] = value;
        }

        public string BracingStyle
        {
            get
            {
                object o = _options[nameof(BracingStyle)];
                return o != null ? (string)o : "Block";
            }
            set => _options[nameof(BracingStyle)] = value;
        }

        public bool ElseOnClosing
        {
            get
            {
                object o = _options[nameof(ElseOnClosing)];
                return o != null && (bool)o;
            }
            set => _options[nameof(ElseOnClosing)] = value;
        }
    }
}
