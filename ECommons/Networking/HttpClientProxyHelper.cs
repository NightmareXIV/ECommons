using ECommons.Logging;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Networking;
/// <summary>
/// An easy, effortless way to introduce proxy support to your HTTP client to let an user bypass georestrictions or censorship.<br />
/// Step 1. Store <see cref="ProxySettings"/> instance in your configuration file.<br />
/// Step 2. Call <see cref="ProxySettings.ImGuiDraw"/> or <see cref="ProxySettings.ImGuiDrawNoCollapsingHeader"/> to draw proxy configuration wherever you want.<br />
/// Step 3. When using with <see cref="HttpClient"/>, call <see cref="ApplyProxySettings"/> on it immediately after construction with <see cref="ProxySettings"/> instance stored in your configuration. 
/// </summary>
public static class HttpClientProxyHelper
{
    /// <summary>
    /// Call this method on <see cref="HttpClient"/> with <see cref="ProxySettings"/> instance to apply proxy settings to it. Call before making any requests with your client.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static HttpClient ApplyProxySettings(this HttpClient client, ProxySettings settings)
    {
        try
        {
            client.GetFoP<HttpClientHandler>("_handler").Proxy = settings.UseProxy ? new WebProxy
            {
                Address = new Uri(settings.ProxyAddress),
                BypassProxyOnLocal = settings.BypassLocal,
                UseDefaultCredentials = !settings.UseProxyAuthentication,
                Credentials = settings.UseProxyAuthentication ? new NetworkCredential(settings.ProxyLogin, settings.ProxyPassword) : default,
            } : default;
        }
        catch(Exception e)
        {
            e.Log();
        }
        return client;
    }
}