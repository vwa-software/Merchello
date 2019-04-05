namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoFiveZero
{
	using System;
	using System.Linq;

	using Merchello.Core.Configuration;
	using Merchello.Core.Persistence.DatabaseModelDefinitions;

	using Umbraco.Core;
	using Umbraco.Core.Persistence.Migrations;
	using Umbraco.Core.Persistence.Migrations.Syntax.Create.Index;
	using Umbraco.Core.Persistence.Migrations.Syntax.Expressions;

	/// <summary>
	/// Adds name, price, sale price, barcode and manufacturer indexes to product variant.
	/// </summary>
	[Migration("2.7.0.1217", 1, MerchelloConfiguration.MerchelloMigrationName)]
	public class AddSortNrToEntityCollections : MerchelloMigrationBase, IMerchelloMigration
	{

		/// <summary>
		/// Updates the merchProduct2EntityCollection table adding the sortOrder field.
		/// </summary>
		public override void Up()
		{
			var dbContext = ApplicationContext.Current.DatabaseContext;
			var database = dbContext.Database;

			var columns = SqlSyntax.GetColumnsInSchema(database).ToArray();

			if (!columns.Any(x => x.TableName.InvariantEquals("merchProduct2EntityCollection") && x.ColumnName.InvariantEquals("sortOrder")))
			{
				Logger.Info(typeof(AddSortNrToEntityCollections), "Adding sortOrder column to merchProduct2EntityCollection table.");

				//// Add the new sortorder code column
				Create.Column("sortOrder").OnTable("merchProduct2EntityCollection").AsInt32().Nullable();
			}

			if (!columns.Any(x => x.TableName.InvariantEquals("merchInvoice2EntityCollection") && x.ColumnName.InvariantEquals("sortOrder")))
			{
				Logger.Info(typeof(AddSortNrToEntityCollections), "Adding sortOrder column to merchInvoice2EntityCollection table.");

				//// Add the new sortorder code column
				Create.Column("sortOrder").OnTable("merchInvoice2EntityCollection").AsInt32().Nullable();
			}

			if (!columns.Any(x => x.TableName.InvariantEquals("merchCustomer2EntityCollection") && x.ColumnName.InvariantEquals("sortOrder")))
			{
				Logger.Info(typeof(AddSortNrToEntityCollections), "Adding sortOrder column to merchCustomer2EntityCollection table.");

				//// Add the new sortorder code column
				Create.Column("sortOrder").OnTable("merchCustomer2EntityCollection").AsInt32().Nullable();
			}
		}

		/// <summary>
		/// Downgrades the database.
		/// </summary>
		public override void Down()
		{
			var dbContext = ApplicationContext.Current.DatabaseContext;
			var database = dbContext.Database;

			var columns = SqlSyntax.GetColumnsInSchema(database).ToArray();

			if (columns.Any(x => x.TableName.InvariantEquals("merchProduct2EntityCollection") && x.ColumnName.InvariantEquals("sortOrder")))
			{
				Logger.Info(typeof(AddSortNrToEntityCollections), "Removing sortOrder column to merchProduct2EntityCollection table.");

				//// remove the  sortorder code column
				Delete.Column("sortOrder").FromTable("merchProduct2EntityCollection");
			}

			if (columns.Any(x => x.TableName.InvariantEquals("merchInvoice2EntityCollection") && x.ColumnName.InvariantEquals("sortOrder")))
			{
				Logger.Info(typeof(AddSortNrToEntityCollections), "Removing sortOrder column to merchInvoice2EntityCollection table.");

				//// remove the  sortorder code column
				Delete.Column("sortOrder").FromTable("merchInvoice2EntityCollection");
			}

			if (columns.Any(x => x.TableName.InvariantEquals("merchCustomer2EntityCollection") && x.ColumnName.InvariantEquals("sortOrder")))
			{
				Logger.Info(typeof(AddSortNrToEntityCollections), "Removing sortOrder column to merchCustomer2EntityCollection table.");

				//// remove the  sortorder code column
				Delete.Column("sortOrder").FromTable("merchCustomer2EntityCollection");
			}

		}


	}
}