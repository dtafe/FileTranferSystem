using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTSS.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FTSS.Web.Form
{
    public class BlobManager
    {
        //these variables are used throughout the class
        string ContainerName { get; set; }
        CloudBlobContainer cloudBlobContainer { get; set; }

        public BlobManager()
        {
            string connectString = Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING);
            string containerName = Common.AppSettingKey(Constant.STORAGE_CONTAINER_NAME).ToLower();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectString);
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            cloudBlobContainer = blobClient.GetContainerReference(containerName);
            ContainerName = containerName;
            // Create the container if it doesn't already exist.
            cloudBlobContainer.CreateIfNotExists();
            //Set public anyone
            cloudBlobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }

        public void UploadFromStream(Stream stream, string targetBlobName)
        {
            //reset the stream back to its starting point (no partial saves)
            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(targetBlobName);
            blob.UploadFromStream(stream);
        }

        public Uri GetURi(string targetBlobName)
        {
            //reset the stream back to its starting point (no partial saves)
            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(targetBlobName);
            if (!blob.Exists())
            {
                throw new Exception(Common.GetResourceString("MSG_FILE_DOWNLOAD_FOUND"));
            }   
            return blob.Uri;
            //blob.DownloadToStream(stream);
        }

        internal void RenameBlob(string blobName, string newBlobName)
        {
            CloudBlockBlob blobSource = cloudBlobContainer.GetBlockBlobReference(blobName);
            if (blobSource.Exists())
            {
                CloudBlockBlob blobTarget = cloudBlobContainer.GetBlockBlobReference(newBlobName);
                blobTarget.StartCopyFromBlob(blobSource);
                blobSource.Delete();
            }
        }


        //if the blob is there, delete it 
        //check returning value to see if it was there or not
        internal void DeleteBlob(string blobName)
        {
            CloudBlockBlob blobSource = cloudBlobContainer.GetBlockBlobReference(blobName);
            bool blobExisted = blobSource.DeleteIfExists();
        }

        internal void DeleteBlobDirectory(string blobName)
        {
            var blobs = cloudBlobContainer.ListBlobs(blobName, true);
            foreach (var blob in blobs)
            {
                cloudBlobContainer.GetBlockBlobReference(((CloudBlockBlob)blob).Name).DeleteIfExists();
            }
        }

        public long CopyBlob(string sourceBlobName, string targetBlobName)
        {
            long contentLength = 0;
            CloudBlockBlob sourceBlob = cloudBlobContainer.GetBlockBlobReference(sourceBlobName);
            if (sourceBlob.Exists())
            {
                sourceBlob.FetchAttributes();
                contentLength = sourceBlob.Properties.Length;
                CloudBlockBlob targetBlob = cloudBlobContainer.GetBlockBlobReference(targetBlobName);
                targetBlob.StartCopyFromBlob(sourceBlob);
                bool blobExisted = sourceBlob.DeleteIfExists();
            }
            return contentLength;
        }
        /// <summary>
        /// parse the blob URI to get just the file name of the blob 
        /// after the container. So this will give you /directory1/directory2/filename if it's in a "subfolder"
        /// </summary>
        /// <param name="theUri"></param>
        /// <returns>name of the blob including subfolders (but not container)</returns>
        private string GetFileNameFromBlobURI(Uri theUri, string containerName)
        {
            string theFile = theUri.ToString();
            int dirIndex = theFile.IndexOf(containerName);
            string oneFile = theFile.Substring(dirIndex + containerName.Length + 1,
                theFile.Length - (dirIndex + containerName.Length + 1));
            return oneFile;
        }

        private string GetDirectoryNameFromBlobURI(Uri theUri, string containerName)
        {
            string theFile = theUri.ToString();
            int dirIndex = theFile.IndexOf(containerName);
            string oneFile = theFile.Substring(dirIndex + containerName.Length + 1,
                theFile.Length - (dirIndex + containerName.Length + 1) -1);
            return oneFile;
        }

        internal List<string> GetDirectoryListInContainer()
        {
            List<string> listOBlobs = new List<string>();
            foreach (IListBlobItem blobItem in cloudBlobContainer.ListBlobs(null, false, BlobListingDetails.None))
            {
                //listOBlobs.Add(blobItem.Uri.LocalPath);
               // blobItem.
                string oneFile = GetDirectoryNameFromBlobURI(blobItem.Uri, ContainerName);
                 listOBlobs.Add(oneFile);
            }
            return listOBlobs;
        }

        internal List<string> GetBlobListForRelPath(string relativePath)
        {
            //first, check the slashes and change them if necessary
            //second, remove leading slash if it's there
            relativePath = relativePath.Replace(@"\", @"/");
            if (relativePath.Substring(0, 1) == @"/")
                relativePath = relativePath.Substring(1, relativePath.Length - 1);

            List<string> listOBlobs = new List<string>();
            foreach (IListBlobItem blobItem in
            cloudBlobContainer.ListBlobs(relativePath, true, BlobListingDetails.All))
            {
                string oneFile = GetFileNameFromBlobURI(blobItem.Uri, ContainerName);
                listOBlobs.Add(oneFile);
            }
            return listOBlobs;
        }
    }
}
