using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Impl;
using Roottech.Tracking.Library.Models.Grid;
using Expression = System.Linq.Expressions.Expression;

namespace Roottech.Tracking.Library.Models.Helpers
{
    public static class LinqExtensions
    {
        #region Searching

        static bool GetAbstractCriterion(Rule rule, ParameterExpression parameter, out AbstractCriterion criterion)
        {
            criterion = null;
            if (string.IsNullOrEmpty(rule.field)) return true;

            MemberExpression memberAccess =
                rule.field.Split('.').Aggregate<string, MemberExpression>(null,
                (current, property) => Expression.Property(current ?? (parameter as Expression), property));
            var memberAccessName = memberAccess.Member.Name;
            var mAccess = memberAccess.ToString().Split('.');
            if (mAccess.Length > 2)
                memberAccessName = rule.field.Split('.')[0] + "." + memberAccess.Member.Name; //ExpressionProcessor.FindMemberExpression(memberAccess.Expression)

            if (GetAbstractCriterionByMember(rule, out criterion, memberAccess.Type, memberAccessName)) return true;
            return false;
        }

        public static AbstractCriterion GetAbstractCriterionByAliases<T>(Rule rule, string groupOp,
            AbstractCriterion resultCriterion, Type memberAccessType, Expression<Func<T>> alias, string propertyName)
        {
            if (string.IsNullOrEmpty(rule.field)) return null;

            AbstractCriterion criterion;
            Filter filter = new Filter {rules = new[] {rule}, groupOp = groupOp};

            if (GetAbstractCriterionByMember(rule, out criterion, memberAccessType,
                BuildPropertyAccess(alias, propertyName))) return null; //continue;

            return GetCombinedAbstractCriterion(filter, resultCriterion, criterion);
        }

        static string BuildPropertyAccess<T>(Expression<Func<T>> alias, string propertyName)
        {
            string aliasName = ExpressionProcessor.FindMemberExpression(alias.Body);

            return string.Format("{0}.{1}", aliasName, propertyName);
        }

        static bool GetAbstractCriterionByMember(Rule rule, out AbstractCriterion criterion,
            Type memberAccessType, string memberAccessName)
        {
            criterion = null;
            // Change the type of the parameter 'value'. it is necessary for comparisons (specially for booleans)
            var safeValue = (string.IsNullOrEmpty(rule.data))
                                ? null
                                : Convert.ChangeType(rule.data, Nullable.GetUnderlyingType(memberAccessType)
                                                                ?? memberAccessType);

            switch ((WhereOperation) StringEnum.Parse(typeof (WhereOperation), rule.op))
            {
                case WhereOperation.Equal:
                    //Expression<Func<T, bool>> expression = i => i == "abc";
                    criterion = (memberAccessType != typeof(string))
                        ? Restrictions.Eq(memberAccessName, safeValue)
                        : Restrictions.IsNotNull(memberAccessName)
                          && Restrictions.Eq(memberAccessName, safeValue);
                    break;
                case WhereOperation.NotEqual:
                    criterion = (memberAccessType != typeof (string))
                        ? Restrictions.Not(Restrictions.Eq(memberAccessName, safeValue))
                        : Restrictions.IsNotNull(memberAccessName)
                          && Restrictions.Not(Restrictions.Eq(memberAccessName, safeValue));
                    break;
                case WhereOperation.Greater:
                    criterion = Restrictions.Gt(memberAccessName, safeValue);
                    break;
                case WhereOperation.GreaterOrEqual:
                    criterion = Restrictions.Ge(memberAccessName, safeValue);
                    break;
                case WhereOperation.Less:
                    criterion = Restrictions.Lt(memberAccessName, safeValue);
                    break;
                case WhereOperation.LessEqual:
                    criterion = Restrictions.Le(memberAccessName, safeValue);
                    break;
                case WhereOperation.Contains:
                    criterion = Restrictions.IsNotNull(memberAccessName) &&
                                Restrictions.InsensitiveLike(memberAccessName, safeValue.ToString(), MatchMode.Anywhere);
                    break;
                case WhereOperation.NotContains:
                    criterion = Restrictions.IsNotNull(memberAccessName) &&
                                Restrictions.Not(Restrictions.InsensitiveLike(memberAccessName, safeValue.ToString(),
                                    MatchMode.Anywhere));
                    break;
                case WhereOperation.BeginsWith:
                    criterion = Restrictions.IsNotNull(memberAccessName) &&
                                Restrictions.InsensitiveLike(memberAccessName, safeValue.ToString(), MatchMode.Start);
                    break;
                case WhereOperation.NotBeginsWith:
                    criterion = Restrictions.IsNotNull(memberAccessName) &&
                                Restrictions.Not(Restrictions.InsensitiveLike(memberAccessName, safeValue.ToString(),
                                    MatchMode.Start));
                    break;
                case WhereOperation.EndsWith:
                    criterion = Restrictions.IsNotNull(memberAccessName) &&
                                Restrictions.InsensitiveLike(memberAccessName, safeValue.ToString(), MatchMode.End);
                    break;
                case WhereOperation.NotEndsWith:
                    criterion = Restrictions.IsNotNull(memberAccessName) &&
                                Restrictions.Not(Restrictions.InsensitiveLike(memberAccessName, safeValue.ToString(),
                                    MatchMode.End));
                    break;
                case WhereOperation.Null:
                    criterion = Restrictions.IsNull(memberAccessName);
                    break;
                case WhereOperation.NotNull:
                    criterion = Restrictions.IsNotNull(memberAccessName);
                    break;
                default:
                    return true;
            }
            return false;
        }

