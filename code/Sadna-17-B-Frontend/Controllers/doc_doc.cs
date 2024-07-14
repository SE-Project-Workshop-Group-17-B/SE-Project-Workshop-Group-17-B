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
 * 
 *      ------------------------------------------------------------------------------
 *      
 * * * edit-discount-policy()   -       abstract discount policy edit
 *      
 *      "store id"          :       int
 *      "discount id"       :       int
 *      "edit type"         :       ( add / remove )
 *      "start date"        :       dd/mm/yyyy
 *      "end date"          :       dd/mm/yyyy
 *      
 *      "strategy"          :       ( flat / precentage / membership )
 *      "flat"              :       double
 *      "precentage"        :       double                                                      
 *                          
 *      "relevant type"     :       ( product / category / products / categories / cart)
 *      "relevant factores" :       ( int / string ) | ...                                // for product id, for category string
 *      
 *      "cond type"         :       ( p amount / p price / c amount / c price )
 *      "cond product"      :       int
 *      "cond op"           :       ( == / != / < / > / <= / >= )
 *      "cond price"        :       double
 *      "cond amount"       :       int
 *      "cond date"         :       dd/mm/yyyy
 * 
 * 
 *      ------------------------------------------------------------------------------
 *      
 * * * show-discount-policy()   -       abstract discount policy edit
 *      
 *      "store id"          :       int
 *      "discount id"       :       int
 * 
 * 
 *      ------------------------------------------------------------------------------
 *      
 * * * edit-purchase-policy()   -       abstract discount policy edit
 *      
 *      "store id"          :       int
 *      "edit type"         :       ( add / remove )
 *      "ancestor id"       :       int
 *      "name"              :       string
 *      "rule type"         :       ( and / or / conditional )                              // for product id, for category string
 *      
 *      "cond type"         :       ( p amount / p price / c amount / c price )
 *      "cond product"      :       int
 *      "cond op"           :       ( == / != / < / > / <= / >= )
 *      "cond price"        :       double
 *      "cond amount"       :       int
 *      "cond date"         :       dd/mm/yyyy
 *      
 *      -------------------------------------------------------------------------------
 *      
 * * * * show-purchase-policy()   -       abstract discount policy edit
 *      
 *      "store id"          :       int
 *      "purchase rule id"  :       int
 * 
 * 
 *      ------------------------------------------------------------------------------
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
 * * *  has-roles() / roles()   -       abstract user roles gathering
 *      
 *      "store id"          :       int 
 *      "roles to check"     :      (founder / owner / manager / guest / subscriber / admin
 *      ) | ... 
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