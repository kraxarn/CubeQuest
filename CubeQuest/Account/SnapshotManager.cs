using System.Threading.Tasks;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.Snapshot;
using Android.Runtime;

namespace CubeQuest.Account
{
    public static class SnapshotManager
    {
        private static GoogleApiClient googleClient;

        private const string CurrentSaveName = "CubeQuestSave";

        public static void Create(GoogleApiClient client)
        {
            googleClient = client;
        }

        private static async Task<ISnapshot> GetSnapshot()
        {
            const int policy = Snapshots.ResolutionPolicyMostRecentlyModified;

            // TODO: No idea if this actually works (probably not)
            var x = await GamesClass.Snapshots.Open(googleClient, CurrentSaveName, true, policy);
            return x.JavaCast<ISnapshot>();
        }

        private static async Task<ISnapshotContents> LoadFromSnapshot() =>
            (await GetSnapshot()).SnapshotContents;

        private static async void SaveSnapshot(byte[] data)
        {
            // Set data payload for the snapshot
            var snapshot = await GetSnapshot();
            snapshot.SnapshotContents.WriteBytes(data);

            // Create operation
            var metadataChange = new SnapshotMetadataChangeBuilder()
                .SetCoverImage(AccountManager.SaveIcon)
                .SetDescription("CubeQuest Save File")
                .Build();

            // Commit
            await GamesClass.Snapshots.CommitAndClose(googleClient, snapshot, metadataChange);
        }

        public static byte[] Snapshot
        {
            get => LoadFromSnapshot().Result.ReadFully();
            set => SaveSnapshot(value);
        }
    }
}