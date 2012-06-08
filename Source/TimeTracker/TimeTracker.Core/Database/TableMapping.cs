namespace TimeTracker.Core.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class TableMapping
    {
        public Type MappedType { get; private set; }

        public string TableName { get; private set; }

        public Column[] Columns { get; private set; }

        public Column PK { get; private set; }

        Column _autoPk = null;
        Column[] _insertColumns = null;
        string _insertSql = null;

        public TableMapping(Type type)
        {
            this.MappedType = type;
            this.TableName = this.MappedType.Name;
            var props = this.MappedType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            var cols = new List<Column>();
            foreach (var p in props)
            {
                var ignore = p.GetCustomAttributes(typeof(IgnoreAttribute), true).Length > 0;
                if (p.CanWrite && !ignore)
                {
                    cols.Add(new PropColumn(p));
                }
            }
            this.Columns = cols.ToArray();
            foreach (var c in this.Columns)
            {
                if (c.IsAutoInc && c.IsPK)
                {
                    this._autoPk = c;
                }
                if (c.IsPK)
                {
                    this.PK = c;
                }
            }

            this.HasAutoIncPK = this._autoPk != null;
        }

        public bool HasAutoIncPK { get; private set; }

        public void SetAutoIncPK(object obj, long id)
        {
            if (this._autoPk != null)
            {
                this._autoPk.SetValue(obj, Convert.ChangeType(id, this._autoPk.ColumnType));
            }
        }

        public Column[] InsertColumns
        {
            get
            {
                if (this._insertColumns == null)
                {
                    this._insertColumns = this.Columns.Where(c => !c.IsAutoInc).ToArray();
                }
                return this._insertColumns;
            }
        }

        public Column FindColumn(string name)
        {
            var exact = this.Columns.Where(c => c.Name == name).FirstOrDefault();
            return exact;
        }

        public string InsertSql(string extra)
        {
            if (this._insertSql == null)
            {
                var cols = this.InsertColumns;
                this._insertSql = string.Format("insert {3} into \"{0}\"({1}) values ({2})", this.TableName, string.Join(",", (from c in cols
                                                                                                                               select "\"" + c.Name + "\"").ToArray()), string.Join(",", (from c in cols
                                                                                                                                                                                          select "?").ToArray()), extra);
            }
            return this._insertSql;
        }

        PreparedSqlLiteInsertCommand _insertCommand;
        string _insertCommandExtra = null;

        public PreparedSqlLiteInsertCommand GetInsertCommand(SQLiteConnection conn, string extra)
        {
            if (this._insertCommand == null || this._insertCommandExtra != extra)
            {
                var insertSql = this.InsertSql(extra);
                this._insertCommand = new PreparedSqlLiteInsertCommand(conn);
                this._insertCommand.CommandText = insertSql;
                this._insertCommandExtra = extra;
            }
            return this._insertCommand;
        }

        public abstract class Column
        {
            public string Name { get; protected set; }

            public Type ColumnType { get; protected set; }

            public string Collation { get; protected set; }

            public bool IsAutoInc { get; protected set; }

            public bool IsPK { get; protected set; }

            public bool IsIndexed { get; protected set; }

            public bool IsNullable { get; protected set; }

            public int MaxStringLength { get; protected set; }

            public abstract void SetValue(object obj, object val);

            public abstract object GetValue(object obj);
        }

        public class PropColumn : Column
        {
            PropertyInfo _prop;

            public PropColumn(PropertyInfo prop)
            {
                this._prop = prop;
                this.Name = prop.Name;
                //If this type is Nullable<T> then Nullable.GetUnderlyingType returns the T, otherwise it returns null, so get the the actual type instead
                this.ColumnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                this.Collation = Orm.Collation(prop);
                this.IsAutoInc = Orm.IsAutoInc(prop);
                this.IsPK = Orm.IsPK(prop);
                this.IsIndexed = Orm.IsIndexed(prop);
                this.IsNullable = !this.IsPK;
                this.MaxStringLength = Orm.MaxStringLength(prop);
            }

            public override void SetValue(object obj, object val)
            {
                this._prop.SetValue(obj, val, null);
            }

            public override object GetValue(object obj)
            {
                return this._prop.GetValue(obj, null);
            }
        }
    }
}