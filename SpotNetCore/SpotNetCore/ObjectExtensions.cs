using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpotNetCore
{
    public static class ObjectExtensions
    {
        public static TResult LetIn<TSource, TResult>(this TSource o, Func<TSource, TResult> f)
        {
            return f(o);
        }
        
        public static async Task<object> LetInAsync<T>(this T o, Func<T, Task<object>> f)
        {
            return await f(o);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> o)
        {
            return o == null || !o.Any();
        }

        //When issuing a command to play whilst the player is already playing it throws a 403.
        public static HttpResponseMessage EnsureSpotifySuccess(this HttpResponseMessage httpResponseMessage) =>
            httpResponseMessage.StatusCode == HttpStatusCode.Forbidden
                ? httpResponseMessage
                : httpResponseMessage.EnsureSuccessStatusCode();
    }
}