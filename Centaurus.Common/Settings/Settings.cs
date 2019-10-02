﻿using CommandLine;
using Newtonsoft.Json;
using stellar_dotnet_sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Centaurus
{
    public abstract class BaseSettings
    {
        public const string ConfigFileArgName = "configFile";

        public KeyPair KeyPair { get; set; }


        [Option(ConfigFileArgName, Required = false, HelpText = "Config file path.")]
        public string ConfigFile { get; set; }

        [Option('s', "secret", Required = true, HelpText = "Current application secret key.")]
        public string Secret { get; set; }

        [Option("cwd", Default = "AppData", Required = false, HelpText = "Working directory for snapshots and other files.")]
        public string CWD { get; set; }

        [Option("network_passphrase", Required = true, HelpText = "Stellar network passphrase.")]
        public string NetworkPassphrase { get; set; }

        [Option("horizon_url", Required = true, HelpText = "URL of Stellar horizon.")]
        public string HorizonUrl { get; set; }

        public virtual void Build()
        {
            KeyPair = KeyPair.FromSecretSeed(Secret);
        }
    }

    public class AuditorSettings : BaseSettings
    {
        [Option("alpha_address", Required = true, HelpText = "URL of Alpha server.")]
        public string AlphaAddress { get; set; }

        [Option("alpha_pubkey", Required = true, HelpText = "Alpha server public key.")]
        public string AlphaPubKey { get; set; }

        [Option("genesis_quorum", Separator = ',', HelpText = "Public keys of all auditors in genesis quorum, separated by comma.")]
        public IEnumerable<string> GenesisQuorum { get; set; }

        public KeyPair AlphaKeyPair { get; set; }

        public override void Build()
        {
            base.Build();
            AlphaKeyPair = KeyPair.FromAccountId(AlphaPubKey);
        }
    }

    public class AlphaSettings : BaseSettings
    { }
}