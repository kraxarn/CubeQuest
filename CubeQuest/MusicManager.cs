using Android.Content;
using Android.Content.Res;
using Android.Media;
using Java.IO;

namespace CubeQuest
{
    public static class MusicManager
    {
        public enum EMusicTrack
        {
            None,
            Title,
            Map
        }

        /// <summary>
        /// Context for creating <see cref="MediaPlayer"/>
        /// </summary>
        private static Context _context;

        /// <summary>
        /// Currently playing track
        /// </summary>
        private static EMusicTrack _playing;

        /// <summary>
        /// Player used to play the music
        /// </summary>
        private static MediaPlayer _player;

        /// <summary>
        /// Private volume used my the media player
        /// </summary>
        private static float _volume;

        /// <summary>
        /// Volume for current track and all future ones (0-1)
        /// </summary>
        public static float Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                _player.SetVolume(value, value);
            }
        }

        public static void Create(Context context)
        {
            _context = context;
            _playing = EMusicTrack.None;
            _volume  = 1f;
        }

        private static bool TryLoadAsset(string path, out AssetFileDescriptor file)
        {
            try
            {
                file = _context.Assets.OpenFd(path);
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
            if (_player?.IsPlaying ?? false)
            {
                _player.Release();
                _player = null;
            }

            // Create basic media player
            _player = new MediaPlayer
            {
                Looping = true
            };

            // Try loading the music file
            if (!TryLoadAsset($"music/{track.ToLower()}.opus", out var file))
                return;

            // Set file, prepare and play
            _player.SetDataSource(file);
            _player.SetVolume(_volume, _volume);
            _player.PrepareAsync();
            _player.Prepared += (sender, args) => _player.Start();
        }

        /// <summary>
        /// Load and play specified track when ready
        /// </summary>
        public static void Play(EMusicTrack track)
        {
            if (track == _playing)
                return;

            if (track == EMusicTrack.None)
            {
                _player.Stop();
                return;
            }

            CreateMediaPlayer(track.ToString());

            _playing = track;
        }

        /// <summary>
        /// Stop playback, same as playing <see cref="EMusicTrack.None"/>
        /// </summary>
        public static void Stop() => Play(EMusicTrack.None);

        /// <summary>
        /// Pauses playback
        /// </summary>
        public static void Pause() => _player.Pause();

        /// <summary>
        /// Resumes playback from where it was paused
        /// </summary>
        public static void Resume() =>  _player.Start();
    }
}