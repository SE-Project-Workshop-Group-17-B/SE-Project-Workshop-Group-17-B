
using Sadna_17_B.DomainLayer.StoreDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;


namespace Sadna_17_B.DomainLayer.Utils
{

    public interface I_informative_class
    {
        
        string info_to_UI(); // return informative string of class's fields for UI use

        string info_to_print(); // return informative string of class's fields for informative printing

    }

   
    public interface I_strategy
    {
        double apply_discount_strategy(double price); // apply specific discount strategy on price

    }

    public interface I_discount
    {
        Mini_Reciept apply_discount(Cart cart); // apply discount on a cart member based on a discount rule  

    }


}




