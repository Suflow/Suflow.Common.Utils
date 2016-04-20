using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Suflow.Common.Utils {
    /// <summary>
    /// Use dictionary to save Set of lambda expression that do pre calculation
    /// Use expression tree visitor to traverse an existing expression tree and update ExpressTree
    /// Reference: http://msdn.microsoft.com/en-us/library/bb546136(v=vs.90).aspx
    /// </summary>
    public class ExpressionOptimizer : ExpressionVisitor {
        private object func;
        private Dictionary<string, MethodCallExpression> _setOfLambdaExpressionThatDoPreCalculation;
        private Dictionary<string, ParameterExpression> _setOfParameterExpresssionToReplaceMethodCalls; //Same ParameterExpression should be used on Body and Parameters of lambda expression.

        private static object GetParameterValue(ParameterExpression parameterExpression, List<ParameterExpression> parameterExpressionList, object[] parameterValueArray) {
            for (var i = 0; i < parameterValueArray.Length; i++) {
                if (parameterExpressionList[i].Name == parameterExpression.Name) {
                    return parameterValueArray[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Helper method to ensure that, when we have (x, y) => f(x) + f(y) + f(2*x) 
        /// Each function gets its parameter correctly. This function will calculate the argument that needs to be sent to the function.
        /// <summary>
        private object[] FindParametersForCallExpression(List<ParameterExpression> parameters, MethodCallExpression methodCallExpression, object[] lambdaParameters) {
            var result = new List<object>();

            foreach (var argument in methodCallExpression.Arguments) {
                if (argument.NodeType == ExpressionType.Parameter) {
                    result.Add(GetParameterValue((ParameterExpression)argument, parameters, lambdaParameters));
                }
                else {
                    // This is not parameter type, we can't get its value directly from lambdaParameters, we have to compile and execute this argument to get the value
                    var parameterExpressionFinder = new ExpressionFinder();
                    var parametersThisArgumentRequired = parameterExpressionFinder.Find(argument); //lets find all ParameterExpression that this Expression required then later use it to create lambda expression
                    var argumentLambda = Expression.Lambda(argument, parametersThisArgumentRequired); //lets create lambda of this Expression so that we can compile and later execute
                    var compiled = argumentLambda.Compile();

                    //already compiled, now lets get parameter value and call  DynamicInvoke
                    var parametersValueThisArgumentShouldReceive = new List<object>();
                    foreach (var parameter in parametersThisArgumentRequired) {
                        parametersValueThisArgumentShouldReceive.Add(GetParameterValue(parameter, parameters, lambdaParameters));
                    }
                    var resultOfExucutingThisExpression = compiled.DynamicInvoke(parametersValueThisArgumentShouldReceive.ToArray());
                    result.Add(resultOfExucutingThisExpression);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Precalculate lambda expression that we collected on _setOfLambdaExpressionThatDoPreCalculation
        /// Run the optimized lambda expression
        /// </summary>
        private object CalculateLambda(LambdaExpression optimized, object[] lambdaParameters) {
            //PRECALCULATION
            var finalLambdaParametersValues = new List<object>(lambdaParameters);
            foreach (var key in _setOfLambdaExpressionThatDoPreCalculation.Keys) {
                var callExpression = _setOfLambdaExpressionThatDoPreCalculation[key];
                var originalParameters = optimized.Parameters.Take(lambdaParameters.Count()).ToList();
                var parametersForCallExpression = FindParametersForCallExpression(originalParameters, callExpression, lambdaParameters);
                finalLambdaParametersValues.Add(callExpression.Method.Invoke(callExpression.Object, parametersForCallExpression));
            }

            //FINAL CALCULATION
            var parameters = finalLambdaParametersValues.ToArray();
            var compiled = optimized.Compile();
            var result = compiled.DynamicInvoke(parameters);
            return result;
        }



        /// <summary>
        /// Helper method to generate key for function and argument combination 
        /// eg: F(x) will get a key of p24755x
        /// NOTE: The key generated for f(2*y) will be diferent to key generated for f(y*2)
        /// </summary>
        private static string GetMethodCallExpressionKey(MethodCallExpression node) {
            var methodKey = node.Method.GetHashCode();
            var argKey = new StringBuilder();
            foreach (var arg in node.Arguments) {
                argKey.Append(arg);
            }
            return "p" + methodKey + argKey.ToString();
        }



        /// <summary>
        /// Method responsible for updating the _setOfLambdaExpressionThatDoPreCalculation
        /// and it is also responsible for converting MethodCallExpression to ParameterExpression for optimization
        /// NOTE: Will optimize even if there is single function call in the lambda expression(not good)
        /// </summary>
        protected override Expression VisitMethodCall(MethodCallExpression node) {

            var key = GetMethodCallExpressionKey(node);

            if (_setOfLambdaExpressionThatDoPreCalculation.ContainsKey(key)) {
                //already added to dictionary
                return _setOfParameterExpresssionToReplaceMethodCalls[key];
            }
            else {
                //Replace MethodCallExpression with ParameterExpression and save MethodCallExpression for precalculation
                var parameterExpression = Expression.Parameter(node.Method.ReturnType, key);
                _setOfLambdaExpressionThatDoPreCalculation.Add(key, node);
                _setOfParameterExpresssionToReplaceMethodCalls.Add(key, parameterExpression);
                return parameterExpression;
            }
        }



        /// <summary>
        /// This method will call VisitMethodCall() which will optimize the body of the lambda expression and
        /// also update _setOfLambdaExpressionThatDoPreCalculation
        /// Then it will call GetNewParameterExpression() to update paramters for new optimized lambda expression
        /// </summary>
        private LambdaExpression OptimizeLambda(LambdaExpression lambdaExpression, object functionF) {
            func = functionF;
            _setOfLambdaExpressionThatDoPreCalculation = new Dictionary<string, MethodCallExpression>();
            _setOfParameterExpresssionToReplaceMethodCalls = new Dictionary<string, ParameterExpression>();

            var optimizedBodyExpression = Visit(lambdaExpression.Body);

            if (_setOfLambdaExpressionThatDoPreCalculation.Count == 0)
                return lambdaExpression;//nothing was optimized

            //combines old parameters (from lambdaExpression) with new (from _setOfLambdaExpressionThatDoPreCalculation)
            var parameters = lambdaExpression.Parameters.ToList();
            parameters.AddRange(_setOfParameterExpresssionToReplaceMethodCalls.Values);

            return Expression.Lambda(optimizedBodyExpression, false, parameters);
        }

        /// <summary>
        /// Given function. This algorith will try to optimize all functions that it receives hence functionF has not been utilized.
        /// One use of functionF can be: use it as a filter while deciding wheather we want to optimize this function or not.
        /// </summary>
        public object OptimizedCalculation<TFunc>(LambdaExpression lambdaExpression, object[] lambdaParameters, TFunc functionF) {
            var optimized = OptimizeLambda(lambdaExpression, functionF);
            var result = CalculateLambda(optimized, lambdaParameters);
            return result;
        }
    }
}
