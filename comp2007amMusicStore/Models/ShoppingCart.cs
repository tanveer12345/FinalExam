using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace comp2007amMusicStore.Models
{
    public class ShoppingCart
    {
        // db connection
        private MusicStoreModel db = new MusicStoreModel();

        // identify the user's cart id with their username or a session-generated value
        string CartId { get; set; }

        // add item to cart
        public void AddToCart(int AlbumId)
        {
            var cartItem = new Cart
            {
                AlbumId = AlbumId,
                Count = 1,
                CartId = "CartIdTest",
                DateCreated = DateTime.Now
            };

            db.Carts.Add(cartItem);
            db.SaveChanges();
        }
    }
}