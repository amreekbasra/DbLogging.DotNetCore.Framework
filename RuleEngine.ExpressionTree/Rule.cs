using System;
using System.Linq.Expressions;

namespace RuleEngine.ExpressionTree.Model
{
    public class Rule
    {

        ///
        /// Denotes the rules predictate (e.g. Name); comparison operator
        /// (e.g. ExpressionType.GreaterThan); value (e.g. "Cole")
        /// 
        public string ComparisonPredicate { get; set; }
        public ExpressionType Comparator { get; set; }
        public string ComparisonValue { get; set; }
        public Rule(string comparison, string value, ExpressionType expression)
        {
            Comparator = expression;
            ComparisonPredicate = comparison;
            ComparisonValue = value;
        }
    }
}
