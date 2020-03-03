using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace FE2PDF
{
    public class DB
    {
        private readonly SQLiteConnection  _connection;
        private SQLiteCommand     _command;
        private SQLiteDataAdapter _adapter;
        private SQLiteTransaction _transaction;

        private static DB _instance;

        private DB()
        {
            _connection = new SQLiteConnection(ConfigurationManager.ConnectionStrings["FE2PDF"].ConnectionString);
        }

        public static DB Instance()
        {
            return _instance ?? (_instance = new DB());
        }

        public void Connect()
        {
            if (_connection == null)
                return;

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public void Disconnect()
        {
            if (_connection == null)
                return;

            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        public void BeginTran()
        {
            if (_transaction == null)
            {
                _transaction = _connection.BeginTransaction();
            }
        }

        public void CommitTran()
        {
            if (_transaction == null) return;

            _transaction.Commit();

            _transaction = null;
        }

        public void RollbackTran()
        {
            if (_transaction == null) return;

            _transaction.Rollback();

            _transaction = null;
        }

        public void ExecuteNonQuery(string query, Hashtable parameters = null)
        {
            _command = _connection.CreateCommand();
            _command.CommandText = query;
            _command.CommandType = CommandType.Text;

            if (parameters != null && parameters.Count > 0)
            {
                _command.Parameters.Clear();

                foreach (DictionaryEntry p in parameters)
                {
                    _command.Parameters.AddWithValue(p.Key.ToString(), p.Value ?? DBNull.Value);
                }
            }

            _command.ExecuteNonQuery();
        }

        public DataTable ExecuteDataTable(string query)
        {
            _command = _connection.CreateCommand();
            _command.CommandText = query;
            _command.CommandType = CommandType.Text;

            var dt = new DataTable("table");
            _adapter = new SQLiteDataAdapter(_command);

            _adapter.Fill(dt);

            _adapter.Dispose();
            _adapter = null;

            return dt;
        }

        public DataSet ExecuteDataSet(string query)
        {
            _command = _connection.CreateCommand();
            _command.CommandText = query;
            _command.CommandType = CommandType.Text;

            var ds = new DataSet("set");
            _adapter = new SQLiteDataAdapter(_command);

            _adapter.Fill(ds);

            _adapter.Dispose();
            _adapter = null;

            return ds;
        }

        public object ExecuteScalar(string query)
        {
            _command = _connection.CreateCommand();
            _command.CommandText = query;
            _command.CommandType = CommandType.Text;
            return _command.ExecuteScalar();
        }
    }
}