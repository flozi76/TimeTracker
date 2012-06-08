namespace TimeTracker.Core.Database.SQLite
{
    using System;

    /// <summary>
    /// Since the insert never changed, we only need to prepare once.
    /// </summary>
    public class PreparedSqlLiteInsertCommand : IDisposable
    {
        public bool Initialized { get; set; }

        protected SQLiteConnection Connection { get; set; }

        public string CommandText { get; set; }

        protected IntPtr Statement { get; set; }

        internal PreparedSqlLiteInsertCommand(SQLiteConnection conn)
        {
            this.Connection = conn;
        }

        public int ExecuteNonQuery(object[] source)
        {
            if (this.Connection.Trace)
            {
                Console.WriteLine("Executing: " + this.CommandText);
            }

            var r = SQLite3.Result.OK;

            if (!this.Initialized)
            {
                this.Statement = this.Prepare();
                this.Initialized = true;
            }

            //bind the values.
            if (source != null)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    SQLiteCommand.BindParameter(this.Statement, i + 1, source[i]);
                }
            }
            r = SQLite3.Step(this.Statement);

            if (r == SQLite3.Result.Done)
            {
                int rowsAffected = SQLite3.Changes(this.Connection.Handle);
                SQLite3.Reset(this.Statement);
                return rowsAffected;
            }
            else if (r == SQLite3.Result.Error)
            {
                string msg = SQLite3.GetErrmsg(this.Connection.Handle);
                SQLite3.Reset(this.Statement);
                throw SQLiteException.New(r, msg);
            }
            else
            {
                SQLite3.Reset(this.Statement);
                throw SQLiteException.New(r, r.ToString());
            }
        }

        protected virtual IntPtr Prepare()
        {
            var stmt = SQLite3.Prepare2(this.Connection.Handle, this.CommandText);
            return stmt;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.Statement != IntPtr.Zero)
            {
                try
                {
                    SQLite3.Finalize(this.Statement);
                }
                finally
                {
                    this.Statement = IntPtr.Zero;
                    this.Connection = null;
                }
            }
        }

        ~PreparedSqlLiteInsertCommand()
        {
            this.Dispose(false);
        }
    }
}