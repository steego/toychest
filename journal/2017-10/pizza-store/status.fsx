#load "model.fsx"

open System
open PizzaStore.Model

/////////////////////////////////////////////////////////////////////////////
//                           EXPRESSING RULES                              //
/////////////////////////////////////////////////////////////////////////////

type OrderStatus = { 
    Order: Order
    TimePlaced: DateTime option
    TimeBeginPrepare: DateTime option
    TimeBeginDelivery: DateTime option
    TimeDelivered: DateTime option
    Paid: bool
    CustomerRating: int option
}

let update (orderInfo:OrderStatus) (entry:OrderEventEntry) = begin
    match entry.Event with
    | OrderPlaced(order) -> 
            { orderInfo with 
                  Order = order
                  TimePlaced = Some(entry.Timestamp)
                  Paid = match order.Payment with
                         | CreditCard(_) -> true
                         | CashOnDelivery -> false 
            }
    | BeingPreparedBy(employee) -> 
            { orderInfo with 
                    TimeBeginPrepare = Some(entry.Timestamp) 
            }
    | BeingDeliveredBy(driver) -> 
            { orderInfo with 
                    TimeBeginDelivery = Some(entry.Timestamp) 
            }
    | Enroute(_) ->
            orderInfo  //  We won't bother updating...
    | Delivered(driver) -> 
            { orderInfo with 
                    TimeDelivered = Some(entry.Timestamp) 
            }
    | CustomerFeedback(rating) ->
            { orderInfo with
                    CustomerRating = Some(rating)
            }
end