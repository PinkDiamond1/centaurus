﻿using Centaurus.Domain;
using Centaurus.Models;
using NLog;
using System.Net.WebSockets;
using static Centaurus.Domain.StateManager;

namespace Centaurus
{
    public class IncomingAuditorConnection : IncomingConnectionBase, IAuditorConnection
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        public IncomingAuditorConnection(ExecutionContext context, KeyPair keyPair, WebSocket webSocket, string ip)
            : base(context, keyPair, webSocket, ip)
        {
            AuditorState = Context.StateManager.GetAuditorState(keyPair);
            quantumWorker = new QuantumSyncWorker(Context, this);
            SendHandshake();
        }

        const int AuditorBufferSize = 50 * 1024 * 1024;

        protected override int inBufferSize => AuditorBufferSize;

        protected override int outBufferSize => AuditorBufferSize;

        private QuantumSyncWorker quantumWorker;

        public ulong CurrentCursor => quantumWorker.CurrentQuantaCursor ?? 0;

        public AuditorState AuditorState { get; }

        public void SetSyncCursor(ulong? quantumCursor, ulong? signaturesCursor)
        {
            logger.Trace($"Connection {PubKeyAddress}, apex cursor reset requested. New quantum cursor {(quantumCursor.HasValue ? quantumCursor.ToString() : "null")}, new result cursor {(signaturesCursor.HasValue ? signaturesCursor.ToString() : "null")}");

            //set new quantum and result cursors
            quantumWorker.SetCursors(quantumCursor, signaturesCursor);
            logger.Trace($"Connection {PubKeyAddress}, cursors reseted.");
        }

        public override void Dispose()
        {
            base.Dispose();
            quantumWorker.Dispose();
        }
    }
}