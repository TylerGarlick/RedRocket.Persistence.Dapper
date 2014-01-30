//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqIf : ILinqIf
    {
        /// <summary>
        ///     Createa a new SQLinqIf object
        /// </summary>
        /// <param name="operator">The operator used for evaluating the IF clause</param>
        /// <param name="ifClause"></param>
        public LinqIf(LinqIfOperator @operator, ILinq ifClause)
        {
            Operator = @operator;
            If = ifClause;
        }

        /// <summary>
        ///     Createa a new SQLinqIf object
        /// </summary>
        /// <param name="clause">The SQL code to use for the IF clause</param>
        public LinqIf(string clause)
            : this(LinqIfOperator.None, clause) {}

        /// <summary>
        ///     Createa a new SQLinqIf object
        /// </summary>
        /// <param name="operator">The operator used for evaluating the IF clause</param>
        /// <param name="ifClause"></param>
        public LinqIf(LinqIfOperator @operator, string ifClause)
        {
            Operator = @operator;
            If = ifClause;
        }

        /// <summary>
        ///     The operator to use for the evaluation of the IF clause
        /// </summary>
        public LinqIfOperator Operator { get; set; }

        /// <summary>
        ///     The clause used to evaluate the IF condition
        /// </summary>
        public object If { get; protected set; }

        /// <summary>
        ///     Specifies the query to execute when the "IF" clause evaluates to TRUE
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ILinq Then { get; set; }

        /// <summary>
        ///     Specifies the query to execute when the "IF" clause evaluates to FALSE
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ILinq Else { get; set; }

        public ILinqResult ToSql(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            int paramCount = existingParameterCount;

            var result = new LinqIfResult
                         {
                             Operator = Operator
                         };

            if (If is string)
            {
                result.If = If as string;
            }
            else
            {
                ILinqResult ifResult = ((ILinq) If).ToSql(paramCount, parameterNamePrefix);
                result.If = ifResult.ToQuery();

                foreach (var p in ifResult.Parameters)
                {
                    result.Parameters.Add(p);
                }

                paramCount = existingParameterCount + result.Parameters.Count;
            }

            if (Then != null)
            {
                ILinqResult thenResult = Then.ToSql(paramCount, parameterNamePrefix);
                result.Then = thenResult.ToQuery();

                foreach (var p in thenResult.Parameters)
                {
                    result.Parameters.Add(p);
                }
                paramCount = existingParameterCount + result.Parameters.Count;
            }

            if (Else != null)
            {
                ILinqResult elseResult = Else.ToSql(paramCount, parameterNamePrefix);
                result.Else = elseResult.ToQuery();

                foreach (var p in elseResult.Parameters)
                {
                    result.Parameters.Add(p);
                }
            }

            return result;
        }
    }
}