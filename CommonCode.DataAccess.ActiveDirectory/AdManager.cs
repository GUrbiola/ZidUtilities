using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace ZidUtilities.CommonCode.DataAccess.ActiveDirectory
{
    /// <summary>
    /// Helper class to work with Active Directory.
    /// </summary>
    public class AdManager
    {

        /// <summary>
        /// DNS or NetBIOS domain identifier used for Active Directory operations.
        /// </summary>
        /// <remarks>
        /// This property must be set to a valid domain identifier before calling any methods that target Active Directory.
        /// Many methods in this class create a <see cref="System.DirectoryServices.AccountManagement.PrincipalContext"/>
        /// or perform LDAP binds and use this value as the domain parameter. Typical valid values are:
        /// - The DNS domain name (FQDN), e.g. "example.com"
        /// - The NetBIOS domain name, e.g. "EXAMPLE"
        /// - A specific domain controller host name, e.g. "dc01.example.com"
        ///
        /// If <see cref="Domain"/> is null, empty, or contains an incorrect value, AD operations will likely fail:
        /// - Creation of a <see cref="System.DirectoryServices.AccountManagement.PrincipalContext"/> may throw or fail to authenticate.
        /// - Directory binds or searches may target the wrong domain (or the local machine), producing incorrect results.
        /// - Methods such as credential validation, group membership checks, password changes, account unlock/enable/disable,
        ///   and user creation/movement may throw exceptions or behave unexpectedly.
        ///
        /// Recommendation:
        /// - Ensure <see cref="Domain"/> is assigned a valid value (FQDN is preferred) immediately after constructing the
        ///   `AdManager` instance and before invoking any AD-related methods.
        /// - Consider validating this value at initialization or before first use and surface a clear error if missing/invalid.
        /// </remarks>
        /// <example>
        /// // Typical usage:
        /// AdManager mgr = new AdManager("dc01.example.com");
        /// mgr.Domain = "example.com";
        /// // Now safe to call methods that rely on the domain (e.g. ChangePassword, IsValidCredential)
        /// </example>
        /// <value>Domain name used for AD connections (FQDN, NetBIOS name or DC host name).</value>
        public string Domain { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdManager"/> class.
        /// </summary>
        /// <param name="domainController">
        /// The Domain Controller.
        /// </param>
        public AdManager(string domainController)
        {
            this.DomainController = domainController;
            this.Attributes = new List<AdAttribute>();
            this.Paths = new List<AdManagerPath>();
            this.Filter = AdManagerFilter.CreateFilter();
        }

        /// <summary>
        /// Domain Controller to which it will connect to
        /// </summary>
        public string DomainController { get; set; }

        /// <summary>
        /// Contains the list of attributes that will be pulled from AD on a search.
        /// </summary>
        public List<AdAttribute> Attributes { get; set; }

        /// <summary>
        /// Contains the paths(OUs) where the search will take place.
        /// </summary>
        public List<AdManagerPath> Paths { get; set; }

        /// <summary>
        /// Defines if a filter should be applied when retrieving records from Active Directory
        /// </summary>
        public AdManagerFilter Filter { get; set; }

        public Exception Exception { get; set; }
        public string LastMessage { get; set; }


        /// <summary>
        /// Executes a query against active directory, results are returned in a data table.
        /// This query is defined by the Paths and the filter.
        /// NOTE: To auto calculate manager's employee id, the attribute list has to contain these 3 attributes: manager, employeeNumber and distinguishedName
        /// </summary>
        /// <returns>
        /// Data table with the active directory records found from the query specified.
        /// </returns>
        public DataTable QueryActiveDirectory()
        {
            List<string> attributeList = new List<string>();
            DataTable back = new DataTable();
            bool employeeIds = false, managerIds = false, dns = false;
            string employeeIdName = string.Empty, dnsName = string.Empty, userNameFieldName = string.Empty;
            Exception = null;
            LastMessage = string.Empty;

            foreach (AdAttribute atr in this.Attributes)
            {
                attributeList.Add(atr.AdName);
                switch (atr.Type)
                {
                    case AdType.DateTime:
                        back.Columns.Add(atr.Alias, typeof(DateTime));
                        break;
                    case AdType.Integer:
                        back.Columns.Add(atr.Alias, typeof(int));
                        break;
                    case AdType.String:
                    case AdType.AccountStatus:
                    case AdType.Special:
                        back.Columns.Add(atr.Alias);
                        break;
                }

                if (atr.AdName.Equals("manager", StringComparison.OrdinalIgnoreCase))
                {
                    managerIds = true;
                    back.Columns.Add("ManagerId");
                    back.Columns.Add("ManagerUserName");
                    back.Columns.Add("IsManager");
                }
                else if (atr.AdName.Equals("employeeNumber", StringComparison.OrdinalIgnoreCase))
                {
                    employeeIds = true;
                    employeeIdName = atr.Alias;
                }
                else if (atr.AdName.Equals("distinguishedName", StringComparison.OrdinalIgnoreCase))
                {
                    dns = true;
                    dnsName = atr.Alias;
                    //back.PrimaryKey = new DataColumn[] { back.Columns[back.Columns.Count - 1] };
                }
                else if (atr.AdName.Equals("sAMAccountName", StringComparison.OrdinalIgnoreCase))
                {
                    userNameFieldName = atr.Alias;
                }
            }

            back.Columns.Add("WorkerType");

            try
            {
                foreach (AdManagerPath path in this.Paths)
                {
                    string searchBase = string.Format("LDAP://{0}/{1}", this.DomainController, path);
                    DirectoryEntry searchRoot = new DirectoryEntry(searchBase);

                    // Query Active Directory for all users with an employeeID set
                    DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot, this.Filter.ToString(), attributeList.ToArray());
                    directorySearcher.PageSize = 10000;

                    SearchResultCollection results = directorySearcher.FindAll();
                    foreach (SearchResult sr in results)
                    {
                        DataRow newRow = back.NewRow();
                        foreach (AdAttribute atr in this.Attributes)
                        {
                            if (sr.Properties.Contains(atr.AdName))
                            {
                                switch (atr.Type)
                                {
                                    case AdType.DateTime:
                                        if (!string.IsNullOrEmpty(atr.Evaluate(sr.Properties[atr.AdName][0].ToString())))
                                        {
                                            newRow[atr.Alias] = DateTime.ParseExact(atr.Evaluate(sr.Properties[atr.AdName][0].ToString()), "yyyyMMdd HHmmss", CultureInfo.InvariantCulture);
                                        }

                                        break;
                                    case AdType.Integer:
                                        if (!string.IsNullOrEmpty(atr.Evaluate(sr.Properties[atr.AdName][0].ToString())))
                                        {
                                            newRow[atr.Alias] = int.Parse(atr.Evaluate(sr.Properties[atr.AdName][0].ToString()));
                                        }

                                        break;

                                    // case AdType.String: && case AdType.AccountStatus: && case AdType.Special:
                                    default:
                                        newRow[atr.Alias] = atr.Evaluate(sr.Properties[atr.AdName][0].ToString());
                                        break;
                                }
                            }
                        }

                        newRow["WorkerType"] = path.Type;
                        back.Rows.Add(newRow);
                    }

                    searchRoot.Dispose();
                    directorySearcher.Dispose();
                }
            }
            catch (Exception ex)
            {
                LastMessage = "Error while querying Active Directory: " + ex.Message;
                Exception = ex;
            }

            if (dns && employeeIds && managerIds)
            {
                this.CalculateManagers(employeeIdName, dnsName, userNameFieldName, back);
            }

            if (LastMessage == null || LastMessage.Length == 0)
                LastMessage = "Query executed successfully, " + back.Rows.Count + " records found.";

            return back;
        }

        /// <summary>
        /// Applies a batch of updates to an AD account identified by distinguished name.
        /// </summary>
        /// <param name="dN">Distinguished name of the AD account</param>
        /// <param name="updates">List of updates to apply</param>
        public void AttributeBatchUpdate(string dN, List<AdAttribute> updates)
        {
            string serverPath = "LDAP://" + this.DomainController + "/";
            DirectoryEntry user = new DirectoryEntry(serverPath + dN);

            if (user == null)
            {
                throw new ActiveDirectoryObjectNotFoundException("The requested user with DN:" + dN + ", was not found in the active directory.");
            }

            foreach (AdAttribute atr in updates)
            {
                if (atr.Type == AdType.StringList)
                {
                    //Clear existing values
                    user.Properties[atr.AdName].Clear();
                    //Set new values
                    //object[] buff = update.UpdateListValue.ToArray();
                    user.Properties[atr.AdName].Value = atr.UpdateListValue.ToArray();
                }
                else
                {
                    if (string.IsNullOrEmpty(atr.UpdateValue))
                        user.Properties[atr.AdName].Clear();
                    else
                        user.Properties[atr.AdName].Value = atr.Evaluate(atr.UpdateValue);
                }
            }

            user.CommitChanges();
        }

        /// <summary>
        /// Applies a batch of updates to an AD account
        /// </summary>
        /// <param name="user">Reference to the AD account</param>
        /// <param name="updates">List of updates to apply</param>
        public void AttributeBatchUpdate(ref DirectoryEntry user, List<AdAttribute> updates)
        {
            foreach (AdAttribute atr in updates)
            {
                if (atr.Type == AdType.StringList)
                {
                    //Clear existing values
                    user.Properties[atr.AdName].Clear();
                    //Set new values
                    //object[] buff = update.UpdateListValue.ToArray();
                    user.Properties[atr.AdName].Value = atr.UpdateListValue.ToArray();
                }
                else
                {
                    if (string.IsNullOrEmpty(atr.UpdateValue))
                        user.Properties[atr.AdName].Clear();
                    else
                        user.Properties[atr.AdName].Value = atr.Evaluate(atr.UpdateValue);
                }
            }

            user.CommitChanges();
        }

        /// <summary>
        /// Updates one attribute value on Active Directory
        /// </summary>
        /// <param name="propertyName">Attribute name to be updated</param>
        /// <param name="newPropertyValue">Attribute value</param>
        /// <param name="dN">Distinguished name</param>
        public void UpdateUserProperty(string propertyName, string newPropertyValue, string dN)
        {
            string serverPath = "LDAP://" + this.DomainController + "/";
            DirectoryEntry user = new DirectoryEntry(serverPath + dN);

            if (user == null)
            {
                throw new ActiveDirectoryObjectNotFoundException("The requested user with DN:" + dN + ", was not found in the active directory.");
            }

            if (string.IsNullOrEmpty(newPropertyValue))
            {
                user.Properties[propertyName].Clear();
            }
            else
            {
                user.Properties[propertyName].Value = newPropertyValue;
            }
            user.CommitChanges();
        }

        /// <summary>
        /// Updates one attribute value on Active Directory
        /// </summary>
        /// <param name="propertyName">Attribute name to be updated</param>
        /// <param name="newPropertyValue">Attribute value</param>
        /// <param name="user">Reference to the AD entry</param>
        public void UpdateUserProperty(string propertyName, string newPropertyValue, ref DirectoryEntry user)
        {
            if (string.IsNullOrEmpty(newPropertyValue))
            {
                user.Properties[propertyName].Clear();
            }
            else
            {
                user.Properties[propertyName].Value = newPropertyValue;
            }
        }

        /// <summary>
        /// Updates a single AD attribute on the user identified by distinguished name.
        /// </summary>
        /// <param name="update">Descriptor that contains the AD attribute name, type, and value(s) to apply.</param>
        /// <param name="dN">Distinguished name of the AD object to update.</param>
        public void UpdateUserProperty(AdAttribute update, string dN)
        {
            string serverPath = "LDAP://" + this.DomainController + "/";
            DirectoryEntry user = new DirectoryEntry(serverPath + dN);

            if (user == null)
            {
                throw new ActiveDirectoryObjectNotFoundException("The requested user with DN:" + dN + ", was not found in the active directory.");
            }

            if (update.Type == AdType.StringList)
            {
                //Clear existing values
                user.Properties[update.AdName].Clear();
                //Set new values
                //object[] buff = update.UpdateListValue.ToArray();
                user.Properties[update.AdName].Value = update.UpdateListValue.ToArray();
            }
            else
            {
                if (string.IsNullOrEmpty(update.UpdateValue))
                    user.Properties[update.AdName].Clear();
                else
                    user.Properties[update.AdName].Value = update.Evaluate(update.UpdateValue);
            }

            user.CommitChanges();
        }

        /// <summary>
        /// Updates a single AD attribute on the provided <see cref="DirectoryEntry"/>.
        /// </summary>
        /// <param name="update">Descriptor that contains the AD attribute name, type, and value(s) to apply.</param>
        /// <param name="user">Reference to the AD entry that will be updated.</param>
        public void UpdateUserProperty(AdAttribute update, ref DirectoryEntry user)
        {
            if (update.Type == AdType.StringList)
            {
                //Clear existing values
                user.Properties[update.AdName].Clear();
                //Set new values
                //object[] buff = update.UpdateListValue.ToArray();
                user.Properties[update.AdName].Value = update.UpdateListValue.ToArray();
            }
            else
            {
                if (string.IsNullOrEmpty(update.UpdateValue))
                    user.Properties[update.AdName].Clear();
                else
                    user.Properties[update.AdName].Value = update.Evaluate(update.UpdateValue);
            }
        }

        /// <summary>
        /// Validates credentials against Active Directory
        /// </summary>
        /// <param name="user">User name</param>
        /// <param name="password">The password</param>
        /// <param name="domain">The domain</param>
        /// <returns>True if the credentials are valid, false otherwise</returns>
        public bool IsValidCredential(string user, string password)
        {
            bool isValid = false;

            // create a "principal context" - e.g. your domain (could be machine, too)
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, Domain))
            {
                // validate the credentials
                isValid = pc.ValidateCredentials(user, password);
            }
            return isValid;
        }

        /// <summary>
        /// Checks if a user belongs to an specific AD Group
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="groupName">Name of the AD Group</param>
        /// <param name="domain">The domain</param>
        /// <returns>True if the employee belongs to the group, false otherwise</returns>
        public bool DoesEmployeeBelongsToActiveDirectoryGroup(string userName, string groupName)
        {
            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, Domain))
            {
                // find a user
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, userName);

                // find the group in question
                GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupName);

                if (user == null)
                {
                    throw new ActiveDirectoryObjectNotFoundException("The requested user:" + userName + ", was not found in the active directory.");
                }

                if (group == null)
                {
                    throw new ActiveDirectoryObjectNotFoundException("The requested group:" + groupName + ", was not found in the active directory");
                }

                return user.IsMemberOf(group);
            }
        }

        /// <summary>
        /// Changes the password of an AD account
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">New password</param>
        /// <param name="domain">The domain</param>
        /// <returns>True if the operation is successful, false otherwise</returns>
        public bool ChangePassword(string userName, string password)
        {
            bool back = false;
            using (var ctx = new PrincipalContext(ContextType.Domain, Domain))
            {
                using (var user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, userName))
                {
                    if (user != null)
                    {
                        user.SetPassword(password);
                        back = true;
                    }
                }
            }
            return back;
        }

        /// <summary>
        /// Checks if an account is locked
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="domain">The domain</param>
        /// <returns>True if the account is locked, false otherwise</returns>
        public bool IsAccountLocked(string userName)
        {
            bool back = false;
            using (var ctx = new PrincipalContext(ContextType.Domain, Domain))
            {
                using (var user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, userName))
                {
                    if (user != null)
                    {
                        back = user.IsAccountLockedOut();
                    }
                }
            }
            return back;
        }

        /// <summary>
        /// Unlocks an AD account
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="domain">The domain</param>
        public void UnlockAccount(string userName)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain, Domain))
            {
                using (var user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, userName))
                {
                    if (user != null && user.IsAccountLockedOut())
                    {
                        user.UnlockAccount();
                    }
                }
            }
        }

        /// <summary>
        /// Enables an account in AD
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="domain">The domain</param>
        public bool EnableAccount(string userName)
        {
            try
            {
                using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, Domain))
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principalContext, userName);
                    user.Enabled = true;
                    user.Save();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Disables an account in AD
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="domain">The domain</param>
        public bool DisableAccount(string userName)
        {
            try
            {
                using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, Domain))
                {
                    UserPrincipal aduser = UserPrincipal.FindByIdentity(principalContext, userName);
                    if (aduser != null)
                    {
                        aduser.Enabled = true;
                        aduser.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the list of groups to which the AD account is registered
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="domain">The domain</param>
        /// <returns>List of groups to which the user belongs to</returns>
        public List<Principal> GetUserMembership(string userName)
        {
            List<Principal> back = new List<Principal>();

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, Domain))
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName);
                foreach (Principal group in user.GetGroups())
                {
                    back.Add(group);
                }
            }
            return back;
        }

        /// <summary>
        /// Includes a user into one AD group
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="groupName">Group name</param>
        /// <param name="domain">The domain</param>
        public void AddGroupMembership(string userName, string groupName)
        {
            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, Domain))
            {
                // find a user
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, userName);

                // find the group in question
                GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupName);

                if (user == null)
                {
                    throw new ActiveDirectoryObjectNotFoundException("The requested user:" + userName + ", was not found in the active directory.");
                }

                if (group == null)
                {
                    throw new ActiveDirectoryObjectNotFoundException("The requested group:" + groupName + ", was not found in the active directory");
                }
                group.Members.Add(user);
            }
        }

        /// <summary>
        /// Removes the user from one group
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="groupName">Group name</param>
        /// <param name="domain">The domain</param>
        public void RemoveGroupMembership(string userName, string groupName)
        {
            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, Domain))
            {
                // find a user
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, userName);

                // find the group in question
                GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupName);

                if (user == null)
                {
                    throw new ActiveDirectoryObjectNotFoundException("The requested user:" + userName + ", was not found in the active directory.");
                }

                if (group == null)
                {
                    throw new ActiveDirectoryObjectNotFoundException("The requested group:" + groupName + ", was not found in the active directory");
                }
                group.Members.Remove(user);


            }
        }

        /// <summary>
        /// Gets the current photo posted for the employee in AD.
        /// </summary>
        /// <param name="userName">Common Name (cn) of the employee.</param>
        /// <param name="thumbnailPhoto">
        /// When true, reads from the jpegPhoto attribute; when false, reads from thumbnailPhoto.
        /// </param>
        /// <returns>Image instance or null if not found</returns>
        public Image GetUserPicture(string userName, bool thumbnailPhoto = true)
        {
            using (DirectorySearcher searcher = new DirectorySearcher())
            {
                searcher.Filter = "(&(objectClass=user) (cn=" + userName + "))";
                SearchResult result = searcher.FindOne();

                using (DirectoryEntry user = new DirectoryEntry(result.Path))
                {
                    byte[] data = (thumbnailPhoto ? user.Properties["jpegPhoto"].Value : user.Properties["thumbnailPhoto"].Value) as byte[];

                    if (data != null)
                    {
                        using (MemoryStream s = new MemoryStream(data))
                        {
                            return Bitmap.FromStream(s);
                        }
                    }

                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the current photo posted for the employee in AD by sAMAccountName.
        /// </summary>
        /// <param name="userName">User name in AD (sAMAccountName) of the employee.</param>
        /// <param name="thumbnailPhoto">
        /// When true, reads from the jpegPhoto attribute; when false, reads from thumbnailPhoto.
        /// </param>
        /// <returns>Image instance or null if not found</returns>
        public Image GetUserPictureByUserName(string userName, bool thumbnailPhoto = true)
        {
            using (DirectorySearcher dsSearcher = new DirectorySearcher())
            {
                dsSearcher.Filter = "(&(objectClass=user) (sAMAccountName=" + userName + "))";
                SearchResult result = dsSearcher.FindOne();

                using (DirectoryEntry user = new DirectoryEntry(result.Path))
                {
                    byte[] data = (thumbnailPhoto ? user.Properties["jpegPhoto"].Value : user.Properties["thumbnailPhoto"].Value) as byte[];

                    if (data != null)
                    {
                        using (MemoryStream s = new MemoryStream(data))
                        {
                            return Bitmap.FromStream(s);
                        }
                    }

                    return null;
                }
            }
        }

        /// <summary>
        /// Wrapper around <see cref="AddPictureToUser(string, byte[])"/>.
        /// Finds a user by sAMAccountName (parameter name retained as fileNumber) and uploads the given image as the user's photo.
        /// </summary>
        /// <param name="fileNumber">User's sAMAccountName (historical parameter name).</param>
        /// <param name="photoFileName">Path to the image file that will be uploaded.</param>
        /// <returns>True if the operation is successful; otherwise, false.</returns>
        public bool AddPictureByUserName(string fileNumber, string photoFileName)
        {
            AdManager activeDirectoryManager = new AdManager("faradayfuture.com");
            activeDirectoryManager.Paths.Add(new AdManagerPath("Worker", "FF-Users"));

            activeDirectoryManager.Filter = AdManagerFilter.CreateFilter().FilterBy(FilterableAttribute.UserName, FilterComparer.Equals, fileNumber);
            activeDirectoryManager.Attributes.Add(new AdAttribute("sAMAccountName", "UserName", AdType.String));
            activeDirectoryManager.Attributes.Add(new AdAttribute("employeeNumber", "EmployeeId", AdType.String));
            activeDirectoryManager.Attributes.Add(new AdAttribute("distinguishedName", "DistinguishedName", AdType.String));

            DataTable results = activeDirectoryManager.QueryActiveDirectory();

            if (results != null && results.Rows.Count == 1)
            {
                // Open file
                System.IO.FileStream inFile = new System.IO.FileStream(photoFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                // Retrive Data into a byte array variable
                byte[] binaryData = new byte[inFile.Length];
                int bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);
                inFile.Close();

                string serverPath = "LDAP://" + this.DomainController + "/";
                string buff = results.Rows[0]["distinguishedName"].ToString();

                if (!string.IsNullOrEmpty(buff))
                {
                    return this.AddPictureToUser(buff, binaryData);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Inserts a picture as the photo employee in AD, for a given employee(determined by DN(DistinguishedName))
        /// </summary>
        /// <param name="dN">Distinguished Name, used to uniquely identify the employee</param>
        /// <param name="photoBinaryData">Binary array that contains the raw data of the image</param>
        /// <returns>True if the operations is successful, false otherwise</returns>
        public bool AddPictureToUser(string dN, byte[] photoBinaryData)
        {

            string serverPath = "LDAP://" + this.DomainController + "/";

            if (!string.IsNullOrEmpty(dN))
            {
                DirectoryEntry user = new DirectoryEntry(serverPath + dN);

                user.Properties["jpegPhoto"].Clear();
                user.Properties["jpegPhoto"].Add(photoBinaryData);
                user.Properties["thumbnailPhoto"].Clear();
                user.Properties["thumbnailPhoto"].Add(photoBinaryData);
                try
                {
                    user.CommitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Calculates the fields: "manager id", "manager user name" and "is manager", based on distinguished name and manager
        /// </summary>
        /// <param name="empIdFieldName">Field name of the employee id</param>
        /// <param name="dnsFieldName">field name of the distinguished name</param>
        /// <param name="userNameFieldName">field name of the user name</param>
        /// <param name="tab">Table where the information is stored.</param>
        public void CalculateManagers(string empIdFieldName, string dnsFieldName, string userNameFieldName, DataTable tab)
        {
            Dictionary<string, AdKeyAttributes> dnsKey = new Dictionary<string, AdKeyAttributes>();
            Dictionary<string, string> managers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            //Create dictionary with dns and other 
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                if (!dnsKey.ContainsKey(tab.Rows[i][dnsFieldName].ToString()))
                {
                    dnsKey.Add(
                        tab.Rows[i][dnsFieldName].ToString(),
                        new AdKeyAttributes()
                        {
                            EmployeeId = tab.Rows[i][empIdFieldName].ToString(),
                            DistinguishedName = tab.Rows[i][dnsFieldName].ToString(),
                            UserName = string.IsNullOrEmpty(userNameFieldName) ? string.Empty : tab.Rows[i][userNameFieldName].ToString()
                        });
                }

                if (!managers.ContainsKey(tab.Rows[i]["manager"].ToString()))
                {
                    managers.Add(tab.Rows[i]["manager"].ToString(), tab.Rows[i][empIdFieldName].ToString());
                }
            }

            foreach (DataRow dr in tab.Rows)
            {
                string dn = dr["manager"].ToString();
                if (!string.IsNullOrEmpty(dn) && dnsKey.ContainsKey(dn))
                {
                    dr["ManagerId"] = dnsKey[dn].EmployeeId;
                    dr["ManagerUserName"] = dnsKey[dn].UserName;
                }

                bool isManager = managers.ContainsKey(dr[dnsFieldName].ToString());
                dr["IsManager"] = isManager;
            }

        }

        /// <summary>
        /// Forces a user to change their password at the next logon by setting the pwdLastSet attribute to 0.
        /// </summary>
        /// <param name="userSamAccountName">The sAMAccountName of the target user.</param>
        /// <param name="ldapPath">LDAP bind path (e.g., LDAP://dc=example,dc=com).</param>
        /// <param name="adminUsername">Administrative username or bind DN used to connect.</param>
        /// <param name="adminPassword">Password for the administrative account.</param>
        public static void ForcePasswordChangeAtNextLogon(string userSamAccountName, string ldapPath, string adminUsername, string adminPassword)
        {
            try
            {
                // Create a DirectoryEntry object for the domain
                using (DirectoryEntry de = new DirectoryEntry(ldapPath, adminUsername, adminPassword))
                {
                    // Create a DirectorySearcher to find the user
                    using (DirectorySearcher ds = new DirectorySearcher(de))
                    {
                        ds.Filter = $"(&(objectClass=user)(sAMAccountName={userSamAccountName}))";
                        SearchResult sr = ds.FindOne();

                        if (sr != null)
                        {
                            // Get the DirectoryEntry for the found user
                            using (DirectoryEntry userEntry = sr.GetDirectoryEntry())
                            {
                                // Set the pwdLastSet attribute to 0 to force password change at next logon
                                userEntry.Properties["pwdLastSet"].Value = 0;
                                userEntry.CommitChanges();
                                Console.WriteLine($"Successfully set 'User must change password at next logon' for user: {userSamAccountName}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"User '{userSamAccountName}' not found.");
                        }
                    }
                }
            }
            catch (DirectoryServicesCOMException ex)
            {
                Console.WriteLine($"LDAP Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Moves an existing user to a specified Organizational Unit (OU).
        /// </summary>
        /// <param name="username">The sAMAccountName of the user to move.</param>
        /// <param name="targetOuDn">The distinguished name of the destination OU.</param>
        /// <param name="domainName">The AD domain name (e.g., example.com).</param>
        public static void MoveUserToOu(string username, string targetOuDn, string domainName)
        {
            try
            {
                // 1. Retrieve the UserPrincipal
                using (PrincipalContext currentContext = new PrincipalContext(ContextType.Domain, domainName))
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(currentContext, IdentityType.SamAccountName, username);

                    if (user != null)
                    {
                        // 2. Create a New PrincipalContext for the Target OU
                        using (PrincipalContext targetContext = new PrincipalContext(ContextType.Domain, domainName, targetOuDn))
                        {
                            // 3. Move the UserPrincipal
                            user.Save(targetContext);
                            Console.WriteLine($"User '{username}' successfully moved to OU: '{targetOuDn}'.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"User '{username}' not found in domain '{domainName}'.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new AD user by copying core attributes and group memberships from an existing user,
        /// then moves the new user to the specified OU.
        /// </summary>
        /// <param name="sourceUserName">sAMAccountName of the source user to copy attributes from.</param>
        /// <param name="newUserName">sAMAccountName for the new user to create.</param>
        /// <param name="newPassword">Initial password for the new user.</param>
        /// <param name="targetOU">Distinguished name of the OU where the new user should be moved.</param>
        public void CopyAdUser(string sourceUserName, string newUserName, string newPassword, string targetOU)
        {
            string curDomain = "faradayfuture.com";

            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
            {
                // 1. Retrieve the Source User
                UserPrincipal sourceUser = UserPrincipal.FindByIdentity(pc, sourceUserName);

                if (sourceUser != null)
                {
                    // 2. Create a New User
                    UserPrincipal newUser = new UserPrincipal(pc, newUserName, newPassword, true); // true for enabled account

                    // 3. Copy Attributes
                    newUser.Name = sourceUser.Name;
                    newUser.SamAccountName = newUserName;
                    newUser.EmailAddress = sourceUser.EmailAddress;
                    newUser.UserPrincipalName = $"{newUserName}@{curDomain}";
                    newUser.DisplayName = sourceUser.DisplayName; // Or construct a new one
                    newUser.GivenName = sourceUser.GivenName;
                    newUser.Surname = sourceUser.Surname;
                    newUser.EmployeeId = sourceUser.EmployeeId;
                    newUser.Description = sourceUser.Description;
                    newUser.UserPrincipalName = $"{newUserName}@{curDomain}"; // Assuming UPN format
                    newUser.Save();

                    // 4. Copy Group Memberships
                    foreach (Principal group in sourceUser.GetGroups())
                    {
                        GroupPrincipal targetGroup = GroupPrincipal.FindByIdentity(pc, group.SamAccountName);
                        if (targetGroup != null)
                        {
                            targetGroup.Members.Add(newUser);
                            targetGroup.Save();
                        }
                    }
                    MoveUserToOu(newUserName, targetOU, curDomain);
                }
                else
                {
                    Console.WriteLine($"Source user '{sourceUserName}' not found.");
                }
            }
        }

        /// <summary>
        /// Gets a basic set of user attributes for a specific user identified by sAMAccountName.
        /// </summary>
        /// <param name="userName">The sAMAccountName of the user.</param>
        /// <returns>A populated <see cref="BasicEmployee"/> if found; otherwise, an empty instance.</returns>
        public BasicEmployee GetBasicEmployee(string userName)
        {
            BasicEmployee emp = new BasicEmployee();
            Attributes = new List<AdAttribute>();
            Attributes.Add(new AdAttribute("employeeId", "EmployeeId"));
            Attributes.Add(new AdAttribute("sAMAccountName", "UserName"));
            Attributes.Add(new AdAttribute("givenName", "FirstName"));
            Attributes.Add(new AdAttribute("sn", "LastName"));
            Attributes.Add(new AdAttribute("mail", "Email"));
            Attributes.Add(new AdAttribute("title", "Title"));
            Attributes.Add(new AdAttribute("dn", "DistinguishedName"));
            Attributes.Add(new AdAttribute("userAccountCOntrol", "Status", AdType.AccountStatus));

            if (Paths == null || Paths.Count == 0)
            {
                Paths = new List<AdManagerPath>();
                Paths.Add(new AdManagerPath("Worker", "FF-Users"));
            }

            Filter.CleanFilter().FilterBy(FilterableAttribute.UserName, FilterComparer.Equals, userName);

            DataTable buff = this.QueryActiveDirectory();
            if (buff != null && buff.Rows.Count > 0)
            {
                emp.EmployeeId = buff.Rows[0]["EmployeeId"].ToString();
                emp.UserName = buff.Rows[0]["UserName"].ToString();
                emp.DistinguishedName = buff.Rows[0]["DistinguishedName"].ToString();
                emp.FirstName = buff.Rows[0]["FirstName"].ToString();
                emp.LastName = buff.Rows[0]["LastName"].ToString();
                emp.Email = buff.Rows[0]["Email"].ToString();
                emp.Title = buff.Rows[0]["Title"].ToString();
                emp.Status = buff.Rows[0]["Status"].ToString();
            }

            return emp;
        }

        /// <summary>
        /// Gets a basic set of user attributes by applying a filter on a specific field.
        /// </summary>
        /// <param name="strValue">The value to match for the specified field.</param>
        /// <param name="fieldFilter">The field on which the filter will be applied.</param>
        /// <returns>A populated <see cref="BasicEmployee"/> if a match is found; otherwise, an empty instance.</returns>
        public BasicEmployee GetBasicEmployeeWithFilter(string strValue, FilterableAttribute fieldFilter)
        {
            BasicEmployee emp = new BasicEmployee();
            Attributes = new List<AdAttribute>();
            Attributes.Add(new AdAttribute("employeeId", "EmployeeId"));
            Attributes.Add(new AdAttribute("sAMAccountName", "UserName"));
            Attributes.Add(new AdAttribute("givenName", "FirstName"));
            Attributes.Add(new AdAttribute("sn", "LastName"));
            Attributes.Add(new AdAttribute("mail", "Email"));
            Attributes.Add(new AdAttribute("title", "Title"));
            Attributes.Add(new AdAttribute("userAccountCOntrol", "Status", AdType.AccountStatus));

            if (Paths == null || Paths.Count == 0)
            {
                Paths = new List<AdManagerPath>();
                Paths.Add(new AdManagerPath("Worker", "FF-Users"));
            }

            Filter.CleanFilter().FilterBy(fieldFilter, FilterComparer.Equals, strValue);

            DataTable buff = this.QueryActiveDirectory();
            if (buff != null && buff.Rows.Count > 0)
            {
                emp.EmployeeId = buff.Rows[0]["EmployeeId"].ToString();
                emp.UserName = buff.Rows[0]["UserName"].ToString();
                emp.FirstName = buff.Rows[0]["FirstName"].ToString();
                emp.LastName = buff.Rows[0]["LastName"].ToString();
                emp.Email = buff.Rows[0]["Email"].ToString();
                emp.Title = buff.Rows[0]["Title"].ToString();
                emp.Status = buff.Rows[0]["Status"].ToString();
            }

            return emp;
        }

        /// <summary>
        /// Gets a collection of basic user attributes for all users in the configured paths.
        /// </summary>
        /// <returns>A list of <see cref="BasicEmployee"/> entries.</returns>
        public List<BasicEmployee> GetBasicEmployees()
        {
            List<BasicEmployee> back = new List<BasicEmployee>();
            Attributes = new List<AdAttribute>();
            Attributes.Add(new AdAttribute("employeeId", "EmployeeId"));
            Attributes.Add(new AdAttribute("sAMAccountName", "UserName"));
            Attributes.Add(new AdAttribute("givenName", "FirstName"));
            Attributes.Add(new AdAttribute("sn", "LastName"));
            Attributes.Add(new AdAttribute("displayName", "DisplayName"));
            Attributes.Add(new AdAttribute("mail", "Email"));
            Attributes.Add(new AdAttribute("title", "Title"));
            Attributes.Add(new AdAttribute("userAccountCOntrol", "Status", AdType.AccountStatus));


            if (Paths == null || Paths.Count == 0)
            {
                Paths = new List<AdManagerPath>();
                Paths.Add(new AdManagerPath("Worker", "FF-Users"));
            }
            Filter.CleanFilter();

            DataTable buff = this.QueryActiveDirectory();
            for (int i = 0; i < buff.Rows.Count; i++)
            {
                BasicEmployee ne = new BasicEmployee();
                ne.EmployeeId = buff.Rows[i]["EmployeeId"].ToString();
                ne.UserName = buff.Rows[i]["UserName"].ToString();
                ne.FirstName = buff.Rows[i]["FirstName"].ToString();
                ne.LastName = buff.Rows[i]["LastName"].ToString();
                ne.Email = buff.Rows[i]["Email"].ToString();
                ne.Title = buff.Rows[i]["Title"].ToString();
                ne.Status = buff.Rows[i]["Status"].ToString();
                back.Add(ne);
            }

            return back;
        }

    }
}
