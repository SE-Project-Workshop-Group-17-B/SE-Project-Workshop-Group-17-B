/*
 * 
 *      ---------------------------------------------------------------------------------
 *              docs :  store service
 *      ---------------------------------------------------------------------------------
 *      
 *      
 *     ------------------------------------------------------------------------------
 *     
 * * * search-product-by()      -       abstract product searching
 *     
 *     "keyword"            :       string | string | ...      
 *     "store id"           :       int
 *     "category"           :       string
 *     "product rating"     :       double
 *     "product price"      :       double | double
 *     "store rating"       :       double
 * 
 *      ------------------------------------------------------------------------------
 *      
 * * * edit-discount-policy()   -       abstract discount policy edit
 *      
 *      "store id"          :       int
 *      "discount id"       :       int
 *      "discount type"     :       ( add / remove )
 *      "start date"        :       dd/mm/yyyy
 *      "end date"          :       dd/mm/yyyy
 *      
 *      "strategy"          :       ( flat / precentage / membership )
 *      "flat"              :       double
 *      "precentage"        :       double
 *      
 *      "relevant type"     :       ( product / category / products / categories )
 *      "relevant factores" :       ( string / int ) | ...                                // for product id, for category string
 *      
 *      "cond type"         :       ( p amount / p price / c amount / c price )
 *      "cond product"      :       int
 *      "cond op"                   ( == / != / < / > / <= / >= )
 *      "cond factor"       :       double
 *      
 *      -------------------------------------------------------------------------------
 *      
 * * *  edit-product-in-store() -       abstract product editing
 *      
 *      "token"             :       string
 *      "store id"          :       int
 *      "product id"        :       int
 *      "name"              :       string       
 *      "category"          :       string
 *      "description"       :       string
 *      
 *      ------------------------------------------------------------------------------
 * 
 * * *  roles()                 -       abstract user roles gathering
 *      
 *      "token"             :       string
 *      "store id"          :       int
 *      
 *      ------------------------------------------------------------------------------
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */