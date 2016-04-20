using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Suflow.Common.Utils {
    public class ExpressionFinder : ExpressionVisitor {

        private List<ParameterExpression> _foundParameterExpressions;

        #region Override

        protected override Expression VisitParameter(ParameterExpression node) {
            if (!_foundParameterExpressions.Contains(node))
                _foundParameterExpressions.Add(node);
            return node;
        }

        #endregion

        #region Public methods

        public List<ParameterExpression> Find(Expression expression) {
            _foundParameterExpressions = new List<ParameterExpression>();
            Visit(expression);
            return _foundParameterExpressions;
        }

        public List<ParameterExpression> Find(ReadOnlyCollection<Expression> expressions) {
            _foundParameterExpressions = new List<ParameterExpression>();
            foreach (var expression in expressions) {
                Visit(expression);
            }
            return _foundParameterExpressions;
        }

        #endregion
    }
}
