﻿using Centaurus.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Centaurus.Domain.Models
{
    public class OrderWrapper
    {
        public OrderWrapper(Order order, Account account)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            Account = account ?? throw new ArgumentNullException(nameof(account));
        }

        public ulong OrderId => Order.OrderId;

        public Order Order { get; }

        public Account Account { get; }

        public OrderWrapper Next { get; set; }

        public OrderWrapper Prev { get; set; }

        public override string ToString()
        {
            var res = $"Apex {OrderId}, amount {Order.Amount}, quote {Order.QuoteAmount}, price {Order.Price}";
            if (Prev != null)
            {
                res += $", prev {Prev.OrderId}";
            }
            if (Next != null)
            {
                res += $", next {Next.OrderId}";
            }
            return res;
        }
    }
}