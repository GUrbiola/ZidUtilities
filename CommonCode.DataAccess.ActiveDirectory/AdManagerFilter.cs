using System.Text;


namespace ZidUtilities.CommonCode.DataAccess.ActiveDirectory
{
    /// <summary>
    /// The ad manager filter.
    /// </summary>
    public class AdManagerFilter
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="AdManagerFilter"/> class from being created.
        /// Instances should get created using the method <see cref="CreateFilter"/>.
        /// </summary>
        /// <remarks>
        /// The constructor initializes the internal string builder that accumulates LDAP filter
        /// fragments and sets the initial grouping operator to AND. It is private to enforce the
        /// use of the <see cref="CreateFilter"/> factory method.
        /// </remarks>
        private AdManagerFilter()
        {
            this.CurrentFilter = new StringBuilder();
            this.GroupOperator = FilterGroupingBy.AndOperator;
            this.FirstGroupingChange = true;
        }

        /// <summary>
        /// Property where the filter is built in.
        /// </summary>
        private StringBuilder CurrentFilter { get; set; }

        /// <summary>
        /// Tracks the current grouping operator that is being used.
        /// </summary>
        private FilterGroupingBy GroupOperator { get; set; }

        /// <summary>
        /// Flag to track when the first change from AND to OR happens.
        /// </summary>
        private bool FirstGroupingChange { get; set; }

        /// <summary>
        /// Returns an empty filter instance.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="AdManagerFilter"/> initialized with an empty filter state.
        /// </returns>
        public static AdManagerFilter CreateFilter()
        {
            return new AdManagerFilter();
        }

        /// <summary>
        /// Starts or appends a filter fragment for the specified attribute using the specified comparer and value.
        /// </summary>
        /// <param name="attr">The attribute on which the filter will be applied (from <see cref="FilterableAttribute"/>).</param>
        /// <param name="cmp">The comparison operator to use (from <see cref="FilterComparer"/>).</param>
        /// <param name="value">The value used in the comparison. This value is inserted verbatim into the LDAP filter fragment.</param>
        /// <returns>
        /// The same <see cref="AdManagerFilter"/> instance with the new filter fragment appended to the internal filter buffer,
        /// enabling a fluent chaining style.
        /// </returns>
        public AdManagerFilter FilterBy(FilterableAttribute attr, FilterComparer cmp, string value)
        {
            string core;

            switch (cmp)
            {
                case FilterComparer.StartsWith:
                    core = string.Format("({0}={1}*)", this.GetAttributeName(attr), value);
                    break;
                case FilterComparer.EndsWith:
                    core = string.Format("({0}=*{1})", this.GetAttributeName(attr), value);
                    break;
                case FilterComparer.Contains:
                    core = string.Format("({0}=*{1}*)", this.GetAttributeName(attr), value);
                    break;
                case FilterComparer.Equals:
                default:
                    core = string.Format("({0}={1})", this.GetAttributeName(attr), value);
                    break;
            }

            this.CurrentFilter.Append(core);
            return this;
        }

        /// <summary>
        /// Sets the grouping operator to AND for subsequent appended filter fragments.
        /// </summary>
        /// <remarks>
        /// If the current grouping operator is already AND, this method has no effect. If the grouping
        /// operator was previously OR, this method closes the OR grouping and starts/resumes an AND grouping
        /// by appending the necessary LDAP grouping characters to the internal buffer.
        /// </remarks>
        /// <returns>
        /// The same <see cref="AdManagerFilter"/> instance (fluent interface).
        /// </returns>
        public AdManagerFilter And()
        {
            if (this.GroupOperator != FilterGroupingBy.AndOperator)
            {
                this.CurrentFilter.Append(")($");
                this.GroupOperator = FilterGroupingBy.AndOperator;
            }

            return this;
        }

        /// <summary>
        /// Sets the grouping operator to OR for subsequent appended filter fragments.
        /// </summary>
        /// <remarks>
        /// If the current grouping operator is already OR, this method has no effect. On the first transition
        /// from AND to OR it opens a new OR grouping; on subsequent transitions it closes the previous grouping
        /// and opens a new OR grouping to correctly nest LDAP OR groups.
        /// </remarks>
        /// <returns>
        /// The same <see cref="AdManagerFilter"/> instance (fluent interface).
        /// </returns>
        public AdManagerFilter Or()
        {
            if (this.GroupOperator != FilterGroupingBy.OrOperator)
            {
                if (this.FirstGroupingChange)
                {
                    this.CurrentFilter.Append("(|");
                    this.FirstGroupingChange = false;
                }
                else
                {
                    this.CurrentFilter.Append(")(|");
                }

                this.GroupOperator = FilterGroupingBy.OrOperator;
            }

            return this;
        }

