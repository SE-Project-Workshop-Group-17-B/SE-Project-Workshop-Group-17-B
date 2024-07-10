
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;


namespace Sadna_17_B.DomainLayer.Utils
{

    public interface informative_class
    {
        
        string info_to_UI(); // return informative string of class's fields for UI use

        string info_to_print(); // return informative string of class's fields for informative printing


    }

}




