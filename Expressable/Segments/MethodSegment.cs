using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Express
{
    public class MethodSegment : IExpressable
    {
        public int ZIndex => 2;

        public bool CanHandle(string strInput)
        {
            return Regex.IsMatch(strInput, "^\\s*(?<root>[$a-z_0-9])(?<add>\\.[a-z_0-9]+)?\\(.*\\)\\s*$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        }

        public Expression Parse(string strInput, List<(string, Expression)> localContext = null)
        {
            throw new NotImplementedException();
        }
    }
}
