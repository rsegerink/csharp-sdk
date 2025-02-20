# Pay.nl C# SDK
---

- [Quick start and examples](#usage)

---

This SDK is available as DotNet Assembly. 

With this SDK you will be able to start transactions and retrieve transactions with their status for the Pay.nl payment service provider.

### Usage

Setting the configuration (for example using mvvmlight as dependency resolver):
```c#
SimpleIoc.Default.Register(() => new LoggerFactory().CreateLogger(nameof(Quicktstart)));
SimpleIoc.Default.Register<ISettingsService>(() => new SettingsService("e41f83b246b706291ea9ad798ccfd9f0fee5e0ab", "SL-3490-4320"));
SimpleIoc.Default.Register<IWebProxy>(() => null);
SimpleIoc.Default.Register<IClientService, ClientService>();
SimpleIoc.Default.Register<IUtilityService, UtilityService>();
```

Getting a list of available payment methods, use the Getservice.
```c#
//paymentMethodId: is optional
//The ID of the payment method. Only the payment options linked to the provided payment method ID will be returned if an ID is provided.
//If omitted, all available payment options are returned. Use the following IDs to filter the options:
//1. SMS.
//2. Pay fixed price.
//3. Pay per call.
//4. Pay per transaction
//5. Pay per minute.
PaymentMethodId? paymentMethodId = null;
var transaction = new PAYNLSDK.Transaction(SimpleIoc.Default.GetInstance<IClientService>());
return transaction.GetServiceAsync(paymentMethodId);
```

Starting a transaction:
```c#
var transaction = new PAYNLSDK.Transaction(SimpleIoc.Default.GetInstance<IClientService>());
var request = transaction.CreateTransactionRequest("127.0.0.1", "http://example.org/visitor-return-after-payment");

request.Amount = 7184; // Amount in cents
request.PaymentOptionId = 10; // Payment profile/option
// request.PaymentOptionSubId = 5081; // Set bank id for iDEAL (example)

// Transaction data
request.Transaction = new PAYNLSDK.Objects.TransactionData
{
    Currency = "EUR",
    CostsVat = null,
    OrderExchangeUrl = "https://example.org/exchange.php",
    Description = "TEST PAYMENT",
    ExpireDate = DateTime.Now.AddDays(14)
};

// Optional Stats data
request.StatsData = new PAYNLSDK.Objects.StatsDetails
{
    Info = "your information",
    Tool = "C#.NET",
    Extra1 = "X",
    Extra2 = "Y",
    Extra3 = "Z"
};

// Initialize Salesdata
request.SalesData = new PAYNLSDK.Objects.SalesData
{
    InvoiceDate = DateTime.Now,
    DeliveryDate = DateTime.Now,
    OrderData = new List<PAYNLSDK.Objects.OrderData>()
    {
        // Add products
        new PAYNLSDK.Objects.OrderData("SKU-8489", "Testproduct 1", 2995, "H", 1),
        new PAYNLSDK.Objects.OrderData("SKU-8421", "Testproduct 2", 995, "H", 1),
        new PAYNLSDK.Objects.OrderData("SKU-2359", "Testproduct 3", 2499, "H", 1)
    }
};

// Add shipping
request.SalesData.OrderData.Add(new PAYNLSDK.Objects.OrderData("SHIPPINGCOST", "Shipping of products", 695, "H", 1, "SHIPPING"));

// enduser
request.Enduser = new PAYNLSDK.Objects.EndUser
{
    Language = "NL",
    Initials = "J.",
    Lastname = "Buyer",
    Gender = Gender.Male,
    BirthDate = new DateTime(1991, 1, 23, 0, 0, 0, DateTimeKind.Local),
    PhoneNumber = "0612345678",
    EmailAddress = "email@domain.com",
    BankAccount = "",
    IBAN = "NL08INGB0000000555",
    BIC = "",

    // enduser address
    Address = new PAYNLSDK.Objects.Address
    {
        StreetName = "Streetname",
        StreetNumber = "8",
        ZipCode = "1234AB",
        City = "City",
        CountryCode = "NL"
    },

    // invoice address
    InvoiceAddress = new PAYNLSDK.Objects.Address
    {
        Initials = "J.",
        LastName = "Jansen",
        Gender = Gender.Male,
        StreetName = "InvoiceStreetname",
        StreetNumber = "10",
        ZipCode = "1234BC",
        City = "City",
        CountryCode = "NL"
    }
};

return transaction.StartAsync(request);
```

To determine if a transaction has been paid, you can use:
```c#
var transaction = new PAYNLSDK.Transaction(SimpleIoc.Default.GetInstance<IClientService>());

// Perform transaction to get response object. Alternately, you could work with a stored ID....
var response = new PAYNLSDK.API.Transaction.Start.Response();

var info = await transaction.InfoAsync(response.Transaction.TransactionId);
var result = info.PaymentDetails.State;

if (transaction.IsPaid(result) || transaction.IsPending(result))
{
    // redirect user to thank you page
}
else
{
    // it has not been paid yet, so redirect user back to checkout
}
```

When implementing the exchange script (where you should process the order in your backend):
```c#
var transaction = new PAYNLSDK.Transaction(SimpleIoc.Default.GetInstance<IClientService>());

// Perform transaction to get response object. Alternately, you could work with a stored ID....
var response = new PAYNLSDK.API.Transaction.Start.Response();

var info = await transaction.InfoAsync(response.Transaction.TransactionId);
var result = info.PaymentDetails.State;

if (transaction.IsPaid(result))
{
    // process the payment
}
else
{
    if (transaction.IsCancelled(result))
    {
        // payment canceled, restock items
    }
}

response.Write("TRUE| ");
// Optionally you can send a message after TRUE|, you can view these messages in the logs.
// https://admin.pay.nl/logs/payment_state
response.Write("Paid");
```

### Contributing



### License

The Assembly is available as open source under the terms of the [MIT License](http://opensource.org/licenses/MIT).
