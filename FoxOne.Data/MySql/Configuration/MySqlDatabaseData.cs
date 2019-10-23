using System.Collections.Generic;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;

namespace FoxOne.Data.MySql
{
    /// <summary>
    /// Describes a <see cref="MySqlDatabase"/> instance, aggregating information from a
    /// <see cref="ConnectionStringSettings"/>.
    /// </summary>
    public class MySqlDatabaseData : DatabaseData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDatabase"/> class with a connection string and a configuration
        /// source.
        ///</summary>
        ///<param name="connectionStringSettings">The <see cref="ConnectionStringSettings"/> for the represented database.</param>
        ///<param name="configurationSource">The <see cref="IConfigurationSource"/> from which additional information can 
        /// be retrieved if necessary.</param>
        public MySqlDatabaseData(ConnectionStringSettings connectionStringSettings,
                                 IConfigurationSource configurationSource)
            : base(connectionStringSettings, configurationSource)
        {
        }

        /// <summary>
        /// Creates a <see cref="TypeRegistration"/> instance describing the database represented by 
        /// this configuration object.
        /// </summary>
        /// <returns>A <see cref="TypeRegistration"/> instance describing a database.</returns>
        public override IEnumerable<TypeRegistration> GetRegistrations()
        {
            yield return new TypeRegistration<Database>(
                () => new MySqlDatabase(
                    ConnectionString))
            {
                Name = Name,
                Lifetime = TypeRegistrationLifetime.Transient
            };
        }
    }
}
