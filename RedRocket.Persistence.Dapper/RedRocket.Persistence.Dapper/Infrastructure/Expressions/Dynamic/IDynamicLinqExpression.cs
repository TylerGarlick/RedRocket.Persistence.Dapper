//Copyright (c) Chris Pietschmann 2012 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic
{
    public interface IDynamicLinqExpression
    {
        SqlExpressionCompilerResult Compile(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix);
    }
}