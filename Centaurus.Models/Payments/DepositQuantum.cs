﻿using System;
using System.Collections.Generic;
using Centaurus.Xdr;

namespace Centaurus.Models
{
    /// <summary>
    /// Quantum created contains aggregated payments provided by <see cref="DepositNotification"/>.
    /// </summary>
    public class DepositQuantum : Quantum
    {
        [XdrField(0)]
        public DepositNotification Source { get; set; }
    }
}