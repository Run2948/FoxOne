//===============================================================================
// Microsoft patterns & practices Enterprise Library Contribution
// Data Access Application Block
//===============================================================================

using System;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using MySql.Data.MySqlClient;

namespace FoxOne.Data.MySql
{
    /// <summary>
	/// Represents a MySQL Database.
	/// </summary>
	/// <remarks>
	/// 	<para>Internally uses MySQL Provider from MySQL (MySQL.Data) to connect to the database.</para>
	/// 	<para>Revision 1: Wesley Hobbie    Date: 20 January 2006 - Updated for Enterprise Library 2.0</para>
	/// 	<para>Revision 2: Wesley Hobbie    Date: 6 February 2007 - Updated to use MySQL Driver 5.0.3</para>
	/// 	<para>Revision 3: Steve Phillips   Date: 23 May 2009 - Updated to use EntLib 4.1 core and MySQL Driver 6.0.3</para>
    /// 	<para>Revision 4: Jeremi Bourgault Date: 19 October 2011 - Updated to use EntLib 5 core and MySQL Driver 6.4.4</para>
    /// </remarks>
	/// <author>Wesley Hobbie</author>
	/// <version>5.0.505.0</version>
	/// <date>05/23/2009</date>
	[ConfigurationElementType(typeof(MySqlDatabaseData))]
    public class MySqlDatabase : Database
	{
		#region Constants
		/// <summary>
		/// The parameter token used to delimit parameters for the MySQL database.
		/// </summary>
		/// <remarks>MySQL now recognises '?' as its preferred parameter token, however the .NET data
		/// provider is still using the '@' sign</remarks>
		protected const char ParameterToken = '@';
		#endregion

		#region Construction
		/// <summary>
		/// Initializes a new instance of the <see cref="MySqlDatabase"/> class
		/// with a connection string.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		public MySqlDatabase(string connectionString)
			: base(connectionString, MySqlClientFactory.Instance)
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Adds a new In <see cref="DbParameter"/> object to the given
		/// <paramref name="command"/>.
		/// </summary>
		/// <param name="command">The command to add the in parameter.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="dbType">One of the <see cref="MySqlDbType"/>
		/// values.</param>
		/// <remarks>
		/// This version of the method is used when you can have the same
		/// parameter object multiple times with different values.
		/// </remarks>
		[CLSCompliantAttribute(false)]
		public void AddInParameter(DbCommand command,
															string name,
															MySqlDbType dbType)
		{
			this.AddParameter(command, name, dbType, ParameterDirection.Input,
												String.Empty, DataRowVersion.Default, null);
		}

		/// <summary>
		/// Adds a new In <see cref="DbParameter"/> object to the given
		/// <paramref name="command"/>.
		/// </summary>
		/// <param name="command">The commmand to add the parameter.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="dbType">One of the <see cref="MySqlDbType"/>
		/// values.</param>
		/// <param name="value">The value of the parameter.</param>
		[CLSCompliantAttribute(false)]
		public void AddInParameter(DbCommand command,
															string name,
															MySqlDbType dbType,
															object value)
		{
			AddParameter(command, name, dbType, ParameterDirection.Input,
									 String.Empty, DataRowVersion.Default, value);
		}

		/// <summary>
		/// Adds a new In <see cref="DbParameter"/> object to the given
		/// <paramref name="command"/>.
		/// </summary>
		/// <param name="command">The command to add the parameter.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="dbType">One of the <see cref="MySqlDbType"/>
		/// values.</param>
		/// <param name="sourceColumn">The name of the source column
		/// mapped to the DataSet and used for loading or returning the value.</param>
		/// <param name="sourceVersion">One of the
		/// <see cref="DataRowVersion"/> values.</param>
		[CLSCompliantAttribute(false)]
		public void AddInParameter(DbCommand command,
															string name,
															MySqlDbType dbType,
															string sourceColumn,
															DataRowVersion sourceVersion)
		{
			this.AddParameter(command, name, dbType, 0,
												ParameterDirection.Input, true, 0, 0,
												sourceColumn, sourceVersion, null);
		}

		/// <summary>
		/// Adds a new Out <see cref="DbParameter"/> object to the given
		/// <paramref name="command"/>.
		/// </summary>
		/// <param name="command">The command to add the out parameter.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="dbType">One of the <see cref="MySqlDbType"/>
		/// values.</param>
		/// <param name="size">The maximum size of the data within the
		/// column.</param>
		[CLSCompliantAttribute(false)]
		public void AddOutParameter(DbCommand command,
																string name,
																MySqlDbType dbType,
																int size)
		{
			this.AddParameter(command, name, dbType, size,
												ParameterDirection.Output, true, 0, 0,
												String.Empty, DataRowVersion.Default,
												DBNull.Value);
		}

