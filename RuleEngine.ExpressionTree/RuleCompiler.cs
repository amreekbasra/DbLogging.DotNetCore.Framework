using Newtonsoft.Json.Serialization;
using RuleEngine.ExpressionTree.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RuleEngine.ExpressionTree
{
    public class RuleCompiler
    {
        ///
        /// A method used to precompile rules for a provided type
        /// 
        public static List<Func<T, bool>> CompileRule<T>(List<T> targetEntity, List<Rule> rules)
        {
            var compiledRules = new List<Func<T, bool>>();

            // Loop through the rules and compile them against the properties of the supplied shallow object 
            rules.ForEach(rule =>
            {
                var genericType = Expression.Parameter(typeof(T));
                var key = MemberExpression.Property(genericType, rule.ComparisonPredicate);
                var propertyType = typeof(T).GetProperty(rule.ComparisonPredicate).PropertyType;
                var value = Expression.Constant(Convert.ChangeType(rule.ComparisonValue, propertyType));
                var binaryExpression = Expression.MakeBinary(rule.Comparator, key, value);

                compiledRules.Add(Expression.Lambda<Func<T,bool>>(binaryExpression, genericType).Compile());
            });

            // Return the compiled rules to the caller
            return compiledRules;
        }
    }
}
