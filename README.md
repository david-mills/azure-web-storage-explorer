azure-web-storage-explorer
==================

Web-based tool for viewing storage, diagnostic and performance information about an azure deployment.

The Azure web explorer is a ASP.NET MVC website that can be used to view/download table storage data.  As it is web based it can be accessed from anywhere.  It also allows you to:

1. View and edit diagnostics information about a deployment (transfer intervals, current performance counters etc)
2. View perforamce counter graphs about a specific deployment.  These graphs are created from the standard azure perf counter tables, and dont require verbose logging to be enabled.

In order to get the solution up and running:

1. In the web.config enter the connection string to the storage account you want to view in the appsetting 'StorageConnectionString'
2. In the web.config enter the names for the roles/websites you want to view diagnostics/performance data for in the 'AvailableRoles' appsetting.  These roles/websites must have their diagnostcis connectionstrings in azure set to the stoage account added in '1'.
3. Make sure any references to Microsoft.Windows.Azure.Diagnostics and Microsoft.Windows.Azure.Diagnostics.StorageUtility are correct.  These dlls can be found in the azure sdk (e.g. C:\Program Files\Microsoft SDKs\Windows Azure\.NET SDK\{version}\bin\plugins\).  The sdk can be found here http://azure.microsoft.com/en-us/downloads/

The explorer was created so that minimal credentials (just storage connection string) are required.  This means if you require the functionality to view/edit performance counters and diagnostics, you will need to enter the current deployment id for the role you wish to view.
