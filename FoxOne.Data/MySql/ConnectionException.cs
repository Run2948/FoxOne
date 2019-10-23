//===============================================================================
// Microsoft patterns & practices Enterprise Library Contribution
// Data Access Application Block
//===============================================================================

using System;
using System.Runtime.Serialization;

namespace FoxOne.Data.MySql
{
	/// <summary>
	/// Extends Exception to be specific to errors that occur in connection
	/// with a connection.
	/// </summary>
	/// <remarks>
	/// Revision 1: Steve Phillips  Date: 23 May 2009 - Updated to use EntLib 4.1 core
	/// </remarks>
	/// <author>Wesley Hobbie</author>
	/// <version>4.1</version>
	/// <date>05/23/2009</date>
	[Serializable]
	public class ConnectionException : SystemException
	{
		#region Construction
		/// <summary>
		/// Creates a throwable ConnectionException.
		/// </summary>
		public ConnectionException()
		{
		}

		/// <summary>
		/// Creates a throwable ConnectionException.
		/// </summary>
		/// <param name="message">A message related to the exception</param>
		public ConnectionException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Creates a throwable ConnectionException.
		/// </summary>
		/// <param name="message">A message related to the exception</param>
		/// <param name="innerException">The inner exception</param>
		public ConnectionException(string message,
															 Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected ConnectionException(SerializationInfo info,
															 StreamingContext context)
			: base(info, context)
		{
		}
		#endregion
	}
}
