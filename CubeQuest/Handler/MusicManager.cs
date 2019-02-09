using Android.Content;
using Android.Content.Res;
using Android.Media;
using Android.OS;
using Java.IO;
using System;

namespace CubeQuest.Handler
{
	public static class MusicManager
    {
        public enum EMusicTrack
        {
            None,
            Map,
            Dead,
            Battle
        }

        /// <summary>
        /// Context for creating <see cref="MediaPlayer"/>
        /// </summary>
        private static Context context;

        /// <summary>
        /// Currently playing track
        /// </summary>
        private static EMusicTrack playing;

        /// <summary>
        /// Player used to play the music
        /// </summary>
        private static MediaPlayer player;

        /// <summary>
        /// Private volume used my the media player
        /// </summary>
        private static float volume;

        /// <summary>
        /// Volume for current track and all future ones (0-1)
        /// </summary>
        public static float Volume
        {
            get => volume;
            set
            {
                volume = value;
                player?.SetVolume(value, value);
            }
        }

        public static void Create(Context appContext)
        {
            context = appContext;
            playing = EMusicTrack.None;
            volume  = 1f;
        }

        private static bool TryLoadAsset(string path, out AssetFileDescriptor file)
        {
            try
            {
                file = context.Assets.OpenFd(path);
                return true;
            }
            catch (IOException)
            {
                file = null;
                return false;
            }
        }

        private static void CreateMediaPlayer(string track)
        {
            // If null or already playing, stop
            if (player?.IsPlaying ?? false)
            {
                player.Release();
                player = null;
            }

            // Create basic media player
            player = new MediaPlayer
            {
                Looping = true
            };

            // Randomize map theme
            if (track == "Map")
                track += $"{new Random().Next(3)}";

            // Try loading the music file
            var filePath = $"music/{track.ToLower()}.opus";
			if (!TryLoadAsset(filePath, out var file))
                return;

			// Set file, prepare and play
			if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
				player.SetDataSource(file);
			else
				player.SetDataSource(file.FileDescriptor, file.StartOffset, file.Length);

			player.SetVolume(volume, volume);
            player.PrepareAsync();
            player.Prepared += (sender, args) => player.Start();
        }

        /// <summary>
        /// Load and play specified track when ready
        /// </summary>
        public static void Play(EMusicTrack track)
        {
            if (track == playing)
                return;

            if (track == EMusicTrack.None)
            {
                player.Stop();
                return;
            }

			CreateMediaPlayer(track.ToString());

			playing = track;
        }

        /// <summary>
        /// Stop playback, same as playing <see cref="EMusicTrack.None"/>
        /// </summary>
        public static void Stop() => Play(EMusicTrack.None);

        /// <summary>
        /// Pauses playback
        /// </summary>
        public static void Pause() => player.Pause();

        /// <summary>
        /// Resumes playback from where it was paused
        /// </summary>
        public static void Resume() =>  player.Start();
    }
}