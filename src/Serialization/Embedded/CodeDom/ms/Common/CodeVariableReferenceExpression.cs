// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace YellowFlavor.Serialization.Embedded.CodeDom.ms.Common
{
    public class CodeVariableReferenceExpression : CodeExpression
    {
        private string _variableName;

        public CodeVariableReferenceExpression() { }

        public CodeVariableReferenceExpression(string variableName)
        {
            _variableName = variableName;
        }

        public string VariableName
        {
            get { return _variableName ?? string.Empty; }
            set { _variableName = value; }
        }
    }
}
