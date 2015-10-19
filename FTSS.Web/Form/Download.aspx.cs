using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using Microsoft.WindowsAzure;

namespace FTSS.Web.Form
{
    public partial class Download : FTSSPageUtil
    {
        protected void Page_Load(object sender, EventArgs e)
        {
                        
        }
          
        protected void cmdCreateContainer_Click(object sender, EventArgs e)
        {
            // Retrieve storage account from connection string.
            // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference("ghg");//mycontainer1234567890");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            //Set quyen public cho anyone
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }

        protected void cmdUpload_Click(object sender, EventArgs e)
        {
            // Retrieve storage account from connection string.
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer1234567890");

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileUpload1.FileName);

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(@Path.GetFileName(FileUpload1.FileName)))  //(@"path\myfile"))
            {
                blockBlob.UploadFromStream(fileStream);
            } 
        }

        protected void cmdListFile_Click(object sender, EventArgs e)
        {
            // Retrieve storage account from connection string.
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the blob client. 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer1234567890");//"photos");

            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    //Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);
                    ListBox1.Items.Add(blob.Uri.ToString());
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;

                    //Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);
                    ListBox1.Items.Add(pageBlob.Uri.ToString());

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;

                    //Console.WriteLine("Directory: {0}", directory.Uri);
                    ListBox1.Items.Add(directory.Uri.ToString());

                }
            }
        }

        protected void cmdDownload_Click(object sender, EventArgs e)
        {
            // Retrieve storage account from connection string.
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer1234567890");

            // Retrieve reference to a blob named "photo1.jpg".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("mypic.jpg");//ListBox1.SelectedValue);

            // Save blob contents to a file.
            using (var fileStream = System.IO.File.OpenWrite(@"D:\\"))
            {
                blockBlob.DownloadToStream(fileStream);
            } 

        }

        protected void cmdDeleteFile_Click(object sender, EventArgs e)
        {
            // Retrieve storage account from connection string.
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer1234567890");

            // Retrieve reference to a blob named "myblob.txt".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myphoto.jpg");

            // Delete the blob.
            blockBlob.Delete(); 
        }

    }
}


//<configuration>
//    <appSettings>
//        <add key="StorageConnectionString" value="DefaultEndpointsProtocol=https;AccountName=[AccountName];AccountKey=[AccountKey" />
//    </appSettings>
//</configuration>