        static AbstractCriterion GetGroupAbstractCriterion(Filter filter, ParameterExpression parameter)
        {
            AbstractCriterion resultCriterion = null;
            foreach (var rule in filter.rules)
            {
                AbstractCriterion criterion;
                if (GetAbstractCriterion(rule, parameter, out criterion)) continue;
                resultCriterion = GetCombinedAbstractCriterion(filter, resultCriterion, criterion);
            }
            if (filter.groups != null)
                if (filter.groups.Any())
                    foreach (var group in filter.groups)
                        resultCriterion = GetCombinedAbstractCriterion(filter, resultCriterion, GetGroupAbstractCriterion(group, parameter));
            return resultCriterion;
        }

        static AbstractCriterion GetCombinedAbstractCriterion(Filter filter, AbstractCriterion resultCriterion, AbstractCriterion criterion)
        {
            return resultCriterion != null
                       ? ((filter.groupOp == "AND")
                              ? resultCriterion && criterion
                              : resultCriterion || criterion)
                       : criterion;
        }

        static IQueryOver<T, T> Where<T>(this IQueryOver<T, T> pQueryOver, Filter filter)
        {
            if (!filter.rules.Any()) return pQueryOver;
            // Create a member expression pointing to given column
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            var groupAbstractCriterion = GetGroupAbstractCriterion(filter, parameter);
            if (groupAbstractCriterion == null) return pQueryOver;
            return pQueryOver.Where(groupAbstractCriterion);
        }

        #endregion

        #region Sorting
        static IQueryOver<T, T> Sorting<T>(this IQueryOver<T, T> query, GridSettings grid) where T : class
        {
            if (!string.IsNullOrEmpty(grid.SortColumn))
            {
                if (grid.SortColumn.IndexOf(",") > -1) //multiple orderby but its overwriting previous one not working.
                {
                    var sortColumns = grid.SortColumn.Trim().Replace(", ", ",").Split(',');
                    IQueryOver<T, T> orderedQueryable = null;
                    for (var i = 0; i < sortColumns.Length; i++)
                    {
                        var sortColumn = sortColumns[i].Split(' ');
                        if (i == 0)
                            orderedQueryable = (sortColumn.Length > 1 ? sortColumn[1] : grid.SortOrder) == "asc"
                                                   ? query.OrderBy(Projections.Property(sortColumn[0])).Asc
                                                   : query.OrderBy(Projections.Property(sortColumn[0])).Desc;
                        else
                            orderedQueryable = (sortColumn.Length > 1 ? sortColumn[1] : grid.SortOrder) == "asc"
                                                   ? orderedQueryable.ThenBy(Projections.Property(sortColumn[0])).Asc
                                                   : orderedQueryable.ThenBy(Projections.Property(sortColumn[0])).Desc;
                    }
                    query = orderedQueryable;
                }
                else
                    query = grid.SortOrder == "asc" ? query.OrderBy(Projections.Property(grid.SortColumn)).Asc : query.OrderBy(Projections.Property(grid.SortColumn)).Desc;
            }
            return query;
        }
        #endregion

