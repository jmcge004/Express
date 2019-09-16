using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Express
{
    public interface IExpressable
    {
        int ZIndex { get; }
        bool CanHandle(string strInput);
        Expression Parse(string strInput, List<(string, Expression)> localContext = null);
    }
}
