namespace TimeTracker.Core.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class TableQuery<T> : IEnumerable<T> where T : new()
    {
        public SQLiteConnection Connection { get; private set; }

        public TableMapping Table { get; private set; }

        Expression _where;
        List<Ordering> _orderBys;
        int? _limit;
        int? _offset;

        class Ordering
        {
            public string ColumnName { get; set; }

            public bool Ascending { get; set; }
        }

        TableQuery(SQLiteConnection conn, TableMapping table)
        {
            this.Connection = conn;
            this.Table = table;
        }

        public TableQuery(SQLiteConnection conn)
        {
            this.Connection = conn;
            this.Table = this.Connection.GetMapping(typeof(T));
        }

        public TableQuery<T> Clone()
        {
            var q = new TableQuery<T>(this.Connection, this.Table);
            q._where = this._where;
            if (this._orderBys != null)
            {
                q._orderBys = new List<Ordering>(this._orderBys);
            }
            q._limit = this._limit;
            q._offset = this._offset;
            return q;
        }

        public TableQuery<T> Where(Expression<Func<T, bool>> predExpr)
        {
            if (predExpr.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)predExpr;
                var pred = lambda.Body;
                var q = this.Clone();
                q.AddWhere(pred);
                return q;
            }
            else
            {
                throw new NotSupportedException("Must be a predicate");
            }
        }

        public TableQuery<T> Take(int n)
        {
            var q = this.Clone();
            q._limit = n;
            return q;
        }

        public TableQuery<T> Skip(int n)
        {
            var q = this.Clone();
            q._offset = n;
            return q;
        }

        public TableQuery<T> OrderBy<U>(Expression<Func<T, U>> orderExpr)
        {
            return this.AddOrderBy<U>(orderExpr, true);
        }

        public TableQuery<T> OrderByDescending<U>(Expression<Func<T, U>> orderExpr)
        {
            return this.AddOrderBy<U>(orderExpr, false);
        }

        private TableQuery<T> AddOrderBy<U>(Expression<Func<T, U>> orderExpr, bool asc)
        {
            if (orderExpr.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)orderExpr;
                var mem = lambda.Body as MemberExpression;
                if (mem != null && (mem.Expression.NodeType == ExpressionType.Parameter))
                {
                    var q = this.Clone();
                    if (q._orderBys == null)
                    {
                        q._orderBys = new List<Ordering>();
                    }
                    q._orderBys.Add(new Ordering
                                        {
                                            ColumnName = mem.Member.Name,
                                            Ascending = asc
                                        });
                    return q;
                }
                else
                {
                    throw new NotSupportedException("Order By does not support: " + orderExpr);
                }
            }
            else
            {
                throw new NotSupportedException("Must be a predicate");
            }
        }

        private void AddWhere(Expression pred)
        {
            if (this._where == null)
            {
                this._where = pred;
            }
            else
            {
                this._where = Expression.AndAlso(this._where, pred);
            }
        }

        private SQLiteCommand GenerateCommand(string selectionList)
        {
            var cmdText = "select " + selectionList + " from \"" + this.Table.TableName + "\"";
            var args = new List<object>();
            if (this._where != null)
            {
                var w = this.CompileExpr(this._where, args);
                cmdText += " where " + w.CommandText;
            }
            if ((this._orderBys != null) && (this._orderBys.Count > 0))
            {
                var t = string.Join(", ", this._orderBys.Select(o => "\"" + o.ColumnName + "\"" + (o.Ascending ? "" : " desc")).ToArray());
                cmdText += " order by " + t;
            }
            if (this._limit.HasValue)
            {
                cmdText += " limit " + this._limit.Value;
            }
            if (this._offset.HasValue)
            {
                if (!this._limit.HasValue)
                {
                    cmdText += " limit -1 ";
                }
                cmdText += " offset " + this._offset.Value;
            }
            return this.Connection.CreateCommand(cmdText, args.ToArray());
        }

        class CompileResult
        {
            public string CommandText { get; set; }

            public object Value { get; set; }
        }

        private CompileResult CompileExpr(Expression expr, List<object> queryArgs)
        {
            if (expr == null)
            {
                throw new NotSupportedException("Expression is NULL");
            }
            else if (expr is BinaryExpression)
            {
                var bin = (BinaryExpression)expr;

                var leftr = this.CompileExpr(bin.Left, queryArgs);
                var rightr = this.CompileExpr(bin.Right, queryArgs);

                //If either side is a parameter and is null, then handle the other side specially (for "is null"/"is not null")
                string text;
                if (leftr.CommandText == "?" && leftr.Value == null)
                    text = this.CompileNullBinaryExpression(bin, rightr);
                else if (rightr.CommandText == "?" && rightr.Value == null)
                    text = this.CompileNullBinaryExpression(bin, leftr);
                else
                    text = "(" + leftr.CommandText + " " + this.GetSqlName(bin) + " " + rightr.CommandText + ")";
                return new CompileResult { CommandText = text };
            }
            else if (expr.NodeType == ExpressionType.Call)
            {

                var call = (MethodCallExpression)expr;
                var args = new CompileResult[call.Arguments.Count];

                for (var i = 0; i < args.Length; i++)
                {
                    args[i] = this.CompileExpr(call.Arguments[i], queryArgs);
                }

                var sqlCall = "";

                if (call.Method.Name == "Like" && args.Length == 2)
                {
                    sqlCall = "(" + args[0].CommandText + " like " + args[1].CommandText + ")";
                }
                else if (call.Method.Name == "Contains" && args.Length == 2)
                {
                    sqlCall = "(" + args[1].CommandText + " in " + args[0].CommandText + ")";
                }
                else
                {
                    sqlCall = call.Method.Name.ToLower() + "(" + string.Join(",", args.Select(a => a.CommandText).ToArray()) + ")";
                }
                return new CompileResult { CommandText = sqlCall };

            }
            else if (expr.NodeType == ExpressionType.Constant)
            {
                var c = (ConstantExpression)expr;
                queryArgs.Add(c.Value);
                return new CompileResult
                           {
                               CommandText = "?",
                               Value = c.Value
                           };
            }
            else if (expr.NodeType == ExpressionType.Convert)
            {
                var u = (UnaryExpression)expr;
                var ty = u.Type;
                var valr = this.CompileExpr(u.Operand, queryArgs);
                return new CompileResult
                           {
                               CommandText = valr.CommandText,
                               Value = valr.Value != null ? Convert.ChangeType(valr.Value, ty) : null
                           };
            }
            else if (expr.NodeType == ExpressionType.MemberAccess)
            {
                var mem = (MemberExpression)expr;

                if (mem.Expression.NodeType == ExpressionType.Parameter)
                {
                    //
                    // This is a column of our table, output just the column name
                    //
                    return new CompileResult { CommandText = "\"" + mem.Member.Name + "\"" };
                }
                else
                {
                    object obj = null;
                    if (mem.Expression != null)
                    {
                        var r = this.CompileExpr(mem.Expression, queryArgs);
                        if (r.Value == null)
                        {
                            throw new NotSupportedException("Member access failed to compile expression");
                        }
                        if (r.CommandText == "?")
                        {
                            queryArgs.RemoveAt(queryArgs.Count - 1);
                        }
                        obj = r.Value;
                    }

                    //
                    // Get the member value
                    //
                    object val = null;

                    if (mem.Member.MemberType == MemberTypes.Property)
                    {
                        var m = (PropertyInfo)mem.Member;
                        val = m.GetValue(obj, null);
                    }
                    else if (mem.Member.MemberType == MemberTypes.Field)
                    {
                        var m = (FieldInfo)mem.Member;
                        val = m.GetValue(obj);
                    }
                    else
                    {
                        throw new NotSupportedException("MemberExpr: " + mem.Member.MemberType.ToString());
                    }

                    //
                    // Work special magic for enumerables
                    //
                    if (val != null && val is System.Collections.IEnumerable && !(val is string))
                    {
                        var sb = new System.Text.StringBuilder();
                        sb.Append("(");
                        var head = "";
                        foreach (var a in (System.Collections.IEnumerable)val)
                        {
                            queryArgs.Add(a);
                            sb.Append(head);
                            sb.Append("?");
                            head = ",";
                        }
                        sb.Append(")");
                        return new CompileResult
                                   {
                                       CommandText = sb.ToString(),
                                       Value = val
                                   };
                    }
                    else
                    {
                        queryArgs.Add(val);
                        return new CompileResult
                                   {
                                       CommandText = "?",
                                       Value = val
                                   };
                    }
                }
            }
            throw new NotSupportedException("Cannot compile: " + expr.NodeType.ToString());
        }

        /// <summary>
        /// Compiles a BinaryExpression where one of the parameters is null.
        /// </summary>
        /// <param name="parameter">The non-null parameter</param>
        private string CompileNullBinaryExpression(BinaryExpression expression, CompileResult parameter)
        {
            if (expression.NodeType == ExpressionType.Equal)
                return "(" + parameter.CommandText + " is ?)";
            else if (expression.NodeType == ExpressionType.NotEqual)
                return "(" + parameter.CommandText + " is not ?)";
            else
                throw new NotSupportedException("Cannot compile Null-BinaryExpression with type " + expression.NodeType.ToString());
        }

        string GetSqlName(Expression expr)
        {
            var n = expr.NodeType;
            if (n == ExpressionType.GreaterThan)
                return ">";
            else if (n == ExpressionType.GreaterThanOrEqual)
            {
                return ">=";
            }
            else if (n == ExpressionType.LessThan)
            {
                return "<";
            }
            else if (n == ExpressionType.LessThanOrEqual)
            {
                return "<=";
            }
            else if (n == ExpressionType.And)
            {
                return "and";
            }
            else if (n == ExpressionType.AndAlso)
            {
                return "and";
            }
            else if (n == ExpressionType.Or)
            {
                return "or";
            }
            else if (n == ExpressionType.OrElse)
            {
                return "or";
            }
            else if (n == ExpressionType.Equal)
            {
                return "=";
            }
            else if (n == ExpressionType.NotEqual)
            {
                return "!=";
            }
            else
            {
                throw new System.NotSupportedException("Cannot get SQL for: " + n.ToString());
            }
        }

        public int Count()
        {
            return this.GenerateCommand("count(*)").ExecuteScalar<int>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.GenerateCommand("*").ExecuteQuery<T>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}