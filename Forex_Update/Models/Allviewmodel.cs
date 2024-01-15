using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Forex_Update.Models
{
    public class AllViewmodel
    {
        public IPagedList<Trading> TradingView { get; set; }
        public IPagedList<Request> DepositView { get; set; }

    }
    public class PromocodeUserViewModel
    {
        public int Id { get; set; }
        public string PromocodeStr { get; set; }
        public string BtcWallet { get; set; }
        public string UserName { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<bool> Active { get; set; }

    }

}