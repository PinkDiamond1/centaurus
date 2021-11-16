﻿using Centaurus.Domain;
using Centaurus.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Centaurus.Domain
{
    public static class Notifier
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Sends the message to the account
        /// </summary>
        /// <param name="account">Target account</param>
        /// <param name="envelope">Message to send</param>
        public static void Notify(this ExecutionContext context, RawPubKey account, MessageEnvelopeBase envelope)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            context.ExtensionsManager.BeforeNotify(account, envelope);
            if (context.IncomingConnectionManager.TryGetConnection(account, out IncomingConnectionBase connection))
                try
                {
                    _ = connection.SendMessage(envelope);
                }
                catch { }
        }

        /// <summary>
        /// Sends the message to all connected auditors
        /// </summary>
        /// <param name="envelope">Message to send</param>
        public static void NotifyAuditors(this ExecutionContext context, MessageEnvelopeBase envelope)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            context.ExtensionsManager.BeforeNotifyAuditors(envelope);
            if (context.RoleManager.ParticipationLevel == CentaurusNodeParticipationLevel.Prime) //if Prime node than send message to all incoming connection
            {
                var auditors = context.IncomingConnectionManager.GetAuditorConnections();
                foreach (var auditor in auditors)
                    try
                    {
                        _ = auditor.SendMessage(envelope);
                    }
                    catch { }
            }
            else //notify all connected auditors
                NotifyConnectedAuditors(context, envelope);
        }

        static void NotifyConnectedAuditors(ExecutionContext context, MessageEnvelopeBase envelope)
        {
            try
            {
                _ = context.OutgoingConnectionManager.SendToAll(envelope);
            }
            catch (Exception exc)
            {
                logger.Error(exc, "Exception occurred during broadcasting message to auditors.");
            }
        }
    }
}