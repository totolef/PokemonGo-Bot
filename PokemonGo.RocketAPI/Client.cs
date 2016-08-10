﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.HttpClient;
using PokemonGo.RocketAPI.Login;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;

namespace PokemonGo.RocketAPI
{
    public class Client
    {
        public Rpc.Login Login;
        public Rpc.Player Player;
        public Rpc.Download Download;
        public Rpc.Inventory Inventory;
        public Rpc.Map Map;
        public Rpc.Fort Fort;
        public Rpc.Encounter Encounter;
        public Rpc.Misc Misc;

        public IApiFailureStrategy ApiFailure { get; set; }
        public ISettings Settings { get; }
        public string AuthToken { get; set; }

        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public double CurrentAltitude { get; set; }

        public AuthType AuthType => Settings.AuthType;

        internal string ApiUrl { get; set; }
        internal readonly PokemonHttpClient PokemonHttpClient;
        internal AuthTicket AuthTicket { get; set; }

        internal static WebProxy proxy;

        public void setFailure(IApiFailureStrategy fail)
        {
            ApiFailure = fail;
        }

        public Client(ISettings settings)
        {


            
            Settings = settings;
            LaunchProxy(settings);
            PokemonHttpClient = new PokemonHttpClient();
            Login = new Rpc.Login(this);
            Player = new Rpc.Player(this);
            Download = new Rpc.Download(this);
            Inventory = new Rpc.Inventory(this);
            Map = new Rpc.Map(this);
            Fort = new Rpc.Fort(this);
            Encounter = new Rpc.Encounter(this);
            Misc = new Rpc.Misc(this);

            Player.SetCoordinates(settings.DefaultLatitude, settings.DefaultLongitude, settings.DefaultAltitude);
        }

        private void LaunchProxy(ISettings settings)
        {
            int port;
            if (!settings.UseProxyVerified || string.IsNullOrWhiteSpace(settings.UseProxyHost) || string.IsNullOrWhiteSpace(settings.UseProxyPort.ToString()) || int.TryParse(settings.UseProxyPort.ToString(), out port)) return;
            var proxyString = settings.UseProxyHost.Contains("http://") ? $"{settings.UseProxyHost}:{settings.UseProxyPort}" : $"http://{settings.UseProxyHost}:{settings.UseProxyPort}";
            proxy = new WebProxy(proxyString);

            if (settings.UseProxyAuthentication && !string.IsNullOrWhiteSpace(settings.UseProxyUsername) && !string.IsNullOrWhiteSpace(settings.UseProxyPassword))
              proxy.Credentials = new NetworkCredential(settings.UseProxyUsername, settings.UseProxyPassword);

       }
}
}