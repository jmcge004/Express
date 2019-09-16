using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Linq;

namespace Express
{
    public abstract class Expressable : IExpressable
    {
        public abstract int ZIndex { get; }
        public abstract bool CanHandle(string strInput);
        public abstract Expression Parse(string strInput, List<(string, Expression)> localContext = null);

        internal static List<Expressable> Expressables { get; }

        public static List<Type> ExposeTypes { get; set; }
        public static List<(string, Expression)> GlobalContext { get; set; }
        public static List<MemberInfo> ExposeMembers { get; set; }
        static Expressable()
        {
            GlobalContext = new List<(string, Expression)>();
            ExposeTypes = new List<Type>();
            ExposeMembers = new List<MemberInfo>();
            if (Expressables == null)
                Expressables = Assembly.GetExecutingAssembly().GetExportedTypes().Where(s => s.IsClass && !s.IsAbstract && s.IsSubclassOf(typeof(Expressable))).Select(s =>
                   (Expressable)Activator.CreateInstance(s)).OrderBy(s=>s.ZIndex).ToList();
        }

        public static Expression ParseExpression(string strExpression)
        {   
                return ParseType(strExpression).Parse(strExpression);
        }

        protected static Expression ParseExpression(string strExpression,List<(string, Expression)> localContext)
        {
            return ParseType(strExpression).Parse(strExpression,localContext);
        }

        public static Expressable ParseType(string strExpression)
        {
            if (Expressables.FirstOrDefault(t => t.CanHandle(strExpression)) != null)
                return Expressables.First(t => t.CanHandle(strExpression));
            throw new Exception("no handler found");
        }
    }
}
