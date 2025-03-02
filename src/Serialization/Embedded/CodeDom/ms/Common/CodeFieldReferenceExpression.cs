// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace YellowFlavor.Serialization.Embedded.CodeDom.ms.Common
{
    public class CodeFieldReferenceExpression : CodeExpression
    {
        private string _fieldName;

        public CodeFieldReferenceExpression() { }

        public CodeFieldReferenceExpression(CodeExpression targetObject, string fieldName)
        {
            TargetObject = targetObject;
            FieldName = fieldName;
        }

        public CodeExpression TargetObject { get; set; }

        public string FieldName
        {
            get { return _fieldName ?? string.Empty; }
            set { _fieldName = value; }
        }
    }
}
