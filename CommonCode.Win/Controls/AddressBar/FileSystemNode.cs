using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ZidUtilities.CommonCode.Win.Controls.AddressBar
{

    public class FileSystemNode : IAddressNode
    {
        #region Class Variables

        /// <summary>
        /// Stores the parent node to this node
        /// </summary>
        private IAddressNode parent = null;

        /// <summary>
        /// Stores the display name of this Node
        /// </summary>
        private String szDisplayName = null;

        /// <summary>
        /// Stores the full path to this node (Unique ID)
        /// </summary>
        private String fullPath = null;

        /// <summary>
        /// Stores the Icon for this node
        /// </summary>
        private Icon icon = null;

        /// <summary>
        /// Stores the child nodes
        /// </summary>
        private IAddressNode[] children = null;

        /// <summary>
        /// Stores user defined data for this node
        /// </summary>
        private Object tag = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/Sets the parent node to this node
        /// </summary>
        public IAddressNode Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        /// <summary>
        /// Gets/Sets the Display name of this node
        /// </summary>
        public String DisplayName
        {
            get{return this.szDisplayName;}
            set { this.szDisplayName = value; }
        }

        /// <summary>
        /// Gets the Icon that represents this node type.
        /// </summary>
        public Icon Icon
        {
            get { return this.icon; }
        }

        /// <summary>
        /// Returns the Unique Id for this node
        /// </summary>
        public Object UniqueID
        {
            get { return this.fullPath; }
        }

        /// <summary>
        /// Gets/Sets user defined data for this object
        /// </summary>
        public Object Tag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }

        /// <summary>
        /// Gets the children of this node
        /// </summary>
        public IAddressNode[] Children
        {
            get { return this.children; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Basic Constructor, initializes this node to start at the root of the first drive found on the disk.
        /// </summary>
        /// <remarks>
        /// Only use this constructor for root nodes representing "My Computer".
        /// This constructor will populate the immediate child drive entries.
        /// </remarks>
        public FileSystemNode()
        {
            GenerateRootNode();
        }

        /// <summary>
        /// Creates a File System node with a given path.
        /// </summary>
        /// <param name="path">Path that this node represents. May be an empty string to represent the root node.</param>
        /// <param name="parent">Parent FileSystemNode instance for this node. May be null for a root node.</param>
        /// <remarks>
        /// The constructor stores the path and parent, then calls GenerateNodeDisplayDetails to populate the icon and display name.
        /// </remarks>
        public FileSystemNode(string path, FileSystemNode parent)
        {
            //fill in the relevant details
            fullPath = path;
            this.parent = parent;

            //get the icon
            GenerateNodeDisplayDetails();
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Finalizer for FileSystemNode.
        /// </summary>
        /// <remarks>
        /// Cleans up managed icon resources if present and clears children references.
        /// Note: Finalizers are non-deterministic; call Dispose on contained icons if deterministic cleanup is required.
        /// </remarks>
        ~FileSystemNode()
        {
            if (children != null)
            {
                for (int i = 0; i < this.children.Length; i++)
                    this.children.SetValue(null, i);

                this.children = null;
            }

            if(icon != null)
                this.icon.Dispose();

            this.icon = null;
        }

        #endregion

        #region Node Update

        /// <summary>
        /// Updates the contents of this node by enumerating subdirectories.
        /// </summary>
        /// <remarks>
        /// If this node's children have not been allocated, this method will call Directory.GetDirectories on this.fullPath
        /// and create child FileSystemNode instances for each subdirectory.
        /// Any exceptions thrown during enumeration are caught and written to standard error.
        /// This method does not refresh existing children if they are already allocated.
        /// </remarks>
        public void UpdateNode()
        {
            try
            {
                //if we have not allocated our children yet
                if (children == null)
                {
                    //get sub-folders for this folder
                    Array subFolders = System.IO.Directory.GetDirectories(fullPath);

                    //create space for the children
                    children = new FileSystemNode[subFolders.Length];

                    for (int i = 0; i < subFolders.Length; i++)
                    {
                        //create the child value
                        children[i] = new FileSystemNode(subFolders.GetValue(i).ToString(), this);
                    }
                }
            }
            /**
            * This is just a sample, so has bad error handling ;)
            * 
            **/
            catch (System.Exception ioex)
            {
                //write a message to stderr
                System.Console.Error.WriteLine(ioex.Message);
            }
        }

        #endregion

        #region General

        /// <summary>
        /// Returns an individual child node, based on a given unique ID.
        /// </summary>
        /// <param name="uniqueID">Unique Object to identify the child. Usually this is the child's full path string.</param>
        /// <param name="recursive">Indicates whether to perform a recursive search through descendants. This sample implementation does not support recursion.</param>
        /// <returns>
        /// Returns a matching child node if found; otherwise returns null.
        /// If <paramref name="recursive"/> is true, this implementation returns null (not implemented).
        /// </returns>
        public IAddressNode GetChild(object uniqueID, bool recursive)
        {
            //sample version doesn't support recursive search ;
            if(recursive)
                return null;

            foreach(IAddressNode node in this.children)
            {
                if (node.UniqueID.ToString() == uniqueID.ToString())
                    return node;
            }

            return null;
        }

        /// <summary>
        /// Creates a clone of this node.
        /// </summary>
        /// <returns>
        /// A cloned IAddressNode instance representing the same path as this node.
        /// If this node represents the root (empty fullPath), a new root FileSystemNode is returned.
        /// Otherwise a new FileSystemNode with the same fullPath and parent reference is returned.
        /// </returns>
        public IAddressNode Clone()
        {
            if (this.fullPath.Length == 0)
                return new FileSystemNode();
            else
                return new FileSystemNode(this.fullPath, (FileSystemNode)this.parent);
        }

        /// <summary>
        /// Populates this node as a root node representing "My Computer".
        /// </summary>
        /// <remarks>
        /// This method enumerates logical drives using Environment.GetLogicalDrives and creates child FileSystemNode
        /// instances for each drive. It also calls GenerateNodeDisplayDetails to populate the display name and icon.
        /// If children are already allocated, the method returns immediately.
        /// </remarks>
        private void GenerateRootNode()
        {
            // if we have data, we can't become a root node.
            if (children != null)
                return;

            //get the display name of the first logical drive
            fullPath = "";
            this.parent = null;

            //get our drives
            string[] drives = Environment.GetLogicalDrives();

            //create space for the children
            children = new FileSystemNode[drives.Length];

            for (int i = 0; i < drives.Length; i++)
            {
                //create the child value
                children[i] = new FileSystemNode(drives[i], this);
            }

            //get the icon
            GenerateNodeDisplayDetails();
        }

        /// <summary>
        /// Sets the icon and display name for the current path represented by this node.
        /// </summary>
        /// <remarks>
        /// - If this.fullPath is non-empty, SHGetFileInfo is called with the path to obtain the icon and display name.
        /// - If this.fullPath is empty, SHGetSpecialFolderLocation is used to obtain a PIDL for "My Computer" and the SHGetFileInfo overload for PIDL is used.
        /// - The native icon handle returned in SHFILEINFO.hIcon is converted to a managed Icon and cloned; the native handle is then destroyed.
        /// - This method only sets the icon/display name once (when this.icon is null).
        /// </remarks>
        private void GenerateNodeDisplayDetails()
        {
            //if the path exists
            if (icon == null)
            {
                //needed to get a handle to our icon
                SHFILEINFO shinfo = new SHFILEINFO();

                //If we have an actual path, then we pass a string
                if (fullPath.Length > 0)
                {
                    //get the icon and display name
                    Win32.SHGetFileInfo(fullPath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON | Win32.SHGFI_DISPLAYNAME);
                }
                else
                {
                    //If we get a blank path we assume the root of our file system, so we get a pidl to My "Computer"

                    //Get a pidl to my computer
                    IntPtr tempPidl = System.IntPtr.Zero;
                    Win32.SHGetSpecialFolderLocation(0, Win32.CSIDL_DRIVES, ref tempPidl);

                    //get the icon and display name
                    Win32.SHGetFileInfo(tempPidl, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_PIDL | Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON | Win32.SHGFI_DISPLAYNAME);

                    //free our pidl
                    Marshal.FreeCoTaskMem(tempPidl);
                }

                //create the managed icon
                this.icon =(Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
                this.szDisplayName = shinfo.szDisplayName;

                //dispose of the old icon
                Win32.DestroyIcon(shinfo.hIcon);
            }
        }

        #endregion
    }

    #region Win32 Interop for Icons

    /// <summary>
    /// Holds information about a file returned by SHGetFileInfo.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        /// <summary>
        /// Handle to the icon that represents the file. Caller is responsible for destroying the icon handle when done (DestroyIcon).
        /// </summary>
        public IntPtr hIcon;

        /// <summary>
        /// Index of the icon image within the system image list.
        /// </summary>
        public int iIcon;

        /// <summary>
        /// File attributes.
        /// </summary>
        public uint dwAttributes;

        /// <summary>
        /// Display name for the file.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        /// <summary>
        /// Type name for the file.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    /// <summary>
    /// Helper class containing Win32 constants and P/Invoke declarations used to obtain file icons and special folder locations.
    /// </summary>
    class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_SMALLICON = 0x1;
        public const uint SHGFI_DISPLAYNAME = 0x200;
        public const uint SHGFI_PIDL = 0x8;


        public const uint CSIDL_DRIVES = 0x11;


        /// <summary>
        /// Retrieves information about an object in the file system, given a path.
        /// </summary>
        /// <param name="pszPath">Path to the file or folder.</param>
        /// <param name="dwFileAttributes">File attribute flags (can be 0).</param>
        /// <param name="psfi">Reference to a SHFILEINFO structure that receives the file information.</param>
        /// <param name="cbSizeFileInfo">Size of the SHFILEINFO structure in bytes (use Marshal.SizeOf).</param>
        /// <param name="uFlags">Flags that specify the file information to retrieve (e.g., icon, display name).</param>
        /// <returns>Returns an IntPtr result; historically returns a handle or pointer value (not used in this managed wrapper).</returns>
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath,
                                    uint dwFileAttributes,
                                    ref SHFILEINFO psfi,
                                    uint cbSizeFileInfo,
                                    uint uFlags);

        /// <summary>
        /// Retrieves information about an object in the file system, given a PIDL (pointer to an item ID list).
        /// </summary>
        /// <param name="pidl">Pointer to an item ID list (PIDL) identifying the file system object.</param>
        /// <param name="dwFileAttributes">File attribute flags (can be 0).</param>
        /// <param name="psfi">Reference to a SHFILEINFO structure that receives the file information.</param>
        /// <param name="cbSizeFileInfo">Size of the SHFILEINFO structure in bytes (use Marshal.SizeOf).</param>
        /// <param name="uFlags">Flags that specify the file information to retrieve (e.g., SHGFI_PIDL to indicate pidl parameter).</param>
        /// <returns>Returns an IntPtr result; historically returns a handle or pointer value (not used directly here).</returns>
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(IntPtr pidl,
                                    uint dwFileAttributes,
                                    ref SHFILEINFO psfi,
                                    uint cbSizeFileInfo,
                                    uint uFlags);

        /// <summary>
        /// Retrieves a pointer to an item identifier list (PIDL) that identifies a special folder.
        /// </summary>
        /// <param name="hwndOwner">Window handle of the owner (can be 0).</param>
        /// <param name="nSpecialFolder">CSIDL value that identifies the special folder (e.g., CSIDL_DRIVES).</param>
        /// <param name="pidl">Reference to an IntPtr that receives the PIDL. Caller must free the PIDL using CoTaskMemFree or Marshal.FreeCoTaskMem.</param>
        /// <returns>Returns S_OK (0) on success or a non-zero HRESULT on failure.</returns>
        [DllImport("shell32.dll")]
        public static extern int SHGetSpecialFolderLocation(int hwndOwner,
                                    uint nSpecialFolder,
                                    ref IntPtr pidl);

        /// <summary>
        /// Destroys an icon and frees any associated system resources.
        /// </summary>
        /// <param name="hIcon">Handle to the icon to destroy.</param>
        /// <returns>True if the icon was successfully destroyed; otherwise false.</returns>
        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);
    }

    #endregion
}
