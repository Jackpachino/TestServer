using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace Forex_Update.Models
{
    public class EmailSender
    {
        public string To { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Message { get; set; }

        public string Name { get; set; }


    }
}