        /// <summary>
        /// Appends an LDAP filter fragment that restricts results by account status.
        /// </summary>
        /// <param name="status">The account status to filter by (from <see cref="FilterAccountStatus"/>).</param>
        /// <returns>
        /// The same <see cref="AdManagerFilter"/> instance with the account status filter appended.
        /// </returns>
        /// <remarks>
        /// Uses a bitwise LDAP match on the <c>userAccountControl</c> attribute with the appropriate
        /// bit flag for Disabled, Locked, or Active accounts.
        /// </remarks>
        public AdManagerFilter AccountIs(FilterAccountStatus status)
        {
            switch (status)
            {
                case FilterAccountStatus.Disabled:
                    this.CurrentFilter.Append("(userAccountControl:1.2.840.113556.1.4.803:=2)");
                    break;
                case FilterAccountStatus.Locked:
                    this.CurrentFilter.Append("(userAccountControl:1.2.840.113556.1.4.803:=16)");
                    break;
                case FilterAccountStatus.Active:
                    this.CurrentFilter.Append("(userAccountControl:1.2.840.113556.1.4.803:=512)");
                    break;
            }

            return this;
        }

        /// <summary>
        /// Clears any filter defined on this instance.
        /// </summary>
        /// <returns>
        /// The same <see cref="AdManagerFilter"/> instance with an empty internal filter buffer.
        /// </returns>
        public AdManagerFilter CleanFilter()
        {
            this.CurrentFilter.Clear();
            return this;
        }

        /// <summary>
        /// Builds and returns the final LDAP filter string expected by <c>DirectorySearcher</c>.
        /// </summary>
        /// <returns>
        /// A fully-formed LDAP filter string representing the combination of the default user filter
        /// elements and any additional filter fragments previously appended to this instance. If no
        /// fragments were appended, a default filter selecting user objects is returned.
        /// </returns>
        public override string ToString()
        {
            string back = string.Empty;
            if (string.IsNullOrEmpty(this.CurrentFilter.ToString()))
            {
                back = "(&(objectCategory=user)(ObjectClass=user))";
            }
            else
            {
                back = string.Format("(&(objectCategory=user)(ObjectClass=user){0})", this.CurrentFilter);
            }

            return back;
        }

        /// <summary>
        /// Maps a <see cref="FilterableAttribute"/> enum value to its corresponding Active Directory attribute name.
        /// </summary>
        /// <param name="attr">The enum value indicating which attribute name to return.</param>
        /// <returns>
        /// The AD attribute name as a <see cref="string"/>. Returns an empty string if the enum value has no mapping.
        /// </returns>
        /// <remarks>
        /// This method centralizes the mapping between developer-friendly enum names and the actual
        /// attribute names used in LDAP filter fragments.
        /// </remarks>
        private string GetAttributeName(FilterableAttribute attr)
        {
            string back = string.Empty;

            switch (attr)
            {
                case FilterableAttribute.FirstName:
                    back = "givenName";
                    break;
                case FilterableAttribute.LastName:
                    back = "sn";
                    break;
                case FilterableAttribute.DisplayName:
                    back = "displayName";
                    break;
                case FilterableAttribute.UserName:
                    back = "sAMAccountName";
                    break;
                case FilterableAttribute.Title:
                    back = "title";
                    break;
                case FilterableAttribute.EmployeeNumber:
                    back = "employeeNumber";
                    break;
                case FilterableAttribute.ManagerId:
                    back = "manager";
                    break;
                case FilterableAttribute.Department:
                    back = "department";
                    break;
                case FilterableAttribute.DepartmentNumber:
                    back = "departmentNumber";
                    break;
                case FilterableAttribute.Location:
                    back = "physicalDeliveryOfficeName";
                    break;
                case FilterableAttribute.Email:
                    back = "mail";
                    break;
                case FilterableAttribute.Division:
                    back = "division";
                    break;
                case FilterableAttribute.Name:
                    back = "name";
                    break;
            }

            return back;
        }
    }
}
