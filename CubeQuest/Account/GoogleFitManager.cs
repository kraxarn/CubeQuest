using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Android.Gms.Fitness.Result;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataType = Android.Gms.Fitness.Data.DataType;
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

        public bool IsConnected => googleClient.IsConnected;

		public GoogleFitManager(Context context, Api.IApiOptionsHasOptions options)
        {
            var connectionListener = new GoogleConnectionListener();
			
            googleClient = new GoogleApiClient.Builder(context)
	            .AddConnectionCallbacks(connectionListener)
				.AddApi(Auth.GOOGLE_SIGN_IN_API, options)
                //.AddApi(FitnessClass.HISTORY_API)
	            .AddApi(FitnessClass.RECORDING_API)
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

            connectionListener.Failed += result => 
	            throw new InvalidOperationException("Google Fit failed");

            googleClient.RegisterConnectionCallbacks(connectionListener);
        }

        public async Task<IList<DataSet>> GetNumSteps(long start, long end)
        {
            var readRequest = new DataReadRequest.Builder()
                .Aggregate(DataType.TypeStepCountDelta, DataType.AggregateStepCountDelta)
                .BucketByTime(31, TimeUnit.Days)
                .SetTimeRange(start, end, TimeUnit.Seconds) // Milliseconds?
                .Build();

            var history = await FitnessClass.HistoryApi.ReadDataAsync(googleClient, readRequest);

            return history.DataSets;
        }

        public async Task<IList<DataSet>> GetNumStepsSinceYesterday() => 
            await GetNumSteps(DateTime.UtcNow - TimeSpan.FromDays(1), DateTime.UtcNow);

		public async Task<IList<DataSet>> GetNumStepsPastMonthAsync() =>
			await GetNumSteps(DateTime.UtcNow - TimeSpan.FromDays(31), DateTime.UtcNow);

		public async Task<IList<DataSet>> GetNumSteps(DateTime start, DateTime end) =>
            await GetNumSteps(start.ToTimestamp(), end.ToTimestamp());

		public async Task<DataReadResult> GetNumStepsExampleAsync()
		{
			return await FitnessClass.HistoryApi.ReadDataAsync(googleClient, QueryFitnessData());
		}

		private DataReadRequest QueryFitnessData()
		{
			var endTime   = DateTime.Now;
			var startTime = endTime.Subtract(TimeSpan.FromDays(7));

			var endTimeElapsed   = endTime.ToMsSinceEpoch();
			var startTimeElapsed = startTime.ToMsSinceEpoch();

			Log.Info("FITNESS_START", startTime.ToString());
			Log.Info("FITNESS_END", endTime.ToString());

			return new DataReadRequest.Builder()
				.Aggregate(DataType.TypeStepCountDelta, DataType.AggregateStepCountDelta)
				.BucketByTime(1, TimeUnit.Days)
				.SetTimeRange(startTimeElapsed, endTimeElapsed, TimeUnit.Milliseconds)
				.Build();
		}

		/// <summary>
		/// Subscribe to Google Fit step counter background service
		/// </summary>
		/// <returns>If successful</returns>
        public async Task<bool> Subscribe()
        {
	        var status = await FitnessClass.RecordingApi.SubscribeAsync(googleClient, DataType.TypeStepCountCumulative);

	        if (status.IsSuccess)
	        {
		        if (status.StatusCode == FitnessStatusCodes.SuccessAlreadySubscribed)
			        Log.Warn("FITNESS", "Tried subscribing, but already subscribed");
		        else
			        Log.Info("FITNESS", "Subscribe successful");
	        }
	        else
		        Log.Error("FITNESS", $"Subscription failed: {CommonStatusCodes.GetStatusCodeString(status.StatusCode)}");

	        return status.IsSuccess;
        }

        public async Task<DataSet> GetDailySteps()
        {
            var result = await FitnessClass.HistoryApi.ReadDailyTotalAsync(googleClient, DataType.TypeStepCountDelta);
            return result.Total;
        }
    }

    public static class FitnessMethodExtensions
    {
	    public static long ToMsSinceEpoch(this DateTime dateTime) =>
		    (long) dateTime.ToUniversalTime()
			    .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
			    .TotalMilliseconds;
    }
}