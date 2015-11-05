using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public static class Utilities
{
    public static string NameOf(Expression<Action> x)
    {
        return NameOfInternal(x.Body);
    }

    public static string NameOf<T>(Expression<Func<T, object>> x)
    {
        return NameOfInternal(x.Body);
    }

    private static string NameOfInternal(Expression expression)
    {
        if (expression is UnaryExpression)
        {
            expression = ((UnaryExpression)expression).Operand;
        }

        if (expression is MemberExpression)
        {
            return BuildMemberExpressionName(expression);
        }
        else if (expression is MethodCallExpression)
        {
            return ((MethodCallExpression)expression).Method.Name;
        }
        else if (expression is ConstantExpression)
        {
            var value = ((ConstantExpression)expression).Value;

            return value != null ? value.ToString() : null;
        }
        else
        {
            throw new InvalidOperationException(string.Format("{0} is not supported.", expression.GetType()));
        }
    }

    private static string BuildMemberExpressionName(Expression expression)
    {
        var members = new List<string>();

        var memberExpression = expression as MemberExpression;

        while (memberExpression != null)
        {
            members.Add(memberExpression.Member.Name);

            memberExpression = memberExpression.Expression as MemberExpression;
        }

        members.Reverse();

        return string.Join(".", members.ToArray());
    }
}