        #region Method

        public static IList<T> SearchGrid<T>(this IQueryOver<T, T> queryOver, GridSettings grid, out int totalRecords) where T : class
        {
            IQueryOver<T, T> queryOverCount = (grid.IsSearch)
                                                  ? queryOver.Where(grid.Where).Clone()
                                                  : queryOver.Clone();

            var futureCount = queryOverCount.Select(Projections.RowCount()).FutureValue<int>();

            //sorting
            queryOver = queryOver.Sorting(grid);

            //paging
            IEnumerable<T> queryOverFuture = grid.PageSize == 0 ? queryOver.Future() : queryOver.Skip((grid.PageIndex - 1) * grid.PageSize).Take(grid.PageSize).Future();

            //records
            totalRecords = futureCount.Value;

            return queryOverFuture.ToList();
        }

        /// <summary>Orders the sequence by specific column and direction.</summary>
        /// <param name="query">The query.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="ascending">if set to true [ascending].</param>
        /*        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string sortColumn, string direction)
                {
                    string methodName = string.Format("OrderBy{0}",
                        direction.ToLower() == "asc" ? "" : "descending");

                    ParameterExpression parameter = Expression.Parameter(query.ElementType, "p");

                    MemberExpression memberAccess = null;
                    foreach (var property in sortColumn.Split('.'))
                        memberAccess = MemberExpression.Property
                           (memberAccess ?? (parameter as Expression), property);

                    LambdaExpression orderByLambda = Expression.Lambda(memberAccess, parameter);

                    MethodCallExpression result = Expression.Call(
                              typeof(Queryable),
                              methodName,
                              new[] { query.ElementType, memberAccess.Type },
                              query.Expression,
                              Expression.Quote(orderByLambda));

                    return query.Provider.CreateQuery<T>(result);
                }


                public static IQueryable<T> Where<T>(this IQueryable<T> query,
                    string column, object value, WhereOperation operation)
                {
                    if (string.IsNullOrEmpty(column))
                        return query;

                    ParameterExpression parameter = Expression.Parameter(query.ElementType, "p");

                    MemberExpression memberAccess = null;
                    foreach (var property in column.Split('.'))
                        memberAccess = MemberExpression.Property
                           (memberAccess ?? (parameter as Expression), property);

                    //change param value type
                    //necessary to getting bool from string
                    ConstantExpression filter = Expression.Constant
                        (
                            Convert.ChangeType(value, memberAccess.Type)
                        );

                    //switch operation
                    Expression condition = null;
                    LambdaExpression lambda = null;
                    switch (operation)
                    {
                        //equal ==
                        case WhereOperation.Equal:
                            condition = Expression.Equal(memberAccess, filter);
                            lambda = Expression.Lambda(condition, parameter);
                            break;
                        //not equal !=
                        case WhereOperation.NotEqual:
                            condition = Expression.NotEqual(memberAccess, filter);
                            lambda = Expression.Lambda(condition, parameter);
                            break;
                        //string.Contains()
                        case WhereOperation.Contains:
                            condition = Expression.Call(memberAccess,
                                typeof(string).GetMethod("Contains"),
                                Expression.Constant(value));
                            lambda = Expression.Lambda(condition, parameter);
                            break;
                    }


                    MethodCallExpression result = Expression.Call(
                           typeof(Queryable), "Where",
                           new[] { query.ElementType },
                           query.Expression,
                           lambda);

                    return query.Provider.CreateQuery<T>(result);
                }

                public static IQueryable<T> Where<T>(this IQueryable<T> pQuery, SearchCriteria[] pCriterias, GroupOperator pGroupOp, bool pCaseSensitive)
                {
                    if (pCriterias.Count() == 0)
                        return pQuery;

                    LambdaExpression lambda;
                    Expression resultCondition = null;

                    // Create a member expression pointing to given column
                    ParameterExpression parameter = Expression.Parameter(pQuery.ElementType, "p");

                    foreach (var searchCriteria in pCriterias)
                    {
                        if (string.IsNullOrEmpty(searchCriteria.Column))
                            continue;

                        MemberExpression memberAccess = null;
                        foreach (var property in searchCriteria.Column.Split('.'))
                            memberAccess = MemberExpression.Property
                                (memberAccess ?? (parameter as Expression), property);

                        // Change the type of the parameter 'value'. it is necessary for comparisons (specially for booleans)
                        ConstantExpression filter = Expression.Constant
                        (
                            Convert.ChangeType(searchCriteria.Value, memberAccess.Type)
                        );

                        //switch operation
                        Expression condition = null;
                        switch (searchCriteria.Operation)
                        {
                            //equal ==
                            case WhereOperation.Equal:
                                if (pCaseSensitive || searchCriteria.CaseSensitive)
                                {
                                    //CaseSensitive
                                    condition = Expression.Equal(memberAccess, filter);
                                }
                                else
                                {
                                    //Case InSensitive
                                    Expression toLower = Expression.Call(memberAccess, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                                    condition = Expression.Equal(toLower, Expression.Constant(searchCriteria.Value.ToString().ToLower()));
                                }
                                break;

                            //not equal !=
                            case WhereOperation.NotEqual:
                                if (pCaseSensitive || searchCriteria.CaseSensitive)
                                {
                                    //CaseSensitive
                                    condition = Expression.NotEqual(memberAccess, filter);
                                }
                                else
                                {
                                    //Case InSensitive
                                    Expression toLower = Expression.Call(memberAccess, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                                    condition = Expression.NotEqual(toLower, Expression.Constant(searchCriteria.Value.ToString().ToLower()));
                                }
                                break;

                            // Greater
                            case WhereOperation.Greater:
                                if (pCaseSensitive || searchCriteria.CaseSensitive)
                                {
                                    //CaseSensitive
                                    condition = Expression.GreaterThan(memberAccess, filter);
                                }
                                else
                                {
                                    //Case InSensitive
                                    Expression toLower = Expression.Call(memberAccess, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                                    condition = Expression.GreaterThan(toLower, Expression.Constant(searchCriteria.Value.ToString().ToLower()));
                                }
                                break;

                            // Greater or equal
                            case WhereOperation.GreaterOrEqual:
                                if (pCaseSensitive || searchCriteria.CaseSensitive)
                                {
                                    //CaseSensitive
                                    condition = Expression.GreaterThanOrEqual(memberAccess, filter);
                                }
                                else
                                {
                                    //Case InSensitive
                                    Expression toLower = Expression.Call(memberAccess, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                                    condition = Expression.GreaterThanOrEqual(toLower, Expression.Constant(searchCriteria.Value.ToString().ToLower()));
                                }
                                break;

                            // Less
                            case WhereOperation.Less:
                                if (pCaseSensitive || searchCriteria.CaseSensitive)
                                {
                                    //CaseSensitive
                                    condition = Expression.LessThan(memberAccess, filter);
                                }
                                else
                                {
                                    //Case InSensitive
                                    Expression toLower = Expression.Call(memberAccess, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                                    condition = Expression.LessThan(toLower, Expression.Constant(searchCriteria.Value.ToString().ToLower()));
                                }
                                break;

                            // Less or equal
                            case WhereOperation.LessEqual:
                                if (pCaseSensitive || searchCriteria.CaseSensitive)
                                {
                                    //CaseSensitive
                                    condition = Expression.LessThanOrEqual(memberAccess, filter);
                                }
                                else
                                {
                                    //Case InSensitive
                                    Expression toLower = Expression.Call(memberAccess, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                                    condition = Expression.LessThanOrEqual(toLower, Expression.Constant(searchCriteria.Value.ToString().ToLower()));
                                }
                                break;

                            //string.Contains()
                            case WhereOperation.Contains:
                                if (pCaseSensitive || searchCriteria.CaseSensitive)
                                {
                                    //CaseSensitive
                                    condition = Expression.Call(memberAccess,
                                                                typeof(string).GetMethod("Contains"),
                                                                Expression.Constant(searchCriteria.Value));
                                }
                                else
                                {
                                    //Case InSensitive
                                    Expression toLower = Expression.Call(memberAccess, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                                    condition = Expression.Call(toLower,
                                                                typeof(string).GetMethod("Contains"),
                                                                Expression.Constant(searchCriteria.Value.ToString().ToLower()));
                                }

                                break;

                            default:
                                continue;
                        }


                        if (pGroupOp == GroupOperator.AND)
                            resultCondition = resultCondition != null ? Expression.And(resultCondition, condition) : condition;
                        else
                            resultCondition = resultCondition != null ? Expression.Or(resultCondition, condition) : condition;
                    }

                    lambda = Expression.Lambda(resultCondition, parameter);

                    MethodCallExpression result = Expression.Call(
                               typeof(Queryable), "Where",
                               new[] { pQuery.ElementType },
                               pQuery.Expression,
                               lambda);

                    return pQuery.Provider.CreateQuery<T>(result);
                }

                public static T[] SearchGrid<T>(this IQueryable<T> query, GridSettings grid, out int totalRecords) where T : class
                {
                    if (grid.IsSearch)
                    {
                        StringBuilder sb = new StringBuilder();
                        int i = 0;
                        foreach (var rule in grid.Where.rules)
                        {
                            string op = null;

                            switch (rule.op)
                            {
                                case "eq":
                                    op = rule.field + "={0}";
                                    break;
                                case "ne":
                                    op = rule.field + "!={0}";
                                    break;
                                case "lt":
                                    op = rule.field + "<{0}";
                                    break;
                                case "le":
                                    op = rule.field + "<={0}";
                                    break;
                                case "gt":
                                    op = rule.field + ">{0}";
                                    break;
                                case "ge":
                                    op = rule.field + ">={0}";
                                    break;
                                case "bw":
                                    op = rule.field + ".StartsWith({0})";
                                    break;
                                case "bn":
                                    op = "!" + rule.field + ".StartsWith({0})";
                                    break;
                                case "in":
                                    break;
                                case "ni":
                                    break;
                                case "ew":
                                    op = rule.field + ".EndsWith({0})";
                                    break;
                                case "en":
                                    op = "!" + rule.field + ".EndsWith({0})";
                                    break;
                                case "cn":
                                    op = rule.field + ".Contains({0})";
                                    break;
                                case "nc":
                                    op = "!" + rule.field + ".Contains({0})";
                                    break;
                                case "nu":
                                    op = rule.field + "==null";
                                    break;
                                case "nn":
                                    op = rule.field + "!=null";
                                    break;
                            }

                            if (op == null)
                                throw new NotSupportedException("rule.op=" + rule.op);

                            op = string.Format(op, "@" + i);

                            sb.Append(op);
                            if (rule != grid.Where.rules.Last())
                            {
                                sb.Append(grid.Where.groupOp == "AND" ? "&&" : "||");
                            }
                            i++;
                        }

                        var predicate = sb.ToString();
                        var values = grid.Where.rules.Select(r => r.data).ToArray();
                        query = query.Where<T>(predicate, values);
                    }

                    //records
                    totalRecords = query.Count();

                    //sorting
                    query = query.OrderBy<T>(grid.SortColumn, grid.SortOrder);

                    //paging
                    var data = query.Skip((grid.PageIndex - 1) * grid.PageSize).Take(grid.PageSize).ToArray();

                    return data;
                }*/

