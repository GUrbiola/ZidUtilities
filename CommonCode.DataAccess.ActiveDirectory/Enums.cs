using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.DataAccess.ActiveDirectory
{
    /// <summary>
    /// Template for the function that is supposed to parse the raw value in Active Directory to a meaningful
    /// string value.
    /// </summary>
    /// <param name="rawValue">The raw value obtained from the Active Directory attribute. May be null.</param>
    /// <returns>
    /// A string representation of the parsed value suitable for consumption by the caller.
    /// Implementations should handle null and unexpected types and return an appropriate string (for example, empty string).
    /// </returns>
    public delegate string GetAttributeValue(object rawValue);

    /// <summary>
    /// Represents the type of value stored in an Active Directory attribute.
    /// </summary>
    public enum AdType
    {
        /// <summary>
        /// The AD attribute holds a string value.
        /// </summary>
        String,

        /// <summary>
        /// The AD attribute holds a numeric value.
        /// </summary>  
        Integer,

        /// <summary>
        /// The AD attribute holds a numeric long value that translates to a DateTime.
        /// </summary>
        DateTime,

        /// <summary>
        /// The attribute is UserAccountControl, which contains info about the status of the AD record.
        /// </summary>
        AccountStatus,

        /// <summary>
        /// The attribute is a list of strings (for example `proxyAddresses` or `memberOf`).
        /// </summary>
        StringList,

        /// <summary>
        /// A special/custom processing type handled by custom logic.
        /// </summary>
        Special
    }

    /// <summary>
    /// Enum containing the valid comparison operators for filtering Active Directory attributes.
    /// </summary>
    public enum FilterComparer
    {
        /// <summary>
        /// The equal operator.
        /// </summary>
        Equals,

        /// <summary>
        /// The starts-with operator.
        /// </summary>
        StartsWith,

        /// <summary>
        /// The ends-with operator.
        /// </summary>
        EndsWith,

        /// <summary>
        /// The contains operator.
        /// </summary>
        Contains
    }

    /// <summary>
    /// Enum with the list of Active Directory attributes available to do filtering with the AdManager class.
    /// Each value corresponds to a common AD attribute that can be used in filters.
    /// </summary>
    public enum FilterableAttribute
    {
        /// <summary>
        /// Worker's first name.
        /// </summary>
        FirstName,

        /// <summary>
        /// Worker's last name.
        /// </summary>
        LastName,

        /// <summary>
        /// Worker's display name.
        /// </summary>
        DisplayName,

        /// <summary>
        /// Worker's title.
        /// </summary>
        Title,

        /// <summary>
        /// Worker's file number / Workday employee id.
        /// </summary>
        EmployeeNumber,

        /// <summary>
        /// Worker's manager id.
        /// </summary>
        ManagerId,

        /// <summary>
        /// Worker's manager name.
        /// </summary>
        ManagerName,

        /// <summary>
        /// Worker's department.
        /// </summary>
        Department,

        /// <summary>
        /// Worker's department number.
        /// </summary>
        DepartmentNumber,

        /// <summary>
        /// Worker's location.
        /// </summary>
        Location,

        /// <summary>
        /// Worker's email.
        /// </summary>
        Email,

        /// <summary>
        /// Worker's division.
        /// </summary>
        Division,

        /// <summary>
        /// Worker's user name.
        /// </summary>
        UserName,

        /// <summary>
        /// AD record name (unique within the OU).
        /// </summary>
        Name
    }

    /// <summary>
    /// Enum meant to be used as a way to specify a filter for the status of an account.
    /// </summary>
    public enum FilterAccountStatus
    {
        /// <summary>
        /// Account is active.
        /// </summary>
        Active,

        /// <summary>
        /// Account is disabled.
        /// </summary>
        Disabled,

        /// <summary>
        /// Account is locked.
        /// </summary>
        Locked
    }

    /// <summary>
    /// Enum used to track grouping when building complex filters (logical AND / OR).
    /// </summary>
    internal enum FilterGroupingBy
    {
        /// <summary>
        /// The AND operator grouping.
        /// </summary>
        AndOperator,

        /// <summary>
        /// The OR operator grouping.
        /// </summary>
        OrOperator
    }

    /// <summary>
    /// Flags that control the behavior of the user account. These values map to the
    /// UserAccountControl attribute in Active Directory.
    /// </summary>
    [Flags]
    internal enum UserAccountControl : int
    {
        /// <summary>
        /// The logon script is executed.
        /// </summary>
        SCRIPT = 0x00000001,

        /// <summary>
        /// The user account is disabled.
        /// </summary>
        ACCOUNTDISABLE = 0x00000002,

        /// <summary>
        /// The home directory is required.
        /// </summary>
        HOMEDIR_REQUIRED = 0x00000008,

        /// <summary>
        /// The account is currently locked out.
        /// </summary>
        LOCKOUT = 0x00000010,

        /// <summary>
        /// No password is required.
        /// </summary>
        PASSWD_NOTREQD = 0x00000020,

        /// <summary>
        /// The user cannot change the password.
        /// </summary>
        /// <remarks>
        /// Note: You cannot assign the permission settings of PASSWD_CANT_CHANGE by directly modifying
        /// the UserAccountControl attribute. See documentation for recommended approaches.
        /// </remarks>
        PASSWD_CANT_CHANGE = 0x00000040,

        /// <summary>
        /// The user can send an encrypted password.
        /// </summary>
        ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x00000080,

        /// <summary>
        /// This is an account for users whose primary account is in another domain (local user account).
        /// </summary>
        TEMP_DUPLICATE_ACCOUNT = 0x00000100,

        /// <summary>
        /// This is a default account type that represents a typical user.
        /// </summary>
        NORMAL_ACCOUNT = 0x00000200,

        /// <summary>
        /// Permit to trust account for a system domain that trusts other domains.
        /// </summary>
        INTERDOMAIN_TRUST_ACCOUNT = 0x00000800,

        /// <summary>
        /// Computer account for a computer that is a member of this domain.
        /// </summary>
        WORKSTATION_TRUST_ACCOUNT = 0x00001000,

        /// <summary>
        /// Computer account for a system backup domain controller that is a member of this domain.
        /// </summary>
        SERVER_TRUST_ACCOUNT = 0x00002000,

        /// <summary>
        /// Not used.
        /// </summary>
        Unused1 = 0x00004000,

        /// <summary>
        /// Not used.
        /// </summary>
        Unused2 = 0x00008000,

        /// <summary>
        /// The password for this account will never expire.
        /// </summary>
        DONT_EXPIRE_PASSWD = 0x00010000,

        /// <summary>
        /// This is an MNS logon account.
        /// </summary>
        MNS_LOGON_ACCOUNT = 0x00020000,

        /// <summary>
        /// The user must log on using a smart card.
        /// </summary>
        SMARTCARD_REQUIRED = 0x00040000,

        /// <summary>
        /// The service account is trusted for Kerberos delegation.
        /// </summary>
        TRUSTED_FOR_DELEGATION = 0x00080000,

        /// <summary>
        /// The security context of the user will not be delegated to a service.
        /// </summary>
        NOT_DELEGATED = 0x00100000,

        /// <summary>
        /// Restrict this principal to DES encryption types for keys.
        /// </summary>
        USE_DES_KEY_ONLY = 0x00200000,

        /// <summary>
        /// This account does not require Kerberos pre-authentication for logon.
        /// </summary>
        DONT_REQUIRE_PREAUTH = 0x00400000,

        /// <summary>
        /// The user password has expired.
        /// </summary>
        PASSWORD_EXPIRED = 0x00800000,

        /// <summary>
        /// The account is enabled for delegation to authenticate to remote servers.
        /// </summary>
        TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x01000000,
    }
}
