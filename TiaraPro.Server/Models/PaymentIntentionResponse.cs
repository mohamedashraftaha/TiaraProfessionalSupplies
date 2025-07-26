using Newtonsoft.Json;

namespace TiaraPro.Server.Models;

public class PaymobIntentionResponse
{
    [JsonProperty("payment_keys")]
    public List<PaymentKey> PaymentKeys { get; set; }

    [JsonProperty("intention_order_id")]
    public long IntentionOrderId { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("intention_detail")]
    public IntentionDetail IntentionDetail { get; set; }

    [JsonProperty("client_secret")]
    public string ClientSecret { get; set; }

    [JsonProperty("payment_methods")]
    public List<PaymentMethod> PaymentMethods { get; set; }

    [JsonProperty("special_reference")]
    public string SpecialReference { get; set; }

    [JsonProperty("extras")]
    public Extras Extras { get; set; }

    [JsonProperty("confirmed")]
    public bool Confirmed { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("created")]
    public DateTime Created { get; set; }

    [JsonProperty("card_detail")]
    public object CardDetail { get; set; }

    [JsonProperty("card_tokens")]
    public List<object> CardTokens { get; set; }

    [JsonProperty("object")]
    public string ObjectType { get; set; }
}

public class PaymentKey
{
    [JsonProperty("integration")]
    public long Integration { get; set; }

    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("gateway_type")]
    public string GatewayType { get; set; }

    [JsonProperty("iframe_id")]
    public object IframeId { get; set; }

    [JsonProperty("order_id")]
    public long OrderId { get; set; }

    [JsonProperty("redirection_url")]
    public string RedirectionUrl { get; set; }

    [JsonProperty("save_card")]
    public bool SaveCard { get; set; }
}

public class IntentionDetail
{
    [JsonProperty("amount")]
    public int Amount { get; set; }

    [JsonProperty("items")]
    public List<Item> Items { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("billing_data")]
    public BillingData BillingData { get; set; }
}

public class ItemResponse
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("amount")]
    public int Amount { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("image")]
    public object Image { get; set; }
}

public class BillingDataResponse
{
    [JsonProperty("apartment")]
    public string Apartment { get; set; }

    [JsonProperty("floor")]
    public string Floor { get; set; }

    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [JsonProperty("last_name")]
    public string LastName { get; set; }

    [JsonProperty("street")]
    public string Street { get; set; }

    [JsonProperty("building")]
    public string Building { get; set; }

    [JsonProperty("phone_number")]
    public string PhoneNumber { get; set; }

    [JsonProperty("shipping_method")]
    public string ShippingMethod { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("postal_code")]
    public string PostalCode { get; set; }
}

public class PaymentMethod
{
    [JsonProperty("integration_id")]
    public long IntegrationId { get; set; }

    [JsonProperty("alias")]
    public object Alias { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("method_type")]
    public string MethodType { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("live")]
    public bool Live { get; set; }

    [JsonProperty("use_cvc_with_moto")]
    public bool UseCvcWithMoto { get; set; }
}

public class Extras
{
    [JsonProperty("creation_extras")]
    public Dictionary<string, object> CreationExtras { get; set; }

    [JsonProperty("confirmation_extras")]
    public object ConfirmationExtras { get; set; }
}
