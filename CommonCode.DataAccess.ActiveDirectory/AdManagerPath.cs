using System.Collections.Generic;


namespace ZidUtilities.CommonCode.DataAccess.ActiveDirectory
{
    /// <summary>
    /// Represents a single Active Directory Organizational Unit (OU) search path used by the AdManager.
    /// </summary>
    /// <remarks>
    /// Instances of this class encapsulate a type label (a pseudo name) for results found under the path
    /// and the sequence of OU folder names that together form the distinguished name (DN) suffix for queries.
    /// The class is a simple value object used to describe where the AdManager should look for user records.
    /// </remarks>
    public class AdManagerPath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdManagerPath"/> class.
        /// </summary>
        /// <param name="type">
        /// The label or pseudo name under which records discovered within this path will be classified.
        /// This is intended to be a short identifier used by calling code to distinguish records coming
        /// from different OU paths.
        /// </param>
        /// <param name="folders">
        /// A variable-length list of OU folder names that make up the path to be searched. The folders
        /// should be provided in order from the nearest OU to the farthest (i.e., left-to-right as they
        /// will appear in the resulting distinguished name when combined with the domain components).
        /// Null or empty strings in this array are ignored; only non-empty folder names are stored.
        /// </param>
        /// <remarks>
        /// The constructor creates and populates an internal list of OU folder names and assigns the
        /// provided <paramref name="type"/>. It does not validate folder name characters beyond checking
        /// for empty or null values.
        /// </remarks>
        public AdManagerPath(string type, params string[] folders)
        {
            this.AdPath = new List<string>();
            foreach (string folder in folders)
            {
                if (!string.IsNullOrEmpty(folder))
                {
                    this.AdPath.Add(folder);
                }
            }
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the pseudo name (type) used to identify records found for this path.
        /// </summary>
        /// <value>
        /// A string label that identifies the logical type or group of directory entries which come from
        /// the OU path represented by this instance. The value may be null or empty if not set.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the internal list of OU folder names that compose the search path.
        /// </summary>
        /// <value>
        /// A list of strings containing the OU segments (in order) which will be used to build the
        /// distinguished name for Active Directory queries. This property is maintained for internal use.
        /// </value>
        private List<string> AdPath { get; set; }

        /// <summary>
        /// Builds and returns the distinguished name (DN) string that represents the OU path for queries.
        /// </summary>
        /// <returns>
        /// A string containing the OU-formatted path concatenated with the domain components.
        /// Example result: "OU=SubUnit,OU=Unit,DC=faradayfuture,DC=com".
        /// The OU segments are prefixed in the order they were provided to the constructor (each as "OU={name},")
        /// and then appended before the fixed domain suffix "DC=faradayfuture,DC=com".
        /// </returns>
        /// <remarks>
        /// Empty or null folder entries stored in <see cref="AdPath"/> are skipped when building the DN.
        /// This method overrides <see cref="object.ToString"/> to provide a string suitable for use as a
        /// search base or distinguished name in Active Directory queries.
        /// </remarks>
        public override string ToString()
        {
            string basePath = "DC=faradayfuture,DC=com";

            foreach (string folder in this.AdPath)
            {
                if (!string.IsNullOrEmpty(folder))
                {
                    basePath = string.Format("OU={0},", folder) + basePath;
                }
            }

            return basePath;
        }

    }
}
