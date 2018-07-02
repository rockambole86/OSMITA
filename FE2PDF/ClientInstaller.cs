using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FE2PDF
{
    [RunInstaller(true)]
    public partial class ClientInstaller : Installer
    {
        public ClientInstaller()
        {
            InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            SetDirectorySecurity();
        }

        private void SetDirectorySecurity()
        {
            var targetdir = Path.GetDirectoryName(Context.Parameters["targetdir"]);

            Log("Adding modify rights for " + targetdir);
            // The following code was copied (and modified) from "Jaso" (http://www.aspnet-answers.com/microsoft/NET-Security/30001760/how-to-change-group-permissions-on-existing-folder.aspx).
            // Retrieve the Directory Security descriptor for the directory
            var dSecurity = Directory.GetAccessControl(targetdir, AccessControlSections.Access);
            // Build a temp domainSID using the Null SID passed in as a SDDL string.
            // The constructor will accept the traditional notation or the SDDL notation interchangeably.
            var domainSid = new SecurityIdentifier("S-1-0-0");
            // Create a security Identifier for the BuiltinUsers Group to be passed to the new accessrule
            var ident = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, domainSid);
            // Create a new Access Rule.
            // ContainerInherit AND ObjectInherit covers both target folder, child folder and child object.
            // However, when using both (combined with AND), the permissions aren't applied.
            // So use two rules.
            // Propagate.none means child and grandchild objects inherit.
            var accessRule1 = new FileSystemAccessRule(ident, FileSystemRights.Modify, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
            var accessRule2 = new FileSystemAccessRule(ident, FileSystemRights.Modify, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow);
            // Add the access rules to the Directory Security Descriptor
            dSecurity.AddAccessRule(accessRule1);
            dSecurity.AddAccessRule(accessRule2);
            // Persist the Directory Security Descriptor to the directory
            Directory.SetAccessControl(targetdir, dSecurity);
            Log("Rights added.");
        }
         
        private void Log(string message)
        {
            Context.LogMessage(message);
            System.Diagnostics.Trace.WriteLine(message, "1177 client installer");
        }
    }
}
