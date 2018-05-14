﻿using System.Data;
using System.Data.SQLite;

namespace ControlSUR
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
            _connection = new SQLiteConnection("Data Source=control_sur_db.db;Version=3;New=False;Compress=True;");
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

        public void ExecuteNonQuery(string query)
        {
            _command = _connection.CreateCommand();
            _command.CommandText = query;
            _command.CommandType = CommandType.Text;
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