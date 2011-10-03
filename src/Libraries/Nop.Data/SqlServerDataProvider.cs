using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Text;
using System.Web.Hosting;
using Nop.Core;
using Nop.Data.Initializers;

namespace Nop.Data
{
    public class SqlServerDataProvider : BaseEfDataProvider
    {
        public override IDbConnectionFactory GetConnectionFactory()
        {
            return new SqlConnectionFactory();
        }

        public override void SetDatabaseInitializer()
        {
            //pass some table names to ensure that we have nopCommerce 2.X installed
            var tablesToValidate = new[] {"Customer", "Discount", "Order", "Product", "ShoppingCartItem"};

            //custom commands (stored proedures, indexes)

            var customCommands = new List<string>();
            customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/SqlServer.Indexes.sql"), false));
            customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/SqlServer.StoredProcedures.sql"), false));
            
            var initializer = new CreateTablesIfNotExist<NopObjectContext>(tablesToValidate, customCommands.ToArray());
            Database.SetInitializer<NopObjectContext>(initializer);
        }

        protected virtual string[] ParseCommands(string filePath, bool throwExceptionIfNonExists)
        {
            if (!File.Exists(filePath))
            {
                if (throwExceptionIfNonExists)
                    throw new ArgumentException(string.Format("Specified file doesn't exist - {0}", filePath));
                else
                    return new string[0];
            }


            var statements = new List<string>();
            using (var stream = File.OpenRead(filePath))
            using (var reader = new StreamReader(stream))
            {
                var statement = "";
                while ((statement = readNextStatementFromStream(reader)) != null)
                {
                    statements.Add(statement);
                }
            }

            return statements.ToArray();
        }

        protected virtual string readNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();

            string lineOfText;

            while (true)
            {
                lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();
                    else
                        return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }

        public override bool StoredProceduredSupported
        {
            get { return true; }
        }
    }
}
