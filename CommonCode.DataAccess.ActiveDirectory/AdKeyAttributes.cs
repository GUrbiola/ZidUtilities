namespace CommonCode.DataAccess.ActiveDirectory
{
    /// <summary>
    /// Holds the three unique identifier attributes for an Active Directory record.
    /// </summary>
    /// <remarks>
    /// This class is a simple container for commonly used keys that identify an AD object:
    /// - DistinguishedName: the LDAP distinguished name of the object.
    /// - EmployeeId: an external HR system identifier (Workday Id).
    /// - UserName: the AD user name (sAMAccountName / userPrincipalName variant).
    /// Instances are plain data holders with no logic.
    /// </remarks>
    public class AdKeyAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdKeyAttributes"/> class.
        /// </summary>
        /// <remarks>
        /// The constructor intentionally does not perform any initialization logic beyond
        /// the default member initialization. Properties can be set after construction.
        /// </remarks>
        public AdKeyAttributes()
        {
        }

        /// <summary>
        /// Gets or sets the LDAP distinguished name (DN) of the Active Directory object.
        /// </summary>
        /// <value>
        /// A string containing the distinguished name, for example:
        /// "CN=John Doe,OU=Users,DC=example,DC=com".
        /// </value>
        public string DistinguishedName { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier from the HR system (Workday Id).
        /// </summary>
        /// <value>
        /// A string representing the unique employee id assigned by the HR system. This
        /// value is often used to correlate AD accounts with HR records.
        /// </value>
        public string EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the user name for the Active Directory account.
        /// </summary>
        /// <value>
        /// A string containing the user's account name (for example, sAMAccountName or UPN).
        /// This value is typically used for sign-in or display of the account identity.
        /// </value>
        public string UserName { get; set; }
    }
}
