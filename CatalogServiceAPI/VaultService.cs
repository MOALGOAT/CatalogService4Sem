﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;
using Microsoft.Extensions.Configuration;
using VaultSharp.Core;

namespace CatalogServiceAPI
{
    public class VaultService
    {
        private readonly IVaultClient _vaultClient;

        public VaultService(IConfiguration _config)
        {
            var EndPoint = _config["vaultConnectionString"];
            var httpClientHandler = new HttpClientHandler();

            // Tillad forbindelser til usikre HTTP-endepunkter
            httpClientHandler.ServerCertificateCustomValidationCallback =
                (message, cert, chain, sslPolicyErrors) => true;


            IAuthMethodInfo authMethod = new TokenAuthMethodInfo("00000000-0000-0000-0000-000000000000");

            var vaultClientSettings = new VaultClientSettings(EndPoint, authMethod)
            {
                Namespace = "",
                MyHttpClientProviderFunc = handler => new HttpClient(httpClientHandler)
                {
                    BaseAddress = new Uri(EndPoint)
                }
            };

            _vaultClient = new VaultClient(vaultClientSettings);
        }

        public async Task<string?> GetConnectionStringAsync(string path, string key)
        {
            try
            {
                var secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: path, mountPoint: "secret");

                // Kontroller om secret blev fundet
                if (secret != null && secret.Data != null && secret.Data.Data != null && secret.Data.Data.ContainsKey(key))
                {
                    return secret.Data.Data[key].ToString();
                }
                else
                {
                    throw new Exception($"Secret with key '{key}' was not found in path '{path}'.");
                }
            }
            catch (VaultApiException ex)
            {
                // Håndter fejl fra Vault API
                throw new Exception($"Error retreiving secret from Vault: {ex.Message}");
            }
            catch (Exception ex)
            {
                return null + "fejl ved hentning af conn string";
            }
        }

        public async Task<string?> GetSecretAsync(string path, string key)
        {
            try
            {
                // Forsøg at læse secret fra Vault
                var kv2Secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: path, mountPoint: "secret");

                // Kontroller om secret blev fundet
                if (kv2Secret != null && kv2Secret.Data != null && kv2Secret.Data.Data != null && kv2Secret.Data.Data.ContainsKey(key))
                {
                    return kv2Secret.Data.Data[key].ToString();
                }
                else
                {
                    throw new Exception($"Secret with key '{key}' was not found in path '{path}'.");
                }
            }
            catch (VaultApiException ex)
            {
                // Håndter fejl fra Vault API
                throw new Exception($"Error retreiving secret from Vault: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retreiving secret from Vault: {0}", ex.Message);
                return null;
            }
        }

    }
}