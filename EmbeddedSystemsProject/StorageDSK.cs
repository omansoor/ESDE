using System;

using System.Windows;

using System.IO.IsolatedStorage;
using System.IO;

namespace EmbeddedSystemsProject
{
    public class StorageDSK
    {
        public static void store(string str)
        {
            // Obtain the virtual store for the application.
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

            // Create a new folder and call it "MyFolder".
            if (!myStore.DirectoryExists("valDir"))
                myStore.CreateDirectory("valDir");


            // Specify the file path and options.
            using (var isoFileStream = new IsolatedStorageFileStream("valDir\\valFile", FileMode.OpenOrCreate, myStore))
            {
                //Write the data
                using (var isoFileWriter = new StreamWriter(isoFileStream))
                {
                    isoFileWriter.WriteLine(str);
                }
            }
        }


        public static string read()
        {
            // Obtain a virtual store for the application.
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (myStore.FileExists("valDir\\valFile"))
            {
                try
                {
                    // Specify the file path and options.
                    using (var isoFileStream = new IsolatedStorageFileStream("valDir\\valFile", FileMode.Open, myStore))
                    {
                        // Read the data.
                        using (var isoFileReader = new StreamReader(isoFileStream))
                        {
                            return isoFileReader.ReadLine();
                        }
                    }

                }
                catch
                {
                    // Handle the case when the user attempts to click the Read button first.
                    Logg.Log("Something went wrong while saving the values", true);
                    return "";
                }
            }
            else return null;
        }
    }
}
