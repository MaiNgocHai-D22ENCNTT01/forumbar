//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace forumbar.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRODUCT_REVIEWS
    {
        public int IdReview { get; set; }
        public Nullable<int> MaSP { get; set; }
        public Nullable<int> IdUser { get; set; }
        public Nullable<int> Rating { get; set; }
        public string Review { get; set; }
        public System.DateTime ReviewDate { get; set; }
    
        public virtual USER_INFO USER_INFO { get; set; }
        public virtual SANPHAM SANPHAM { get; set; }
    }
}
