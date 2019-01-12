using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.Snapshot;
using System;
using System.Threading.Tasks;

namespace CubeQuest.Account
{
    public class SnapshotManager
    {
        private readonly GoogleApiClient googleClient;

        private const string CurrentSaveName = "CubeQuestSave";

        public Intent SelectSnapshotIntent => 
	        GamesClass.Snapshots.GetSelectSnapshotIntent(googleClient, "Select Save", false, true, 10);

        public SnapshotManager(GoogleApiClient client) => 
	        googleClient = client;
        
        public async Task<ISnapshot> LoadSnapshotAsync()
        {
            var result = await OpenSnapshotAsync();
            
            switch (result.Status.StatusCode)
            {
                case GamesStatusCodes.StatusSnapshotNotFound:
                case GamesStatusCodes.StatusSnapshotContentsUnavailable:
                case GamesStatusCodes.StatusSnapshotFolderUnavailable:
                    return null;

                default:
                    return result.Snapshot;
            }
        }
        
        private async Task<ISnapshotsOpenSnapshotResult> OpenSnapshotAsync() => 
	        await GamesClass.Snapshots.OpenAsync(googleClient, CurrentSaveName, true);
        
        public async void SaveSnapshotAsync(byte[] data)
        {
	        // Try to open snapshot
	        var result = await OpenSnapshotAsync();

	        // Try to write it
			if (result.Snapshot != null)
				await CommitSnapshotAsync(result.Snapshot, data);
		}
		
		private async Task<ISnapshotsCommitSnapshotResult> CommitSnapshotAsync(ISnapshot snapshot, byte[] data)
		{
			// Write data to snapshot
			snapshot.SnapshotContents.WriteBytes(data);

			// Save snapshot
			var metadata = new SnapshotMetadataChangeBuilder()
				.SetDescription($"Modified at {DateTime.Now}")
				.Build();

			return await GamesClass.Snapshots.CommitAndCloseAsync(googleClient, snapshot, metadata);
		}
    }
}