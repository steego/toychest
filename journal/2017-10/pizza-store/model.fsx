module PizzaStore.Model

open System

/////////////////////////////////////////////////////////////////////////////
//                               TEST MODEL                                //
/////////////////////////////////////////////////////////////////////////////

type Customer = { Name: string; Phone: string; Email: string }
type Address = { Street: string; City: string; State: string; Zip: string }
type PaymentType = 
    | CreditCard of CardNumber: string * Expiration: string
    | CashOnDelivery

type GPSLocation = { Latitude: double; Longitude: double }

type StoreLocation = { Code: string; Address: Address }
type Employee = { Name: string }

type OrderItem = { Qty: int; Description: string; Price: decimal }
type Order = { Customer: Customer; StoreLocation: StoreLocation; 
               Address: Address; Payment: PaymentType; Items: OrderItem list }

type OrderEvent = 
    | OrderPlaced of Order
    | BeingPreparedBy of Employee
    | BeingDeliveredBy of Employee
    | Enroute of GPSLocation
    | Delivered of GPSLocation
    | CustomerFeedback of Stars:int

type OrderEventEntry = { Timestamp: DateTime; Event: OrderEvent }


/////////////////////////////////////////////////////////////////////////////
//                              EXAMPLE DATA                               //
/////////////////////////////////////////////////////////////////////////////

let hoboken = { 
    Code = "HOBO"
    Address = { Street = "123 Test Street"; City = "Hoboken"; State = "NJ"; Zip = "07111" }
}

let steveAddress = { 
    Street = "456 Delivery Street"
    City = "Hoboken"
    State = "NJ"
    Zip = "07111" 
}


let sampleOrder = {
    StoreLocation = hoboken
    Customer = {
                    Name = "Steve"
                    Phone = "201-123-4567"
                    Email = "pizzalover@aol.com"
                }
    Address = { 
                    Street = "456 Delivery Street"
                    City = "Hoboken"
                    State = "NJ"
                    Zip = "07111" 
              }
    Payment = CashOnDelivery
    Items = 
    [
        { Qty = 1; Price = 15.99M; Description = "Large Pepperoni Pizza" }
    ]
}

let Event(time, event) = 
    { Timestamp = DateTime.Parse(time); Event = event}

let orderEvents = [
    Event("2017-10-18 4:20 PM", OrderPlaced(sampleOrder))
    Event("2017-10-18 4:21 PM", BeingPreparedBy({ Name = "Tyler" }))
    Event("2017-10-18 4:31 PM", BeingDeliveredBy({ Name = "Sharon" }))
    Event("2017-10-18 4:32 PM", Enroute({ Latitude = 71.233; Longitude = 40.234 }))
    Event("2017-10-18 4:33 PM", Enroute({ Latitude = 71.233; Longitude = 40.234 }))
    Event("2017-10-18 4:34 PM", Delivered({ Latitude = 71.233; Longitude = 40.234 }))
    Event("2017-10-18 4:40 PM", CustomerFeedback(5))
]