        #region Searching

        static bool IsNullableType(Type t)
        {
            return typeof(string) == t || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        static bool GetCondition(bool pCaseSensitive, Rule rule, ParameterExpression parameter, out Expression condition)
        {
            condition = null;
            if (string.IsNullOrEmpty(rule.field)) return true;

            MemberExpression memberAccess = null;
            foreach (var property in rule.field.Split('.'))
                memberAccess = Expression.Property(memberAccess ?? (parameter as Expression), property);

            // Change the type of the parameter 'value'. it is necessary for comparisons (specially for booleans)
            var safeValue = (string.IsNullOrEmpty(rule.data)) ? null
                                               : Convert.ChangeType(rule.data, Nullable.GetUnderlyingType(memberAccess.Type)
                                                ?? memberAccess.Type);
            Expression e1 = memberAccess;
            Expression e2 = Expression.Constant(safeValue);
            if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
                e2 = Expression.Convert(e2, e1.Type);
            else if (!IsNullableType(e1.Type) && (IsNullableType(e2.Type) || safeValue == null))
                e1 = Expression.Convert(e1, e2.Type);

            var methodCallExpression = new[]
                              {
                                  e2, Expression.Constant(pCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)
                              };
            var notNullExpression = IsNullableType(memberAccess.Type) ? (Expression)Expression.Not(Expression.Equal(memberAccess, Expression.Constant(null))) : Expression.Constant(true); //Expression.Empty();

            var stringComparisionTypes = new[] { typeof(string), typeof(StringComparison) };

            switch ((WhereOperation)StringEnum.Parse(typeof(WhereOperation), rule.op))
            {
                case WhereOperation.Equal:
                    condition = pCaseSensitive || (memberAccess.Type != typeof(string)) ? Expression.Equal(e1, e2) :
                        Expression.AndAlso(notNullExpression,
                        Expression.Call(memberAccess, typeof(string).GetMethod("Equals", stringComparisionTypes), methodCallExpression));//Expression.Equal(toLowerLeft, toLowerRight);
                    break;
                case WhereOperation.NotEqual:
                    condition = pCaseSensitive || (memberAccess.Type != typeof(string)) ? Expression.NotEqual(e1, e2) :
                        Expression.AndAlso(notNullExpression,
                        Expression.Not(Expression.Call(memberAccess, typeof(string).GetMethod("Equals", stringComparisionTypes), methodCallExpression)));//Expression.NotEqual(toLowerLeft, toLowerRight);
                    break;
                case WhereOperation.Greater:
                    condition = Expression.GreaterThan(e1, e2);
                    break;
                case WhereOperation.GreaterOrEqual:
                    condition = Expression.GreaterThanOrEqual(e1, e2);
                    break;
                case WhereOperation.Less:
                    condition = Expression.LessThan(e1, e2);
                    break;
                case WhereOperation.LessEqual:
                    condition = Expression.LessThanOrEqual(e1, e2);
                    break;
                case WhereOperation.Contains:
                    condition = Expression.AndAlso(notNullExpression,
                        Expression.NotEqual(Expression.Call(memberAccess, typeof(string).GetMethod("IndexOf", stringComparisionTypes), methodCallExpression), Expression.Constant(-1)));
                    break;
                case WhereOperation.NotContains:
                    condition = Expression.AndAlso(notNullExpression,
                        Expression.Equal(Expression.Call(memberAccess, typeof(string).GetMethod("IndexOf", stringComparisionTypes), methodCallExpression), Expression.Constant(-1)));
                    break;
                case WhereOperation.BeginsWith:
                    condition = Expression.AndAlso(notNullExpression,
                        Expression.Call(memberAccess, typeof(string).GetMethod("StartsWith", stringComparisionTypes), methodCallExpression));
                    break;
                case WhereOperation.NotBeginsWith:
                    condition = Expression.AndAlso(notNullExpression,
                        Expression.Not(Expression.Call(memberAccess, typeof(string).GetMethod("StartsWith", stringComparisionTypes), methodCallExpression)));
                    break;
                case WhereOperation.EndsWith:
                    condition = Expression.AndAlso(notNullExpression,
                        Expression.Call(memberAccess, typeof(string).GetMethod("EndsWith", stringComparisionTypes), methodCallExpression));
                    break;
                case WhereOperation.NotEndsWith:
                    condition = Expression.AndAlso(notNullExpression,
                        Expression.Not(Expression.Call(memberAccess, typeof(string).GetMethod("EndsWith", stringComparisionTypes), methodCallExpression)));
                    break;
                case WhereOperation.Null:
                    condition = Expression.Equal(e1, e2);
                    break;
                case WhereOperation.NotNull:
                    condition = Expression.Not(Expression.Equal(e1, e2));
                    break;
                default:
                    return true;
            }
            return false;
        }

        static Expression GetExpression(Filter filter, bool pCaseSensitive, ParameterExpression parameter)
        {
            Expression resultCondition = null;
            Expression condition;
            foreach (var rule in filter.rules)
            {
                if (GetCondition(pCaseSensitive, rule, parameter, out condition)) continue;
                resultCondition = resultCondition != null ?
                    ((filter.groupOp == "AND") ?
                        Expression.And(resultCondition, condition) :
                        Expression.Or(resultCondition, condition)) :
                    condition;
            }
            if (filter.groups != null)
                if (filter.groups.Any())
                {
                    foreach (var group in filter.groups)
                    {
                        condition = GetExpression(group, pCaseSensitive, parameter);

                        resultCondition = resultCondition != null ?
                            ((filter.groupOp == "AND") ?
                                Expression.And(resultCondition, condition) :
                                Expression.Or(resultCondition, condition)) :
                            condition;
                    }
                }
            return resultCondition;
        }

        static IQueryable<T> Where<T>(this IQueryable<T> pQuery, Filter filter, bool pCaseSensitive)
        {
            if (!filter.rules.Any())// pCriterias.Any())
                return pQuery;

            // Create a member expression pointing to given column
            ParameterExpression parameter = Expression.Parameter(pQuery.ElementType, "p");

            var lambda = Expression.Lambda(GetExpression(filter, false, parameter), parameter);
            MethodCallExpression result = Expression.Call(
                       typeof(Queryable), "Where", new[] { pQuery.ElementType }, pQuery.Expression, lambda);

            return pQuery.Provider.CreateQuery<T>(result);
        }
        #endregion

        #region Sorting
        static IQueryable<T> Sorting<T>(IQueryable<T> query, GridSettings grid) where T : class
        {
            if (!string.IsNullOrEmpty(grid.SortColumn))
            {
                if (grid.SortColumn.IndexOf(",") > -1) //multiple orderby but its overwriting previous one not working.
                {
                    var sortColumns = grid.SortColumn.Trim().Replace(", ", ",").Split(',');
                    IOrderedQueryable<T> orderedQueryable = null;
                    for (var i = 0; i < sortColumns.Length; i++)
                    {
                        var sortColumn = sortColumns[i].Split(' ');
                        if (i == 0)
                            orderedQueryable = (sortColumn.Length > 1 ? sortColumn[1] : grid.SortOrder) == "asc"
                                                   ? query.OrderBy(sortColumn[0])
                                                   : query.OrderByDescending(sortColumn[0]);
                        else
                            orderedQueryable = (sortColumn.Length > 1 ? sortColumn[1] : grid.SortOrder) == "asc"
                                                   ? orderedQueryable.ThenBy(sortColumn[0])
                                                   : orderedQueryable.ThenByDescending(sortColumn[0]);
                    }
                    query = orderedQueryable;
                }
                else
                    query = grid.SortOrder == "asc" ? query.OrderBy(grid.SortColumn) : query.OrderByDescending(grid.SortColumn);
            }
            return query;
        }

        static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), string.Empty); // I don't care about some naming
            MemberExpression property = Expression.PropertyOrField(param, propertyName);
            LambdaExpression sort = Expression.Lambda(property, param);

            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }

        static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, false);
        }

        static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, false);
        }

        static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, true);
        }

        static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, true);
        }
        #endregion
        public static IQueryable<T> SearchGrid<T>(this IQueryable<T> query, GridSettings grid, out int totalRecords) where T : class
        {
            //filtering
            if (grid.IsSearch) query = query.Where(grid.Where, false);

            //records
            totalRecords = query.Count();

            //sorting
            query = Sorting(query, grid);

            //paging
            var data = query.Skip((grid.PageIndex - 1) * grid.PageSize).Take(grid.PageSize).ToArray();

            return data.AsQueryable();
        }
        #endregion
    }
}