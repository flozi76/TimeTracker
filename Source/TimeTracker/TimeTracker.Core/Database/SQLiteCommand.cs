namespace TimeTracker.Core.Database
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class SQLiteCommand
    {
        SQLiteConnection _conn;
        private List<Binding> _bindings;

        public string CommandText { get; set; }

        internal SQLiteCommand(SQLiteConnection conn)
        {
            this._conn = conn;
            this._bindings = new List<Binding>();
            this.CommandText = "";
        }

        public int ExecuteNonQuery()
        {
            if (this._conn.Trace)
            {
                Console.WriteLine("Executing: " + this);
            }

            var r = SQLite3.Result.OK;
            var stmt = this.Prepare();
            r = SQLite3.Step(stmt);
            this.Finalize(stmt);
            if (r == SQLite3.Result.Done)
            {
                int rowsAffected = SQLite3.Changes(this._conn.Handle);
                return rowsAffected;
            }
            else if (r == SQLite3.Result.Error)
            {
                string msg = SQLite3.GetErrmsg(this._conn.Handle);
                throw SQLiteException.New(r, msg);
            }
            else
            {
                throw SQLiteException.New(r, r.ToString());
            }
        }

        public List<T> ExecuteQuery<T>() where T : new()
        {
            return this.ExecuteQuery<T>(this._conn.GetMapping(typeof(T)));
        }

        public List<T> ExecuteQuery<T>(TableMapping map)
        {
            if (this._conn.Trace)
            {
                Console.WriteLine("Executing Query: " + this);
            }

            var r = new List<T>();

            var stmt = this.Prepare();

            var cols = new TableMapping.Column[SQLite3.ColumnCount(stmt)];

            for (int i = 0; i < cols.Length; i++)
            {
                var name = Marshal.PtrToStringUni(SQLite3.ColumnName16(stmt, i));
                cols[i] = map.FindColumn(name);
            }

            while (SQLite3.Step(stmt) == SQLite3.Result.Row)
            {
                var obj = Activator.CreateInstance(map.MappedType);
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i] == null)
                        continue;
                    var colType = SQLite3.ColumnType(stmt, i);
                    var val = this.ReadCol(stmt, i, colType, cols[i].ColumnType);
                    cols[i].SetValue(obj, val);
                }
                r.Add((T)obj);
            }

            this.Finalize(stmt);
            return r;
        }

        public T ExecuteScalar<T>()
        {
            if (this._conn.Trace)
            {
                Console.WriteLine("Executing Query: " + this);
            }

            T val = default(T);

            var stmt = this.Prepare();
            if (SQLite3.Step(stmt) == SQLite3.Result.Row)
            {
                var colType = SQLite3.ColumnType(stmt, 0);
                val = (T)this.ReadCol(stmt, 0, colType, typeof(T));
            }
            this.Finalize(stmt);

            return val;
        }

        public void Bind(string name, object val)
        {
            this._bindings.Add(new Binding
                                   {
                                       Name = name,
                                       Value = val
                                   });
        }

        public void Bind(object val)
        {
            this.Bind(null, val);
        }

        public override string ToString()
        {
            var parts = new string[1 + this._bindings.Count];
            parts[0] = this.CommandText;
            var i = 1;
            foreach (var b in this._bindings)
            {
                parts[i] = string.Format("  {0}: {1}", i - 1, b.Value);
                i++;
            }
            return string.Join(Environment.NewLine, parts);
        }

        IntPtr Prepare()
        {
            var stmt = SQLite3.Prepare2(this._conn.Handle, this.CommandText);
            this.BindAll(stmt);
            return stmt;
        }

        void Finalize(IntPtr stmt)
        {
            SQLite3.Finalize(stmt);
        }

        void BindAll(IntPtr stmt)
        {
            int nextIdx = 1;
            foreach (var b in this._bindings)
            {
                if (b.Name != null)
                {
                    b.Index = SQLite3.BindParameterIndex(stmt, b.Name);
                }
                else
                {
                    b.Index = nextIdx++;
                }
            }
            foreach (var b in this._bindings)
            {
                BindParameter(stmt, b.Index, b.Value);
            }
        }

        internal static IntPtr NegativePointer = new IntPtr(-1);

        internal static void BindParameter(IntPtr stmt, int index, object value)
        {
            if (value == null)
            {
                SQLite3.BindNull(stmt, index);
            }
            else
            {
                if (value is Int32)
                {
                    SQLite3.BindInt(stmt, index, (int)value);
                }
                else if (value is String)
                {
                    SQLite3.BindText(stmt, index, (string)value, -1, NegativePointer);
                }
                else if (value is Byte || value is UInt16 || value is SByte || value is Int16)
                {
                    SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
                }
                else if (value is Boolean)
                {
                    SQLite3.BindInt(stmt, index, (bool)value ? 1 : 0);
                }
                else if (value is UInt32 || value is Int64)
                {
                    SQLite3.BindInt64(stmt, index, Convert.ToInt64(value));
                }
                else if (value is Single || value is Double || value is Decimal)
                {
                    SQLite3.BindDouble(stmt, index, Convert.ToDouble(value));
                }
                else if (value is DateTime)
                {
                    SQLite3.BindText(stmt, index, ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"), -1, NegativePointer);
                }
                else if (value.GetType().IsEnum)
                {
                    SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
                }
                else if (value is byte[])
                {
                    SQLite3.BindBlob(stmt, index, (byte[])value, ((byte[])value).Length, NegativePointer);
                }
                else
                {
                    throw new NotSupportedException("Cannot store type: " + value.GetType());
                }
            }
        }

        class Binding
        {
            public string Name { get; set; }

            public object Value { get; set; }

            public int Index { get; set; }
        }

        object ReadCol(IntPtr stmt, int index, SQLite3.ColType type, Type clrType)
        {
            if (type == SQLite3.ColType.Null)
            {
                return null;
            }
            else
            {
                if (clrType == typeof(String))
                {
                    return SQLite3.ColumnString(stmt, index);
                }
                else if (clrType == typeof(Int32))
                {
                    return (int)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Boolean))
                {
                    return SQLite3.ColumnInt(stmt, index) == 1;
                }
                else if (clrType == typeof(double))
                {
                    return SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(float))
                {
                    return (float)SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(DateTime))
                {
                    var text = SQLite3.ColumnString(stmt, index);
                    return DateTime.Parse(text);
                }
                else if (clrType.IsEnum)
                {
                    return SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Int64))
                {
                    return SQLite3.ColumnInt64(stmt, index);
                }
                else if (clrType == typeof(UInt32))
                {
                    return (uint)SQLite3.ColumnInt64(stmt, index);
                }
                else if (clrType == typeof(decimal))
                {
                    return (decimal)SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(Byte))
                {
                    return (byte)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(UInt16))
                {
                    return (ushort)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Int16))
                {
                    return (short)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(sbyte))
                {
                    return (sbyte)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(byte[]))
                {
                    return SQLite3.ColumnByteArray(stmt, index);
                }
                else
                {
                    throw new NotSupportedException("Don't know how to read " + clrType);
                }
            }
        }
    }
}