﻿using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Centaurus.Models;

namespace Centaurus.Domain
{
    public class HandshakeRequestHandler : MessageHandlerBase<OutgoingConnection>
    {
        public HandshakeRequestHandler(ExecutionContext context)
            : base(context)
        {
        }

        public override string SupportedMessageType { get; } = typeof(HandshakeRequest).Name;

        public override ConnectionState[] ValidConnectionStates { get; } = new ConnectionState[] { ConnectionState.Ready };

        public override async Task HandleMessage(OutgoingConnection connection, IncomingMessage message)
        {
            var handshakeRequest = (HandshakeRequest)message.Envelope.Message;

            var quantaCursor = Context.QuantumHandler.LastAddedQuantumApex;
            //if Prime than the node will receive results from auditors
            var resultCursor = Context.RoleManager.ParticipationLevel == CentaurusNodeParticipationLevel.Prime 
                ? ulong.MaxValue 
                : Context.PendingUpdatesManager.LastSavedApex;
            await connection.SendMessage(new AuditorHandshakeResponse
            {
                HandshakeData = handshakeRequest.HandshakeData,
                QuantaCursor = quantaCursor,
                ResultCursor = resultCursor,
                State = Context.StateManager.State
            });

            //after sending auditor handshake the connection becomes ready
            connection.ConnectionState = ConnectionState.Ready;
        }
    }
}