using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comp2007amMusicStore.Models
{
    public class EFOrdersRepository : IMockOrdersRepository
    {
        // db connection
        private MusicStoreModel db = new MusicStoreModel();

        public IQueryable<Order> Orders { get { return db.Orders; } }

        public void Delete(Order order)
        {
            db.Orders.Remove(order);
            db.SaveChanges();
        }

        public Order Save(Order order)
        {
            // if no id, create a new order
            if (order.OrderId == null)
            {
                db.Orders.Add(order);
            }
            else
            {
                // mark the state of the current object as modified
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();
            return order;
        }
    }
}