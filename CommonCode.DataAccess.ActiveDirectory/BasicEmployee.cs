namespace ZidUtilities.CommonCode.DataAccess.ActiveDirectory
{
    /// <summary>
    /// Represents a basic Active Directory employee record with commonly used identity and contact fields.
    /// This DTO is intended for lightweight lookups where only fundamental employee information is required.
    /// </summary>
    public class BasicEmployee
    {
        /// <summary>
        /// Gets or sets the unique employee identifier assigned by the organization.
        /// </summary>
        /// <value>A <see cref="string"/> containing the employee's identifier (for example, employee number or ID).</value>
        public string EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the distinguished name (DN) of the user object in Active Directory.
        /// </summary>
        /// <value>A <see cref="string"/> containing the AD distinguished name, e.g. "CN=John Doe,OU=Users,DC=example,DC=com".</value>
        public string DistinguishedName { get; set; }

        /// <summary>
        /// Gets or sets the user's logon name or username used to authenticate (sAMAccountName or similar).
        /// </summary>
        /// <value>A <see cref="string"/> representing the user's username/login identifier.</value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the employee's given (first) name.
        /// </summary>
        /// <value>A <see cref="string"/> containing the employee's first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the employee's family (last) name.
        /// </summary>
        /// <value>A <see cref="string"/> containing the employee's last name (surname).</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the employee's email address.
        /// </summary>
        /// <value>A <see cref="string"/> containing the primary email address for the user, if available.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the employee's job title or position within the organization.
        /// </summary>
        /// <value>A <see cref="string"/> describing the employee's job title (for example, "Software Engineer").</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the account or employment status for the employee.
        /// </summary>
        /// <value>A <see cref="string"/> indicating status such as "Active", "Disabled", "Terminated", or other organization-defined values.</value>
        public string Status { get; set; }
    }
}
