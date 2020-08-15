﻿using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteQueryMultipleTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region ExecuteQueryMultiple.Extract

        #region Extract<TEntity>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithoutParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP 1 * FROM [sc].[IdentityTable];
                    SELECT TOP 2 * FROM [sc].[IdentityTable];
                    SELECT TOP 3 * FROM [sc].[IdentityTable];
                    SELECT TOP 4 * FROM [sc].[IdentityTable];
                    SELECT TOP 5 * FROM [sc].[IdentityTable];"))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract<IdentityTable>();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithMultipleTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable];",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5 }))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract<IdentityTable>();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithMultipleArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5, ColumnInt = new[] { 1, 2, 3, 4, 5 } }))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract<IdentityTable>();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractWithNormalStatementFollowedByStoredProcedures()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    EXEC [dbo].[sp_get_identity_tables];
                    EXEC [dbo].[sp_get_identity_table_by_id] @Id",
                    new { Top1 = 1, tables.Last().Id }, CommandType.Text))
                {
                    // Act
                    var value1 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(1, value1.Count());
                    Helper.AssertPropertiesEquality(tables.Where(t => t.Id == value1.First().Id).First(), value1.First());

                    // Act
                    var value2 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(tables.Count, value2.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, value2.ElementAt(tables.IndexOf(item))));

                    // Act
                    var value3 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(1, value3.Count());
                    Helper.AssertPropertiesEquality(tables.Where(t => t.Id == value3.First().Id).First(), value3.First());

                }
            }
        }

        #endregion

        #region Extract<dynamic>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractAsDynamicWithoutParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP 1 * FROM [sc].[IdentityTable];
                    SELECT TOP 2 * FROM [sc].[IdentityTable];
                    SELECT TOP 3 * FROM [sc].[IdentityTable];
                    SELECT TOP 4 * FROM [sc].[IdentityTable];
                    SELECT TOP 5 * FROM [sc].[IdentityTable];"))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractAsDynamicWithMultipleTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable];",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5 }))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractAsDynamicWithMultipleArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5, ColumnInt = new[] { 1, 2, 3, 4, 5 } }))
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.Extract();

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForExtractAsDynamicWithNormalStatementFollowedByStoredProcedures()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    EXEC [dbo].[sp_get_identity_tables];
                    EXEC [dbo].[sp_get_identity_table_by_id] @Id",
                    new { Top1 = 1, tables.Last().Id }, CommandType.Text))
                {
                    // Act
                    var value1 = result.Extract<IdentityTable>();

                    // Assert
                    Assert.AreEqual(1, value1.Count());
                    Helper.AssertPropertiesEquality(tables.Where(t => t.Id == value1.First().Id).First(), value1.First());

                    // Act
                    var value2 = result.Extract();

                    // Assert
                    Assert.AreEqual(tables.Count, value2.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, value2.ElementAt(tables.IndexOf(item))));

                    // Act
                    var value3 = result.Extract();

                    // Assert
                    Assert.AreEqual(1, value3.Count());
                    Helper.AssertMembersEquality(tables.Where(t => t.Id == value3.First().Id).First(), (ExpandoObject)value3.First());
                }
            }
        }

        #endregion

        #endregion

        #region ExecuteQueryMultipleAsync.ExtractAsync

        #region ExtractAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncMultipleForExtractAsyncWithoutParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP 1 * FROM [sc].[IdentityTable];
                    SELECT TOP 2 * FROM [sc].[IdentityTable];
                    SELECT TOP 3 * FROM [sc].[IdentityTable];
                    SELECT TOP 4 * FROM [sc].[IdentityTable];
                    SELECT TOP 5 * FROM [sc].[IdentityTable];").Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync<IdentityTable>().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncWithMultipleTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable];",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5 }).Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync<IdentityTable>().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncWithMultipleArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5, ColumnInt = new[] { 1, 2, 3, 4, 5 } }).Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync<IdentityTable>().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertPropertiesEquality(tables.ElementAt(c), items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncWithNormalStatementFollowedByStoredProcedures()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    EXEC [dbo].[sp_get_identity_tables];
                    EXEC [dbo].[sp_get_identity_table_by_id] @Id",
                    new { Top1 = 1, tables.Last().Id }, CommandType.Text).Result)
                {
                    // Act
                    var value1 = result.ExtractAsync<IdentityTable>().Result;

                    // Assert
                    Assert.AreEqual(1, value1.Count());
                    Helper.AssertPropertiesEquality(tables.Where(t => t.Id == value1.First().Id).First(), value1.First());

                    // Act
                    var value2 = result.ExtractAsync<IdentityTable>().Result;

                    // Assert
                    Assert.AreEqual(tables.Count, value2.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, value2.ElementAt(tables.IndexOf(item))));

                    // Act
                    var value3 = result.ExtractAsync<IdentityTable>().Result;

                    // Assert
                    Assert.AreEqual(1, value3.Count());
                    Helper.AssertPropertiesEquality(tables.Where(t => t.Id == value3.First().Id).First(), value3.First());

                }
            }
        }

        #endregion

        #region ExtractAsync<dynamic>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncAsDynamicWithoutParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP 1 * FROM [sc].[IdentityTable];
                    SELECT TOP 2 * FROM [sc].[IdentityTable];
                    SELECT TOP 3 * FROM [sc].[IdentityTable];
                    SELECT TOP 4 * FROM [sc].[IdentityTable];
                    SELECT TOP 5 * FROM [sc].[IdentityTable];").Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncAsDynamicWithMultipleTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable];",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5 }).Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncAsDynamicWithMultipleArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top2) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top3) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top4) * FROM [sc].[IdentityTable];
                    SELECT TOP (@Top5) * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { Top1 = 1, Top2 = 2, Top3 = 3, Top4 = 4, Top5 = 5, ColumnInt = new[] { 1, 2, 3, 4, 5 } }).Result)
                {
                    while (result.Position >= 0)
                    {
                        // Index
                        var index = result.Position + 1;

                        // Act
                        var items = result.ExtractAsync().Result;

                        // Assert
                        Assert.AreEqual(index, items.Count());

                        // Assert
                        for (var c = 0; c < index; c++)
                        {
                            Helper.AssertMembersEquality(tables.ElementAt(c), (ExpandoObject)items.ElementAt(c));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForExtractAsyncAsDynamicWithNormalStatementFollowedByStoredProcedures()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT TOP (@Top1) * FROM [sc].[IdentityTable];
                    EXEC [dbo].[sp_get_identity_tables];
                    EXEC [dbo].[sp_get_identity_table_by_id] @Id",
                    new { Top1 = 1, tables.Last().Id }, CommandType.Text).Result)
                {
                    // Act
                    var value1 = result.ExtractAsync().Result;

                    // Assert
                    Assert.AreEqual(1, value1.Count());
                    Helper.AssertMembersEquality(tables.Where(t => t.Id == value1.First().Id).First(), value1.First());

                    // Act
                    var value2 = result.ExtractAsync().Result;

                    // Assert
                    Assert.AreEqual(tables.Count, value2.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, value2.ElementAt(tables.IndexOf(item))));

                    // Act
                    var value3 = result.ExtractAsync().Result;

                    // Assert
                    Assert.AreEqual(1, value3.Count());
                    Helper.AssertMembersEquality(tables.Where(t => t.Id == value3.First().Id).First(), (ExpandoObject)value3.First());
                }
            }
        }

        #endregion

        #endregion

        #region ExecuteQueryMultiple.Scalar

        #region Scalar<TResult>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarAsTypedResultWithoutParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT GETUTCDATE();
                    SELECT (2 * 7);
                    SELECT 'USER';"))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(typeof(DateTime), value1.GetType());

                    // Assert
                    var value2 = result.Scalar<int>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(14, value2);

                    // Assert
                    var value3 = result.Scalar<string>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual("USER", value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarAsTypedResultWithMultipleParameters()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = (2 * 7),
                Value3 = "USER"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT @Value1;
                    SELECT @Value2;
                    SELECT @Value3;", param))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar<int>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(param.Value2, value2);

                    // Assert
                    var value3 = result.Scalar<string>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(param.Value3, value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarAsTypedResultWithSimpleScalaredValueFollowedByMultipleStoredProcedures()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 2,
                Value3 = 3
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT @Value1;
                    EXEC [dbo].[sp_get_database_date_time];
                    EXEC [dbo].[sp_multiply] @Value2, @Value3;", param))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(typeof(DateTime), value2.GetType());

                    // Assert
                    var value3 = result.Scalar<int>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(6, value3);
                }
            }
        }

        #endregion

        #region Scalar<object>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarWithoutParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT GETUTCDATE();
                    SELECT (2 * 7);
                    SELECT 'USER';"))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(typeof(DateTime), value1.GetType());

                    // Assert
                    var value2 = result.Scalar();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(14, value2);

                    // Assert
                    var value3 = result.Scalar();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual("USER", value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarWithMultipleParameters()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = (2 * 7),
                Value3 = "USER"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT @Value1;
                    SELECT @Value2;
                    SELECT @Value3;", param))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(param.Value2, value2);

                    // Assert
                    var value3 = result.Scalar();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(param.Value3, value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleForScalarWithSimpleScalaredValueFollowedByMultipleStoredProcedures()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 2,
                Value3 = 3
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple(@"SELECT @Value1;
                    EXEC [dbo].[sp_get_database_date_time];
                    EXEC [dbo].[sp_multiply] @Value2, @Value3;", param))
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(typeof(DateTime), value2.GetType());

                    // Assert
                    var value3 = result.Scalar();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(6, value3);
                }
            }
        }

        #endregion

        #endregion

        #region ExecuteQueryMultipleAsync.ScalarAsync

        #region ScalarAsync<TResult>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarAsTypedResultWithoutParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT GETUTCDATE();
                    SELECT (2 * 7);
                    SELECT 'USER';").Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(typeof(DateTime), value1.GetType());

                    // Assert
                    var value2 = result.Scalar<int>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(14, value2);

                    // Assert
                    var value3 = result.Scalar<string>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual("USER", value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarAsTypedResultWithMultipleParameters()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = (2 * 7),
                Value3 = "USER"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT @Value1;
                    SELECT @Value2;
                    SELECT @Value3;", param).Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar<int>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(param.Value2, value2);

                    // Assert
                    var value3 = result.Scalar<string>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(param.Value3, value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarAsTypedResultWithSimpleScalaredValueFollowedByMultipleStoredProcedures()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 2,
                Value3 = 3
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT @Value1;
                    EXEC [dbo].[sp_get_database_date_time];
                    EXEC [dbo].[sp_multiply] @Value2, @Value3;", param).Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.Scalar<DateTime>();
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(typeof(DateTime), value2.GetType());

                    // Assert
                    var value3 = result.Scalar<int>();
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(6, value3);
                }
            }
        }

        #endregion

        #region ScalarAsync<object>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarWithoutParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT GETUTCDATE();
                    SELECT (2 * 7);
                    SELECT 'USER';").Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(typeof(DateTime), value1.GetType());

                    // Assert
                    var value2 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(14, value2);

                    // Assert
                    var value3 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value3);
                    Assert.AreEqual("USER", value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarWithMultipleParameters()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = (2 * 7),
                Value3 = "USER"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT @Value1;
                    SELECT @Value2;
                    SELECT @Value3;", param).Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(param.Value2, value2);

                    // Assert
                    var value3 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(param.Value3, value3);
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleAsyncForScalarWithSimpleScalaredValueFollowedByMultipleStoredProcedures()
        {
            // Setup
            var param = new
            {
                Value1 = DateTime.UtcNow,
                Value2 = 2,
                Value3 = 3
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                using (var result = connection.ExecuteQueryMultipleAsync(@"SELECT @Value1;
                    EXEC [dbo].[sp_get_database_date_time];
                    EXEC [dbo].[sp_multiply] @Value2, @Value3;", param).Result)
                {
                    // Index
                    var index = result.Position + 1;

                    // Assert
                    var value1 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value1);
                    Assert.AreEqual(param.Value1, value1);

                    // Assert
                    var value2 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value2);
                    Assert.AreEqual(typeof(DateTime), value2.GetType());

                    // Assert
                    var value3 = result.ScalarAsync().Result;
                    Assert.IsNotNull(value3);
                    Assert.AreEqual(6, value3);
                }
            }
        }

        #endregion

        #endregion

    }
}