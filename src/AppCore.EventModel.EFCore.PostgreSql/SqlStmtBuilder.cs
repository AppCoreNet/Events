using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AppCore.EventModel.EntityFrameworkCore.PostgreSql
{
    internal class SqlStmtBuilder
    {
        private readonly StringBuilder _stmt = new StringBuilder();
        private readonly char _quoteNameStart = '"';
        private readonly char _quoteNameEnd = '"';
        private readonly IModel _model;

        public SqlStmtBuilder(IModel model)
        {
            _model = model;
        }

        public SqlStmtBuilder Append(char ch)
        {
            _stmt.Append(ch);
            return this;
        }

        public SqlStmtBuilder Append(string str)
        {
            _stmt.Append(str);
            return this;
        }

        private SqlStmtBuilder AppendQuoted(string name)
        {
            return Append(_quoteNameStart)
                   .Append(name)
                   .Append(_quoteNameEnd);
        }

        public SqlStmtBuilder AppendTableName<TEntity>(string alias = null)
        {
            IEntityType type = _model.FindEntityType(typeof(TEntity));

            string schema = type.GetSchema();
            if (schema != null)
            {
                AppendQuoted(schema);
                Append('.');
            }

            AppendQuoted(type.GetTableName());

            if (alias != null)
            {
                Append(' ');
                AppendQuoted(alias);
            }

            return this;
        }

        public SqlStmtBuilder AppendColumnName<TEntity>(string propertyName, string alias = null)
        {
            IEntityType type = _model.FindEntityType(typeof(TEntity));

            IProperty property = type.FindProperty(propertyName);
            if (property == null)
                throw new ArgumentException($"Unknown property {propertyName}.");

            if (alias != null)
            {
                AppendQuoted(alias);
                Append(".");
            }

            return AppendQuoted(property.GetColumnName());
        }

        public SqlStmtBuilder AppendColumnNames<TEntity>(IEnumerable<string> propertyNames, string alias = null)
        {
            int count = 0;
            foreach (string propertyName in propertyNames)
            {
                if (count > 0)
                    Append(',');

                AppendColumnName<TEntity>(propertyName, alias);
                ++count;
            }

            return this;
        }

        public override string ToString()
        {
            return _stmt.ToString();
        }
    }
}