using System;
using System.Drawing;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win.Controls.AddressBar;

namespace TesterWin
{
    /// <summary>
    /// Demonstration form showing how to use GenericNode with the AddressBar control.
    /// This demonstrates both design-time configuration and runtime event-driven navigation.
    /// </summary>
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            SetupDynamicNavigation();
        }

        /// <summary>
        /// Sets up event-driven navigation for the root node
        /// </summary>
        private void SetupDynamicNavigation()
        {
            // Example 1: Event-driven navigation (children loaded on demand)
            rootNode.UpdateChildren += RootNode_UpdateChildren;

            // Example 2: Build the tree structure programmatically
            // This demonstrates building a complete hierarchy upfront
            BuildProgrammaticTree();
        }

        /// <summary>
        /// Event handler that populates children dynamically when needed
        /// </summary>
        private void RootNode_UpdateChildren(object sender, UpdateChildrenEventArgs e)
        {
            GenericNode node = sender as GenericNode;
            if (node == null) return;

            // Example: Populate children based on the parent node
            if (node.UniqueID.ToString() == "Root")
            {
                // Add some dynamic children
                e.Children.Add(new GenericNode("Documents", "docs", SystemIcons.Asterisk));
                e.Children.Add(new GenericNode("Pictures", "pics", SystemIcons.Information));
                e.Children.Add(new GenericNode("Videos", "vids", SystemIcons.Question));
            }
        }

        /// <summary>
        /// Builds a tree structure programmatically to demonstrate
        /// adding nodes at runtime without events
        /// </summary>
        private void BuildProgrammaticTree()
        {
            // Create a second root node for comparison
            GenericNode programmaticRoot = new GenericNode("My Computer", "computer", SystemIcons.Application);
            programmaticRoot.AutoLoadChildren = false; // Disable auto-loading since we're building manually

            // Add drives
            GenericNode cDrive = new GenericNode("C: Drive", "C:", SystemIcons.Shield);
            GenericNode dDrive = new GenericNode("D: Drive", "D:", SystemIcons.Shield);

            programmaticRoot.ChildNodes.Add(cDrive);
            programmaticRoot.ChildNodes.Add(dDrive);

            // Add folders to C: Drive
            GenericNode windows = new GenericNode("Windows", "C:\\Windows", SystemIcons.WinLogo);
            GenericNode programFiles = new GenericNode("Program Files", "C:\\Program Files", SystemIcons.Application);

            cDrive.ChildNodes.Add(windows);
            cDrive.ChildNodes.Add(programFiles);

            // Store the programmatic tree for later use
            // You could switch between rootNode and programmaticRoot
            this.Tag = programmaticRoot;
        }

        private void addressBar1_SelectionChange(object sender, NodeChangedArgs nca)
        {
            // Update the label to show the selected node
            lblSelectedNode.Text = $"Selected: {nca.OUniqueID}";
        }

        private void btnSwitchToDesignTimeTree_Click(object sender, EventArgs e)
        {
            // Switch to the design-time configured tree
            addressBar1.RootNode = rootNode;
            lblTreeType.Text = "Current Tree: Design-Time Configured (with events)";
        }

        private void btnSwitchToProgrammaticTree_Click(object sender, EventArgs e)
        {
            // Switch to the programmatically built tree
            if (this.Tag is GenericNode programmaticRoot)
            {
                addressBar1.RootNode = programmaticRoot;
                lblTreeType.Text = "Current Tree: Programmatically Built";
            }
        }

        private void btnAddNodeToCurrentSelection_Click(object sender, EventArgs e)
        {
            // Demonstrates adding a node to the currently selected node
            if (addressBar1.CurrentNode is GenericNode currentNode)
            {
                string newNodeName = $"New Node {DateTime.Now:HHmmss}";
                GenericNode newNode = new GenericNode(newNodeName, newNodeName, SystemIcons.Information);

                currentNode.ChildNodes.Add(newNode);
                currentNode.Reset(); // Reset so children are refreshed

                MessageBox.Show($"Added '{newNodeName}' to '{currentNode.DisplayName}'", "Node Added");
            }
            else
            {
                MessageBox.Show("Current node is not a GenericNode", "Error");
            }
        }
    }
}
