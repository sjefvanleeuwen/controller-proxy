# controller-proxy
referentie implementatie .net 4.x mvc controller proxy met bearer token -> web api .net core 3.0 preview

in appSettings.json

```json
{
  "AppSettings": {
    "Secret": "SYMETRIC KEY"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}

```

in core webapi valuescontroller

```csharp
[ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    
    ....

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "ClaimsType.Name", User.Identity.Name };
        }
```

in javascript

```javascript
    var request = new XMLHttpRequest();
// Open a new connection, using the GET request on the URL endpoint
    request.open('GET', 'api/values', true);

request.onload = function () {
  // Begin accessing JSON data here
    alert(request.response);
  }


// Send request
request.send()
```

in web.confg

```xml
  <appSettings>
    ...
    <add key="webapi:ProxyUrl" value="http://127.0.0.1:5000" />
    <add key="webapi:JwtSecret" value="SYMETRIC KEY" />
  </appSettings>

```

in mvc proxycontroller

```csharp

 [AcceptVerbs(Http.Get, Http.Head, Http.MkCol, Http.Post, Http.Put)]
        public async Task<HttpResponseMessage> Proxy()
        {
            var token = GenerateToken("s00000001:1234567890");
            this.Request.Headers.Add("Authorization", "Bearer " + token);

            using (HttpClient http = new HttpClient())
            {
                this.Request.RequestUri = new Uri(ConfigurationManager.AppSettings["webapi:ProxyUrl"] + this.Request.RequestUri.PathAndQuery);
                if (this.Request.Method == HttpMethod.Get)
                {
                    this.Request.Content = null;
                }
                try
                {
                    var s = await http.SendAsync(this.Request);
                    return s;
                }
                catch (Exception ex)
                {
                    // todo some stuff, as your client will get a hard 500 undescriptive error.
                }
                return null;
            }
        }

```
