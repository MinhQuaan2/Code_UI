using Google.Cloud.Firestore;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CodeUI.Service.Utilities
{
    public static class FirestoreUtils
    {
        public static async void WriteDataToFirestore(int userId, Dictionary<string,object> snippets)
        {
            try
            {
                // Initialize Firestore (if not already initialized)
                FirestoreDb db = FirestoreDb.Create("codeui-node");

                // Reference to a collection
                CollectionReference usersCollection = db.Collection("snippets");

                // Document reference for the user
                DocumentReference userDocRef = usersCollection.Document($"{userId}");

                // Write data to Firestore
                await userDocRef.SetAsync(snippets);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Dictionary<string, object>> GetDataFromFirestore(int userID)
        {
            try { 
                // Initialize Firestore
                FirestoreDb db = FirestoreDb.Create("codeui-node");

                // Reference to a collection
                CollectionReference usersCollection = db.Collection("snippets");

                // Reference to a specific document
                DocumentReference userDocRef = usersCollection.Document($"{userID}");

                // Read data from the document
                DocumentSnapshot snapshot = await userDocRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    // Access the data
                    Dictionary<string, object> userData = snapshot.ToDictionary();
                    // Process the userData as needed

                    return userData;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
