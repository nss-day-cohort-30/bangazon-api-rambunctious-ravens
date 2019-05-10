using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int PaymentTypeId { get; set; }
    }
}


//CREATE TABLE[Order] (
//   Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
//   CustomerId INTEGER NOT NULL,
//	PaymentTypeId INTEGER,
//    CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
//    CONSTRAINT FK_Order_Payment FOREIGN KEY(PaymentTypeId) REFERENCES PaymentType(Id)
//);
