using Newtonsoft.Json;

namespace TiaraPro.Server.Models;

public class PaymobIntentionRequest
{

    [JsonProperty("amount")]
    public int Amount { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("payment_methods")]
    public List<object> PaymentMethods { get; set; }
    [JsonProperty("items")]
    public List<Item> Items { get; set; }
    [JsonProperty("billing_data")]
    public BillingData BillingData { get; set; }
    [JsonProperty("customer")]
    public Customer? Customer { get; set; }
    [JsonProperty("extras")]

    public Dictionary<string, object>? Extras { get; set; }

    [JsonProperty("special_reference")]
    public string? SpecialReference { get; set; }


}

public class Item
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("amount")]
    public int Amount { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("quantity")]
    public int Quantity { get; set; }
}

public class BillingData
{
    [JsonProperty("apartment")]
    public string? Apartment { get; set; }
    [JsonProperty("first_name")]
    public string FirstName { get; set; }
    [JsonProperty("last_name")]
    public string LastName { get; set; }
    [JsonProperty("street")]
    public string? Street { get; set; }
    [JsonProperty("building")]
    public string? Building { get; set; }
    [JsonProperty("phone_number")]
    public string PhoneNumber { get; set; }
    [JsonProperty("country")]
    public string? Country { get; set; }
    [JsonProperty("email")]
    public string? Email { get; set; }
    [JsonProperty("floor")]
    public string? Floor { get; set; }
    [JsonProperty("state")]
    public string? State { get; set; }
}

public class Customer
{
    [JsonProperty("first_name")]
    public string? FirstName { get; set; }
    [JsonProperty("last_name")]
    public string? LastName { get; set; }
    [JsonProperty("email")]
    public string? Email { get; set; }
    [JsonProperty("extras")]
    public Dictionary<string, object>? Extras { get; set; }
}
