using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2007amMusicStore.Models
{
    // mock repository for unit testing OrdersController methods
    public interface IMockOrdersRepository
    {
        IQueryable<Order> Orders { get; }
        Order Save(Order order);
        void Delete(Order order);
    }
}