		/// <summary>
		/// Adds a new instance of a <see cref="DbParameter"/> object to
		/// the command.
		/// </summary>
		/// <param name="command">The command to add the parameter.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="dbType">One of the <see cref="DbType"/> values.</param>
		/// <param name="size">The maximum size of the data within the
		/// column.</param>
		/// <param name="direction">One of the
		/// <see cref="ParameterDirection"/> values.</param>
		/// <param name="nullable">A value indicating whether the
		/// parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual
		/// Basic) values.</param>
		/// <param name="precision">The maximum number of digits used to
		/// represent the <paramref name="value"/>.</param>
		/// <param name="scale">The number of decimal places to which
		/// <paramref name="value"/> is resolved.</param>
		/// <param name="sourceColumn">The name of the source column
		/// mapped to the DataSet and used for loading or returning the
		/// <paramref name="value"/>.</param>
		/// <param name="sourceVersion">One of the
		/// <see cref="DataRowVersion"/> values.</param>
		/// <param name="value">The value of the parameter.</param>
		[CLSCompliantAttribute(false)]
		public virtual void AddParameter(DbCommand command,
																		string name,
																		MySqlDbType dbType,
																		int size,
																		ParameterDirection direction,
																		bool nullable,
																		byte precision,
																		byte scale,
																		string sourceColumn,
																		DataRowVersion sourceVersion,
																		object value)
		{
			DbParameter parameter = this.CreateParameter(name, dbType, size,
																									 direction, nullable,
																									 precision, scale,
																									 sourceColumn,
																									 sourceVersion, value);
			command.Parameters.Add(parameter);
		}

		/// <summary>
		/// Adds a new instance of a <see cref="DbParameter"/> object to
		/// the command.
		/// </summary>
		/// <param name="command">The command to add the parameter.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="dbType">One of the <see cref="MySqlDbType"/>
		/// values.</param>
		/// <param name="direction">One of the
		/// <see cref="ParameterDirection"/> values.</param>
		/// <param name="sourceColumn">The name of the source column
		/// mapped to the DataSet and used for loading or returning the
		/// <paramref name="value"/>.</param>
		/// <param name="sourceVersion">One of the
		/// <see cref="DataRowVersion"/> values.</param>
		/// <param name="value">The value of the parameter.</param>
		[CLSCompliantAttribute(false)]
		public void AddParameter(DbCommand command,
														string name,
														MySqlDbType dbType,
														ParameterDirection direction,
														string sourceColumn,
														DataRowVersion sourceVersion,
														object value)
		{
			this.AddParameter(command, name, dbType, 0, direction, false, 0, 0,
												sourceColumn, sourceVersion, value);
		}

		/// <summary>
		/// Builds a value parameter name for the current database.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>A correctly formatted parameter name.</returns>
		public override string BuildParameterName(string name)
		{
			if (name[0] != ParameterToken)
			{
				return name.Insert(0, new string(ParameterToken, 1));
			}
			return name;
		}

		/// <summary>
		/// Configures a given <see cref="DbParameter"/>.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="dbType">One of the <see cref="MySqlDbType"/>
		/// values.</param>
		/// <param name="size">The maximum size of the data within the
		/// column.</param>
		/// <param name="direction">One of the
		/// <see cref="ParameterDirection"/> values.</param>
		/// <param name="nullable">A value indicating whether the
		/// parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual
		/// Basic) values.</param>
		/// <param name="precision">The maximum number of digits used to
		/// represent the <paramref name="value"/>.</param>
		/// <param name="scale">The number of decimal places to which
		/// <paramref name="value"/> is resolved.</param>
		/// <param name="sourceColumn">The name of the source column
		/// mapped to the DataSet and used for loading or returning the
		/// <paramref name="value"/>.</param>
		/// <param name="sourceVersion">One of the
		/// <see cref="DataRowVersion"/> values.</param>
		/// <param name="value">The value of the parameter.</param>
		[CLSCompliantAttribute(false)]
		protected virtual void ConfigureParameter(MySqlParameter parameter,
																							string name,
																							MySqlDbType dbType,
																							int size,
																							ParameterDirection direction,
																							bool nullable,
																							byte precision,
																							byte scale,
																							string sourceColumn,
																							DataRowVersion sourceVersion,
																							object value)
		{
			parameter.MySqlDbType = dbType;
			parameter.Size = size;
			parameter.Value = (value == null) ? DBNull.Value : value;
			parameter.Direction = direction;
			parameter.IsNullable = nullable;
			parameter.Precision = precision;
			parameter.Scale = scale;
			parameter.SourceColumn = sourceColumn;
			parameter.SourceVersion = sourceVersion;
		}

