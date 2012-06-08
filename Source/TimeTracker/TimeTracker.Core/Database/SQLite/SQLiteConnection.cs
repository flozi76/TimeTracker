namespace TimeTracker.Core.Database.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an open connection to a SQLite database.
    /// </summary>
    public class SQLiteConnection : IDisposable
    {
        private bool open;
        private TimeSpan busyTimeout;
        private Dictionary<string, TableMapping> mappings;
        private Dictionary<string, TableMapping> tables;
        private System.Diagnostics.Stopwatch stopwatch;
        private long elapsedMilliseconds;

        public IntPtr Handle { get; private set; }

        public string DatabasePath { get; private set; }

        public bool TimeExecution { get; set; }

        public bool Trace { get; set; }

        /// <summary>
        /// Constructs a new SQLiteConnection and opens a SQLite database specified by databasePath.
        /// </summary>
        /// <param name="databasePath">
        /// Specifies the path to the database file.
        /// </param>
        public SQLiteConnection(string databasePath)
        {
            this.DatabasePath = databasePath;
            IntPtr handle;
            var r = SQLite3.Open(this.DatabasePath, out handle);
            this.Handle = handle;
            if (r != SQLite3.Result.OK)
            {
                throw SQLiteException.New(r, "Could not open database file: " + this.DatabasePath);
            }
            this.open = true;

            this.BusyTimeout = TimeSpan.FromSeconds(0.1);
        }

        static SQLiteConnection()
        {
            if (PreserveDuringLinkMagic)
            {
                new TableInfo { name = "magic" };
            }
        }

        /// <summary>
        /// Used to list some code that we want the MonoTouch linker
        /// to see, but that we never want to actually execute.
        /// </summary>
        private const bool PreserveDuringLinkMagic = false;

        /// <summary>
        /// Sets a busy handler to sleep the specified amount of time when a table is locked.
        /// The handler will sleep multiple times until a total time of <see cref="BusyTimeout"/> has accumulated.
        /// </summary>
        public TimeSpan BusyTimeout
        {
            get { return this.busyTimeout; }
            set
            {
                this.busyTimeout = value;
                if (this.Handle != IntPtr.Zero)
                {
                    SQLite3.BusyTimeout(this.Handle, (int)this.busyTimeout.TotalMilliseconds);
                }
            }
        }

        /// <summary>
        /// Returns the mappings from types to tables that the connection
        /// currently understands.
        /// </summary>
        public IEnumerable<TableMapping> TableMappings
        {
            get
            {
                if (this.tables == null)
                {
                    return Enumerable.Empty<TableMapping>();
                }

                return this.tables.Values;
            }
        }

        /// <summary>
        /// Retrieves the mapping that is automatically generated for the given type.
        /// </summary>
        /// <param name="type">
        /// The type whose mapping to the database is returned.
        /// </param>
        /// <returns>
        /// The mapping represents the schema of the columns of the database and contains 
        /// methods to set and get properties of objects.
        /// </returns>
        public TableMapping GetMapping(Type type)
        {
            if (this.mappings == null)
            {
                this.mappings = new Dictionary<string, TableMapping>();
            }
            TableMapping map;
            if (!this.mappings.TryGetValue(type.FullName, out map))
            {
                map = new TableMapping(type);
                this.mappings[type.FullName] = map;
            }
            return map;
        }

        /// <summary>
        /// Executes a "create table if not exists" on the database. It also
        /// creates any specified indexes on the columns of the table. It uses
        /// a schema automatically generated from the specified type. You can
        /// later access this schema by calling GetMapping.
        /// </summary>
        /// <returns>
        /// The number of entries added to the database schema.
        /// </returns>
        public int CreateTable<T>()
        {
            var ty = typeof(T);

            if (this.tables == null)
            {
                this.tables = new Dictionary<string, TableMapping>();
            }
            TableMapping map;
            if (!this.tables.TryGetValue(ty.FullName, out map))
            {
                map = this.GetMapping(ty);
                this.tables.Add(ty.FullName, map);
            }
            var query = "create table \"" + map.TableName + "\"(\n";

            var decls = map.Columns.Select(p => Orm.SqlDecl(p));
            var decl = string.Join(",\n", decls.ToArray());
            query += decl;
            query += ")";

            var count = 0;

            try
            {
                this.Execute(query);
                count = 1;
            }
            catch (SQLiteException)
            {
            }

            if (count == 0)
            {
                // Table already exists, migrate it
                this.MigrateTable(map);
            }

            foreach (var p in map.Columns.Where(x => x.IsIndexed))
            {
                var indexName = map.TableName + "_" + p.Name;
                var q = string.Format("create index if not exists \"{0}\" on \"{1}\"(\"{2}\")", indexName, map.TableName, p.Name);
                count += this.Execute(q);
            }

            return count;
        }

        void MigrateTable(TableMapping map)
        {
            var query = "pragma table_info(\"" + map.TableName + "\")";

            var existingCols = this.Query<TableInfo>(query);

            var toBeAdded = new List<TableMapping.Column>();

            foreach (var p in map.Columns)
            {
                var found = false;
                foreach (var c in existingCols)
                {
                    found = p.Name == c.name;
                    if (found)
                        break;
                }
                if (!found)
                {
                    toBeAdded.Add(p);
                }
            }

            foreach (var p in toBeAdded)
            {
                var addCol = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(p);
                this.Execute(addCol);
            }
        }

        /// <summary>
        /// Creates a new SQLiteCommand given the command text with arguments. Place a '?'
        /// in the command text for each of the arguments.
        /// </summary>
        /// <param name="cmdText">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the command text.
        /// </param>
        /// <returns>
        /// A <see cref="SQLiteCommand"/>
        /// </returns>
        public SQLiteCommand CreateCommand(string cmdText, params object[] ps)
        {
            if (!this.open)
            {
                throw SQLiteException.New(SQLite3.Result.Error, "Cannot create commands from unopened database");
            }
            else
            {
                var cmd = new SQLiteCommand(this);
                cmd.CommandText = cmdText;
                foreach (var o in ps)
                {
                    cmd.Bind(o);
                }
                return cmd;
            }
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// Use this method instead of Query when you don't expect rows back. Such cases include
        /// INSERTs, UPDATEs, and DELETEs.
        /// You can set the Trace or TimeExecution properties of the connection
        /// to profile execution.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// The number of rows modified in the database as a result of this execution.
        /// </returns>
        public int Execute(string query, params object[] args)
        {
            var cmd = this.CreateCommand(query, args);

            if (this.TimeExecution)
            {
                if (this.stopwatch == null)
                {
                    this.stopwatch = new System.Diagnostics.Stopwatch();
                }
                this.stopwatch.Reset();
                this.stopwatch.Start();
            }

            int r = cmd.ExecuteNonQuery();

            if (this.TimeExecution)
            {
                this.stopwatch.Stop();
                this.elapsedMilliseconds += this.stopwatch.ElapsedMilliseconds;
                Console.WriteLine("Finished in {0} ms ({1:0.0} s total)", this.stopwatch.ElapsedMilliseconds, this.elapsedMilliseconds / 1000.0);
            }

            return r;
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the mapping automatically generated for
        /// the given type.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// </returns>
        public List<T> Query<T>(string query, params object[] args) where T : new()
        {
            var cmd = this.CreateCommand(query, args);
            return cmd.ExecuteQuery<T>();
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the specified mapping. This function is
        /// only used by libraries in order to query the database via introspection. It is
        /// normally not used.
        /// </summary>
        /// <param name="map">
        /// A <see cref="TableMapping"/> to use to convert the resulting rows
        /// into objects.
        /// </param>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// </returns>
        public List<object> Query(TableMapping map, string query, params object[] args)
        {
            var cmd = this.CreateCommand(query, args);
            return cmd.ExecuteQuery<object>(map);
        }

        /// <summary>
        /// Returns a queryable interface to the table represented by the given type.
        /// </summary>
        /// <returns>
        /// A queryable object that is able to translate Where, OrderBy, and Take
        /// queries into native SQL.
        /// </returns>
        public TableQuery<T> Table<T>() where T : new()
        {
            return new TableQuery<T>(this);
        }

        /// <summary>
        /// Attempts to retrieve an object with the given primary key from the table
        /// associated with the specified type. Use of this method requires that
        /// the given type have a designated PrimaryKey (using the PrimaryKeyAttribute).
        /// </summary>
        /// <param name="pk">
        /// The primary key.
        /// </param>
        /// <returns>
        /// The object with the given primary key. Throws a not found exception
        /// if the object is not found.
        /// </returns>
        public T Get<T>(object pk) where T : new()
        {
            var map = this.GetMapping(typeof(T));
            string query = string.Format("select * from \"{0}\" where \"{1}\" = ?", map.TableName, map.PK.Name);
            return this.Query<T>(query, pk).First();
        }

        /// <summary>
        /// Whether <see cref="BeginTransaction"/> has been called and the database is waiting for a <see cref="Commit"/>.
        /// </summary>
        public bool IsInTransaction { get; private set; }

        /// <summary>
        /// Begins a new transaction. Call <see cref="Commit"/> to end the transaction.
        /// </summary>
        public void BeginTransaction()
        {
            if (!this.IsInTransaction)
            {
                this.Execute("begin transaction");
                this.IsInTransaction = true;
            }
        }

        /// <summary>
        /// Rolls back the transaction that was begun by <see cref="BeginTransaction"/>.
        /// </summary>
        public void Rollback()
        {
            if (this.IsInTransaction)
            {
                this.Execute("rollback");
                this.IsInTransaction = false;
            }
        }

        /// <summary>
        /// Commits the transaction that was begun by <see cref="BeginTransaction"/>.
        /// </summary>
        public void Commit()
        {
            if (this.IsInTransaction)
            {
                this.Execute("commit");
                this.IsInTransaction = false;
            }
        }

        /// <summary>
        /// Executes <param name="action"> within a transaction and automatically rollsback the transaction
        /// if an exception occurs. The exception is rethrown.
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> to perform within a transaction. <param name="action"> can contain any number
        /// of operations on the connection but should never call <see cref="BeginTransaction"/>,
        /// <see cref="Rollback"/>, or <see cref="Commit"/>.
        /// </param>
        public void RunInTransaction(Action action)
        {
            if (this.IsInTransaction)
            {
                throw new InvalidOperationException("The connection must not already be in a transaction when RunInTransaction is called");
            }
            try
            {
                this.BeginTransaction();
                action();
                this.Commit();
            }
            catch (Exception)
            {
                this.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Inserts all specified objects.
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        public int InsertAll(System.Collections.IEnumerable objects)
        {
            this.BeginTransaction();
            var c = 0;
            foreach (var r in objects)
            {
                c += this.Insert(r);
            }
            this.Commit();
            return c;
        }

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        public int Insert(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return this.Insert(obj, "", obj.GetType());
        }

        public int Insert(object obj, Type objType)
        {
            return this.Insert(obj, "", objType);
        }

        public int Insert(object obj, string extra)
        {
            if (obj == null)
            {
                return 0;
            }
            return this.Insert(obj, extra, obj.GetType());
        }

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <param name="extra">
        /// Literal SQL code that gets placed into the command. INSERT {extra} INTO ...
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        public int Insert(object obj, string extra, Type objType)
        {
            if (obj == null || objType == null)
            {
                return 0;
            }

            var map = this.GetMapping(objType);

            var cols = map.InsertColumns;
            var vals = new object[cols.Length];
            for (var i = 0; i < vals.Length; i++)
            {
                vals[i] = cols[i].GetValue(obj);
            }

            var insertCmd = map.GetInsertCommand(this, extra);
            var count = insertCmd.ExecuteNonQuery(vals);

            if (map.HasAutoIncPK)
            {
                var id = SQLite3.LastInsertRowid(this.Handle);
                map.SetAutoIncPK(obj, id);
            }

            return count;
        }

        /// <summary>
        /// Updates all of the columns of a table using the specified object
        /// except for its primary key.
        /// The object is required to have a primary key.
        /// </summary>
        /// <param name="obj">
        /// The object to update. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <returns>
        /// The number of rows updated.
        /// </returns>
        public int Update(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return this.Update(obj, obj.GetType());
        }

        public int Update(object obj, Type objType)
        {
            if (obj == null || objType == null)
            {
                return 0;
            }

            var map = this.GetMapping(objType);

            var pk = map.PK;

            if (pk == null)
            {
                throw new NotSupportedException("Cannot update " + map.TableName + ": it has no PK");
            }

            var cols = from p in map.Columns
                       where p != pk
                       select p;
            var vals = from c in cols
                       select c.GetValue(obj);
            var ps = new List<object>(vals);
            ps.Add(pk.GetValue(obj));
            var q = string.Format("update \"{0}\" set {1} where {2} = ? ", map.TableName, string.Join(",", (from c in cols
                                                                                                            select "\"" + c.Name + "\" = ? ").ToArray()), pk.Name);
            return this.Execute(q, ps.ToArray());
        }

        /// <summary>
        /// Deletes the given object from the database using its primary key.
        /// </summary>
        /// <param name="obj">
        /// The object to delete. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <returns>
        /// The number of rows deleted.
        /// </returns>
        public int Delete<T>(T obj)
        {
            var map = this.GetMapping(obj.GetType());
            var pk = map.PK;
            if (pk == null)
            {
                throw new NotSupportedException("Cannot delete " + map.TableName + ": it has no PK");
            }
            var q = string.Format("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
            return this.Execute(q, pk.GetValue(obj));
        }

        public void Dispose()
        {
            this.Close();
        }

        public void Close()
        {
            if (this.open && this.Handle != IntPtr.Zero)
            {
                SQLite3.Close(this.Handle);
                this.Handle = IntPtr.Zero;
                this.open = false;
            }
        }
    }
}