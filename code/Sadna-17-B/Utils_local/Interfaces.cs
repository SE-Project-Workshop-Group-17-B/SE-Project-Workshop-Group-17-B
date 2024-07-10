
using Sadna_17_B.DomainLayer.StoreDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Cart = Sadna_17_B.DomainLayer.User.Cart;


namespace Sadna_17_B.DomainLayer.Utils
{

    public interface I_informative_class
    {
        string info(); // info to ui
    }

   
    public interface I_strategy
    {
        double apply_discount_strategy(double price); // apply specific discount strategy on price

    }

    public interface I_discount
    {
        Mini_Checkout apply_discount(Cart cart); // apply discount on a cart member based on a discount rule  

    }


}




