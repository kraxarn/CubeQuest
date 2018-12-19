using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeUnit = Java.Util.Concurrent.TimeUnit;

namespace CubeQuest.Account
{
    public class GoogleFitManager
    {
        public delegate void SuccessEvent(Statuses status);

        /// <summary>
        /// Login was successful
        /// </summary>
        public event SuccessEvent Success;

        public delegate void FailureEvent(Statuses status);

        /// <summary>
        /// Login failed or canceled
        /// </summary>
        public event FailureEvent Failure;

        private readonly GoogleApiClient googleClient;

        public GoogleFitManager(Context context, Api.IApiOptionsHasOptions options)
        {
            var connectionListener = new GoogleConnectionListener();

            googleClient = new GoogleApiClient.Builder(context)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, options)
                .AddApi(FitnessClass.HISTORY_API)
                .Build();

            googleClient.Connect(GoogleApiClient.SignInModeOptional);

            connectionListener.Connected += async hint =>
            {
                var silent = await Auth.GoogleSignInApi.SilentSignIn(googleClient);

                if (silent.Status.IsSuccess)
                    Success?.Invoke(silent.Status);
                else
                    Failure?.Invoke(silent.Status);
            };

            googleClient.RegisterConnectionCallbacks(connectionListener);
        }

        public async Task<IList<DataSet>> GetNumSteps(long start, long end)
        {
            var readRequest = new DataReadRequest.Builder()
                .Aggregate(DataType.TypeStepCountDelta, DataType.AggregateStepCountDelta)
                .BucketByTime(1, TimeUnit.Days)
                .SetTimeRange(start, end, TimeUnit.Seconds)
                .Build();

            var history = await FitnessClass.HistoryApi.ReadDataAsync(googleClient, readRequest);

            return history.DataSets;
        }

        public async Task<IList<DataSet>> GetNumStepsSinceYesterday() => 
            await GetNumSteps(DateTime.UtcNow - TimeSpan.FromDays(1), DateTime.UtcNow);

        public async Task<IList<DataSet>> GetNumSteps(DateTime start, DateTime end) =>
            await GetNumSteps(start.ToTimestamp(), end.ToTimestamp());

        public async void SubscribeToSteps()
        {
            var result = await FitnessClass.RecordingApi.SubscribeAsync(googleClient, DataType.TypeStepCountCumulative);
        }

        public async Task<DataSet> GetDailySteps()
        {
            var result = await FitnessClass.HistoryApi.ReadDailyTotalAsync(googleClient, DataType.TypeStepCountDelta);
            return result.Total;
        }
    }
}