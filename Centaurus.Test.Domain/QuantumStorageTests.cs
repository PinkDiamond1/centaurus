﻿using Centaurus.Domain;
using Centaurus.Models;
using Centaurus.PaymentProvider;
using Centaurus.PersistentStorage;
using Centaurus.Xdr;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Centaurus.Test
{
    public class QuantumStorageTests
    {
        private ExecutionContext context;

        [SetUp]
        public void Setup()
        {
            EnvironmentHelper.SetTestEnvironmentVariable();
            context = GlobalInitHelper.DefaultAlphaSetup().Result;
        }

        [Test]
        public void QuantumStorageTest()
        {
            var account = context.AccountStorage.GetAll().First();
            var lastApex = context.QuantumHandler.CurrentApex;
            var items = new List<IPersistentModel>();
            for (var i = 0; i < 500_000; i++)
            {
                var quantum = new AccountDataRequestQuantum
                {
                    RequestEnvelope = new AccountDataRequest
                    {
                        Account = account.Pubkey,
                        RequestId = DateTime.UtcNow.Ticks
                    }.CreateEnvelope<MessageEnvelopeSignless>(),
                    Apex = ++lastApex,
                    EffectsProof = new byte[] { },
                    PayloadHash = new byte[] { },
                    PrevHash = new byte[32],
                    Timestamp = DateTime.UtcNow.Ticks
                };

                context.SyncStorage.AddQuantum(quantum.Apex, new SyncQuantaBatchItem { Quantum = quantum });
                items.Add(new QuantumPersistentModel { Apex = quantum.Apex, RawQuantum = XdrConverter.Serialize(quantum), Signatures = new List<SignatureModel>(), TimeStamp = quantum.Timestamp, Effects = new List<AccountEffects>() });
            }

            context.PersistentStorage.SaveBatch(items);

            var random = new Random();
            for (var i = 0ul; i < context.QuantumHandler.CurrentApex;)
            {
                var quanta = context.SyncStorage.GetQuanta(i, random.Next(200, 500));
                foreach (var q in quanta)
                {
                    Assert.AreEqual(((Quantum)q.Quantum).Apex, ++i);
                }
            }
        }
    }
}