		/// <summary>
		/// Adds a new instance of a <see cref="DbParameter"/> object.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="dbType">One of the <see cref="DbType"/> values.</param>
		/// <param name="size">The maximum size of the data within the
		/// column.</param>
		/// <param name="direction">One of the
		/// <see cref="ParameterDirection"/> values.</param>
		/// <param name="nullable">A value indicating whether the
		/// parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual
		/// Basic) values.</param>
		/// <param name="precision">The maximum number of digits used to
		/// represent the <paramref name="value"/>.</param>
		/// <param name="scale">The number of decimal places to which
		/// <paramref name="value"/> is resolved.</param>
		/// <param name="sourceColumn">The name of the source column
		/// mapped to the DataSet and used for loading or returning the
		/// <paramref name="value"/>.</param>
		/// <param name="sourceVersion">One of the
		/// <see cref="DataRowVersion"/> values.</param>
		/// <param name="value">The value of the parameter.</param>
		/// <returns></returns>
		[CLSCompliantAttribute(false)]
		protected DbParameter CreateParameter(string name,
																					MySqlDbType dbType,
																					int size,
																					ParameterDirection direction,
																					bool nullable,
																					byte precision,
																					byte scale,
																					string sourceColumn,
																					DataRowVersion sourceVersion,
																					object value)
		{
			MySqlParameter param = this.CreateParameter(name) as MySqlParameter;
			this.ConfigureParameter(param, name, dbType, size, direction,
															nullable, precision, scale, sourceColumn,
															sourceVersion, value);
			return param;
		}

		/// <summary>
		/// Retrieves parameter information from the stored procedure specified
		/// in the <see cref="DbCommand"/> and populates the Parameters
		/// collection of the specified <see cref="DbCommand"/> object.
		/// </summary>
		/// <param name="discoveryCommand">The <see cref="DbCommand"/> to do
		/// the discovery.</param>
		/// <remarks>The <see cref="DbCommand"/> must be a
		/// <see cref="MySqlCommand"/> instance.</remarks>
		protected override void DeriveParameters(DbCommand discoveryCommand)
		{
			MySqlCommandBuilder.DeriveParameters((MySqlCommand)discoveryCommand);
		}

		/// <summary>
		/// Determines if the number of parameters in the command matches the
		/// array of parameter values.
		/// </summary>
		/// <param name="command">The <see cref="DbCommand"/> containing the
		/// parameters.</param>
		/// <param name="values">The array of parameter values.</param>
		/// <returns>
		/// 	<see langword="true"/> if the number of parameters and
		/// values match; otherwise, <see langword="false"/>.
		/// </returns>
		protected override bool SameNumberOfParametersAndValues(DbCommand command,
																														object[] values)
		{
			int returnParameterCount = 0;
			int numberOfParametersToStoredProcedure = command.Parameters.Count - returnParameterCount;
			int numberOfValuesProvidedForStoredProcedure = values.Length;
			return numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure;
		}

		/// <summary>
		/// Sets the RowUpdated event for the data adapter.
		/// </summary>
		/// <param name="adapter">The <see cref="DbDataAdapter"/> to set the
		/// event.</param>
		protected override void SetUpRowUpdatedEvent(DbDataAdapter adapter)
		{
			((MySqlDataAdapter)adapter).RowUpdated += new MySqlRowUpdatedEventHandler(OnMySqlRowUpdated);
		}
		#endregion

		#region Public Static methods
		/// <summary>
		/// Checks if a database command is a MySql command and converts.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns>converted MySqlCommand</returns>
		public static MySqlCommand CheckIfMySqlCommand(DbCommand command)
		{
			MySqlCommand mySqlCommand = command as MySqlCommand;
			if (mySqlCommand == null)
				throw new ArgumentException("The command must be a SqlCommand.", "command");

			return mySqlCommand;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Called when [my SQL row updated].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="rowThatCouldNotBeWritten">The <see cref="MySqlRowUpdatedEventArgs"/> instance containing the event data.</param>
		/// <devdoc>
		/// Listens for the RowUpdate event on a data adapter to support
		/// UpdateBehavior.Continue
		/// </devdoc>
		private void OnMySqlRowUpdated(object sender, MySqlRowUpdatedEventArgs rowThatCouldNotBeWritten)
		{
			if (rowThatCouldNotBeWritten.RecordsAffected == 0)
			{
				if (rowThatCouldNotBeWritten.Errors != null)
				{
					rowThatCouldNotBeWritten.Row.RowError = "Failed to update row ";
					rowThatCouldNotBeWritten.Status = UpdateStatus.SkipCurrentRow;
				}
			}
		}
		#endregion
	}
}
