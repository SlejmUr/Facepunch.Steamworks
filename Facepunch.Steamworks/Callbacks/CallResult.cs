using Steamworks.Data;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Steamworks
{
	/// <summary>
	/// An awaitable version of a SteamAPICall_t
	/// </summary>
	internal struct CallResult<T> : INotifyCompletion where T : struct, ICallbackData
	{
		SteamAPICall_t call;
		readonly ISteamUtils utils;
		readonly bool server;

		public CallResult( SteamAPICall_t call, bool server )
		{
			this.call = call;
			this.server = server;

			utils = (server ? SteamUtils.InterfaceServer : SteamUtils.InterfaceClient) as ISteamUtils;

			utils ??= SteamUtils.Interface as ISteamUtils;
		}

		/// <summary>
		/// This gets called if IsComplete returned false on the first call.
		/// The Action "continues" the async call. We pass it to the Dispatch
		/// to be called when the callback returns.
		/// </summary>
		public readonly void OnCompleted( Action continuation )
		{
			if (IsCompleted)
				continuation();
			else
				Dispatch.OnCallComplete<T>(call, continuation, server);
		}

		/// <summary>
		/// Gets the result. This is called internally by the async shit.
		/// </summary>
		public readonly T? GetResult()
		{
			bool failed = false;
			if ( !utils.IsAPICallCompleted( call, ref failed ) || failed )
				return null;

			var t = default( T );
			var size = t.DataSize;
			var ptr = Marshal.AllocHGlobal( size );

			try
			{
				if ( !utils.GetAPICallResult( call, ptr, size, (int)t.CallbackType, ref failed ) || failed )
				{
					Dispatch.OnDebugCallback?.Invoke( t.CallbackType, "!GetAPICallResult or failed", server );
					return null;
				}

				Dispatch.OnDebugCallback?.Invoke( t.CallbackType, Dispatch.CallbackToString( t.CallbackType, ptr, size ), server );

				return Marshal.PtrToStructure<T>( ptr );
			}
			finally
			{
				Marshal.FreeHGlobal( ptr );
			}
		}

		/// <summary>
		/// Return true if complete or failed
		/// </summary>
		public readonly bool IsCompleted
		{
			get
			{
				bool failed = false;
				if ( utils.IsAPICallCompleted( call, ref failed ) || failed )
					return true;

				return false;
			}
		}

		/// <summary>
		/// This is what makes this struct awaitable
		/// </summary>
		internal readonly CallResult<T> GetAwaiter()
		{
			return this;
		}
	}
}
